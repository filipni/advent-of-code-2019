using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace AdventOfCode
{
    class Day3
    {
        public static void Part1And2()
        {
            string filepath = Path.Combine(Utils.INPUT_DIR, "day3.txt");
            string text = System.IO.File.ReadAllText(filepath);
            string[] inputs = text.Split("\n");

            var wire1Directions = ParseDay3Input(inputs[0]);
            var wire2Directions = ParseDay3Input(inputs[1]);

            var wire1Path = CreateWirePath(wire1Directions);
            var wire2Path = CreateWirePath(wire2Directions);

            var intersections = wire1Path.Where(x => wire2Path.Contains(x));

            int distanceClosestIntersect = intersections.Select(x => Math.Abs(x.Item1) + Math.Abs(x.Item2)).Min();
            Console.WriteLine("Distance to closest intersection: {0}", distanceClosestIntersect);

            int minSteps = intersections.Select(x => wire1Path.IndexOf(x) + wire2Path.IndexOf(x) + 2).Min(); // Add 2 to compensate for zero based indexing 
            Console.WriteLine("Fewest steps to intersection: {0}", minSteps);
        }

        static List<(char, int)> ParseDay3Input(string input)
        {
            var matches = Regex.Matches(input, @"([UDLR])(\d+)");
            var parsedInputs = new List<(char, int)>();

            foreach (Match m in matches)
            {
                char direction = char.Parse(m.Groups[1].Value);
                int steps = int.Parse(m.Groups[2].Value);
                parsedInputs.Add((direction, steps)); 
            }

            return parsedInputs;
        }

        static List<(int, int)> CreateWirePath(List<(char, int)> directions)
        {
            var currentPos = (X: 0, Y: 0);
            var path = new List<(int, int)>();

            foreach ((char direction, int steps) in directions)
            {
                for (int i = 1; i <= steps; i++)
                {
                    switch (direction)
                    {
                        case 'U':
                            currentPos.Y += 1;
                            break;
                        case 'D':
                            currentPos.Y -= 1;
                            break;
                        case 'L':
                            currentPos.X -= 1;
                            break;
                        case 'R':
                            currentPos.X += 1;
                            break;
                    }
                    path.Add(currentPos);
                }
            }

            return path;
        }
    }
}