using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class LinePattern : ILinePattern
    {
        private const string LineStartPattern = "^";
        private readonly List<ILinePattern> _possibleFollowingLinePatterns;
        private ILinePattern _mandatoryFollowingLinePattern;
        private readonly Regex _pattern;

        public LinePattern(string pattern)
        {
            _pattern = new Regex(LineStartPattern + pattern);
            _possibleFollowingLinePatterns = new List<ILinePattern>();
        }

        public IEnumerable<ILinePattern> PossibleFollowingLinePatterns => _possibleFollowingLinePatterns;

        public ILinePattern MandatoryFollowingLinePattern => _mandatoryFollowingLinePattern;

        public ILinePattern CanBeFollowedBy(ILinePattern linePattern)
        {
            _possibleFollowingLinePatterns.Add(linePattern);
            return this;
        }

        public ILinePattern MustBeFollowedBy(ILinePattern linePattern)
        {
            _mandatoryFollowingLinePattern = linePattern;
            return this;
        }

        public bool IsApplyingTo(string line)
        {
            return _pattern.IsMatch(line);
        }

        public override string ToString()
        {
            return _pattern.ToString();
        }
    }
}