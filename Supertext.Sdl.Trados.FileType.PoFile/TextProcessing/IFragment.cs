using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Supertext.Sdl.Trados.FileType.PoFile.TextProcessing
{
    public interface IFragment
    {
        InlineType InlineType { get; }

        string Content { get; }

        SegmentationHint SegmentationHint { get; }

        bool IsContentTranslatable { get; }
    }
}