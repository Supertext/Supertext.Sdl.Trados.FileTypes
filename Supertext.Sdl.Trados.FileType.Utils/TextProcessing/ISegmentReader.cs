using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Supertext.Sdl.Trados.FileType.Utils.TextProcessing
{
    public interface ISegmentReader
    {
        string GetText(IParagraph targetParagraph);

        string GetTargetText(ISegmentPair segmentPair);

        string GetTargetText(IEnumerable<ISegmentPair> segmentPair);

        string GetSourceText(ISegmentPair segmentPair);
    }
}