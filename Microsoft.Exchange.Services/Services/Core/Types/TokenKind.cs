using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public enum TokenKind
	{
		StartTag,
		EndTag,
		Text,
		EmptyTag,
		OverlappedClose,
		OverlappedReopen,
		IgnorableTag
	}
}
