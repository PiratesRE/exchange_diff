using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class QueueDigestDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			GenericWorkItemHelper.CreateAllDefinitions(new List<string>
			{
				"QueueDigest.xml"
			}, base.Broker, base.TraceContext, base.Result);
		}

		private const int E15MajorVersion = 15;
	}
}
