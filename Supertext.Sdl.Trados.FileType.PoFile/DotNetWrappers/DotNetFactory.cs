namespace Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers
{
    public class DotNetFactory : IDotNetFactory
    {
        public IStreamReader CreateStreamReader(string filePath)
        {
            return new StreamReaderWrapper(filePath);
        }
    }
}