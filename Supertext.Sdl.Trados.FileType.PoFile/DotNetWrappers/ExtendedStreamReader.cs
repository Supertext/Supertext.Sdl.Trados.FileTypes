namespace Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers
{

    public class ExtendedStreamReader : IExtendedStreamReader, IStreamReader
    {
        private readonly IStreamReader _streamReader;
        private bool _isEndReached;

        public ExtendedStreamReader(IStreamReader streamReader)
        {
            _streamReader = streamReader;
        }

        public string ReadLine()
        {
            var line = _streamReader.ReadLine();

            if (line != null || _isEndReached)
            {
                return line;
            }

            _isEndReached = true;
            return MarkerLines.EndOfFile;
        }

        public void Close()
        {
            _streamReader.Close();
        }

        public void Dispose()
        {
            _streamReader.Dispose();
        }
    }
}
