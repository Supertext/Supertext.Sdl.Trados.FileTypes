using System;

namespace Supertext.Sdl.Trados.FileType.YamlFile.Parsing
{
    public interface IYamlTextWriter: IDisposable
    {
        void Write(string path, string value);

        void FinishWriting();
    }
}