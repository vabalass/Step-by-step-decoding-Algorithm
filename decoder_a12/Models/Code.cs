using decoder_a12.Services;

namespace decoder_a12.Models;

public class Code
{
    public int[] m {  get; set; } // pradinis vektorius
    public int[] y { get; set; } // sugrįžęs vektorius

    public int[] decoded { get; set; } = { 0 }; // dekoduotas vektorius
    public int n { get; set; } // stulpeliai
    public int k { get; set; } // eilutės

    public double p { get; set; } // iškraipymo tikimybė

    public int[,] G { get; set; } // generuojanti matrica

    public int[]? C { get; set; } // užkoduotas vektorius

    public int[,]? H {  get; set; } // kontrolinė matrica 

    public Code()
    {
        MatrixCalculationsService matrixCalculationsService = new MatrixCalculationsService();
        k = 2;
        n = 5;
        G = matrixCalculationsService.GenerateLinearCodeMatrix(k, n);
        m = [1, 0];
        y = [1, 0];
        C = matrixCalculationsService.MultiplyMatrixByVector(G, m);
        p = 0;
    }
    public Code(int rows, int columns, double probability)
    {
        MatrixCalculationsService matrixCalculationsService = new MatrixCalculationsService();
        k = rows;
        n = columns;
        p = probability;
        G = new int[k, n];
        m = new int[k];
    }
    public void InitializeG(int[,] values)
    {
        if (values.GetLength(0) != k || values.GetLength(1) != n)
        {
            throw new ArgumentException("Klaida: matricos išmatavimai neatitinka!");
        }

        G = values;

        // Check if G is a linear code generator matrix
        if (!IsLinearCodeMatrix(G))
        {
            throw new ArgumentException("Matrix G is not a valid linear code generator matrix.");
        }
    }

    private bool IsLinearCodeMatrix(int[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        // Check if the rows are linearly independent
        for (int i = 0; i < rows; i++)
        {
            for (int j = i + 1; j < rows; j++)
            {
                if (AreRowsLinearlyDependent(matrix, i, j, cols))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool AreRowsLinearlyDependent(int[,] matrix, int row1, int row2, int cols)
    {
        for (int k = 0; k < cols; k++)
        {
            if (matrix[row1, k] != matrix[row2, k])
            {
                return false;
            }
        }

        return true;
    }

    public void PrintG()
    {
        for (int i = 0; i < k; i++)
        {
            for (int j = 0; j < n; j++)
            {
                Console.Write(G[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    public void PrintVector(int[] vector)
    {
        Console.WriteLine(string.Join(" ", vector));
    }

    public void GenerateH()
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

}

