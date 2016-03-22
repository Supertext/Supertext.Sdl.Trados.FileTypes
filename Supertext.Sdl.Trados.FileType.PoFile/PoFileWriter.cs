using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class PoFileWriter : AbstractBilingualFileTypeComponent, IBilingualWriter, INativeOutputSettingsAware
    {
        private readonly IFileHelper _fileHelper;
        private IPersistentFileConversionProperties _originalFileProperties;
        private INativeOutputFileProperties _nativeFileProperties;
        private IStreamReader _streamReader;
        private IStreamWriter _streamWriter;

        public PoFileWriter(IFileHelper fileHelper)
        {
            _fileHelper = fileHelper;
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
            var msgidPosition = int.Parse(paragraphUnit.Properties.Contexts.Contexts[1].GetMetaData("msgidPosition"));

            var currentInputLineNumber = 0;
            string currentInputLine;
            while ((currentInputLine = _streamReader.ReadLine()) != null)
            {
                ++currentInputLineNumber;

                if (currentInputLineNumber < msgidPosition)
                {
                    _streamWriter.WriteLine(currentInputLine);
                }
            }
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