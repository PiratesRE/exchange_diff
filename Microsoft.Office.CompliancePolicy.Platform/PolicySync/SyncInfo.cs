using System;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[Serializable]
	public sealed class SyncInfo
	{
		public SyncInfo()
		{
		}

		public SyncInfo(byte[] previousSyncCookie, byte[] currentSyncCookie, PolicyVersion latestVersion)
		{
			this.PreviousSyncCookie = previousSyncCookie;
			this.CurrentSyncCookie = currentSyncCookie;
			this.LatestVersion = latestVersion;
		}

		public byte[] PreviousSyncCookie { get; set; }

		public byte[] CurrentSyncCookie { get; set; }

		public PolicyVersion LatestVersion { get; set; }
	}
}
