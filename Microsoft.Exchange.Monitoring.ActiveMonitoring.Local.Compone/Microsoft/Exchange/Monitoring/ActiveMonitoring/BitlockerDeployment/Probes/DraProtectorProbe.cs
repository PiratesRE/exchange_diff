using System;
using System.Reflection;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.BitlockerDeployment.Probes
{
	public class DraProtectorProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateDefinition(string probeName, int recurrenceInterval)
		{
			return DraProtectorProbe.CreateDefinition("BitlockerDeployment", Environment.MachineName, probeName, recurrenceInterval, recurrenceInterval / 2, 3);
		}

		public static ProbeDefinition CreateDefinition(string serviceName, string targetResource, string probeName, int recurrenceInterval, int timeout, int maxRetry)
		{
			return new ProbeDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				ServiceName = serviceName,
				TypeName = typeof(DraProtectorProbe).FullName,
				Name = probeName,
				RecurrenceIntervalSeconds = recurrenceInterval,
				TimeoutSeconds = timeout,
				MaxRetryAttempts = maxRetry,
				TargetResource = targetResource
			};
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!BitlockerDeploymentConstants.WorkflowFve)
			{
				base.Result.StateAttribute1 = "WorkflowFve=false; Skipping the Probe";
				return;
			}
			string volumesNotProtectedWithDra = BitlockerDeploymentUtility.GetVolumesNotProtectedWithDra();
			base.Result.StateAttribute1 = "Volumes not protected with DRA:" + volumesNotProtectedWithDra;
			if (string.IsNullOrEmpty(volumesNotProtectedWithDra))
			{
				base.Result.StateAttribute11 = "Passed";
				return;
			}
			base.Result.StateAttribute11 = "Failed";
			throw new Exception(string.Format("Some Volumes {0} on Server {1} are not protected with DRA", volumesNotProtectedWithDra, Environment.MachineName));
		}
	}
}
