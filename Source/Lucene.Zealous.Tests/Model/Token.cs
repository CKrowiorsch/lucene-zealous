namespace Krowiorsch.Lucene.Model
{
    /// <summary> only for tetsing purpose </summary>
    internal class Token
    {
        public string Term { get; set; } 
        public int PositionIncrement { get; set; }
        public int StartOffset { get; set; }
        public int EndOffset { get; set; }
    }
}