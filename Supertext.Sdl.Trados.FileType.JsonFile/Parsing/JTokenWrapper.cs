using Newtonsoft.Json.Linq;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Parsing
{
    internal class JTokenWrapper : IJToken
    {
        private readonly JToken _token;

        public JTokenWrapper(JToken token)
        {
            _token = token;
        }

        public IJToken SelectToken(string path)
        {
            return new JTokenWrapper(_token.SelectToken(path));
        }

        public JTokenType Type
        {
            get { return _token.Type; }
        }

        public IJProperty Parent
        {
            get { return new JPropertyWrapper((JProperty) _token.Parent); }
        }

        public override string ToString()
        {
            return _token.ToString();
        }
    }
}