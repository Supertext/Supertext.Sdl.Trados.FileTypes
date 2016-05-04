using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Parsing
{
    internal class JsonFactory : IJsonFactory
    {
        public IJsonTextReader CreateJsonTextReader(string path)
        {
            var file = File.OpenText(path);
            return new JsonTextReaderWrapper(file, new JsonTextReader(file));
        }

        public IJToken GetRootToken(string file)
        {
            var input = File.ReadAllText(file);
            var token = JToken.Parse(input);
            return new JTokenWrapper(token);
        }
    }
}