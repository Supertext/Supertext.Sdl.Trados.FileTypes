using Newtonsoft.Json.Linq;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Parsing
{
    public interface IJsonFactory
    {
        IJsonTextReader CreateJsonTextReader(string file);

        IJToken GetRootToken(string file);
    }
}
