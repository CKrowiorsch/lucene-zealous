using System;
using System.IO;
using System.Linq;
using LandauMedia.CommandCenter.Infrastructure.TextSearchAlgorithm.Analyzers.Filters.Aufzaehlung;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Util;
using Krowiorsch.Lucene;

namespace LandauMedia.CommandCenter.Infrastructure.TextSearchAlgorithm.Analyzers.Filters
{
    /// <summary>  </summary>
    public class ContextTokenizer : LetterDigitSpecialCharacterTokenizer
    {
        Tuple<int, int>[] _enumerationPositions;
        readonly AufzaehlungDetector _aufzaehlungDetector = new AufzaehlungDetector();

        IOffsetAttribute _offsetAttribute;
        IFlagsAttribute _flagsAttribute;

        string _content;
        bool _isInitialized;

        public ContextTokenizer(string content)
            : base(new StringReader(content))
        {
            _content = content;
        }

        public ContextTokenizer(AttributeSource source, TextReader input)
            : base(source, input)
        {
            throw new NotImplementedException();
        }

        public ContextTokenizer(AttributeFactory factory, TextReader input)
            : base(factory, input)
        {
            throw new NotImplementedException();
        }

        public override bool IncrementToken()
        {
            if (!_isInitialized)
                Init(_content);

            if (base.IncrementToken())
            {
                if (IsTokenInSpan(_offsetAttribute.StartOffset))
                    _flagsAttribute.Flags = KnownContextElements.AufzaehlungFlag;

                return true;
            }

            return false;
        }

        void Init(string content)
        {
            _enumerationPositions = _aufzaehlungDetector.FindAufzaehlungsspans(content).ToArray();
            _offsetAttribute = AddAttribute<IOffsetAttribute>();
            _flagsAttribute = AddAttribute<IFlagsAttribute>();

            _isInitialized = true;
        }

        bool IsTokenInSpan(int startOffset)
        {
            return _enumerationPositions.Any(i => startOffset >= i.Item1 && startOffset <= i.Item2);
        }
    }
}