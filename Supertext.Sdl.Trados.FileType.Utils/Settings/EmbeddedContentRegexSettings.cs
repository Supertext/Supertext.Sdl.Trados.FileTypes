using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Supertext.Sdl.Trados.FileType.Utils.Settings
{
    public sealed class EmbeddedContentRegexSettings : FileTypeSettingsBase, IEmbeddedContentRegexSettings
    {
        private const string SettingRegexEmbeddingEnabled = "RegexEmbeddingEnabled";
        private const string SettingStructureInfoList = "StructureInfoList";
        private const string SettingMatchRuleList = "MatchRuleList";
        private const bool DefaultRegexEmbeddingEnabled = true;

        private static readonly ObservableList<string> DefaultStructureInfos = new ObservableList<string>
        {
            "sdl:field"
        };

        public static ComplexObservableList<MatchRule> DefaultMatchRules = new ComplexObservableList<MatchRule>
        {
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.Placeholder,
                StartTagRegexValue = @"\$\[\w+\]",
                SegmentationHint = SegmentationHint.IncludeWithText
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.Placeholder,
                StartTagRegexValue = @"%\w+",
                SegmentationHint = SegmentationHint.IncludeWithText
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.Placeholder,
                StartTagRegexValue = @"\$\w+",
                SegmentationHint = SegmentationHint.IncludeWithText
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.Placeholder,
                StartTagRegexValue = @"{?{\d+}}?",
                SegmentationHint = SegmentationHint.IncludeWithText
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<base([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/base[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<command([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/command[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<link([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/link[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<meta([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/meta[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<noscript([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/noscript[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<script([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/script[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<style([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/style[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<title([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/title[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<abbr([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/abbr[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<area([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/area[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<address([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/address[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<article([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/article[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<aside([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/aside[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<audio([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/audio[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<bdo([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/bdo[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<blockquote([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/blockquote[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<button([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/button[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<canvas([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/canvas[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<datalist([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/datalist[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<del([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/del[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<details([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/details[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<dialog([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/dialog[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<div([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/div[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<dl([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/dl[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<fieldset([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/fieldset[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<figure([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/figure[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<footer([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/footer[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<form([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/form[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<h1([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/h1[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<h2([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/h2[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<h3([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/h3[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<h4([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/h4[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<h5([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/h5[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<h6([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/h6[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<header([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/header[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<hgroup([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/hgroup[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<hr([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/hr[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<input([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/input[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<ins([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/ins[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<label([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/label[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<map([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/map[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<mark([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/mark[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<meter([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/meter[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<nav([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/nav[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<ol([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/ol[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<object([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/object[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<output([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/output[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<p([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/p[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<pre([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/pre[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<progress([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/progress[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<q([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/q[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<ruby([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/ruby[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<samp([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/samp[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<section([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/section[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<select([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/select[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<svg([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/svg[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<table([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/table[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<textarea([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/textarea[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<time([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/time[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<ul([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/ul[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<html([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/html[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<body([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/body[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<head([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/head[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<dd([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/dd[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<dt([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/dt[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<li([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/li[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<rt([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/rt[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<legend([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/legend[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<option([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/option[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<optgroup([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/optgroup[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<caption([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/caption[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<colgroup([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/colgroup[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<thead([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/thead[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<tfoot([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/tfoot[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<tbody([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/tbody[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<tr([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/tr[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<col([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/col[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<th([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/th[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<td([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/td[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<summary([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/summary[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<figcaption([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/figcaption[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<bdi([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/bdi[^<>]*>",
                SegmentationHint = SegmentationHint.Exclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                StartTagRegexValue = @"<br[^<>]*\s*\/?>",
                TagType = MatchRule.TagTypeOption.Placeholder,
                SegmentationHint = SegmentationHint.MayExclude
            },
            new MatchRule
            {
                StartTagRegexValue = @"<area[^<>]*\s*\/?>",
                TagType = MatchRule.TagTypeOption.Placeholder,
                SegmentationHint = SegmentationHint.MayExclude
            },
            new MatchRule
            {
                StartTagRegexValue = @"<base[^<>]*\s*\/?>",
                TagType = MatchRule.TagTypeOption.Placeholder,
                SegmentationHint = SegmentationHint.MayExclude
            },
            new MatchRule
            {
                StartTagRegexValue = @"<col[^<>]*\s*\/?>",
                TagType = MatchRule.TagTypeOption.Placeholder,
                SegmentationHint = SegmentationHint.MayExclude
            },
            new MatchRule
            {
                StartTagRegexValue = @"<embed[^<>]*\s*\/?>",
                TagType = MatchRule.TagTypeOption.Placeholder,
                SegmentationHint = SegmentationHint.MayExclude
            },
            new MatchRule
            {
                StartTagRegexValue = @"<hr[^<>]*\s*\/?>",
                TagType = MatchRule.TagTypeOption.Placeholder,
                SegmentationHint = SegmentationHint.MayExclude
            },
            new MatchRule
            {
                StartTagRegexValue = @"<img[^<>]*\s*\/?>",
                TagType = MatchRule.TagTypeOption.Placeholder,
                SegmentationHint = SegmentationHint.MayExclude
            },
            new MatchRule
            {
                StartTagRegexValue = @"<input[^<>]*\s*\/?>",
                TagType = MatchRule.TagTypeOption.Placeholder,
                SegmentationHint = SegmentationHint.MayExclude
            },
            new MatchRule
            {
                StartTagRegexValue = @"<link[^<>]*\s*\/?>",
                TagType = MatchRule.TagTypeOption.Placeholder,
                SegmentationHint = SegmentationHint.MayExclude
            },
            new MatchRule
            {
                StartTagRegexValue = @"<meta[^<>]*\s*\/?>",
                TagType = MatchRule.TagTypeOption.Placeholder,
                SegmentationHint = SegmentationHint.MayExclude
            },
            new MatchRule
            {
                StartTagRegexValue = @"<param[^<>]*\s*\/?>",
                TagType = MatchRule.TagTypeOption.Placeholder,
                SegmentationHint = SegmentationHint.MayExclude
            },
            new MatchRule
            {
                StartTagRegexValue = @"<source[^<>]*\s*\/?>",
                TagType = MatchRule.TagTypeOption.Placeholder,
                SegmentationHint = SegmentationHint.MayExclude
            },
            new MatchRule
            {
                StartTagRegexValue = @"<track[^<>]*\s*\/?>",
                TagType = MatchRule.TagTypeOption.Placeholder,
                SegmentationHint = SegmentationHint.MayExclude
            },
            new MatchRule
            {
                StartTagRegexValue = @"<wbr[^<>]*\s*\/?>",
                TagType = MatchRule.TagTypeOption.Placeholder,
                SegmentationHint = SegmentationHint.MayExclude
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.Placeholder,
                StartTagRegexValue = @"<[a-zA-Z][a-zA-Z0-9]*[^<>]*\s*\/>",
                SegmentationHint = SegmentationHint.MayExclude
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.TagPair,
                StartTagRegexValue = @"<[a-zA-Z][a-zA-Z0-9]*([^<>\/]|\"".*?\"")*>",
                EndTagRegexValue = @"<\/[a-zA-Z][a-zA-Z0-9]*[^<>]*>",
                SegmentationHint = SegmentationHint.MayExclude,
                IsContentTranslatable = true
            },
            new MatchRule
            {
                TagType = MatchRule.TagTypeOption.Placeholder,
                StartTagRegexValue = "&([a-z0-9]+|#[0-9]{1,6}|#x[0-9a-fA-F]{1,6});",
                SegmentationHint = SegmentationHint.IncludeWithText
            }
        };

        private bool _isIsEnabled;
        private ObservableList<string> _structureInfos;
        private ComplexObservableList<MatchRule> _matchRules;

        public EmbeddedContentRegexSettings()
        {
            ResetToDefaults();
        }

        public bool IsEnabled
        {
            get { return _isIsEnabled; }
            set
            {
                _isIsEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        public ObservableList<string> StructureInfos
        {
            get { return _structureInfos; }
            set
            {
                _structureInfos = value;
                OnPropertyChanged("StructureInfos");
            }
        }

        public ComplexObservableList<MatchRule> MatchRules
        {
            get { return _matchRules; }
            set
            {
                _matchRules = value;
                OnPropertyChanged("MatchRules");
            }
        }

        public override void ResetToDefaults()
        {
            IsEnabled = DefaultRegexEmbeddingEnabled;
            StructureInfos = DefaultStructureInfos;
            MatchRules = DefaultMatchRules;
        }

        public override void PopulateFromSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            var settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
            IsEnabled = GetSettingFromSettingsGroup(settingsGroup, SettingRegexEmbeddingEnabled,
                DefaultRegexEmbeddingEnabled);

            if (settingsGroup.ContainsSetting(SettingStructureInfoList))
            {
                _structureInfos.Clear();
                _structureInfos.PopulateFromSettingsGroup(settingsGroup, SettingStructureInfoList);
            }

            if (settingsGroup.ContainsSetting(SettingMatchRuleList))
            {
                _matchRules.Clear();
                _matchRules.PopulateFromSettingsGroup(settingsGroup, SettingMatchRuleList);
            }
        }

        public override void SaveToSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            var settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
            UpdateSettingInSettingsGroup(settingsGroup, SettingRegexEmbeddingEnabled, IsEnabled,
                DefaultRegexEmbeddingEnabled);
            _structureInfos.SaveToSettingsGroup(settingsGroup, SettingStructureInfoList);
            _matchRules.SaveToSettingsGroup(settingsGroup, SettingMatchRuleList);
        }
    }
}