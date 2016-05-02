using System.Collections.Generic;
using Supertext.Sdl.Trados.FileType.Utils.Settings;

namespace Supertext.Sdl.Trados.FileType.Utils.TextProcessing
{
    public interface ITextProcessor
    {
        IList<IFragment> Process(string value);

        void InitializeWith(IEnumerable<MatchRule> matchRules);
    }
}