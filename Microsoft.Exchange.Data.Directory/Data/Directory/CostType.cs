using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal enum CostType
	{
		Connection,
		HangingConnection,
		CAS,
		CMDLET,
		ActiveRunspace,
		ReplicationHealth
	}
}
