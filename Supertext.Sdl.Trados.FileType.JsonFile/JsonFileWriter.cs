using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Supertext.Sdl.Trados.FileType.JsonFile
{
    public class JsonFileWriter : AbstractNativeFileWriter, INativeContentCycleAware
    {
        private IPersistentFileConversionProperties _conversionProperties;
        private string _currentPath;
        private StringBuilder _currentPropertyValueBuilder;

        //State during writing
        private JToken _rootToken;

        public void SetFileProperties(IFileProperties properties)
        {
            _conversionProperties = properties.FileConversionProperties;
        }

        public void StartOfInput()
        {
            var input = File.ReadAllText(_conversionProperties.OriginalFilePath);
            _rootToken = JToken.Parse(input);
            _currentPropertyValueBuilder = new StringBuilder();
        }

        public void EndOfInput()
        {
            File.WriteAllText(OutputProperties.OutputFilePath, _rootToken.ToString());
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

                var property = (JProperty)selectedToken.Parent;

                property.Replace(new JProperty(property.Name, _currentPropertyValueBuilder.ToString()));
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