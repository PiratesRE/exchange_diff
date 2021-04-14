using System;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public abstract class RpcConfiguration
	{
		public RpcConfiguration()
		{
		}

		public abstract ILamRpc LamRpc { get; }

		public abstract IThrottleHelper ThrottleHelper { get; }

		public void Initialize()
		{
			Dependencies.RegisterInterfaces(this.LamRpc, this.ThrottleHelper);
		}
	}
}
