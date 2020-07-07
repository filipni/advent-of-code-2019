using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Program
    {
        const string INPUT_DIR = @"input";

        static int[] GetInput(string filename, string delimiter)
        {
            string path     = Path.Combine(INPUT_DIR, filename);
            string text     = File.ReadAllText(path);
            var input       = text.Split(delimiter).Where(x => double.TryParse(x, out _));
            var parsedInput = input.Select(x => int.Parse(x));

            return parsedInput.ToArray();
        }
        
        static int Day1Part1() => GetInput("day1.txt", "\n").Select(x => x / 3 - 2).Sum();

        static void Day1Part2()
        {
            var input = new Queue<int>(GetInput("day1.txt", "\n"));
            int sum = 0;

            while (input.Count != 0)
            {
                int module = input.Dequeue();
                int fuel = module / 3 - 2;
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
            int[] data = GetInput("day2.txt", ",");
            Console.WriteLine("Output: {0}", RunProgram(12, 2, data));
        }

        static void Day2Part2()
        {
            int[] data = GetInput("day2.txt", ",");

            var outputs = from x in Enumerable.Range(0, 100)
                          from y in Enumerable.Range(0, 100)
                          where RunProgram(x, y, (int[])data.Clone()) == 19690720
                          select 100 * x + y;

            Console.WriteLine("Output: {0}", outputs.FirstOrDefault());
        }

        static int RunProgram(int noun, int verb, int[] memory)
        {
            memory[1] = noun;
            memory[2] = verb;

            for (int i = 0; i < memory.Length; i += 4)
            {
                int opcode = memory[i];
                int input1Index = memory[i + 1];
                int input2Index = memory[i + 2];
                int outputIndex = memory[i + 3];

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
            string filepath = Path.Combine(INPUT_DIR, "day3.txt");
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
            Console.WriteLine($"Part 1: {RunDiagnosticTest(1)}");
            Console.WriteLine($"Part 2: {RunDiagnosticTest(5)}");
        }

        static int RunDiagnosticTest(int code)
        {
            int[] data = GetInput("day5.txt", ",");
            var input = new List<int> {code};

            IntcodeComputer computer = new IntcodeComputer(data, input);
            var output = computer.Run();

            return output[output.Count - 1];
        }

        static Dictionary<string, string> ParseDay6Input()
        {
            var orbits = new Dictionary<string, string>();

            string filepath = Path.Combine(INPUT_DIR, "day6.txt");
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

        static void Day7()
        {
            var phaseCodes = new HashSet<int> {0, 1, 2, 3, 4};
            var permutations = GetPermutations(phaseCodes); 
            int largestOutput = -1;

            foreach (var p in permutations)
            {
                int output = 0;
                p.ForEach(phaseCode => output = AmplifySignal(phaseCode, output));

                if (output > largestOutput)
                    largestOutput = output;
            }

            Console.WriteLine($"Largest output: {largestOutput}");
        }

        public static List<List<T>> GetPermutations<T>(IEnumerable<T> collection)
        {
            var permutations = new List<List<T>>();
            var set = new HashSet<T>(collection);
            GetPermutationsHelper(set, new List<T>(), permutations); 
            return permutations;
        }

        private static void GetPermutationsHelper<T>(HashSet<T> set, List<T> candidate, List<List<T>> permutations)
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
                    GetPermutationsHelper(set, updatedCandidate, permutations);
                }
            }
        }

        public static int AmplifySignal(int phaseSetting, int signal)
        {
            int[] program = Utils.GetInput("day7.txt", ",");
            var input = new List<int> {phaseSetting, signal};
            var computer = new IntcodeComputer(program, input, true);

            List<int> output = computer.Run();
            return output[0];
        }

        static void Main() => Day7();
    }
}
