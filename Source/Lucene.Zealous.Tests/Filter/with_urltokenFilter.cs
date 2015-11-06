using System.Collections.Generic;
using System.IO;
using System.Linq;
using Krowiorsch.Lucene.Exentsions;
using Lucene.Net.Analysis;
using Machine.Specifications;
using Token = Krowiorsch.Lucene.Model.Token;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local



namespace Krowiorsch.Lucene.Filter
{
    class with_urltokenFilter
    {
        protected static void SetupFilter(string text)
        {
            _tokenfilter = new UrlTokenFilter(new WhitespaceTokenizer(new StringReader(text)));
        }

        Because of = () => _result = _tokenfilter.ReadTokens();

        protected static UrlTokenFilter _tokenfilter;
        protected static IEnumerable<Token> _result;
    }

    class when_filter_a_non_url_token : with_urltokenFilter
    {
        Establish context = () => SetupFilter("text without any url");
        It should_not_have_any_token = () => _result.ShouldBeEmpty();
    }

    class when_filter_a_text_with_a_url_without_protocol : with_urltokenFilter
    {
        Establish context = () => SetupFilter("text with simple url: www.google.de");
        It should_have_token_with_entry_www_google_de = () => _result.Select(t => t.Term).ShouldContainOnly("www.google.de");
    }
}