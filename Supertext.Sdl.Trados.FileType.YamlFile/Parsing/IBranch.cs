namespace Supertext.Sdl.Trados.FileType.YamlFile.Parsing
{
    public interface IBranch
    {
        bool IsComplete { get; }

        void SetScalar(string value);

        void Continue();

        string GetSubPath();
    }
}