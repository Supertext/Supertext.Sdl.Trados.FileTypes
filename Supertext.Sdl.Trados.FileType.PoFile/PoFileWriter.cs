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
        private IExtendedStreamReader _extendedStreamReader;
        private IStreamWriter _streamWriter;

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
            _extendedStreamReader = _fileHelper.GetExtendedStreamReader(_originalFileProperties.OriginalFilePath);
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
            var messageStringStart = int.Parse(contextInfo.GetMetaData(ContextKeys.MessageStringStart));
            var messageStringEnd = int.Parse(contextInfo.GetMetaData(ContextKeys.MessageStringEnd));

            WriteOriginalLinesUntil(messageStringStart);

            _streamWriter.WriteLine("msgstr \"" + _segmentReader.GetTargetText(paragraphUnit.SegmentPairs) + "\"");

            MoveTo(messageStringEnd);
        }

        private void WriteOriginalLinesUntil(int lineNumber)
        {
            string currentInputLine;
            while ((currentInputLine = _extendedStreamReader.ReadLineWithEofLine()) != null)
            {

                if (_extendedStreamReader.CurrentLineNumber < lineNumber)
                {
                    _streamWriter.WriteLine(currentInputLine);
                }
                else
                {
                    break;
                }
            }
        }

        private void MoveTo(int lineNumber)
        {
            do
            {
                if (_extendedStreamReader.CurrentLineNumber < lineNumber)
                {
                    continue;
                }

                break;
            } while (_extendedStreamReader.ReadLineWithEofLine() != null);
        }

        public void Complete()
        {
        }

        public void FileComplete()
        {
            _extendedStreamReader.Close();
            _extendedStreamReader.Dispose();
            _streamWriter.Close();
            _streamWriter.Dispose();
        }

        public void Dispose()
        {
        }
    }
}