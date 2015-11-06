using System;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace Krowiorsch.Lucene.Filter
{
    public class ApostopheFilter : TokenFilter
    {
        readonly string[] _apothropheSymbols = { "'s", "`s", "´s" };
        readonly ITermAttribute _termAttribute;

        public ApostopheFilter(TokenStream input)
            : base(input)
        {
            _termAttribute = AddAttribute<ITermAttribute>();
        }

        public override bool IncrementToken()
        {
            while (input.IncrementToken())
            {
                var term = _termAttribute.Term;

                // found some apothrophe
                if (term.Length > 2 && _apothropheSymbols.Any(s => term.EndsWith(s, StringComparison.OrdinalIgnoreCase)))
                {
                    _termAttribute.SetTermBuffer(term.Substring(0, term.Length - 2));
                    _termAttribute.SetTermLength(term.Length - 2);
                }

                return true;
            }


            return false;
        }
    }
}