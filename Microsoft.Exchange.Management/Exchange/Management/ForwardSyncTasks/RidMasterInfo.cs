using System;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	internal class RidMasterInfo
	{
		public RidMasterInfo(string ridMasterServer, int ridMasterVersionFromAD)
		{
			this.RidMasterServer = ridMasterServer;
			this.RidMasterVersionFromAD = ridMasterVersionFromAD;
		}

		public string RidMasterServer { get; private set; }

		public int RidMasterVersionFromAD { get; private set; }
	}
}
