using System;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class RpsTenantAdminProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			new RpsProbeHelper(this, false).DoWork(cancellationToken);
		}
	}
}
