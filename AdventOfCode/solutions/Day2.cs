using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace AdventOfCode
{
    class Day2
    {
        public static void Part1()
        {
            long[] data = Utils.GetInput("day2.txt", ",");
            Console.WriteLine("Output: {0}", RunProgram(12, 2, data));
        }

        public static void Part2()
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
    }
}