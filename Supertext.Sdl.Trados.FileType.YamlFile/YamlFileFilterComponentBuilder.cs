using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.YamlFile.Parsing;
using Supertext.Sdl.Trados.FileType.YamlFile.Resources;
using Supertext.Sdl.Trados.FileType.YamlFile.TextProcessing;
using Supertext.Sdl.Trados.FileType.Utils.FileHandling;
using Supertext.Sdl.Trados.FileType.Utils.Settings;
using Supertext.Sdl.Trados.FileType.Utils.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.YamlFile
{
    [FileTypeComponentBuilder(Id = "YamlFile_FilterComponentBuilderExtension_Id",
        Name = "YamlFile_FilterComponentBuilderExtension_Name",
        Description = "YamlFile_FilterComponentBuilderExtension_Description")]
    public class YamlFileFilterComponentBuilder : IFileTypeComponentBuilder
    {
        public IFileTypeManager FileTypeManager { get; set; }

        public IFileTypeDefinition FileTypeDefinition { get; set; }

        public IFileTypeInformation BuildFileTypeInformation(string name)
        {
            var info = FileTypeManager.BuildFileTypeInformation();

            info.FileTypeDefinitionId = new FileTypeDefinitionId("YAML file type 1.4.0.0");
            info.FileTypeName = new LocalizableString(YamlFileTypeResources.Yaml_File);
            info.FileTypeDocumentName = new LocalizableString(YamlFileTypeResources.Yaml_File);
            info.FileTypeDocumentsName = new LocalizableString(YamlFileTypeResources.Yaml_Files);
            info.Description = new LocalizableString(YamlFileTypeResources.Yaml_File_Filter_Description);
            info.FileDialogWildcardExpression = "*.yaml;*.yml";
            info.DefaultFileExtension = "yaml";
            info.Icon = new IconDescriptor(YamlFileTypeResources.YamlFileIcon);

            info.WinFormSettingsPageIds = new[]
            {
                "YamlFile_Parsing_Settings",
                "Community_Embeddded_Content_Processor_Settings"
            };

            return info;
        }

        public IQuickTagsFactory BuildQuickTagsFactory(string name)
        {
            var quickTags = FileTypeManager.BuildQuickTagsFactory();
            return quickTags;
        }

        public INativeFileSniffer BuildFileSniffer(string name)
        {
            return new YamlFileSniffer(new YamlFactory());
        }

        public IFileExtractor BuildFileExtractor(string name)
        {
            var parsingSettings = new ParsingSettings();

            var parser = new YamlFileParser(
                new YamlFactory(),
                new FileHelper(),
                new EmbeddedContentRegexSettings(),
                parsingSettings,
                new ParagraphUnitFactory(),
                new SegmentDataCollector(parsingSettings));

            var fileExtractor = FileTypeManager.BuildFileExtractor(parser, this);

            var processor = new RegexEmbeddedBilingualProcessor(
                new EmbeddedContentVisitor(),
                new EmbeddedContentRegexSettings(),
                new TextProcessor());

            fileExtractor.AddBilingualProcessor(processor);

            return fileExtractor;
        }

        public IFileGenerator BuildFileGenerator(string name)
        {
            var writer = new YamlFileWriter(new YamlFactory(),new SegmentReader());
            return FileTypeManager.BuildFileGenerator(writer);
        }

        public IAdditionalGeneratorsInfo BuildAdditionalGeneratorsInfo(string name)
        {
            return null;
        }

        public IAbstractGenerator BuildAbstractGenerator(string name)
        {
            return null;
        }

        public IPreviewSetsFactory BuildPreviewSetsFactory(string name)
        {
            var previewFactory = FileTypeManager.BuildPreviewSetsFactory();
            return previewFactory;
        }

        public IAbstractPreviewControl BuildPreviewControl(string name)
        {
            return null;
        }

        public IAbstractPreviewApplication BuildPreviewApplication(string name)
        {
            return null;
        }

        public IBilingualDocumentGenerator BuildBilingualGenerator(string name)
        {
            return null;
        }

        public IVerifierCollection BuildVerifierCollection(string name)
        {
            return null;
        }
    }
}