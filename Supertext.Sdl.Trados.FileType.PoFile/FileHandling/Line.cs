namespace Supertext.Sdl.Trados.FileType.PoFile.FileHandling
{
    public struct Line
    {
        public int Number;
        public string Content;

        public Line(int number, string content)
        {
            Number = number;
            Content = content;
        }
    }
}
