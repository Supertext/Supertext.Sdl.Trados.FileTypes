using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class LineParser : ILineParser, ILineValidationSession
    {
        private static readonly LinePattern Start;
        private LinePattern _lastLinePattern;

        static LineParser()
        {
            Start = new LinePattern(string.Empty);
            var msgid = new LinePattern(@"msgid\s+"".*""");
            var msgstr = new LinePattern(@"msgstr\s+"".*""");
            var text = new LinePattern("\"");
            var comment = new LinePattern("#");

            Start
                .MustBeFollowedBy(msgid)
                .CanBeFollowedBy(comment);

            msgid
                .MustBeFollowedBy(msgstr)
                .CanBeFollowedBy(text);

            msgstr
                .CanBeFollowedBy(text)
                .CanBeFollowedBy(comment)
                .CanBeFollowedBy(msgid);

            text
                .CanBeFollowedBy(text)
                .CanBeFollowedBy(comment)
                .CanBeFollowedBy(msgstr)
                .CanBeFollowedBy(msgid);

            comment
                .CanBeFollowedBy(comment)
                .CanBeFollowedBy(msgid);
        }

        public ILineValidationSession StartValidationSession()
        {
            _lastLinePattern = Start;
            return this;
        }

        public bool Check(string line)
        {
            var applyingLinePattern = GetApplyingLinePattern(_lastLinePattern, line);

            if (applyingLinePattern == null)
            {
                return false;
            }

            if (applyingLinePattern.Equals(_lastLinePattern.MandatoryFollowingLinePattern) ||
                _lastLinePattern.MandatoryFollowingLinePattern == null)
            {
                _lastLinePattern = applyingLinePattern;
            }

            return true;
        }

        public bool IsEndValid()
        {
            return _lastLinePattern.MandatoryFollowingLinePattern == null;
        }

        private static LinePattern GetApplyingLinePattern(LinePattern lastLinePattern, string currentLine)
        {
            if (lastLinePattern.MandatoryFollowingLinePattern != null &&
                lastLinePattern.MandatoryFollowingLinePattern.IsApplyingTo(currentLine))
            {
                return lastLinePattern.MandatoryFollowingLinePattern;
            }

            return lastLinePattern.PossibleFollowingLinePatterns.FirstOrDefault(
                linePattern => linePattern.IsApplyingTo(currentLine));
        }

        private class LinePattern
        {
            private const string LineStartPattern = "^";
            private readonly List<LinePattern> _possibleFollowingLinePatterns;
            private LinePattern _mandatoryFollowingLinePattern;
            private readonly Regex _pattern;

            public LinePattern(string pattern)
            {
                _pattern = new Regex(LineStartPattern + pattern);
                _possibleFollowingLinePatterns = new List<LinePattern>();
            }

            public IEnumerable<LinePattern> PossibleFollowingLinePatterns => _possibleFollowingLinePatterns;

            public LinePattern MandatoryFollowingLinePattern => _mandatoryFollowingLinePattern;

            public LinePattern CanBeFollowedBy(LinePattern linePattern)
            {
                _possibleFollowingLinePatterns.Add(linePattern);
                return this;
            }

            public LinePattern MustBeFollowedBy(LinePattern linePattern)
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
}