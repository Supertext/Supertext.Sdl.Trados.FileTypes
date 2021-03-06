﻿using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.Parsing;
using Supertext.Sdl.Trados.FileType.PoFile.Settings;
using Supertext.Sdl.Trados.FileType.PoFile.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.PoFile.Tests
{
    [TestFixture]
    public class ParagraphUnitFactoryTest
    {
        private IDocumentItemFactory _itemFactoryMock;
        private IPropertiesFactory _propertiesFactoryMock;
        private IParagraphUnit _paragraphUnitMock;
        private IParagraph _paragraphSourceMock;
        private IParagraph _paragraphTargetMock;
        private ISegmentPairProperties _segmentPairPropertiesMock;
        private ITranslationOrigin _translationOriginMmock;
        private ISegment _sourceSegment0Mock;
        private ISegment _targetSegment0Mock;
        private IText _textMsgidMock;
        private IText _textMsgidPluralMock;
        private IText _textMsgstrMock;
        private IText _textMsgstr0Mock;
        private IText _textMsgstr1Mock;
        private IText _textMsgstr2Mock;
        private ISegment _sourceSegment1Mock;
        private ISegment _targetSegment1Mock;
        private ISegment _sourceSegment2Mock;
        private ISegment _targetSegment2Mock;
        private ISegment _sourceSegment3Mock;
        private ISegment _targetSegment3Mock;

        [SetUp]
        public void SetUp()
        {
            _itemFactoryMock = A.Fake<IDocumentItemFactory>();
            _propertiesFactoryMock = A.Fake<IPropertiesFactory>();

            _paragraphUnitMock = A.Fake<IParagraphUnit>();
            A.CallTo(() => _itemFactoryMock.CreateParagraphUnit(A<LockTypeFlags>.Ignored)).Returns(_paragraphUnitMock);

            _paragraphSourceMock = A.Fake<IParagraph>();
            A.CallTo(() => _paragraphUnitMock.Source).Returns(_paragraphSourceMock);

            _paragraphTargetMock = A.Fake<IParagraph>();
            A.CallTo(() => _paragraphUnitMock.Target).Returns(_paragraphTargetMock);

            _segmentPairPropertiesMock = A.Fake<ISegmentPairProperties>();
            A.CallTo(() => _itemFactoryMock.CreateSegmentPairProperties()).Returns(_segmentPairPropertiesMock);

            _translationOriginMmock = A.Fake<ITranslationOrigin>();
            A.CallTo(() => _itemFactoryMock.CreateTranslationOrigin()).Returns(_translationOriginMmock);

            _sourceSegment0Mock = A.Fake<ISegment>();
            _targetSegment0Mock = A.Fake<ISegment>();
            _sourceSegment1Mock = A.Fake<ISegment>();
            _targetSegment1Mock = A.Fake<ISegment>();
            _sourceSegment2Mock = A.Fake<ISegment>();
            _targetSegment2Mock = A.Fake<ISegment>();
            _sourceSegment3Mock = A.Fake<ISegment>();
            _targetSegment3Mock = A.Fake<ISegment>();
            A.CallTo(() => _itemFactoryMock.CreateSegment(A<ISegmentPairProperties>.Ignored))
                .ReturnsNextFromSequence(
                    _sourceSegment0Mock, _targetSegment0Mock,
                    _sourceSegment1Mock, _targetSegment1Mock,
                    _sourceSegment2Mock, _targetSegment2Mock,
                    _sourceSegment3Mock, _targetSegment3Mock
                );

            var textPropertiesMsgidMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("message id")).Returns(textPropertiesMsgidMock);

            var textPropertiesMsgidPluralMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("message id plural"))
                .Returns(textPropertiesMsgidPluralMock);

            var textPropertiesMsgstrMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("message string"))
                .Returns(textPropertiesMsgstrMock);

            var textPropertiesMsgstr0Mock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("message string 0"))
                .Returns(textPropertiesMsgstr0Mock);

            var textPropertiesMsgstr1Mock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("message string 1"))
                .Returns(textPropertiesMsgstr1Mock);

            var textPropertiesMsgstr2Mock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("message string 2"))
                .Returns(textPropertiesMsgstr2Mock);

            _textMsgidMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMsgidMock)).Returns(_textMsgidMock);

            _textMsgidPluralMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMsgidPluralMock)).Returns(_textMsgidPluralMock);

            _textMsgstrMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMsgstrMock)).Returns(_textMsgstrMock);

            _textMsgstr0Mock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMsgstr0Mock)).Returns(_textMsgstr0Mock);

            _textMsgstr1Mock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMsgstr1Mock)).Returns(_textMsgstr1Mock);

            _textMsgstr2Mock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMsgstr2Mock)).Returns(_textMsgstr2Mock);
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
            A.CallTo(() => contextInfoMock.SetMetaData(ContextKeys.MetaMessageStringStart, "4")).MustHaveHappened();
            A.CallTo(() => contextInfoMock.SetMetaData(ContextKeys.MetaMessageStringEnd, "5")).MustHaveHappened();
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
        public void Create_ShouldAddText()
        {
            // Arrange
            var testee = CreateTestee();

            var entry = new Entry
            {
                MessageId = "message id",
                MessageString = "message string",
            };

            // Act
            testee.Create(entry, LineType.MessageId, false);

            // Assert
            A.CallTo(() => _sourceSegment0Mock.Add(_textMsgidMock)).MustHaveHappened();
        }

        [Test]
        public void Create_WhenTextHasSoftLineBreaks_ShouldReplaceSoftLineBreaks()
        {
            // Arrange
            var testee = CreateTestee();

            var entry = new Entry
            {
                MessageId = "message id\\n",
                MessageString = "message string",
            };

            var textPropertiesMsgidMock = A.Fake<ITextProperties>();
            A.CallTo(() => _propertiesFactoryMock.CreateTextProperties("message id\n")).Returns(textPropertiesMsgidMock);

           var textMsgidMock = A.Fake<IText>();
            A.CallTo(() => _itemFactoryMock.CreateText(textPropertiesMsgidMock)).Returns(textMsgidMock);

            // Act
            testee.Create(entry, LineType.MessageId, false);

            // Assert
            A.CallTo(() => _sourceSegment0Mock.Add(textMsgidMock)).MustHaveHappened();
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

            // Act
            testee.Create(entry, LineType.MessageId, false);

            // Assert
            A.CallTo(() => _sourceSegment0Mock.Add(_textMsgidMock)).MustHaveHappened();
            A.CallTo(() => _paragraphSourceMock.Add(_sourceSegment0Mock)).MustHaveHappened();
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

            // Act
            testee.Create(entry, LineType.MessageString, false);

            // Assert
            A.CallTo(() => _sourceSegment0Mock.Add(_textMsgstrMock)).MustHaveHappened();
            A.CallTo(() => _paragraphSourceMock.Add(_sourceSegment0Mock)).MustHaveHappened();
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

            // Act
            testee.Create(entry, LineType.MessageId, true);

            // Assert
            A.CallTo(() => _targetSegment0Mock.Add(_textMsgstrMock)).MustHaveHappened();
            A.CallTo(() => _paragraphTargetMock.Add(_targetSegment0Mock)).MustHaveHappened();
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

            // Act
            testee.Create(entry, LineType.MessageId, false);

            // Assert
            A.CallTo(() => _targetSegment0Mock.Add(_textMsgstrMock)).MustNotHaveHappened();
            A.CallTo(() => _paragraphTargetMock.Add(_targetSegment0Mock)).MustNotHaveHappened();
        }

        [Test]
        public void Create_WhenHasPluralFormAndTargetNeeded_ShouldAddMsgidWithFirstMsgstr()
        {
            // Arrange
            var testee = CreateTestee();

            var entry = new Entry
            {
                MessageId = "message id",
                MessageIdPlural = "message id plural",
                MessageStringPlurals = new List<string>
                {
                    "message string 0",
                    "message string 1",
                    "message string 2",
                },
            };

            // Act
            testee.Create(entry, LineType.MessageId, true);

            // Assert
            A.CallTo(() => _sourceSegment0Mock.Add(_textMsgidMock)).MustHaveHappened();
            A.CallTo(() => _targetSegment0Mock.Add(_textMsgstr0Mock)).MustHaveHappened();
            A.CallTo(() => _paragraphSourceMock.Add(_sourceSegment0Mock)).MustHaveHappened();
            A.CallTo(() => _paragraphTargetMock.Add(_targetSegment0Mock)).MustHaveHappened();
        }

        [Test]
        public void Create_WhenHasPluralFormAndTargetNeeded_ShouldAddMsgidpluralWithSecondMsgstr()
        {
            // Arrange
            var testee = CreateTestee();

            var entry = new Entry
            {
                MessageId = "message id",
                MessageIdPlural = "message id plural",
                MessageStringPlurals = new List<string>
                {
                    "message string 0",
                    "message string 1",
                    "message string 2",
                },
            };

            // Act
            testee.Create(entry, LineType.MessageId, true);

            // Assert
            A.CallTo(() => _sourceSegment1Mock.Add(_textMsgidPluralMock)).MustHaveHappened();
            A.CallTo(() => _targetSegment1Mock.Add(_textMsgstr1Mock)).MustHaveHappened();
            A.CallTo(() => _paragraphSourceMock.Add(_sourceSegment1Mock)).MustHaveHappened();
            A.CallTo(() => _paragraphTargetMock.Add(_targetSegment1Mock)).MustHaveHappened();
        }

        [Test]
        public void Create_WhenHasPluralFormAndTargetNeeded_ShouldAddMsgidpluralWithThirdMsgstr()
        {
            // Arrange
            var testee = CreateTestee();

            var entry = new Entry
            {
                MessageId = "message id",
                MessageIdPlural = "message id plural",
                MessageStringPlurals = new List<string>
                {
                    "message string 0",
                    "message string 1",
                    "message string 2",
                },
            };

            // Act
            testee.Create(entry, LineType.MessageId, true);

            // Assert
            A.CallTo(() => _sourceSegment2Mock.Add(_textMsgidPluralMock)).MustHaveHappened();
            A.CallTo(() => _targetSegment2Mock.Add(_textMsgstr2Mock)).MustHaveHappened();
            A.CallTo(() => _paragraphSourceMock.Add(_sourceSegment2Mock)).MustHaveHappened();
            A.CallTo(() => _paragraphTargetMock.Add(_targetSegment2Mock)).MustHaveHappened();
        }

        [Test]
        public void Create_ShouldSetSegmentPairToUnspecified()
        {
            // Arrange
            var testee = CreateTestee();

            var entry = new Entry
            {
                MessageId = "message id",
                MessageString = "message string",
            };

            // Act
            testee.Create(entry, LineType.MessageId, false);

            // Assert
            _segmentPairPropertiesMock.ConfirmationLevel.Should().Be(ConfirmationLevel.Unspecified);
            _translationOriginMmock.OriginType.Should().Be(DefaultTranslationOrigin.NotTranslated);
        }

        [Test]
        public void Create_WhenTargetTextIsNeededAndMsgstrIsSetAndSourceIsMsgid_ShouldSetSegmentPairToMachineTranslated()
        {
            // Arrange
            var testee = CreateTestee();

            var entry = new Entry
            {
                MessageId = "message id",
                MessageString = "message string",
            };

            // Act
            testee.Create(entry, LineType.MessageId, true);

            // Assert
            _segmentPairPropertiesMock.ConfirmationLevel.Should().Be(ConfirmationLevel.Translated);
            _translationOriginMmock.OriginType.Should().Be(DefaultTranslationOrigin.MachineTranslation);
        }


        public ParagraphUnitFactory CreateTestee()
        {
            return new ParagraphUnitFactory
            {
                ItemFactory = _itemFactoryMock,
                PropertiesFactory = _propertiesFactoryMock,
            };
        }
    }
}