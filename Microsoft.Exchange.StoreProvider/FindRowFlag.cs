using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum FindRowFlag
	{
		None = 0,
		FindBackward = 1,
		DeferredErrors = 8,
		DisableFastFind = 16,
		DisableSlowFind = 32,
		DisableQP = 64
	}
}
