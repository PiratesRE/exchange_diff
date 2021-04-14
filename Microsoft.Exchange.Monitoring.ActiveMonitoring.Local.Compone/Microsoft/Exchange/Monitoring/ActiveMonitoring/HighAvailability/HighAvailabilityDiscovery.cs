using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability
{
	public sealed class HighAvailabilityDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.cToken = cancellationToken;
			try
			{
				if (FfoLocalEndpointManager.IsForefrontForOfficeDatacenter)
				{
					base.Result.StateAttribute1 = "FFO Datacenter Detected. HA monitoring will bail out as designed.";
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, "HighAvailabilityDiscovery:: DoWork(): {0} is in an FFO datacenter and has no role here.", Environment.MachineName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\HighAvailabilityDiscovery.cs", 64);
				}
				else
				{
					base.Result.StateAttribute4 = string.Empty;
					LocalEndpointManager instance = LocalEndpointManager.Instance;
					this.enrolledWorkItems = new List<MonitoringContextBase.EnrollmentResult>();
					this.runtimeMessages = new List<string>();
					this.ProcessMonitoringContext(new AllRolesMonitoringContext(base.Broker, base.TraceContext));
					base.Result.StateAttribute1 = string.Format("Responders Disabled={0}. ", HighAvailabilityConstants.DisableResponders);
					if (instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
					{
						this.ProcessMonitoringContext(new ClusterMonitoringContext(base.Broker, base.TraceContext));
						this.ProcessMonitoringContext(new DataRedundancyMonitoringContext(base.Broker, base.TraceContext));
						this.ProcessMonitoringContext(new ReplServiceMonitoringContext(base.Broker, base.TraceContext));
						if (instance.MailboxDatabaseEndpoint != null && instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count > 0)
						{
							this.ProcessMonitoringContext(new DatabaseCopyMonitoringContext(base.Broker, instance, base.TraceContext));
							this.ProcessMonitoringContext(new EseDatabaseMonitoringContext(base.Broker, instance, base.TraceContext));
						}
						else
						{
							MaintenanceResult result = base.Result;
							result.StateAttribute1 += string.Format("MDbEndpoint=NULL or MDbInfoCollectionForBackend is Empty. ", new object[0]);
						}
					}
					else
					{
						MaintenanceResult result2 = base.Result;
						result2.StateAttribute1 += "MailboxRole not installed! ";
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, "HighAvailabilityDiscovery:: DoWork(): {0} doesn't have Mailbox role installed.", Environment.MachineName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\HighAvailabilityDiscovery.cs", 110);
					}
					this.WriteEnrollmentLog();
					if (this.exceptionCaught != null && this.exceptionCaught.Count > 0)
					{
						int num = this.exceptionCaught.Count;
						foreach (Exception ex in this.exceptionCaught)
						{
							if (ex != null && ex is EndpointManagerEndpointUninitializedException)
							{
								num--;
							}
						}
						if (num > 0)
						{
							throw new AggregateException(this.exceptionCaught);
						}
						base.Result.StateAttribute2 = "!!! EndpointManagerEndpointUninitializedException caught during Context creation. !!!";
					}
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				base.Result.StateAttribute2 = "!!! EndpointManagerEndpointUninitializedException caught. !!!";
				WTFDiagnostics.TraceInformation(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, "HighAvailabilityDiscovery:: DoWork(): EndpointManagerEndpointUninitializedException caught. Ignoring", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\HighAvailabilityDiscovery.cs", 146);
			}
			catch (OperationCanceledException ex2)
			{
				base.Result.StateAttribute2 = string.Format("!!! Cancellation requested at stage {0}. !!!", ex2.Message);
				WTFDiagnostics.TraceInformation(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, "HighAvailabilityDiscovery:: DoWork(): Cancellation requested.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\HighAvailabilityDiscovery.cs", 154);
			}
		}

		private void ProcessMonitoringContext(MonitoringContextBase context)
		{
			if (HighAvailabilityUtility.CheckCancellationRequested(this.cToken))
			{
				throw new OperationCanceledException((context == null) ? "Unknown" : context.GetType().ToString());
			}
			if (context != null)
			{
				string text = string.Empty;
				try
				{
					context.CreateContext();
				}
				catch (Exception item)
				{
					this.exceptionCaught.Add(item);
					text += "OuterEx ";
				}
				if (context.WorkItemsEnrollmentResult != null && context.WorkItemsEnrollmentResult.Count > 0)
				{
					this.enrolledWorkItems.AddRange(context.WorkItemsEnrollmentResult);
				}
				if (context.LoggedMessages != null)
				{
					this.runtimeMessages.Add(context.LoggedMessages);
				}
				if (context.ExceptionCaught != null && context.ExceptionCaught.Count > 0)
				{
					this.exceptionCaught.AddRange(context.ExceptionCaught);
					text += "InnerEx ";
				}
				if (string.IsNullOrEmpty(text))
				{
					text = "OK ";
				}
				MaintenanceResult result = base.Result;
				result.StateAttribute4 += string.Format("Enrolled {0}. Result={1}{2};", context.GetType().ToString(), text, Environment.NewLine);
				return;
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, "HighAvailabilityDiscovery:: ProcessMonitoringContext(): Context is NULL!", null, "ProcessMonitoringContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\HighAvailabilityDiscovery.cs", 217);
		}

		private void WriteEnrollmentLog()
		{
			base.Result.StateAttribute3 = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			string value = HighAvailabilityUtility.RegReader.GetValue<string>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\Parameters", "WorkItemEnrollmentLogPath", string.Empty);
			bool flag = !string.IsNullOrEmpty(value);
			if (flag)
			{
				base.Result.StateAttribute3 = string.Format("Extended Logging will be written to '{0}'", value);
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
					File.WriteAllText(value, stringBuilder.ToString());
				}
				catch (Exception ex)
				{
					base.Result.StateAttribute3 = string.Format("Exception caught in extended logging = {0}", ex.ToString());
				}
			}
			MaintenanceResult result = base.Result;
			result.StateAttribute3 += string.Format("Probes Enrolled={0}; Monitors Enrolled={1}; Responders Enrolled={2}", num, num2, num3);
			base.Result.StateAttribute5 = string.Join(Environment.NewLine, this.runtimeMessages);
		}

		private List<MonitoringContextBase.EnrollmentResult> enrolledWorkItems;

		private List<string> runtimeMessages;

		private CancellationToken cToken;

		private readonly List<Exception> exceptionCaught = new List<Exception>();
	}
}
