using System.Collections.Generic;
using System.Linq;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Supertext.Sdl.Trados.FileType.PoFile.Settings;
using Supertext.Sdl.Trados.FileType.PoFile.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.PoFile.Paragraphing
{
    public class RegexEmbeddedBilingualProcessor : AbstractBilingualContentProcessor, ISettingsAware
    {
        private readonly IEmbeddedContentVisitorFactory _embeddedContentVisitorFactory;
        private readonly IEmbeddedContentRegexSettings _settings;
        private ITextProcessor _textProcessor;

        public RegexEmbeddedBilingualProcessor(IEmbeddedContentVisitorFactory embeddedContentVisitorFactory, IEmbeddedContentRegexSettings settings)
        {
            _embeddedContentVisitorFactory = embeddedContentVisitorFactory;
            _settings = settings;
            _textProcessor = new TextProcessor(new List<MatchRule>());
        }

        public void InitializeSettings(ISettingsBundle settingsBundle, string configurationId)
        {
            _settings.PopulateFromSettingsBundle(settingsBundle, configurationId);
            _textProcessor = new TextProcessor(_settings.MatchRules.ToList());
        }

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            if (_settings.IsEnabled)
            {
                ProcessParagraph(paragraphUnit.Source);
                ProcessParagraph(paragraphUnit.Target);
            }
            base.ProcessParagraphUnit(paragraphUnit);
        }

        private void ProcessParagraph(IParagraph paragraph)
        {
            var embeddedContentVisitor = _embeddedContentVisitorFactory.CreateVisitor(ItemFactory, ItemFactory.PropertiesFactory, _textProcessor);

            foreach (var item in paragraph)
            {
                item.AcceptVisitor(embeddedContentVisitor);
            }

            var generatedParagraph = embeddedContentVisitor.GeneratedParagraph;

            CopyParagraphContents(generatedParagraph, paragraph);
        }

        private static void CopyParagraphContents(IParagraph fromParagraph, IParagraph toParagraph)
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