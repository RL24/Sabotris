using System;
using UnityEngine;

namespace Sabotris.Util
{
    public static class Random
    {
        private static System.Random m_random = new System.Random((int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);

        private const string Consonants = "bcdfghjklmnpqrstvwxyz";
        private const string Vowels = "aeiou";

        public static void SetSeed(int seed)
        {
            m_random = new System.Random(seed);
        }

        public static float Range(float min, float max)
        {
            var next = m_random.NextDouble();
            return (float) (min + (max - min) * next);
        }

        public static int Range(int min, int max) => (int) Math.Round(Range((float) min, max));

        public static Color RandomColor() => RandomColor(0, 1, 0.7f, 0.7f, 1, 1);
        
        public static Color RandomColor(float hueMin, float hueMax, float saturationMin, float saturationMax, float valueMin, float valueMax) =>
            Color.HSVToRGB(Range(hueMin, hueMax), Range(saturationMin, saturationMax), Range(valueMin, valueMax), true);

        public static Color RandomPoweredColor() => Color.HSVToRGB(0, 0, 0.2f);

        public static bool Boolean()
        {
            return m_random.NextDouble() > 0.5;
        }

        public static string RandomName(int length)
        {
            var name = "";
            char? prevChar = null;
            var vowel = Boolean();

            for (var i = 0; i < length; i++)
            {
                vowel = !vowel;
                var selector = vowel ? Vowels : Consonants;
                var nextChar = selector[Range(0, selector.Length - 1)];
                if (prevChar == null)
                {
                    name += nextChar;
                    prevChar = nextChar;
                    continue;
                }

                name += nextChar;
                prevChar = nextChar;
            }

            name = name[0].ToString().ToUpper() + name.Substring(1, name.Length - 1);
            return name;
        }
    }
}