using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum CreateMessageFlags
	{
		None = 0,
		ContentAggregation = 1,
		Associated = 64,
		DeferredErrors = 8
	}
}
