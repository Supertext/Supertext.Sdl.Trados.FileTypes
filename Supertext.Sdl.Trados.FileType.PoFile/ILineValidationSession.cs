namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface ILineValidationSession
    {
        bool Check(string line);

        bool IsEndValid();

        string NextMandatoryLinePattern { get; }
    }
}