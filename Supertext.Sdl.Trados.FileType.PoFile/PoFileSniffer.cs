using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class PoFileSniffer : INativeFileSniffer
    {
        private readonly IDotNetFactory _dotNetFactory;
        private readonly ILineParser _lineParser;

        public PoFileSniffer(IDotNetFactory dotNetFactory, ILineParser lineParser)
        {
            _dotNetFactory = dotNetFactory;
            _lineParser = lineParser;
        }

        public SniffInfo Sniff(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage,
            INativeTextLocationMessageReporter messageReporter, ISettingsGroup settingsGroup)
        {
            var lineValidationSession = _lineParser.StartValidationSession();

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
                        "test"),
                    "End of file");

                return new SniffInfo {IsSupported = false};
            }

            return new SniffInfo {IsSupported = true};
        }
    }
}