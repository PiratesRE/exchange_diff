using System;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal enum CLUSTER_REG_COMMAND
	{
		CLUSREG_COMMAND_NONE,
		CLUSREG_SET_VALUE,
		CLUSREG_CREATE_KEY,
		CLUSREG_DELETE_KEY,
		CLUSREG_DELETE_VALUE,
		CLUSREG_SET_KEY_SECURITY,
		CLUSREG_VALUE_DELETED,
		CLUSREG_LAST_COMMAND
	}
}
