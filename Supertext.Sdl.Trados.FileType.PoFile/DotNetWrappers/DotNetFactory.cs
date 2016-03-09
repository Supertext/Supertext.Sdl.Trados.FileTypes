namespace Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers
{
    public class DotNetFactory : IDotNetFactory
    {
        public IReader CreateStreamReader(string filePath)
        {
            return new StreamReaderWrapper(filePath);
        }
    }
}