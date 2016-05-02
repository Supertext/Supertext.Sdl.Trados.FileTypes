using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.JsonFile.Resources;

namespace Supertext.Sdl.Trados.FileType.JsonFile
{
    public class JsonFileSniffer : INativeFileSniffer
    {
        public SniffInfo Sniff(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage,
            INativeTextLocationMessageReporter messageReporter, ISettingsGroup settingsGroup)
        {
            try
            {
                var input = File.ReadAllText(nativeFilePath);
                JToken.Parse(input);
            }
            catch (JsonException e)
            {
                messageReporter.ReportMessage(this, nativeFilePath,
                    ErrorLevel.Error, JsonFileTypeResources.Invalid_Json_Format, e.Message);

                return new SniffInfo {IsSupported = false};
            }
            catch (Exception)
            {
                messageReporter.ReportMessage(this, nativeFilePath,
                    ErrorLevel.Error, JsonFileTypeResources.Json_Could_Not_Be_Validated, string.Empty);

                return new SniffInfo { IsSupported = false };
            }

            return new SniffInfo {IsSupported = true};
        }
    }
}