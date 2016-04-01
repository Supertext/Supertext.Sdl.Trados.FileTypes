using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class ParagraphUnitFactoryTest
    {
        private ITextProcessor _textProcessorMock;
        private IDocumentItemFactory _itemFactoryMock;
        private IPropertiesFactory _propertiesFactoryMock;
        private IParagraphUnit _paragraphUnitMock;
        private IParagraph _paragraphSourceMock;
        private IParagraph _paragraphTargetMock;
        private ISegment _sourceSegmentMock;
        private ISegment _targetSegmentMock;

        [SetUp]
        public void SetUp()
        {
            _textProcessorMock = A.Fake<ITextProcessor>();
            _itemFactoryMock = A.Fake<IDocumentItemFactory>();
            _propertiesFactoryMock = A.Fake<IPropertiesFactory>();

            _paragraphUnitMock = A.Fake<IParagraphUnit>();
            A.CallTo(() => _itemFactoryMock.CreateParagraphUnit(A<LockTypeFlags>.Ignored)).Returns(_paragraphUnitMock);

            _paragraphSourceMock = A.Fake<IParagraph>();
            A.CallTo(() => _paragraphUnitMock.Source).Returns(_paragraphSourceMock);

            _paragraphTargetMock = A.Fake<IParagraph>();
            A.CallTo(() => _paragraphUnitMock.Target).Returns(_paragraphTargetMock);

            _sourceSegmentMock = A.Fake<ISegment>();
            _targetSegmentMock = A.Fake<ISegment>();
            A.CallTo(() => _itemFactoryMock.CreateSegment(A<ISegmentPairProperties>.Ignored))
                .ReturnsNextFromSequence(_sourceSegmentMock, _targetSegmentMock);

            //Default
            A.CallTo(() => _textProcessorMock.Process(A<string>.Ignored)).ReturnsLazily((string value) => new List<Fragment>
            {
                new Fragment(InlineType.Text, value)
            });
        }

        [Test]
        public void Create_ShouldSetMessageStringLocationContext()
        {
            // Arrange
            var testee = CreateTestee();

            var contextPropertiesMock = A.Fake<IContextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextProperties()).Returns(contextPropertiesMock);

            var contextInfoMock = A.Fake<IContextInfo>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextInfo(ContextKeys.LocationContextType))
                .Returns(contextInfoMock);

            var entry = new Entry
            {
                MessageIdStart = 2,
                MessageIdEnd = 3,
                MessageStringStart = 4,
                MessageStringEnd = 5
            };

            // Act
            testee.Create(entry, LineType.MessageId, false);

            // Assert
            A.CallTo(() => contextInfoMock.SetMetaData(ContextKeys.MessageStringStart, "4")).MustHaveHappened();
            A.CallTo(() => contextInfoMock.SetMetaData(ContextKeys.MessageStringEnd, "5")).MustHaveHappened();
            A.CallTo(() => contextPropertiesMock.Contexts.Add(contextInfoMock)).MustHaveHappened();
        }

        [Test]
        public void Create_WhenEntryHasMessageContext_ShouldSetMessageContextContext()
        {
            // Arrange
            var testee = CreateTestee();

            var contextPropertiesMock = A.Fake<IContextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextProperties()).Returns(contextPropertiesMock);

            var contextInfoMock = A.Fake<IContextInfo>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextInfo(ContextKeys.MessageContext))
                .Returns(contextInfoMock);

            var entry = new Entry
            {
                MessageContext = "message context"
            };

            // Act
            testee.Create(entry, LineType.MessageId, false);

            // Assert
            contextInfoMock.Description.Should().Be("message context");
            A.CallTo(() => contextPropertiesMock.Contexts.Add(contextInfoMock)).MustHaveHappened();
        }

        [Test]
        public void Create_WhenEntryHasNoMessageContext_ShouldNotSetMessageContextContext()
        {
            // Arrange
            var testee = CreateTestee();

            var contextPropertiesMock = A.Fake<IContextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextProperties()).Returns(contextPropertiesMock);

            var contextInfoMock = A.Fake<IContextInfo>();
            A.CallTo(() => _propertiesFactoryMock.CreateContextInfo(ContextKeys.MessageContext))
                .Returns(contextInfoMock);

            var entry = new Entry();

            // Act
            testee.Create(entry, LineType.MessageId, false);

            // Assert
            contextInfoMock.Description.Should().BeNullOrEmpty();
            A.CallTo(() => contextPropertiesMock.Contexts.Add(contextInfoMock)).MustNotHaveHappened();
        }

        [Test]
        public void Create_WhenTextIsTextInlineType_ShouldAddText()
        {
            // Arrange
            var testee = CreateTestee();

            var entry = new Entry
            {
                MessageId = "message id",
                MessageString = "message string",
            };

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("message id")).Returns(textPropertiesMock);

            var textMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(textMock);

            A.CallTo(() => _textProcessorMock.Process("message id")).Returns(new List<Fragment>
            {
                new Fragment(InlineType.Text, "message id")
            });

            // Act
            testee.Create(entry, LineType.MessageId, false);

            // Assert
            A.CallTo(() => _sourceSegmentMock.Add(textMock)).MustHaveHappened();
        }

        [Test]
        public void Create_WhenTextIsPlaceholderInlineType_ShouldAddPlaceholder()
        {
            // Arrange
            var testee = CreateTestee();

            var entry = new Entry
            {
                MessageId = "message id",
                MessageString = "message string",
            };

            var placeholderTagPropertiesMock = A.Fake<IPlaceholderTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreatePlaceholderTagProperties("message id")).Returns(placeholderTagPropertiesMock);

            var placeholderTagMock = A.Fake<IPlaceholderTag>();
            A.CallTo(() => _itemFactoryMock.CreatePlaceholderTag(placeholderTagPropertiesMock)).Returns(placeholderTagMock);

            A.CallTo(() => _textProcessorMock.Process("message id")).Returns(new List<Fragment>
            {
                new Fragment(InlineType.Placeholder, "message id")
            });

            // Act
            testee.Create(entry, LineType.MessageId, false);

            // Assert
            A.CallTo(() => _sourceSegmentMock.Add(placeholderTagMock)).MustHaveHappened();
        }

        [Test]
        public void Create_WhenTextHasTags_ShouldAddTagPair()
        {
            // Arrange
            var testee = CreateTestee();

            var entry = new Entry
            {
                MessageId = "start message id end",
                MessageString = "message string",
            };

            var startTagPropertiesMock = A.Fake<IStartTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateStartTagProperties("start")).Returns(startTagPropertiesMock);

            var endTagPropertiesMock = A.Fake<IEndTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateEndTagProperties("end")).Returns(endTagPropertiesMock);

            var tagPairMock = A.Fake<ITagPair>();
            A.CallTo(() => _itemFactoryMock.CreateTagPair(startTagPropertiesMock, endTagPropertiesMock))
                .Returns(tagPairMock);

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("message id")).Returns(textPropertiesMock);

            var textMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(textMock);

            A.CallTo(() => _textProcessorMock.Process("start message id end")).Returns(new List<Fragment>
            {
                new Fragment(InlineType.StartTag, "start"),
                new Fragment(InlineType.Text, "message id"),
                new Fragment(InlineType.EndTag, "end")
            });

            // Act
            testee.Create(entry, LineType.MessageId, false);

            // Assert
            A.CallTo(() => _sourceSegmentMock.Add(tagPairMock)).MustHaveHappened();
        }

        [Test]
        public void Create_WhenTextHasStartTagButNoEndTag_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var testee = CreateTestee();

            var entry = new Entry
            {
                MessageId = "start message id",
                MessageString = "message string",
            };

            var startTagPropertiesMock = A.Fake<IStartTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateStartTagProperties("start")).Returns(startTagPropertiesMock);

            var endTagPropertiesMock = A.Fake<IEndTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateEndTagProperties("end")).Returns(endTagPropertiesMock);

            var tagPairMock = A.Fake<ITagPair>();
            A.CallTo(() => _itemFactoryMock.CreateTagPair(startTagPropertiesMock, endTagPropertiesMock))
                .Returns(tagPairMock);

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("message id")).Returns(textPropertiesMock);

            var textMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(textMock);

            A.CallTo(() => _textProcessorMock.Process("start message id")).Returns(new List<Fragment>
            {
                new Fragment(InlineType.StartTag, "start"),
                new Fragment(InlineType.Text, "message id")
            });

            // Act, Assert
            testee.Invoking(factory => factory.Create(entry, LineType.MessageId, false))
                .ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Create_WhenTextHasNestedTags_ShouldAddAllTagPairs()
        {
            // Arrange
            var testee = CreateTestee();

            var entry = new Entry
            {
                MessageId = "start1 message start2 id end2 end1",
                MessageString = "message string",
            };

            var startTagPropertiesMock1 = A.Fake<IStartTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateStartTagProperties("start1")).Returns(startTagPropertiesMock1);

            var endTagPropertiesMock1 = A.Fake<IEndTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateEndTagProperties("end1")).Returns(endTagPropertiesMock1);

            var tagPairMock1 = A.Fake<ITagPair>();
            A.CallTo(() => _itemFactoryMock.CreateTagPair(startTagPropertiesMock1, endTagPropertiesMock1))
                .Returns(tagPairMock1);

            var startTagPropertiesMock2 = A.Fake<IStartTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateStartTagProperties("start2")).Returns(startTagPropertiesMock2);

            var endTagPropertiesMock2 = A.Fake<IEndTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateEndTagProperties("end2")).Returns(endTagPropertiesMock2);

            var tagPairMock2 = A.Fake<ITagPair>();
            A.CallTo(() => _itemFactoryMock.CreateTagPair(startTagPropertiesMock2, endTagPropertiesMock2))
                .Returns(tagPairMock1);

            A.CallTo(() => _textProcessorMock.Process("start1 message start2 id end2 end1")).Returns(new List<Fragment>
            {
                new Fragment(InlineType.StartTag, "start1"),
                new Fragment(InlineType.Text, "message "),
                new Fragment(InlineType.StartTag, "start2"),
                new Fragment(InlineType.Text, "id "),
                new Fragment(InlineType.EndTag, "end2"),
                new Fragment(InlineType.EndTag, "end1")
            });

            // Act
            testee.Create(entry, LineType.MessageId, false);

            // Assert
            A.CallTo(() => tagPairMock1.Add(tagPairMock2));
            A.CallTo(() => _sourceSegmentMock.Add(tagPairMock1)).MustHaveHappened();
        }

        [Test]
        public void Create_WhenTextHasTagsWithPlaceholderEnclosed_ShouldAddTagPairWithPlaceholder()
        {
            // Arrange
            var testee = CreateTestee();

            var entry = new Entry
            {
                MessageId = "start message id placeholder end",
                MessageString = "message string",
            };

            var startTagPropertiesMock = A.Fake<IStartTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateStartTagProperties("start")).Returns(startTagPropertiesMock);

            var endTagPropertiesMock = A.Fake<IEndTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateEndTagProperties("end")).Returns(endTagPropertiesMock);

            var tagPairMock = A.Fake<ITagPair>();
            A.CallTo(() => _itemFactoryMock.CreateTagPair(startTagPropertiesMock, endTagPropertiesMock))
                .Returns(tagPairMock);

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("message id")).Returns(textPropertiesMock);

            var textMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(textMock);

            A.CallTo(() => _textProcessorMock.Process("start message id placeholder end")).Returns(new List<Fragment>
            {
                new Fragment(InlineType.StartTag, "start"),
                new Fragment(InlineType.Text, "message id"),
                new Fragment(InlineType.Placeholder, "placeholder"),
                new Fragment(InlineType.EndTag, "end")
            });

            // Act
            testee.Create(entry, LineType.MessageId, false);

            // Assert
            A.CallTo(() => tagPairMock.Add(textMock)).MustHaveHappened();
            A.CallTo(() => _sourceSegmentMock.Add(tagPairMock)).MustHaveHappened();
        }

        [Test]
        public void Create_WhenSourceLineTypeIsMessageId_ShouldUseMessageIdAsSourceText()
        {
            // Arrange
            var testee = CreateTestee();

            var entry = new Entry
            {
                MessageId = "message id",
                MessageString = "message string",
            };

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("message id")).Returns(textPropertiesMock);

            var textMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(textMock);

            // Act
            testee.Create(entry, LineType.MessageId, false);

            // Assert
            A.CallTo(() => _sourceSegmentMock.Add(textMock)).MustHaveHappened();
            A.CallTo(() => _paragraphSourceMock.Add(_sourceSegmentMock)).MustHaveHappened();
        }

        [Test]
        public void Create_WhenSourceLineTypeIsMessageString_ShouldUseMessageStringAsSourceText()
        {
            // Arrange
            var testee = CreateTestee();

            var entry = new Entry
            {
                MessageId = "message id",
                MessageString = "message string",
            };

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("message string")).Returns(textPropertiesMock);

            var textMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(textMock);

            // Act
            testee.Create(entry, LineType.MessageString, false);

            // Assert
            A.CallTo(() => _sourceSegmentMock.Add(textMock)).MustHaveHappened();
            A.CallTo(() => _paragraphSourceMock.Add(_sourceSegmentMock)).MustHaveHappened();
        }

        [Test]
        public void Create_WhenTargetTextIsNeeded_ShouldAddTargetText()
        {
            // Arrange
            var testee = CreateTestee();

            var entry = new Entry
            {
                MessageId = "message id",
                MessageString = "message string",
            };

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("message string")).Returns(textPropertiesMock);

            var textMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(textMock);

            // Act
            testee.Create(entry, LineType.MessageId, true);

            // Assert
            A.CallTo(() => _targetSegmentMock.Add(textMock)).MustHaveHappened();
            A.CallTo(() => _paragraphTargetMock.Add(_targetSegmentMock)).MustHaveHappened();
        }

        [Test]
        public void Create_WhenTargetTextIsNotNeeded_ShouldNotAddTargetText()
        {
            // Arrange
            var testee = CreateTestee();

            var entry = new Entry
            {
                MessageId = "message id",
                MessageString = "message string",
            };

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("message string")).Returns(textPropertiesMock);

            var textMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(textMock);

            // Act
            testee.Create(entry, LineType.MessageId, false);

            // Assert
            A.CallTo(() => _targetSegmentMock.Add(textMock)).MustNotHaveHappened();
            A.CallTo(() => _paragraphTargetMock.Add(_targetSegmentMock)).MustNotHaveHappened();
        }



        public ParagraphUnitFactory CreateTestee()
        {
            return new ParagraphUnitFactory(_textProcessorMock)
            {
                ItemFactory = _itemFactoryMock,
                PropertiesFactory = _propertiesFactoryMock,
            };
        }
    }
}