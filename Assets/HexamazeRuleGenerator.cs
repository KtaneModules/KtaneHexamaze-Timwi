using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hexamaze
{
    sealed class HexamazeRuleGenerator
    {
        public const int Size = 12;
        public const int SubmazeSize = 4;
        public const int sw = 2 * Size + 1;

        public static GeneratedMaze Generate(MonoRandom rnd)
        {
            // PART 1: GENERATE MAZE (walls)
            var walls = Ut.NewArray(3 * sw * sw, ix =>
            {
                var r = (ix / 3) % sw - Size;
                var q = (ix / 3) / sw - Size;
                var h = new Hex(q, r);
                return h.Distance < Size || h.GetNeighbor(ix % 3).Distance < Size;
            });
            var allWalls = (bool[]) walls.Clone();

            var stack = new Stack<Hex>();
            var curHex = new Hex(0, 0);
            stack.Push(curHex);
            var taken = new HashSet<Hex> { curHex };

            // Step 1.1: generate a single giant maze
            while (true)
            {
                var neighbors = curHex.Neighbors;
                var availableNeighborIndices = neighbors.SelectIndexWhere(n => !taken.Contains(n) && n.Distance < Size).ToArray();
                if (availableNeighborIndices.Length == 0)
                {
                    if (stack.Count == 0)
                        break;
                    curHex = stack.Pop();
                    continue;
                }
                var dir = availableNeighborIndices[rnd.Next(0, availableNeighborIndices.Length)];
                walls[wallIndex(curHex, dir)] = false;
                stack.Push(curHex);
                curHex = neighbors[dir];
                taken.Add(curHex);
            }

            // Step 1.2: Go through all submazes and make sure they’re all connected and all have at least one exit on each side
            // This is parallelizable and uses multiple threads
            var allSubmazes = Hex.LargeHexagon(Size - SubmazeSize + 1).Select(h => (Hex?) h).ToArray();
            Hex? lastHex1 = null, lastHex2 = null;
            while (true)
            {
                var candidateCounts = new Dictionary<int, int>();

                for (var smIx = 0; smIx < allSubmazes.Length; smIx++)
                {
                    if (allSubmazes[smIx] == null)
                        continue;

                    var centerHex = allSubmazes[smIx].Value;

                    // We do not need to examine this submaze if the wall we last removed isn’t even in it
                    if (lastHex1 != null && (lastHex1.Value - centerHex).Distance > SubmazeSize &&
                        lastHex2 != null && (lastHex2.Value - centerHex).Distance > SubmazeSize)
                        continue;

                    var validity = DetermineSubmazeValidity(centerHex, walls);
                    if (validity.IsValid)
                    {
                        allSubmazes[smIx] = null;
                        continue;
                    }

                    // Find out which walls might benefit from removing
                    foreach (var fh in validity.Filled)
                    {
                        var neighbors = fh.Neighbors;
                        for (var dir = 0; dir < neighbors.Length; dir++)
                        {
                            var th = neighbors[dir];
                            var offset = th - centerHex;
                            if ((offset.Distance < SubmazeSize && walls[wallIndex(fh, dir)] && !validity.Filled.Contains(th)) ||
                                (offset.Distance == SubmazeSize && offset.GetEdges(SubmazeSize).Any(e => !validity.EdgesReachable[e])))
                                candidateCounts.IncSafe(wallIndex(fh, dir));
                        }
                    }
                }

                if (candidateCounts.Count == 0)
                    break;

                // Remove one wall out of the “most wanted”
                var topScore = 0;
                var topScorers = new List<int>();
                foreach (var kvp in candidateCounts)
                    if (kvp.Value > topScore)
                    {
                        topScore = kvp.Value;
                        topScorers.Clear();
                        topScorers.Add(kvp.Key);
                    }
                    else if (kvp.Value == topScore)
                        topScorers.Add(kvp.Key);
                topScorers.Sort();
                var randomWall = topScorers[rnd.Next(0, topScorers.Count)];
                walls[randomWall] = false;

                var rcdir = randomWall % 3;
                lastHex1 = new Hex((randomWall / 3) / sw - Size, (randomWall / 3) % sw - Size);
                lastHex2 = lastHex1.Value.GetNeighbor(rcdir);
            }

            // Step 1.3: Put as many walls back in as possible
            var missingWalls = Enumerable.Range(0, allWalls.Length).Where(ix => allWalls[ix] && !walls[ix]).ToList();
            while (missingWalls.Count > 0)
            {
                var randomMissingWallIndex = rnd.Next(0, missingWalls.Count);
                var randomMissingWall = missingWalls[randomMissingWallIndex];
                missingWalls.RemoveAt(randomMissingWallIndex);
                walls[randomMissingWall] = true;

                var affectedHex1 = new Hex((randomMissingWall / 3) / sw - Size, (randomMissingWall / 3) % sw - Size);
                var affectedHex2 = affectedHex1.GetNeighbor(randomMissingWall % 3);

                foreach (var centerHex in rnd.ShuffleFisherYates(Hex.LargeHexagon(Size - SubmazeSize + 1).ToList()))
                {
                    // We do not need to examine this submaze if the wall we put in isn’t even in it
                    if (((affectedHex1 - centerHex).Distance <= SubmazeSize ||
                        (affectedHex2 - centerHex).Distance <= SubmazeSize) &&
                        !DetermineSubmazeValidity(centerHex, walls).IsValid)
                    {
                        // This wall cannot be added, take it back out.
                        walls[randomMissingWall] = false;
                        break;
                    }
                }
            }

            // PART 2: GENERATE MARKINGS
            tryAgain:
            var markings = new Marking[sw * sw];
            // List Circle and Hexagon twice so that triangles don’t completely dominate the distribution
            var allowedMarkings = new[] { Marking.Circle, Marking.Circle, Marking.Hexagon, Marking.Hexagon, Marking.TriangleDown, Marking.TriangleLeft, Marking.TriangleRight, Marking.TriangleUp };

            // Step 2.1: Put random markings in until there are no more ambiguities
            while (!areMarkingsUnique(markings))
            {
                var availableHexes = Hex.LargeHexagon(Size)
                    .Where(h => markings[markingIndex(h)] == Marking.None &&
                        h.Neighbors.SelectMany(n1 => n1.Neighbors)
                            .All(n2 => n2.Distance >= Size || markings[markingIndex(n2)] == Marking.None))
                    .ToArray();
                if (availableHexes.Length == 0)
                    goto tryAgain;
                var randomHex = availableHexes[rnd.Next(0, availableHexes.Length)];
                markings[markingIndex(randomHex)] = allowedMarkings[rnd.Next(0, allowedMarkings.Length)];
            }

            // Step 2.2: Find markings to remove again
            var removableMarkings = markings.SelectIndexWhere(m => m != Marking.None).ToList();
            while (removableMarkings.Count > 0)
            {
                var tryRemoveIndex = rnd.Next(0, removableMarkings.Count);
                var tryRemove = removableMarkings[tryRemoveIndex];
                removableMarkings.RemoveAt(tryRemoveIndex);
                var prevMarking = markings[tryRemove];
                markings[tryRemove] = Marking.None;
                if (!areMarkingsUnique(markings))
                {
                    // No longer unique — put it back in
                    markings[tryRemove] = prevMarking;
                }
            }

            return new GeneratedMaze(walls, markings);
        }

        private static int markingIndex(Hex h)
        {
            return (h.Q + Size) * sw + h.R + Size;
        }

        private static int wallIndex(Hex hex, int dir)
        {
            return dir < 3
                ? 3 * sw * (hex.Q + Size) + 3 * (hex.R + Size) + dir
                : wallIndex(hex.GetNeighbor(dir), dir - 3);
        }

        private sealed class SubmazeValidity
        {
            public HashSet<Hex> Filled;
            public bool[] EdgesReachable;
            public bool IsValid;
        }

        private static bool hasWall(Hex hex, int dir, bool[] walls)
        {
            return dir < 3 ? walls[3 * sw * (hex.Q + Size) + 3 * (hex.R + Size) + dir] : hasWall(hex.GetNeighbor(dir), dir - 3, walls);
        }

        private static SubmazeValidity DetermineSubmazeValidity(Hex centerHex, bool[] walls)
        {
            var ret = new SubmazeValidity();
            ret.Filled = new HashSet<Hex> { centerHex };
            var q = new Queue<Hex>();
            q.Enqueue(centerHex);
            ret.EdgesReachable = new bool[6];

            // Flood-fill as much of the maze as possible
            while (q.Count > 0)
            {
                var hex = q.Dequeue();
                var neighbors = hex.Neighbors;
                for (int dir = 0; dir < 6; dir++)
                {
                    var offset = neighbors[dir] - centerHex;
                    if (offset.Distance < SubmazeSize && !hasWall(hex, dir, walls) && ret.Filled.Add(neighbors[dir]))
                        q.Enqueue(neighbors[dir]);
                    if (offset.Distance == SubmazeSize && !hasWall(hex, dir, walls))
                        foreach (var edge in offset.GetEdges(SubmazeSize))
                            ret.EdgesReachable[edge] = true;
                }
            }

            ret.IsValid =
                // All hexes filled?
                (ret.Filled.Count >= 3 * SubmazeSize * (SubmazeSize - 1) + 1) &&
                // All edges reachable?
                !ret.EdgesReachable.Contains(false);
            return ret;
        }

        private static bool areMarkingsUnique(Marking[] markings)
        {
            var unique = new HashSet<string>();
            foreach (var centerHex in Hex.LargeHexagon(Size - SubmazeSize + 1))
                for (int rotation = 0; rotation < 6; rotation++)
                    if (!unique.Add(Hex.LargeHexagon(SubmazeSize).Select(h => (int) markings[markingIndex(h.Rotate(rotation) + centerHex)].Rotate(-rotation)).Join("")))
                        return false;
            return true;
        }
    }
}
