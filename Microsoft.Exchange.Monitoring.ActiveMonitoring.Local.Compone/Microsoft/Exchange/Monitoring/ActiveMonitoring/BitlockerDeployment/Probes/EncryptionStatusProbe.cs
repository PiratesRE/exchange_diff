using System;
using System.Management;
using System.Reflection;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.BitlockerDeployment.Probes
{
	public class EncryptionStatusProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateDefinition(string probeName, int recurrenceInterval)
		{
			return EncryptionStatusProbe.CreateDefinition("BitlockerDeployment", Environment.MachineName, probeName, recurrenceInterval, recurrenceInterval / 2, 3);
		}

		public static ProbeDefinition CreateDefinition(string serviceName, string targetResource, string probeName, int recurrenceInterval, int timeout, int maxRetry)
		{
			return new ProbeDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				ServiceName = serviceName,
				TypeName = typeof(EncryptionStatusProbe).FullName,
				Name = probeName,
				RecurrenceIntervalSeconds = recurrenceInterval,
				TimeoutSeconds = timeout,
				MaxRetryAttempts = maxRetry,
				TargetResource = targetResource
			};
		}

		private string ConstructEncryptionStatusLine(string DeviceId, BitlockerDeploymentConstants.BitlockerEncryptionStates conversionStatus, int percentage)
		{
			return string.Format("Volume = {0}. ConversionStatus = {1}. EncryptionPercentage = {2} \r\n", DeviceId, conversionStatus, percentage);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			ManagementObjectCollection encryptableDataVolumes = BitlockerDeploymentUtility.GetEncryptableDataVolumes();
			if (encryptableDataVolumes == null || encryptableDataVolumes.Count == 0)
			{
				base.Result.StateAttribute1 = string.Format("Bitlocker Not Installed on this server {0}.; Skipping the Probe", Environment.MachineName);
				return;
			}
			if (!BitlockerDeploymentConstants.WorkflowFve)
			{
				base.Result.StateAttribute1 = "WorkflowFve=false; Skipping the Probe";
				return;
			}
			bool flag = true;
			foreach (ManagementBaseObject managementBaseObject in encryptableDataVolumes)
			{
				ManagementObject encryptableVolume = (ManagementObject)managementBaseObject;
				if (!BitlockerDeploymentUtility.IsVolumeLocked(encryptableVolume))
				{
					BitlockerDeploymentUtility.GetDeviceID(encryptableVolume);
					BitlockerDeploymentConstants.BitlockerEncryptionStates conversionStatus = BitlockerDeploymentUtility.GetConversionStatus(encryptableVolume);
					BitlockerDeploymentUtility.EncryptionPercentage(encryptableVolume);
					if (conversionStatus == BitlockerDeploymentConstants.BitlockerEncryptionStates.FullyDecrypted)
					{
						flag = false;
					}
					else if (conversionStatus == BitlockerDeploymentConstants.BitlockerEncryptionStates.DecryptionInProgress || conversionStatus == BitlockerDeploymentConstants.BitlockerEncryptionStates.DecryptionPaused)
					{
						flag = false;
					}
				}
			}
			base.Result.StateAttribute1 = "Encrypted Volumes : " + BitlockerDeploymentUtility.GetEncryptedEncryptableDataVolumes();
			base.Result.StateAttribute2 = "Unencrypted Volumes : " + BitlockerDeploymentUtility.GetUnencryptedEncryptableDataVolumes();
			base.Result.StateAttribute11 = (flag ? "Passed" : "Failed");
			if (!flag)
			{
				throw new Exception(string.Format("Some Volumes {0} on Server {1} aren't encrypted", BitlockerDeploymentUtility.GetUnencryptedEncryptableDataVolumes(), Environment.MachineName));
			}
		}
	}
}
