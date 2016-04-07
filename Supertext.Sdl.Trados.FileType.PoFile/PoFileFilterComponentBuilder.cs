using System.Collections.Generic;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Supertext.Sdl.Trados.FileType.PoFile.FileHandling;
using Supertext.Sdl.Trados.FileType.PoFile.Parsing;
using Supertext.Sdl.Trados.FileType.PoFile.ElementFactories;
using Supertext.Sdl.Trados.FileType.PoFile.Resources;
using Supertext.Sdl.Trados.FileType.PoFile.Settings;
using Supertext.Sdl.Trados.FileType.PoFile.TextProcessing;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    [FileTypeComponentBuilderAttribute(Id = "PoFile_FilterComponentBuilderExtension_Id",
        Name = "PoFile_FilterComponentBuilderExtension_Name",
        Description = "PoFile_FilterComponentBuilderExtension_Description")]
    public class PoFileFilterComponentBuilder : IFileTypeComponentBuilder
    {
        public IFileTypeManager FileTypeManager { get; set; }

        public IFileTypeDefinition FileTypeDefinition { get; set; }

        public IFileTypeInformation BuildFileTypeInformation(string name)
        {
            var info = FileTypeManager.BuildFileTypeInformation();

            info.FileTypeDefinitionId = new FileTypeDefinitionId("PO File Filter 1.0.0.0");
            info.FileTypeName = new LocalizableString("PO files");
            info.FileTypeDocumentName = new LocalizableString("PO files");
            info.FileTypeDocumentsName = new LocalizableString("PO files");
            info.Description = new LocalizableString("This filter is used to process PO files.");
            info.FileDialogWildcardExpression = "*.po";
            info.DefaultFileExtension = "po";
            info.Icon = new IconDescriptor(PoFileTypeResources.PoFileIcon);

            info.WinFormSettingsPageIds = new[]
            {
                "PoFile_Segment_Settings",
                "QuickInserts_Settings",
            };

            return info;
        }

        public IQuickTagsFactory BuildQuickTagsFactory(string name)
        {
            IQuickTagsFactory quickTags = FileTypeManager.BuildQuickTagsFactory();
            return quickTags;
        }

        public INativeFileSniffer BuildFileSniffer(string name)
        {
            return new PoFileSniffer(new FileHelper(), new LineParser());
        }

        public IFileExtractor BuildFileExtractor(string name)
        {
            var parser = new PoFileParser(
                new FileHelper(),
                new LineParser(),
                new SegmentSettings(),
                new ParagraphUnitFactory(), 
                new EntryBuilder());

            var fileExtractor = FileTypeManager.BuildFileExtractor(parser, this);

            //var processor = new EmbeddedContentProcessor();
            //fileExtractor.AddBilingualProcessor(processor);

            return fileExtractor;
        }

        public IFileGenerator BuildFileGenerator(string name)
        {
            var writer = new PoFileWriter(new FileHelper(), new SegmentReader(), new LineParser(), new EntryBuilder());
            return FileTypeManager.BuildFileGenerator(writer);
        }

        public IAdditionalGeneratorsInfo BuildAdditionalGeneratorsInfo(string name)
        {
            return null;
        }

        public IAbstractGenerator BuildAbstractGenerator(string name)
        {
            return null;
        }

        public IPreviewSetsFactory BuildPreviewSetsFactory(string name)
        {
            IPreviewSetsFactory previewFactory = FileTypeManager.BuildPreviewSetsFactory();

            return previewFactory;
        }

        public IAbstractPreviewControl BuildPreviewControl(string name)
        {
            return null;
        }

        public IAbstractPreviewApplication BuildPreviewApplication(string name)
        {
            return null;
        }

        public IBilingualDocumentGenerator BuildBilingualGenerator(string name)
        {
            return null;
        }

        public IVerifierCollection BuildVerifierCollection(string name)
        {
            return null;
        }
    }

    /*
    public class EmbeddedContentProcessor : RegexEmbeddedBilingualProcessor
    {
        protected override EmbeddedContentRegexSettings GetProcessorSettings()
        {
            return new EmbeddedContentRegexSettings();
        }
    }

    public class RegexEmbeddedBilingualProcessor : AbstractBilingualContentProcessor, ISettingsAware
    {
        private bool _isEnabled;
        private List<string> _structureInfos;
        private List<MatchRule> _matchRules;
        private EmbeddedContentRegexSettings _settings;

        public List<string> StructureInfo
        {
            get
            {
                return _structureInfos;
            }
            set
            {
                _structureInfos = value;
            }
        }

        public List<MatchRule> MatchRules
        {
            get
            {
                return this._matchRules;
            }
            set
            {
                this._matchRules = value;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return this._isEnabled;
            }
            set
            {
                this._isEnabled = value;
            }
        }

        public EmbeddedContentRegexSettings Settings
        {
            get
            {
                return this._settings;
            }
            set
            {
                this._settings = value;
                this.ApplySettings();
            }
        }

        public RegexEmbeddedBilingualProcessor()
        {
            this._settings = this.GetProcessorSettings();
            this._isEnabled = false;
            this._structureInfos = new List<string>();
            this._matchRules = new List<MatchRule>();
        }

        protected virtual EmbeddedContentRegexSettings GetProcessorSettings()
        {
            return new EmbeddedContentRegexSettings();
        }

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            if (this._isEnabled)
            {
                this.ProcessParagraph(paragraphUnit.Source);
                this.ProcessParagraph(paragraphUnit.Target);
            }
            base.ProcessParagraphUnit(paragraphUnit);
        }

        private void ProcessParagraph(IParagraph paragraph)
        {
            EmbeddedContentVisitor embeddedContentVisitor = new EmbeddedContentVisitor(this.ItemFactory, this._matchRules);
            foreach (IAbstractMarkupData current in paragraph)
            {
                current.AcceptVisitor(embeddedContentVisitor);
            }
            IParagraph generatedParagraph = embeddedContentVisitor.GeneratedParagraph;
            this.CopyParagraphContents(generatedParagraph, paragraph);
        }

        private void CopyParagraphContents(IParagraph fromParagraph, IParagraph toParagraph)
        {
            toParagraph.Clear();
            while (fromParagraph.Count > 0)
            {
                IAbstractMarkupData item = fromParagraph[0];
                fromParagraph.RemoveAt(0);
                toParagraph.Add(item);
            }
        }

        private void ApplySettings()
        {
            this._isEnabled = this._settings.Enabled;
            this._structureInfos.Clear();
            foreach (string current in this._settings.StructureInfos)
            {
                this._structureInfos.Add(current);
            }
            this._matchRules.Clear();
            foreach (MatchRule current2 in this._settings.MatchRules)
            {
                this._matchRules.Add(current2);
            }
        }

        public void InitializeSettings(ISettingsBundle settingsBundle, string configurationId)
        {
            this._settings.PopulateFromSettingsBundle(settingsBundle, configurationId);
            this.ApplySettings();
        }
    }

    internal class EmbeddedContentVisitor : IMarkupDataVisitor
    {
        private const string EmbeddedContentMetaKey = "OriginalEmbeddedContent";

        private List<MatchRule> _MatchRules;

        private IAbstractMarkupDataContainer _currentContainer;

        private IDocumentItemFactory _Factory;

        private IParagraph _ParentParagraph;

        public IParagraph GeneratedParagraph
        {
            get
            {
                return this._ParentParagraph;
            }
        }

        public EmbeddedContentVisitor(IDocumentItemFactory factory, List<MatchRule> matchRules)
        {
            this._Factory = factory;
            IParagraphUnit paragraphUnit = factory.CreateParagraphUnit(LockTypeFlags.Unlocked);
            this._ParentParagraph = paragraphUnit.Source;
            this._currentContainer = this._ParentParagraph;
            this._MatchRules = matchRules;
        }

        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            ICommentMarker commentMarker2 = this._Factory.CreateCommentMarker(commentMarker.Comments);
            this._currentContainer.Add(commentMarker2);
            this._currentContainer = commentMarker2;
            foreach (IAbstractMarkupData current in commentMarker)
            {
                current.AcceptVisitor(this);
            }
            this._currentContainer = commentMarker2.Parent;
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
            this._currentContainer.Add((ILocationMarker)location.Clone());
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
            this._currentContainer.Add((ILockedContent)lockedContent.Clone());
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
            IOtherMarker otherMarker = this._Factory.CreateOtherMarker();
            otherMarker.Id = marker.Id;
            otherMarker.MarkerType = marker.MarkerType;
            this._currentContainer.Add(otherMarker);
            this._currentContainer = otherMarker;
            foreach (IAbstractMarkupData current in marker)
            {
                current.AcceptVisitor(this);
            }
            this._currentContainer = otherMarker.Parent;
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            this._currentContainer.Add((IPlaceholderTag)tag.Clone());
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
            IRevisionMarker revisionMarker2 = this.CreateRevisionOrFeedback(revisionMarker.Properties);
            this._currentContainer.Add(revisionMarker2);
            this._currentContainer = revisionMarker2;
            foreach (IAbstractMarkupData current in revisionMarker)
            {
                current.AcceptVisitor(this);
            }
            this._currentContainer = revisionMarker2.Parent;
        }

        private IRevisionMarker CreateRevisionOrFeedback(IRevisionProperties properties)
        {
            switch (properties.RevisionType)
            {
                case RevisionType.Insert:
                case RevisionType.Delete:
                case RevisionType.Unchanged:
                    return this._Factory.CreateRevision(properties);
                case RevisionType.FeedbackComment:
                case RevisionType.FeedbackAdded:
                case RevisionType.FeedbackDeleted:
                    return this._Factory.CreateFeedback(properties);
                default:
                    return this._Factory.CreateRevision(properties);
            }
        }

        public void VisitSegment(ISegment segment)
        {
            ISegment segment2 = this._Factory.CreateSegment(segment.Properties);
            this._currentContainer.Add(segment2);
            this._currentContainer = segment2;
            foreach (IAbstractMarkupData current in segment)
            {
                current.AcceptVisitor(this);
            }
            this._currentContainer = segment2.Parent;
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            ITagPair tagPair2 = this._Factory.CreateTagPair(tagPair.StartTagProperties, tagPair.EndTagProperties);
            this._currentContainer.Add(tagPair2);
            this._currentContainer = tagPair2;
            foreach (IAbstractMarkupData current in tagPair)
            {
                current.AcceptVisitor(this);
            }
            this._currentContainer = tagPair2.Parent;
        }

        public void VisitText(IText text)
        {
            string text2 = text.Properties.Text;
            List<RegexMatch> list = RegexProcessorHelper.ApplyRegexRules(text2, this._MatchRules);
            int num = 0;
            foreach (RegexMatch current in list)
            {
                if (current.Index > num)
                {
                    this._currentContainer.Add(this.CreateText(text2.Substring(num, current.Index - num)));
                }
                else if (num > current.Index)
                {
                    continue;
                }
                switch (current.Type)
                {
                    case RegexMatch.TagType.Placeholder:
                        this._currentContainer.Add(this.CreatePlaceholderTag(current));
                        break;
                    case RegexMatch.TagType.TagPairOpening:
                        this.AddOpenTagContainer(current);
                        break;
                    case RegexMatch.TagType.TagPairClosing:
                        this.AddCloseTagContainer();
                        break;
                }
                num = current.Index + current.Value.Length;
            }
            if (num < text2.Length)
            {
                this._currentContainer.Add(this.CreateText(text2.Substring(num, text2.Length - num)));
            }
        }

        private void AddOpenTagContainer(RegexMatch match)
        {
            if (match.Rule.IsContentTranslatable)
            {
                ITagPair tagPair = this.CreateTagPair(match);
                this._currentContainer.Add(tagPair);
                this._currentContainer = tagPair;
                return;
            }
            ILockedContent lockedContent = this.CreateLockedContent(match);
            this._currentContainer.Add(lockedContent);
            this._currentContainer = lockedContent.Content;
        }

        private void AddCloseTagContainer()
        {
            ITagPair tagPair = this._currentContainer as ITagPair;
            if (tagPair != null)
            {
                this._currentContainer = tagPair.Parent;
                return;
            }
            ILockedContainer lockedContainer = this._currentContainer as ILockedContainer;
            if (lockedContainer != null)
            {
                this._currentContainer = lockedContainer.LockedContent.Parent;
            }
        }

        private ILockedContent CreateLockedContent(RegexMatch match)
        {
            ILockedContentProperties properties = this._Factory.PropertiesFactory.CreateLockedContentProperties(LockTypeFlags.Manual);
            return this._Factory.CreateLockedContent(properties);
        }

        private IText CreateText(string textContent)
        {
            ITextProperties textInfo = this._Factory.PropertiesFactory.CreateTextProperties(textContent);
            return this._Factory.CreateText(textInfo);
        }

        private ITagPair CreateTagPair(RegexMatch match)
        {
            IStartTagProperties startTagProperties = RegexProcessorHelper.CreateStartTagProperties(this._Factory.PropertiesFactory, match.Value, match.Rule);
            startTagProperties.SetMetaData("OriginalEmbeddedContent", match.Value);
            IEndTagProperties endTagProperties = RegexProcessorHelper.CreateEndTagProperties(this._Factory.PropertiesFactory, match.Value, match.Rule);
            endTagProperties.SetMetaData("OriginalEmbeddedContent", match.Value);
            return this._Factory.CreateTagPair(startTagProperties, endTagProperties);
        }

        private IPlaceholderTag CreatePlaceholderTag(RegexMatch match)
        {
            IPlaceholderTagProperties placeholderTagProperties = RegexProcessorHelper.CreatePlaceholderTagProperties(this._Factory.PropertiesFactory, match.Value, match.Rule);
            placeholderTagProperties.SetMetaData("OriginalEmbeddedContent", match.Value);
            return this._Factory.CreatePlaceholderTag(placeholderTagProperties);
        }
    }*/
}