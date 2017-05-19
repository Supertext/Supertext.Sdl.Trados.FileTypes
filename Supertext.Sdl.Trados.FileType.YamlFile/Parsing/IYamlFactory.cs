namespace Supertext.Sdl.Trados.FileType.YamlFile.Parsing
{
    public interface IYamlFactory
    {
        IYamlTextReader CreateYamlTextReader(string file);

        IYamlTextWriter CreateYamlTextWriter(string inputFile, string outputFile);
    }
}
