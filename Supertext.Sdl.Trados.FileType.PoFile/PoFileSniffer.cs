using System.IO;
using System.Linq;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class PoFileSniffer : INativeFileSniffer
    {
        private readonly IDotNetFactory _dotNetFactory;
        private readonly ILinePattern _startingLinePattern;

        public PoFileSniffer(IDotNetFactory dotNetFactory, ILinePattern startingLinePattern)
        {
            _dotNetFactory = dotNetFactory;
            _startingLinePattern = startingLinePattern;
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

        public SniffInfo Sniff(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage,
            INativeTextLocationMessageReporter messageReporter, ISettingsGroup settingsGroup)
        {
            var lastLinePattern = _startingLinePattern;

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
    }
}