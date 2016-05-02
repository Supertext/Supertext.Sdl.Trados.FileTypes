using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Supertext.Sdl.Trados.FileType.Utils.TextProcessing
{
    public interface IEmbeddedContentVisitor : IMarkupDataVisitor
    {
        IParagraph GeneratedParagraph { get; }
    }
}