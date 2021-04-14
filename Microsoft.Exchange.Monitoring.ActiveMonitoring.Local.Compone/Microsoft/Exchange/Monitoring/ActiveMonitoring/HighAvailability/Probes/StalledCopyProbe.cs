using System;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Probes
{
	internal class StalledCopyProbe : CopyStatusProbeBase
	{
		public static ProbeDefinition CreateDefinition(string name, string serviceName, MailboxDatabaseInfo targetDatabase, int recurrenceInterval)
		{
			return CopyStatusProbeBase.CreateDefinition(name, typeof(StalledCopyProbe).FullName, serviceName, targetDatabase, recurrenceInterval, recurrenceInterval / 2, 3);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.DoWork(cancellationToken);
			if (base.CopyStatus.CopyStatus == CopyStatusEnum.DisconnectedAndHealthy || base.CopyStatus.CopyStatus == CopyStatusEnum.DisconnectedAndResynchronizing)
			{
				base.Result.StateAttribute11 = "Failed";
				throw new HighAvailabilityMAProbeRedResultException(string.Format("Database Copy {0} on server {1} is {2}", base.CopyStatus.DatabaseName, Environment.MachineName, base.CopyStatus.CopyStatus));
			}
			base.Result.StateAttribute11 = string.Format("Passed. CurrentState={0}", base.CopyStatus.CopyStatus);
		}
	}
}
