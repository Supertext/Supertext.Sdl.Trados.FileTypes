using System.Collections.Generic;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Settings
{
    public interface IParsingSettings
    {
        bool IsPathFilteringEnabled { get; }

        IList<string> PathPatterns { get; }
    }
}
