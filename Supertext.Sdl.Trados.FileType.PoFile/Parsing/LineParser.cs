using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Supertext.Sdl.Trados.FileType.Utils.FileHandling;

namespace Supertext.Sdl.Trados.FileType.PoFile.Parsing
{
    public class LineParser : ILineParser, ILineValidationSession, ILineParsingSession
    {
        private static readonly LinePattern BeginOfFile;
        private LinePattern _lastLinePattern;
        private LinePattern _context;

        static LineParser()
        {
            BeginOfFile = new LinePattern(LineType.BeginOfFile, MarkerLines.BeginOfFile, string.Empty);
            var emptyLine = new LinePattern(LineType.Empty, "^$", string.Empty);
            var msgctxt = new LinePattern(LineType.MessageContext, @"msgctxt\s+"".*""", @"""(.*)""");
            var msgid = new LinePattern(LineType.MessageId, @"msgid\s+"".*""", @"""(.*)""");
            var msgidplural = new LinePattern(LineType.MessageIdPlural, @"msgid_plural\s+"".*""", @"""(.*)""");
            var msgstr = new LinePattern(LineType.MessageString, @"msgstr\s+"".*""", @"""(.*)""");
            var msgstrplural = new LinePattern(LineType.MessageStringPlural, @"msgstr\[\d+\]\s+"".*""", @"""(.*)""");
            var text = new LinePattern(LineType.Text, "\"", @"""(.*)""");
            var comment = new LinePattern(LineType.Comment, "#", @"#([\s:,.|]\s*.*)");
            var endOfFile = new LinePattern(LineType.EndOfFile, MarkerLines.EndOfFile, string.Empty);

            BeginOfFile
                .CanBeFollowedBy(msgid)
                .CanBeFollowedBy(comment)
                .CanBeFollowedBy(msgctxt)
                .CanBeFollowedBy(emptyLine);

            msgctxt
                .CanBeFollowedBy(msgid);

            msgid
                .CanBeFollowedBy(msgstr)
                .CanBeFollowedBy(text)
                .CanBeFollowedBy(msgidplural);

            msgstr
                .CanBeFollowedBy(text)
                .CanBeFollowedBy(comment)
                .CanBeFollowedBy(msgctxt)
                .CanBeFollowedBy(msgid)
                .CanBeFollowedBy(endOfFile)
                .CanBeFollowedBy(emptyLine);

            msgidplural
                .CanBeFollowedBy(msgstrplural)
                .CanBeFollowedBy(text);

            msgstrplural
                .CanBeFollowedBy(msgstrplural)
                .CanBeFollowedBySameAs(msgstr);

            text
                .After(msgid)
                .CanBeFollowedBySameAs(msgid)
                .After(msgstr)
                .CanBeFollowedBySameAs(msgstr)
                .After(msgidplural)
                .CanBeFollowedBySameAs(msgidplural)
                .After(msgstrplural)
                .CanBeFollowedBySameAs(msgstrplural);

            comment
                .CanBeFollowedBy(comment)
                .CanBeFollowedBy(msgctxt)
                .CanBeFollowedBy(msgid)
                .CanBeFollowedBy(emptyLine)
                .CanBeFollowedBy(endOfFile);

            emptyLine
                .CanBeIgnored();
        }

        public string NextExpectedLineDescription
            => string.Join(" or ", _lastLinePattern.GetFollowingLinePatterns(_context));

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
            return Parse(line) != null;
        }

        public IParseResult Parse(string line)
        {
            var applyingLinePattern = GetApplyingLinePattern(_context, _lastLinePattern, line);

            if (applyingLinePattern == null)
            {
                return null;
            }

            if (!applyingLinePattern.IsIgnored && !applyingLinePattern.Equals(_lastLinePattern))
            {
                _context = _lastLinePattern;
            }

            _lastLinePattern = applyingLinePattern.IsIgnored ? _lastLinePattern : applyingLinePattern;

            return new ParseResult(applyingLinePattern.LineType, applyingLinePattern.GetContent(line));
        }

        private static LinePattern GetApplyingLinePattern(LinePattern context, LinePattern lastLinePattern,
            string currentLine)
        {
            return lastLinePattern
                .GetFollowingLinePatterns(context)
                .FirstOrDefault(linePattern => linePattern.IsApplyingTo(currentLine));
        }

        private class LinePattern
        {
            private const string StartOfStringPattern = "^";

            private readonly List<LinePattern> _followingLinePatterns;
            private readonly Dictionary<LinePattern, List<LinePattern>> _contextfollowingLinePatterns;
            private readonly Regex _lineRegex;
            private readonly Regex _lineContentRegex;

            private LinePattern _context;

            public LinePattern(LineType lineType, string linePattern, string lineContentPattern)
            {
                LineType = lineType;

                _lineRegex = new Regex(StartOfStringPattern + linePattern);
                _lineContentRegex = new Regex(lineContentPattern);
                _followingLinePatterns = new List<LinePattern>();
                _contextfollowingLinePatterns = new Dictionary<LinePattern, List<LinePattern>>();
            }

            public LineType LineType { get; }

            public bool IsIgnored { get; private set; }

            public LinePattern CanBeFollowedBy(LinePattern linePattern)
            {
                if (_context == null)
                {
                    _followingLinePatterns.Add(linePattern);
                    return this;
                }

                if (!_contextfollowingLinePatterns.ContainsKey(_context))
                {
                    _contextfollowingLinePatterns.Add(_context, new List<LinePattern>());
                }

                _contextfollowingLinePatterns[_context].Add(linePattern);

                return this;
            }

            public LinePattern CanBeFollowedBySameAs(LinePattern linePattern)
            {
                if (_context == null)
                {
                    _followingLinePatterns.AddRange(linePattern._followingLinePatterns);
                    return this;
                }

                if (!_contextfollowingLinePatterns.ContainsKey(_context))
                {
                    _contextfollowingLinePatterns.Add(_context, new List<LinePattern>());
                }

                _contextfollowingLinePatterns[_context].AddRange(linePattern._followingLinePatterns);

                return this;
            }

            public LinePattern After(LinePattern context)
            {
                _context = context;

                return this;
            }

            public void CanBeIgnored()
            {
                IsIgnored = true;
            }

            public IEnumerable<LinePattern> GetFollowingLinePatterns(LinePattern context)
            {
                if (context == null || !_contextfollowingLinePatterns.ContainsKey(context))
                {
                    return _followingLinePatterns;
                }

                return _contextfollowingLinePatterns[context];
            }

            public bool IsApplyingTo(string line)
            {
                return _lineRegex.IsMatch(line);
            }

            public string GetContent(string line)
            {
                var match = _lineContentRegex.Match(line);
                return match.Groups[1].Value;
            }

            public override string ToString()
            {
                return LineType + "(" + _lineRegex + ")";
            }
        }
    }
}