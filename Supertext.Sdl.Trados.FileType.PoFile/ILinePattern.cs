using System.Collections.Generic;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface ILinePattern
    {
        IEnumerable<ILinePattern> PossibleFollowingLinePatterns { get; }

        ILinePattern MandatoryFollowingLinePattern { get; }

        ILinePattern CanBeFollowedBy(ILinePattern linePattern);

        ILinePattern MustBeFollowedBy(ILinePattern linePattern);

        bool IsApplyingTo(string line);
    }
}