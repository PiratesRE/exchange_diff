using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum InferenceProcessingActions : long
	{
		None = 0L,
		ProcessImplicitMarkAsNotClutter = 1L
	}
}
