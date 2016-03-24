using System.Globalization;
using System.Linq;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class PoFileParser : AbstractBilingualParser, INativeContentCycleAware, ISettingsAware
    {
        private readonly IFileHelper _fileHelper;
        private readonly ILineParser _lineParser;
        private readonly IUserSettings _userSettings;
        private readonly EntryBuilder _entryBuilder;
        private IPersistentFileConversionProperties _fileConversionProperties;

        //Parsing STATE --- is being changed during parsing 
        private ILineParsingSession _lineParsingSession;
        private IExtendedStreamReader _extendedStreamReader;
        private byte _progressInPercent;
        private int _totalNumberOfLines;
        private int _currentLineNumber;

        public PoFileParser(IFileHelper fileHelper, ILineParser lineParser, IUserSettings defaultUserSettings)
        {
            _fileHelper = fileHelper;
            _lineParser = lineParser;
            _userSettings = defaultUserSettings;
            _entryBuilder = new EntryBuilder();
        }

        public byte ProgressInPercent
        {
            get { return _progressInPercent; }
            set
            {
                _progressInPercent = value;
                OnProgress(_progressInPercent);
            }
        }

        public void SetFileProperties(IFileProperties properties)
        {
            _fileConversionProperties = properties.FileConversionProperties;

            Output.Initialize(DocumentProperties);

            var fileProperties = ItemFactory.CreateFileProperties();
            fileProperties.FileConversionProperties = _fileConversionProperties;
            Output.SetFileProperties(fileProperties);
        }

        public void StartOfInput()
        {
            _lineParsingSession = _lineParser.StartLineParsingSession();
            _extendedStreamReader = _fileHelper.GetExtendedStreamReader(_fileConversionProperties.OriginalFilePath);
            _totalNumberOfLines =
                _fileHelper.GetExtendedStreamReader(_fileConversionProperties.OriginalFilePath)
                    .GetLinesWithEofLine()
                    .Count();
            _currentLineNumber = 0;

            ProgressInPercent = 0;
        }

        public void EndOfInput()
        {
            Output.FileComplete();
            Output.Complete();

            _extendedStreamReader.Close();
            _extendedStreamReader.Dispose();
            _extendedStreamReader = null;
        }

        public void InitializeSettings(ISettingsBundle settingsBundle, string configurationId)
        {
            _userSettings.PopulateFromSettingsBundle(settingsBundle, configurationId);
        }

        public override bool ParseNext()
        {
            string currentLine;

            while ((currentLine = _extendedStreamReader.ReadLineWithEofLine()) != null)
            {
                ProgressInPercent = (byte) (++_currentLineNumber*100/_totalNumberOfLines);

                var parseResult = _lineParsingSession.Parse(currentLine);

                _entryBuilder.Add(parseResult, _currentLineNumber);

                if (_entryBuilder.CompleteEntry == null || string.IsNullOrEmpty(_entryBuilder.CompleteEntry.MessageId))
                {
                    continue;
                }

                var paragraphUnit = CreateParagraphUnit(
                    _entryBuilder.CompleteEntry,
                    _userSettings.SourceLineType,
                    _userSettings.IsTargetTextNeeded);

                Output.ProcessParagraphUnit(paragraphUnit);

                return parseResult.LineType != LineType.EndOfFile;
            }

            return false;
        }

        private IParagraphUnit CreateParagraphUnit(Entry entry, LineType sourceLineType, bool isTargetTextNeeded)
        {
            var paragraphUnit = ItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);
            var segmentPairProperties = ItemFactory.CreateSegmentPairProperties();

            var sourceText = sourceLineType == LineType.MessageString ? entry.MessageString : entry.MessageId;

            var sourceSegment = ItemFactory.CreateSegment(segmentPairProperties);
            sourceSegment.Add(CreateText(sourceText));
            paragraphUnit.Source.Add(sourceSegment);

            if (isTargetTextNeeded)
            {
                var targetSegment = ItemFactory.CreateSegment(segmentPairProperties);
                targetSegment.Add(CreateText(entry.MessageString));
                paragraphUnit.Target.Add(targetSegment);
            }

            paragraphUnit.Properties.Contexts = CreateContextProperties(entry);

            return paragraphUnit;
        }

        private IContextProperties CreateContextProperties(Entry entry)
        {
            var contextProperties = PropertiesFactory.CreateContextProperties();
            var contextInfo = PropertiesFactory.CreateContextInfo(StandardContextTypes.Paragraph);
            contextInfo.Purpose = ContextPurpose.Information;

            var contextId = PropertiesFactory.CreateContextInfo(ContextKeys.LocationContextType);
            contextId.SetMetaData(ContextKeys.MessageIdStart,
                entry.MessageIdStart.ToString(CultureInfo.InvariantCulture));
            contextId.SetMetaData(ContextKeys.MessageIdEnd, entry.MessageIdEnd.ToString(CultureInfo.InvariantCulture));
            contextId.SetMetaData(ContextKeys.MessageStringStart,
                entry.MessageStringStart.ToString(CultureInfo.InvariantCulture));
            contextId.SetMetaData(ContextKeys.MessageStringEnd,
                entry.MessageStringEnd.ToString(CultureInfo.InvariantCulture));

            contextProperties.Contexts.Add(contextInfo);
            contextProperties.Contexts.Add(contextId);
            return contextProperties;
        }

        private IText CreateText(string value)
        {
            var textProperties = PropertiesFactory.CreateTextProperties(value);

            return ItemFactory.CreateText(textProperties);
        }

        // TODO move extracted class and test
        private class EntryBuilder
        {
            private bool _collectingMessageId;
            private bool _collectingMessagString;
            private Entry _entryInCreation;

            public Entry CompleteEntry { get; private set; }

            public void Add(IParseResult parseResult, int lineNumber)
            {
                CollectData(parseResult, lineNumber);

                if (parseResult.LineType == LineType.MessageId)
                {
                    _entryInCreation = new Entry();
                    _entryInCreation.MessageId += parseResult.LineContent;
                    _entryInCreation.MessageIdStart = lineNumber;
                    _collectingMessageId = true;
                }

                if (parseResult.LineType == LineType.MessageString)
                {
                    _entryInCreation.MessageString += parseResult.LineContent;
                    _entryInCreation.MessageStringStart = lineNumber;
                    _collectingMessagString = true;
                }
            }

            private void CollectData(IParseResult parseResult, int lineNumber)
            {
                CompleteEntry = null;

                if (_collectingMessageId && parseResult.LineType != LineType.Text)
                {
                    _collectingMessageId = false;
                    _entryInCreation.MessageIdEnd = lineNumber - 1;
                }
                else if (_collectingMessagString && parseResult.LineType != LineType.Text)
                {
                    _collectingMessagString = false;
                    _entryInCreation.MessageStringEnd = lineNumber - 1;
                    CompleteEntry = _entryInCreation;
                }
                else if (_collectingMessageId && parseResult.LineType == LineType.Text)
                {
                    _entryInCreation.MessageId += parseResult.LineContent;
                }
                else if (_collectingMessagString && parseResult.LineType == LineType.Text)
                {
                    _entryInCreation.MessageString += parseResult.LineContent;
                }
            }
        }

        private class Entry
        {
            public string MessageId { get; set; } = string.Empty;

            public string MessageString { get; set; } = string.Empty;

            public int MessageIdStart { get; set; }

            public int MessageIdEnd { get; set; }

            public int MessageStringStart { get; set; }

            public int MessageStringEnd { get; set; }
        }
    }
}