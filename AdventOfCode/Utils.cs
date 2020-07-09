using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace AdventOfCode 
{
    public class Utils
    {
        public const string INPUT_DIR = @"input";

        public static long[] GetInput(string filename, string delimiter)
        {
            string path     = Path.Combine(INPUT_DIR, filename);
            string text     = File.ReadAllText(path);
            var input       = text.Split(delimiter).Where(x => double.TryParse(x, out _));
            var parsedInput = input.Select(x => long.Parse(x));

            return parsedInput.ToArray();
        }

        public static string[] GetTextInput(string filename, string delimiter)
        {
            string path = Path.Combine(INPUT_DIR, filename);
            string text = File.ReadAllText(path);
            return text.Split(delimiter);
        }

        public static string GetText(string filename)
        {
            string path = Path.Combine(INPUT_DIR, filename);
            return File.ReadAllText(path);
        }
    }
}