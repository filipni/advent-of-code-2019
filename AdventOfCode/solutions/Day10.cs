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
            var input = Utils.GetRows("day10.txt");
            List<(int, int)> asteroidPositions = GetAsteroidPositions(input);

            int maxAsteroidCount = -1;
            (int x, int y) maxPosition = (-1, -1);

            foreach (var position in asteroidPositions)
            {
                int asteroidCount = 0;
                var potentialNeighbours = new List<(int, int)>(asteroidPositions);
                potentialNeighbours.Remove(position);

                foreach (var neighbour in potentialNeighbours)
                {
                    var potentialBlockers = new List<(int, int)>(potentialNeighbours);
                    potentialBlockers.Remove(neighbour);

                    bool isBlocked = false;
                    foreach (var blocker in potentialBlockers)
                    {
                        if (PointOnLine(position, neighbour, blocker))
                        {
                            isBlocked = true;
                            break;
                        }
                    }
                    
                    if (!isBlocked)
                        asteroidCount++;
                }

                if (asteroidCount > maxAsteroidCount)
                {
                    maxAsteroidCount = asteroidCount;
                    maxPosition = position;
                }
            }

            Console.WriteLine($"Maximum asteroids that can be seen: {maxAsteroidCount} (from ({maxPosition.x}, {maxPosition.y}))");
        }

        static public void Part2()
        {
            (int x, int y) basePosition = (29, 28);
            var input = Utils.GetRows("day10.txt");

            List<(int, int)> asteroidPositions = GetAsteroidPositions(input);
            asteroidPositions.Remove(basePosition);

            var angleToAsteroids = new Dictionary<double, List<(double, (int, int))>>();
            foreach ((int x, int y) asteroid in asteroidPositions)
            {
                double angle = (Math.Atan2(asteroid.y - basePosition.y, asteroid.x - basePosition.x) + 2.5 * Math.PI) % (2 * Math.PI);
                double distance = Distance(asteroid, basePosition);
                
                if (angleToAsteroids.ContainsKey(angle))
                    angleToAsteroids[angle].Add((distance, asteroid)); 
                else
                    angleToAsteroids.Add(angle, new List<(double, (int, int))> {(distance, asteroid)});
            }

            var angles = angleToAsteroids.Keys.OrderBy(x => x);
            int counter = 0;

            while(true)
            {
                foreach (double a in angles)
                {
                    var asteroids = angleToAsteroids[a];
                    if (asteroids.Count == 0)
                        continue;

                    (double distance, (int x, int y) position) asteroid = asteroids[0];

                    asteroids.Remove(asteroid);
                    counter++;

                    if (counter == 200)
                    {
                        int answer = asteroid.position.x * 100 + asteroid.position.y;
                        Console.WriteLine($"Answer: {answer}");
                        System.Environment.Exit(0);
                    }
                }
            }
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
            return Math.Abs(diff) < 0.00001;
        }

        private static double Distance((double x, double y) a, (double x, double y) b)
        {
            var distance = Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
            return distance;
        }
    }
}