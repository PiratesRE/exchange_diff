using System;

namespace Microsoft.Exchange.Extensibility.Internal
{
	[Flags]
	internal enum DsnFlags
	{
		None = 0,
		Delivery = 1,
		Delay = 2,
		Failure = 4,
		Relay = 8,
		Expansion = 16,
		Quarantine = 32,
		AllFlags = 63
	}
}
