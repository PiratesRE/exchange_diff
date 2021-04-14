using System;

namespace Microsoft.Exchange.TextProcessing
{
	[Flags]
	internal enum ClusteringStatusEnum
	{
		None = 0,
		UkOneOrMultiSource = 1,
		OneSource = 2,
		MultiSource = 4,
		SourceMask = 7,
		OneAndMultiSource = 8,
		FpFeed = 1048576,
		ThirdPartyFeed = 16777216,
		HoneypotFeed = 33554432,
		FnFeed = 67108864,
		SenFeed = 134217728,
		SewrFeed = 268435456,
		SpamFeedMask = 520093696,
		SpamVerdict = 536870912
	}
}
