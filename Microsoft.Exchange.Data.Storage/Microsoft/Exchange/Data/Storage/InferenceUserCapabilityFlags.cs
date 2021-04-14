using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum InferenceUserCapabilityFlags
	{
		None = 0,
		ClassificationReady = 1,
		UIReady = 2,
		ClutterEnabled = 4,
		ClassificationEnabled = 8,
		HasBeenClutterInvited = 16
	}
}
