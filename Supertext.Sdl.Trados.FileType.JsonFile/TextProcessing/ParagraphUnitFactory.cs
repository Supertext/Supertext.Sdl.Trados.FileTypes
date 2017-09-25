using System.Text.RegularExpressions;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.JsonFile.Resources;

namespace Supertext.Sdl.Trados.FileType.JsonFile.TextProcessing
{
    public class ParagraphUnitFactory: IParagraphUnitFactory
    {
        public IDocumentItemFactory ItemFactory { get; set; }

        public IPropertiesFactory PropertiesFactory { get; set; }


        public IParagraphUnit Create(string sourcePath, string sourceValue, string targetPath, string targetValue)
        {
            var paragraphUnit = ItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);

            // TODO remove this quick fix once json.NET library has fixed issue
            // Escape quotes in path
            sourcePath = EscapePath(sourcePath);
            targetPath = EscapePath(targetPath);

            AddSegmentPair(paragraphUnit, sourceValue, targetValue);

            paragraphUnit.Properties.Contexts = CreateContextProperties(sourcePath, targetPath);

            return paragraphUnit;
        }

        private static string EscapePath(string path)
        {
            var mathes = Regex.Matches(path, @"\['[^[]*'\]");
            foreach (Match match in mathes)
            {
                path = path.Replace(match.Value, Regex.Replace(match.Value, @"((?<!\\|\[)'(?!\]))", "\\'"));
            }
            return path;
        }

        private void AddSegmentPair(IParagraphUnit paragraphUnit, string sourceValue, string targetValue)
        {
            if (string.IsNullOrEmpty(targetValue))
            {
                paragraphUnit.Source.Add(CreateText(sourceValue));
            }
            else
            {
                var segmentPairProperties = CreateSegmentPairProperties();

                var sourceSegment = ItemFactory.CreateSegment(segmentPairProperties);
                sourceSegment.Add(CreateText(sourceValue));
                paragraphUnit.Source.Add(sourceSegment);

                var targetSegment = ItemFactory.CreateSegment(segmentPairProperties);
                targetSegment.Add(CreateText(targetValue));
                paragraphUnit.Target.Add(targetSegment);
            }
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

        private IContextProperties CreateContextProperties(string sourcePath, string targetPath)
        {
            var contextProperties = PropertiesFactory.CreateContextProperties();

            contextProperties.Contexts.Add(CreateFieldContextInfo(sourcePath));
            contextProperties.Contexts.Add(CreateLocationContextInfo(sourcePath, targetPath));

            return contextProperties;
        }

        private IContextInfo CreateFieldContextInfo(string path)
        {
            var contextInfo = PropertiesFactory.CreateContextInfo(StandardContextTypes.Paragraph);
            contextInfo.Purpose = ContextPurpose.Match;
            contextInfo.Description = path;
            return contextInfo;
        }

        private IContextInfo CreateLocationContextInfo(string sourcePath, string targetPath)
        {
            var contextInfo = PropertiesFactory.CreateContextInfo(ContextKeys.ValueLocation);
            contextInfo.Purpose = ContextPurpose.Location;
            contextInfo.DisplayName = JsonFileTypeResources.Value_Path;
            contextInfo.Description = JsonFileTypeResources.Value_Path;
            contextInfo.SetMetaData(ContextKeys.SourcePath, sourcePath);
            contextInfo.SetMetaData(ContextKeys.TargetPath, targetPath);
            return contextInfo;
        }
        
    }
}
