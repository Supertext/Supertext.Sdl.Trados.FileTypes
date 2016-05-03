using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.JsonFile.Parsing;
using Supertext.Sdl.Trados.FileType.Utils.FileHandling;

namespace Supertext.Sdl.Trados.FileType.JsonFile
{
    public class JsonFileWriter : AbstractNativeFileWriter, INativeContentCycleAware
    {
        private readonly IJsonFactory _jsonFactory;
        private readonly IFileHelper _fileHelper;
        private IPersistentFileConversionProperties _conversionProperties;
       
        //State during writing
        private IJToken _rootToken;
        private StringBuilder _currentPropertyValueBuilder;
        private string _currentPath;

        public JsonFileWriter(IJsonFactory jsonFactory, IFileHelper fileHelper)
        {
            _jsonFactory = jsonFactory;
            _fileHelper = fileHelper;
        }

        public void SetFileProperties(IFileProperties properties)
        {
            _conversionProperties = properties.FileConversionProperties;
        }

        public void StartOfInput()
        {
            _rootToken = _jsonFactory.GetRootToken(_conversionProperties.OriginalFilePath);
            _currentPropertyValueBuilder = new StringBuilder();
        }

        public void EndOfInput()
        {
            _fileHelper.WriteAllText(OutputProperties.OutputFilePath, _rootToken.ToString());
            _rootToken = null;
        }

        public override void ParagraphUnitStart(IParagraphUnitProperties properties)
        {
            if (properties.Contexts == null)
            {
                return;
            }

            _currentPath = properties.Contexts.Contexts[1].GetMetaData(ContextKeys.ValuePath);
        }

        public override void ParagraphUnitEnd()
        {
            if (_currentPath != null)
            {
                var selectedToken = _rootToken.SelectToken(_currentPath);

                if (selectedToken.Type != JTokenType.String)
                {
                    return;
                }

                var property = selectedToken.Parent;

                property.Replace(property.Name, _currentPropertyValueBuilder.ToString());
            }

            _currentPath = null;
            _currentPropertyValueBuilder = new StringBuilder();
        }

        public override void Text(ITextProperties textInfo)
        {
            _currentPropertyValueBuilder.Append(textInfo.Text);
        }

        public override void InlinePlaceholderTag(IPlaceholderTagProperties tagInfo)
        {
            _currentPropertyValueBuilder.Append(tagInfo.TagContent);
        }

        public override void InlineStartTag(IStartTagProperties tagInfo)
        {
            _currentPropertyValueBuilder.Append(tagInfo.TagContent);
        }

        public override void InlineEndTag(IEndTagProperties tagInfo)
        {
            _currentPropertyValueBuilder.Append(tagInfo.TagContent);
        }
    }
}