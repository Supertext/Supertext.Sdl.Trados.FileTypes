using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            if (currentBranch.IsKeyNeeded)
            {
                currentBranch.SetKey(e.Value);
                return;
            }

            _currentPath.Push(currentBranch.GetNextSubPath());

            Path = GetStringPath(_currentPath);
            Value = e.Value;

            _currentPath.Pop();
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

        private void AddBranch(IBranch newBranch)
        {
            _currentPath.Push(_branches.Count > 0 ? _branches.Peek().GetNextSubPath() : string.Empty);

            _branches.Push(newBranch);
        }

        private static string GetStringPath(IEnumerable<string> path)
        {
            return string.Join("", path.Reverse()).TrimStart('.');
        }
    }
}