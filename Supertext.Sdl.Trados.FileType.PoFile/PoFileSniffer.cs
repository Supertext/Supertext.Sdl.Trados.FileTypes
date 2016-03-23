using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class PoFileSniffer : INativeFileSniffer
    {
        private readonly IFileHelper _fileHelper;
        private readonly ILineParser _lineParser;

        public PoFileSniffer(IFileHelper fileHelper, ILineParser lineParser)
        {
            _fileHelper = fileHelper;
            _lineParser = lineParser;
        }

        public SniffInfo Sniff(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage,
            INativeTextLocationMessageReporter messageReporter, ISettingsGroup settingsGroup)
        {
            var lineValidationSession = _lineParser.StartLineValidationSession();
            var lineNumber = 0;
            using (var extendedStreamReader = _fileHelper.GetExtendedStreamReader(nativeFilePath))
            {
                foreach (var line in extendedStreamReader.GetLinesWithEofLine())
                {
                    ++lineNumber;

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
            }

            return new SniffInfo {IsSupported = true};
        }
    }
}