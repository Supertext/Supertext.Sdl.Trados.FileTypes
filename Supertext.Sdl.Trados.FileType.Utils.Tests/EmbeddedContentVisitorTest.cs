using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.Utils.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.Utils.Tests
{
    [TestFixture]
    public class EmbeddedContentVisitorTest
    {
        private const string TexBeforeProcessing = "textBeforeProcessing";

        private IDocumentItemFactory _itemFactoryMock;
        private IPropertiesFactory _propertiesFactoryMock;
        private ITextProcessor _textProcessorMock;
        private IParagraph _sourceParagraphMock;
        private IText _textMock;

        [SetUp]
        public void SetUp()
        {
            _itemFactoryMock = A.Fake<IDocumentItemFactory>();
            _propertiesFactoryMock = A.Fake<IPropertiesFactory>();
            _textProcessorMock = A.Fake<ITextProcessor>();
            _sourceParagraphMock = A.Fake<IParagraph>();
            _textMock = A.Fake<IText>();

            var paragraphUnitMock = A.Fake<IParagraphUnit>();
            A.CallTo(() => _itemFactoryMock.CreateParagraphUnit(LockTypeFlags.Unlocked)).Returns(paragraphUnitMock);
            A.CallTo(() => paragraphUnitMock.Source).Returns(_sourceParagraphMock);

            var propertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => propertiesMock.Text).Returns(TexBeforeProcessing);
            A.CallTo(() => _textMock.Properties).Returns(propertiesMock);
        }

        [Test]
        public void VisitText_WhenTextIsTextInlineType_ShouldAddText()
        {
            // Arrange
            var testee = CreateTestee();

            A.CallTo(() => _textProcessorMock.Process(TexBeforeProcessing)).Returns(new List<IFragment>
            {
                new Fragment(InlineType.Text, "text")
            });

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("text")).Returns(textPropertiesMock);

            var newTextMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(newTextMock);

            // Act
            testee.VisitText(_textMock);

            // Assert
            A.CallTo(() => _sourceParagraphMock.Add(newTextMock)).MustHaveHappened();
        }

        [Test]
        public void VisitText_WhenTextIsPlaceholderInlineType_ShouldAddPlaceholder()
        {
            // Arrange
            var testee = CreateTestee();

            A.CallTo(() => _textProcessorMock.Process(TexBeforeProcessing)).Returns(new List<IFragment>
            {
                new Fragment(InlineType.Placeholder, "{0}")
            });

            var placeholderTagPropertiesMock = A.Fake<IPlaceholderTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreatePlaceholderTagProperties("{0}")).Returns(placeholderTagPropertiesMock);

            var newPlactholderMock = A.Fake<IPlaceholderTag>();
            A.CallTo(() => _itemFactoryMock.CreatePlaceholderTag(placeholderTagPropertiesMock)).Returns(newPlactholderMock);

            // Act
            testee.VisitText(_textMock);

            // Assert
            A.CallTo(() => _sourceParagraphMock.Add(newPlactholderMock)).MustHaveHappened();
        }

        [Test]
        public void VisitText_WhenTextIsPlaceholderInlineType_ShouldAddPlaceholderProperties()
        {
            // Arrange
            var testee = CreateTestee();

            A.CallTo(() => _textProcessorMock.Process(TexBeforeProcessing)).Returns(new List<IFragment>
            {
                new Fragment(InlineType.Placeholder, "{0}", SegmentationHint.IncludeWithText, false)
            });

            var placeholderTagPropertiesMock = A.Fake<IPlaceholderTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreatePlaceholderTagProperties("{0}")).Returns(placeholderTagPropertiesMock);

            var newPlactholderMock = A.Fake<IPlaceholderTag>();
            A.CallTo(() => _itemFactoryMock.CreatePlaceholderTag(placeholderTagPropertiesMock)).Returns(newPlactholderMock);

            // Act
            testee.VisitText(_textMock);

            // Assert
            placeholderTagPropertiesMock.TagContent.Should().Be("{0}");
            placeholderTagPropertiesMock.DisplayText.Should().Be("{0}");
            placeholderTagPropertiesMock.SegmentationHint.Should().Be(SegmentationHint.IncludeWithText);
        }

        [Test]
        public void VisitText_WhenTextHasTags_ShouldAddTagPair()
        {
            // Arrange
            var testee = CreateTestee();

            A.CallTo(() => _textProcessorMock.Process(TexBeforeProcessing)).Returns(new List<IFragment>
            {
                new Fragment(InlineType.StartTag, "start", SegmentationHint.Undefined, true),
                new Fragment(InlineType.EndTag, "end", SegmentationHint.Undefined, true)
            });

            var startTagPropertiesMock = A.Fake<IStartTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateStartTagProperties("start")).Returns(startTagPropertiesMock);

            var endTagPropertiesMock = A.Fake<IEndTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateEndTagProperties("end")).Returns(endTagPropertiesMock);

            var tagPairMock = A.Fake<ITagPair>();
            A.CallTo(() => _itemFactoryMock.CreateTagPair(startTagPropertiesMock, endTagPropertiesMock)).Returns(tagPairMock);

            // Act
            testee.VisitText(_textMock);

            // Assert
            A.CallTo(() => _sourceParagraphMock.Add(tagPairMock)).MustHaveHappened();
        }

        [Test]
        public void VisitText_WhenTextHasTags_ShouldAddTagPairProperties()
        {
            // Arrange
            var testee = CreateTestee();

            A.CallTo(() => _textProcessorMock.Process(TexBeforeProcessing)).Returns(new List<IFragment>
            {
                new Fragment(InlineType.StartTag, "start", SegmentationHint.IncludeWithText, true),
                new Fragment(InlineType.EndTag, "end", SegmentationHint.IncludeWithText, true)
            });

            var startTagPropertiesMock = A.Fake<IStartTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateStartTagProperties("start")).Returns(startTagPropertiesMock);

            var endTagPropertiesMock = A.Fake<IEndTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateEndTagProperties("end")).Returns(endTagPropertiesMock);

            var tagPairMock = A.Fake<ITagPair>();
            A.CallTo(() => _itemFactoryMock.CreateTagPair(startTagPropertiesMock, endTagPropertiesMock)).Returns(tagPairMock);

            // Act
            testee.VisitText(_textMock);

            // Assert
            startTagPropertiesMock.DisplayText.Should().Be("start");
            startTagPropertiesMock.SegmentationHint.Should().Be(SegmentationHint.IncludeWithText);
            endTagPropertiesMock.DisplayText.Should().Be("end");
        }

        [Test]
        public void VisitText_WhenTextHasTagsButNotTranslatable_ShouldAddLockedContent()
        {
            // Arrange
            var testee = CreateTestee();

            A.CallTo(() => _textProcessorMock.Process(TexBeforeProcessing)).Returns(new List<IFragment>
            {
                new Fragment(InlineType.StartTag, "start", SegmentationHint.Undefined, false),
                new Fragment(InlineType.Text, "text"),
                new Fragment(InlineType.EndTag, "end", SegmentationHint.Undefined, false)
            });

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("text")).Returns(textPropertiesMock);

            var newTextMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMock)).Returns(newTextMock);

            var startTagPropertiesMock = A.Fake<IStartTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateStartTagProperties("start")).Returns(startTagPropertiesMock);

            var endTagPropertiesMock = A.Fake<IEndTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateEndTagProperties("end")).Returns(endTagPropertiesMock);

            var lockedContentPropertiesMock = A.Fake<ILockedContentProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateLockedContentProperties(LockTypeFlags.Manual))
                .Returns(lockedContentPropertiesMock);
            var lockedContentMock = A.Fake<ILockedContent>();
            var lockedContainerMock= A.Fake<ILockedContainer>();
            A.CallTo(() => lockedContentMock.Content).Returns(lockedContainerMock);
            A.CallTo(() => _itemFactoryMock.CreateLockedContent(lockedContentPropertiesMock)).Returns(lockedContentMock);

            // Act
            testee.VisitText(_textMock);

            // Assert
            A.CallTo(() => lockedContainerMock.Add(newTextMock)).MustHaveHappened();
            A.CallTo(() => _sourceParagraphMock.Add(lockedContentMock)).MustHaveHappened();
        }

        [Test]
        public void VisitText_WhenTextHasStartTagButNoEndTag_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var testee = CreateTestee();

            A.CallTo(() => _textProcessorMock.Process(TexBeforeProcessing)).Returns(new List<IFragment>
            {
                new Fragment(InlineType.StartTag, "start", SegmentationHint.Undefined, true)
            });

            var startTagPropertiesMock = A.Fake<IStartTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateStartTagProperties("start")).Returns(startTagPropertiesMock);

            var endTagPropertiesMock = A.Fake<IEndTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateEndTagProperties("end")).Returns(endTagPropertiesMock);

            var tagPairMock = A.Fake<ITagPair>();
            A.CallTo(() => _itemFactoryMock.CreateTagPair(startTagPropertiesMock, endTagPropertiesMock)).Returns(tagPairMock);

            // Act, Assert
            testee.Invoking(factory => testee.VisitText(_textMock))
                .Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void VisitText_WhenTextHasEndTagButNoStartTag_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var testee = CreateTestee();

            A.CallTo(() => _textProcessorMock.Process(TexBeforeProcessing)).Returns(new List<IFragment>
            {
                new Fragment(InlineType.EndTag, "end", SegmentationHint.Undefined, true)
            });

            var startTagPropertiesMock = A.Fake<IStartTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateStartTagProperties("start")).Returns(startTagPropertiesMock);

            var endTagPropertiesMock = A.Fake<IEndTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateEndTagProperties("end")).Returns(endTagPropertiesMock);

            var tagPairMock = A.Fake<ITagPair>();
            A.CallTo(() => _itemFactoryMock.CreateTagPair(startTagPropertiesMock, endTagPropertiesMock)).Returns(tagPairMock);

            // Act, Assert
            testee.Invoking(factory => testee.VisitText(_textMock))
                .Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void VisitText_WhenTextHasNestedTags_ShouldAddAllTagPairs()
        {
            // Arrange
            var testee = CreateTestee();

            A.CallTo(() => _textProcessorMock.Process(TexBeforeProcessing)).Returns(new List<IFragment>
            {
                new Fragment(InlineType.StartTag, "start1", SegmentationHint.Undefined, true),
                new Fragment(InlineType.Text, "message "),
                new Fragment(InlineType.StartTag, "start2", SegmentationHint.Undefined, true),
                new Fragment(InlineType.Text, "id "),
                new Fragment(InlineType.EndTag, "end2", SegmentationHint.Undefined, true),
                new Fragment(InlineType.EndTag, "end1", SegmentationHint.Undefined, true)
            });

            var startTagPropertiesMock = A.Fake<IStartTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateStartTagProperties("start1")).Returns(startTagPropertiesMock);

            var endTagPropertiesMock = A.Fake<IEndTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateEndTagProperties("end1")).Returns(endTagPropertiesMock);

            var tagPairMock = A.Fake<ITagPair>();
            A.CallTo(() => _itemFactoryMock.CreateTagPair(startTagPropertiesMock, endTagPropertiesMock)).Returns(tagPairMock);

            var startTagPropertiesMock2 = A.Fake<IStartTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateStartTagProperties("start2")).Returns(startTagPropertiesMock2);

            var endTagPropertiesMock2 = A.Fake<IEndTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateEndTagProperties("end2")).Returns(endTagPropertiesMock2);

            var tagPairMock2 = A.Fake<ITagPair>();
            A.CallTo(() => _itemFactoryMock.CreateTagPair(startTagPropertiesMock2, endTagPropertiesMock2)).Returns(tagPairMock2);

            // Act
            testee.VisitText(_textMock);

            // Assert
            A.CallTo(() => tagPairMock.Add(tagPairMock2)).MustHaveHappened();
            A.CallTo(() => _sourceParagraphMock.Add(tagPairMock)).MustHaveHappened();
        }

        [Test]
        public void VisitText_WhenTextHasTagsWithPlaceholderEnclosed_ShouldAddTagPairWithPlaceholder()
        {
            // Arrange
            var testee = CreateTestee();

            A.CallTo(() => _textProcessorMock.Process(TexBeforeProcessing)).Returns(new List<IFragment>
            {
                new Fragment(InlineType.StartTag, "start1", SegmentationHint.Undefined, true),
                new Fragment(InlineType.Placeholder, "placeholder"),
                new Fragment(InlineType.EndTag, "end1", SegmentationHint.Undefined, true)
            });

            var startTagPropertiesMock = A.Fake<IStartTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateStartTagProperties("start1")).Returns(startTagPropertiesMock);

            var endTagPropertiesMock = A.Fake<IEndTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateEndTagProperties("end1")).Returns(endTagPropertiesMock);

            var tagPairMock = A.Fake<ITagPair>();
            A.CallTo(() => _itemFactoryMock.CreateTagPair(startTagPropertiesMock, endTagPropertiesMock)).Returns(tagPairMock);

            var placeholderTagPropertiesMock = A.Fake<IPlaceholderTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreatePlaceholderTagProperties("placeholder")).Returns(placeholderTagPropertiesMock);

            var placeholderMock = A.Fake<IPlaceholderTag>();
            A.CallTo(() => _itemFactoryMock.CreatePlaceholderTag(placeholderTagPropertiesMock)).Returns(placeholderMock);

            // Act
            testee.VisitText(_textMock);

            // Assert
            A.CallTo(() => tagPairMock.Add(placeholderMock)).MustHaveHappened();
            A.CallTo(() => _sourceParagraphMock.Add(tagPairMock)).MustHaveHappened();
        }

        [Test]
        public void VisitText_WhenTextHasTags_ShouldAddTagPairWithDisplayText()
        {
            // Arrange
            var testee = CreateTestee();

            A.CallTo(() => _textProcessorMock.Process(TexBeforeProcessing)).Returns(new List<IFragment>
            {
                new Fragment(InlineType.StartTag, @"<test attr=""23"">", SegmentationHint.Undefined, true),
                new Fragment(InlineType.EndTag, "</test>", SegmentationHint.Undefined, true)
            });

            var startTagPropertiesMock = A.Fake<IStartTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateStartTagProperties(@"<test attr=""23"">")).Returns(startTagPropertiesMock);

            var endTagPropertiesMock = A.Fake<IEndTagProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateEndTagProperties("</test>")).Returns(endTagPropertiesMock);

            var tagPairMock = A.Fake<ITagPair>();
            A.CallTo(() => _itemFactoryMock.CreateTagPair(startTagPropertiesMock, endTagPropertiesMock)).Returns(tagPairMock);

            // Act
            testee.VisitText(_textMock);

            // Assert
            startTagPropertiesMock.DisplayText.Should().Be("test");
            endTagPropertiesMock.DisplayText.Should().Be("test");
        }

        private IEmbeddedContentVisitor CreateTestee()
        {
            var testeeFactory = new EmbeddedContentVisitor();

            return testeeFactory.CreateVisitor(_itemFactoryMock, _propertiesFactoryMock, _textProcessorMock);
        }
    }
}
