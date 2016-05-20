using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.JsonFile.Parsing;
using Supertext.Sdl.Trados.FileType.Utils.FileHandling;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Tests
{
    [TestFixture]
    public class JsonFileWriterTest
    {
        private const string ThePath = "the.path";
        private IJToken _rootTokenMock;
        private IJProperty _parentProperty;

        [SetUp]
        public void SetUp()
        {
            _rootTokenMock = A.Fake<IJToken>();
            _parentProperty = A.Fake<IJProperty>();
            A.CallTo(() => _parentProperty.Name).Returns("parentproperty");
        }

        [Test]
        public void ParagraphUnitEnd_WhenIsStringToken_ShouldReplaceCorrespondingPropertyWithNewStringContent()
        {
            // Arrange
            var testee = CreateTestee();
            
            // Act
            testee.ParagraphUnitEnd();

            // Assert
            A.CallTo(() => _parentProperty.Replace(_parentProperty.Name, A<string>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void ParagraphUnitEnd_WhenIsNotStringToken_ShouldDoNothing()
        {
            // Arrange
            var testee = CreateTestee();

            var selectedToken = A.Fake<IJToken>();

            A.CallTo(() => _rootTokenMock.SelectToken(ThePath)).Returns(selectedToken);
            A.CallTo(() => selectedToken.Type).Returns(JTokenType.Property);

            // Act
            testee.ParagraphUnitEnd();

            // Assert
            A.CallTo(() => _parentProperty.Replace(A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void ParagraphUnitEnd_WhenTextHasBeenCalled_ShouldReplacePropertyWithNewText()
        {
            // Arrange
            const string testText = "test text";
            var testee = CreateTestee();

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => textPropertiesMock.Text).Returns(testText);

            testee.Text(textPropertiesMock);

            // Act
            testee.ParagraphUnitEnd();

            // Assert
            A.CallTo(() => _parentProperty.Replace(A<string>.Ignored, testText)).MustHaveHappened();
        }

        [Test]
        public void ParagraphUnitEnd_WhenTextHasBeenSuccessivelyCalled_ShouldReplacePropertyWithNewText()
        {
            // Arrange
            const string testText = "test text";
            var testee = CreateTestee();

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => textPropertiesMock.Text).Returns(testText);

            testee.Text(textPropertiesMock);
            testee.Text(textPropertiesMock);

            // Act
            testee.ParagraphUnitEnd();

            // Assert
            A.CallTo(() => _parentProperty.Replace(A<string>.Ignored, testText+testText)).MustHaveHappened();
        }

        [Test]
        public void ParagraphUnitEnd_WhenInlinePlaceholderTagHasBeenCalled_ShouldReplacePropertyWithNewText()
        {
            // Arrange
            const string testText = "test placeholder";
            var testee = CreateTestee();

            var placeholderTagPropertiesMock = A.Fake<IPlaceholderTagProperties>();
            A.CallTo(() => placeholderTagPropertiesMock.TagContent).Returns(testText);

            testee.InlinePlaceholderTag(placeholderTagPropertiesMock);

            // Act
            testee.ParagraphUnitEnd();

            // Assert
            A.CallTo(() => _parentProperty.Replace(A<string>.Ignored, testText)).MustHaveHappened();
        }

        [Test]
        public void ParagraphUnitEnd_WhenInlinePlaceholderTagHasSuccessivelyBeenCalled_ShouldReplacePropertyWithNewText()
        {
            // Arrange
            const string testText = "test placeholder";
            var testee = CreateTestee();

            var placeholderTagPropertiesMock = A.Fake<IPlaceholderTagProperties>();
            A.CallTo(() => placeholderTagPropertiesMock.TagContent).Returns(testText);

            testee.InlinePlaceholderTag(placeholderTagPropertiesMock);
            testee.InlinePlaceholderTag(placeholderTagPropertiesMock);

            // Act
            testee.ParagraphUnitEnd();

            // Assert
            A.CallTo(() => _parentProperty.Replace(A<string>.Ignored, testText+testText)).MustHaveHappened();
        }

        [Test]
        public void ParagraphUnitEnd_WhenInlineStartTagHasBeenCalled_ShouldReplacePropertyWithNewText()
        {
            // Arrange
            const string testText = "test start tag";
            var testee = CreateTestee();

            var startTagPropertiesMock = A.Fake<IStartTagProperties>();
            A.CallTo(() => startTagPropertiesMock.TagContent).Returns(testText);

            testee.InlineStartTag(startTagPropertiesMock);

            // Act
            testee.ParagraphUnitEnd();

            // Assert
            A.CallTo(() => _parentProperty.Replace(A<string>.Ignored, testText)).MustHaveHappened();
        }

        [Test]
        public void ParagraphUnitEnd_WhenInlineStartTagHasSuccessivelyBeenCalled_ShouldReplacePropertyWithNewText()
        {
            // Arrange
            const string testText = "test start tag";
            var testee = CreateTestee();

            var startTagPropertiesMock = A.Fake<IStartTagProperties>();
            A.CallTo(() => startTagPropertiesMock.TagContent).Returns(testText);

            testee.InlineStartTag(startTagPropertiesMock);
            testee.InlineStartTag(startTagPropertiesMock);

            // Act
            testee.ParagraphUnitEnd();

            // Assert
            A.CallTo(() => _parentProperty.Replace(A<string>.Ignored, testText + testText)).MustHaveHappened();
        }

        [Test]
        public void ParagraphUnitEnd_WhenInlineEndTagHasBeenCalled_ShouldReplacePropertyWithNewText()
        {
            // Arrange
            const string testText = "test end tag";
            var testee = CreateTestee();

            var endTagPropertiesMock = A.Fake<IEndTagProperties>();
            A.CallTo(() => endTagPropertiesMock.TagContent).Returns(testText);

            testee.InlineEndTag(endTagPropertiesMock);

            // Act
            testee.ParagraphUnitEnd();

            // Assert
            A.CallTo(() => _parentProperty.Replace(A<string>.Ignored, testText)).MustHaveHappened();
        }

        [Test]
        public void ParagraphUnitEnd_WhenInlineEndTagHasSuccessivelyBeenCalled_ShouldReplacePropertyWithNewText()
        {
            // Arrange
            const string testText = "test end tag";
            var testee = CreateTestee();

            var endTagPropertiesMock = A.Fake<IEndTagProperties>();
            A.CallTo(() => endTagPropertiesMock.TagContent).Returns(testText);

            testee.InlineEndTag(endTagPropertiesMock);
            testee.InlineEndTag(endTagPropertiesMock);

            // Act
            testee.ParagraphUnitEnd();

            // Assert
            A.CallTo(() => _parentProperty.Replace(A<string>.Ignored, testText + testText)).MustHaveHappened();
        }

        [Test]
        public void ParagraphUnitEnd_WhenMultipleTextAndTagsHaveBeenAdded_ShouldReplacePropertyWithNewText()
        {
            // Arrange
            const string testText = "test text";
            const string testPlaceholderTagText = "test placeholder text";
            const string testStartTagText = "test start tag";
            const string testEndTagText = "test end tag";
            var testee = CreateTestee();

            var textPropertiesMock = A.Fake<ITextProperties>();
            A.CallTo(() => textPropertiesMock.Text).Returns(testText);

            var placeholderTagPropertiesMock = A.Fake<IPlaceholderTagProperties>();
            A.CallTo(() => placeholderTagPropertiesMock.TagContent).Returns(testPlaceholderTagText);

            var startTagPropertiesMock = A.Fake<IStartTagProperties>();
            A.CallTo(() => startTagPropertiesMock.TagContent).Returns(testStartTagText);

            var endTagPropertiesMock = A.Fake<IEndTagProperties>();
            A.CallTo(() => endTagPropertiesMock.TagContent).Returns(testEndTagText);

            testee.Text(textPropertiesMock);
            testee.InlinePlaceholderTag(placeholderTagPropertiesMock);
            testee.InlineStartTag(startTagPropertiesMock);
            testee.InlineEndTag(endTagPropertiesMock);

            // Act
            testee.ParagraphUnitEnd();

            // Assert
            A.CallTo(() => _parentProperty.Replace(A<string>.Ignored, testText + testPlaceholderTagText + testStartTagText + testEndTagText)).MustHaveHappened();
        }

        private JsonFileWriter CreateTestee()
        {
            var jsonFactoryMock = A.Fake<IJsonFactory>();
           
            A.CallTo(() => jsonFactoryMock.GetRootToken(A<string>.Ignored)).Returns(_rootTokenMock);

            var fileHelperMock = A.Fake<IFileHelper>();
            var filePropertiesMock = A.Fake<IFileProperties>();
            var fileConversionPropertiesMock = A.Fake<IPersistentFileConversionProperties>();
            A.CallTo(() => filePropertiesMock.FileConversionProperties).Returns(fileConversionPropertiesMock);

            var paragraphUnitPropertiesMock = A.Fake<IParagraphUnitProperties>();
            var contextPropertiesMock = A.Fake<IContextProperties>();
            var contextInfoFieldMock = A.Fake<IContextInfo>();
            var contextInfoPathMock = A.Fake<IContextInfo>();
            A.CallTo(() => paragraphUnitPropertiesMock.Contexts).Returns(contextPropertiesMock);
            A.CallTo(() => contextPropertiesMock.Contexts[0]).Returns(contextInfoFieldMock);
            A.CallTo(() => contextPropertiesMock.Contexts[1]).Returns(contextInfoPathMock);
            A.CallTo(() => contextInfoPathMock.GetMetaData(ContextKeys.SourcePath)).Returns(ThePath);

            var selectedToken = A.Fake<IJToken>();
            
            A.CallTo(() => _rootTokenMock.SelectToken(ThePath)).Returns(selectedToken);
            A.CallTo(() => selectedToken.Type).Returns(JTokenType.String);
            A.CallTo(() => selectedToken.Parent).Returns(_parentProperty);

            var testee = new JsonFileWriter(jsonFactoryMock, fileHelperMock);
            testee.SetFileProperties(filePropertiesMock);
            testee.StartOfInput();
            testee.ParagraphUnitStart(paragraphUnitPropertiesMock);
            return testee;
        }
    }
}
