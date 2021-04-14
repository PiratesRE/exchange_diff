using System;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal static class HtmlConverterData
	{
		public static HtmlTagInstruction[] tagInstructions = new HtmlTagInstruction[]
		{
			default(HtmlTagInstruction),
			default(HtmlTagInstruction),
			default(HtmlTagInstruction),
			default(HtmlTagInstruction),
			default(HtmlTagInstruction),
			default(HtmlTagInstruction),
			default(HtmlTagInstruction),
			default(HtmlTagInstruction),
			new HtmlTagInstruction(FormatContainerType.HyperLink, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Name, PropertyId.BookmarkName, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
				new HtmlAttributeInstruction(HtmlNameIndex.Href, PropertyId.HyperlinkUrl, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
				new HtmlAttributeInstruction(HtmlNameIndex.Target, PropertyId.HyperlinkTarget, HtmlConverterData.PropertyValueParsingMethods.ParseTarget),
				new HtmlAttributeInstruction(HtmlNameIndex.Id, PropertyId.Id, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			default(HtmlTagInstruction),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Block, 4, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 0, 2, null),
			new HtmlTagInstruction(FormatContainerType.Area, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Href, PropertyId.HyperlinkUrl, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
				new HtmlAttributeInstruction(HtmlNameIndex.Target, PropertyId.HyperlinkTarget, HtmlConverterData.PropertyValueParsingMethods.ParseTarget),
				new HtmlAttributeInstruction(HtmlNameIndex.Alt, PropertyId.ImageAltText, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage),
				new HtmlAttributeInstruction(HtmlNameIndex.Shape, PropertyId.Shape, HtmlConverterData.PropertyValueParsingMethods.ParseAreaShape),
				new HtmlAttributeInstruction(HtmlNameIndex.Coords, PropertyId.Coords, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 1, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			default(HtmlTagInstruction),
			new HtmlTagInstruction(FormatContainerType.BaseFont, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Face, PropertyId.FontFace, HtmlConverterData.PropertyValueParsingMethods.ParseFontFace),
				new HtmlAttributeInstruction(HtmlNameIndex.Size, PropertyId.FontSize, HtmlConverterData.PropertyValueParsingMethods.ParseFontSize),
				new HtmlAttributeInstruction(HtmlNameIndex.Color, PropertyId.FontColor, HtmlConverterData.PropertyValueParsingMethods.ParseColor)
			}),
			new HtmlTagInstruction(FormatContainerType.Inline, 11, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			default(HtmlTagInstruction),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 2, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.BlockQuote, 16, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Block, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.BGColor, PropertyId.BackColor, HtmlConverterData.PropertyValueParsingMethods.ParseColor),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 0, 2, null),
			new HtmlTagInstruction(FormatContainerType.Button, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Name, PropertyId.BookmarkName, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
				new HtmlAttributeInstruction(HtmlNameIndex.Value, PropertyId.QuotingLevelDelta, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
				new HtmlAttributeInstruction(HtmlNameIndex.Disabled, PropertyId.Overloaded2, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.TableCaption, 13, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseBlockAlignment)
			}),
			new HtmlTagInstruction(FormatContainerType.Block, 13, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 4, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 9, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.TableColumn, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Width, PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.Span, PropertyId.NumColumns, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Valign, PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseVerticalAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.TableColumnGroup, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Width, PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.Span, PropertyId.NumColumns, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Valign, PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseVerticalAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			default(HtmlTagInstruction),
			new HtmlTagInstruction(FormatContainerType.Block, 25, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 3, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 4, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.List, 24, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Block, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage),
				new HtmlAttributeInstruction(HtmlNameIndex.Id, PropertyId.Id, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty)
			}),
			new HtmlTagInstruction(FormatContainerType.Block, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Block, 25, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 4, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 0, 2, null),
			new HtmlTagInstruction(FormatContainerType.FieldSet, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Face, PropertyId.FontFace, HtmlConverterData.PropertyValueParsingMethods.ParseFontFace),
				new HtmlAttributeInstruction(HtmlNameIndex.Size, PropertyId.FontSize, HtmlConverterData.PropertyValueParsingMethods.ParseFontSize),
				new HtmlAttributeInstruction(HtmlNameIndex.Color, PropertyId.FontColor, HtmlConverterData.PropertyValueParsingMethods.ParseColor),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Form, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Action, PropertyId.HyperlinkUrl, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
				new HtmlAttributeInstruction(HtmlNameIndex.EncType, PropertyId.ImageUrl, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
				new HtmlAttributeInstruction(HtmlNameIndex.Accept, PropertyId.ImageAltText, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
				new HtmlAttributeInstruction(HtmlNameIndex.AcceptCharset, PropertyId.QuotingLevelDelta, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			default(HtmlTagInstruction),
			default(HtmlTagInstruction),
			new HtmlTagInstruction(FormatContainerType.Block, 17, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Block, 18, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Block, 19, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Block, 20, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Block, 21, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Block, 22, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			default(HtmlTagInstruction),
			new HtmlTagInstruction(FormatContainerType.HorizontalLine, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Size, PropertyId.Height, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.Width, PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.Color, PropertyId.FontColor, HtmlConverterData.PropertyValueParsingMethods.ParseColor),
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseHorizontalAlignment)
			}),
			default(HtmlTagInstruction),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 4, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Block, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseBlockAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Height, PropertyId.Height, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.Width, PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.Src, PropertyId.ImageUrl, HtmlConverterData.PropertyValueParsingMethods.ParseUrl)
			}),
			new HtmlTagInstruction(FormatContainerType.Image, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseBlockAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Src, PropertyId.ImageUrl, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
				new HtmlAttributeInstruction(HtmlNameIndex.Width, PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.Height, PropertyId.Height, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.Border, PropertyId.TableBorder, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.Alt, PropertyId.ImageAltText, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage),
				new HtmlAttributeInstruction(HtmlNameIndex.UseMap, PropertyId.BookmarkName, HtmlConverterData.PropertyValueParsingMethods.ParseUrl)
			}),
			new HtmlTagInstruction(FormatContainerType.Image, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseBlockAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Src, PropertyId.ImageUrl, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
				new HtmlAttributeInstruction(HtmlNameIndex.Width, PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.Height, PropertyId.Height, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.Border, PropertyId.TableBorder, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.Alt, PropertyId.ImageAltText, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage),
				new HtmlAttributeInstruction(HtmlNameIndex.UseMap, PropertyId.BookmarkName, HtmlConverterData.PropertyValueParsingMethods.ParseUrl)
			}),
			new HtmlTagInstruction(FormatContainerType.Input, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Name, PropertyId.BookmarkName, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
				new HtmlAttributeInstruction(HtmlNameIndex.ReadOnly, PropertyId.Overloaded1, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
				new HtmlAttributeInstruction(HtmlNameIndex.Disabled, PropertyId.Overloaded2, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
				new HtmlAttributeInstruction(HtmlNameIndex.Checked, PropertyId.Overloaded3, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
				new HtmlAttributeInstruction(HtmlNameIndex.Value, PropertyId.QuotingLevelDelta, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
				new HtmlAttributeInstruction(HtmlNameIndex.Size, PropertyId.TableFrame, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
				new HtmlAttributeInstruction(HtmlNameIndex.MaxLength, PropertyId.TableRules, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
				new HtmlAttributeInstruction(HtmlNameIndex.Src, PropertyId.ImageUrl, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
				new HtmlAttributeInstruction(HtmlNameIndex.Alt, PropertyId.ImageAltText, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 5, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Block, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Name, PropertyId.BookmarkName, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
				new HtmlAttributeInstruction(HtmlNameIndex.Prompt, PropertyId.ImageAltText, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
				new HtmlAttributeInstruction(HtmlNameIndex.Disabled, PropertyId.Overloaded2, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 9, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Label, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.For, PropertyId.HyperlinkUrl, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Legend, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseBlockAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.ListItem, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Start, PropertyId.ListStart, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			default(HtmlTagInstruction),
			new HtmlTagInstruction(FormatContainerType.Block, 15, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Map, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Name, PropertyId.BookmarkName, HtmlConverterData.PropertyValueParsingMethods.ParseUrl)
			}),
			new HtmlTagInstruction(FormatContainerType.Block, 0, 2, null),
			new HtmlTagInstruction(FormatContainerType.List, 24, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			default(HtmlTagInstruction),
			default(HtmlTagInstruction),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 12, 2, null),
			default(HtmlTagInstruction),
			default(HtmlTagInstruction),
			default(HtmlTagInstruction),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 0, 2, null),
			new HtmlTagInstruction(FormatContainerType.List, 23, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Start, PropertyId.ListStart, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.OptionGroup, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Label, PropertyId.ImageAltText, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
				new HtmlAttributeInstruction(HtmlNameIndex.Disabled, PropertyId.Overloaded2, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Option, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Selected, PropertyId.Overloaded1, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
				new HtmlAttributeInstruction(HtmlNameIndex.Label, PropertyId.ImageAltText, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
				new HtmlAttributeInstruction(HtmlNameIndex.Value, PropertyId.QuotingLevelDelta, HtmlConverterData.PropertyValueParsingMethods.ParseStringProperty),
				new HtmlAttributeInstruction(HtmlNameIndex.Disabled, PropertyId.Overloaded2, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Block, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 0, 2, null),
			new HtmlTagInstruction(FormatContainerType.Block, 14, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Block, 14, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage),
				new HtmlAttributeInstruction(HtmlNameIndex.Width, PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 0, 2, null),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 0, 2, null),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 0, 2, null),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 3, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 9, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			default(HtmlTagInstruction),
			new HtmlTagInstruction(FormatContainerType.Select, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Name, PropertyId.BookmarkName, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
				new HtmlAttributeInstruction(HtmlNameIndex.Multiple, PropertyId.Overloaded1, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
				new HtmlAttributeInstruction(HtmlNameIndex.Disabled, PropertyId.Overloaded2, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 6, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			default(HtmlTagInstruction),
			default(HtmlTagInstruction),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 3, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 1, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			default(HtmlTagInstruction),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 7, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 8, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.Table, 0, 3, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Width, PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseHorizontalAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Border, PropertyId.TableBorder, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.Frame, PropertyId.TableFrame, HtmlConverterData.PropertyValueParsingMethods.ParseTableFrame),
				new HtmlAttributeInstruction(HtmlNameIndex.Rules, PropertyId.TableRules, HtmlConverterData.PropertyValueParsingMethods.ParseTableRules),
				new HtmlAttributeInstruction(HtmlNameIndex.BGColor, PropertyId.BackColor, HtmlConverterData.PropertyValueParsingMethods.ParseColor),
				new HtmlAttributeInstruction(HtmlNameIndex.CellSpacing, PropertyId.TableCellSpacing, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.CellPadding, PropertyId.TableCellPadding, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			default(HtmlTagInstruction),
			new HtmlTagInstruction(FormatContainerType.TableExtraContent, 0, 2, null),
			new HtmlTagInstruction(FormatContainerType.TableCell, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Height, PropertyId.Height, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.Width, PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.ColSpan, PropertyId.NumColumns, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
				new HtmlAttributeInstruction(HtmlNameIndex.RowSpan, PropertyId.NumRows, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Valign, PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseVerticalAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.NoWrap, PropertyId.TableCellNoWrap, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
				new HtmlAttributeInstruction(HtmlNameIndex.BGColor, PropertyId.BackColor, HtmlConverterData.PropertyValueParsingMethods.ParseColor),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.TextArea, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Name, PropertyId.BookmarkName, HtmlConverterData.PropertyValueParsingMethods.ParseUrl),
				new HtmlAttributeInstruction(HtmlNameIndex.ReadOnly, PropertyId.Overloaded1, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
				new HtmlAttributeInstruction(HtmlNameIndex.Disabled, PropertyId.Overloaded2, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
				new HtmlAttributeInstruction(HtmlNameIndex.Cols, PropertyId.NumColumns, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
				new HtmlAttributeInstruction(HtmlNameIndex.Rows, PropertyId.NumRows, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			default(HtmlTagInstruction),
			new HtmlTagInstruction(FormatContainerType.TableCell, 0, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Height, PropertyId.Height, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.Width, PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.ColSpan, PropertyId.NumColumns, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
				new HtmlAttributeInstruction(HtmlNameIndex.RowSpan, PropertyId.NumRows, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeInteger),
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Valign, PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseVerticalAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.NoWrap, PropertyId.TableCellNoWrap, HtmlConverterData.PropertyValueParsingMethods.ParseBooleanAttribute),
				new HtmlAttributeInstruction(HtmlNameIndex.BGColor, PropertyId.BackColor, HtmlConverterData.PropertyValueParsingMethods.ParseColor),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			default(HtmlTagInstruction),
			default(HtmlTagInstruction),
			new HtmlTagInstruction(FormatContainerType.TableRow, 0, 4, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Height, PropertyId.Height, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength),
				new HtmlAttributeInstruction(HtmlNameIndex.Align, PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.Valign, PropertyId.BlockAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseVerticalAlignment),
				new HtmlAttributeInstruction(HtmlNameIndex.BGColor, PropertyId.BackColor, HtmlConverterData.PropertyValueParsingMethods.ParseColor),
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 10, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 5, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.List, 24, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			new HtmlTagInstruction(FormatContainerType.PropertyContainer, 4, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			default(HtmlTagInstruction),
			new HtmlTagInstruction(FormatContainerType.Block, 14, 2, new HtmlAttributeInstruction[]
			{
				new HtmlAttributeInstruction(HtmlNameIndex.Dir, PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection),
				new HtmlAttributeInstruction(HtmlNameIndex.Lang, PropertyId.Language, HtmlConverterData.PropertyValueParsingMethods.ParseLanguage)
			}),
			default(HtmlTagInstruction),
			default(HtmlTagInstruction),
			default(HtmlTagInstruction),
			default(HtmlTagInstruction)
		};

		public static CssPropertyInstruction[] cssPropertyInstructions = new CssPropertyInstruction[]
		{
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.Null, null, HtmlConverterData.MultiPropertyParsingMethods.ParseCssWhiteSpace),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.BlockAlignment, null, HtmlConverterData.MultiPropertyParsingMethods.ParseCssVerticalAlignment),
			new CssPropertyInstruction(PropertyId.RightBorderWidth, null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeBorder),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.FontFace, HtmlConverterData.PropertyValueParsingMethods.ParseFontFace, null),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.BottomBorderWidth, null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeBorder),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.BackColor, HtmlConverterData.PropertyValueParsingMethods.ParseColorCss, null),
			new CssPropertyInstruction(PropertyId.BorderStyles, HtmlConverterData.PropertyValueParsingMethods.ParseBorderStyle, null),
			new CssPropertyInstruction(PropertyId.TableShowEmptyCells, HtmlConverterData.PropertyValueParsingMethods.ParseEmptyCells, null),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.TextAlignment, HtmlConverterData.PropertyValueParsingMethods.ParseTextAlignment, null),
			new CssPropertyInstruction(PropertyId.FirstFlag, HtmlConverterData.PropertyValueParsingMethods.ParseFontWeight, null),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.TableCaptionSideTop, HtmlConverterData.PropertyValueParsingMethods.ParseCaptionSide, null),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.LeftMargin, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, null),
			new CssPropertyInstruction(PropertyId.BorderWidths, HtmlConverterData.PropertyValueParsingMethods.ParseBorderWidth, null),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.UnicodeBiDi, HtmlConverterData.PropertyValueParsingMethods.ParseUnicodeBiDi, null),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.Paddings, null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeNonNegativeLength),
			new CssPropertyInstruction(PropertyId.BottomBorderWidth, HtmlConverterData.PropertyValueParsingMethods.ParseBorderWidth, null),
			new CssPropertyInstruction(PropertyId.Visible, HtmlConverterData.PropertyValueParsingMethods.ParseVisibility, null),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.Overloaded1, HtmlConverterData.PropertyValueParsingMethods.ParseTableLayout, null),
			new CssPropertyInstruction(PropertyId.LeftBorderColor, HtmlConverterData.PropertyValueParsingMethods.ParseColorCss, null),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.Height, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, null),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.Margins, null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeNonNegativeLength),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.BottomBorderStyle, HtmlConverterData.PropertyValueParsingMethods.ParseBorderStyle, null),
			new CssPropertyInstruction(PropertyId.BorderColors, null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeColor),
			new CssPropertyInstruction(PropertyId.Null, null, HtmlConverterData.MultiPropertyParsingMethods.ParseCssTextDecoration),
			new CssPropertyInstruction(PropertyId.Display, HtmlConverterData.PropertyValueParsingMethods.ParseDisplay, null),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.BottomMargin, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, null),
			new CssPropertyInstruction(PropertyId.BorderStyles, null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeBorderStyle),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.Null, null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeAllBorders),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.Width, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, null),
			new CssPropertyInstruction(PropertyId.FontColor, HtmlConverterData.PropertyValueParsingMethods.ParseColorCss, null),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.Overloaded2, HtmlConverterData.PropertyValueParsingMethods.ParseBorderCollapse, null),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.TableBorderSpacingVertical, null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompoundBorderSpacing),
			new CssPropertyInstruction(PropertyId.Null, null, HtmlConverterData.MultiPropertyParsingMethods.ParseCssTextTransform),
			new CssPropertyInstruction(PropertyId.RightBorderWidth, HtmlConverterData.PropertyValueParsingMethods.ParseBorderWidth, null),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.FirstLineIndent, HtmlConverterData.PropertyValueParsingMethods.ParseLength, null),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.BottomBorderColor, HtmlConverterData.PropertyValueParsingMethods.ParseColorCss, null),
			new CssPropertyInstruction(PropertyId.RightMargin, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, null),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.RightPadding, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, null),
			new CssPropertyInstruction(PropertyId.RightBorderStyle, HtmlConverterData.PropertyValueParsingMethods.ParseBorderStyle, null),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.BackColor, null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeBackground),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.BorderWidths, null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeBorderWidth),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.BorderColors, HtmlConverterData.PropertyValueParsingMethods.ParseColorCss, null),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.RightToLeft, HtmlConverterData.PropertyValueParsingMethods.ParseDirection, null),
			new CssPropertyInstruction(PropertyId.SmallCaps, HtmlConverterData.PropertyValueParsingMethods.ParseFontVariant, null),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.FontSize, null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeFont),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.RightBorderColor, HtmlConverterData.PropertyValueParsingMethods.ParseColorCss, null),
			new CssPropertyInstruction(PropertyId.Italic, HtmlConverterData.PropertyValueParsingMethods.ParseFontStyle, null),
			new CssPropertyInstruction(PropertyId.Margins, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, null),
			new CssPropertyInstruction(PropertyId.LeftBorderWidth, null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeBorder),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.LeftBorderWidth, HtmlConverterData.PropertyValueParsingMethods.ParseBorderWidth, null),
			new CssPropertyInstruction(PropertyId.BottomPadding, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, null),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.FontSize, HtmlConverterData.PropertyValueParsingMethods.ParseCssFontSize, null),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.LeftBorderStyle, HtmlConverterData.PropertyValueParsingMethods.ParseBorderStyle, null),
			new CssPropertyInstruction(PropertyId.Paddings, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, null),
			new CssPropertyInstruction(PropertyId.LeftPadding, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, null),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			default(CssPropertyInstruction),
			new CssPropertyInstruction(PropertyId.BorderWidths, null, HtmlConverterData.MultiPropertyParsingMethods.ParseCompositeBorder),
			default(CssPropertyInstruction)
		};

		public static class DefaultStyle
		{
			public const int None = 0;

			public const int B = 1;

			public const int Big = 2;

			public const int Del = 3;

			public const int EM = 4;

			public const int I = 4;

			public const int Ins = 5;

			public const int S = 3;

			public const int Small = 6;

			public const int Strike = 3;

			public const int Strong = 1;

			public const int Sub = 7;

			public const int Sup = 8;

			public const int U = 5;

			public const int Var = 4;

			public const int Code = 9;

			public const int Cite = 4;

			public const int Dfn = 4;

			public const int Kbd = 9;

			public const int Samp = 9;

			public const int TT = 10;

			public const int Bdo = 11;

			public const int NoBR = 12;

			public const int Center = 13;

			public const int Xmp = 14;

			public const int Pre = 14;

			public const int Listing = 15;

			public const int PlainText = 14;

			public const int BlockQuote = 16;

			public const int Address = 4;

			public const int H1 = 17;

			public const int H2 = 18;

			public const int H3 = 19;

			public const int H4 = 20;

			public const int H5 = 21;

			public const int H6 = 22;

			public const int OL = 23;

			public const int UL = 24;

			public const int Dir = 24;

			public const int Menu = 24;

			public const int DT = 25;

			public const int DD = 25;

			public const int Caption = 13;
		}

		public static class PropertyValueParsingMethods
		{
			public static readonly PropertyValueParsingMethod ParseAreaShape = new PropertyValueParsingMethod(HtmlSupport.ParseAreaShape);

			public static readonly PropertyValueParsingMethod ParseBlockAlignment = new PropertyValueParsingMethod(HtmlSupport.ParseBlockAlignment);

			public static readonly PropertyValueParsingMethod ParseBooleanAttribute = new PropertyValueParsingMethod(HtmlSupport.ParseBooleanAttribute);

			public static readonly PropertyValueParsingMethod ParseBorderCollapse = new PropertyValueParsingMethod(HtmlSupport.ParseBorderCollapse);

			public static readonly PropertyValueParsingMethod ParseBorderStyle = new PropertyValueParsingMethod(HtmlSupport.ParseBorderStyle);

			public static readonly PropertyValueParsingMethod ParseBorderWidth = new PropertyValueParsingMethod(HtmlSupport.ParseBorderWidth);

			public static readonly PropertyValueParsingMethod ParseCaptionSide = new PropertyValueParsingMethod(HtmlSupport.ParseCaptionSide);

			public static readonly PropertyValueParsingMethod ParseColor = new PropertyValueParsingMethod(HtmlSupport.ParseColor);

			public static readonly PropertyValueParsingMethod ParseColorCss = new PropertyValueParsingMethod(HtmlSupport.ParseColorCss);

			public static readonly PropertyValueParsingMethod ParseCssFontSize = new PropertyValueParsingMethod(HtmlSupport.ParseCssFontSize);

			public static readonly PropertyValueParsingMethod ParseDirection = new PropertyValueParsingMethod(HtmlSupport.ParseDirection);

			public static readonly PropertyValueParsingMethod ParseDisplay = new PropertyValueParsingMethod(HtmlSupport.ParseDisplay);

			public static readonly PropertyValueParsingMethod ParseEmptyCells = new PropertyValueParsingMethod(HtmlSupport.ParseEmptyCells);

			public static readonly PropertyValueParsingMethod ParseFontFace = new PropertyValueParsingMethod(HtmlSupport.ParseFontFace);

			public static readonly PropertyValueParsingMethod ParseFontSize = new PropertyValueParsingMethod(HtmlSupport.ParseFontSize);

			public static readonly PropertyValueParsingMethod ParseFontStyle = new PropertyValueParsingMethod(HtmlSupport.ParseFontStyle);

			public static readonly PropertyValueParsingMethod ParseFontVariant = new PropertyValueParsingMethod(HtmlSupport.ParseFontVariant);

			public static readonly PropertyValueParsingMethod ParseFontWeight = new PropertyValueParsingMethod(HtmlSupport.ParseFontWeight);

			public static readonly PropertyValueParsingMethod ParseHorizontalAlignment = new PropertyValueParsingMethod(HtmlSupport.ParseHorizontalAlignment);

			public static readonly PropertyValueParsingMethod ParseLanguage = new PropertyValueParsingMethod(HtmlSupport.ParseLanguage);

			public static readonly PropertyValueParsingMethod ParseLength = new PropertyValueParsingMethod(HtmlSupport.ParseLength);

			public static readonly PropertyValueParsingMethod ParseNonNegativeInteger = new PropertyValueParsingMethod(HtmlSupport.ParseNonNegativeInteger);

			public static readonly PropertyValueParsingMethod ParseNonNegativeLength = new PropertyValueParsingMethod(HtmlSupport.ParseNonNegativeLength);

			public static readonly PropertyValueParsingMethod ParseStringProperty = new PropertyValueParsingMethod(HtmlSupport.ParseStringProperty);

			public static readonly PropertyValueParsingMethod ParseTableFrame = new PropertyValueParsingMethod(HtmlSupport.ParseTableFrame);

			public static readonly PropertyValueParsingMethod ParseTableLayout = new PropertyValueParsingMethod(HtmlSupport.ParseTableLayout);

			public static readonly PropertyValueParsingMethod ParseTableRules = new PropertyValueParsingMethod(HtmlSupport.ParseTableRules);

			public static readonly PropertyValueParsingMethod ParseTarget = new PropertyValueParsingMethod(HtmlSupport.ParseTarget);

			public static readonly PropertyValueParsingMethod ParseTextAlignment = new PropertyValueParsingMethod(HtmlSupport.ParseTextAlignment);

			public static readonly PropertyValueParsingMethod ParseUnicodeBiDi = new PropertyValueParsingMethod(HtmlSupport.ParseUnicodeBiDi);

			public static readonly PropertyValueParsingMethod ParseUrl = new PropertyValueParsingMethod(HtmlSupport.ParseUrl);

			public static readonly PropertyValueParsingMethod ParseVerticalAlignment = new PropertyValueParsingMethod(HtmlSupport.ParseVerticalAlignment);

			public static readonly PropertyValueParsingMethod ParseVisibility = new PropertyValueParsingMethod(HtmlSupport.ParseVisibility);
		}

		public static class MultiPropertyParsingMethods
		{
			public static readonly MultiPropertyParsingMethod ParseCompositeAllBorders = new MultiPropertyParsingMethod(HtmlSupport.ParseCompositeAllBorders);

			public static readonly MultiPropertyParsingMethod ParseCompositeBackground = new MultiPropertyParsingMethod(HtmlSupport.ParseCompositeBackground);

			public static readonly MultiPropertyParsingMethod ParseCompositeBorder = new MultiPropertyParsingMethod(HtmlSupport.ParseCompositeBorder);

			public static readonly MultiPropertyParsingMethod ParseCompositeBorderStyle = new MultiPropertyParsingMethod(HtmlSupport.ParseCompositeBorderStyle);

			public static readonly MultiPropertyParsingMethod ParseCompositeBorderWidth = new MultiPropertyParsingMethod(HtmlSupport.ParseCompositeBorderWidth);

			public static readonly MultiPropertyParsingMethod ParseCompositeColor = new MultiPropertyParsingMethod(HtmlSupport.ParseCompositeColor);

			public static readonly MultiPropertyParsingMethod ParseCompositeFont = new MultiPropertyParsingMethod(HtmlSupport.ParseCompositeFont);

			public static readonly MultiPropertyParsingMethod ParseCompositeLength = new MultiPropertyParsingMethod(HtmlSupport.ParseCompositeLength);

			public static readonly MultiPropertyParsingMethod ParseCompositeNonNegativeLength = new MultiPropertyParsingMethod(HtmlSupport.ParseCompositeNonNegativeLength);

			public static readonly MultiPropertyParsingMethod ParseCompoundBorderSpacing = new MultiPropertyParsingMethod(HtmlSupport.ParseCompoundBorderSpacing);

			public static readonly MultiPropertyParsingMethod ParseCssTextDecoration = new MultiPropertyParsingMethod(HtmlSupport.ParseCssTextDecoration);

			public static readonly MultiPropertyParsingMethod ParseCssTextTransform = new MultiPropertyParsingMethod(HtmlSupport.ParseCssTextTransform);

			public static readonly MultiPropertyParsingMethod ParseCssVerticalAlignment = new MultiPropertyParsingMethod(HtmlSupport.ParseCssVerticalAlignment);

			public static readonly MultiPropertyParsingMethod ParseCssWhiteSpace = new MultiPropertyParsingMethod(HtmlSupport.ParseCssWhiteSpace);
		}
	}
}
