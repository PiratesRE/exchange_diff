using System;
using Microsoft.Exchange.Data.HA.DirectoryServices;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmEvtRejoinNodeForDac : AmEvtBase
	{
		internal AmEvtRejoinNodeForDac(IADDatabaseAvailabilityGroup dag, IADServer localServer)
		{
			this.Dag = dag;
			this.LocalServer = localServer;
		}

		internal IADDatabaseAvailabilityGroup Dag { get; set; }

		internal IADServer LocalServer { get; set; }

		public override string ToString()
		{
			return string.Format("{0}: Params: (Dag={1})", base.GetType().Name, this.Dag);
		}
	}
}
