using System;
using System.Management;
using System.Reflection;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.BitlockerDeployment.Probes
{
	public class LockStatusProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateDefinition(string probeName, int recurrenceInterval)
		{
			return LockStatusProbe.CreateDefinition("BitlockerDeployment", Environment.MachineName, probeName, recurrenceInterval, recurrenceInterval / 2, 3);
		}

		public static ProbeDefinition CreateDefinition(string serviceName, string targetResource, string probeName, int recurrenceInterval, int timeout, int maxRetry)
		{
			return new ProbeDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				ServiceName = serviceName,
				TypeName = typeof(LockStatusProbe).FullName,
				Name = probeName,
				RecurrenceIntervalSeconds = recurrenceInterval,
				TimeoutSeconds = timeout,
				MaxRetryAttempts = maxRetry,
				TargetResource = targetResource
			};
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			ManagementObjectCollection encryptableVolumes = BitlockerDeploymentUtility.GetEncryptableVolumes();
			if (encryptableVolumes == null || encryptableVolumes.Count == 0)
			{
				base.Result.StateAttribute1 = string.Format("Bitlocker Not Installed on this server {0}; Skipping the Probe", Environment.MachineName);
				return;
			}
			string lockedVolumes = BitlockerDeploymentUtility.GetLockedVolumes();
			string unlockedVolumes = BitlockerDeploymentUtility.GetUnlockedVolumes();
			base.Result.StateAttribute1 = "Locked Volumes:" + lockedVolumes;
			base.Result.StateAttribute2 = "Unlocked Volumes:" + unlockedVolumes;
			if (string.IsNullOrEmpty(lockedVolumes))
			{
				base.Result.StateAttribute11 = "Passed";
				return;
			}
			base.Result.StateAttribute11 = "Failed";
			throw new Exception(string.Format("Some Volumes {0} on Server {1} are locked", lockedVolumes, Environment.MachineName));
		}
	}
}
