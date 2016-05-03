using System.Collections.Generic;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Settings
{
    public interface IParsingSettings
    {
        IEnumerable<string> PathPatterns { get; }
    }

    public class ParsingSettings : IParsingSettings
    {
        public IEnumerable<string> PathPatterns { get; }
    }
}
