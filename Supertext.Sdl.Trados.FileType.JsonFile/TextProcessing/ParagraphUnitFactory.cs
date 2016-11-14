using System.Text.RegularExpressions;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.JsonFile.Parsing;
using Supertext.Sdl.Trados.FileType.JsonFile.Resources;

namespace Supertext.Sdl.Trados.FileType.JsonFile.TextProcessing
{
    public class ParagraphUnitFactory: IParagraphUnitFactory
    {
        public IDocumentItemFactory ItemFactory { get; set; }

        public IPropertiesFactory PropertiesFactory { get; set; }


        public IParagraphUnit Create(IJsonTextReader reader)
        {
            var paragraphUnit = ItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);

            AddSegmentPair(paragraphUnit, reader.Value.ToString());

            // TODO remove this quick fix once json.NET library has fixed issue
            // Escape quotes in path
            var path = reader.Path;
            var mathes = Regex.Matches(path, @"\['[^[]*'\]");
            foreach (Match match in mathes)
            {
                path = path.Replace(match.Value, Regex.Replace(match.Value, @"((?<!\\|\[)'(?!\]))", "\\'"));
            }

            paragraphUnit.Properties.Contexts = CreateContextProperties(path);

            return paragraphUnit;
        }

        private void AddSegmentPair(IParagraphUnit paragraphUnit, string text)
        {
            var segmentPairProperties = CreateSegmentPairProperties();
            var sourceSegment = ItemFactory.CreateSegment(segmentPairProperties);
            sourceSegment.Add(CreateText(text));
            paragraphUnit.Source.Add(sourceSegment);
        }

        private ISegmentPairProperties CreateSegmentPairProperties()
        {
            var segmentPairProperties = ItemFactory.CreateSegmentPairProperties();
            segmentPairProperties.TranslationOrigin = ItemFactory.CreateTranslationOrigin();
            segmentPairProperties.TranslationOrigin.MatchPercent = 0;
            segmentPairProperties.TranslationOrigin.OriginType = DefaultTranslationOrigin.NotTranslated;
            segmentPairProperties.ConfirmationLevel = ConfirmationLevel.Unspecified;
            return segmentPairProperties;
        }

        private IText CreateText(string value)
        {
            var textProperties = PropertiesFactory.CreateTextProperties(value);

            return ItemFactory.CreateText(textProperties);
        }

        private IContextProperties CreateContextProperties(string path)
        {
            var contextProperties = PropertiesFactory.CreateContextProperties();

            contextProperties.Contexts.Add(CreateFieldContextInfo(path));
            contextProperties.Contexts.Add(CreateLocationContextInfo(path));

            return contextProperties;
        }

        private IContextInfo CreateFieldContextInfo(string path)
        {
            var contextInfo = PropertiesFactory.CreateContextInfo(StandardContextTypes.Field);
            contextInfo.Purpose = ContextPurpose.Match;
            contextInfo.Description = path;
            return contextInfo;
        }

        private IContextInfo CreateLocationContextInfo(string sourcePath)
        {
            var contextInfo = PropertiesFactory.CreateContextInfo(ContextKeys.ValueLocation);
            contextInfo.Purpose = ContextPurpose.Location;
            contextInfo.DisplayName = JsonFileTypeResources.Value_Path;
            contextInfo.Description = JsonFileTypeResources.Value_Path;
            contextInfo.SetMetaData(ContextKeys.SourcePath, sourcePath);
            contextInfo.SetMetaData(ContextKeys.TargetPath, sourcePath);
            return contextInfo;
        }
        
    }
}
