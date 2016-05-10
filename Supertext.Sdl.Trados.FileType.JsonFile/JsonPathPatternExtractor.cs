using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Supertext.Sdl.Trados.FileType.JsonFile.Parsing;

namespace Supertext.Sdl.Trados.FileType.JsonFile
{
    public class JsonPathPatternExtractor
    {
        private readonly IJsonFactory _jsonFactory;

        public JsonPathPatternExtractor(IJsonFactory jsonFactory)
        {
            _jsonFactory = jsonFactory;
        }

        public IEnumerable<string> ExtractPathPatterns(string file)
        {
            var pathPatterns = new HashSet<string>();
            return Extract(file, pathPatterns);
        }

        public IEnumerable<string> ExtractPathPatterns(string file, IEnumerable<string> existingPathPatterns)
        {
            var pathPatterns = new HashSet<string>();

            foreach (var existingPathPattern in existingPathPatterns)
            {
                pathPatterns.Add(existingPathPattern);
            }

            return Extract(file, pathPatterns);
        }

        private IEnumerable<string> Extract(string file, HashSet<string> pathPatterns)
        {
            try
            {
                using (var reader = _jsonFactory.CreateJsonTextReader(file))
                {
                    while (reader.Read())
                    {
                        if (reader.TokenType != JsonToken.String)
                        {
                            continue;
                        }

                        var patternedPath = reader.Path.Replace(".", @"\.");
                        patternedPath = Regex.Replace(patternedPath, @"\[\d+\]", @"\[\d+\]");
                        pathPatterns.Add(patternedPath);
                    }
                }
            }
            catch (Exception)
            {
                return new List<string>();
            }

            return pathPatterns.ToList();
        }
    }
}
