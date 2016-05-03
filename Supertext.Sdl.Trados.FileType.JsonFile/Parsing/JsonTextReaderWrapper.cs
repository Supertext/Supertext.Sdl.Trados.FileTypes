using System.IO;
using Newtonsoft.Json;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Parsing
{
    public class JsonTextReaderWrapper : IJsonTextReader
    {
        private readonly TextReader _file;
        private readonly JsonTextReader _jsonTextReader;

        public JsonTextReaderWrapper(TextReader file, JsonTextReader jsonTextReader)
        {
            _file = file;
            _jsonTextReader = jsonTextReader;
        }

        public bool Read()
        {
            return _jsonTextReader.Read();
        }

        public int LineNumber
        {
            get { return _jsonTextReader.LineNumber; }
        }

        public string Path
        {
            get { return _jsonTextReader.Path; }
        }

        public object Value
        {
            get
            {
                return _jsonTextReader.Value;
            }
           
        }

        public JsonToken TokenType
        {
            get
            {
                return _jsonTextReader.TokenType;
            }
        }

        public void Close()
        {
            _jsonTextReader.Close();
        }

        public void Dispose()
        {
            _file.Dispose();
        }
    }
}