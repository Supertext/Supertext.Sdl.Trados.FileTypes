using System.Collections.Generic;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface IEofLineFileReader
    {
        IEnumerable<string> ReadLines(string filePath);
    }

    public class EofLineFileReader : IEofLineFileReader
    {
        private readonly IFileHelper _fileHelper;

        public EofLineFileReader(IFileHelper fileHelper)
        {
            _fileHelper = fileHelper;
        }

        public IEnumerable<string> ReadLines(string filePath)
        {
            foreach (var line in _fileHelper.ReadLines(filePath))
            {
                yield return line;
            }

            yield return LineType.EndOfFile.ToString();
        }
    }
}