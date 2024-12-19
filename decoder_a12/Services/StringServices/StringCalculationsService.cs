using System.Text;

namespace decoder_a12.Services.StringServices;

public class StringCalculationsService
{
    public string StringToBinary(string input)
    {
        StringBuilder binaryString = new StringBuilder();
        foreach (char c in input)
        {
            string binaryChar = Convert.ToString(c, 2).PadLeft(8, '0'); // Ensure 8-bit representation
            binaryString.Append(binaryChar);
        }
        return binaryString.ToString(); // No spaces between binary digits
    }

    // Converts a continuous binary string back to a regular string (e.g., "0110100001100101011011000110110001101111" -> "hello")
    public string BinaryToString(string binaryInput)
    {
        StringBuilder output = new StringBuilder();
        for (int i = 0; i < binaryInput.Length; i += 8)
        {
            string binaryChar = binaryInput.Substring(i, 8); // Extract 8 bits
            int charCode = Convert.ToInt32(binaryChar, 2); // Convert binary to decimal
            output.Append((char)charCode); // Convert decimal to character
        }
        return output.ToString();
    }

    public int[,] BinaryStringToIntMatrix(string binaryString, int k)
    {
        // Calculate the number of vectors (rows) based on the length of the binary string and `k`
        int numberOfVectors = (int)Math.Ceiling((double)binaryString.Length / k);

        // Initialize the matrix to hold the binary vectors
        int[,] binaryMatrix = new int[numberOfVectors, k];

        // Convert the binary string into the matrix
        for (int i = 0; i < numberOfVectors; i++)
        {
            for (int j = 0; j < k; j++)
            {
                // Calculate the position in the binary string
                int index = i * k + j;

                if (index < binaryString.Length)
                {
                    // Convert the character at `index` in the binary string to an integer
                    binaryMatrix[i, j] = binaryString[index] - '0';
                }
                else
                {
                    // Fill with 0 if there are no more characters left in the binary string
                    binaryMatrix[i, j] = 0;
                }
            }
        }

        return binaryMatrix;
    }

    public string IntMatrixToBinaryString(int[,] binaryMatrix, int length)
    {
        // Initialize a StringBuilder to build the binary string
        StringBuilder binaryString = new StringBuilder();

        // Get the number of rows and columns in the matrix
        int rows = binaryMatrix.GetLength(0);
        int columns = binaryMatrix.GetLength(1);

        // Loop through each element in the matrix
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Convert each integer to its corresponding binary character
                binaryString.Append(binaryMatrix[i, j]);
            }
        }

        // Ensure the resulting string is exactly the specified length
        if (binaryString.Length > length)
        {
            // Trim the string to the original specified length
            binaryString.Length = length;
        }

        return binaryString.ToString();
    }
}
