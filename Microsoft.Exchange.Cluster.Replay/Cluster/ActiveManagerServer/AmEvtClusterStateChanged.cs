using System;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmEvtClusterStateChanged : AmEvtBase
	{
		internal AmEvtClusterStateChanged()
		{
		}

		public override string ToString()
		{
			return string.Format("{0}: Params: (<none>)", base.GetType().Name);
		}
	}
}
