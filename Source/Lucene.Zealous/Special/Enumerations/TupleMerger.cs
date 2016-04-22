using System;
using System.Collections.Generic;
using System.Linq;

namespace LandauMedia.CommandCenter.Infrastructure.TextSearchAlgorithm.Analyzers.Filters.Aufzaehlung
{
    internal static class TupleMerger
    {
        const int OverlappingSlop = 2;

        public static List<Tuple<int, int>> Merge(List<Tuple<int, int>> positions)
        {
            var ordered = positions.OrderBy(t => t.Item1).ToList();

            bool changed = true;
            while (changed)
            {
                changed = false;

                for (int i = 0; i < ordered.Count() - 1; i++)
                {
                    if (!IsOverlapping(ordered[i], ordered[i + 1]))
                        continue;

                    // merge fragments
                    ordered[i] = AppendAfter(ordered[i], ordered[i + 1]);
                    ordered.RemoveAt(i + 1);

                    changed = true;
                    break;
                }
            }

            return ordered;
        }

        static Tuple<int, int> AppendAfter(Tuple<int, int> tupleOne, Tuple<int, int> tupleTwo)
        {
            if (tupleOne.Item1 > tupleTwo.Item1)
                throw new ArgumentException("matches must follow one another");

            var newLength = Math.Max(tupleTwo.Item2 - tupleOne.Item1, tupleOne.Item2 - tupleTwo.Item1);

            if (newLength > 0)
                return Tuple.Create(tupleOne.Item1, tupleTwo.Item2);

            return tupleOne;
        }

        static bool IsOverlapping(Tuple<int, int> tupleOne, Tuple<int, int> tupleTwo)
        {
            return tupleOne.Item2 + OverlappingSlop > tupleTwo.Item1;
        }
    }
}