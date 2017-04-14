using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Supertext.Sdl.Trados.FileType.JsonFile.TextProcessing
{
    public interface IParagraphUnitFactory
    {
        IDocumentItemFactory ItemFactory { get; set; }

        IPropertiesFactory PropertiesFactory { get; set; }

        IParagraphUnit Create(string sourcePath, string sourceValue, string targetPath, string targetValue);
    }
}