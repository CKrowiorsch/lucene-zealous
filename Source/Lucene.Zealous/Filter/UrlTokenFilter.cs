using System;
using System.Text.RegularExpressions;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace Krowiorsch.Lucene.Filter
{
    /// <summary> gibt nur den Domainteil einer Url als Token zurück 
    /// http://www.google.de/query/something -> www.google.de
    /// </summary>
    public sealed class UrlTokenFilter : TokenFilter
    {
        readonly Regex _domain = new Regex(@"\.[A-Za-z]{2,8}([\s\/]|$)", RegexOptions.IgnoreCase);
        readonly ITermAttribute _termAttribute;

        const string StandardProtocol = "http://";

        public UrlTokenFilter(TokenStream input)
            : base(input)
        {
            _termAttribute = AddAttribute<ITermAttribute>();
        }
        public override bool IncrementToken()
        {

            while (input.IncrementToken())
            {
                var term = _termAttribute.Term;

                if (TermIsUrl(term))
                {
                    var domainTerm = ConcatUrl(term);

                    _termAttribute.ResizeTermBuffer(domainTerm.Length);
                    _termAttribute.SetTermBuffer(domainTerm);

                    return true;
                }
            }

            return false;
        }

        static string ConcatUrl(string url)
        {
            //TODO: das muss besser gehen -> schreibe eigenen URL-Parser für partielle URLs
            var analyzedUrl = url;

            if (!url.Contains("://"))
                analyzedUrl = StandardProtocol + url;

            try
            {
                var uri = new Uri(analyzedUrl, UriKind.RelativeOrAbsolute);
                return uri.DnsSafeHost;
            }
            catch
            {
                return url;
            }

        }

        bool TermIsUrl(string term)
        {
            return _domain.IsMatch(term);
        }
    }
}