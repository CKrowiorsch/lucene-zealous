using LandauMedia.CommandCenter.Infrastructure.TextSearchAlgorithm.Analyzers.Filters.Aufzaehlung;

namespace Krowiorsch.Lucene.Special.Enumerations
{
    class with_Aufzaehlungsparser
    {
        protected static void SetupFilter(string text)
        {
            _sut = new AufzaehlungDetector();
        }

        protected static AufzaehlungDetector _sut;
    }
}
