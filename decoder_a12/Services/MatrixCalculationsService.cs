using System;

namespace decoder_a12.Services;

public class MatrixCalculationsService
{
    public int[] MultiplyMatrixByVector(int[,] matrix, int[] vector)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        if (vector.Length != rows)
        {
            Console.WriteLine("Klaida! Vektoriaus ilgis turi būti toks pats, koks matricos eilučių skaičius.");
            throw new ArgumentException("Klaida! Vektoriaus ilgis turi būti toks pats, koks matricos eilučių skaičius.");
        }

        int[] result = new int[cols]; // Resulting vector of size n (columns of the matrix)

        for (int j = 0; j < cols; j++) // Iterate through columns
        {
            result[j] = 0; // Initialize result[j] to 0
            for (int i = 0; i < rows; i++) // Iterate through rows
            {
                result[j] ^= matrix[i, j] * vector[i]; // XOR nes sumuojam 1 ir 0
            }
        }

        return result;
    }

    public int[] MultiplyVectorByMatrix(int[] vector, int[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        int[] result = new int[rows];

        for (int i = 0; i < rows; i++)
        {
            int sum = 0;
            for (int j = 0; j < cols; j++)
            {
                sum ^= vector[j] * matrix[i, j];
            }
            result[i] = sum;
        }

        return result;
    }

    public void PrintMatrix(int[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Console.Write(matrix[i, j] + " "); // Spausdiname elementą su tarpeliu
            }
            Console.WriteLine(); // Nauja eilutė po kiekvienos eilutės
        }
    }
}
