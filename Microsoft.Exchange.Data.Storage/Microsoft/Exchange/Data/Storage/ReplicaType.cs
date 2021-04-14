using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum ReplicaType
	{
		LegacyReplicas = 1,
		Exchange2007OrLaterReplicas = 2,
		CurrentServerVersionReplicas = 4,
		AllReplicas = 3
	}
}
