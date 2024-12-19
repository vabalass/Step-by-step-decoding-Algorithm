using decoder_a12.Models;
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

    public int[,] GenerateMatrix(int rows, int cols)
    {
        var matrix = new int[rows, cols];
        var random = new Random();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = random.Next(0, 2);
            }
        }
        return matrix;
    }

    public int[,] GenerateLinearCodeMatrix(int rows, int cols)
    {
        if (cols <= rows)
        {
            throw new ArgumentException("Number of columns must be greater than rows for a valid generator matrix.");
        }

        int[,] matrix = new int[rows, cols];

        // Step 1: Fill the left part with an identity matrix
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                matrix[i, j] = (i == j) ? 1 : 0; // Identity matrix
            }
        }

        // Step 2: Fill the right part with random binary values
        Random random = new Random();
        for (int i = 0; i < rows; i++)
        {
            for (int j = rows; j < cols; j++)
            {
                matrix[i, j] = random.Next(0, 2); // Random 0 or 1
            }
        }
        return matrix;
    }

    public int[,] ParseMatrix(string input, int expectedRows, int expectedCols)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Input cannot be null or empty.");
        }

        // Split the input into lines
        var lines = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length == 0)
        {
            throw new ArgumentException("Input must contain at least one row.");
        }

        if (lines.Length != expectedRows)
        {
            throw new ArgumentException($"Number of rows ({lines.Length}) does not match the expected value ({expectedRows}).");
        }

        // Parse the first line to determine the number of columns
        var firstRow = lines[0].Split(new[] { ',' }, StringSplitOptions.TrimEntries);
        int cols = firstRow.Length;

        if (cols == 0)
        {
            throw new ArgumentException("Each row must contain at least one column.");
        }

        if (cols != expectedCols)
        {
            throw new ArgumentException($"Number of columns ({cols}) does not match the expected value ({expectedCols}).");
        }

        // Create the matrix
        int[,] matrix = new int[lines.Length, cols];

        for (int i = 0; i < lines.Length; i++)
        {
            var row = lines[i].Split(new[] { ',' }, StringSplitOptions.TrimEntries);

            if (row.Length != cols)
            {
                throw new ArgumentException("All rows must have the same number of columns.");
            }

            for (int j = 0; j < row.Length; j++)
            {
                if (!int.TryParse(row[j], out int value))
                {
                    throw new ArgumentException($"Invalid integer value at row {i + 1}, column {j + 1}: '{row[j]}'.");
                }

                matrix[i, j] = value;
            }
        }

        return matrix;
    }

    public int[,] CloneMatrix(int[,] originalMatrix)
    {
        if (originalMatrix == null)
        {
            return null;
        }

        int rows = originalMatrix.GetLength(0);
        int cols = originalMatrix.GetLength(1);
        int[,] newMatrix = new int[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                newMatrix[i, j] = originalMatrix[i, j];
            }
        }

        return newMatrix;
    }

    public int[] CloneVector(int[] originalVector)
    {
        if (originalVector == null)
        {
            return null;
        }

        // Create a new array and copy the elements from the original vector
        int[] clonedVector = new int[originalVector.Length];
        Array.Copy(originalVector, clonedVector, originalVector.Length);

        return clonedVector;
    }

}
