using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using Token = Krowiorsch.Lucene.Model.Token;

namespace Krowiorsch.Lucene.Exentsions
{
    static class TokenFilterExtentions
    {
        internal static IEnumerable<Token> ReadTokens(this TokenFilter filter)
        {
            var termAttribute = filter.GetAttribute<ITermAttribute>();

            var positionAttribute = filter.HasAttribute<IPositionIncrementAttribute>()
                ? filter.GetAttribute<IPositionIncrementAttribute>()
                : null;

            var offsetAttribute = filter.GetAttribute<IOffsetAttribute>();

            while(filter.IncrementToken())
            {
                yield return new Token
                {
                    Term = termAttribute.Term,
                    PositionIncrement = positionAttribute != null ? positionAttribute.PositionIncrement : 0,
                    StartOffset = offsetAttribute.StartOffset,
                    EndOffset = offsetAttribute.EndOffset
                };
            }
        } 
    }
}