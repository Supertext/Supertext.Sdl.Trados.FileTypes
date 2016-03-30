namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class Entry
    {
        public string MessageContext { get; set; } = string.Empty;

        public string MessageId { get; set; } = string.Empty;

        public string MessageString { get; set; } = string.Empty;

        public int MessageIdStart { get; set; }

        public int MessageIdEnd { get; set; }

        public int MessageStringStart { get; set; }

        public int MessageStringEnd { get; set; }

        public string Description => MessageIdStart + "-" + MessageStringEnd;
    }
}