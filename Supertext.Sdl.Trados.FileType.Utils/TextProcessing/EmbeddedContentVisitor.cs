using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Supertext.Sdl.Trados.FileType.Utils.TextProcessing
{
    public class EmbeddedContentVisitor : IEmbeddedContentVisitorFactory, IEmbeddedContentVisitor
    {
        private const string DisplayTextpattern = @"(\w+)";

        private IDocumentItemFactory _itemFactory;
        private IPropertiesFactory _propertiesFactory;
        private ITextProcessor _textProcessor;
        private IAbstractMarkupDataContainer _currentContainer;

        public IParagraph GeneratedParagraph { get; private set; }

        public IEmbeddedContentVisitor CreateVisitor(IDocumentItemFactory itemFactory,
            IPropertiesFactory propertiesFactory, ITextProcessor textProcessor)
        {
            _itemFactory = itemFactory;
            _propertiesFactory = propertiesFactory;
            _textProcessor = textProcessor;
            GeneratedParagraph = _itemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked).Source;
            _currentContainer = GeneratedParagraph;
            return this;
        }

        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            var commentMarker2 = _itemFactory.CreateCommentMarker(commentMarker.Comments);
            _currentContainer.Add(commentMarker2);
            _currentContainer = commentMarker2;
            foreach (var current in commentMarker)
            {
                current.AcceptVisitor(this);
            }
            _currentContainer = commentMarker2.Parent;
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
            _currentContainer.Add((ILocationMarker) location.Clone());
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
            _currentContainer.Add((ILockedContent) lockedContent.Clone());
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
            var otherMarker = _itemFactory.CreateOtherMarker();
            otherMarker.Id = marker.Id;
            otherMarker.MarkerType = marker.MarkerType;
            _currentContainer.Add(otherMarker);
            _currentContainer = otherMarker;
            foreach (var current in marker)
            {
                current.AcceptVisitor(this);
            }
            _currentContainer = otherMarker.Parent;
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            _currentContainer.Add((IPlaceholderTag) tag.Clone());
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
            var revisionMarker2 = CreateRevisionOrFeedback(revisionMarker.Properties);
            _currentContainer.Add(revisionMarker2);
            _currentContainer = revisionMarker2;
            foreach (var current in revisionMarker)
            {
                current.AcceptVisitor(this);
            }
            _currentContainer = revisionMarker2.Parent;
        }

        public void VisitSegment(ISegment segment)
        {
            var segment2 = _itemFactory.CreateSegment(segment.Properties);
            _currentContainer.Add(segment2);
            _currentContainer = segment2;
            foreach (var current in segment)
            {
                current.AcceptVisitor(this);
            }
            _currentContainer = segment2.Parent;
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            var tagPair2 = _itemFactory.CreateTagPair(tagPair.StartTagProperties, tagPair.EndTagProperties);
            _currentContainer.Add(tagPair2);
            _currentContainer = tagPair2;
            foreach (var current in tagPair)
            {
                current.AcceptVisitor(this);
            }
            _currentContainer = tagPair2.Parent;
        }

        public void VisitText(IText text)
        {
            var fragments = new Queue<IFragment>(_textProcessor.Process(text.Properties.Text));

            while (fragments.Count > 0)
            {
                var fragment = fragments.Dequeue();

                switch (fragment.InlineType)
                {
                    case InlineType.Text:
                        _currentContainer.Add(CreateText(fragment.Content));
                        break;
                    case InlineType.Placeholder:
                        _currentContainer.Add(CreatePlaceholder(fragment.Content, fragment.SegmentationHint));
                        break;
                    case InlineType.StartTag:
                        _currentContainer.Add(CreateTags(fragment, fragments));
                        break;
                    case InlineType.EndTag:
                        throw new ArgumentOutOfRangeException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private IRevisionMarker CreateRevisionOrFeedback(IRevisionProperties properties)
        {
            switch (properties.RevisionType)
            {
                case RevisionType.Insert:
                case RevisionType.Delete:
                case RevisionType.Unchanged:
                    return _itemFactory.CreateRevision(properties);
                case RevisionType.FeedbackComment:
                case RevisionType.FeedbackAdded:
                case RevisionType.FeedbackDeleted:
                    return _itemFactory.CreateFeedback(properties);
                default:
                    return _itemFactory.CreateRevision(properties);
            }
        }

        private IText CreateText(string value)
        {
            var textProperties = _propertiesFactory.CreateTextProperties(value);
            return _itemFactory.CreateText(textProperties);
        }

        private IPlaceholderTag CreatePlaceholder(string content, SegmentationHint segmentationHint)
        {
            var placeholderProperties = _propertiesFactory.CreatePlaceholderTagProperties(content);
            placeholderProperties.TagContent = content;
            placeholderProperties.DisplayText = content;
            placeholderProperties.SegmentationHint = segmentationHint;

            return _itemFactory.CreatePlaceholderTag(placeholderProperties);
        }

        private IAbstractMarkupData CreateTags(IFragment startTagFragment, Queue<IFragment> fragments)
        {
            var startTagProperties = _propertiesFactory.CreateStartTagProperties(startTagFragment.Content);
            startTagProperties.DisplayText = CreateDisplayText(startTagFragment.Content);
            startTagProperties.SegmentationHint = startTagFragment.SegmentationHint;

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
                        enclosedContent.Add(CreatePlaceholder(fragment.Content, fragment.SegmentationHint));
                        break;
                    case InlineType.StartTag:
                        enclosedContent.Add(CreateTags(fragment, fragments));
                        break;
                    case InlineType.EndTag:
                        var endTagProperties = _propertiesFactory.CreateEndTagProperties(fragment.Content);
                        endTagProperties.DisplayText = CreateDisplayText(fragment.Content);

                        return fragment.IsContentTranslatable
                            ? CreateTagPair(startTagProperties, endTagProperties, enclosedContent)
                            : CreateLockedContent(enclosedContent);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            throw new ArgumentOutOfRangeException();
        }

        private static string CreateDisplayText(string tagContent)
        {
            var displayTextRegex = new Regex(DisplayTextpattern, RegexOptions.IgnoreCase);
            var match = displayTextRegex.Match(tagContent);

            return match.Success ? match.Value : tagContent;
        }

        private IAbstractMarkupData CreateLockedContent(IEnumerable<IAbstractMarkupData> enclosedContent)
        {
            var properties = _propertiesFactory.CreateLockedContentProperties(LockTypeFlags.Manual);
            var lockedContent = _itemFactory.CreateLockedContent(properties);
            
            foreach (var enclosedData in enclosedContent)
            {
                lockedContent.Content.Add(enclosedData);
            }

            return lockedContent;
        }

        private ITagPair CreateTagPair(IStartTagProperties startTagProperties, IEndTagProperties endTagProperties,
            IEnumerable<IAbstractMarkupData> enclosedContent)
        {
            var tagPair = _itemFactory.CreateTagPair(startTagProperties, endTagProperties);

            foreach (var enclosedData in enclosedContent)
            {
                tagPair.Add(enclosedData);
            }

            return tagPair;
        }
    }
}