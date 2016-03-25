using System;
using System.Collections.Generic;
using System.Linq;

namespace EuchreChampion
{
    public static class Extensions
    {
        public static int NextPlayer(this int index)
        {
            return (index + 1) % 4;
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            Random random = new Random();
            T[] elements = source.ToArray();
            for (int i = elements.Length - 1; i >= 0; i--)
            {
                int swapIndex = random.Next(i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }
        }
    }
}
