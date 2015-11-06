using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using Machine.Specifications;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local



namespace Krowiorsch.Lucene.Filter
{
    public class with_apothropheFilter
    {
        protected static void SetupFilter(string text)
        {
            _sut = new ApostopheFilter(new WhitespaceTokenizer(new StringReader(text))); 
        }

        protected static IEnumerable<string> ReadTermsFromFilter(TokenFilter filter)
        {
            var term = filter.GetAttribute<ITermAttribute>();

            while(filter.IncrementToken())
                yield return term.Term;
        }

        protected static IEnumerable<int> ReadPositionsFromFilter(TokenFilter filter)
        {
            var position = filter.GetAttribute<IPositionIncrementAttribute>();

            while (filter.IncrementToken())
                yield return position.PositionIncrement;
        }

        protected static ApostopheFilter _sut;
    }

    public class when_have_a_standard_apothrophe : with_apothropheFilter
    {
        Establish context = () =>
            SetupFilter("peter's");

        Because of = () =>
            _result = ReadTermsFromFilter(_sut).ToArray();

        It should_have_one_token_with_peter = () =>
            _result.ShouldContainOnly("peter");

        static string[] _result;
    }
}