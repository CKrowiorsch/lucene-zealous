using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace Krowiorsch.Lucene.Filter
{
    /// <summary> filters filters every token that have a specific flag</summary>
    public sealed class FlagFilter : TokenFilter
    {
        readonly IFlagsAttribute _flagsAttribute;
        readonly int _flags;

        public FlagFilter(int flags, TokenStream input)
            : base(input)
        {
            _flagsAttribute = AddAttribute<IFlagsAttribute>();
            _flags = flags;
        }

        public override bool IncrementToken()
        {
            while (input.IncrementToken())
            {
                // when flag is set -> discard
                if ((_flagsAttribute.Flags & _flags) == _flags)
                    continue;

                return true;
            }

            return false;
        }
    }
}