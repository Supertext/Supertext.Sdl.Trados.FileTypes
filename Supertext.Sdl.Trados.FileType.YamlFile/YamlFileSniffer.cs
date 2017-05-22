using System;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.YamlFile.Parsing;
using Supertext.Sdl.Trados.FileType.YamlFile.Resources;
using YamlDotNet.Core;

namespace Supertext.Sdl.Trados.FileType.YamlFile
{
    public class YamlFileSniffer : INativeFileSniffer
    {
        private readonly IYamlFactory _yamlFactory;

        public YamlFileSniffer(IYamlFactory yamlFactory)
        {
            _yamlFactory = yamlFactory;
        }

        public SniffInfo Sniff(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage,
            INativeTextLocationMessageReporter messageReporter, ISettingsGroup settingsGroup)
        {
            try
            {
                using (var reader = _yamlFactory.CreateYamlTextReader(nativeFilePath))
                {
                    while (reader.Read())
                    {

                    }
                }
            }
            catch (YamlException e)
            {
                messageReporter.ReportMessage(this, nativeFilePath,
                    ErrorLevel.Error, YamlFileTypeResources.Invalid_Yaml_Format + "("+e.Message+")", e.Message);

                return new SniffInfo {IsSupported = false};
            }
            catch (Exception e)
            {
                messageReporter.ReportMessage(this, nativeFilePath,
                    ErrorLevel.Error, YamlFileTypeResources.Yaml_Could_Not_Be_Validated, e.Message);

                return new SniffInfo { IsSupported = false };
            }

            return new SniffInfo {IsSupported = true};
        }
    }
}