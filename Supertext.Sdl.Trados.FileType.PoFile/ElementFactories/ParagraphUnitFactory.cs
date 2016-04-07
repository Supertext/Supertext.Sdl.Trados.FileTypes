using System;
using System.Collections.Generic;
using System.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.Parsing;
using Supertext.Sdl.Trados.FileType.PoFile.Resources;
using Supertext.Sdl.Trados.FileType.PoFile.Settings;
using Supertext.Sdl.Trados.FileType.PoFile.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.PoFile.ElementFactories
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
            var segmentPairProperties = ItemFactory.CreateSegmentPairProperties();

            var sourceText = sourceLineType == LineType.MessageString ? messageString : messageId;

            var sourceSegment = ItemFactory.CreateSegment(segmentPairProperties);
            AddText(sourceSegment, sourceText);
            paragraphUnit.Source.Add(sourceSegment);

            if (!isTargetTextNeeded)
            {
                return;
            }

            var targetSegment = ItemFactory.CreateSegment(segmentPairProperties);
            AddText(targetSegment, messageString);
            paragraphUnit.Target.Add(targetSegment);
        }

        private void AddText(ISegment segment, string text)
        {
            segment.Add(CreateText(text));
        }

        private IText CreateText(string value)
        {
            var textProperties = PropertiesFactory.CreateTextProperties(value);

            return ItemFactory.CreateText(textProperties);
        }

        private IPlaceholderTag CreatePlaceholder(string content)
        {
            var placeholderProperties = PropertiesFactory.CreatePlaceholderTagProperties(content);
            placeholderProperties.TagContent = content;
            placeholderProperties.DisplayText = content;

            return ItemFactory.CreatePlaceholderTag(placeholderProperties);
        }

        private ITagPair CreateTagPair(Fragment startTagfragment, Queue<Fragment> fragments)
        {
            var startTagProperties = PropertiesFactory.CreateStartTagProperties(startTagfragment.Content);
            startTagProperties.DisplayText = startTagfragment.Content;

            var enclosedContent = new List<IAbstractMarkupData>();

            while (fragments.Count > 0)
            {
                var fragment = fragments.Dequeue();

                switch (fragment.InlineType)
                {
                    case InlineType.Text:
                        enclosedContent.Add(CreateText(fragment.Content));
                        break;
                    case InlineType.Placeholder:
                        enclosedContent.Add(CreatePlaceholder(fragment.Content));
                        break;
                    case InlineType.StartTag:
                        enclosedContent.Add(CreateTagPair(fragment, fragments));
                        break;
                    case InlineType.EndTag:
                        var endTagProperties = PropertiesFactory.CreateEndTagProperties(fragment.Content);
                        endTagProperties.DisplayText = fragment.Content;

                        var tagPair = ItemFactory.CreateTagPair(startTagProperties, endTagProperties);

                        foreach (var abstractMarkupData in enclosedContent)
                        {
                            tagPair.Add(abstractMarkupData);
                        }

                        return tagPair;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            throw new ArgumentOutOfRangeException();
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