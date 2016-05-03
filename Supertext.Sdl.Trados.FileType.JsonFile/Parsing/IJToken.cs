using Newtonsoft.Json.Linq;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Parsing
{
    public interface IJToken
    {
        IJToken SelectToken(string path);

        JTokenType Type { get; }

        IJProperty Parent { get; }

        string ToString();
    }

    public interface IJProperty
    {
        string Name { get; }

        void Replace(string name, string value);
    }
}