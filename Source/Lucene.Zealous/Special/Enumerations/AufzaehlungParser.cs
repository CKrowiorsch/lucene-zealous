using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LandauMedia.CommandCenter.Infrastructure.TextSearchAlgorithm.Analyzers.Filters.Aufzaehlung
{
    public class AufzaehlungParser
    {
        readonly string[] _seperatorSymbols = { "und", "oder", ";", ",", "sowie" };
        readonly string[] _nonImportantWords = { "der", "die", "das", "ihre", "ihr", "den", "des", "dessen", "sein" };

        readonly char[] _blockChars = { ':' };

        public IEnumerable<Token> Parse(string input)
        {
            if (string.IsNullOrEmpty(input))
                return Enumerable.Empty<Token>();

            return Lexer(input).Select(t =>
            {
                t.Symbol = GetSymbolOfToken(t.Term);
                return t;
            });
        }

        string GetSymbolOfToken(string term)
        {
            if (string.IsNullOrEmpty(term))
                return Token.KnownSymbols.Undefinied;

            var first = term[0];

            if (char.IsUpper(first) || char.IsDigit(first) || first == '\"' || first == '\'')
            {
                return _blockChars.Any(c => term.EndsWith(c + "")) ? Token.KnownSymbols.Block : Token.KnownSymbols.Substantiv;
            }

            if (_blockChars.Any(c => term.EndsWith(c + "")))
                return Token.KnownSymbols.Block;

            if (_nonImportantWords.Any(t => t.Equals(term, StringComparison.OrdinalIgnoreCase)))
                return Token.KnownSymbols.NonImportant;

            if (_seperatorSymbols.Any(t => t.Equals(term, StringComparison.OrdinalIgnoreCase)))
                return Token.KnownSymbols.Seperator;

            return Token.KnownSymbols.Undefinied;
        }

        IEnumerable<Token> Lexer(string input)
        {
            var lastStart = 0;
            var hasContent = false;
            var sb = new StringBuilder();

            var position = 0;

            for (var i = 0; i < input.Length; i++)
            {
                if (input[i] == ',' || input[i] == ';')
                {
                    if (hasContent)
                    {
                        yield return new Token { Start = lastStart + 1, End = i, Term = sb.ToString().Trim(), Position = position++ };
                        sb.Clear();
                    }

                    yield return new Token { Start = i, End = i + 1, Term = ",", Position = position++ };
                    lastStart = i;
                    hasContent = false;
                }
                else if (char.IsWhiteSpace(input[i]) && hasContent)
                {
                    yield return new Token { Start = lastStart + 1, End = i, Term = sb.ToString().Trim(), Position = position++ };
                    sb.Clear();
                    lastStart = i;
                    hasContent = false;
                }
                else
                {
                    hasContent = true;
                    sb.Append(input[i]);
                }
            }

            if (hasContent)
                yield return new Token { Start = lastStart + 1, End = input.Length, Term = sb.ToString().Trim(), Position = position };
        }

        public class Token
        {
            public string Term { get; set; }

            public int Position { get; set; }

            public int Start { get; set; }
            public int End { get; set; }

            public string Symbol { get; set; }

            public class KnownSymbols
            {
                public const string Seperator = "seperator";
                public const string Substantiv = "substantiv";

                /// <summary> nach diesen Symbolen geht die Aufzaehlung nie weiter </summary>
                public const string Block = "block";

                /// <summary> diese Worter können bei der Aufzählung ignoriert werden</summary>
                public const string NonImportant = "NonImportant";


                public const string Undefinied = "undefinied";
            }
        }

    }
}