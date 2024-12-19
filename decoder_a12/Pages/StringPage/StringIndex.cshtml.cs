using decoder_a12.Models;
using decoder_a12.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace decoder_a12.Pages.StringPage
{
    public class StringIndexModel : PageModel
    {
        private readonly MatrixCalculationsService _matrixCalculationsService;
        private readonly SendChannelService _sendChannelService;
        private readonly CodeDecoderService _codeDecoderService;
        [BindProperty]
        public Code code { get; set; }

        [BindProperty]
        public string GInput { get; set; } = string.Empty;
        [BindProperty]
        public string? MInput { get; set; } = string.Empty;
        [BindProperty]
        public string? MResult { get; set; } = "";
        public string? CResult { get; set; } = "";

        public StringIndexModel()
        {
            _matrixCalculationsService = new MatrixCalculationsService();
            _sendChannelService = new SendChannelService();
            _codeDecoderService = new CodeDecoderService();
            code = new Code();
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string action)
        {
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

                try
                {
                    StringCode strCodes = new StringCode(code.G, code.n, code.k, code.p, MInput);

                    //siuntimas be kodavimo
                    _sendChannelService.SendStringCodeThroughChannel(strCodes, false);
                    MResult = _codeDecoderService.DecodeStringCode(strCodes);

                    //siuntimas su kodavimu
                    _sendChannelService.SendStringCodeThroughChannel(strCodes, true);
                    CResult = _codeDecoderService.DecodeStringCode(strCodes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return Page();
        }
    }
}
