using System;
using System.IO;
using System.Linq;

namespace AdventOfCode 
{
    public class Utils
    {
        public const string INPUT_DIR = @"input";

        public static int GetDigits(int num, int start, int length)
        {
            start = (int)Math.Pow(10, start);
            length = (int)Math.Pow(10, length);
            return num / start % length;
        }

        public static int[] GetInput(string filename, string delimiter)
        {
            string path     = Path.Combine(INPUT_DIR, filename);
            string text     = File.ReadAllText(path);
            var input       = text.Split(delimiter).Where(x => double.TryParse(x, out _));
            var parsedInput = input.Select(x => int.Parse(x));

            return parsedInput.ToArray();
        }

    }
}