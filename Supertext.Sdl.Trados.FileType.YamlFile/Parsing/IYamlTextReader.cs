using System;

namespace Supertext.Sdl.Trados.FileType.YamlFile.Parsing
{
    public interface IYamlTextReader : IDisposable
    {
        int LineNumber { get; }

        string Path { get; }

        string Value { get; }

        bool Read();

        void Close();
    }

    public interface IBranch
    {
        bool IsKeyNeeded { get; }

        void SetKey(string value);

        string GetNextSubPath();
    }

    public class Map : IBranch
    {
        private string _currentKey;

        public bool IsKeyNeeded => _currentKey == null;

        public void SetKey(string value)
        {
            _currentKey = value;
        }

        public string GetNextSubPath()
        {
            var nextPath = $".{_currentKey}";
            _currentKey = null;
            return nextPath;
        }
    }

    public class Sequence : IBranch
    {
        private int _counter;

        public bool IsKeyNeeded => false;

        public void SetKey(string value)
        {
            //Nothing to do here
        }

        public string GetNextSubPath()
        {
            return $"[{_counter++}]";
        }
    }
}