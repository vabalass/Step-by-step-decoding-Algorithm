using decoder_a12.Models;
using decoder_a12.Services.StringServices;
using System.Text;

namespace decoder_a12.Services;

public class CodeDecoderService
{
    private MatrixCalculationsService _matrixCalculationsService;
    private SyndromeService _syndromeService;
    private StringCalculationsService _stringCalcService;

    public CodeDecoderService()
    {
        _matrixCalculationsService = new MatrixCalculationsService();
        _syndromeService = new SyndromeService();
        _stringCalcService = new StringCalculationsService();
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


    public string DecodeStringCode(StringCode stringCode)
    {
        if (stringCode.H == null)
        {
            throw new Exception("H matrix is not initialized in the StringCode object.");
        }

        if (stringCode.syndromeTable == null)
        {
            throw new Exception("Syndrome Table is not initialized in the StringCode object.");
        }

        //Console.WriteLine("Decoding each code in the StringCode object...");

        StringBuilder decodedBinaryString = new StringBuilder();

        for (int i = 0; i < stringCode.codes.Length; i++)
        {
            var code = stringCode.codes[i];

            if (code == null)
            {
                //Console.WriteLine($"Code {i}: Code object is null. Skipping.");
                continue;
            }

            if (code.y == null)
            {
                //Console.WriteLine($"Code {i}: No received vector (y). Skipping.");
                continue;
            }

            // Decode the received vector
            int[] decodedValue = Decode(code.y, stringCode.H, stringCode.k, stringCode.syndromeTable);

            if (decodedValue == null)
            {
                //Console.WriteLine($"Code {i}: Decoding failed.");
                continue;
            }

            code.decoded = decodedValue;

            foreach (int bit in decodedValue)
            {
                decodedBinaryString.Append(bit);
            }

            //Console.WriteLine($"Code {i}: Decoded Value (binary) = [{string.Join("", decodedValue)}]");
        }

        if (decodedBinaryString.Length == 0)
        {
            Console.WriteLine("No codes were successfully decoded.");
            return "";
        }

        string decodedString = _stringCalcService.BinaryToString(decodedBinaryString.ToString());
        return decodedString;
    }

}
