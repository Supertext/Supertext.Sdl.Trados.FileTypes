using System.Collections.Generic;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Settings
{
    public interface IParsingSettings
    {
        IEnumerable<string> PathPatterns { get; set; }
    }
}
