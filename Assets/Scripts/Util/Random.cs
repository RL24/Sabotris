using System;
using UnityEngine;

namespace Sabotris.Util
{
    public static class Random
    {
        private static System.Random m_random = new System.Random((int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
        // private static System.Random m_random = new System.Random(12345); // DEBUG ONLY

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

        public static Color RandomColor(float hueMin, float hueMax, float saturationMin, float saturationMax, float valueMin, float valueMax) =>
            Color.HSVToRGB(Range(hueMin, hueMax), Range(saturationMin, saturationMax), Range(valueMin, valueMax), true);
    }
}