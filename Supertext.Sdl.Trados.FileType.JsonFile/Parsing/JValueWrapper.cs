using Newtonsoft.Json.Linq;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Parsing
{
    internal class JValueWrapper : IJValue
    {
        private readonly JValue _value;

        public JValueWrapper(JValue value)
        {
            _value = value;
        }

        public object Value
        {
            get { return _value.Value; }
            set { _value.Value = value; }
        }
    }
}