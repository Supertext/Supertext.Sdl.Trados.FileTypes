namespace Supertext.Sdl.Trados.FileType.PoFile.Parsing
{
    public interface IEntryBuilder
    {
        Entry CompleteEntry { get; }

        void Add(IParseResult parseResult, int lineNumber);
    }
}