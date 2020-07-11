using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace AdventOfCode
{
    class Day6
    {
        public static void Part1And2()
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

    }
}