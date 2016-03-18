using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class PoFileSniffer : INativeFileSniffer
    {
        private readonly IExtendedStreamReader _extendedStreamReader;
        private readonly ILineParser _lineParser;

        public PoFileSniffer(IExtendedStreamReader extendedStreamReader, ILineParser lineParser)
        {
            _extendedStreamReader = extendedStreamReader;
            _lineParser = lineParser;
        }

        public SniffInfo Sniff(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage,
            INativeTextLocationMessageReporter messageReporter, ISettingsGroup settingsGroup)
        {
            var lineValidationSession = _lineParser.StartLineValidationSession();
            var lineNumber = 0;

            foreach (var line in _extendedStreamReader.GetLinesWithEofLine(nativeFilePath))
            {
                ++lineNumber;

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var isValidLine = lineValidationSession.IsValid(line);

                if (isValidLine)
                {
                    continue;
                }

                var message = line == MarkerLines.EndOfFile
                    ? string.Format(PoFileTypeResources.Sniffer_Unexpected_End_Of_File,
                        lineValidationSession.NextExpectedLineDescription)
                    : PoFileTypeResources.Sniffer_Unexpected_Line;

                messageReporter.ReportMessage(this, nativeFilePath,
                    ErrorLevel.Error, message,
                    lineNumber + ": " + line);

                return new SniffInfo {IsSupported = false};
            }

            return new SniffInfo {IsSupported = true};
        }
    }
}