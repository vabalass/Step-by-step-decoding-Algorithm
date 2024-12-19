namespace decoder_a12.Models;

public class ChannelResult
{
    public int[]? OriginalCode { get; set; }
    public int[]? DistortedCode { get; set; }
    public int[]? ErrorPositions { get; set; }
}
