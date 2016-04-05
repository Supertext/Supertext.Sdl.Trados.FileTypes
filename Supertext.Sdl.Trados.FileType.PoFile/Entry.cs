using System.Collections.Generic;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class Entry
    {
        public int Start { get; set; }

        public int End { get; set; }

        public List<string> Comments { get; set; } = new List<string>();

        public string MessageContext { get; set; } = string.Empty;

        public string MessageId { get; set; } = string.Empty;

        public string MessageIdPlural { get; set; } = string.Empty;

        public string MessageString { get; set; } = string.Empty;

        public List<string> MessageStringPlurals { get; set; } = new List<string>();

        public int MessageIdStart { get; set; }

        public int MessageIdEnd { get; set; }

        public int MessageStringStart { get; set; }

        public int MessageStringEnd { get; set; }

        public bool IsPluralForm => !string.IsNullOrEmpty(MessageIdPlural) && MessageStringPlurals.Count > 0;

        public string Description => string.Format(PoFileTypeResources.Entry_With_Location, Start , End);
    }
}