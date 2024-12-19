using System;
using System.Numerics;

namespace decoder_a12.Models
{
    public static class CodeStorage
    {
        private static Code _code;
        private static string _word;
        private static int[,] _matrix;
        private static int[] _vector;

        public static void Save(Code codeModel, string gInput)
        {
            if (codeModel == null)
            {
                throw new ArgumentNullException(nameof(codeModel), "The CodeModel cannot be null.");
            }

            _code = codeModel;
            _word = gInput;
        }

        public static void SaveMatrix(int[,] matrix)
        {
            _matrix = matrix;
        }

        public static int[,] RetrieveMatrix()
        {
            return _matrix;
        }
        public static void SaveVector(int[] vector)
        {
            _vector = vector;
        }

        public static int[] RetrieveVector()
        {
            return _vector;
        }

        public static Code RetrieveCode()
        {
            return _code;
        }
        public static void SaveWord(string word)
        {
            _word = word;
        }
        public static string RetrieveWord()
        {
            return _word;
        }
    }
}
