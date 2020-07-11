using System;
using System.Linq;
using System.Collections.Generic;

namespace AdventOfCode
{
    class Day1
    {
        public static void Part1() => Console.WriteLine(Utils.GetInput("day1.txt", "\n").Select(x => x / 3 - 2).Sum());

        public static void Part2()
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
    }
}