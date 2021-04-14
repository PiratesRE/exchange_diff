using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	public enum MobileFeaturesEnabled
	{
		None = 0,
		AirSyncDisabled = 4,
		MowaDisabled = 8
	}
}
