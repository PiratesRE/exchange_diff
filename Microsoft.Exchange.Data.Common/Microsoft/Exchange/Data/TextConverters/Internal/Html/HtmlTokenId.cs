using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal enum HtmlTokenId : byte
	{
		None,
		EndOfFile,
		Text,
		EncodingChange,
		Tag,
		Restart,
		OverlappedClose,
		OverlappedReopen,
		InjectionBegin,
		InjectionEnd
	}
}
