namespace Supertext.Sdl.Trados.FileType.YamlFile.Parsing
{
    public class Sequence : IBranch
    {
        private int _counter;

        public bool IsComplete => true;

        public void SetScalar(string value)
        {
            //Nothing to do here
        }

        public void Continue()
        {
            ++_counter;
        }

        public string GetSubPath()
        {
            return $"[{_counter}]";
        }
    }
}