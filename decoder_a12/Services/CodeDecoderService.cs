using decoder_a12.Models;

namespace decoder_a12.Services;

public class CodeDecoderService
{
    private MatrixCalculationsService _matrixCalculationsService;
    private SyndromeService _syndromeService;

    public CodeDecoderService()
    {
        _matrixCalculationsService = new MatrixCalculationsService();
        _syndromeService = new SyndromeService();
    }

    public int[] Decode(int[] y, int[,] H, int k, SyndromeTable syndromeTable)
    {
        // Make a copy of the original vector to avoid modifying the original `y`
        int[] decodedVector = (int[])y.Clone();

        // Suskaičiuojame pradžioje sindromą
        int[] syndrome = _matrixCalculationsService.MultiplyVectorByMatrix(decodedVector, H);
        int syndromeWeight = _syndromeService.CalculateWeight(syndrome);

        if (syndromeWeight == 0)
        {
            // Jei nėra klaidų, grąžiname pirmąsias k koordinates
            return decodedVector.Take(k).ToArray();
        }

        // Iteruojame per visas koordinates
        for (int i = 0; i < decodedVector.Length; i++)
        {
            // Apverčiame i-tą bitą
            decodedVector[i] ^= 1;

            // Perskaičiuojame sindromą
            int[] newSyndrome = _matrixCalculationsService.MultiplyVectorByMatrix(decodedVector, H);
            int newSyndromeWeight = _syndromeService.CalculateWeight(newSyndrome);

            if (newSyndromeWeight < syndromeWeight)
            {
                // Jei svoris sumažėjo, naujasis sindromas ir vektorius tampa dabartiniais
                syndrome = newSyndrome;
                syndromeWeight = newSyndromeWeight;

                if (syndromeWeight == 0)
                {
                    // Jei sindromo svoris pasiekia 0, grąžiname dekoduotą vektorių
                    return decodedVector.Take(k).ToArray();
                }
            }
            else
            {
                // Jei svoris nepasikeitė, grąžiname bitą į pradinę būseną
                decodedVector[i] ^= 1;
            }
        }

        // Jei ciklas baigiasi ir sindromo svoris nėra 0, dekodavimas nepavyko
        Console.WriteLine("Dekodavimas nepavyko!");
        return null;
    }

}
