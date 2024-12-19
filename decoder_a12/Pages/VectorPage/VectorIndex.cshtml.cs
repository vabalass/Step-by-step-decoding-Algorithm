using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using decoder_a12.Models;
using decoder_a12.Services;
using System.Reflection;

namespace decoder_a12.Pages.VectorPage;

public class VectorIndexModel : PageModel
{
    private SendChannelService _sendChannelService;
    private readonly CodeDecoderService _decoderService;
    private readonly MatrixCalculationsService _matrixCalculationsService;
    private readonly SyndromeService _syndromeService;
    SyndromeTable? sindTable;

    [BindProperty]
    public Code code { get; set; }

    [BindProperty]
    public string GInput { get; set; } = string.Empty;
    [BindProperty]
    public string? MInput {  get; set; } = string.Empty;
    [BindProperty]
    public string? YInput { get; set; } = string.Empty;

    [BindProperty]
    public int ErrorCount { get; set; }
    [BindProperty]
    public int[] ErrorsPositions { get; set; }

    public VectorIndexModel()
    {
        _decoderService = new CodeDecoderService();
        _matrixCalculationsService = new MatrixCalculationsService();
        _syndromeService = new SyndromeService();
        _sendChannelService = new SendChannelService();
        code = new Code();
    }

    public void OnGet()
    {
        ErrorCount = 0;
        ErrorsPositions = new int[] { 0, 0, 0, 0, 0 };
    }

    public IActionResult OnPost(string action)
    {
        Console.WriteLine(".........OnPost suveikė, action = " + action);

        if (!string.IsNullOrEmpty(MInput))
        {
            try
            {
                // Convert string to array of integers
                code.m = MInput
                    .Select(c => int.Parse(c.ToString())) // Convert each character to an integer
                    .ToArray();
            }
            catch (FormatException)
            {
                ModelState.AddModelError("MInput", "Invalid format. Please enter a binary string (e.g., 1101).");
                return Page();
            }
        }

        if (!ModelState.IsValid)
        {
            Console.WriteLine("ModelState ne valid...");
            foreach (var kvp in ModelState)
            {
                var fieldName = kvp.Key;
                var state = kvp.Value;
                foreach (var error in state.Errors)
                {
                    Console.WriteLine($"Klaida lauke {fieldName}: {error.ErrorMessage}");
                }
            }
            return Page();
        }

        if (action == "generate")
        {
            if (GInput[0] != '-')
            {
                Console.WriteLine("Rankinė matricos parsinimas");
                try
                {
                    var parsedG = _matrixCalculationsService.ParseMatrix(GInput, code.k, code.n);
                    code.InitializeG(parsedG);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return Page();
                }
            }
            else
            {
                code.G = _matrixCalculationsService.GenerateLinearCodeMatrix(code.k, code.n);
            }

            Console.WriteLine("Skaičiavimai");
            try
            {
                //vektoriaus c ir matricos H generavimas
                code.C = _matrixCalculationsService.MultiplyMatrixByVector(code.G, code.m);
                code.GenerateH();
                CodeStorage.SaveMatrix(_matrixCalculationsService.CloneMatrix(code.H));
                CodeStorage.SaveVector(_matrixCalculationsService.CloneVector(code.C));
                Console.Write("pradinis vektorius:");
                code.PrintVector(code.m);
                Console.Write("Siunčiamas:");
                code.PrintVector(code.C);
                Console.WriteLine("Iškraipymo tikimybė: " + code.p);

                //siuntimas Kanalu
                ChannelResult sendResult = _sendChannelService.Send(code.C, code.p);
                code.y = sendResult.DistortedCode;
                ErrorsPositions = sendResult.ErrorPositions;
                ErrorCount = ErrorsPositions.Length;
                Console.Write("Iš kanalo:");
                if (code.y != null) code.PrintVector(code.y);

                YInput = string.Join("", code.y ?? []);

                //dekodavimas
                sindTable = _syndromeService.GenerateSyndromeTable(code.H);
                code.decoded = _decoderService.Decode(code.y, code.H, code.k, sindTable);
                Console.Write("Dekoduotas:");
                code.PrintVector(code.decoded);
                //CodeStorage.Save(code, GInput);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        if (action == "decode")
        {
            Console.WriteLine("Dekodavimas...");

            code.C = CodeStorage.RetrieveVector();
            code.H = CodeStorage.RetrieveMatrix();
            _matrixCalculationsService.PrintMatrix(code.H);
            Console.WriteLine("Iškraipymo tikimybė: " + code.p);

            if (code.y != null)
            {
                try
                {
                    // Convert string to array of integers
                    code.y = YInput
                        .Select(y => int.Parse(y.ToString()))
                        .ToArray();
                    code.PrintVector (code.y);
                }
                catch (FormatException)
                {
                    ModelState.AddModelError("YInput", "Invalid format. Please enter a binary string (e.g., 1101).");
                    return Page();
                }
                sindTable = _syndromeService.GenerateSyndromeTable(code.H);
                code.decoded = _decoderService.Decode(code.y, code.H, code.k, sindTable);
            }
        }

        return Page();
    }
}
