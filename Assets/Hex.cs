﻿using System;
using System.Collections.Generic;

namespace Hexamaze
{
    public struct Hex : IEquatable<Hex>
    {
        public static IEnumerable<Hex> LargeHexagon(int sideLength)
        {
            for (int r = -sideLength + 1; r < sideLength; r++)
                for (int q = -sideLength + 1; q < sideLength; q++)
                {
                    var hex = new Hex(q, r);
                    if (hex.Distance < sideLength)
                        yield return hex;
                }
        }
        public static readonly double WidthToHeight = Math.Sqrt(3) / 2;

        public static double LargeWidth(int sideLength) { return (3 * sideLength - 1) * .5; }
        public static double LargeHeight(int sideLength) { return 2 * sideLength - 1; }

        public static IEnumerable<PointD> LargeHexagonOutline(int sideLength, double hexWidth, double expand = 0)
        {
            sideLength--;

            var tan30 = Math.Tan(Math.PI / 6);
            var sec30 = 1 / Math.Cos(Math.PI / 6);

            // North-east
            var offset = expand * new PointD(tan30, -1);
            for (int i = 0; i <= sideLength; i++)
            {
                if (i > 0)
                    yield return new PointD((i * .75 - .25) * hexWidth, (i * .5 - sideLength - .50) * hexWidth * WidthToHeight) + offset;
                yield return new PointD((i * .75 + .25) * hexWidth, (i * .5 - sideLength - .50) * hexWidth * WidthToHeight) + offset;
            }
            // East
            offset = expand * new PointD(sec30, 0);
            for (int i = 0; i <= sideLength; i++)
            {
                if (i > 0)
                    yield return new PointD((sideLength * .75 + .25) * hexWidth, (sideLength * .5 - sideLength + i - .50) * hexWidth * WidthToHeight) + offset;
                yield return new PointD((sideLength * .75 + .50) * hexWidth, (sideLength * .5 - sideLength + i + .00) * hexWidth * WidthToHeight) + offset;
            }
            // South-east
            offset = expand * new PointD(tan30, 1);
            for (int i = 0; i <= sideLength; i++)
            {
                if (i > 0)
                    yield return new PointD(((sideLength - i) * .75 + .50) * hexWidth, ((sideLength - i) * .5 + i + .00) * hexWidth * WidthToHeight) + offset;
                yield return new PointD(((sideLength - i) * .75 + .25) * hexWidth, ((sideLength - i) * .5 + i + .50) * hexWidth * WidthToHeight) + offset;
            }
            // South-west
            offset = expand * new PointD(-tan30, 1);
            for (int i = 0; i <= sideLength; i++)
            {
                if (i > 0)
                    yield return new PointD((-i * .75 + .25) * hexWidth, (-i * .5 + sideLength + .50) * hexWidth * WidthToHeight) + offset;
                yield return new PointD((-i * .75 - .25) * hexWidth, (-i * .5 + sideLength + .50) * hexWidth * WidthToHeight) + offset;
            }
            // West
            offset = expand * new PointD(-sec30, 0);
            for (int i = 0; i <= sideLength; i++)
            {
                if (i > 0)
                    yield return new PointD((-sideLength * .75 - .25) * hexWidth, (-sideLength * .5 + sideLength - i + .50) * hexWidth * WidthToHeight) + offset;
                yield return new PointD((-sideLength * .75 - .50) * hexWidth, (-sideLength * .5 + sideLength - i + .00) * hexWidth * WidthToHeight) + offset;
            }
            // North-west
            offset = expand * new PointD(-tan30, -1);
            for (int i = 0; i <= sideLength; i++)
            {
                if (i > 0)
                    yield return new PointD(((-sideLength + i) * .75 - .50) * hexWidth, ((-sideLength + i) * .5 - i + .00) * hexWidth * WidthToHeight) + offset;
                yield return new PointD(((-sideLength + i) * .75 - .25) * hexWidth, ((-sideLength + i) * .5 - i - .50) * hexWidth * WidthToHeight) + offset;
            }
        }

        public int Q { get; private set; }
        public int R { get; private set; }

        public Hex[] Neighbors
        {
            get
            {
                return Ut.NewArray(
                    new Hex(Q - 1, R),
                    new Hex(Q, R - 1),
                    new Hex(Q + 1, R - 1),
                    new Hex(Q + 1, R),
                    new Hex(Q, R + 1),
                    new Hex(Q - 1, R + 1));
            }
        }

        public int Distance { get { return Math.Max(Math.Abs(Q), Math.Max(Math.Abs(R), Math.Abs(-Q - R))); } }

        public IEnumerable<int> GetEdges(int size)
        {
            // Don’t use ‘else’ because multiple conditions could apply
            if (Q + R == -size)
                yield return 0;
            if (R == -size)
                yield return 1;
            if (Q == size)
                yield return 2;
            if (Q + R == size)
                yield return 3;
            if (R == size)
                yield return 4;
            if (Q == -size)
                yield return 5;
        }

        public PointD[] GetPolygon(double hexWidth)
        {
            return Ut.NewArray(
                new PointD((Q * .75 - .50) * hexWidth, (Q * .5 + R + .00) * hexWidth * WidthToHeight),
                new PointD((Q * .75 - .25) * hexWidth, (Q * .5 + R - .50) * hexWidth * WidthToHeight),
                new PointD((Q * .75 + .25) * hexWidth, (Q * .5 + R - .50) * hexWidth * WidthToHeight),
                new PointD((Q * .75 + .50) * hexWidth, (Q * .5 + R + .00) * hexWidth * WidthToHeight),
                new PointD((Q * .75 + .25) * hexWidth, (Q * .5 + R + .50) * hexWidth * WidthToHeight),
                new PointD((Q * .75 - .25) * hexWidth, (Q * .5 + R + .50) * hexWidth * WidthToHeight)
            );
        }

        public PointD GetCenter(double hexWidth) { return new PointD(Q * .75 * hexWidth, (Q * .5 + R) * hexWidth * WidthToHeight); }

        public override string ToString() { return string.Format("({0}, {1})", Q, R); }

        public Hex(int q, int r) : this() { Q = q; R = r; }

        public bool Equals(Hex other) { return Q == other.Q && R == other.R; }
        public override bool Equals(object obj) { return obj is Hex && Equals((Hex) obj); }
        public static bool operator ==(Hex one, Hex two) { return one.Q == two.Q && one.R == two.R; }
        public static bool operator !=(Hex one, Hex two) { return one.Q != two.Q || one.R != two.R; }
        public override int GetHashCode() { return Q * 47 + R; }

        public static Hex operator +(Hex one, Hex two) { return new Hex(one.Q + two.Q, one.R + two.R); }
        public static Hex operator -(Hex one, Hex two) { return new Hex(one.Q - two.Q, one.R - two.R); }

        public Hex Rotate(int rotation)
        {
            switch (((rotation % 6) + 6) % 6)
            {
                case 0: return this;
                case 1: return new Hex(-R, Q + R);
                case 2: return new Hex(-Q - R, Q);
                case 3: return new Hex(-Q, -R);
                case 4: return new Hex(R, -Q - R);
                case 5: return new Hex(Q + R, -Q);
            }
            throw new ArgumentException("Rotation must be between 0 and 5.", "rotation");
        }
    }
}
