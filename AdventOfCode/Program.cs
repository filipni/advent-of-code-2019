using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Program
    {
        static long Day1Part1() => Utils.GetInput("day1.txt", "\n").Select(x => x / 3 - 2).Sum();

        static void Day1Part2()
        {
            var input = new Queue<long>(Utils.GetInput("day1.txt", "\n"));
            long sum = 0;

            while (input.Count != 0)
            {
                long module = input.Dequeue();
                long fuel = module / 3 - 2;
                if (fuel > 0)
                {
                    sum += fuel;
                    input.Enqueue(fuel);
                }
            }

            Console.WriteLine("Sum: {0}", sum);
        }

        static void Day2Part1()
        {
            long[] data = Utils.GetInput("day2.txt", ",");
            Console.WriteLine("Output: {0}", RunProgram(12, 2, data));
        }

        static void Day2Part2()
        {
            long[] data = Utils.GetInput("day2.txt", ",");

            var outputs = from x in Enumerable.Range(0, 100)
                          from y in Enumerable.Range(0, 100)
                          where RunProgram(x, y, (long[])data.Clone()) == 19690720
                          select 100 * x + y;

            Console.WriteLine("Output: {0}", outputs.FirstOrDefault());
        }

        static long RunProgram(int noun, int verb, long[] memory)
        {
            memory[1] = noun;
            memory[2] = verb;

            for (int i = 0; i < memory.Length; i += 4)
            {
                long opcode = memory[i];
                long input1Index = memory[i + 1];
                long input2Index = memory[i + 2];
                long outputIndex = memory[i + 3];

                switch (opcode)
                {
                    case 1:
                        memory[outputIndex] = memory[input1Index] + memory[input2Index];
                        break;
                    case 2:
                        memory[outputIndex] = memory[input1Index] * memory[input2Index];
                        break;
                    case 99:
                        return memory[0];
                    default:
                        Console.WriteLine("Unknown opcode: {0}", opcode);
                        return -1;
                }
            }

            Console.WriteLine("Program was never halted");
            return -1;
        }

        static void Day3()
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

        static void Day4()
        {
            Console.WriteLine("Number of different passwords");
            int numPasswords = GetNumberOfPasswords(x => x >= 2);
            Console.WriteLine("Part1: {0}", numPasswords);
            numPasswords = GetNumberOfPasswords(x => x == 2);
            Console.WriteLine("Part2: {0}", numPasswords);
        }

        static int GetNumberOfPasswords(Func<int, bool> streakFilter)
        {
            int numPasswords = 0;

            for (int password = 125730; password <= 579381; password++)
            {
                bool isIncreasing = true;
                var streaks = new List<int>();
                var currentStreak = 0;
                char previousDigit = ' ';

                foreach (char digit in password.ToString())
                {
                    if (digit == previousDigit)
                        currentStreak++;
                    else
                    {
                        streaks.Add(currentStreak);
                        currentStreak = 1;
                    }

                    if (digit < previousDigit)
                    {
                        isIncreasing = false;
                        break;
                    }

                    previousDigit = digit;
                }

                streaks.Add(currentStreak);
                bool hasDoubleDigit = streaks.Where(streakFilter).Any();

                if (hasDoubleDigit && isIncreasing)
                    numPasswords++;
            }

            return numPasswords;
        }

        static void Day5()
        {
            Console.WriteLine($"Part 1:");
            RunDiagnosticTest(1, "day5.txt");

            Console.WriteLine($"Part 2:");
            RunDiagnosticTest(5, "day5.txt");
        }

        static void RunDiagnosticTest(int code, string inputFilename)
        {
            long[] data = Utils.GetInput(inputFilename, ",");
            var inputQueue = new BlockingCollection<long> {code};
            var outputQueue = new BlockingCollection<long>();

            IntcodeComputer computer = new IntcodeComputer(data, inputQueue, outputQueue);
            computer.Run().Wait();

            var output = new List<long>();
            while (!outputQueue.IsCompleted)
                output.Add(outputQueue.Take());

            Console.WriteLine(string.Join(",", output));
        }

        static Dictionary<string, string> ParseDay6Input()
        {
            var orbits = new Dictionary<string, string>();

            string filepath = Path.Combine(Utils.INPUT_DIR, "day6.txt");
            var lines = System.IO.File.ReadAllLines(filepath);

            foreach (string s in lines)
            {
                string[] split = s.Split(")");
                orbits.Add(split[1], split[0]);
            }

            return orbits;
        }

        static List<string> getPath(string node, Dictionary<string, string> relationships)
        {
            var path = new List<string>();

            if (!relationships.ContainsKey(node))
                return path;

            string parent = relationships[node];
            path.Add(parent); // Add direct relative

            while (relationships.ContainsKey(parent))
            {
                path.Add(relationships[parent]); // Add indirect relative
                parent = relationships[parent];
            }

            return path;
        }

        static void Day6()
        {
            Dictionary<string, string> orbits = ParseDay6Input();
            int numOrbits = 0;

            foreach (string planet in orbits.Keys)
                numOrbits += getPath(planet, orbits).Count;

            Console.WriteLine("Total number of orbits: {0}", numOrbits);

            List<string> myOrbits = getPath("YOU", orbits);
            List<string> santasOrbits = getPath("SAN", orbits);

            int numUniqueOrbitsSanta = santasOrbits.Where(x => !myOrbits.Contains(x)).Count();
            int numUniqueOrbitsMe = myOrbits.Where(x => !santasOrbits.Contains(x)).Count();

            var minTransfers = numUniqueOrbitsMe + numUniqueOrbitsSanta; 
            Console.WriteLine("Minimum number of transfers: {0}", minTransfers);
        }

        static void Day7Part1() => Day7(Enumerable.Range(0, 5), false);
        static void Day7Part2() => Day7(Enumerable.Range(5, 5), true);

        private static void Day7(IEnumerable<int> phaseCodes, bool feedbackOn)
        {
            List<List<int>> sequenceSettings = Permutations(phaseCodes); 
            long largestThrusterValue = -1;
            var finalSettingSequence = "";

            foreach (List<int> settings in sequenceSettings)
            {
                List<long> settingsConverted = settings.ConvertAll(x => (long)x);
                long[] program = Utils.GetInput("day7.txt", ",");

                var amplifier = new AmplifierSequence(settingsConverted, program, feedbackOn);
                long thrusterValue = amplifier.Run(0);

                if (thrusterValue > largestThrusterValue)
                {
                    largestThrusterValue = thrusterValue;
                    finalSettingSequence = string.Join(",", settings);
                }
            }

            Console.WriteLine($"Max thruster signal: {largestThrusterValue} (setting sequence: {finalSettingSequence})");
        }

        public static List<List<T>> Permutations<T>(IEnumerable<T> collection)
        {
            var permutations = new List<List<T>>();
            var set = new HashSet<T>(collection);
            _Permutations(set, new List<T>(), permutations); 
            return permutations;
        }

        private static void _Permutations<T>(HashSet<T> set, List<T> candidate, List<List<T>> permutations)
        {
            if (candidate.Count == set.Count)
            {
                permutations.Add(candidate);
                return;
            }

            foreach(var item in set)
            {
                if (!candidate.Contains(item))
                {
                    var updatedCandidate = new List<T>(candidate);
                    updatedCandidate.Add(item);
                    _Permutations(set, updatedCandidate, permutations);
                }
            }
        }

        public static void Day8Part1()
        {
            string imageData = Utils.GetText("day8.txt"); 
            List<char[,]> imageLayers = GetImageLayers(imageData, 25, 6);

            var counts = new List<Dictionary<char,int>>();
            var minLayer = -1;
            var min = int.MaxValue;  

            for (int i = 0; i < imageLayers.Count; i++)
            {
                var digitCount = new Dictionary<char, int>();
                char[,] layer = imageLayers[i];

                foreach (char c in layer)
                {
                    if (digitCount.ContainsKey(c))
                        digitCount[c]++;
                    else
                        digitCount.Add(c, 1);
                }

                if (digitCount.ContainsKey('0') && digitCount['0'] <= min)
                {
                    minLayer = i;
                    min = digitCount['0'];
                }

                counts.Add(digitCount);
            }

            Dictionary<char,int> minLayerDigitCount = counts[minLayer];
            minLayerDigitCount.TryGetValue('1', out int count1);
            minLayerDigitCount.TryGetValue('2', out int count2);

            var biosPassword = count1 * count2;
            Console.WriteLine($"BIOS password: {biosPassword}");
        }

        public static void Day8Part2()
        {
            const int imageWidth = 25;
            const int imageHeight = 6;

            string imageData = Utils.GetText("day8.txt"); 
            List<char[,]> imageLayers = GetImageLayers(imageData, imageWidth, imageHeight);

            var image = new List<char[]>();

            // Init image with transparent pixels
            for(int i = 0; i < imageHeight; i++)
                image.Add(new String('2', imageWidth).ToArray());

            imageLayers.ForEach(layer => SuperImposeLayer(image, layer));
            
            image.ForEach(row => Console.WriteLine(row));
        }

        private static List<char[,]> GetImageLayers(string imageData, int layerWidth, int layerHeight)
        {
            int layerSize = layerWidth * layerHeight;
            int numLayers = imageData.Length / layerSize;

            List<char[,]> imageLayers = new List<char[,]>();

            for (int i = 0; i < numLayers; i++)
            {
                char[,] layer = new char[layerHeight, layerWidth]; 
                for (int j = 0; j < layerHeight; j++)
                {
                    for (int k = 0; k < layerWidth; k++)
                        layer[j, k] = imageData[i * layerSize + j * layerWidth + k];
                }
                imageLayers.Add(layer);
            }

            return imageLayers;
        }

        static private void SuperImposeLayer(List<char[]> image, char[,] layer)
        {
            for(int i = 0; i < layer.GetLength(0); i++)
                for (int j = 0; j < layer.GetLength(1); j++)
                {
                    char layerPixel = layer[i, j];
                    if (image[i][j] == '2' && layerPixel != '2')
                        image[i][j] = layerPixel == '0' ? '█' : ' ';
                }
        }

        static private void Day9()
        {
            Console.WriteLine($"Part 1:");
            RunDiagnosticTest(1, "day9.txt");

            Console.WriteLine($"Part 2:");
            RunDiagnosticTest(2, "day9.txt");
        }

        static void Main() => Day9();
    }
}
