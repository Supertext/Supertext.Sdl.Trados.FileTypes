namespace Supertext.Sdl.Trados.FileType.YamlFile.Parsing
{
    public class Map : IBranch
    {
        private string _currentKey;

        public bool IsComplete => !string.IsNullOrEmpty(_currentKey);

        public void SetScalar(string value)
        {
            _currentKey = value;
        }

        public void Continue()
        {
            _currentKey = null;
        }

        public string GetSubPath()
        {
            return $".{_currentKey}";
        }
    }
}