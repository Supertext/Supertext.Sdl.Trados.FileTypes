using Newtonsoft.Json.Linq;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Parsing
{
    internal class JPropertyWrapper : IJProperty
    {
        private readonly JProperty _property;

        public JPropertyWrapper(JProperty property)
        {
            _property = property;
        }

        public string Name
        {
            get { return _property.Name; }
        }

        public void Replace(string name, string value)
        {
            _property.Replace(new JProperty(name, value));
        }
    }
}