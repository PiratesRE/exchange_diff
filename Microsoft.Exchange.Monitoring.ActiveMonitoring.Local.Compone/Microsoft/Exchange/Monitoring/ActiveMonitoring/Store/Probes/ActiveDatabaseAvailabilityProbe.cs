using System;
using System.Threading;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Store.Probes
{
	public class ActiveDatabaseAvailabilityProbe : DatabaseAvailabilityProbeBase
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.ActiveCopy = true;
			base.DoWork(cancellationToken);
		}
	}
}
