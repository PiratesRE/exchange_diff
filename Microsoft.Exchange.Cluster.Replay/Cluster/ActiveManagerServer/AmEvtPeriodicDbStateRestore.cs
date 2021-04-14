using System;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmEvtPeriodicDbStateRestore : AmEvtBase
	{
		internal AmEvtPeriodicDbStateRestore(object context)
		{
			this.Context = context;
		}

		internal object Context { get; private set; }
	}
}
