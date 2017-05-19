namespace Supertext.Sdl.Trados.FileType.Utils.TextProcessing
{
    public interface ISegmentDataCollector
    {
        SegmentData CompleteSegmentData { get; set; }

        void Add(string path, string value);
    }
}