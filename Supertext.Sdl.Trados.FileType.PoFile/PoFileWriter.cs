using System.IO;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class PoFileWriter : AbstractNativeFileWriter, INativeContentCycleAware
    {
        private readonly IFileHelper _fileHelper;
        private IStreamWriter _streamWriter;

        public PoFileWriter(IFileHelper fileHelper)
        {
            _fileHelper = fileHelper;
        }

        public void SetFileProperties(IFileProperties properties)
        {
            
        }

        public void StartOfInput()
        {
            _streamWriter = _fileHelper.GetStreamWriter(OutputProperties.OutputFilePath);
        }

        public void EndOfInput()
        {
            _streamWriter.Close();
            _streamWriter.Dispose();
            _streamWriter = null;
        }

        public override void StructureTag(IStructureTagProperties tagInfo)
        {
            _streamWriter.WriteLine(tagInfo.TagContent);
        }

        public override void Text(ITextProperties textInfo)
        {
            _streamWriter.Write(textInfo.Text);
        }

        public override void InlineStartTag(IStartTagProperties tagInfo)
        {
            _streamWriter.Write(tagInfo.TagContent);
        }

        public override void InlineEndTag(IEndTagProperties tagInfo)
        {
            _streamWriter.Write(tagInfo.TagContent);
        }

        public override void SegmentEnd()
        {
            _streamWriter.WriteLine();
        }
    }
}