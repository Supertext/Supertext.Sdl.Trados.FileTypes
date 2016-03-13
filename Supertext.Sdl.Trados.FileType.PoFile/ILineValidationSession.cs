namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface ILineValidationSession
    {
        bool IsValid(string line);

        string NextExpectedLineDescription { get; }
    }
}