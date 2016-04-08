using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Supertext.Sdl.Trados.FileType.PoFile.Paragraphing
{
    public interface IEmbeddedContentVisitor : IMarkupDataVisitor
    {
        IParagraph GeneratedParagraph { get; }
    }
}