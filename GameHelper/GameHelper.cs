using System;

namespace ReFMGame.GameHelper
{
    public static class GameHelper
    {
        public static double Map(this double val, double min, double max, double toMin, double toMax)
        {
            val = Math.Min(max, Math.Max(min, val));
            return (val - min) * (toMax - toMin) / (max - min) + toMin;
        }
        public static long ExtractBits(this long value, int from, int to)
        {
            long mask = (1 << (to - from + 1)) - 1;
            return (value >> from) & mask;
        }
    }
}
