using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Supertext.Sdl.Trados.FileType.PoFile.TextProcessing
{
    public class Fragment : IFragment
    {
        public Fragment(InlineType inlineType, string content)
            : this(inlineType, content, SegmentationHint.Undefined, false)
        {
        }

        public Fragment(InlineType inlineType, string content, SegmentationHint segmentationHint,
            bool isContentTranslatable)
        {
            InlineType = inlineType;
            Content = content;
            SegmentationHint = segmentationHint;
            IsContentTranslatable = isContentTranslatable;
        }

        public InlineType InlineType { get; }

        public string Content { get; }

        public SegmentationHint SegmentationHint { get; }

        public bool IsContentTranslatable { get; }
    }
}