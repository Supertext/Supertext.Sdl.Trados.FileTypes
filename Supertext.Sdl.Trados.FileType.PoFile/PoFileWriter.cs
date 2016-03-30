using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class PoFileWriter : AbstractBilingualFileTypeComponent, IBilingualWriter, INativeOutputSettingsAware
    {
        private readonly IFileHelper _fileHelper;
        private readonly ISegmentReader _segmentReader;
        private IPersistentFileConversionProperties _originalFileProperties;
        private INativeOutputFileProperties _nativeFileProperties;
        private IStreamReader _streamReader;
        private IStreamWriter _streamWriter;
        private int _currentInputLineNumber;

        public PoFileWriter(IFileHelper fileHelper, ISegmentReader segmentReader)
        {
            _fileHelper = fileHelper;
            _segmentReader = segmentReader;
        }

        public void Initialize(IDocumentProperties documentInfo)
        {
        }

        public void SetFileProperties(IFileProperties fileInfo)
        {
            _streamReader = _fileHelper.GetStreamReader(_originalFileProperties.OriginalFilePath);
            _streamWriter = _fileHelper.GetStreamWriter(_nativeFileProperties.OutputFilePath);
        }

        public void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties, IOutputFileInfo proposedFileInfo)
        {
            _originalFileProperties = fileProperties;
        }

        public void SetOutputProperties(INativeOutputFileProperties properties)
        {
            _nativeFileProperties = properties;
        }

        public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            var contextInfo = paragraphUnit.Properties.Contexts.Contexts[1];
            var messageIdEnd = int.Parse(contextInfo.GetMetaData(ContextKeys.MessageIdEnd));
            var messageStringEnd = int.Parse(contextInfo.GetMetaData(ContextKeys.MessageStringEnd));

            WriteOriginalLinesUntil(messageIdEnd);

            _streamWriter.WriteLine("msgstr \"" + _segmentReader.GetTargetText(paragraphUnit.SegmentPairs) + "\"");

            MoveTo(messageStringEnd);
        }

        private void WriteOriginalLinesUntil(int messageIdEnd)
        {
            string currentInputLine;
            while ((currentInputLine = _streamReader.ReadLine()) != null)
            {
                ++_currentInputLineNumber;

                if (_currentInputLineNumber <= messageIdEnd)
                {
                    _streamWriter.WriteLine(currentInputLine);
                }
                else
                {
                    break;
                }
            }
        }

        private void MoveTo(int messageStringEnd)
        {
            do
            {
                if (_currentInputLineNumber < messageStringEnd)
                {
                    ++_currentInputLineNumber;
                    continue;
                }

                break;
            } while (_streamReader.ReadLine() != null);
        }

        public void Complete()
        {
        }

        public void FileComplete()
        {
            _streamReader.Close();
            _streamReader.Dispose();
            _streamWriter.Close();
            _streamWriter.Dispose();
        }

        public void Dispose()
        {
        }
    }
}