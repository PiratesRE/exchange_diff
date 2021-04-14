﻿using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	[Flags]
	internal enum CharClass : uint
	{
		Invalid = 0U,
		NotInterestingText = 1U,
		Control = 2U,
		Whitespace = 4U,
		Alpha = 8U,
		Numeric = 16U,
		Backslash = 32U,
		LessThan = 64U,
		Equals = 128U,
		GreaterThan = 256U,
		Solidus = 512U,
		Ampersand = 1024U,
		Nbsp = 2048U,
		Comma = 4096U,
		SingleQuote = 8192U,
		DoubleQuote = 16384U,
		GraveAccent = 32768U,
		Circumflex = 65536U,
		VerticalLine = 131072U,
		Parentheses = 262144U,
		CurlyBrackets = 524288U,
		SquareBrackets = 1048576U,
		Tilde = 2097152U,
		Colon = 4194304U,
		UniqueMask = 16777215U,
		AlphaHex = 2147483648U,
		HtmlSuffix = 1073741824U,
		RtfInteresting = 536870912U,
		OverlappedMask = 4278190080U,
		Quote = 57344U,
		Brackets = 1572864U,
		NonWhitespaceText = 16775163U,
		NonWhitespaceNonControlText = 16775161U,
		HtmlNonWhitespaceText = 16774075U,
		NonWhitespaceNonUri = 3917120U,
		NonWhitespaceUri = 12858043U,
		HtmlTagName = 16776443U,
		HtmlTagNamePrefix = 12582139U,
		HtmlAttrName = 16776315U,
		HtmlAttrNamePrefix = 12582011U,
		HtmlAttrValue = 16718587U,
		HtmlScanQuoteSensitive = 132U,
		HtmlEntity = 24U,
		HtmlSimpleTagName = 12524731U,
		HtmlEndTagName = 772U,
		HtmlSimpleAttrName = 12524667U,
		HtmlEndAttrName = 900U,
		HtmlSimpleAttrQuotedValue = 16718847U,
		HtmlSimpleAttrUnquotedValue = 16718587U,
		HtmlEndAttrUnquotedValue = 260U,
		Hex = 2147483664U
	}
}