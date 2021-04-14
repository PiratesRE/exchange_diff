using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmEvtBase
	{
		internal AmEvtBase()
		{
		}

		internal bool Notify(bool isHighPriority)
		{
			AmTrace.Debug("Notifying system manager about event arrival: {0}", new object[]
			{
				this.ToString()
			});
			return AmSystemManager.Instance.EnqueueEvent(this, isHighPriority);
		}

		internal bool Notify()
		{
			return this.Notify(false);
		}
	}
}
