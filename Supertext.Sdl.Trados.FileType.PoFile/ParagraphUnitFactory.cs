using System;
using System.Collections.Generic;
using System.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface IParagraphUnitFactory
    {
        IDocumentItemFactory ItemFactory { get; set; }

        IPropertiesFactory PropertiesFactory { get; set; }

        IParagraphUnit Create(Entry entry, LineType sourceLineType, bool isTargetTextNeeded);
    }

    public class ParagraphUnitFactory : IParagraphUnitFactory
    {
        private readonly ITextProcessor _textProcessor;

        public ParagraphUnitFactory(ITextProcessor textProcessor)
        {
            _textProcessor = textProcessor;
        }

        public IDocumentItemFactory ItemFactory { get; set; }

        public IPropertiesFactory PropertiesFactory { get; set; }

        public IParagraphUnit Create(Entry entry, LineType sourceLineType, bool isTargetTextNeeded)
        {
            var paragraphUnit = ItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);
            var segmentPairProperties = ItemFactory.CreateSegmentPairProperties();

            var sourceText = sourceLineType == LineType.MessageString ? entry.MessageString : entry.MessageId;

            var sourceSegment = ItemFactory.CreateSegment(segmentPairProperties);
            AddText(sourceSegment, sourceText);
            paragraphUnit.Source.Add(sourceSegment);

            if (isTargetTextNeeded)
            {
                var targetSegment = ItemFactory.CreateSegment(segmentPairProperties);
                AddText(targetSegment, entry.MessageString);
                paragraphUnit.Target.Add(targetSegment);
            }

            paragraphUnit.Properties.Contexts = CreateContextProperties(entry);

            return paragraphUnit;
        }

        private void AddText(ISegment segment, string text)
        {
            var fragments = new Queue<Fragment>(_textProcessor.Process(text));

            while (fragments.Count > 0)
            {
                var fragment = fragments.Dequeue();

                switch (fragment.InlineType)
                {
                    case InlineType.Text:
                        segment.Add(CreateText(fragment.Content));
                        break;
                    case InlineType.Placeholder:
                        segment.Add(CreatePlaceholder(fragment.Content));
                        break;
                    case InlineType.StartTag:
                        segment.Add(CreateTagPair(fragment, fragments));
                        break;
                    case InlineType.EndTag:
                        throw new ArithmeticException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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

            var contextInfo = PropertiesFactory.CreateContextInfo(StandardContextTypes.Field);
            contextInfo.Purpose = ContextPurpose.Information;
            contextProperties.Contexts.Add(contextInfo);

            var contextId = PropertiesFactory.CreateContextInfo(ContextKeys.LocationContextType);
            contextId.SetMetaData(ContextKeys.MessageIdStart, entry.MessageIdStart.ToString(CultureInfo.InvariantCulture));
            contextId.SetMetaData(ContextKeys.MessageIdEnd, entry.MessageIdEnd.ToString(CultureInfo.InvariantCulture));
            contextId.SetMetaData(ContextKeys.MessageStringStart, entry.MessageStringStart.ToString(CultureInfo.InvariantCulture));
            contextId.SetMetaData(ContextKeys.MessageStringEnd, entry.MessageStringEnd.ToString(CultureInfo.InvariantCulture));
            contextProperties.Contexts.Add(contextId);

            return contextProperties;
        }
    }
}