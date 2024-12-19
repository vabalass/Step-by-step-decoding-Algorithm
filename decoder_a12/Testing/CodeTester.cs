using decoder_a12.Services;

namespace decoder_a12.Models;

public class CodeTester
{
    public void Run()
    {
        Console.WriteLine("CODE TESTER................");
        MatrixCalculationsService matrixService = new MatrixCalculationsService();
        int[,] generatorMatrix2 = matrixService.GenerateLinearCodeMatrix(3, 7);
        Code code = new Code(3, 5, 0);

        string GInput = "1, 0, 0, 1, 1\r\n0, 1, 0, 1, 0\r\n0, 0, 1, 1, 0";
        code.InitializeG(matrixService.ParseMatrix(GInput, 3, 5));

        int[] inputVector = { 1, 0, 1 };
        Console.WriteLine("pradinis vektorius:");
        Console.WriteLine(string.Join(" ", inputVector));

        // Create a Code instance
        //code.G = generatorMatrix2;
        //code.PrintG();
        code.m = inputVector;
        code.GenerateH();

        // Multiply the matrix by the vector
        code.C = matrixService.MultiplyMatrixByVector(code.G, code.m);

        SyndromeService syndromeService = new SyndromeService();
        SyndromeTable sindTable = syndromeService.GenerateSyndromeTable(code.H);
        //Console.WriteLine("Syndromes table:");
        //sindTable.Print();


        // Display the result
        Console.WriteLine("Encoded vector (C):");
        Console.WriteLine(string.Join(" ", code.C));
        //Console.WriteLine("Matrica G: ");
        //matrixService.PrintMatrix(code.G);
        //Console.WriteLine("Matrica H: ");
        //matrixService.PrintMatrix(code.H);

        Console.WriteLine("Pažeistas vektorius: ");
        var channelService = new SendChannelService();
        var result = channelService.Send(code.C, code.p);
        code.y = result.DistortedCode;
        Console.WriteLine(string.Join(" ", code.y));

        Console.WriteLine("Dekodavimas:");
        var codeDecoderService = new CodeDecoderService();
        var decotedVector = codeDecoderService.Decode(code.y, code.H, code.k, sindTable);
        Console.WriteLine(string.Join(" ", decotedVector));


        Console.WriteLine("...........................");
    }
}
