using System.Collections.Generic;
using System.IO;
using System.Linq;
using Krowiorsch.Lucene.Exentsions;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using Machine.Specifications;
using Token = Krowiorsch.Lucene.Model.Token;

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

        protected static ApostopheFilter _sut;
    }

    public class when_have_a_standard_apothrophe : with_apothropheFilter
    {
        Establish context = () =>
            SetupFilter("peter's");

        Because of = () =>
            _result = _sut.ReadTokens().ToArray();

        It should_have_one_token_with_peter = () =>
            _result.Select(t => t.Term).ShouldContainOnly("peter");

        It should_have_the_startoffset_of_zero = () =>
            _result.ElementAt(0).StartOffset.ShouldEqual(0);

        It should_have_the_endoffset_of_the_complete_term = () =>
            _result.ElementAt(0).EndOffset.ShouldEqual(7);

        static Token[] _result;
    }


}