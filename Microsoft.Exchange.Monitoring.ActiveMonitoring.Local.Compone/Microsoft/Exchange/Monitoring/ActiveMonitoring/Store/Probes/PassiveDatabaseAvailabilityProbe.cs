using System;
using System.Threading;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Store.Probes
{
	public class PassiveDatabaseAvailabilityProbe : DatabaseAvailabilityProbeBase
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.ActiveCopy = false;
			base.DoWork(cancellationToken);
		}
	}
}
