﻿using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.JsonFile.Resources;
using Supertext.Sdl.Trados.FileType.Utils.Settings;
using Supertext.Sdl.Trados.FileType.Utils.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.JsonFile
{
    [FileTypeComponentBuilder(Id = "JsonFile_FilterComponentBuilderExtension_Id",
        Name = "JsonFile_FilterComponentBuilderExtension_Name",
        Description = "JsonFile_FilterComponentBuilderExtension_Description")]
    public class JsonFileFilterComponentBuilder : IFileTypeComponentBuilder
    {
        public IFileTypeManager FileTypeManager { get; set; }

        public IFileTypeDefinition FileTypeDefinition { get; set; }

        public IFileTypeInformation BuildFileTypeInformation(string name)
        {
            var info = FileTypeManager.BuildFileTypeInformation();

            info.FileTypeDefinitionId = new FileTypeDefinitionId("JSON File Filter 1.0.0.0");
            info.FileTypeName = new LocalizableString(JsonFileTypeResources.Json_File);
            info.FileTypeDocumentName = new LocalizableString(JsonFileTypeResources.Json_File);
            info.FileTypeDocumentsName = new LocalizableString(JsonFileTypeResources.Json_Files);
            info.Description = new LocalizableString(JsonFileTypeResources.Json_File_Filter_Description);
            info.FileDialogWildcardExpression = "*.instapage";
            info.DefaultFileExtension = "json";
            info.Icon = new IconDescriptor(JsonFileTypeResources.JsonFileIcon);

            info.WinFormSettingsPageIds = new[]
            {
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
            return new JsonFileSniffer();
        }

        public IFileExtractor BuildFileExtractor(string name)
        {
            var parser = new JsonFileParser(new TextProcessor(), new EmbeddedContentRegexSettings());
            var fileExtractor = FileTypeManager.BuildFileExtractor(FileTypeManager.BuildNativeExtractor(parser), this);

            var processor = new RegexEmbeddedBilingualProcessor(
                new EmbeddedContentVisitor(),
                new EmbeddedContentRegexSettings(),
                new TextProcessor());

            fileExtractor.AddBilingualProcessor(processor);

            return fileExtractor;
        }

        public IFileGenerator BuildFileGenerator(string name)
        {
            var writer = new JsonFileWriter();
            var generator = FileTypeManager.BuildFileGenerator(FileTypeManager.BuildNativeGenerator(writer));
            return generator;

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