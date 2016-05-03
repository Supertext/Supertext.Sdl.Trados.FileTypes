using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Settings
{
    public interface IParsingSettings
    {
        bool IsPathFilteringEnabled { get; }

        ObservableList<string> PathPatterns { get; }
    }
}
