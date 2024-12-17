using decoder_a12.Services;

namespace decoder_a12.Models;

public class CodeTester
{
    public void Run()
    {
        int[,] generatorMatrix = {
            { 1, 0, 0, 1, 1, 0, 1 },
            { 0, 1, 0, 1, 0, 1, 1 },
            { 0, 0, 1, 0, 1, 1, 1 }
        };

        int[] inputVector = { 1, 0, 1 };
        Console.WriteLine("pradinis vektorius:");
        Console.WriteLine(string.Join(" ", inputVector));

        // Create a Code instance
        Code code = new Code(3, 7);
        code.G = generatorMatrix;
        code.m = inputVector;
        code.GenerateH();

        // Multiply the matrix by the vector
        MatrixCalculationsService matrixService = new MatrixCalculationsService();
        code.C = matrixService.MultiplyMatrixByVector(code.G, code.m);

        SyndromeService syndromeService = new SyndromeService();
        SyndromeTable sindTable = syndromeService.GenerateSyndromeTable(code.H);
        Console.WriteLine("Syndromes table:");
        sindTable.Print();


        // Display the result
        Console.WriteLine("Encoded vector (C):");
        Console.WriteLine(string.Join(" ", code.C));
        //Console.WriteLine("Matrica G: ");
        //matrixService.PrintMatrix(code.G);
        //Console.WriteLine("Matrica H: ");
        //matrixService.PrintMatrix(code.H);

        Console.WriteLine();
        Console.WriteLine("Pažeistas vektorius: ");
        code.C[6] = 0;
        Console.WriteLine(string.Join(" ", code.C));

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("Dekodavimas:");
        var codeDecoderService = new CodeDecoderService();
        var decotedVector = codeDecoderService.Decode(code.C, code.H, code.k, sindTable); //padaryti, kad code turetu kintamaji y ir siusti code
        Console.WriteLine(string.Join(" ", decotedVector));
    }
}
