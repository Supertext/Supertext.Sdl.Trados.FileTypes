using System;
using Newtonsoft.Json;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Parsing
{
    public interface IJsonTextReader : IDisposable
    {
        bool Read();

        int LineNumber { get; }

        string Path { get;}

        object Value { get; }

        JsonToken TokenType { get; }

        void Close();
    }
}