using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface ISegmentReader
    {
        string GetTargetText(IEnumerable<ISegmentPair> segmentPairs);
    }

    public class SegmentReader : ISegmentReader, IMarkupDataVisitor
    {
        private StringBuilder _textStringBuilder;

        public string GetTargetText(IEnumerable<ISegmentPair> segmentPairs)
        {
            _textStringBuilder = new StringBuilder();

            foreach (var segmentPair in segmentPairs)
            {
                foreach (var segment in segmentPair.Target)
                {
                    segment.AcceptVisitor(this);
                }
            }

            return _textStringBuilder.ToString();
        }

        public void VisitTagPair(ITagPair tagPair)
        {
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
        }

        public void VisitText(IText text)
        {
            _textStringBuilder.Append(text.Properties.Text);
        }

        public void VisitSegment(ISegment segment)
        {
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
        }

        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
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