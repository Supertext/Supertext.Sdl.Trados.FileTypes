using System;
using System.Collections.Generic;
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
        private readonly ITextProcessor _textProcessor;
        private readonly EntryBuilder _entryBuilder;
        private string _originalFilePath;

        //Parsing STATE --- is being changed during parsing 
        private ILineParsingSession _lineParsingSession;
        private IExtendedStreamReader _extendedStreamReader;
        private byte _progressInPercent;
        private int _totalNumberOfLines;

        public PoFileParser(IFileHelper fileHelper, ILineParser lineParser, IUserSettings defaultUserSettings, ITextProcessor textProcessor)
        {
            _fileHelper = fileHelper;
            _lineParser = lineParser;
            _userSettings = defaultUserSettings;
            _textProcessor = textProcessor;
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
            AddText(sourceSegment, sourceText);
            paragraphUnit.Source.Add(sourceSegment);

            if (isTargetTextNeeded)
            {
                var targetSegment = ItemFactory.CreateSegment(segmentPairProperties);
                AddText(targetSegment, entry.MessageString);
                paragraphUnit.Target.Add(targetSegment);
            }

            paragraphUnit.Properties.Contexts = CreateContextProperties(entry);

            return paragraphUnit;
        }

        private void AddText(ISegment segment, string text)
        {
            var fragments = new Queue<Fragment>(_textProcessor.Process(text));

            while(fragments.Count > 0)
            {
                var fragment = fragments.Dequeue();

                switch (fragment.InlineType)
                {
                    case InlineType.Text:
                        segment.Add(CreateText(fragment.Content));
                        break;
                    case InlineType.Placeholder:
                        segment.Add(CreatePlaceholder(fragment.Content));
                        break;
                    case InlineType.StartTag:
                        segment.Add(CreateTagPair(fragment, fragments));
                        break;
                    case InlineType.EndTag:
                        throw new ArithmeticException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private IText CreateText(string value)
        {
            var textProperties = PropertiesFactory.CreateTextProperties(value);

            return ItemFactory.CreateText(textProperties);
        }

        private IPlaceholderTag CreatePlaceholder(string content)
        {
            var placeholderProperties = PropertiesFactory.CreatePlaceholderTagProperties(content);
            placeholderProperties.TagContent = content;
            placeholderProperties.DisplayText = content;

            return ItemFactory.CreatePlaceholderTag(placeholderProperties);
        }

        private ITagPair CreateTagPair(Fragment startTagfragment, Queue<Fragment> fragments)
        {
            var startTagProperties = PropertiesFactory.CreateStartTagProperties(startTagfragment.Content);
            startTagProperties.DisplayText = startTagfragment.Content;

            var enclosedContent = new List<IAbstractMarkupData>();

            while (fragments.Count > 0)
            {
                var fragment = fragments.Dequeue();

                switch (fragment.InlineType)
                {
                    case InlineType.Text:
                        enclosedContent.Add(CreateText(fragment.Content));
                        break;
                    case InlineType.Placeholder:
                        enclosedContent.Add(CreatePlaceholder(fragment.Content));
                        break;
                    case InlineType.StartTag:
                        enclosedContent.Add(CreateTagPair(fragment, fragments));
                        break;
                    case InlineType.EndTag:
                        var endTagProperties = PropertiesFactory.CreateEndTagProperties(fragment.Content);
                        endTagProperties.DisplayText = fragment.Content;

                        var tagPair = ItemFactory.CreateTagPair(startTagProperties, endTagProperties);

                        foreach (var abstractMarkupData in enclosedContent)
                        {
                            tagPair.Add(abstractMarkupData);
                        }

                        return tagPair;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            throw new ArgumentOutOfRangeException();
        }

        private IContextProperties CreateContextProperties(Entry entry)
        {
            var contextProperties = PropertiesFactory.CreateContextProperties();

            var contextInfo = PropertiesFactory.CreateContextInfo(StandardContextTypes.Field);
            contextInfo.Purpose = ContextPurpose.Information;
            contextProperties.Contexts.Add(contextInfo);

            var contextId = PropertiesFactory.CreateContextInfo(ContextKeys.LocationContextType);
            contextId.SetMetaData(ContextKeys.MessageIdStart, entry.MessageIdStart.ToString(CultureInfo.InvariantCulture));
            contextId.SetMetaData(ContextKeys.MessageIdEnd, entry.MessageIdEnd.ToString(CultureInfo.InvariantCulture));
            contextId.SetMetaData(ContextKeys.MessageStringStart, entry.MessageStringStart.ToString(CultureInfo.InvariantCulture));
            contextId.SetMetaData(ContextKeys.MessageStringEnd, entry.MessageStringEnd.ToString(CultureInfo.InvariantCulture));
            contextProperties.Contexts.Add(contextId);

            return contextProperties;
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
    }
}