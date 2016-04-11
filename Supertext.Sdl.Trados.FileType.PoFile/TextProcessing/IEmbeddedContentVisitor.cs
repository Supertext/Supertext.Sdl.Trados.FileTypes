using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Supertext.Sdl.Trados.FileType.PoFile.TextProcessing
{
    public interface IEmbeddedContentVisitor : IMarkupDataVisitor
    {
        IParagraph GeneratedParagraph { get; }
    }
}