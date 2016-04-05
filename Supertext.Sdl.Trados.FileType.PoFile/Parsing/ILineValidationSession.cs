namespace Supertext.Sdl.Trados.FileType.PoFile.Parsing
{
    public interface ILineValidationSession
    {
        bool IsValid(string line);

        string NextExpectedLineDescription { get; }
    }
}