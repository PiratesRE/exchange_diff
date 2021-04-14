using System;

namespace Microsoft.Exchange.Transport
{
	[Flags]
	internal enum ResourceObservingComponents
	{
		None = 0,
		BootScanner = 1,
		ContentAggregator = 2,
		EnhancedDns = 4,
		IsMemberOfResolver = 8,
		MessageResubmission = 16,
		PickUp = 32,
		RemoteDelivery = 64,
		ShadowRedundancy = 128,
		SmtpIn = 256,
		All = 4095
	}
}
