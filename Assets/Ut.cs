using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hexamaze
{
    static class Ut
    {
        public static T[] NewArray<T>(params T[] array)
        {
            return array;
        }

        private static Marking[] _markingsSquare = new[] { Marking.SquareNS, Marking.SquareNeSw, Marking.SquareNwSe };
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

        public static string ToMarkingString(this IEnumerable<Marking> markings) { return markings.Select(m => m == Marking.None ? "•" : ((int) m).ToString()).JoinString(); }

        /// <summary>
        ///     Turns all elements in the enumerable to strings and joins them using the specified <paramref
        ///     name="separator"/> and the specified <paramref name="prefix"/> and <paramref name="suffix"/> for each string.</summary>
        /// <param name="values">
        ///     The sequence of elements to join into a string.</param>
        /// <param name="separator">
        ///     Optionally, a separator to insert between each element and the next.</param>
        /// <param name="prefix">
        ///     Optionally, a string to insert in front of each element.</param>
        /// <param name="suffix">
        ///     Optionally, a string to insert after each element.</param>
        /// <param name="lastSeparator">
        ///     Optionally, a separator to use between the second-to-last and the last element.</param>
        /// <example>
        ///     <code>
        ///         // Returns "[Paris], [London], [Tokyo]"
        ///         (new[] { "Paris", "London", "Tokyo" }).JoinString(", ", "[", "]")
        ///         
        ///         // Returns "[Paris], [London] and [Tokyo]"
        ///         (new[] { "Paris", "London", "Tokyo" }).JoinString(", ", "[", "]", " and ");</code></example>
        public static string JoinString<T>(this IEnumerable<T> values, string separator = null, string prefix = null, string suffix = null, string lastSeparator = null)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (lastSeparator == null)
                lastSeparator = separator;

            using (var enumerator = values.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return "";

                // Optimise the case where there is only one element
                var one = enumerator.Current;
                if (!enumerator.MoveNext())
                    return prefix + one + suffix;

                // Optimise the case where there are only two elements
                var two = enumerator.Current;
                if (!enumerator.MoveNext())
                {
                    // Optimise the (common) case where there is no prefix/suffix; this prevents an array allocation when calling string.Concat()
                    if (prefix == null && suffix == null)
                        return one + lastSeparator + two;
                    return prefix + one + suffix + lastSeparator + prefix + two + suffix;
                }

                StringBuilder sb = new StringBuilder()
                    .Append(prefix).Append(one).Append(suffix).Append(separator)
                    .Append(prefix).Append(two).Append(suffix);
                var prev = enumerator.Current;
                while (enumerator.MoveNext())
                {
                    sb.Append(separator).Append(prefix).Append(prev).Append(suffix);
                    prev = enumerator.Current;
                }
                sb.Append(lastSeparator).Append(prefix).Append(prev).Append(suffix);
                return sb.ToString();
            }
        }
    }
}
