using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Supertext.Sdl.Trados.FileType.YamlFile.Parsing;

namespace Supertext.Sdl.Trados.FileType.YamlFile
{
    public class YamlPathPatternExtractor
    {
        private readonly IYamlFactory _yamlFactory;

        public YamlPathPatternExtractor(IYamlFactory yamlFactory)
        {
            _yamlFactory = yamlFactory;
        }

        public IEnumerable<string> ExtractPathPatterns(string file)
        {
            var pathPatterns = new HashSet<string>();
            try
            {
                using (var reader = _yamlFactory.CreateYamlTextReader(file))
                {
                    while (reader.Read())
                    {
                        if (String.IsNullOrEmpty(reader.Value))
                        {
                            continue;
                        }

                        var patternedPath = reader.Path.Replace(".", @"\.");
                        patternedPath = patternedPath.Replace("[", @"\[");
                        patternedPath = patternedPath.Replace("]", @"\]");
                        patternedPath = Regex.Replace(patternedPath, @"\\\[\d+\\\]", @"\[\d+\]");
                        pathPatterns.Add($"{RegexConstants.StartChar}{patternedPath}{RegexConstants.EndChar}");
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
