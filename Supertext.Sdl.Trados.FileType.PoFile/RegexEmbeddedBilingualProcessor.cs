using System.Linq;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Supertext.Sdl.Trados.FileType.PoFile.Settings;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public class RegexEmbeddedBilingualProcessor : AbstractBilingualContentProcessor, ISettingsAware
    {
        private readonly EmbeddedContentRegexSettings _settings;

        public RegexEmbeddedBilingualProcessor(EmbeddedContentRegexSettings settings)
        {
            _settings = settings;
        }

        public void InitializeSettings(ISettingsBundle settingsBundle, string configurationId)
        {
            _settings.PopulateFromSettingsBundle(settingsBundle, configurationId);
        }

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            if (_settings.Enabled)
            {
                ProcessParagraph(paragraphUnit.Source);
                ProcessParagraph(paragraphUnit.Target);
            }
            base.ProcessParagraphUnit(paragraphUnit);
        }

        private void ProcessParagraph(IParagraph paragraph)
        {
            var embeddedContentVisitor = new EmbeddedContentVisitor(ItemFactory, _settings.MatchRules.ToList());

            foreach (var current in paragraph)
            {
                current.AcceptVisitor(embeddedContentVisitor);
            }

            var generatedParagraph = embeddedContentVisitor.GeneratedParagraph;

            CopyParagraphContents(generatedParagraph, paragraph);
        }

        private void CopyParagraphContents(IParagraph fromParagraph, IParagraph toParagraph)
        {
            toParagraph.Clear();
            while (fromParagraph.Count > 0)
            {
                var item = fromParagraph[0];
                fromParagraph.RemoveAt(0);
                toParagraph.Add(item);
            }
        }
    }
}