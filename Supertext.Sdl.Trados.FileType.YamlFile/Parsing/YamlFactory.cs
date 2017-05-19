namespace Supertext.Sdl.Trados.FileType.YamlFile.Parsing
{
    public class YamlFactory : IYamlFactory
    {
        public IYamlTextReader CreateYamlTextReader(string file)
        {
            return new YamlTextReader(file);
        }

        public IYamlTextWriter CreateYamlTextWriter(string inputFile, string outputFile)
        {
            return new YamlTextWriter(inputFile, outputFile);
        }
    }
}