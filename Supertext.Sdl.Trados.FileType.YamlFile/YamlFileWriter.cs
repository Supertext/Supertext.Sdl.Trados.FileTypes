using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.YamlFile.Parsing;
using Supertext.Sdl.Trados.FileType.YamlFile.TextProcessing;
using Supertext.Sdl.Trados.FileType.Utils.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.YamlFile
{
    public class YamlFileWriter : AbstractBilingualFileTypeComponent, IBilingualWriter, INativeOutputSettingsAware
    {
        private readonly IYamlFactory _yamlFactory;
        private readonly ISegmentReader _segmentReader;
        private INativeOutputFileProperties _nativeFileProperties;
        private IPersistentFileConversionProperties _originalFileProperties;

        //State during writing
        private IYamlTextWriter _yamlTextWriter;

        public YamlFileWriter(IYamlFactory yamlFactory, ISegmentReader segmentReader)
        {
            _yamlFactory = yamlFactory;
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
            try
            {
                _yamlTextWriter = _yamlFactory.CreateYamlTextWriter(_originalFileProperties.OriginalFilePath, _nativeFileProperties.OutputFilePath);
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                //if we can't find the directory, we read from the embedded base64 in the sdlxliff file
                _yamlTextWriter = _yamlFactory.CreateYamlTextWriter(_originalFileProperties.DependencyFiles.First().CurrentFilePath, 
                                                                    _nativeFileProperties.OutputFilePath);
            }
        }

        public void FileComplete()
        {
            _yamlTextWriter.FinishWriting();
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

            var targetText = _segmentReader.GetText(paragraphUnit.Target);

            _yamlTextWriter.Write(targetPath, targetText);
        }

        public void Dispose()
        {
            _yamlTextWriter.Dispose();
        }
    }
}