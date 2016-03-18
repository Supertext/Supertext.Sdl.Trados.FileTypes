using System.Collections.Generic;

namespace Supertext.Sdl.Trados.FileType.PoFile.FileHandling
{

    public class ExtendedStreamReader : IExtendedStreamReader
    {
        private readonly IStreamReader _streamReader;
        private bool _isEndReached;

        public ExtendedStreamReader(IStreamReader streamReader)
        {
            _streamReader = streamReader;
        }

        public string ReadLineWithEofLine()
        {
            var line = _streamReader.ReadLine();

            if (line != null || _isEndReached)
            {
                return line;
            }

            _isEndReached = true;
            return MarkerLines.EndOfFile;
        }

        public IEnumerable<string> GetLinesWithEofLine()
        {
            string line;

            while ((line = _streamReader.ReadLine()) != null)
            {
                yield return line;
            }

            yield return MarkerLines.EndOfFile;
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
