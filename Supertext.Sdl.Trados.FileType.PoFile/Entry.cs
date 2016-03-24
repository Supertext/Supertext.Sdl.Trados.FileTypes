namespace Supertext.Sdl.Trados.FileType.PoFile
{
    internal class Entry
    {
        public string MessageId { get; set; } = string.Empty;

        public string MessageString { get; set; } = string.Empty;

        public int MessageIdStart { get; set; }

        public int MessageIdEnd { get; set; }

        public int MessageStringStart { get; set; }

        public int MessageStringEnd { get; set; }
    }
}