using System.Collections.Generic;
using Supertext.Sdl.Trados.FileType.PoFile.Settings;

namespace Supertext.Sdl.Trados.FileType.PoFile.TextProcessing
{
    public interface ITextProcessor
    {
        IList<IFragment> Process(string value);

        void InitializeWith(IEnumerable<MatchRule> matchRules);
    }
}