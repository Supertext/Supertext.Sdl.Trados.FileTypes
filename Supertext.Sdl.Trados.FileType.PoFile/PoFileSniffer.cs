using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class PoFileSniffer : INativeFileSniffer
    {
        private readonly IDotNetFactory _dotNetFactory;

        public PoFileSniffer(IDotNetFactory dotNetFactory)
        {
            _dotNetFactory = dotNetFactory;
        }

        private static LinePattern GetLinePatternRules()
        {
            var start = new LinePattern(string.Empty);
            var msgid = new LinePattern(@"msgid\s+"".*""");
            var msgstr = new LinePattern(@"msgstr\s+"".*""");
            var text = new LinePattern("\"");
            var comment = new LinePattern("#");

            start
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

            return start;
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

        public SniffInfo Sniff(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage,
            INativeTextLocationMessageReporter messageReporter, ISettingsGroup settingsGroup)
        {
            var lastLinePattern = GetLinePatternRules();

            using (var reader = _dotNetFactory.CreateStreamReader(nativeFilePath))
            {
                string currentLine;
                var lineNumber = 0;

                while ((currentLine = reader.ReadLine()) != null)
                {
                    ++lineNumber;

                    if (string.IsNullOrWhiteSpace(currentLine))
                    {
                        continue;
                    }

                    var currentLinePattern = GetApplyingLinePattern(lastLinePattern, currentLine);

                    if (currentLinePattern == null)
                    {
                        messageReporter.ReportMessage(this, nativeFilePath,
                            ErrorLevel.Error, PoFileTypeResources.Sniffer_Unexpected_Line,
                            lineNumber + ": " + currentLine);

                        return new SniffInfo {IsSupported = false};
                    }

                    if (lastLinePattern.MandatoryFollowingLinePattern != null &&
                        !currentLinePattern.Equals(lastLinePattern.MandatoryFollowingLinePattern))
                    {
                        continue;
                    }

                    lastLinePattern = currentLinePattern;
                }
            }

            if (lastLinePattern.MandatoryFollowingLinePattern != null)
            {
                messageReporter.ReportMessage(this, nativeFilePath,
                    ErrorLevel.Error,
                    string.Format(PoFileTypeResources.Sniffer_Unexpected_End_Of_File,
                        lastLinePattern.MandatoryFollowingLinePattern),
                    "End of file");

                return new SniffInfo {IsSupported = false};
            }

            return new SniffInfo {IsSupported = true};
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