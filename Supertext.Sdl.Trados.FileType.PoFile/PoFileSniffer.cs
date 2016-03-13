using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;

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

            foreach (var line in _fileHelper.ReadLines(nativeFilePath))
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

                messageReporter.ReportMessage(this, nativeFilePath,
                    ErrorLevel.Error, PoFileTypeResources.Sniffer_Unexpected_Line,
                    lineNumber + ": " + line);

                return new SniffInfo {IsSupported = false};
            }


            if (!lineValidationSession.IsValid(LineType.EndOfFile.ToString()))
            {
                messageReporter.ReportMessage(this, nativeFilePath,
                    ErrorLevel.Error,
                    string.Format(PoFileTypeResources.Sniffer_Unexpected_End_Of_File,
                        lineValidationSession.NextExpectedLineDescription),
                    "End of file");

                return new SniffInfo {IsSupported = false};
            }

            return new SniffInfo {IsSupported = true};
        }
    }
}