using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;
using Supertext.Sdl.Trados.FileType.PoFile.Parsing;
using Supertext.Sdl.Trados.FileType.PoFile.Resources;

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
            using (var extendedStreamReader = _fileHelper.GetExtendedStreamReader(nativeFilePath))
            {
                string currentLine;
                while ((currentLine = extendedStreamReader.ReadLineWithEofLine()) != null)
                {
                    var isValidLine = lineValidationSession.IsValid(currentLine);

                    if (isValidLine)
                    {
                        continue;
                    }

                    var message = string.Format(
                        PoFileTypeResources.Sniffer_Unexpected_Line,
                        extendedStreamReader.CurrentLineNumber + ": " + currentLine,
                        lineValidationSession.NextExpectedLineDescription);

                    messageReporter.ReportMessage(this, nativeFilePath,
                        ErrorLevel.Error, message,
                        extendedStreamReader.CurrentLineNumber + ": " + currentLine);

                    return new SniffInfo {IsSupported = false};
                }
            }

            return new SniffInfo {IsSupported = true};
        }
    }
}