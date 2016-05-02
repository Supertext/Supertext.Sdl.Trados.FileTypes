using System.IO;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Parsing
{
    public class JsonFactory : IJsonFactory
    {
        public IJsonTextReader CreateJsonTextReader(string path)
        {
            var file = File.OpenText(path);
            return new JsonTextReaderWrapper(file);
        }
    }
}