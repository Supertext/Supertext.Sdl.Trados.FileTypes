using System;

namespace Supertext.Sdl.Trados.FileType.YamlFile.Parsing
{
    public interface IYamlTextReader : IDisposable
    {
        int LineNumber { get; }

        string Path { get; }

        string Value { get; }

        bool Read();

        void Close();
    }
}