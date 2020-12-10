using System;
using System.Collections.Generic;
using System.Linq;

namespace SecretSantaApplication.Extensions
{
    public static class ListExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items)
        {
            Random random = new Random();
            return items.OrderBy(k => random.Next(100));
        }
    }
}