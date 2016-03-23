using System;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;
using Supertext.Sdl.Trados.FileType.PoFile.Properties;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    [FileTypeComponentBuilderAttribute(Id = "PoFile_FilterComponentBuilderExtension_Id",
        Name = "PoFile_FilterComponentBuilderExtension_Name",
        Description = "PoFile_FilterComponentBuilderExtension_Description")]
    public class PoFileFilterComponentBuilder : IFileTypeComponentBuilder
    {
        public IFileTypeManager FileTypeManager { get; set; }

        public IFileTypeDefinition FileTypeDefinition { get; set; }

        public IFileTypeInformation BuildFileTypeInformation(string name)
        {
            var info = FileTypeManager.BuildFileTypeInformation();

            info.FileTypeDefinitionId = new FileTypeDefinitionId("PO File Filter 1.0.0.0");
            info.FileTypeName = new LocalizableString("PO files");
            info.FileTypeDocumentName = new LocalizableString("PO files");
            info.FileTypeDocumentsName = new LocalizableString("PO files");
            info.Description = new LocalizableString("This filter is used to process PO files.");
            info.FileDialogWildcardExpression = "*.po";
            info.DefaultFileExtension = "po";
            info.Icon = new IconDescriptor(PoFileTypeResources.PoFileIcon);

            info.WinFormSettingsPageIds = new[]
            {
                "PoFile_Settings",
                "QuickInserts_Settings",
            };

            return info;
        }

        public IQuickTagsFactory BuildQuickTagsFactory(string name)
        {
            IQuickTagsFactory quickTags = FileTypeManager.BuildQuickTagsFactory();
            return quickTags;
        }

        public INativeFileSniffer BuildFileSniffer(string name)
        {
            return new PoFileSniffer(new FileHelper(), new LineParser());
        }

        public IFileExtractor BuildFileExtractor(string name)
        {
            var parser = new PoFileParser(new FileHelper(), new LineParser(), new UserSettings());
            return FileTypeManager.BuildFileExtractor(parser, this);
        }

        public IFileGenerator BuildFileGenerator(string name)
        {
            var writer = new PoFileWriter(new FileHelper(), new SegmentReader());
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
            IPreviewSetsFactory previewFactory = FileTypeManager.BuildPreviewSetsFactory();

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