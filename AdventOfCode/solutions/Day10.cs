using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

namespace AdventOfCode
{
    class Day10
    {
        static public void Part1()
        {
            var input = Utils.GetTextInput("day10.txt", "\n");
            List<(int, int)> asteroidPositions = GetAsteroidPositions(input);

            int max = -1;
            (int, int) maxPosition = (-1, -1);

            foreach (var currentPosition in asteroidPositions)
            {
                int asteroidCount = 0;
                var candidatePositions = new List<(int, int)>(asteroidPositions);
                candidatePositions.Remove(currentPosition);

                foreach (var candidate in candidatePositions)
                {
                    var potentialBlockers = new List<(int, int)>(candidatePositions);
                    potentialBlockers.Remove(candidate);

                    bool isBlocked = false;

                    foreach (var blocker in potentialBlockers)
                    {
                        if (PointOnLine(currentPosition, candidate, blocker))
                        {
                            isBlocked = true;
                            break;
                        }
                    }
                    
                    if (!isBlocked)
                        asteroidCount++;
                }

                if (asteroidCount > max)
                {
                    max = asteroidCount;
                    maxPosition = currentPosition;
                }
            }

            Console.WriteLine($"Maximum asteroids that can be seen: {max} (from ({maxPosition.Item1}, {maxPosition.Item2}))");
        }

        static public void Part2()
        {
            (int x, int y) stationPosition = (8, 3);
            var input = Utils.GetTextInput("day10.txt", "\n");

            List<(int, int)> asteroidPositions = GetAsteroidPositions(input);
            asteroidPositions.Remove(stationPosition);

            double laserMagnitude = 30;            
            double laserPhase = -Math.PI / 2;

            (int x, int y) latestRemoved = (-1, -1);
            int counter = 0;

            while (counter < 200)
            {
                (double x, double y) laserEnd;
                Complex complexPos = Complex.FromPolarCoordinates(laserMagnitude, laserPhase);
                laserEnd.x = complexPos.Real;
                laserEnd.y = complexPos.Imaginary;

                var candidates = new List<(int, int)>();
                foreach ((int x, int y) pos in asteroidPositions)
                {
                    (int x, int y) normailizedPos;
                    normailizedPos.x = pos.x - stationPosition.x;
                    normailizedPos.y = pos.y - stationPosition.y;

                    if (PointOnLine((0, 0), laserEnd, normailizedPos))
                        candidates.Add(pos);
                }

                (int, int) minPos = (0, 0);
                double min = double.MaxValue;
                foreach (var p in candidates)
                {
                    double distance = Distance(stationPosition, p);
                    if (distance < min)
                    {
                        min = distance;
                        minPos = p;
                    }
                }

                if (candidates.Any())
                {
                    asteroidPositions.Remove(minPos);
                    latestRemoved = minPos;
                    counter++;
                    Console.WriteLine($"The {counter} to be vaporized is at {minPos}");
                }

                laserPhase += 0.0037;
            }

            Console.WriteLine($"200th asteroid removed: {latestRemoved}");
            int answer = latestRemoved.x * 100 + latestRemoved.y;
            Console.WriteLine($"Answer: {answer}");
        }

        private static List<(int, int)> GetAsteroidPositions(string[] input)
        {
            var asteroidPositions = new List<(int, int)>();
            for (int i = 0; i < input.Length; i++)
            {
                string row = input[i];
                for (int j = 0; j < row.Length; j++)
                {
                    if (row[j] == '#')
                        asteroidPositions.Add((j, i));
                }
            }
            return asteroidPositions;
        }

        private static bool PointOnLine((double x, double y) lineStart, (double x, double y) lineEnd, (double x, double y) point) 
        {
            double diff = Distance(lineStart, point) + Distance(lineEnd, point) - Distance(lineStart, lineEnd);
            return Math.Abs(diff) < 0.000009;
        }

        private static double Distance((double x, double y) a, (double x, double y) b)
        {
            var distance = Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
            return distance;
        }
    }
}