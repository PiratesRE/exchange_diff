using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmEvtMoveAllDatabasesOnComponentRequest : AmEvtMoveAllDatabasesBase
	{
		internal AmEvtMoveAllDatabasesOnComponentRequest(AmServerName nodeName) : base(nodeName)
		{
		}
	}
}
