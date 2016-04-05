using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.Parsing;

namespace Supertext.Sdl.Trados.FileType.PoFile.ElementFactories
{
    public interface IParagraphUnitFactory
    {
        IDocumentItemFactory ItemFactory { get; set; }

        IPropertiesFactory PropertiesFactory { get; set; }

        IParagraphUnit Create(Entry entry, LineType sourceLineType, bool isTargetTextNeeded);
    }
}