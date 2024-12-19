using decoder_a12.Services;
using decoder_a12.Services.StringServices;

namespace decoder_a12.Models;

public class StringCode
{
    private StringCalculationsService _stringCalService;
    private SyndromeService _syndromeService;
    private MatrixCalculationsService _matrixCalculationsService;
    public string originalString {  get; set; }
    public string binaryString {  get; set; }
    public int n;
    public int k;
    public double p;
    public int[,] G;
    public int[,]? H;
    public SyndromeTable syndromeTable { get; set; }
    public int LenghtBinary;

    public Code[] codes { get; set; }
    public int codesCount;
    public StringCode(int[,] gMatrix, int n, int k, double p, string originalString)
    {
        _stringCalService = new StringCalculationsService();
        _syndromeService = new SyndromeService();
        _matrixCalculationsService = new MatrixCalculationsService();
        this.n = n;
        this.k = k;
        this.p = p;
        G = gMatrix;
        GenerateH();
        if (H != null) syndromeTable = _syndromeService.GenerateSyndromeTable(H);
            else throw new Exception("H matrica yra null!");

        this.originalString = originalString;
        binaryString = _stringCalService.StringToBinary(originalString);
        LenghtBinary = binaryString.Length;

        //binary matrix radimas
        int [,] binaryMatrix = _stringCalService.BinaryStringToIntMatrix(binaryString, k);
        int numberOfRows = binaryMatrix.GetLength(0);

        // Initialize codes array
        codes = new Code[numberOfRows];
        codesCount = numberOfRows;

        for (int rowIndex = 0; rowIndex < numberOfRows; rowIndex++)
        {
            codes[rowIndex] = new Code(k, n, p);

            // Assign the row from binaryMatrix to codes[rowIndex].m
            codes[rowIndex].m = new int[k];
            for (int colIndex = 0; colIndex < k; colIndex++)
            {
                codes[rowIndex].m[colIndex] = binaryMatrix[rowIndex, colIndex];
            }

            codes[rowIndex].InitializeG(gMatrix);
            codes[rowIndex].C = _matrixCalculationsService.MultiplyMatrixByVector(G, codes[rowIndex].m);
            codes[rowIndex].y = codes[rowIndex].C;
        }
    }

    public StringCode()
    {
        _stringCalService = new StringCalculationsService();
        originalString = "Labas pasauli!";
        G = new int[k, n];
        binaryString = _stringCalService.StringToBinary(originalString);
        LenghtBinary = binaryString.Length;
        n = LenghtBinary / 3;

        //for(int i = 0;  i < n; i++) 
    }

    private void GenerateH()
    {
        int pRows = k; // G eilutės
        int pCols = n - k; // G papildomi stulpeliai
        Console.WriteLine($"pRows = {pRows}, pCols = {pCols}");

        // Patikriname, ar G turi pakankamai stulpelių (n > k)
        if (pCols <= 0)
            throw new InvalidOperationException("G matrix must have at least (n-k) extra columns.");

        // Sukuriame vietą P ir vienetinės matricos (identity) dydį
        int[,] P = new int[pRows, pCols];
        int[,] identity = new int[pCols, pCols];

        // Ištraukiame P iš G
        for (int i = 0; i < pRows; i++)
        {
            for (int j = 0; j < pCols; j++)
            {
                if (k + j >= n)
                    throw new IndexOutOfRangeException("Invalid index while extracting P from G.");

                P[i, j] = G[i, k + j];
            }
        }

        // Sukuriame vienetinę matricą I{n-k}
        for (int i = 0; i < pCols; i++)
        {
            for (int j = 0; j < pCols; j++)
            {
                identity[i, j] = (i == j) ? 1 : 0;
            }
        }

        // Sukuriame H = [P^T | I{n-k}]
        H = new int[pCols, n];
        for (int i = 0; i < pCols; i++)
        {
            for (int j = 0; j < pRows; j++)
            {
                H[i, j] = P[j, i]; // P transponuota
            }
            for (int j = 0; j < pCols; j++)
            {
                H[i, pRows + j] = identity[i, j]; // I dalis
            }
        }
    }
    public void PrintCodesMValues()
    {
        Console.WriteLine("Kodų m vektoriai: ");
        for (int codeIndex = 0; codeIndex < codesCount; codeIndex++)
        {
            if (codes[codeIndex]?.m != null)
            {
                Console.Write(codeIndex + ". ");
                Console.WriteLine(string.Join("", codes[codeIndex].m));
            }
            else
            {
                Console.WriteLine("null");
            }
        }
    }

    public void PrintCodesCMatrixes()
    {
        Console.WriteLine("Kodų C matricos: ");
        for (int codeIndex = 0; codeIndex < codesCount; codeIndex++)
        {
            if (codes[codeIndex]?.m != null)
            {
                Console.Write(codeIndex + ". ");
                Console.WriteLine(string.Join("", codes[codeIndex].C));
            }
            else
            {
                Console.WriteLine("null");
            }
        }
    }
}
