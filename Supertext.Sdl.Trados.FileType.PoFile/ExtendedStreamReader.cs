using System.Collections.Generic;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface IExtendedStreamReader
    {
        int GetTotalNumberOfLines(string path);

        IEnumerable<string> GetLinesWithEofLine(string path);
    }

    public class ExtendedStreamReader : IExtendedStreamReader
    {
        private readonly IFileHelper _fileHelper;

        public ExtendedStreamReader(IFileHelper fileHelper)
        {
            _fileHelper = fileHelper;
        }

        public int GetTotalNumberOfLines(string path)
        {
            return _fileHelper.GetTotalNumberOfLines(path) + 1;
        }

        public IEnumerable<string> GetLinesWithEofLine(string filePath)
        {
            using (var streamReader = _fileHelper.GetStreamReader(filePath))
            {
                string line;

                while ((line = streamReader.ReadLine()) != null)
                {
                    yield return line;
                }

                yield return MarkerLines.EndOfFile;
            }
        }
    }
}