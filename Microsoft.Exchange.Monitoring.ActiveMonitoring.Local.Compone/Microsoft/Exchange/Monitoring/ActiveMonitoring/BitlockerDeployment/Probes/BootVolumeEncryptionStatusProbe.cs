using System;
using System.Reflection;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.BitlockerDeployment.Probes
{
	public class BootVolumeEncryptionStatusProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateDefinition(string probeName, int recurrenceInterval)
		{
			return BootVolumeEncryptionStatusProbe.CreateDefinition("BitlockerDeployment", Environment.MachineName, probeName, recurrenceInterval, recurrenceInterval / 2, 3);
		}

		public static ProbeDefinition CreateDefinition(string serviceName, string targetResource, string probeName, int recurrenceInterval, int timeout, int maxRetry)
		{
			return new ProbeDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				ServiceName = serviceName,
				TypeName = typeof(BootVolumeEncryptionStatusProbe).FullName,
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
			if (BitlockerDeploymentUtility.IsBootVolumeEncrypted())
			{
				base.Result.StateAttribute1 = "Boot Volume is Encrypted";
				base.Result.StateAttribute11 = "Failed";
				throw new Exception(string.Format("Boot Volume {0} is encrypted", Environment.MachineName));
			}
			base.Result.StateAttribute1 = "Boot Volume is not Encrypted";
			base.Result.StateAttribute11 = "Passed";
		}
	}
}
