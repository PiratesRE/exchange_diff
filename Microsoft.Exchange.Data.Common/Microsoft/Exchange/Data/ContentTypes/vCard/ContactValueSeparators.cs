using System;

namespace Microsoft.Exchange.Data.ContentTypes.vCard
{
	[Flags]
	public enum ContactValueSeparators
	{
		None = 0,
		Comma = 1,
		Semicolon = 2
	}
}
