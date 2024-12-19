using decoder_a12.Models;
using decoder_a12.Services;
using decoder_a12.Services.StringServices;

namespace decoder_a12.Testing;

public class StringCodeTester
{
    public void RunSimpleTest()
    {
        Console.WriteLine("String Code tester............");
        MatrixCalculationsService _matrixService = new MatrixCalculationsService();
        StringCalculationsService _stringCalService = new StringCalculationsService();

        /////////////vartojo imputas
        int[,] generatorMatrix = _matrixService.GenerateLinearCodeMatrix(3, 7);
        string m = "Naktis";

        ///////////konvertavimai
        var binaryString = _stringCalService.StringToBinary(m);
        Console.WriteLine("Converted to binary: " + binaryString);

        var stringFromBinary = _stringCalService.BinaryToString(binaryString);
        Console.WriteLine("Converted back to ASCII: " + stringFromBinary);

        var binaryMatrix = _stringCalService.BinaryStringToIntMatrix(binaryString, 4);
        _matrixService.PrintMatrix(binaryMatrix);

        //StringCode strCode(generatorMatrix)
        int Mlenght = m.Length;
        int n = 5;
        Console.WriteLine("Starting string: " + m);


        var stringFromBinaryMatrix = _stringCalService.IntMatrixToBinaryString(binaryMatrix, binaryString.Length);
        Console.WriteLine("Converted from Int array to binary: " + stringFromBinaryMatrix);

        var backtobackString = _stringCalService.BinaryToString(stringFromBinaryMatrix);
        Console.WriteLine("Back to back string: " +  backtobackString);

        ///////////////////////////////////////////////////////


        Console.WriteLine("............................");
    }

    public void RunFullTest()
    {
        Console.WriteLine("Full String Code tester............");
        MatrixCalculationsService _matrixService = new MatrixCalculationsService();
        StringCalculationsService _stringCalService = new StringCalculationsService();
        CodeDecoderService _codeDecoderService = new CodeDecoderService();
        SendChannelService _sendChannelService = new SendChannelService();

        /////////////vartojo imputas
        int k = 3;
        int n = 7;
        double p = 0.001;
        int[,] generatorMatrix = _matrixService.GenerateLinearCodeMatrix(k, n);
        string m = "Naktis";

        ///////////String Code sukurimas
        StringCode strCodes = new StringCode(generatorMatrix, n, k, p, m);
        strCodes.PrintCodesMValues();
        strCodes.PrintCodesCMatrixes();

        /////////siuntimas kanalu
        _sendChannelService.SendStringCodeThroughChannel(strCodes, false);


        ////dekodavimas
        var result = _codeDecoderService.DecodeStringCode(strCodes);
        Console.WriteLine("Tikiuos veikia: " + result);

        Console.WriteLine("............................");
    }
}
