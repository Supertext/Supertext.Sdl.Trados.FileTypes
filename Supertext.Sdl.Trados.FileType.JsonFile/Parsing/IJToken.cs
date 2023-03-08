using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Parsing
{
    public interface IJToken
    {
        IJToken SelectToken(string path);

        JTokenType Type { get; }

        IJValue Value { get; }

        string ToString(Formatting formatting = Formatting.None);
    }
}