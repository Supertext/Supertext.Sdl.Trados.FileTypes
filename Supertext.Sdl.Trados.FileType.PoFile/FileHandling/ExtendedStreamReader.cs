using System.Collections.Generic;

namespace Supertext.Sdl.Trados.FileType.PoFile.FileHandling
{
    //Todo: maybe refactor with introducing line class with number and content
    public class ExtendedStreamReader : IExtendedStreamReader
    {
        private readonly IStreamReader _streamReader;
        private bool _isEndReached;

        public ExtendedStreamReader(IStreamReader streamReader)
        {
            _streamReader = streamReader;
        }

        public int CurrentLineNumber { get; private set; }

        public string ReadLineWithEofLine()
        {
            var line = _streamReader.ReadLine();

            if (line == null && !_isEndReached)
            {
                line = MarkerLines.EndOfFile;
                _isEndReached = true;
            }

            if (line != null)
            {
                ++CurrentLineNumber;
            }

            return line;
        }

        public IEnumerable<string> GetLinesWithEofLine()
        {
            string line;

            while ((line = _streamReader.ReadLine()) != null)
            {
                ++CurrentLineNumber;
                yield return line;
            }

            ++CurrentLineNumber;
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
