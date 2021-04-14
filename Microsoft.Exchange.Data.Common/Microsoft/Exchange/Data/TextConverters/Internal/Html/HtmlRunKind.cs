using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal enum HtmlRunKind : uint
	{
		Invalid,
		Text = 67108864U,
		TagPrefix = 134217728U,
		TagSuffix = 201326592U,
		Name = 268435456U,
		NamePrefixDelimiter = 285212672U,
		TagWhitespace = 335544320U,
		AttrEqual = 402653184U,
		AttrQuote = 469762048U,
		AttrValue = 536870912U,
		TagText = 603979776U
	}
}
