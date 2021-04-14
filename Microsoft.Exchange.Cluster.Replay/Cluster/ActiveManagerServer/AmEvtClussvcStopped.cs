using System;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmEvtClussvcStopped : AmEvtBase
	{
		internal AmEvtClussvcStopped()
		{
		}

		public override string ToString()
		{
			return string.Format("{0}: Params: (<none>)", base.GetType().Name);
		}
	}
}
