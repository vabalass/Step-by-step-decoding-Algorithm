namespace decoder_a12.Models;

public class SyndromeEntry
{
    public string? SyndromeKey { get; set; } // Sindromo reikšmė (pvz., "101")
    public int[]? ErrorVector { get; set; } // Klaidos vektorius (pvz., [0, 0, 1, 0, 0, 0, 1])
    public int Weight { get; set; }

    public SyndromeEntry(string syndromeKey, int[] errorVector, int weight)
    {
        SyndromeKey = syndromeKey;
        ErrorVector = errorVector;
        Weight = weight;
    }
}
