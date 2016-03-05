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

        public SniffInfo Sniff(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage,
            INativeTextLocationMessageReporter messageReporter, ISettingsGroup settingsGroup)
        {
            var sniffInfo = new SniffInfo();

            var lastLinePattern = GetLineRules();

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

                    if (lastLinePattern.MandatoryFollowingLinePattern != null &&
                        lastLinePattern.MandatoryFollowingLinePattern.IsApplyingTo(currentLine))
                    {
                        lastLinePattern = lastLinePattern.MandatoryFollowingLinePattern;
                        continue;
                    }
                 
                    var currentLinePattern = lastLinePattern.PossibleFollowingLinePatterns.FirstOrDefault(
                            linePattern => linePattern.IsApplyingTo(currentLine));
                    

                    if (lastLinePattern.MandatoryFollowingLinePattern != null && currentLinePattern != null)
                    {
                        continue;
                    }

                    if (lastLinePattern.MandatoryFollowingLinePattern == null && currentLinePattern != null)
                    {
                        lastLinePattern = currentLinePattern;
                        continue;
                    }

                    sniffInfo.IsSupported = false;
                    messageReporter.ReportMessage(this, nativeFilePath,
                        ErrorLevel.Error, PoFileTypeResources.Sniffer_Message,
                        lineNumber + ": " + currentLine);

                    return sniffInfo;
                }

                if (lastLinePattern.MandatoryFollowingLinePattern != null)
                {
                    sniffInfo.IsSupported = false;
                    messageReporter.ReportMessage(this, nativeFilePath,
                        ErrorLevel.Error, PoFileTypeResources.Sniffer_Message,
                        "end" );

                    return sniffInfo;
                }

                sniffInfo.IsSupported = true;
                return sniffInfo;
            }
        }

        private static LinePattern GetLineRules()
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

        private class LinePattern
        {
            private const string StartPattern = "^";
            private readonly List<LinePattern> _possibleFollowingLinePatterns;
            private LinePattern _mandatoryFollowingLinePattern;
            private readonly Regex _pattern;

            public LinePattern(string pattern)
            {
                _pattern = new Regex(StartPattern + pattern);
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
        }
    }
}