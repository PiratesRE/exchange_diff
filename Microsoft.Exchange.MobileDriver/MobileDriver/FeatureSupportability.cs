using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	[Flags]
	internal enum FeatureSupportability
	{
		None = 0,
		Expiry = 16,
		Deferral = 32
	}
}
