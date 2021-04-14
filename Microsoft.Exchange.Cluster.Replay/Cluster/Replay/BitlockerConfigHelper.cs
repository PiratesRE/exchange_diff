using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class BitlockerConfigHelper
	{
		public static bool IsBitlockerWin8UsedOnlyEncryptionFeatureEnabled()
		{
			return !RegistryParameters.BitlockerWin8UsedOnlyDisabled;
		}

		public static bool IsBitlockerEmptyWin8VolumesUsedOnlyEncryptionFeatureEnabled()
		{
			return !RegistryParameters.BitlockerWin8EmptyUsedOnlyDisabled;
		}

		public static bool IsBitlockerEmptyWin7VolumesFullVolumeEncryptionFeatureEnabled()
		{
			return !RegistryParameters.BitlockerWin7EmptyFullVolumeDisabled;
		}

		public static bool IsBitlockerManagerEnabled()
		{
			return !RegistryParameters.BitlockerFeatureDisabled;
		}
	}
}
