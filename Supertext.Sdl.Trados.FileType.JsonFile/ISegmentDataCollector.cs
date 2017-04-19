namespace Supertext.Sdl.Trados.FileType.JsonFile
{
    public interface ISegmentDataCollector
    {
        SegmentData CompleteSegmentData { get; set; }

        void Add(string path, string value);
    }
}