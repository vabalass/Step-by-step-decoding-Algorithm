namespace decoder_a12.Models;

public class SyndromeTable
{
    public List<SyndromeEntry> Entries { get; private set; } = new List<SyndromeEntry>();

    public void AddEntry(string syndromeKey, int[] errorVector, int weight)
    {
        Entries.Add(new SyndromeEntry(syndromeKey, errorVector, weight));
    }

    public int[]? FindErrorVector(string syndrome)
    {
        return Entries.FirstOrDefault(e => e.SyndromeKey == syndrome)?.ErrorVector;
    }

    public void Print()
    {
        Console.WriteLine("Sindromų lentelė:");
        foreach (var entry in Entries)
        {
            Console.WriteLine($"Sindromas: {entry.SyndromeKey}, Klaidos vektorius: {string.Join("", entry.ErrorVector)}, Svoris: {entry.Weight}");
        }
    }
}
