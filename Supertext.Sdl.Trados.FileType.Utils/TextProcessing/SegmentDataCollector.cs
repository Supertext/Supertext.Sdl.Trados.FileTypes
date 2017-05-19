using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Supertext.Sdl.Trados.FileType.Utils.Settings;

namespace Supertext.Sdl.Trados.FileType.Utils.TextProcessing
{
    public class SegmentDataCollector : ISegmentDataCollector
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
                SetCompleteSegmentData(path, value, path, string.Empty);

                return;
            }
            
            CollectByPathRules(path, value);
        }

        private static string GetKey(PathRule pathRule, string path)
        {
            var key = pathRule.GetHashCode().ToString();

            return IndexerRegex.Matches(path).Cast<Match>().Aggregate(key, (current, match) => current + match.Value);
        }

        private void CollectByPathRules(string path, string value)
        {
            foreach (var pathRule in _parsingSettings.PathRules)
            {
                var regex = new Regex(pathRule.SourcePathPattern,
                    pathRule.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

                if (regex.IsMatch(path))
                {
                    ProcessAsSource(pathRule, path, value);
                    return;
                }

                if (!pathRule.IsBilingual)
                {
                    continue;
                }

                regex = new Regex(pathRule.TargetPathPattern,
                    pathRule.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

                if (regex.IsMatch(path))
                {
                    ProcessAsTarget(pathRule, path, value);
                    return;
                }
            }
        }

        private void ProcessAsSource(PathRule pathRule, string path, string value)
        {
            if (!pathRule.IsBilingual)
            {
                SetCompleteSegmentData(path, value, path, string.Empty);
                return;
            }

            var key = GetKey(pathRule, path);

            if (_tempTargets.ContainsKey(key))
            {
                var target = _tempTargets[key];

                SetCompleteSegmentData(path, value, target.Key, pathRule.IsTargetValueNeeded ? target.Value : string.Empty);

                return;
            }

            _tempSources.Add(key, new KeyValuePair<string, string>(path, value));
        }

        private void ProcessAsTarget(PathRule pathRule, string path, string value)
        {
            var key = GetKey(pathRule, path);

            if (_tempSources.ContainsKey(key))
            {
                var source = _tempSources[key];

                SetCompleteSegmentData(source.Key, source.Value, path, pathRule.IsTargetValueNeeded ? value : string.Empty);

                return;
            }

            _tempTargets.Add(key, new KeyValuePair<string, string>(path, value));
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
    }
}