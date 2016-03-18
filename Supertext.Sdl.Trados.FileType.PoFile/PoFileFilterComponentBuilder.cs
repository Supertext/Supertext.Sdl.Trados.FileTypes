using System;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;
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
            return new PoFileSniffer(new ExtendedStreamReader(new FileHelper()), new LineParser());
        }

        public IFileExtractor BuildFileExtractor(string name)
        {
            var parser = new PoFileParser(new ExtendedStreamReader(new FileHelper()), new LineParser(), new UserSettings());
            var extractor = FileTypeManager.BuildFileExtractor(FileTypeManager.BuildNativeExtractor(parser), this);
            return extractor;
        }

        public IFileGenerator BuildFileGenerator(string name)
        {
            var writer = new PoFileWriter(new FileHelper());
            var generator = FileTypeManager.BuildFileGenerator(FileTypeManager.BuildNativeGenerator(writer));
            return generator;
        }

        public IAdditionalGeneratorsInfo BuildAdditionalGeneratorsInfo(string name)
        {
            return null;
        }

        public IAbstractGenerator BuildAbstractGenerator(string name)
        {
            return FileTypeManager.BuildFileGenerator(FileTypeManager.BuildNativeGenerator(new PoFileWriter(new FileHelper())));
        }

        public IPreviewSetsFactory BuildPreviewSetsFactory(string name)
        {
            IPreviewSetsFactory previewFactory = FileTypeManager.BuildPreviewSetsFactory();

            IPreviewSet externalPreviewSet = previewFactory.CreatePreviewSet();
            externalPreviewSet.Id = new PreviewSetId("ExternalPreview");
            externalPreviewSet.Name = new LocalizableString(Resources.ExternalPreview_Name);

            IApplicationPreviewType sourceAppPreviewType = previewFactory.CreatePreviewType<IApplicationPreviewType>() as IApplicationPreviewType;

            if (sourceAppPreviewType != null)
            {
                sourceAppPreviewType.SourceGeneratorId = new GeneratorId("DefaultPreview");
                sourceAppPreviewType.SingleFilePreviewApplicationId = new PreviewApplicationId("ExternalPreview");
                externalPreviewSet.Source = sourceAppPreviewType;
            }

            IApplicationPreviewType targetAppPreviewType = previewFactory.CreatePreviewType<IApplicationPreviewType>() as IApplicationPreviewType;
            if (targetAppPreviewType != null)
            {
                targetAppPreviewType.TargetGeneratorId = new GeneratorId("DefaultPreview");
                targetAppPreviewType.SingleFilePreviewApplicationId = new PreviewApplicationId("ExternalPreview");
                externalPreviewSet.Target = targetAppPreviewType;
            }

            previewFactory.GetPreviewSets(null).Add(externalPreviewSet);

            IPreviewSet internalStaticPreviewSet = previewFactory.CreatePreviewSet();
            internalStaticPreviewSet.Id = new PreviewSetId("InternalStaticPreview");
            internalStaticPreviewSet.Name = new LocalizableString(Resources.InternalStaticPreview_Name);

            IControlPreviewType sourceControlPreviewType1 = previewFactory.CreatePreviewType<IControlPreviewType>() as IControlPreviewType;
            if (sourceControlPreviewType1 != null)
            {
                sourceControlPreviewType1.SourceGeneratorId = new GeneratorId("StaticPreview");
                sourceControlPreviewType1.SingleFilePreviewControlId = new PreviewControlId("InternalNavigablePreview");
                internalStaticPreviewSet.Source = sourceControlPreviewType1;
            }

            IControlPreviewType targetControlPreviewType1 = previewFactory.CreatePreviewType<IControlPreviewType>() as IControlPreviewType;
            if (targetControlPreviewType1 != null)
            {
                targetControlPreviewType1.TargetGeneratorId = new GeneratorId("StaticPreview");
                targetControlPreviewType1.SingleFilePreviewControlId = new PreviewControlId("InternalNavigablePreview");
                internalStaticPreviewSet.Target = targetControlPreviewType1;
            }
            previewFactory.GetPreviewSets(null).Add(internalStaticPreviewSet);

            IPreviewSet internalRealPreviewSet = previewFactory.CreatePreviewSet();
            internalRealPreviewSet.Id = new PreviewSetId("InternalRealTimePreview");
            internalRealPreviewSet.Name = new LocalizableString(Resources.InternalRealTimeNavigablePreview_Name);

            IControlPreviewType sourceControlPreviewType2 = previewFactory.CreatePreviewType<IControlPreviewType>() as IControlPreviewType;
            if (sourceControlPreviewType2 != null)
            {
                sourceControlPreviewType2.SourceGeneratorId = new GeneratorId("RealTimePreview");
                sourceControlPreviewType2.SingleFilePreviewControlId = new PreviewControlId("InternalNavigablePreview");
                internalRealPreviewSet.Source = sourceControlPreviewType2;
            }

            IControlPreviewType targetControlPreviewType2 = previewFactory.CreatePreviewType<IControlPreviewType>() as IControlPreviewType;
            if (targetControlPreviewType2 != null)
            {
                targetControlPreviewType2.TargetGeneratorId = new GeneratorId("RealTimePreview");
                targetControlPreviewType2.SingleFilePreviewControlId = new PreviewControlId("InternalNavigablePreview");
                internalRealPreviewSet.Target = targetControlPreviewType2;
            }
            previewFactory.GetPreviewSets(null).Add(internalRealPreviewSet);

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