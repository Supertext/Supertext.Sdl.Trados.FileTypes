using System.Collections.Generic;

namespace Supertext.Sdl.Trados.FileType.PoFile.TextProcessing
{
    public interface ITextProcessor
    {
        IList<Fragment> Process(string value, Dictionary<string, InlineType> embeddedContentPatterns);
    }
}