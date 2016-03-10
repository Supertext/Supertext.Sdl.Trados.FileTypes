using System;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.DotNetWrappers;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class PoFileParser : INativeFileParser, INativeContentCycleAware, ISettingsAware
    {
        private readonly IDotNetFactory _dotNetFactory;
        private readonly ILineValidator _lineValidator;
        private ILinePattern _lastLinePattern;
        private IPersistentFileConversionProperties _fileConversionProperties;
        private IReader _streamReader;

        public PoFileParser(IDotNetFactory dotNetFactory, ILineValidator lineValidator)
        {
            _dotNetFactory = dotNetFactory;
            _lineValidator = lineValidator;
        }

        public event EventHandler<ProgressEventArgs> Progress;

        public IPropertiesFactory PropertiesFactory { get; set; }

        public INativeContentStreamMessageReporter MessageReporter { get; set; }

        public INativeExtractionContentHandler Output { get; set; }

        public void SetFileProperties(IFileProperties properties)
        {
            _fileConversionProperties = properties.FileConversionProperties;
        }

        public void StartOfInput()
        {
            _streamReader = _dotNetFactory.CreateStreamReader(_fileConversionProperties.OriginalFilePath);
        }

        public void EndOfInput()
        {
            _streamReader.Close();
        }

        public bool ParseNext()
        {

            return false;
        }

        public void InitializeSettings(ISettingsBundle settingsBundle, string configurationId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _streamReader == null)
            {
                return;
            }

            _streamReader.Dispose();
            _streamReader = null;
        }
    }
}