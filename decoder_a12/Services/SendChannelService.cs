using decoder_a12.Models;
using System;
using System.Collections.Generic;

namespace decoder_a12.Services;

public class SendChannelService
{
    private readonly Random _random; // Atsitiktinių skaičių generatorius
    private double _p; // Klaidos tikimybė

    public SendChannelService ()
    {
        _random = new Random ();
        _p = 0;
    }

    public SendChannelService(double p)
    {
        if (p < 0 || p > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(p), "Klaidos tikimybė pe turi būti tarp 0 ir 1.");
        }

        _p = p;
        _random = new Random(); // Inicializuojamas vieną kartą paleidus programą
    }

    public ChannelResult Send(int[] code, double p)
    {
        _p = p;
        if (code == null || code.Length == 0)
        {
            throw new ArgumentException("Siunčiamas vektorius negali būti tuščias.", nameof(code));
        }

        int[] distortedCode = new int[code.Length];
        List<int> errorPositions = new List<int>();

        for (int i = 0; i < code.Length; i++)
        {
            bool isDistorted = false;
            distortedCode[i] = DistortSymbol(code[i], ref isDistorted);

            if (isDistorted)
            {
                errorPositions.Add(i); // Registruojame klaidos poziciją
            }
        }

        return new ChannelResult
        {
            OriginalCode = code,
            DistortedCode = distortedCode,
            ErrorPositions = errorPositions.ToArray()
        };
    }

    private int DistortSymbol(int symbol, ref bool isDistorted)
    {
        if (symbol != 0 && symbol != 1)
        {
            throw new ArgumentOutOfRangeException(nameof(symbol), "Dvejetainis simbolis turi būti 0 arba 1.");
        }

        double randomValue = _random.NextDouble();

        if (randomValue < _p)
        {
            isDistorted = true;
            return 1 - symbol; // Apverčiame 0 į 1 arba 1 į 0
        }

        isDistorted = false;
        return symbol;
    }

    public void SendStringCodeThroughChannel(StringCode stringCode, bool isEncoded)
    {
        if (stringCode == null)
        {
            throw new ArgumentNullException(nameof(stringCode), "StringCode object cannot be null.");
        }

        if (stringCode.codes == null || stringCode.codes.Length == 0)
        {
            throw new ArgumentException("StringCode object contains no codes to send.", nameof(stringCode));
        }

        Console.WriteLine($"Sending vectors {(isEncoded ? "encoded" : "original")} through the channel...");

        for (int i = 0; i < stringCode.codes.Length; i++)
        {
            var code = stringCode.codes[i];

            if (code == null)
            {
                Console.WriteLine($"Code {i}: Code object is null. Skipping.");
                continue;
            }

            int[] vectorToSend = isEncoded ? code.C : code.m;

            if (vectorToSend == null)
            {
                Console.WriteLine($"Code {i}: Vector {(isEncoded ? "C" : "m")} is null. Skipping.");
                continue;
            }

            // If sending the original vector (m), expand it to match the length of encoded vectors
            if (!isEncoded && vectorToSend.Length < stringCode.n)
            {
                int[] expandedVector = new int[stringCode.n];
                Array.Copy(vectorToSend, expandedVector, vectorToSend.Length);
                vectorToSend = expandedVector; // Use the expanded vector
            }

            // Send the chosen vector through the channel
            ChannelResult result = Send(vectorToSend, stringCode.p);

            // Assign the distorted vector to `y`
            code.y = result.DistortedCode;

            Console.WriteLine($"Code {i}:");
            Console.WriteLine($"  Original ({(isEncoded ? "C" : "m")}): [{string.Join("", vectorToSend)}]");
            Console.WriteLine($"  Distorted (y): [{string.Join("", code.y)}]");
            if (result.ErrorPositions.Length > 0)
            {
                Console.WriteLine($"  Error Positions: {string.Join(", ", result.ErrorPositions)}");
            }
            else
            {
                Console.WriteLine("  No errors introduced.");
            }
        }
    }

}
