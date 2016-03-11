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

            using (var reader = _fileHelper.CreateStreamReader(nativeFilePath))
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

                    var isValidLine = lineValidationSession.Check(currentLine);

                    if (isValidLine)
                    {
                        continue;
                    }

                    messageReporter.ReportMessage(this, nativeFilePath,
                        ErrorLevel.Error, PoFileTypeResources.Sniffer_Unexpected_Line,
                        lineNumber + ": " + currentLine);

                    return new SniffInfo {IsSupported = false};
                }
            }

            if (!lineValidationSession.IsEndValid())
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