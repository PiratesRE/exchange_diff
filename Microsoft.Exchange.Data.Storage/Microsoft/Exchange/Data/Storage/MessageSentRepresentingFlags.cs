using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum MessageSentRepresentingFlags
	{
		None = 0,
		SendAs = 1,
		SendOnBehalfOf = 2
	}
}
