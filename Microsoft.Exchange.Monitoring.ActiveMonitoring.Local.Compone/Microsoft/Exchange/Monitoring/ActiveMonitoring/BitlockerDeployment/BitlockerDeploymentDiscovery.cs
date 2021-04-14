using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.BitlockerDeployment
{
	public sealed class BitlockerDeploymentDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (FfoLocalEndpointManager.IsForefrontForOfficeDatacenter)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, "BitlockerDeploymentDiscovery:: DoWork(): {0} is in an FFO datacenter and has no role here.", Environment.MachineName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\BitlockerDeployment\\BitlockerDeploymentDiscovery.cs", 44);
				return;
			}
			List<MonitoringContextBase> list = new List<MonitoringContextBase>();
			base.Result.StateAttribute1 = string.Format("Responders Disabled={0}", BitlockerDeploymentConstants.DisableResponders);
			if (instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				list.Add(new BootVolumeEncryptionStatusMonitoringContext(base.Broker, base.TraceContext));
				list.Add(new LockStatusMonitoringContext(base.Broker, base.TraceContext));
				list.Add(new EncryptionSuspendedMonitoringContext(base.Broker, base.TraceContext));
				list.Add(new DraProtectorMonitoringContext(base.Broker, base.TraceContext));
				list.Add(new DraDecryptorMonitoringContext(base.Broker, base.TraceContext));
			}
			else
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, "BitlockerDeploymentDiscovery:: DoWork(): {0} doesn't have Mailbox role installed.", Environment.MachineName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\BitlockerDeployment\\BitlockerDeploymentDiscovery.cs", 69);
			}
			this.enrolledWorkItems = new List<MonitoringContextBase.EnrollmentResult>();
			this.runtimeMessages = new List<string>();
			if (list.Count > 0)
			{
				foreach (MonitoringContextBase monitoringContextBase in list)
				{
					monitoringContextBase.CreateContext();
					this.enrolledWorkItems.AddRange(monitoringContextBase.WorkItemsEnrollmentResult);
					this.runtimeMessages.Add(monitoringContextBase.LoggedMessages);
				}
			}
			this.WriteEnrollmentLog();
		}

		private void WriteEnrollmentLog()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			string logPath = BitlockerDeploymentConstants.LogPath;
			bool flag = !string.IsNullOrEmpty(logPath);
			if (flag)
			{
				base.Result.StateAttribute3 = string.Format("Extended Logging will be written to '{0}'", logPath);
			}
			foreach (MonitoringContextBase.EnrollmentResult enrollmentResult in this.enrolledWorkItems)
			{
				switch (enrollmentResult.WorkItemType)
				{
				case MonitoringContextBase.EnrollmentType.Probe:
					num++;
					break;
				case MonitoringContextBase.EnrollmentType.Monitor:
					num2++;
					break;
				case MonitoringContextBase.EnrollmentType.Responder:
					num3++;
					break;
				}
				if (flag)
				{
					stringBuilder.Append("WorkItemType=");
					stringBuilder.AppendLine(enrollmentResult.WorkItemType.ToString());
					stringBuilder.Append("WorkItemResultName=");
					stringBuilder.AppendLine(enrollmentResult.WorkItemResultName);
					stringBuilder.Append("WorkItemClassName=");
					stringBuilder.AppendLine(enrollmentResult.WorkItemClass);
				}
			}
			if (flag)
			{
				stringBuilder.AppendLine(string.Format("List generated at {0}.", DateTime.UtcNow.ToLongTimeString()));
				try
				{
					File.WriteAllText(logPath, stringBuilder.ToString());
				}
				catch (Exception ex)
				{
					base.Result.StateAttribute4 = string.Format("Exception caught in log writing = {0}", ex.ToString());
				}
			}
			base.Result.StateAttribute2 = string.Format("Probes Enrolled={0}; Monitors Enrolled={1}; Responders Enrolled={2}", num, num2, num3);
			base.Result.StateAttribute5 = string.Join(Environment.NewLine, this.runtimeMessages);
		}

		private List<MonitoringContextBase.EnrollmentResult> enrolledWorkItems;

		private List<string> runtimeMessages;
	}
}
