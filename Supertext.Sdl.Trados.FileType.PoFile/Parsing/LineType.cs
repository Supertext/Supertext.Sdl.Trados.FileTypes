namespace Supertext.Sdl.Trados.FileType.PoFile.Parsing
{
    public enum LineType
    {
        BeginOfFile,
        MessageContext,
        MessageId,
        MessageIdPlural,
        MessageString,
        MessageStringPlural,
        Text,
        Comment,
        EndOfFile,
        Empty
    }
}