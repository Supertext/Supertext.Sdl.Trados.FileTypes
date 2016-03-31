using System;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public interface IEntryBuilder
    {
        Entry CompleteEntry { get; }

        void Add(IParseResult parseResult, int lineNumber);
    }

    public class EntryBuilder : IEntryBuilder
    {
        private Entry _entryInCreation;
        private Action<string> _collectText;
        private Action<int> _finishCollectingText;
        private Action<string> _collectMessageStringPlural;
        private Action _finishCollectingMessageStringPlural;
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

            if (_collectMessageStringPlural != null && parseResult.LineType == LineType.MessageStringPlural)
            {
                _collectMessageStringPlural(parseResult.LineContent);
                return;
            }

            _collectMessageStringPlural = null;

            if (_finishCollectingMessageStringPlural != null)
            {
                _finishCollectingMessageStringPlural();
            }

            _finishCollectingMessageStringPlural = null;

            if (CompleteEntry != null || _entryInCreation == null)
            {
                _entryInCreation = new Entry();
            }

            CollectMessage(parseResult, lineNumber);

            CollectComment(parseResult);
        }

        private void CollectMessage(IParseResult parseResult, int lineNumber)
        {
            switch (parseResult.LineType)
            {
                case LineType.MessageContext:
                    _entryInCreation.MessageContext = parseResult.LineContent;
                    break;

                case LineType.MessageId:
                    CollectMessageId(parseResult, lineNumber);
                    break;

                case LineType.MessageIdPlural:
                    CollectMessageIdPlural(parseResult);
                    break;

                case LineType.MessageString:
                    CollectMessageString(parseResult, lineNumber);
                    break;

                case LineType.MessageStringPlural:
                    StartCollectingMessageStringPlural(parseResult, lineNumber);
                    break;
            }
        }

        private void CollectMessageId(IParseResult parseResult, int lineNumber)
        {
            _entryInCreation.MessageId += parseResult.LineContent;
            _entryInCreation.MessageIdStart = lineNumber;
            _collectText = lineContent => _entryInCreation.MessageId += lineContent;
            _finishCollectingText =
                currentLineNumber => _entryInCreation.MessageIdEnd = currentLineNumber - 1;
        }

        private void CollectMessageIdPlural(IParseResult parseResult)
        {
            _entryInCreation.MessageIdPlural += parseResult.LineContent;
            _collectText = lineContent => _entryInCreation.MessageIdPlural += lineContent;
            _finishCollectingText = currentLineNumber => _entryInCreation.MessageIdEnd = currentLineNumber - 1;
        }

        private void CollectMessageString(IParseResult parseResult, int lineNumber)
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

        private void StartCollectingMessageStringPlural(IParseResult parseResult, int lineNumber)
        {
            _entryInCreation.MessageStringStart = lineNumber;

            CollectMessageStringPlural(parseResult.LineContent);

            _collectMessageStringPlural = CollectMessageStringPlural;

            _finishCollectingMessageStringPlural = SetCompleteEntry;
        }

        private void CollectMessageStringPlural(string lineContent)
        {
            _tmpMessageStringPluralContent = lineContent;
            _collectText = currentLineContent => _tmpMessageStringPluralContent += currentLineContent;
            _finishCollectingText =
                currentLineNumber =>
                {
                    _entryInCreation.MessageStringEnd = currentLineNumber - 1;
                    _entryInCreation.MessageStringPlural.Add(_tmpMessageStringPluralContent);
                };

        }

        private void CollectComment(IParseResult parseResult)
        {
            if (parseResult.LineType == LineType.Comment)
            {
                _entryInCreation.Comments.Add(parseResult.LineContent);
            }
        }

        private void SetCompleteEntry()
        {
            CompleteEntry = _entryInCreation;
        }
    }
}