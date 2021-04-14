using System;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmEvtGroupOwnerButUnknown : AmEvtBase
	{
		internal AmEvtGroupOwnerButUnknown()
		{
		}

		public override string ToString()
		{
			return string.Format("{0}: Params: (<none>)", base.GetType().Name);
		}
	}
}
