using System.Globalization;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.Parsing;
using Supertext.Sdl.Trados.FileType.PoFile.Resources;

namespace Supertext.Sdl.Trados.FileType.PoFile.TextProcessing
{
    public class ParagraphUnitFactory : IParagraphUnitFactory
    {
        public IDocumentItemFactory ItemFactory { get; set; }

        public IPropertiesFactory PropertiesFactory { get; set; }

        public IParagraphUnit Create(Entry entry, LineType sourceLineType, bool isTargetTextNeeded)
        {
            var paragraphUnit = ItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);

            if (entry.IsPluralForm)
            {
                AddPluralFormSegmentPairs(paragraphUnit, entry, sourceLineType, isTargetTextNeeded);
            }
            else
            {
                AddSegmentPair(paragraphUnit, entry.MessageId, entry.MessageString, sourceLineType, isTargetTextNeeded);
            }
            
            paragraphUnit.Properties.Contexts = CreateContextProperties(entry);

            return paragraphUnit;
        }

        private void AddPluralFormSegmentPairs(IParagraphUnit paragraphUnit, Entry entry, LineType sourceLineType, bool isTargetTextNeeded)
        {
            var counter = 0;
            foreach (var messageString in entry.MessageStringPlurals)
            {
                var messageId = counter == 0 ? entry.MessageId : entry.MessageIdPlural;

                AddSegmentPair(paragraphUnit, messageId, messageString,
                    sourceLineType, isTargetTextNeeded);

                ++counter;
            }
        }

        private void AddSegmentPair(IParagraphUnit paragraphUnit, string messageId, string messageString, LineType sourceLineType, bool isTargetTextNeeded)
        {
            var segmentPairProperties = CreateSegmentPairProperties();

            var sourceText = sourceLineType == LineType.MessageString ? messageString : messageId;

            var sourceSegment = ItemFactory.CreateSegment(segmentPairProperties);
            sourceSegment.Add(CreateText(sourceText));
            paragraphUnit.Source.Add(sourceSegment);

            if (!isTargetTextNeeded)
            {
                return;
            }

            if (sourceLineType == LineType.MessageId && !string.IsNullOrEmpty(messageString))
            {
                segmentPairProperties.TranslationOrigin.OriginType = DefaultTranslationOrigin.MachineTranslation;
                segmentPairProperties.ConfirmationLevel = ConfirmationLevel.Translated;
            }

            var targetSegment = ItemFactory.CreateSegment(segmentPairProperties);
            targetSegment.Add(CreateText(messageString));
            paragraphUnit.Target.Add(targetSegment);
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

        private IContextProperties CreateContextProperties(Entry entry)
        {
            var contextProperties = PropertiesFactory.CreateContextProperties();

            contextProperties.Contexts.Add(CreateFieldContextInfo(entry));
            contextProperties.Contexts.Add(CreateLocationContextInfo(entry));

            if (!string.IsNullOrEmpty(entry.MessageContext))
            {
                contextProperties.Contexts.Add(CreateMessageContextInfo(entry));
            }

            return contextProperties;
        }

        private IContextInfo CreateFieldContextInfo(Entry entry)
        {
            var contextInfo = PropertiesFactory.CreateContextInfo(StandardContextTypes.Field);
            contextInfo.Purpose = ContextPurpose.Match;
            contextInfo.Description = entry.Description;
            return contextInfo;
        }

        private IContextInfo CreateLocationContextInfo(Entry entry)
        {
            var contextInfo = PropertiesFactory.CreateContextInfo(ContextKeys.LocationContextType);
            contextInfo.DisplayName = PoFileTypeResources.ResourceManager.GetString(ContextKeys.LocationContextType);
            contextInfo.Purpose = ContextPurpose.Location;
            contextInfo.Description = PoFileTypeResources.ResourceManager.GetString(ContextKeys.LocationContextType);
            contextInfo.SetMetaData(ContextKeys.MetaMessageStringStart, entry.MessageStringStart.ToString(CultureInfo.InvariantCulture));
            contextInfo.SetMetaData(ContextKeys.MetaMessageStringEnd, entry.MessageStringEnd.ToString(CultureInfo.InvariantCulture));
            return contextInfo;
        }

        private IContextInfo CreateMessageContextInfo(Entry entry)
        {
            var contextInfo = PropertiesFactory.CreateContextInfo(ContextKeys.MessageContext);
            contextInfo.DisplayName = PoFileTypeResources.ResourceManager.GetString(ContextKeys.MessageContext);
            contextInfo.Purpose = ContextPurpose.Information;
            contextInfo.Description = entry.MessageContext;
            contextInfo.DisplayCode = "FCX";
            return contextInfo;
        }
    }
}