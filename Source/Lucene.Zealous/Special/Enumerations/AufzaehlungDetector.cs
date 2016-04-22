using System;
using System.Collections.Generic;
using System.Linq;

namespace LandauMedia.CommandCenter.Infrastructure.TextSearchAlgorithm.Analyzers.Filters.Aufzaehlung
{
    public class AufzaehlungDetector
    {
        const int MaxSteps = 4;

        readonly AufzaehlungParser _parser = new AufzaehlungParser();

        public IEnumerable<Tuple<int, int>> FindAufzaehlungsspans(string input)
        {

            var tokens = _parser.Parse(input).ToArray();

            var positions = new List<Tuple<int, int>>();

            var currentPosition = 0;
            var lastPosition = 0;
            while ((currentPosition = FindNextSeperator(tokens, currentPosition + 1)) >= 0)
            {
                var currentTuple = FindEnumerationSurround(tokens, currentPosition, lastPosition, out currentPosition);

                if (currentTuple != null)
                    positions.Add(currentTuple);

                if (currentPosition == -1)
                    break;

                if (lastPosition == currentPosition)
                    break;

                lastPosition = currentPosition;
            }

            return TupleMerger.Merge(positions);
        }

        int FindNextSeperator(AufzaehlungParser.Token[] tokens, int start)
        {
            for (int i = start; i < tokens.Length; i++)
            {
                if (tokens[i].Symbol == AufzaehlungParser.Token.KnownSymbols.Seperator)
                    return i;
            }

            return -1;
        }

        Tuple<int, int> FindEnumerationSurround(AufzaehlungParser.Token[] tokens, int position, int minPosition, out int endPosition)
        {
            // finde das nächste nicht Substantiv vor dem zeichen
            // Substantiv = "Wort wo erster Buchstabe gross oder Zahl"

            var lookBackwards = false;
            var lookForewards = false;

            var startPos = LookBackwards(tokens, position, minPosition, out lookBackwards);
            var endPos = LookForward(tokens, position, out endPosition, out lookForewards);

            if (!lookBackwards || !lookForewards)
                return null;

            return Tuple.Create(startPos, endPos);
        }

        int LookForward(AufzaehlungParser.Token[] tokens, int position, out int endPosition, out bool foundSomething)
        {
            var relativPosition = 0;
            foundSomething = false;

            for (int i = position + 1; i < Math.Min(position + 1 + MaxSteps, tokens.Count()); i++, relativPosition++)
            {
                var symbol = tokens[i].Symbol;

                if (relativPosition == 0 && symbol == AufzaehlungParser.Token.KnownSymbols.NonImportant)
                    continue;

                if (symbol == AufzaehlungParser.Token.KnownSymbols.Substantiv)
                    foundSomething = true;

                if (symbol == AufzaehlungParser.Token.KnownSymbols.Substantiv && relativPosition <= MaxSteps)
                    continue;


                endPosition = position + relativPosition;
                return tokens[endPosition].End;
            }

            endPosition = position + relativPosition;
            return tokens[endPosition].End;
        }

        int LookBackwards(AufzaehlungParser.Token[] tokens, int position, int minPosition, out bool foundSomething)
        {
            var relativPosition = 0;
            foundSomething = false;

            for (int i = position - 1; i > Math.Max(position - 1 - MaxSteps, 0); i--, relativPosition++)
            {
                var symbol = tokens[i].Symbol;

                if (relativPosition == 0 && symbol == AufzaehlungParser.Token.KnownSymbols.NonImportant)
                    continue;

                if (symbol == AufzaehlungParser.Token.KnownSymbols.Block)
                    break;

                if (symbol == AufzaehlungParser.Token.KnownSymbols.Substantiv)
                    foundSomething = true;

                if (symbol == AufzaehlungParser.Token.KnownSymbols.Substantiv && i > minPosition)
                    continue;

                return tokens[i + 1].Start;
            }

            return tokens[position - relativPosition].Start;
        }
    }

}