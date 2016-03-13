using System.Collections.Generic;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface IExtendedFileReader
    {
        IEnumerable<string> ReadLines(string filePath);
    }

    public class ExtendedFileReader : IExtendedFileReader
    {
        private readonly IFileHelper _fileHelper;

        public ExtendedFileReader(IFileHelper fileHelper)
        {
            _fileHelper = fileHelper;
        }

        public IEnumerable<string> ReadLines(string filePath)
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