using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Css
{
	internal static class CssData
	{
		private static CssNameIndex[] InitializeNameIndex()
		{
			CssNameIndex[] array = new CssNameIndex[136];
			for (int i = 0; i < CssData.names.Length; i++)
			{
				if (CssData.names[i].PublicNameId != CssNameIndex.Unknown)
				{
					array[(int)CssData.names[i].PublicNameId] = (CssNameIndex)i;
				}
			}
			array[0] = CssNameIndex.Unknown;
			return array;
		}

		public const short MAX_NAME = 26;

		public const short MAX_TAG_NAME = 26;

		public const short NAME_HASH_SIZE = 329;

		public const int NAME_HASH_MODIFIER = 2;

		public static CssNameIndex[] nameHashTable = new CssNameIndex[]
		{
			CssNameIndex.ScrollbarArrowColor,
			CssNameIndex.Unknown,
			CssNameIndex.WhiteSpace,
			CssNameIndex.Unknown,
			CssNameIndex.LineBreak,
			CssNameIndex.Orphans,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.WritingMode,
			CssNameIndex.Unknown,
			CssNameIndex.Scrollbar3dLightColor,
			CssNameIndex.Unknown,
			CssNameIndex.TextAutospace,
			CssNameIndex.VerticalAlign,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.BorderRight,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Bottom,
			CssNameIndex.LineHeight,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.BorderBottom,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.ScrollbarBaseColor,
			CssNameIndex.MinWidth,
			CssNameIndex.BackgroundColor,
			CssNameIndex.Unknown,
			CssNameIndex.BorderTopStyle,
			CssNameIndex.Unknown,
			CssNameIndex.EmptyCells,
			CssNameIndex.Unknown,
			CssNameIndex.ListStyleType,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.TextAlign,
			CssNameIndex.Unknown,
			CssNameIndex.FontWeight,
			CssNameIndex.Unknown,
			CssNameIndex.OutlineWidth,
			CssNameIndex.CaptionSide,
			CssNameIndex.ScrollbarShadowColor,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Clip,
			CssNameIndex.Unknown,
			CssNameIndex.MarginLeft,
			CssNameIndex.BorderTopWidth,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Azimuth,
			CssNameIndex.Float,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.LayoutFlow,
			CssNameIndex.MinHeight,
			CssNameIndex.Content,
			CssNameIndex.Unknown,
			CssNameIndex.Padding,
			CssNameIndex.BorderBottomWidth,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Visibility,
			CssNameIndex.Unknown,
			CssNameIndex.Overflow,
			CssNameIndex.BorderLeftColor,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Pitch,
			CssNameIndex.Pause,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.OverflowY,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.ScrollbarHighlightColor,
			CssNameIndex.Unknown,
			CssNameIndex.Height,
			CssNameIndex.Unknown,
			CssNameIndex.WordWrap,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Top,
			CssNameIndex.ListStyle,
			CssNameIndex.Unknown,
			CssNameIndex.Margin,
			CssNameIndex.Unknown,
			CssNameIndex.TextKashidaSpace,
			CssNameIndex.VoiceFamily,
			CssNameIndex.CueBefore,
			CssNameIndex.Clear,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.TextOverflow,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.BorderBottomStyle,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.BorderColor,
			CssNameIndex.TextDecoration,
			CssNameIndex.Display,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.CounterReset,
			CssNameIndex.MarginBottom,
			CssNameIndex.BorderStyle,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.LayoutGrid,
			CssNameIndex.Quotes,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Accelerator,
			CssNameIndex.Border,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Zoom,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.OutlineStyle,
			CssNameIndex.Unknown,
			CssNameIndex.Width,
			CssNameIndex.Unknown,
			CssNameIndex.Color,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.PageBreakInside,
			CssNameIndex.Unknown,
			CssNameIndex.PitchRange,
			CssNameIndex.BorderCollapse,
			CssNameIndex.Cue,
			CssNameIndex.Unknown,
			CssNameIndex.Left,
			CssNameIndex.LayoutGridMode,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.SpeakPunctuation,
			CssNameIndex.LayoutGridLine,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.BorderSpacing,
			CssNameIndex.Unknown,
			CssNameIndex.TextTransform,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.BorderRightWidth,
			CssNameIndex.PageBreakBefore,
			CssNameIndex.TextIndent,
			CssNameIndex.LayoutGridChar,
			CssNameIndex.SpeechRate,
			CssNameIndex.PauseBefore,
			CssNameIndex.Unknown,
			CssNameIndex.ScrollbarFaceColor,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.PlayDuring,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.WordBreak,
			CssNameIndex.BorderBottomColor,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.MarginRight,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.SpeakNumeral,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.TextJustify,
			CssNameIndex.PaddingRight,
			CssNameIndex.BorderRightStyle,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.CounterIncrement,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.TextUnderlinePosition,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.WordSpacing,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Background,
			CssNameIndex.Unknown,
			CssNameIndex.OverflowX,
			CssNameIndex.BorderWidth,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.ZIndex,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.MaxWidth,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.ScrollbarDarkshadowColor,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.CueAfter,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.SpeakHeader,
			CssNameIndex.Unknown,
			CssNameIndex.Direction,
			CssNameIndex.FontVariant,
			CssNameIndex.Unknown,
			CssNameIndex.Richness,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Font,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Outline,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.BorderRightColor,
			CssNameIndex.Unknown,
			CssNameIndex.FontStyle,
			CssNameIndex.MarginTop,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.BorderLeft,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.ListStylePosition,
			CssNameIndex.Unknown,
			CssNameIndex.BorderLeftWidth,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.PaddingBottom,
			CssNameIndex.Unknown,
			CssNameIndex.LayoutGridType,
			CssNameIndex.PageBreakAfter,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.FontSize,
			CssNameIndex.Unknown,
			CssNameIndex.Position,
			CssNameIndex.BorderLeftStyle,
			CssNameIndex.PaddingLeft,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Right,
			CssNameIndex.PauseAfter,
			CssNameIndex.MaxHeight,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.LetterSpacing,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.Unknown,
			CssNameIndex.BorderTop
		};

		public static CssData.NameDef[] names = new CssData.NameDef[]
		{
			new CssData.NameDef(0, null, CssNameIndex.Unknown),
			new CssData.NameDef(0, "scrollbar-arrow-color", CssNameIndex.ScrollbarArrowColor),
			new CssData.NameDef(2, "white-space", CssNameIndex.WhiteSpace),
			new CssData.NameDef(4, "line-break", CssNameIndex.LineBreak),
			new CssData.NameDef(5, "orphans", CssNameIndex.Orphans),
			new CssData.NameDef(10, "writing-mode", CssNameIndex.WritingMode),
			new CssData.NameDef(12, "scrollbar-3dlight-color", CssNameIndex.Scrollbar3dLightColor),
			new CssData.NameDef(14, "text-autospace", CssNameIndex.TextAutospace),
			new CssData.NameDef(15, "vertical-align", CssNameIndex.VerticalAlign),
			new CssData.NameDef(18, "border-right", CssNameIndex.BorderRight),
			new CssData.NameDef(21, "bottom", CssNameIndex.Bottom),
			new CssData.NameDef(21, "font-family", CssNameIndex.FontFamily),
			new CssData.NameDef(22, "line-height", CssNameIndex.LineHeight),
			new CssData.NameDef(25, "border-bottom", CssNameIndex.BorderBottom),
			new CssData.NameDef(32, "scrollbar-base-color", CssNameIndex.ScrollbarBaseColor),
			new CssData.NameDef(33, "min-width", CssNameIndex.MinWidth),
			new CssData.NameDef(34, "background-color", CssNameIndex.BackgroundColor),
			new CssData.NameDef(36, "border-top-style", CssNameIndex.BorderTopStyle),
			new CssData.NameDef(38, "empty-cells", CssNameIndex.EmptyCells),
			new CssData.NameDef(40, "list-style-type", CssNameIndex.ListStyleType),
			new CssData.NameDef(45, "text-align", CssNameIndex.TextAlign),
			new CssData.NameDef(47, "font-weight", CssNameIndex.FontWeight),
			new CssData.NameDef(49, "outline-width", CssNameIndex.OutlineWidth),
			new CssData.NameDef(50, "caption-side", CssNameIndex.CaptionSide),
			new CssData.NameDef(51, "scrollbar-shadow-color", CssNameIndex.ScrollbarShadowColor),
			new CssData.NameDef(55, "clip", CssNameIndex.Clip),
			new CssData.NameDef(55, "volume", CssNameIndex.Volume),
			new CssData.NameDef(57, "margin-left", CssNameIndex.MarginLeft),
			new CssData.NameDef(58, "border-top-width", CssNameIndex.BorderTopWidth),
			new CssData.NameDef(61, "azimuth", CssNameIndex.Azimuth),
			new CssData.NameDef(61, "unicode-bidi", CssNameIndex.UnicodeBidi),
			new CssData.NameDef(62, "float", CssNameIndex.Float),
			new CssData.NameDef(66, "layout-flow", CssNameIndex.LayoutFlow),
			new CssData.NameDef(67, "min-height", CssNameIndex.MinHeight),
			new CssData.NameDef(68, "content", CssNameIndex.Content),
			new CssData.NameDef(70, "padding", CssNameIndex.Padding),
			new CssData.NameDef(71, "border-bottom-width", CssNameIndex.BorderBottomWidth),
			new CssData.NameDef(74, "visibility", CssNameIndex.Visibility),
			new CssData.NameDef(76, "overflow", CssNameIndex.Overflow),
			new CssData.NameDef(76, "table-layout", CssNameIndex.TableLayout),
			new CssData.NameDef(77, "border-left-color", CssNameIndex.BorderLeftColor),
			new CssData.NameDef(80, "pitch", CssNameIndex.Pitch),
			new CssData.NameDef(81, "pause", CssNameIndex.Pause),
			new CssData.NameDef(89, "overflow-y", CssNameIndex.OverflowY),
			new CssData.NameDef(93, "scrollbar-highlight-color", CssNameIndex.ScrollbarHighlightColor),
			new CssData.NameDef(95, "height", CssNameIndex.Height),
			new CssData.NameDef(97, "word-wrap", CssNameIndex.WordWrap),
			new CssData.NameDef(104, "top", CssNameIndex.Top),
			new CssData.NameDef(105, "list-style", CssNameIndex.ListStyle),
			new CssData.NameDef(107, "margin", CssNameIndex.Margin),
			new CssData.NameDef(109, "text-kashida-space", CssNameIndex.TextKashidaSpace),
			new CssData.NameDef(110, "voice-family", CssNameIndex.VoiceFamily),
			new CssData.NameDef(111, "cue-before", CssNameIndex.CueBefore),
			new CssData.NameDef(112, "clear", CssNameIndex.Clear),
			new CssData.NameDef(116, "text-overflow", CssNameIndex.TextOverflow),
			new CssData.NameDef(125, "border-bottom-style", CssNameIndex.BorderBottomStyle),
			new CssData.NameDef(128, "border-color", CssNameIndex.BorderColor),
			new CssData.NameDef(129, "text-decoration", CssNameIndex.TextDecoration),
			new CssData.NameDef(130, "display", CssNameIndex.Display),
			new CssData.NameDef(136, "counter-reset", CssNameIndex.CounterReset),
			new CssData.NameDef(137, "margin-bottom", CssNameIndex.MarginBottom),
			new CssData.NameDef(138, "border-style", CssNameIndex.BorderStyle),
			new CssData.NameDef(142, "layout-grid", CssNameIndex.LayoutGrid),
			new CssData.NameDef(143, "quotes", CssNameIndex.Quotes),
			new CssData.NameDef(147, "accelerator", CssNameIndex.Accelerator),
			new CssData.NameDef(148, "border", CssNameIndex.Border),
			new CssData.NameDef(151, "zoom", CssNameIndex.Zoom),
			new CssData.NameDef(154, "outline-style", CssNameIndex.OutlineStyle),
			new CssData.NameDef(156, "width", CssNameIndex.Width),
			new CssData.NameDef(158, "color", CssNameIndex.Color),
			new CssData.NameDef(163, "page-break-inside", CssNameIndex.PageBreakInside),
			new CssData.NameDef(165, "pitch-range", CssNameIndex.PitchRange),
			new CssData.NameDef(166, "border-collapse", CssNameIndex.BorderCollapse),
			new CssData.NameDef(166, "speak", CssNameIndex.Speak),
			new CssData.NameDef(167, "cue", CssNameIndex.Cue),
			new CssData.NameDef(169, "left", CssNameIndex.Left),
			new CssData.NameDef(170, "layout-grid-mode", CssNameIndex.LayoutGridMode),
			new CssData.NameDef(173, "speak-punctuation", CssNameIndex.SpeakPunctuation),
			new CssData.NameDef(174, "layout-grid-line", CssNameIndex.LayoutGridLine),
			new CssData.NameDef(179, "border-spacing", CssNameIndex.BorderSpacing),
			new CssData.NameDef(181, "text-transform", CssNameIndex.TextTransform),
			new CssData.NameDef(185, "border-right-width", CssNameIndex.BorderRightWidth),
			new CssData.NameDef(186, "page-break-before", CssNameIndex.PageBreakBefore),
			new CssData.NameDef(187, "text-indent", CssNameIndex.TextIndent),
			new CssData.NameDef(188, "layout-grid-char", CssNameIndex.LayoutGridChar),
			new CssData.NameDef(189, "speech-rate", CssNameIndex.SpeechRate),
			new CssData.NameDef(190, "pause-before", CssNameIndex.PauseBefore),
			new CssData.NameDef(192, "scrollbar-face-color", CssNameIndex.ScrollbarFaceColor),
			new CssData.NameDef(196, "play-during", CssNameIndex.PlayDuring),
			new CssData.NameDef(199, "word-break", CssNameIndex.WordBreak),
			new CssData.NameDef(200, "border-bottom-color", CssNameIndex.BorderBottomColor),
			new CssData.NameDef(208, "margin-right", CssNameIndex.MarginRight),
			new CssData.NameDef(211, "speak-numeral", CssNameIndex.SpeakNumeral),
			new CssData.NameDef(216, "text-justify", CssNameIndex.TextJustify),
			new CssData.NameDef(217, "padding-right", CssNameIndex.PaddingRight),
			new CssData.NameDef(218, "border-right-style", CssNameIndex.BorderRightStyle),
			new CssData.NameDef(221, "counter-increment", CssNameIndex.CounterIncrement),
			new CssData.NameDef(227, "text-underline-position", CssNameIndex.TextUnderlinePosition),
			new CssData.NameDef(233, "word-spacing", CssNameIndex.WordSpacing),
			new CssData.NameDef(236, "background", CssNameIndex.Background),
			new CssData.NameDef(238, "overflow-x", CssNameIndex.OverflowX),
			new CssData.NameDef(239, "border-width", CssNameIndex.BorderWidth),
			new CssData.NameDef(239, "widows", CssNameIndex.Widows),
			new CssData.NameDef(245, "z-index", CssNameIndex.ZIndex),
			new CssData.NameDef(245, "border-top-color", CssNameIndex.BorderTopColor),
			new CssData.NameDef(252, "max-width", CssNameIndex.MaxWidth),
			new CssData.NameDef(257, "scrollbar-darkshadow-color", CssNameIndex.ScrollbarDarkshadowColor),
			new CssData.NameDef(261, "cue-after", CssNameIndex.CueAfter),
			new CssData.NameDef(269, "speak-header", CssNameIndex.SpeakHeader),
			new CssData.NameDef(271, "direction", CssNameIndex.Direction),
			new CssData.NameDef(272, "font-variant", CssNameIndex.FontVariant),
			new CssData.NameDef(274, "richness", CssNameIndex.Richness),
			new CssData.NameDef(274, "stress", CssNameIndex.Stress),
			new CssData.NameDef(281, "font", CssNameIndex.Font),
			new CssData.NameDef(281, "elevation", CssNameIndex.Elevation),
			new CssData.NameDef(285, "outline", CssNameIndex.Outline),
			new CssData.NameDef(289, "border-right-color", CssNameIndex.BorderRightColor),
			new CssData.NameDef(291, "font-style", CssNameIndex.FontStyle),
			new CssData.NameDef(292, "margin-top", CssNameIndex.MarginTop),
			new CssData.NameDef(295, "border-left", CssNameIndex.BorderLeft),
			new CssData.NameDef(298, "list-style-position", CssNameIndex.ListStylePosition),
			new CssData.NameDef(298, "outline-color", CssNameIndex.OutlineColor),
			new CssData.NameDef(300, "border-left-width", CssNameIndex.BorderLeftWidth),
			new CssData.NameDef(305, "padding-bottom", CssNameIndex.PaddingBottom),
			new CssData.NameDef(307, "layout-grid-type", CssNameIndex.LayoutGridType),
			new CssData.NameDef(308, "page-break-after", CssNameIndex.PageBreakAfter),
			new CssData.NameDef(311, "font-size", CssNameIndex.FontSize),
			new CssData.NameDef(313, "position", CssNameIndex.Position),
			new CssData.NameDef(314, "border-left-style", CssNameIndex.BorderLeftStyle),
			new CssData.NameDef(314, "padding-top", CssNameIndex.PaddingTop),
			new CssData.NameDef(315, "padding-left", CssNameIndex.PaddingLeft),
			new CssData.NameDef(318, "right", CssNameIndex.Right),
			new CssData.NameDef(319, "pause-after", CssNameIndex.PauseAfter),
			new CssData.NameDef(320, "max-height", CssNameIndex.MaxHeight),
			new CssData.NameDef(323, "letter-spacing", CssNameIndex.LetterSpacing),
			new CssData.NameDef(328, "border-top", CssNameIndex.BorderTop),
			new CssData.NameDef(329, null, CssNameIndex.Unknown)
		};

		public static CssNameIndex[] nameIndex = CssData.InitializeNameIndex();

		public static CssData.FilterActionEntry[] filterInstructions = new CssData.FilterActionEntry[]
		{
			new CssData.FilterActionEntry(CssData.FilterAction.Drop),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.CheckContent),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.CheckContent),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.CheckContent),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.CheckContent),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.CheckContent),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.CheckContent),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.CheckContent),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Keep),
			new CssData.FilterActionEntry(CssData.FilterAction.Drop)
		};

		public struct NameDef
		{
			public NameDef(short hash, string name, CssNameIndex publicNameId)
			{
				this.Hash = hash;
				this.Name = name;
				this.PublicNameId = publicNameId;
			}

			public short Hash;

			public string Name;

			public CssNameIndex PublicNameId;
		}

		public enum FilterAction : byte
		{
			Unknown,
			Drop,
			Keep,
			CheckContent
		}

		public struct FilterActionEntry
		{
			public FilterActionEntry(CssData.FilterAction propertyAction)
			{
				this.propertyAction = propertyAction;
			}

			public CssData.FilterAction propertyAction;
		}
	}
}
