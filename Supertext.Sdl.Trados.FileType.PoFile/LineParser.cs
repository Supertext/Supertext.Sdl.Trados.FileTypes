using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class LineParser : ILineParser, ILineValidationSession, ILineParsingSession
    {
        private static readonly LinePattern BeginOfFile;
        private LinePattern _lastLinePattern;

        static LineParser()
        {
            var beginOfFile = new LinePattern(LineType.BeginOfFile, LineType.BeginOfFile.ToString(), string.Empty);
            var msgid = new LinePattern(LineType.MessageId, @"msgid\s+"".*""", @"""(.*)""");
            var msgstr = new LinePattern(LineType.MessageString, @"msgstr\s+"".*""", @"""(.*)""");
            var text = new LinePattern(LineType.Text, "\"", @"""(.*)""");
            var comment = new LinePattern(LineType.Comment, "#", @"#[\s:,.|]\s*(.*)");
            var endOfFile = new LinePattern(LineType.EndOfFile, LineType.EndOfFile.ToString(), string.Empty);

            beginOfFile
                .MustBeFollowedBy(msgid)
                .CanBeFollowedBy(comment);

            msgid
                .MustBeFollowedBy(msgstr)
                .CanBeFollowedBy(text);

            msgstr
                .CanBeFollowedBy(text)
                .CanBeFollowedBy(comment)
                .CanBeFollowedBy(msgid)
                .CanBeFollowedBy(endOfFile);

            text
                .CanBeFollowedBy(text)
                .CanBeFollowedBy(comment)
                .CanBeFollowedBy(msgstr)
                .CanBeFollowedBy(msgid)
                .CanBeFollowedBy(endOfFile);

            comment
                .CanBeFollowedBy(comment)
                .CanBeFollowedBy(msgid);

            BeginOfFile = beginOfFile;
        }

        public string NextExpectedLineDescription
            => _lastLinePattern.ExpectedFollowingLinePattern?.ToString() ?? string.Empty;

        public ILineValidationSession StartLineValidationSession()
        {
            _lastLinePattern = BeginOfFile;
            return this;
        }

        public ILineParsingSession StartLineParsingSession()
        {
            _lastLinePattern = BeginOfFile;
            return this;
        }

        public bool IsValid(string line)
        {
            var applyingLinePattern = GetApplyingLinePattern(_lastLinePattern, line);

            if (applyingLinePattern == null)
            {
                return false;
            }

            if (applyingLinePattern.Equals(_lastLinePattern.ExpectedFollowingLinePattern) ||
                _lastLinePattern.ExpectedFollowingLinePattern == null)
            {
                _lastLinePattern = applyingLinePattern;
            }

            return true;
        }

        public bool IsEndValid()
        {
            return IsValid(LineType.EndOfFile.ToString());
        }

        public IParseResult Parse(string line)
        {
            var applyingLinePattern = GetApplyingLinePattern(_lastLinePattern, line);

            if (applyingLinePattern == null)
            {
                return null;
            }

            _lastLinePattern = applyingLinePattern;

            return new ParseResult(applyingLinePattern.LineType, applyingLinePattern.GetContent(line));
        }

        private static LinePattern GetApplyingLinePattern(LinePattern lastLinePattern, string currentLine)
        {
            if (lastLinePattern.ExpectedFollowingLinePattern != null &&
                lastLinePattern.ExpectedFollowingLinePattern.IsApplyingTo(currentLine))
            {
                return lastLinePattern.ExpectedFollowingLinePattern;
            }

            return lastLinePattern.PossibleFollowingLinePatterns.FirstOrDefault(
                linePattern => linePattern.IsApplyingTo(currentLine));
        }

        private class LinePattern
        {
            private const string StartOfStringPattern = "^";
            private readonly List<LinePattern> _possibleFollowingLinePatterns;
            private readonly Regex _lineStartRegex;
            private readonly Regex _lineContentRegex;
            private LinePattern _expectedFollowingLinePattern;

            public LinePattern(LineType lineType, string lineStartPattern, string lineContentPattern)
            {
                LineType = lineType;

                _lineStartRegex = new Regex(StartOfStringPattern + lineStartPattern);
                _lineContentRegex = new Regex(lineContentPattern);
                _possibleFollowingLinePatterns = new List<LinePattern>();
            }

            public LineType LineType { get; }

            public IEnumerable<LinePattern> PossibleFollowingLinePatterns => _possibleFollowingLinePatterns;

            public LinePattern ExpectedFollowingLinePattern => _expectedFollowingLinePattern;

            public LinePattern CanBeFollowedBy(LinePattern linePattern)
            {
                _possibleFollowingLinePatterns.Add(linePattern);
                return this;
            }

            public LinePattern MustBeFollowedBy(LinePattern linePattern)
            {
                _expectedFollowingLinePattern = linePattern;
                return this;
            }

            public bool IsApplyingTo(string line)
            {
                return _lineStartRegex.IsMatch(line);
            }

            public string GetContent(string line)
            {
                var match = _lineContentRegex.Match(line);
                return match.Groups[1].Value;
            }

            public override string ToString()
            {
                return LineType + "(" + _lineStartRegex + ")";
            }
        }
    }
}