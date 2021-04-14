using System;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.BitlockerDeployment.Responders
{
	public class BitlockerDeploymentLockResponder : ResponderWorkItem
	{
		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState)
		{
			return BitlockerDeploymentLockResponder.CreateDefinition(name, serviceName, alertTypeId, alertMask, targetResource, targetHealthState, 10, 3, 180, "Dag");
		}

		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, int maxAllowedAttemptsInADay, int maxAllowedAttemptsInAnHour, int minSecondsBetweenAttempts, string throttleGroupName)
		{
			return new ResponderDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				TypeName = typeof(BitlockerDeploymentLockResponder).FullName,
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

		private string ConstructLockStatusLine(string DeviceId)
		{
			return string.Format("{0}\r\n", DeviceId);
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			ManagementObjectCollection encryptableDataVolumes = BitlockerDeploymentUtility.GetEncryptableDataVolumes();
			bool flag = true;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			foreach (ManagementBaseObject managementBaseObject in encryptableDataVolumes)
			{
				ManagementObject encryptableVolume = (ManagementObject)managementBaseObject;
				string deviceID = BitlockerDeploymentUtility.GetDeviceID(encryptableVolume);
				if (BitlockerDeploymentUtility.IsVolumeLocked(encryptableVolume))
				{
					string text;
					string text2;
					bool flag2 = BitlockerDeploymentUtility.Unlock(encryptableVolume, out text, out text2);
					if (flag2)
					{
						stringBuilder2.AppendLine(this.ConstructLockStatusLine(deviceID));
					}
					else
					{
						stringBuilder.AppendLine(this.ConstructLockStatusLine(deviceID));
						flag = false;
					}
				}
				else
				{
					stringBuilder3.AppendLine(this.ConstructLockStatusLine(deviceID));
				}
			}
			base.Result.StateAttribute1 = stringBuilder.ToString();
			base.Result.StateAttribute2 = stringBuilder2.ToString();
			base.Result.StateAttribute3 = stringBuilder3.ToString();
			if (!flag)
			{
				throw new Exception(string.Format("BitlockerDeploymentLockResponder failed to unlock {0} volumes on machine {1}", stringBuilder, Environment.MachineName));
			}
		}
	}
}
