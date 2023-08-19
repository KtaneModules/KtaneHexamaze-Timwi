﻿using UnityEngine;

namespace Hexamaze
{
    sealed class GeneratedMaze
    {
        private readonly bool[] _walls;
        private readonly Marking[] _markings;
        public GeneratedMaze(bool[] walls, Marking[] markings)
        {
            _walls = walls;
            _markings = markings;
        }

        public Marking GetMarking(Hex hex)
        {
            return _markings[markingIndex(hex)];
        }

        public bool HasWall(Hex hex, int dir)
        {
            var ix = wallIndex(hex, dir);
            if (ix < 0 || ix >= _walls.Length)
                Debug.LogFormat("<> {0}, {1}", ix, _walls.Length);
            return _walls[ix];
        }

        private static int markingIndex(Hex h)
        {
            return (h.Q + HexamazeRuleGenerator.Size) * HexamazeRuleGenerator.sw + h.R + HexamazeRuleGenerator.Size;
        }

        private static int markingIndex(int q, int r)
        {
            return (q + HexamazeRuleGenerator.Size) * HexamazeRuleGenerator.sw + r + HexamazeRuleGenerator.Size;
        }

        private static int wallIndex(Hex hex, int dir)
        {
            return dir < 3
                ? 3 * HexamazeRuleGenerator.sw * (hex.Q + HexamazeRuleGenerator.Size) + 3 * (hex.R + HexamazeRuleGenerator.Size) + dir
                : wallIndex(hex.GetNeighbor(dir), dir - 3);
        }

        private void setWalls(int q, int r, bool[] walls)
        {
            var b = 3 * HexamazeRuleGenerator.sw * (q + HexamazeRuleGenerator.Size) + 3 * (r + HexamazeRuleGenerator.Size);
            for (var i = 0; i < 3; i++)
                if (new Hex(q, r).Distance < HexamazeRuleGenerator.Size || new Hex(q, r).GetNeighbor(i).Distance < HexamazeRuleGenerator.Size)
                    _walls[b + i] = walls[i];
        }

        public static readonly GeneratedMaze Default = new GeneratedMaze(
            new[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, true, false, false, true, false, false, true, false, false, false, false, false, true, false, false, true, false, false, true, false, false, true, false, false, true, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, true, true, false, true, true, true, true, false, false, true, true, false, true, true, false, true, false, true, false, false, true, true, false, false, true, false, true, true, false, true, true, true, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, true, false, true, true, false, true, true, false, false, true, true, false, true, true, false, true, false, true, true, false, false, true, true, false, true, true, false, true, true, true, true, true, true, true, true, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, false, true, true, true, true, true, true, true, true, false, true, false, true, true, false, true, true, true, true, true, true, true, true, true, true, true, true, false, true, true, true, false, true, true, false, false, true, true, true, true, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, true, true, true, true, false, true, true, false, true, true, true, false, true, true, false, true, false, true, true, false, false, true, false, false, true, false, true, false, true, false, false, false, false, true, false, false, true, true, true, false, false, false, true, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, false, false, true, true, false, true, true, false, true, true, false, true, true, true, true, false, false, true, true, false, true, true, true, true, false, false, true, false, true, true, false, true, true, false, true, false, false, false, true, false, true, true, false, false, true, true, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, false, false, true, true, false, true, true, false, true, false, false, false, true, false, true, false, false, true, false, false, false, true, false, false, true, false, false, false, true, true, false, true, true, false, true, true, false, true, true, false, true, false, true, true, false, true, false, true, true, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, false, true, false, true, false, true, true, false, false, false, true, true, false, false, false, false, true, true, false, true, false, true, true, false, true, true, false, false, true, true, false, true, false, false, true, true, false, false, true, true, true, false, true, true, false, true, true, false, false, false, true, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, true, true, false, false, true, true, true, true, true, true, true, false, true, true, false, true, true, false, false, true, true, true, false, true, true, false, true, true, false, false, true, true, false, false, true, false, false, true, false, false, false, true, true, false, false, true, true, false, true, false, true, true, false, true, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, false, true, true, false, false, true, false, false, false, true, false, false, false, true, true, true, true, false, true, false, true, true, false, true, true, false, true, true, true, true, false, true, true, false, true, true, false, true, true, false, true, false, true, true, false, false, true, true, false, true, false, true, true, true, false, true, false, true, false, true, true, false, false, false, false, false, false, false, false, false, true, true, true, true, false, false, true, true, false, true, true, true, true, false, true, true, false, true, false, false, true, false, true, true, false, true, true, false, true, true, false, true, true, true, true, false, true, true, false, true, true, false, false, true, false, true, true, false, true, true, false, true, false, true, true, false, true, true, true, false, false, true, true, false, true, true, false, false, false, false, false, false, true, true, false, true, false, false, true, true, false, true, true, false, false, false, false, false, true, true, true, false, true, false, false, true, false, false, true, false, false, false, false, false, true, false, false, true, true, false, true, false, true, true, true, true, false, false, true, true, false, true, true, true, false, true, false, true, false, true, false, true, true, false, true, true, false, true, false, true, true, false, false, false, false, true, true, true, true, true, true, false, true, true, true, true, true, true, false, false, true, true, false, true, true, true, false, true, true, false, true, true, false, false, false, true, false, true, false, true, true, false, false, false, false, true, false, false, true, true, false, true, false, false, true, false, true, true, true, false, true, true, false, true, true, false, false, true, false, true, false, true, true, false, true, false, false, false, false, true, true, true, true, false, true, false, false, false, false, true, false, true, true, false, false, false, false, false, true, true, true, false, true, true, false, true, true, true, false, true, false, true, true, false, true, false, true, true, true, false, true, true, false, true, true, false, true, false, false, false, false, false, true, true, false, true, true, true, false, true, true, false, true, true, false, true, true, false, false, false, false, false, false, false, true, true, true, true, false, true, false, true, true, true, false, false, true, true, false, false, true, true, false, false, true, false, true, true, false, true, true, true, false, true, false, false, false, false, true, false, false, true, true, false, true, false, true, false, false, true, false, true, false, true, false, true, false, true, true, false, true, false, true, true, true, false, true, true, true, false, false, false, false, false, false, false, false, false, false, false, true, false, true, true, true, true, false, false, true, true, false, false, true, false, true, false, false, false, false, false, false, true, true, false, true, true, true, false, true, false, true, true, false, true, true, false, true, true, true, false, true, true, false, true, true, false, false, false, true, true, true, false, true, true, false, true, false, true, true, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, false, false, false, false, false, false, false, false, true, false, false, false, false, true, true, false, true, true, false, false, true, false, false, true, true, true, false, true, false, true, false, false, true, false, true, true, true, false, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, true, false, true, true, false, false, false, true, false, true, true, true, false, false, true, true, false, true, false, false, true, false, false, false, true, false, false, false, true, false, true, false, false, false, true, true, true, false, false, true, false, false, true, true, false, true, true, false, true, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, false, true, false, true, true, false, false, false, true, true, false, true, true, false, true, true, true, false, true, true, false, true, true, false, false, true, true, false, true, true, false, true, false, true, true, false, true, true, false, true, false, true, true, false, true, true, true, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, true, true, true, true, true, true, true, false, false, true, true, false, false, true, false, false, true, true, false, true, true, false, false, true, true, false, true, true, true, false, true, true, false, true, true, false, true, true, false, true, true, false, true, false, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, false, false, true, false, true, true, true, true, true, true, true, false, true, true, false, true, false, true, false, true, false, false, false, false, false, true, false, true, false, false, false, false, true, false, true, true, false, true, true, true, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, false, false, true, false, true, true, true, true, false, false, true, true, false, true, false, true, true, false, true, true, false, false, true, true, true, true, false, true, false, true, true, false, true, false, false, false, true, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, false, true, true, false, true, false, false, true, false, true, true, true, true, true, false, true, true, false, false, true, false, true, true, false, false, true, false, true, false, false, true, true, false, true, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, true, true, true, true, true, false, true, false, true, false, true, true, true, true, true, true, true, true, true, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, true, false, false, true, false, false, true, false, false, true, false, false, true, false, false, true, false, false, true, false, false, false, false, false, true, false, false, true, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
            new[] { Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.Hexagon, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.TriangleLeft, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.Circle, Marking.None, Marking.None, Marking.Circle, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.Hexagon, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.TriangleDown, Marking.None, Marking.None, Marking.None, Marking.TriangleUp, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.Circle, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.TriangleRight, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.TriangleRight, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.Hexagon, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.TriangleUp, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.Circle, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.Hexagon, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.Circle, Marking.None, Marking.None, Marking.None, Marking.TriangleDown, Marking.None, Marking.None, Marking.None, Marking.TriangleRight, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.TriangleLeft, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.Hexagon, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None, Marking.None });
    }
}
