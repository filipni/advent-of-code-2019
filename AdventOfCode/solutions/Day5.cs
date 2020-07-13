using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace AdventOfCode
{
    class Day5
    {
        public static void Part1And2()
        {
            Console.WriteLine($"Part 1:");
            RunDiagnosticTest(1, "day5.txt");

            Console.WriteLine($"Part 2:");
            RunDiagnosticTest(5, "day5.txt");
        }

        public static void RunDiagnosticTest(int code, string inputFilename)
        {
            long[] data = Utils.GetNumberInput(inputFilename, ",");
            var inputQueue = new BlockingCollection<long> {code};
            var outputQueue = new BlockingCollection<long>();

            IntcodeComputer computer = new IntcodeComputer(data, inputQueue, outputQueue);
            computer.Run().Wait();

            var output = new List<long>();
            while (!outputQueue.IsCompleted)
                output.Add(outputQueue.Take());

            Console.WriteLine(string.Join(",", output));
        }
    }
}