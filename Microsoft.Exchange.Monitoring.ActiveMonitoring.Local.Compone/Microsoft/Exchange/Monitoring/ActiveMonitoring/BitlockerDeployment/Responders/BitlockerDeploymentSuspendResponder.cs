using System;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.BitlockerDeployment.Responders
{
	public class BitlockerDeploymentSuspendResponder : ResponderWorkItem
	{
		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState)
		{
			return BitlockerDeploymentSuspendResponder.CreateDefinition(name, serviceName, alertTypeId, alertMask, targetResource, targetHealthState, 10, 3, 180, "Dag");
		}

		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, int maxAllowedAttemptsInADay, int maxAllowedAttemptsInAnHour, int minSecondsBetweenAttempts, string throttleGroupName)
		{
			return new ResponderDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				TypeName = typeof(BitlockerDeploymentSuspendResponder).FullName,
				Name = name,
				ServiceName = serviceName,
				AlertTypeId = alertTypeId,
				AlertMask = alertMask,
				TargetResource = targetResource,
				TargetHealthState = targetHealthState,
				RecurrenceIntervalSeconds = 300,
				WaitIntervalSeconds = 30,
				TimeoutSeconds = 300,
				MaxRetryAttempts = 3,
				Enabled = true,
				ThrottleGroupName = throttleGroupName
			};
		}

		private string ConstructSuspendStatusLine(string DeviceId)
		{
			return string.Format("{0}\r\n", DeviceId);
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			ManagementObjectCollection encryptableDataVolumes = BitlockerDeploymentUtility.GetEncryptableDataVolumes();
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			foreach (ManagementBaseObject managementBaseObject in encryptableDataVolumes)
			{
				ManagementObject encryptableVolume = (ManagementObject)managementBaseObject;
				string deviceID = BitlockerDeploymentUtility.GetDeviceID(encryptableVolume);
				if (BitlockerDeploymentUtility.IsVolumeEncryptionSuspended(encryptableVolume))
				{
					bool flag2 = BitlockerDeploymentUtility.Resume(encryptableVolume);
					if (flag2)
					{
						stringBuilder2.AppendLine(this.ConstructSuspendStatusLine(deviceID));
					}
					else
					{
						stringBuilder.AppendLine(this.ConstructSuspendStatusLine(deviceID));
						flag = true;
					}
				}
				else
				{
					stringBuilder2.AppendLine(this.ConstructSuspendStatusLine(deviceID));
				}
			}
			base.Result.StateAttribute1 = "Volume failed Resume: " + stringBuilder.ToString();
			base.Result.StateAttribute2 = "Resumed Volumes: " + stringBuilder2.ToString();
			if (flag)
			{
				throw new Exception(string.Format("BitlockerDeploymentSuspendResponder failed to resume {0} volumes on machine {1}", stringBuilder, Environment.MachineName));
			}
		}
	}
}
