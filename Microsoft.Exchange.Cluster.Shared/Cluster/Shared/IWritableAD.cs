using System;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal interface IWritableAD
	{
		bool SetDatabaseLegacyDnAndOwningServer(Guid mdbGuid, AmServerName lastMountedServerName, AmServerName masterServerName, bool isForceUpdate);

		void ResetAllowFileRestoreDsFlag(Guid mdbGuid, AmServerName lastMountedServerName, AmServerName masterServerName);
	}
}
