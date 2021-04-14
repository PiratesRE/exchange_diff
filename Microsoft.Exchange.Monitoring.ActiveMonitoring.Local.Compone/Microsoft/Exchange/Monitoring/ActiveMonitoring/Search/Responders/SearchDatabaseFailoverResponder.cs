using System;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Responders
{
	public class SearchDatabaseFailoverResponder : DatabaseFailoverResponder
	{
		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			string targetResource = base.Definition.TargetResource;
			SearchMonitoringHelper.LogRecoveryAction(this, "Performing database failover for '{0}'.", new object[]
			{
				targetResource
			});
			try
			{
				base.DoResponderWork(cancellationToken);
			}
			catch (Exception ex)
			{
				if (ex is ThrottlingRejectedOperationException)
				{
					SearchMonitoringHelper.LogRecoveryAction(this, "Database failover throttled.", new object[0]);
				}
				else
				{
					SearchMonitoringHelper.LogRecoveryAction(this, "Caught exception: '{0}'.", new object[]
					{
						ex
					});
				}
				throw;
			}
			SearchMonitoringHelper.LogRecoveryAction(this, "Failover completed.", new object[0]);
		}
	}
}
