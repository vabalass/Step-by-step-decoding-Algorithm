namespace decoder_a12.Models;

public class Code
{
    public int[] m {  get; set; } // pradinis vektorius
    public int n { get; set; } // stulpeliai
    public int k { get; set; } // eilutės

    public double p { get; set; } // iškraipymo tikimybė

    public int[,] G { get; set; } // generuojanti matrica

    public int[]? C { get; set; } // užkoduotas vektorius

    public int[,]? H {  get; set; } // kontrolinė matrica 

    public Code(int rows, int columns)
    {
        k = rows;
        n = columns;
        G = new int[k, n];
        m = new int[k];
    }
    public void InitializeG(int[,] values)
    {
        if (values.GetLength(0) != k || values.GetLength(1) != n)
        {
            throw new ArgumentException("Klaida: matricos išmatavimai neatitinka!");
        }

        for (int i = 0; i < k; i++)
        {
            for (int j = 0; j < n; j++)
            {
                G[i, j] = values[i, j];
            }
        }
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

    public void GenerateH()
    {
        int pRows = k; // G eilutės
        int pCols = n - k; // G papildomi stulpeliai

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

        // Sukuriame vienetinę matricą I_{n-k}
        for (int i = 0; i < pCols; i++)
        {
            for (int j = 0; j < pCols; j++)
            {
                identity[i, j] = (i == j) ? 1 : 0;
            }
        }

        // Sukuriame H = [P^T | I_{n-k}]
        H = new int[pCols, n];
        for (int i = 0; i < pCols; i++)
        {
            for (int j = 0; j < pRows; j++)
            {
                H[i, j] = P[j, i]; // P transponuota
            }
            for (int j = 0; j < pCols; j++)
            {
                H[i, pRows + j] = identity[i, j]; // Identity dalis
            }
        }
    }

}

