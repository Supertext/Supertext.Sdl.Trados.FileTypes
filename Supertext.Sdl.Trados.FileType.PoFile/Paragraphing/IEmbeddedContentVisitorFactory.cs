using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.PoFile.Paragraphing
{
    public interface IEmbeddedContentVisitorFactory
    {
        IEmbeddedContentVisitor CreateVisitor(IDocumentItemFactory itemFactory, IPropertiesFactory propertiesFactory,
            ITextProcessor textProcessor);
    }
}