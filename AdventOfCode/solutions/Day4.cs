using System;
using System.Linq;
using System.Collections.Generic;

namespace AdventOfCode
{
    class Day4
    {
        public static void Part1And2()
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
    }
}