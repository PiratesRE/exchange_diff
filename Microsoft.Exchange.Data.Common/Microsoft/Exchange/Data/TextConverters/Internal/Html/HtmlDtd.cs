using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal static class HtmlDtd
	{
		public static bool IsTagInSet(HtmlTagIndex tag, HtmlDtd.SetId set)
		{
			return 0 != (HtmlDtd.sets[(int)(set + (short)(tag >> 3))] & (byte)(1 << (int)(tag & HtmlTagIndex.Unknown)));
		}

		public static HtmlDtd.TagDefinition[] tags = new HtmlDtd.TagDefinition[]
		{
			new HtmlDtd.TagDefinition(HtmlTagIndex._NULL, HtmlNameIndex._NOTANAME, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex._ROOT, HtmlNameIndex._NOTANAME, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, HtmlDtd.SetId.Empty, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.EXCLUDE, HtmlTagIndex.Body, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.Root, HtmlDtd.ContextTextType.Full, HtmlDtd.SetId.Null, HtmlDtd.SetId.Empty, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex._COMMENT, HtmlNameIndex._COMMENT, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.NUL_NUL_NUL_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex._CONDITIONAL, HtmlNameIndex._CONDITIONAL, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.NUL_NUL_NUL_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex._BANG, HtmlNameIndex._BANG, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.NUL_NUL_NUL_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex._DTD, HtmlNameIndex._DTD, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.NUL_NUL_NUL_NUL, HtmlDtd.TagFmt.BRK_BRK_BRK_BRK, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex._ASP, HtmlNameIndex._ASP, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.NUL_NUL_NUL_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Unknown, HtmlNameIndex.Unknown, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, HtmlDtd.SetId.Empty, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.A, HtmlNameIndex.A, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, (HtmlDtd.SetId)48, (HtmlDtd.SetId)64, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Abbr, HtmlNameIndex.Abbr, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Acronym, HtmlNameIndex.Acronym, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Address, HtmlNameIndex.Address, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)96, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Applet, HtmlNameIndex.Applet, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_PUT_PUT_PUT, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)144, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, (HtmlDtd.SetId)160, (HtmlDtd.SetId)176, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Area, HtmlNameIndex.Area, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.QUERY, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.B, HtmlNameIndex.B, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Base, HtmlNameIndex.Base, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.BaseFont, HtmlNameIndex.BaseFont, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, (HtmlDtd.SetId)192, (HtmlDtd.SetId)176, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Bdo, HtmlNameIndex.Bdo, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.BGSound, HtmlNameIndex.BGSound, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Big, HtmlNameIndex.Big, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Blink, HtmlNameIndex.Blink, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.BlockQuote, HtmlNameIndex.BlockQuote, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)96, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Body, HtmlNameIndex.Body, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_BRK_BRK_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)144, (HtmlDtd.SetId)144, (HtmlDtd.SetId)208, (HtmlDtd.SetId)224, (HtmlDtd.SetId)144, HtmlTagIndex.Html, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.Body, HtmlDtd.ContextTextType.Full, HtmlDtd.SetId.Null, (HtmlDtd.SetId)240, (HtmlDtd.SetId)256),
			new HtmlDtd.TagDefinition(HtmlTagIndex.BR, HtmlNameIndex.BR, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.NBR_BRK_NBR_BRK, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.ALWAYS, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Body, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Button, HtmlNameIndex.Button, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.EAT_EAT_EAT_PUT, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.ALWAYS, (HtmlDtd.SetId)96, (HtmlDtd.SetId)96, HtmlDtd.SetId.Null, (HtmlDtd.SetId)48, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Caption, HtmlNameIndex.Caption, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)272, (HtmlDtd.SetId)272, HtmlDtd.SetId.Null, (HtmlDtd.SetId)288, (HtmlDtd.SetId)272, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Center, HtmlNameIndex.Center, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)32, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Cite, HtmlNameIndex.Cite, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Code, HtmlNameIndex.Code, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Col, HtmlNameIndex.Col, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)304, HtmlDtd.SetId.Null, (HtmlDtd.SetId)320, (HtmlDtd.SetId)304, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.ColGroup, HtmlNameIndex.ColGroup, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)272, (HtmlDtd.SetId)272, HtmlDtd.SetId.Null, (HtmlDtd.SetId)288, (HtmlDtd.SetId)272, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.EXCLUDE, HtmlTagIndex.TC, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Comment, HtmlNameIndex.Comment, HtmlDtd.Literal.Tags | HtmlDtd.Literal.Entities, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.Comment, HtmlDtd.ContextTextType.Literal, HtmlDtd.SetId.Null, HtmlDtd.SetId.Empty, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.DD, HtmlNameIndex.DD, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.ALWAYS, (HtmlDtd.SetId)96, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)336, (HtmlDtd.SetId)112, HtmlTagIndex.Body, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Del, HtmlNameIndex.Del, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Dfn, HtmlNameIndex.Dfn, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Dir, HtmlNameIndex.Dir, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_BRK_NBR_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)96, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Div, HtmlNameIndex.Div, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)96, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.DL, HtmlNameIndex.DL, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)96, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.DT, HtmlNameIndex.DT, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.ALWAYS, (HtmlDtd.SetId)96, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)336, (HtmlDtd.SetId)112, HtmlTagIndex.Body, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.EM, HtmlNameIndex.EM, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Embed, HtmlNameIndex.Embed, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_PUT_PUT_PUT, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.ALWAYS, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Body, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.FieldSet, HtmlNameIndex.FieldSet, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_PUT, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)96, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Font, HtmlNameIndex.Font, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Form, HtmlNameIndex.Form, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, (HtmlDtd.SetId)352, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Frame, HtmlNameIndex.Frame, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, HtmlDtd.SetId.Empty, HtmlDtd.SetId.Null, (HtmlDtd.SetId)352, (HtmlDtd.SetId)368, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.Frame, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.FrameSet, HtmlNameIndex.FrameSet, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)144, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, (HtmlDtd.SetId)384, (HtmlDtd.SetId)144, HtmlTagIndex.Html, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.Frameset, HtmlDtd.ContextTextType.None, (HtmlDtd.SetId)400, HtmlDtd.SetId.Null, (HtmlDtd.SetId)144),
			new HtmlDtd.TagDefinition(HtmlTagIndex.H1, HtmlNameIndex.H1, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)32, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, (HtmlDtd.SetId)416, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.H2, HtmlNameIndex.H2, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)32, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, (HtmlDtd.SetId)416, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.H3, HtmlNameIndex.H3, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)32, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, (HtmlDtd.SetId)416, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.H4, HtmlNameIndex.H4, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)32, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, (HtmlDtd.SetId)416, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.H5, HtmlNameIndex.H5, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)32, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, (HtmlDtd.SetId)416, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.H6, HtmlNameIndex.H6, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)32, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, (HtmlDtd.SetId)416, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Head, HtmlNameIndex.Head, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.BRK_BRK_BRK_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)144, (HtmlDtd.SetId)144, (HtmlDtd.SetId)176, HtmlDtd.SetId.Null, (HtmlDtd.SetId)144, HtmlTagIndex.Html, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.Head, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)432),
			new HtmlDtd.TagDefinition(HtmlTagIndex.HR, HtmlNameIndex.HR, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.BRK_BRK_BRK_BRK, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.ALWAYS, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Html, HtmlNameIndex.Html, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.BRK_BRK_BRK_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)144, (HtmlDtd.SetId)144, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.EXCLUDE, HtmlTagIndex.Body, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.I, HtmlNameIndex.I, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Iframe, HtmlNameIndex.Iframe, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.BRK_BRK_BRK_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.ALWAYS, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, (HtmlDtd.SetId)448, (HtmlDtd.SetId)208, HtmlTagIndex.Body, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.IFrame, HtmlDtd.ContextTextType.Full, (HtmlDtd.SetId)448, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Image, HtmlNameIndex.Image, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.ALWAYS, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Body, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Img, HtmlNameIndex.Img, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_PUT_PUT_PUT, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.ALWAYS, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Body, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Input, HtmlNameIndex.Input, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_PUT_PUT_PUT, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.QUERY, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, (HtmlDtd.SetId)48, (HtmlDtd.SetId)464, (HtmlDtd.SetId)176, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Ins, HtmlNameIndex.Ins, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.IsIndex, HtmlNameIndex.IsIndex, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.ALWAYS, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Body, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Kbd, HtmlNameIndex.Kbd, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Label, HtmlNameIndex.Label, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Legend, HtmlNameIndex.Legend, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.ALWAYS, (HtmlDtd.SetId)96, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)480, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.LI, HtmlNameIndex.LI, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_NBR, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.ALWAYS, (HtmlDtd.SetId)96, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)496, (HtmlDtd.SetId)112, HtmlTagIndex.Body, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Link, HtmlNameIndex.Link, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Listing, HtmlNameIndex.Listing, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_PUT_PUT_EAT, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.ALWAYS, (HtmlDtd.SetId)96, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.Pre, HtmlDtd.ContextTextType.Literal, HtmlDtd.SetId.Null, (HtmlDtd.SetId)240, (HtmlDtd.SetId)256),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Map, HtmlNameIndex.Map, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Marquee, HtmlNameIndex.Marquee, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_EAT_EAT, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.ALWAYS, (HtmlDtd.SetId)144, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)208, HtmlTagIndex.Body, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Menu, HtmlNameIndex.Menu, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_BRK_NBR_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)96, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Meta, HtmlNameIndex.Meta, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.NUL_NUL_NUL_NUL, HtmlDtd.TagFmt.BRK_BRK_BRK_BRK, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.NextId, HtmlNameIndex.NextId, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.NoBR, HtmlNameIndex.NoBR, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)32, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, (HtmlDtd.SetId)512, (HtmlDtd.SetId)208, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.NoEmbed, HtmlNameIndex.NoEmbed, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, (HtmlDtd.SetId)528, (HtmlDtd.SetId)544, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.NoFrames, HtmlNameIndex.NoFrames, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, (HtmlDtd.SetId)528, (HtmlDtd.SetId)544, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.NoScript, HtmlNameIndex.NoScript, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, (HtmlDtd.SetId)528, (HtmlDtd.SetId)544, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Object, HtmlNameIndex.Object, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.EAT_EAT_EAT_EAT, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)144, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, (HtmlDtd.SetId)160, (HtmlDtd.SetId)176, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.OL, HtmlNameIndex.OL, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_BRK_NBR_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)96, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.OptGroup, HtmlNameIndex.OptGroup, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)464, (HtmlDtd.SetId)464, HtmlDtd.SetId.Null, (HtmlDtd.SetId)560, (HtmlDtd.SetId)464, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Option, HtmlNameIndex.Option, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_EAT_EAT_PUT, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)464, (HtmlDtd.SetId)464, HtmlDtd.SetId.Null, (HtmlDtd.SetId)576, (HtmlDtd.SetId)464, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.P, HtmlNameIndex.P, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)96, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Param, HtmlNameIndex.Param, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)160, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)160, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.EXCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.PlainText, HtmlNameIndex.PlainText, HtmlDtd.Literal.Tags | HtmlDtd.Literal.Entities, true, HtmlDtd.TagFill.PUT_PUT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.ALWAYS, (HtmlDtd.SetId)96, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.Pre, HtmlDtd.ContextTextType.Literal, HtmlDtd.SetId.Null, (HtmlDtd.SetId)240, (HtmlDtd.SetId)256),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Pre, HtmlNameIndex.Pre, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_PUT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.ALWAYS, (HtmlDtd.SetId)96, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.Pre, HtmlDtd.ContextTextType.Literal, HtmlDtd.SetId.Null, (HtmlDtd.SetId)240, (HtmlDtd.SetId)256),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Q, HtmlNameIndex.Q, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.RP, HtmlNameIndex.RP, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)592, (HtmlDtd.SetId)608, HtmlDtd.SetId.Null, (HtmlDtd.SetId)624, (HtmlDtd.SetId)640, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.RT, HtmlNameIndex.RT, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)608, (HtmlDtd.SetId)608, HtmlDtd.SetId.Null, (HtmlDtd.SetId)656, (HtmlDtd.SetId)640, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Ruby, HtmlNameIndex.Ruby, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, (HtmlDtd.SetId)672, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.S, HtmlNameIndex.S, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Samp, HtmlNameIndex.Samp, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Script, HtmlNameIndex.Script, HtmlDtd.Literal.Tags | HtmlDtd.Literal.Entities, false, HtmlDtd.TagFill.NUL_NUL_NUL_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.Script, HtmlDtd.ContextTextType.Literal, HtmlDtd.SetId.Null, HtmlDtd.SetId.Empty, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Select, HtmlNameIndex.Select, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_PUT_PUT_PUT, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.ALWAYS, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, (HtmlDtd.SetId)48, (HtmlDtd.SetId)464, (HtmlDtd.SetId)208, HtmlTagIndex.Body, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.Select, HtmlDtd.ContextTextType.Full, (HtmlDtd.SetId)688, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Small, HtmlNameIndex.Small, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Span, HtmlNameIndex.Span, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Data, HtmlNameIndex.Data, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Meter, HtmlNameIndex.Meter, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Strike, HtmlNameIndex.Strike, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Strong, HtmlNameIndex.Strong, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Style, HtmlNameIndex.Style, HtmlDtd.Literal.Tags | HtmlDtd.Literal.Entities, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.Style, HtmlDtd.ContextTextType.Literal, HtmlDtd.SetId.Null, HtmlDtd.SetId.Empty, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Sub, HtmlNameIndex.Sub, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Sup, HtmlNameIndex.Sup, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Table, HtmlNameIndex.Table, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_BRK_BRK_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.ALWAYS, (HtmlDtd.SetId)272, (HtmlDtd.SetId)704, HtmlDtd.SetId.Null, (HtmlDtd.SetId)720, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.EXCLUDE, HtmlTagIndex.TC, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Tbody, HtmlNameIndex.Tbody, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_BRK_BRK_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)272, (HtmlDtd.SetId)272, HtmlDtd.SetId.Null, (HtmlDtd.SetId)288, (HtmlDtd.SetId)272, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.EXCLUDE, HtmlTagIndex.TC, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.TC, HtmlNameIndex.Unknown, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)272, (HtmlDtd.SetId)272, HtmlDtd.SetId.Null, (HtmlDtd.SetId)736, (HtmlDtd.SetId)272, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex.TC, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.TD, HtmlNameIndex.TD, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)752, (HtmlDtd.SetId)272, HtmlDtd.SetId.Null, (HtmlDtd.SetId)768, (HtmlDtd.SetId)784, HtmlTagIndex.TR, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.TableCell, HtmlDtd.ContextTextType.Full, HtmlDtd.SetId.Null, (HtmlDtd.SetId)240, (HtmlDtd.SetId)256),
			new HtmlDtd.TagDefinition(HtmlTagIndex.TextArea, HtmlNameIndex.TextArea, HtmlDtd.Literal.Tags, false, HtmlDtd.TagFill.PUT_PUT_PUT_PUT, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.ALWAYS, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, (HtmlDtd.SetId)48, (HtmlDtd.SetId)464, (HtmlDtd.SetId)176, HtmlTagIndex.Body, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Tfoot, HtmlNameIndex.Tfoot, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_BRK_BRK_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)272, (HtmlDtd.SetId)272, HtmlDtd.SetId.Null, (HtmlDtd.SetId)288, (HtmlDtd.SetId)272, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.EXCLUDE, HtmlTagIndex.TC, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.TH, HtmlNameIndex.TH, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)752, (HtmlDtd.SetId)272, HtmlDtd.SetId.Null, (HtmlDtd.SetId)768, (HtmlDtd.SetId)784, HtmlTagIndex.TR, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.TableCell, HtmlDtd.ContextTextType.Full, HtmlDtd.SetId.Null, (HtmlDtd.SetId)240, (HtmlDtd.SetId)256),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Thead, HtmlNameIndex.Thead, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_BRK_BRK_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)272, (HtmlDtd.SetId)272, HtmlDtd.SetId.Null, (HtmlDtd.SetId)288, (HtmlDtd.SetId)272, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.EXCLUDE, HtmlTagIndex.TC, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Title, HtmlNameIndex.Title, HtmlDtd.Literal.Tags, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.Title, HtmlDtd.ContextTextType.Full, HtmlDtd.SetId.Null, HtmlDtd.SetId.Empty, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.TR, HtmlNameIndex.TR, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_BRK_BRK_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.NEVER, (HtmlDtd.SetId)800, (HtmlDtd.SetId)272, HtmlDtd.SetId.Null, (HtmlDtd.SetId)816, (HtmlDtd.SetId)832, HtmlTagIndex.Tbody, false, HtmlDtd.TagTextScope.EXCLUDE, HtmlTagIndex.TC, HtmlDtd.SetId.Null, HtmlTagIndex.TD, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.TT, HtmlNameIndex.TT, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.U, HtmlNameIndex.U, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.UL, HtmlNameIndex.UL, HtmlDtd.Literal.None, true, HtmlDtd.TagFill.PUT_EAT_PUT_EAT, HtmlDtd.TagFmt.BRK_BRK_NBR_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)96, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Var, HtmlNameIndex.Var, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.OVERLAP, HtmlDtd.TagTextType.QUERY, (HtmlDtd.SetId)32, (HtmlDtd.SetId)32, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)80, HtmlTagIndex.Body, true, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Wbr, HtmlNameIndex.Wbr, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.ALWAYS, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Body, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Xmp, HtmlNameIndex.Xmp, HtmlDtd.Literal.Tags | HtmlDtd.Literal.Entities, false, HtmlDtd.TagFill.PUT_PUT_PUT_EAT, HtmlDtd.TagFmt.BRK_NBR_NBR_BRK, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.ALWAYS, (HtmlDtd.SetId)96, (HtmlDtd.SetId)112, HtmlDtd.SetId.Null, (HtmlDtd.SetId)128, (HtmlDtd.SetId)112, HtmlTagIndex.Body, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.Pre, HtmlDtd.ContextTextType.Literal, HtmlDtd.SetId.Null, (HtmlDtd.SetId)240, (HtmlDtd.SetId)256),
			new HtmlDtd.TagDefinition(HtmlTagIndex.Xml, HtmlNameIndex.Xml, HtmlDtd.Literal.Tags | HtmlDtd.Literal.Entities, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.NESTED, HtmlDtd.TagTextType.QUERY, HtmlDtd.SetId.Empty, (HtmlDtd.SetId)144, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, (HtmlDtd.SetId)176, HtmlTagIndex.Head, false, HtmlDtd.TagTextScope.INCLUDE, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex._Pxml, HtmlNameIndex._Pxml, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, HtmlDtd.SetId.Empty, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex._Import, HtmlNameIndex._Import, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, HtmlDtd.SetId.Empty, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null),
			new HtmlDtd.TagDefinition(HtmlTagIndex._Xml_Namespace, HtmlNameIndex._Xml_Namespace, HtmlDtd.Literal.None, false, HtmlDtd.TagFill.PUT_NUL_PUT_NUL, HtmlDtd.TagFmt.AUT_AUT_AUT_AUT, HtmlDtd.TagScope.EMPTY, HtmlDtd.TagTextType.NEVER, HtmlDtd.SetId.Empty, HtmlDtd.SetId.Empty, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, false, HtmlDtd.TagTextScope.NEUTRAL, HtmlTagIndex._NULL, HtmlDtd.SetId.Null, HtmlTagIndex._NULL, HtmlDtd.ContextType.None, HtmlDtd.ContextTextType.None, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null, HtmlDtd.SetId.Null)
		};

		public static byte[] sets = new byte[]
		{
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			3,
			0,
			0,
			64,
			0,
			32,
			0,
			0,
			0,
			0,
			18,
			0,
			0,
			0,
			0,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			64,
			3,
			0,
			0,
			0,
			0,
			32,
			0,
			0,
			0,
			0,
			18,
			0,
			0,
			0,
			0,
			64,
			3,
			0,
			0,
			16,
			0,
			32,
			0,
			0,
			0,
			192,
			187,
			0,
			0,
			0,
			0,
			96,
			3,
			40,
			0,
			0,
			0,
			104,
			64,
			16,
			0,
			192,
			187,
			4,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			2,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			64,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			16,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			32,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			64,
			0,
			0,
			0,
			16,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			64,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			16,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			48,
			80,
			0,
			0,
			128,
			5,
			0,
			0,
			0,
			0,
			0,
			0,
			128,
			64,
			0,
			65,
			0,
			80,
			0,
			2,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			64,
			0,
			0,
			0,
			0,
			0,
			0,
			66,
			0,
			0,
			0,
			0,
			0,
			128,
			1,
			16,
			128,
			41,
			0,
			0,
			0,
			0,
			0,
			64,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			64,
			0,
			0,
			0,
			0,
			0,
			0,
			2,
			0,
			0,
			0,
			0,
			0,
			128,
			1,
			16,
			128,
			41,
			0,
			0,
			0,
			0,
			0,
			0,
			65,
			0,
			0,
			0,
			0,
			0,
			2,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			8,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			32,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			8,
			16,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			128,
			0,
			0,
			0,
			0,
			49,
			0,
			0,
			0,
			8,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			192,
			15,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			64,
			0,
			0,
			0,
			80,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			16,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			2,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			2,
			0,
			2,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			2,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			28,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			64,
			0,
			0,
			32,
			16,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			128,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			3,
			0,
			0,
			64,
			0,
			32,
			0,
			128,
			1,
			0,
			18,
			0,
			0,
			0,
			0,
			0,
			3,
			0,
			0,
			64,
			0,
			32,
			0,
			0,
			1,
			0,
			18,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			64,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			192,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			128,
			1,
			0,
			0,
			0,
			0,
			128,
			0,
			0,
			0,
			0,
			0,
			0,
			8,
			0,
			128,
			1,
			8,
			192,
			191,
			0,
			0,
			0,
			0,
			0,
			2,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			64,
			19,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			128,
			1,
			16,
			64,
			1,
			0,
			0,
			0,
			0,
			0,
			66,
			0,
			0,
			0,
			0,
			0,
			128,
			1,
			16,
			0,
			19,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			64,
			128,
			0,
			0,
			0,
			1,
			0,
			70,
			0,
			200,
			15,
			0,
			16,
			128,
			1,
			16,
			0,
			19,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			128,
			0,
			0,
			0,
			0,
			0,
			2,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			192,
			58,
			0,
			0,
			0,
			1,
			0,
			70,
			0,
			200,
			15,
			0,
			16,
			128,
			1,
			16,
			0,
			129,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			128,
			40,
			0,
			0
		};

		internal enum TagScope : byte
		{
			EMPTY,
			OVERLAP,
			NESTED
		}

		internal enum TagTextType : byte
		{
			NEVER,
			ALWAYS,
			QUERY
		}

		internal enum TagTextScope : byte
		{
			NEUTRAL,
			EXCLUDE,
			INCLUDE
		}

		internal enum ContextType : byte
		{
			None,
			Root,
			Head,
			Title,
			Body,
			TableCell,
			Select,
			Pre,
			Frameset,
			Frame,
			IFrame,
			Object,
			Script,
			Style,
			Comment,
			NoShow
		}

		internal enum ContextTextType : byte
		{
			None,
			Discard = 0,
			Literal,
			Full
		}

		[Flags]
		internal enum Literal : byte
		{
			None = 0,
			Tags = 1,
			Entities = 2
		}

		internal enum FillCode : byte
		{
			NUL,
			PUT,
			EAT
		}

		internal struct TagFill
		{
			public HtmlDtd.FillCode LB
			{
				get
				{
					return (HtmlDtd.FillCode)(this.value >> 6);
				}
			}

			public HtmlDtd.FillCode RB
			{
				get
				{
					return (HtmlDtd.FillCode)(this.value >> 4 & 3);
				}
			}

			public HtmlDtd.FillCode LE
			{
				get
				{
					return (HtmlDtd.FillCode)(this.value >> 2 & 3);
				}
			}

			public HtmlDtd.FillCode RE
			{
				get
				{
					return (HtmlDtd.FillCode)(this.value & 3);
				}
			}

			private TagFill(HtmlDtd.FillCode lB, HtmlDtd.FillCode rB, HtmlDtd.FillCode lE, HtmlDtd.FillCode rE)
			{
				this.value = (byte)(lB << 6 | rB << 4 | lE << 2 | rE);
			}

			internal readonly byte value;

			public static readonly HtmlDtd.TagFill PUT_NUL_PUT_NUL = new HtmlDtd.TagFill(HtmlDtd.FillCode.PUT, HtmlDtd.FillCode.NUL, HtmlDtd.FillCode.PUT, HtmlDtd.FillCode.NUL);

			public static readonly HtmlDtd.TagFill NUL_NUL_NUL_NUL = new HtmlDtd.TagFill(HtmlDtd.FillCode.NUL, HtmlDtd.FillCode.NUL, HtmlDtd.FillCode.NUL, HtmlDtd.FillCode.NUL);

			public static readonly HtmlDtd.TagFill NUL_EAT_EAT_NUL = new HtmlDtd.TagFill(HtmlDtd.FillCode.NUL, HtmlDtd.FillCode.EAT, HtmlDtd.FillCode.EAT, HtmlDtd.FillCode.NUL);

			public static readonly HtmlDtd.TagFill PUT_EAT_PUT_EAT = new HtmlDtd.TagFill(HtmlDtd.FillCode.PUT, HtmlDtd.FillCode.EAT, HtmlDtd.FillCode.PUT, HtmlDtd.FillCode.EAT);

			public static readonly HtmlDtd.TagFill PUT_PUT_PUT_PUT = new HtmlDtd.TagFill(HtmlDtd.FillCode.PUT, HtmlDtd.FillCode.PUT, HtmlDtd.FillCode.PUT, HtmlDtd.FillCode.PUT);

			public static readonly HtmlDtd.TagFill EAT_EAT_EAT_PUT = new HtmlDtd.TagFill(HtmlDtd.FillCode.EAT, HtmlDtd.FillCode.EAT, HtmlDtd.FillCode.EAT, HtmlDtd.FillCode.PUT);

			public static readonly HtmlDtd.TagFill PUT_PUT_PUT_EAT = new HtmlDtd.TagFill(HtmlDtd.FillCode.PUT, HtmlDtd.FillCode.PUT, HtmlDtd.FillCode.PUT, HtmlDtd.FillCode.EAT);

			public static readonly HtmlDtd.TagFill PUT_EAT_PUT_PUT = new HtmlDtd.TagFill(HtmlDtd.FillCode.PUT, HtmlDtd.FillCode.EAT, HtmlDtd.FillCode.PUT, HtmlDtd.FillCode.PUT);

			public static readonly HtmlDtd.TagFill PUT_EAT_EAT_EAT = new HtmlDtd.TagFill(HtmlDtd.FillCode.PUT, HtmlDtd.FillCode.EAT, HtmlDtd.FillCode.EAT, HtmlDtd.FillCode.EAT);

			public static readonly HtmlDtd.TagFill EAT_EAT_EAT_EAT = new HtmlDtd.TagFill(HtmlDtd.FillCode.EAT, HtmlDtd.FillCode.EAT, HtmlDtd.FillCode.EAT, HtmlDtd.FillCode.EAT);

			public static readonly HtmlDtd.TagFill PUT_EAT_EAT_PUT = new HtmlDtd.TagFill(HtmlDtd.FillCode.PUT, HtmlDtd.FillCode.EAT, HtmlDtd.FillCode.EAT, HtmlDtd.FillCode.PUT);
		}

		internal enum FmtCode : byte
		{
			AUT,
			BRK,
			NBR
		}

		internal struct TagFmt
		{
			public HtmlDtd.FmtCode LB
			{
				get
				{
					return (HtmlDtd.FmtCode)(this.value >> 6);
				}
			}

			public HtmlDtd.FmtCode RB
			{
				get
				{
					return (HtmlDtd.FmtCode)(this.value >> 4 & 3);
				}
			}

			public HtmlDtd.FmtCode LE
			{
				get
				{
					return (HtmlDtd.FmtCode)(this.value >> 2 & 3);
				}
			}

			public HtmlDtd.FmtCode RE
			{
				get
				{
					return (HtmlDtd.FmtCode)(this.value & 3);
				}
			}

			private TagFmt(HtmlDtd.FmtCode lB, HtmlDtd.FmtCode rB, HtmlDtd.FmtCode lE, HtmlDtd.FmtCode rE)
			{
				this.value = (byte)(lB << 6 | rB << 4 | lE << 2 | rE);
			}

			internal readonly byte value;

			public static readonly HtmlDtd.TagFmt BRK_BRK_BRK_BRK = new HtmlDtd.TagFmt(HtmlDtd.FmtCode.BRK, HtmlDtd.FmtCode.BRK, HtmlDtd.FmtCode.BRK, HtmlDtd.FmtCode.BRK);

			public static readonly HtmlDtd.TagFmt AUT_AUT_AUT_AUT = new HtmlDtd.TagFmt(HtmlDtd.FmtCode.AUT, HtmlDtd.FmtCode.AUT, HtmlDtd.FmtCode.AUT, HtmlDtd.FmtCode.AUT);

			public static readonly HtmlDtd.TagFmt NBR_BRK_NBR_BRK = new HtmlDtd.TagFmt(HtmlDtd.FmtCode.NBR, HtmlDtd.FmtCode.BRK, HtmlDtd.FmtCode.NBR, HtmlDtd.FmtCode.BRK);

			public static readonly HtmlDtd.TagFmt BRK_NBR_NBR_BRK = new HtmlDtd.TagFmt(HtmlDtd.FmtCode.BRK, HtmlDtd.FmtCode.NBR, HtmlDtd.FmtCode.NBR, HtmlDtd.FmtCode.BRK);

			public static readonly HtmlDtd.TagFmt BRK_BRK_NBR_BRK = new HtmlDtd.TagFmt(HtmlDtd.FmtCode.BRK, HtmlDtd.FmtCode.BRK, HtmlDtd.FmtCode.NBR, HtmlDtd.FmtCode.BRK);

			public static readonly HtmlDtd.TagFmt BRK_NBR_NBR_NBR = new HtmlDtd.TagFmt(HtmlDtd.FmtCode.BRK, HtmlDtd.FmtCode.NBR, HtmlDtd.FmtCode.NBR, HtmlDtd.FmtCode.NBR);
		}

		public enum SetId : short
		{
			Null,
			Empty
		}

		public class TagDefinition
		{
			public TagDefinition(HtmlTagIndex tagIndex, HtmlNameIndex nameIndex, HtmlDtd.Literal literal, bool blockElement, HtmlDtd.TagFill fill, HtmlDtd.TagFmt fmt, HtmlDtd.TagScope scope, HtmlDtd.TagTextType textType, HtmlDtd.SetId endContainers, HtmlDtd.SetId beginContainers, HtmlDtd.SetId maskingContainers, HtmlDtd.SetId prohibitedContainers, HtmlDtd.SetId requiredContainers, HtmlTagIndex defaultContainer, bool queueForRequired, HtmlDtd.TagTextScope textScope, HtmlTagIndex textSubcontainer, HtmlDtd.SetId match, HtmlTagIndex unmatchedSubstitute, HtmlDtd.ContextType contextType, HtmlDtd.ContextTextType contextTextType, HtmlDtd.SetId accept, HtmlDtd.SetId reject, HtmlDtd.SetId ignoreEnd)
			{
				this.TagIndex = tagIndex;
				this.NameIndex = nameIndex;
				this.Literal = literal;
				this.BlockElement = blockElement;
				this.Fill = fill;
				this.Fmt = fmt;
				this.Scope = scope;
				this.TextType = textType;
				this.EndContainers = endContainers;
				this.BeginContainers = beginContainers;
				this.MaskingContainers = maskingContainers;
				this.ProhibitedContainers = prohibitedContainers;
				this.RequiredContainers = requiredContainers;
				this.DefaultContainer = defaultContainer;
				this.QueueForRequired = queueForRequired;
				this.TextScope = textScope;
				this.TextSubcontainer = textSubcontainer;
				this.Match = match;
				this.UnmatchedSubstitute = unmatchedSubstitute;
				this.ContextType = contextType;
				this.ContextTextType = contextTextType;
				this.Accept = accept;
				this.Reject = reject;
				this.IgnoreEnd = ignoreEnd;
			}

			public HtmlNameIndex NameIndex;

			public HtmlTagIndex TagIndex;

			public HtmlDtd.Literal Literal;

			public bool BlockElement;

			public HtmlDtd.TagFill Fill;

			public HtmlDtd.TagFmt Fmt;

			public HtmlDtd.TagScope Scope;

			public HtmlDtd.TagTextType TextType;

			public HtmlDtd.SetId EndContainers;

			public HtmlDtd.SetId BeginContainers;

			public HtmlDtd.SetId MaskingContainers;

			public HtmlDtd.SetId ProhibitedContainers;

			public HtmlDtd.SetId RequiredContainers;

			public HtmlTagIndex DefaultContainer;

			public bool QueueForRequired;

			public HtmlDtd.TagTextScope TextScope;

			public HtmlTagIndex TextSubcontainer;

			public HtmlDtd.SetId Match;

			public HtmlTagIndex UnmatchedSubstitute;

			public HtmlDtd.ContextType ContextType;

			public HtmlDtd.ContextTextType ContextTextType;

			public HtmlDtd.SetId Accept;

			public HtmlDtd.SetId Reject;

			public HtmlDtd.SetId IgnoreEnd;
		}
	}
}
