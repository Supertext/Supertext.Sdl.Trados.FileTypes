﻿using Newtonsoft.Json;
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
            var selectedToken = _token.SelectToken(path);

            return selectedToken == null ? null : new JTokenWrapper(selectedToken);
        }

        public string ToString(Formatting formatting = Formatting.None)
        {
            return _token.ToString(formatting);
        }

        public JTokenType Type
        {
            get { return _token.Type; }
        }

        public IJValue Value
        {
            get
            {
                var jValue = _token as JValue;
                return jValue == null ? null : new JValueWrapper(jValue);
            }
        }
    }
}