using System.IO;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Util;

namespace LandauMedia.CommandCenter.Infrastructure.TextSearchAlgorithm.Analyzers
{
    public class LetterDigitSpecialCharacterTokenizer : CharTokenizer
    {
        readonly char[] _specialChacters = { '-', '+', '*', '@', '_', '\'', '^', '&' };

        public LetterDigitSpecialCharacterTokenizer(TextReader input)
            : base(input)
        {
        }

        public LetterDigitSpecialCharacterTokenizer(AttributeSource source, TextReader input)
            : base(source, input)
        {
        }

        public LetterDigitSpecialCharacterTokenizer(AttributeFactory factory, TextReader input)
            : base(factory, input)
        {
        }

        protected override bool IsTokenChar(char c)
        {
            return char.IsLetterOrDigit(c) || _specialChacters.Contains(c);
        }
    }
}