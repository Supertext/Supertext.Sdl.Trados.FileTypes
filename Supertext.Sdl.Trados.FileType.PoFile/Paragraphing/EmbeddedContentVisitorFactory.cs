using System;
using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.PoFile.Paragraphing
{
    public class EmbeddedContentVisitorFactory : IEmbeddedContentVisitorFactory, IEmbeddedContentVisitor
    {
        private IDocumentItemFactory _itemFactory;
        private IPropertiesFactory _propertiesFactory;
        private ITextProcessor _textProcessor;
        private IAbstractMarkupDataContainer _currentContainer;

        public IParagraph GeneratedParagraph { get; private set; }

        public IEmbeddedContentVisitor CreateVisitor(IDocumentItemFactory itemFactory, IPropertiesFactory propertiesFactory, ITextProcessor textProcessor)
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
            var fragments = new Queue<Fragment>(_textProcessor.Process(text.Properties.Text));

            while (fragments.Count > 0)
            {
                var fragment = fragments.Dequeue();

                switch (fragment.InlineType)
                {
                    case InlineType.Text:
                        _currentContainer.Add(CreateText(fragment.Content));
                        break;
                    case InlineType.Placeholder:
                        _currentContainer.Add(CreatePlaceholder(fragment.Content));
                        break;
                    case InlineType.StartTag:
                        _currentContainer.Add(CreateTagPair(fragment, fragments));
                        break;
                    case InlineType.EndTag:
                        throw new ArithmeticException();
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

        private IPlaceholderTag CreatePlaceholder(string content)
        {
            var placeholderProperties = _propertiesFactory.CreatePlaceholderTagProperties(content);
            placeholderProperties.TagContent = content;
            placeholderProperties.DisplayText = content;

            return _itemFactory.CreatePlaceholderTag(placeholderProperties);
        }

        private IAbstractMarkupData CreateTagPair(Fragment startTagfragment, Queue<Fragment> fragments)
        {
            var startTagProperties = _propertiesFactory.CreateStartTagProperties(startTagfragment.Content);
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
                        var endTagProperties = _propertiesFactory.CreateEndTagProperties(fragment.Content);
                        endTagProperties.DisplayText = fragment.Content;

                        var tagPair = _itemFactory.CreateTagPair(startTagProperties, endTagProperties);

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

        /*
            private void AddOpenTagContainer(RegexMatch match)
            {
                if (match.Rule.IsContentTranslatable)
                {
                    ITagPair tagPair = this.CreateTagPair(match);
                    this._currentContainer.Add(tagPair);
                    this._currentContainer = tagPair;
                    return;
                }
                ILockedContent lockedContent = this.CreateLockedContent(match);
                this._currentContainer.Add(lockedContent);
                this._currentContainer = lockedContent.Content;
            }

            private void AddCloseTagContainer()
            {
                ITagPair tagPair = this._currentContainer as ITagPair;
                if (tagPair != null)
                {
                    this._currentContainer = tagPair.Parent;
                    return;
                }
                ILockedContainer lockedContainer = this._currentContainer as ILockedContainer;
                if (lockedContainer != null)
                {
                    this._currentContainer = lockedContainer.LockedContent.Parent;
                }
            }

            private ILockedContent CreateLockedContent(RegexMatch match)
            {
                ILockedContentProperties properties = this._itemFactory.PropertiesFactory.CreateLockedContentProperties(LockTypeFlags.Manual);
                return this._itemFactory.CreateLockedContent(properties);
            }

            private ITagPair CreateTagPair(RegexMatch match)
            {
                IStartTagProperties startTagProperties = RegexProcessorHelper.CreateStartTagProperties(this._itemFactory.PropertiesFactory, match.Value, match.Rule);
                startTagProperties.SetMetaData("OriginalEmbeddedContent", match.Value);
                IEndTagProperties endTagProperties = RegexProcessorHelper.CreateEndTagProperties(this._itemFactory.PropertiesFactory, match.Value, match.Rule);
                endTagProperties.SetMetaData("OriginalEmbeddedContent", match.Value);
                return this._itemFactory.CreateTagPair(startTagProperties, endTagProperties);
            }

*/
        
    }

    public interface IEmbeddedContentVisitor : IMarkupDataVisitor
    {
        IParagraph GeneratedParagraph { get; }
    }

    public interface IEmbeddedContentVisitorFactory
    {
        IEmbeddedContentVisitor CreateVisitor(IDocumentItemFactory itemFactory, IPropertiesFactory propertiesFactory, ITextProcessor textProcessor);
    }
}