using decoder_a12.Models;

namespace decoder_a12.Services;

public class SyndromeService
{
    private readonly MatrixCalculationsService _matrixService;

    public SyndromeService()
    {
        _matrixService = new MatrixCalculationsService();
    }

    /// <summary>
    /// Generuoja sindromų lentelę pagal kontrolinę matricą H.
    /// </summary>
    /// <param name="H">Kontrolinė matrica.</param>
    /// <returns>Sindromų lentelė.</returns>
    public SyndromeTable GenerateSyndromeTable(int[,] H)
    {
        int n = H.GetLength(1); // Kodo ilgis
        var syndromeTable = new SyndromeTable();

        for (int i = 0; i < (1 << n); i++)
        {
            // Sukuriame klaidos vektorių
            int[] errorVector = new int[n];
            for (int j = 0; j < n; j++)
            {
                errorVector[j] = (i & (1 << j)) != 0 ? 1 : 0;
            }

            // Apskaičiuojame sindromą
            int[] syndrome = _matrixService.MultiplyVectorByMatrix(errorVector, H);

            // Apskaičiuojame klaidos vektoriaus svorį
            int weight = CalculateWeight(errorVector);

            // Paverčiame sindromą į string (raktui)
            string syndromeKey = string.Join("", syndrome);

            // Pridedame įrašą į lentelę
            if (syndromeTable.FindErrorVector(syndromeKey) == null)
            {
                syndromeTable.AddEntry(syndromeKey, errorVector, weight);
            }
        }

        return syndromeTable;
    }

    public int CalculateWeight(int[] vector)
    {
        int weight = 0;

        foreach (int bit in vector)
        {
            if (bit == 1)
            {
                weight++;
            }
        }

        return weight;
    }
}
