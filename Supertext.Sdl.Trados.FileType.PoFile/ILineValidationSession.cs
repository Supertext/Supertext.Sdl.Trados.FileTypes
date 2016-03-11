namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface ILineValidationSession
    {
        bool IsValid(string line);

        bool IsEndValid();

        string NextExpectedLineDescription { get; }
    }
}