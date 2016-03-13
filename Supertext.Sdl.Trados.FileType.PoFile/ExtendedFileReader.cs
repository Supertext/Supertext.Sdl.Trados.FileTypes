using System.Collections.Generic;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface IExtendedFileReader
    {
        int GetTotalNumberOfLines(string path);

        IEnumerable<string> ReadLinesWithEofLine(string path);
    }

    public class ExtendedFileReader : IExtendedFileReader
    {
        private readonly IFileHelper _fileHelper;

        public ExtendedFileReader(IFileHelper fileHelper)
        {
            _fileHelper = fileHelper;
        }

        public int GetTotalNumberOfLines(string path)
        {
            return _fileHelper.GetTotalNumberOfLines(path) + 1;
        }

        public IEnumerable<string> ReadLinesWithEofLine(string filePath)
        {
            yield return LineType.BeginOfFile.ToString();

            foreach (var line in _fileHelper.ReadLines(filePath))
            {
                yield return line;
            }

            yield return LineType.EndOfFile.ToString();
        }
    }
}