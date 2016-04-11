using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Supertext.Sdl.Trados.FileType.PoFile.TextProcessing
{
    public interface IEmbeddedContentVisitorFactory
    {
        IEmbeddedContentVisitor CreateVisitor(IDocumentItemFactory itemFactory, IPropertiesFactory propertiesFactory,
            ITextProcessor textProcessor);
    }
}