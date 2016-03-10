namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface ILineParser
    {
        ILineValidationSession StartLineValidationSession();

        ILineParsingSession StartLineParsingSession();
    }
}