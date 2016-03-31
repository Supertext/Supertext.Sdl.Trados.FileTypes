using System;
using System.Linq;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
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
        private readonly IParagraphUnitFactory _paragraphUnitFactory;
        private readonly EntryBuilder _entryBuilder;
        private string _originalFilePath;

        //Parsing STATE --- is being changed during parsing 
        private ILineParsingSession _lineParsingSession;
        private IExtendedStreamReader _extendedStreamReader;
        private byte _progressInPercent;
        private int _totalNumberOfLines;

        public PoFileParser(IFileHelper fileHelper, ILineParser lineParser, IUserSettings defaultUserSettings,
            IParagraphUnitFactory paragraphUnitFactory)
        {
            _fileHelper = fileHelper;
            _lineParser = lineParser;
            _userSettings = defaultUserSettings;
            _paragraphUnitFactory = paragraphUnitFactory;
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
            Output.Initialize(DocumentProperties);

            var fileProperties = ItemFactory.CreateFileProperties();
            fileProperties.FileConversionProperties = properties.FileConversionProperties;
            Output.SetFileProperties(fileProperties);

            _paragraphUnitFactory.ItemFactory = ItemFactory;
            _paragraphUnitFactory.PropertiesFactory = PropertiesFactory;

            _originalFilePath = properties.FileConversionProperties.OriginalFilePath;
        }

        public void StartOfInput()
        {
            _lineParsingSession = _lineParser.StartLineParsingSession();
            _extendedStreamReader = _fileHelper.GetExtendedStreamReader(_originalFilePath);
            _totalNumberOfLines =
                _fileHelper.GetExtendedStreamReader(_originalFilePath)
                    .GetLinesWithEofLine()
                    .Count();

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
                ProgressInPercent = (byte) (_extendedStreamReader.CurrentLineNumber*100/_totalNumberOfLines);

                var parseResult = _lineParsingSession.Parse(currentLine);

                _entryBuilder.Add(parseResult, _extendedStreamReader.CurrentLineNumber);

                if (_entryBuilder.CompleteEntry == null || string.IsNullOrEmpty(_entryBuilder.CompleteEntry.MessageId))
                {
                    continue;
                }

                var paragraphUnit = _paragraphUnitFactory.Create(
                    _entryBuilder.CompleteEntry,
                    _userSettings.SourceLineType,
                    _userSettings.IsTargetTextNeeded);

                Output.ProcessParagraphUnit(paragraphUnit);

                return parseResult.LineType != LineType.EndOfFile;
            }

            return false;
        }

        private class EntryBuilder
        {
            private Entry _entryInCreation;
            private Action<string> _collectText;
            private Action<int> _finishCollectingText;
            private Action<string> _collectMessageStringPlurals;
            private Action _finishCollectingMessageStringPlurals;
            private string _tmpMessageStringPluralContent;

            public Entry CompleteEntry { get; private set; }

            public void Add(IParseResult parseResult, int lineNumber)
            {
                CompleteEntry = null;

                if (_collectText != null && parseResult.LineType == LineType.Text)
                {
                    _collectText(parseResult.LineContent);
                    return;
                }

                _collectText = null;

                if (_finishCollectingText != null)
                {
                    _finishCollectingText(lineNumber);
                }

                _finishCollectingText = null;

                if (_collectMessageStringPlurals != null && parseResult.LineType == LineType.MessageStringPlural)
                {
                    _collectMessageStringPlurals(parseResult.LineContent);
                    return;
                }

                _collectMessageStringPlurals = null;

                if (_finishCollectingMessageStringPlurals != null)
                {
                    _finishCollectingMessageStringPlurals();
                }

                _finishCollectingMessageStringPlurals = null;

                if (CompleteEntry != null || _entryInCreation == null)
                {
                    _entryInCreation = new Entry();
                }

                CollectMessage(parseResult, lineNumber);
            }

            private void CollectMessage(IParseResult parseResult, int lineNumber)
            {
                if (parseResult.LineType == LineType.MessageContext)
                {
                    _entryInCreation.MessageContext = parseResult.LineContent;
                }

                if (parseResult.LineType == LineType.MessageId)
                {
                    _entryInCreation.MessageId += parseResult.LineContent;
                    _entryInCreation.MessageIdStart = lineNumber;
                    _collectText = lineContent => _entryInCreation.MessageId += lineContent;
                    _finishCollectingText = currentLineNumber => _entryInCreation.MessageIdEnd = currentLineNumber - 1;
                }

                if (parseResult.LineType == LineType.MessageIdPlural)
                {
                    _entryInCreation.MessageIdPlural += parseResult.LineContent;
                    _collectText = lineContent => _entryInCreation.MessageIdPlural += lineContent;
                    _finishCollectingText = currentLineNumber => { };
                }

                if (parseResult.LineType == LineType.MessageString)
                {
                    _entryInCreation.MessageString += parseResult.LineContent;
                    _entryInCreation.MessageStringStart = lineNumber;
                    _collectText = lineContent => _entryInCreation.MessageString += lineContent;
                    _finishCollectingText = currentLineNumber =>
                    {
                        _entryInCreation.MessageStringEnd = currentLineNumber - 1;
                        SetCompleteEntry();
                    };
                }

                if (parseResult.LineType == LineType.MessageStringPlural)
                {
                    _tmpMessageStringPluralContent = parseResult.LineContent;
                    _collectText = lineContent => _tmpMessageStringPluralContent += lineContent;
                    _finishCollectingText =
                        currentLineNumber => _entryInCreation.MessageStringPlural.Add(_tmpMessageStringPluralContent);

                    _collectMessageStringPlurals = lineContent =>
                    {
                        _tmpMessageStringPluralContent = lineContent;
                        _collectText = sdf => _tmpMessageStringPluralContent += sdf;
                        _finishCollectingText =
                            currentLineNumber => _entryInCreation.MessageStringPlural.Add(_tmpMessageStringPluralContent);
                    };
                    _finishCollectingMessageStringPlurals = SetCompleteEntry;
                }
            }

            private void SetCompleteEntry()
            {
                CompleteEntry = _entryInCreation;
            }
        }
    }
}