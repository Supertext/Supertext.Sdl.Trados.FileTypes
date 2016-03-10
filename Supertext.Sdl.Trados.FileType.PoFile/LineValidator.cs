using System;
using System.Linq;
using System.Xml;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class LineValidator : ILineValidator, ILineValidationSession
    {
        private static readonly LinePattern Start;
        private ILinePattern _lastLinePattern;

        static LineValidator()
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

        private static ILinePattern GetApplyingLinePattern(ILinePattern lastLinePattern, string currentLine)
        {
            if (lastLinePattern.MandatoryFollowingLinePattern != null &&
                lastLinePattern.MandatoryFollowingLinePattern.IsApplyingTo(currentLine))
            {
                return lastLinePattern.MandatoryFollowingLinePattern;
            }

            return lastLinePattern.PossibleFollowingLinePatterns.FirstOrDefault(
                linePattern => linePattern.IsApplyingTo(currentLine));
        }

        public ILineValidationSession StartValidationSession()
        {
            _lastLinePattern = Start;
            return this;
        }

        public ILineInfo Check(string line)
        {
            var applyingLinePattern = GetApplyingLinePattern(_lastLinePattern, line);

            if (applyingLinePattern == null)
            {
                return new LineInfo();
            }

            if (applyingLinePattern.Equals(_lastLinePattern.MandatoryFollowingLinePattern) ||
                _lastLinePattern.MandatoryFollowingLinePattern == null)
            {
                _lastLinePattern = applyingLinePattern;
            }

            return new LineInfo
            {
                IsValid = true
            };
        }

        public bool IsEndValid()
        {
            return _lastLinePattern.MandatoryFollowingLinePattern == null;
        }

        private class LineInfo : ILineInfo
        {
            public bool IsValid { get; set; }
        }
    }

    public interface ILineValidator
    {
        ILineValidationSession StartValidationSession();
    }

    public interface ILineValidationSession
    {
        ILineInfo Check(string line);

        bool IsEndValid();
    }

    public interface ILineInfo
    {
        bool IsValid { get; }
    }
}