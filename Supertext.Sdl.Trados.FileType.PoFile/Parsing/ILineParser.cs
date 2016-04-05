namespace Supertext.Sdl.Trados.FileType.PoFile.Parsing
{
    public interface ILineParser
    {
        ILineValidationSession StartLineValidationSession();

        ILineParsingSession StartLineParsingSession();
    }
}