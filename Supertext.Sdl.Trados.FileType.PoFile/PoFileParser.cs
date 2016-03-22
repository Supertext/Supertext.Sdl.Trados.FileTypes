using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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
        private readonly EntryBuilder _entryBuilder;
        private IPersistentFileConversionProperties _fileConversionProperties;

        //Parsing STATE --- is being changed during parsing 
        private ILineParsingSession _lineParsingSession;
        private IExtendedStreamReader _extendedStreamReader;
        private byte _progressInPercent;
        private int _totalNumberOfLines;
        private int _numberOfProcessedLines;

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
            _numberOfProcessedLines = 0;

            ProgressInPercent = 0;
        }

        public void EndOfInput()
        {
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
                ProgressInPercent = (byte)(++_numberOfProcessedLines * 100 / _totalNumberOfLines);

                if (string.IsNullOrEmpty(currentLine))
                {
                    continue;
                }

                var parseResult = _lineParsingSession.Parse(currentLine);

                _entryBuilder.Add(parseResult);

                if (_entryBuilder.CompleteEntry == null)
                {
                    continue;
                }

                CreateParagraphUnit(_entryBuilder.CompleteEntry);

                return parseResult.LineType != LineType.EndOfFile;
            }

            return false;
        }

        private void CreateParagraphUnit(Entry entry)
        {
            var paragraphUnit = ItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);
            var segmentPairProperties = ItemFactory.CreateSegmentPairProperties();

            var sourceSegment = ItemFactory.CreateSegment(segmentPairProperties);
            sourceSegment.Add(CreateText(entry.MessageId));
            paragraphUnit.Source.Add(sourceSegment);

            var targetSegment = ItemFactory.CreateSegment(segmentPairProperties);
            targetSegment.Add(CreateText(entry.MessageString));
            paragraphUnit.Target.Add(targetSegment);

            Output.ProcessParagraphUnit(paragraphUnit);
        }

        private IText CreateText(string value)
        {
            var textProperties = PropertiesFactory.CreateTextProperties(value);

            return ItemFactory.CreateText(textProperties);
        }

        private class EntryBuilder
        {
            private bool _collectingMessageId;
            private bool _collectingMessagString;
            private Entry _entryInCreation;
            
            public Entry CompleteEntry { get; private set; }

            public void Add(IParseResult parseResult)
            {
                CompleteEntry = null;

                if (_collectingMessageId && parseResult.LineType != LineType.Text)
                {
                    _collectingMessageId = false;
                }
                else if (_collectingMessagString && parseResult.LineType != LineType.Text)
                {
                    _collectingMessagString = false;
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

                if (parseResult.LineType == LineType.MessageId)
                {
                    _entryInCreation = new Entry();
                    _entryInCreation.MessageId += parseResult.LineContent;
                    _collectingMessageId = true;
                }

                if (parseResult.LineType == LineType.MessageString)
                {
                    _entryInCreation.MessageString += parseResult.LineContent;
                    _collectingMessagString = true;
                }
            }
        }

        private class Entry
        {
            public string MessageId { get; set; } = string.Empty;

            public string MessageString { get; set; } = string.Empty;
        }
    }
}