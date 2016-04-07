using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.Settings;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class EmbeddedContentVisitor : IMarkupDataVisitor
    {
        private readonly IDocumentItemFactory _factory;
        private readonly List<MatchRule> _matchRules;

        private IAbstractMarkupDataContainer _currentContainer;

        public EmbeddedContentVisitor(IDocumentItemFactory factory, List<MatchRule> matchRules)
        {
            _factory = factory;
            _matchRules = matchRules;
            GeneratedParagraph = factory.CreateParagraphUnit(LockTypeFlags.Unlocked).Source;
            _currentContainer = GeneratedParagraph;
        }

        public IParagraph GeneratedParagraph { get; }

        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            ICommentMarker commentMarker2 = _factory.CreateCommentMarker(commentMarker.Comments);
            _currentContainer.Add(commentMarker2);
            this._currentContainer = commentMarker2;
            foreach (IAbstractMarkupData current in commentMarker)
            {
                current.AcceptVisitor(this);
            }
            this._currentContainer = commentMarker2.Parent;
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
            this._currentContainer.Add((ILocationMarker)location.Clone());
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
            this._currentContainer.Add((ILockedContent)lockedContent.Clone());
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
            IOtherMarker otherMarker = this._factory.CreateOtherMarker();
            otherMarker.Id = marker.Id;
            otherMarker.MarkerType = marker.MarkerType;
            this._currentContainer.Add(otherMarker);
            this._currentContainer = otherMarker;
            foreach (IAbstractMarkupData current in marker)
            {
                current.AcceptVisitor(this);
            }
            this._currentContainer = otherMarker.Parent;
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            this._currentContainer.Add((IPlaceholderTag)tag.Clone());
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
            IRevisionMarker revisionMarker2 = this.CreateRevisionOrFeedback(revisionMarker.Properties);
            this._currentContainer.Add(revisionMarker2);
            this._currentContainer = revisionMarker2;
            foreach (IAbstractMarkupData current in revisionMarker)
            {
                current.AcceptVisitor(this);
            }
            this._currentContainer = revisionMarker2.Parent;
        }

        private IRevisionMarker CreateRevisionOrFeedback(IRevisionProperties properties)
        {
            switch (properties.RevisionType)
            {
                case RevisionType.Insert:
                case RevisionType.Delete:
                case RevisionType.Unchanged:
                    return this._factory.CreateRevision(properties);
                case RevisionType.FeedbackComment:
                case RevisionType.FeedbackAdded:
                case RevisionType.FeedbackDeleted:
                    return this._factory.CreateFeedback(properties);
                default:
                    return this._factory.CreateRevision(properties);
            }
        }

        public void VisitSegment(ISegment segment)
        {
            ISegment segment2 = this._factory.CreateSegment(segment.Properties);
            this._currentContainer.Add(segment2);
            this._currentContainer = segment2;
            foreach (IAbstractMarkupData current in segment)
            {
                current.AcceptVisitor(this);
            }
            this._currentContainer = segment2.Parent;
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            ITagPair tagPair2 = this._factory.CreateTagPair(tagPair.StartTagProperties, tagPair.EndTagProperties);
            this._currentContainer.Add(tagPair2);
            this._currentContainer = tagPair2;
            foreach (IAbstractMarkupData current in tagPair)
            {
                current.AcceptVisitor(this);
            }
            this._currentContainer = tagPair2.Parent;
        }

        public void VisitText(IText text)
        {
            /*string text2 = text.Properties.Text;
            List<RegexMatch> list = RegexProcessorHelper.ApplyRegexRules(text2, this._matchRules);
            int num = 0;
            foreach (RegexMatch current in list)
            {
                if (current.Index > num)
                {
                    this._currentContainer.Add(this.CreateText(text2.Substring(num, current.Index - num)));
                }
                else if (num > current.Index)
                {
                    continue;
                }
                switch (current.Type)
                {
                    case RegexMatch.TagType.Placeholder:
                        this._currentContainer.Add(this.CreatePlaceholderTag(current));
                        break;
                    case RegexMatch.TagType.TagPairOpening:
                        this.AddOpenTagContainer(current);
                        break;
                    case RegexMatch.TagType.TagPairClosing:
                        this.AddCloseTagContainer();
                        break;
                }
                num = current.Index + current.Value.Length;
            }
            if (num < text2.Length)
            {
                this._currentContainer.Add(this.CreateText(text2.Substring(num, text2.Length - num)));
            }*/
        }

        /*private void AddOpenTagContainer(RegexMatch match)
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
            ILockedContentProperties properties = this._factory.PropertiesFactory.CreateLockedContentProperties(LockTypeFlags.Manual);
            return this._factory.CreateLockedContent(properties);
        }

        private IText CreateText(string textContent)
        {
            ITextProperties textInfo = this._factory.PropertiesFactory.CreateTextProperties(textContent);
            return this._factory.CreateText(textInfo);
        }

        private ITagPair CreateTagPair(RegexMatch match)
        {
            IStartTagProperties startTagProperties = RegexProcessorHelper.CreateStartTagProperties(this._factory.PropertiesFactory, match.Value, match.Rule);
            startTagProperties.SetMetaData("OriginalEmbeddedContent", match.Value);
            IEndTagProperties endTagProperties = RegexProcessorHelper.CreateEndTagProperties(this._factory.PropertiesFactory, match.Value, match.Rule);
            endTagProperties.SetMetaData("OriginalEmbeddedContent", match.Value);
            return this._factory.CreateTagPair(startTagProperties, endTagProperties);
        }

        private IPlaceholderTag CreatePlaceholderTag(RegexMatch match)
        {
            IPlaceholderTagProperties placeholderTagProperties = RegexProcessorHelper.CreatePlaceholderTagProperties(this._factory.PropertiesFactory, match.Value, match.Rule);
            placeholderTagProperties.SetMetaData("OriginalEmbeddedContent", match.Value);
            return this._factory.CreatePlaceholderTag(placeholderTagProperties);
        }*/
    }
}