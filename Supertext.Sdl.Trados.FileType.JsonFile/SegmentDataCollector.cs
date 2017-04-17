using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Supertext.Sdl.Trados.FileType.JsonFile.Settings;

namespace Supertext.Sdl.Trados.FileType.JsonFile
{
    public class SegmentDataCollector
    {
        private static readonly Regex IndexerRegex = new Regex(@"\[\d\]");

        private readonly IParsingSettings _parsingSettings;
        private readonly Dictionary<string, KeyValuePair<string, string>> _tempSources;
        private readonly Dictionary<string, KeyValuePair<string, string>> _tempTargets;

        public SegmentDataCollector(IParsingSettings parsingSettings)
        {
            _parsingSettings = parsingSettings;
            _tempSources = new Dictionary<string, KeyValuePair<string, string>>();
            _tempTargets = new Dictionary<string, KeyValuePair<string, string>>();
        }

        public SegmentData CompleteSegmentData { get; set; }

        public void Add(string path, string value)
        {
            CompleteSegmentData = null;

            if (!_parsingSettings.IsPathFilteringEnabled)
            {
                SetCompleteSegmentData(path, value, path, value);

                return;
            }
            
            CollectByPathRules(path, value);
        }

        private void CollectByPathRules(string path, string value)
        {
            foreach (var pathRule in _parsingSettings.PathRules)
            {
                var regex = new Regex(pathRule.SourcePathPattern,
                    pathRule.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

                if (regex.IsMatch(path))
                {
                    var key = GetKey(pathRule, path);

                    if (_tempTargets.ContainsKey(key))
                    {
                        var target = _tempTargets[key];

                        SetCompleteSegmentData(path, value, target.Key, target.Value);

                        return;
                    }

                    _tempSources.Add(key, new KeyValuePair<string, string>(path, value));
                    return;
                }

                regex = new Regex(pathRule.TargetPathPattern,
                    pathRule.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

                if (regex.IsMatch(path))
                {
                    var key = GetKey(pathRule, path);

                    if (_tempSources.ContainsKey(key))
                    {
                        var source = _tempSources[key];

                        SetCompleteSegmentData(source.Key, source.Value, path, value);

                        return;
                    }

                    _tempTargets.Add(key, new KeyValuePair<string, string>(path, value));
                    return;
                }
            }
        }

        private void SetCompleteSegmentData(string sourcePath, string sourceValue, string targetPath, string targetValue)
        {
            CompleteSegmentData = new SegmentData
            {
                SourcePath = sourcePath,
                SourceValue = sourceValue,
                TargetPath = targetPath,
                TargetValue = targetValue
            };
        }

        private static string GetKey(PathRule pathRule, string path)
        {
            var key = pathRule.GetHashCode().ToString();

            return IndexerRegex.Matches(path).Cast<Match>().Aggregate(key, (current, match) => current + match.Value);
        }
    }

    public class SegmentData
    {
        public string SourcePath { get; set; }

        public string SourceValue { get; set; }

        public string TargetPath { get; set; }

        public object TargetValue { get; set; }
    }
}