using UnityEngine;

namespace Sabotris.Util
{
    public static class ColorUtil
    {
        public static Color GenerateColor()
        {
            return Random.ColorHSV(0, 1, 0.7f, 0.7f, 1, 1);
        }

        public static Color GeneratePoweredColor()
        {
            return Color.HSVToRGB(0, 0, 0.2f);
        }
    }
}