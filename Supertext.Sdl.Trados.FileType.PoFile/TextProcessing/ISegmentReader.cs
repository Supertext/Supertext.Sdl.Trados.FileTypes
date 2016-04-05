using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Supertext.Sdl.Trados.FileType.PoFile.TextProcessing
{
    public interface ISegmentReader
    {
        string GetTargetText(IEnumerable<ISegmentPair> segmentPairs);

        string GetTargetText(ISegmentPair segmentPair);
    }
}