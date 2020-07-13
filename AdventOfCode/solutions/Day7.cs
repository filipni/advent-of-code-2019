using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace AdventOfCode
{
    class Day7
    {
        public static void Part1() => SolveDay7(Enumerable.Range(0, 5), false);
        public static void Part2() => SolveDay7(Enumerable.Range(5, 5), true);

        private static void SolveDay7(IEnumerable<int> phaseCodes, bool feedbackOn)
        {
            List<List<int>> sequenceSettings = Permutations(phaseCodes); 
            long largestThrusterValue = -1;
            var finalSettingSequence = "";

            foreach (List<int> settings in sequenceSettings)
            {
                List<long> settingsConverted = settings.ConvertAll(x => (long)x);
                long[] program = Utils.GetNumberInput("day7.txt", ",");

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

    }
}