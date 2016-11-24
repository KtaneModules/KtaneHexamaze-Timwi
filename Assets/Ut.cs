using System;
using System.Collections.Generic;
using System.Linq;

using Rnd = UnityEngine.Random;

namespace Hexamaze
{
    static class Ut
    {
        public static T[] NewArray<T>(params T[] array)
        {
            return array;
        }

        private static Marking[] _markingsSquare = new[] { Marking.SquareNeSw, Marking.SquareNS, Marking.SquareNwSe };
        private static Marking[] _markingsTriangle1 = new[] { Marking.TriangleUp, Marking.TriangleDown };
        private static Marking[] _markingsTriangle2 = new[] { Marking.TriangleLeft, Marking.TriangleRight };
        public static Marking Rotate(this Marking marking, int rotation)
        {
            switch (marking)
            {
                case Marking.None: return Marking.None;
                case Marking.Circle: return Marking.Circle;

                case Marking.SquareNeSw:
                case Marking.SquareNwSe:
                case Marking.SquareNS:
                    return _markingsSquare[(Array.IndexOf(_markingsSquare, marking) + (rotation % 6 + 6) % 6) % 3];

                case Marking.TriangleUp:
                case Marking.TriangleDown:
                    return _markingsTriangle1[(Array.IndexOf(_markingsTriangle1, marking) + (rotation % 6 + 6) % 6) % 2];

                case Marking.TriangleLeft:
                case Marking.TriangleRight:
                    return _markingsTriangle2[(Array.IndexOf(_markingsTriangle2, marking) + (rotation % 6 + 6) % 6) % 2];

                default:
                    throw new ArgumentException("Invalid marking.", "marking");
            }
        }
        
        /// <summary>
        ///     Gets a value from a dictionary by key. If the key does not exist in the dictionary, the default value is
        ///     returned instead.</summary>
        /// <param name="dict">
        ///     Dictionary to operate on.</param>
        /// <param name="key">
        ///     Key to look up.</param>
        /// <param name="defaultVal">
        ///     Value to return if key is not contained in the dictionary.</param>
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultVal)
        {
            if (dict == null)
                throw new ArgumentNullException("dict");
            if (key == null)
                throw new ArgumentNullException("key", "Null values cannot be used for keys in dictionaries.");
            TValue value;
            if (dict.TryGetValue(key, out value))
                return value;
            else
                return defaultVal;
        }

        public static T PickRandom<T>(this IEnumerable<T> src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            var arr = src.ToArray();
            if (arr.Length == 0)
                throw new InvalidOperationException("Cannot pick a random element from an empty set.");
            return arr[Rnd.Range(0, arr.Length)];
        }
    }
}
