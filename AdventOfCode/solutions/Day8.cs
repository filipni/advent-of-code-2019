using System;
using System.Linq;
using System.Collections.Generic;

namespace AdventOfCode
{
    class Day8
    {
        public static void Part1()
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

        public static void Part2()
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
    }
}