using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmEvtMoveAllDatabasesOnAdminRequest : AmEvtMoveAllDatabasesBase
	{
		internal AmEvtMoveAllDatabasesOnAdminRequest(AmServerName nodeName) : base(nodeName)
		{
		}
	}
}
