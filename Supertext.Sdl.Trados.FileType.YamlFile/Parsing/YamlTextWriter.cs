using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Supertext.Sdl.Trados.FileType.YamlFile.Parsing
{
    public class YamlTextWriter : IYamlTextWriter, IParsingEventVisitor
    {
        private readonly Stack<IBranch> _branches;
        private readonly Stack<string> _currentPath;
        private readonly StreamReader _input;
        private readonly StreamWriter _ouput;
        private readonly Parser _parser;
        private readonly Emitter _emitter;

        private string _pathToWrite;
        private string _valueToWrite;

        public YamlTextWriter(string inputFile, string outputFile)
        {
            _input = File.OpenText(inputFile);
            _ouput = new StreamWriter(File.OpenWrite(outputFile));
            _parser = new Parser(new Scanner(_input, true));
            _emitter = new Emitter(_ouput);
            _branches = new Stack<IBranch>();
            _currentPath = new Stack<string>();
        }

        public void Write(string path, string value)
        {
            _pathToWrite = path;
            _valueToWrite = value;

            while (!string.IsNullOrEmpty(_pathToWrite) && _parser.MoveNext())
            {
                _parser.Current.Accept(this);
            }
        }

        public void FinishWriting()
        {
            while (_parser.MoveNext())
            {
                _parser.Current.Accept(this);
            }

            _ouput.Flush();
            _ouput.Close();
        }

        public void Visit(AnchorAlias e)
        {
            _emitter.Emit(e);
        }

        public void Visit(StreamStart e)
        {
            _emitter.Emit(e);
        }

        public void Visit(StreamEnd e)
        {
            _emitter.Emit(e);
        }

        public void Visit(DocumentStart e)
        {
            _emitter.Emit(e);
        }

        public void Visit(DocumentEnd e)
        {
            _emitter.Emit(e);
        }

        public void Visit(Scalar e)
        {
            var currentBranch = _branches.Peek();

            if (currentBranch.IsKeyNeeded)
            {
                _emitter.Emit(e);
                currentBranch.SetKey(e.Value);
                return;
            }

            _currentPath.Push(currentBranch.GetNextSubPath());

            if (GetStringPath(_currentPath) == _pathToWrite)
            {
                var newScalar = new Scalar(e.Anchor, e.Tag, _valueToWrite, e.Style, e.IsPlainImplicit, e.IsQuotedImplicit);
                _emitter.Emit(newScalar);
                _pathToWrite = null;
            }
            else
            {
                _emitter.Emit(e);
            }

            _currentPath.Pop();
        }

        public void Visit(SequenceStart e)
        {
            _emitter.Emit(e);
            AddBranch(new Sequence());
        }

        public void Visit(SequenceEnd e)
        {
            _emitter.Emit(e);
            _branches.Pop();
            _currentPath.Pop();
        }

        public void Visit(MappingStart e)
        {
            _emitter.Emit(e);
            AddBranch(new Map());
        }

        public void Visit(MappingEnd e)
        {
            _emitter.Emit(e);
            _branches.Pop();
            _currentPath.Pop();
        }

        public void Visit(Comment e)
        {
            _emitter.Emit(e);
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
                _ouput?.Dispose();
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