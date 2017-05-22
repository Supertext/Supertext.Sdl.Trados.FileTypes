using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Supertext.Sdl.Trados.FileType.YamlFile.Parsing
{
    internal class YamlTextReader : IYamlTextReader, IParsingEventVisitor
    {
        private readonly Stack<IBranch> _branches;
        private readonly Stack<string> _currentPath;
        private readonly StreamReader _input;
        private readonly Parser _parser;

        public YamlTextReader(string file)
        {
            _input = File.OpenText(file);
            _parser = new Parser(_input);
            _branches = new Stack<IBranch>();
            _currentPath = new Stack<string>();
        }

        public int LineNumber { get; private set; }

        public string Path { get; private set; }

        public string Value { get; private set; }

        public bool Read()
        {
            Value = null;
            Path = null;

            var hasMore = _parser.MoveNext();

            if (!hasMore)
            {
                return false;
            }
            
            _parser.Current.Accept(this);

            LineNumber = _parser.Current.Start.Line;

            return true;
        }

        public void Visit(AnchorAlias e)
        {
            _branches.Peek().Continue();
        }

        public void Visit(StreamStart e)
        {
        }

        public void Visit(StreamEnd e)
        {
        }

        public void Visit(DocumentStart e)
        {
        }

        public void Visit(DocumentEnd e)
        {
        }

        public void Visit(Scalar e)
        {
            var currentBranch = _branches.Peek();

            if (!currentBranch.IsComplete)
            {
                currentBranch.SetScalar(e.Value);
                return;
            }

            if (!IsText(e))
            {
                currentBranch.Continue();
                return;
            }

            _currentPath.Push(currentBranch.GetSubPath());

            Path = GetStringPath(_currentPath);
            Value = e.Value;

            _currentPath.Pop();

            currentBranch.Continue();
        }

        public void Visit(SequenceStart e)
        {
            AddBranch(new Sequence());
        }

        public void Visit(SequenceEnd e)
        {
            _branches.Pop();
            _currentPath.Pop();
        }

        public void Visit(MappingStart e)
        {
            AddBranch(new Map());
        }

        public void Visit(MappingEnd e)
        {
            _branches.Pop();
            _currentPath.Pop();
        }

        public void Visit(Comment e)
        {
        }

        public void Close()
        {
            _input?.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _input?.Dispose();
            }
        }

        private static string GetStringPath(IEnumerable<string> path)
        {
            return string.Join("", path.Reverse()).TrimStart('.');
        }

        private static bool IsText(Scalar e)
        {
            return Regex.IsMatch(e.Value, "[a-zA-Z]+");
        }

        private void AddBranch(IBranch newBranch)
        {
            var subPath = string.Empty;

            if (_branches.Count > 0)
            {
                var currentBranch = _branches.Peek();
                subPath = currentBranch.GetSubPath();
                currentBranch.Continue();
            }

            _currentPath.Push(subPath);

            _branches.Push(newBranch);
        }
    }
}