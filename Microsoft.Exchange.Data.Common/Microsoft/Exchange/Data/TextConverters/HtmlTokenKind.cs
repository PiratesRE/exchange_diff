using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	public enum HtmlTokenKind
	{
		Text,
		StartTag,
		EndTag,
		EmptyElementTag,
		SpecialTag,
		OverlappedClose,
		OverlappedReopen
	}
}
