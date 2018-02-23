using System.Linq;
using Newtonsoft.Json.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.JsonFile.Parsing;
using Supertext.Sdl.Trados.FileType.JsonFile.TextProcessing;
using Supertext.Sdl.Trados.FileType.Utils.FileHandling;
using Supertext.Sdl.Trados.FileType.Utils.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.JsonFile
{
    public class JsonFileWriter : AbstractBilingualFileTypeComponent, IBilingualWriter, INativeOutputSettingsAware
    {
        private readonly IFileHelper _fileHelper;
        private readonly IJsonFactory _jsonFactory;
        private readonly ISegmentReader _segmentReader;
        private INativeOutputFileProperties _nativeFileProperties;
        private IPersistentFileConversionProperties _originalFileProperties;

        //State during writing
        private IJToken _rootToken;

        public JsonFileWriter(IJsonFactory jsonFactory, IFileHelper fileHelper, ISegmentReader segmentReader)
        {
            _jsonFactory = jsonFactory;
            _fileHelper = fileHelper;
            _segmentReader = segmentReader;
        }

        public void Initialize(IDocumentProperties documentInfo)
        {
        }

        public void Complete()
        {
        }

        public void SetFileProperties(IFileProperties properties)
        {
            _rootToken = _jsonFactory.GetRootToken(_originalFileProperties.OriginalFilePath);
        }

        public void FileComplete()
        {
            _fileHelper.WriteAllText(_nativeFileProperties.OutputFilePath, _rootToken.ToString());
            _rootToken = null;
        }

        public void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties,
            IOutputFileInfo proposedFileInfo)
        {
            _originalFileProperties = fileProperties;
        }

        public void SetOutputProperties(INativeOutputFileProperties properties)
        {
            _nativeFileProperties = properties;
        }

        public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            var locationContextInfo = paragraphUnit.Properties.Contexts.Contexts[1];
            var sourcePath = locationContextInfo.GetMetaData(ContextKeys.SourcePath);
            var targetPath = locationContextInfo.GetMetaData(ContextKeys.TargetPath);

            if (string.IsNullOrEmpty(targetPath))
            {
                targetPath = sourcePath;
            }

            var targetToken = _rootToken.SelectToken(targetPath);

            if (targetToken.Type != JTokenType.String)
            {
                return;
            }

            var targetText = _segmentReader.GetText(paragraphUnit.Target);

            var value = targetToken.Value;

            if (value == null)
            {
                return;
            }

            value.Value = targetText;
        }

        public void Dispose()
        {
        }
    }
}