using System;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmEvtSystemStartup : AmEvtBase
	{
		internal AmEvtSystemStartup()
		{
		}

		public override string ToString()
		{
			return string.Format("{0}: Params: (<None>)", base.GetType().Name);
		}
	}
}
