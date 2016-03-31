using System.Collections.Generic;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class Entry
    {
        public List<string> Comments { get; set; } = new List<string>();

        public string MessageContext { get; set; } = string.Empty;

        public string MessageId { get; set; } = string.Empty;

        public string MessageIdPlural { get; set; } = string.Empty;

        public string MessageString { get; set; } = string.Empty;

        public List<string> MessageStringPlural { get; set; } = new List<string>();

        public int MessageIdStart { get; set; }

        public int MessageIdEnd { get; set; }

        public int MessageStringStart { get; set; }

        public int MessageStringEnd { get; set; }

        public string Description => MessageIdStart + "-" + MessageStringEnd;
    }
}