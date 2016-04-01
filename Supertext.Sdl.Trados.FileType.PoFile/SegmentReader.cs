using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface ISegmentReader
    {
        string GetTargetText(IEnumerable<ISegmentPair> segmentPairs);

        string GetTargetText(ISegmentPair segmentPair);
    }

    public class SegmentReader : ISegmentReader, IMarkupDataVisitor
    {
        private StringBuilder _textStringBuilder;

        public string GetTargetText(IEnumerable<ISegmentPair> segmentPairs)
        {
            _textStringBuilder = new StringBuilder();

            foreach (var segmentPair in segmentPairs)
            {
                VisitChildren(segmentPair.Target);
            }

            return _textStringBuilder.ToString();
        }

        public string GetTargetText(ISegmentPair segmentPair)
        {
            _textStringBuilder = new StringBuilder();

            VisitChildren(segmentPair.Target);

            return _textStringBuilder.ToString();
        }

        private void VisitChildren(IAbstractMarkupDataContainer container)
        {
            foreach (var segment in container)
            {
                segment.AcceptVisitor(this);
            }
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            _textStringBuilder.Append(tagPair.StartTagProperties.TagContent);
            VisitChildren(tagPair);
            _textStringBuilder.Append(tagPair.EndTagProperties.TagContent);
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            _textStringBuilder.Append(tag.TagProperties.TagContent);
        }

        public void VisitText(IText text)
        {
            _textStringBuilder.Append(text.Properties.Text);
        }

        public void VisitSegment(ISegment segment)
        {
            VisitChildren(segment);
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
        }

        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            VisitChildren(commentMarker);
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
        }
    }
}