using System;
using System.Diagnostics.Eventing.Reader;
using System.Management;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Audit;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders
{
	public class RemoteRestartServiceResponder : ResponderWorkItem
	{
		public static ResponderDefinition CreateDefinition(string responderName, string monitorName, string machineName, string serviceName, ServiceHealthStatus responderTargetState, Component component, int waitIntervalSeconds, bool startServiceOnly = false)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = RemoteRestartServiceResponder.AssemblyPath;
			responderDefinition.TypeName = RemoteRestartServiceResponder.TypeName;
			responderDefinition.AlertTypeId = "*";
			responderDefinition.RecurrenceIntervalSeconds = 0;
			responderDefinition.TimeoutSeconds = 300;
			responderDefinition.Enabled = true;
			if (string.IsNullOrEmpty(responderName))
			{
				throw new ArgumentNullException("responderName");
			}
			if (string.IsNullOrEmpty(monitorName))
			{
				throw new ArgumentNullException("monitorName");
			}
			if (string.IsNullOrEmpty(machineName))
			{
				throw new ArgumentNullException("machineName");
			}
			if (string.IsNullOrEmpty(serviceName))
			{
				throw new ArgumentNullException("serviceName");
			}
			if (!machineName.Contains("."))
			{
				throw new ArgumentException("Machine name is not FQDN", "machineName");
			}
			responderDefinition.Name = responderName;
			responderDefinition.TargetGroup = machineName;
			responderDefinition.TargetResource = serviceName;
			responderDefinition.TargetHealthState = responderTargetState;
			responderDefinition.AlertMask = string.Format("{0}/{1}", monitorName, machineName);
			responderDefinition.ServiceName = component.Name;
			responderDefinition.WaitIntervalSeconds = waitIntervalSeconds;
			responderDefinition.Attributes["ServiceStartOnly"] = startServiceOnly.ToString();
			return responderDefinition;
		}

		internal static DateTime? GetLastServiceStartTime(string machineName, string serviceDisplayName)
		{
			DateTime? result = null;
			string query = string.Format("<QueryList><Query Id=\"0\" Path=\"System\"><Select Path=\"System\">*[System[Provider[@Name='Service Control Manager'] and (EventID=7036)] and (EventData/Data[@Name='param1']='{0}') and (EventData/Data[@Name='param2']='running')]</Select></Query></QueryList>", serviceDisplayName);
			using (EventLogReader eventLogReader = new EventLogReader(new EventLogQuery("System", PathType.LogName, query)
			{
				Session = new EventLogSession(machineName),
				ReverseDirection = true
			}))
			{
				using (EventRecord eventRecord = eventLogReader.ReadEvent())
				{
					if (eventRecord != null)
					{
						WTFDiagnostics.TraceDebug<string, string, DateTime?>(ExTraceGlobals.HeartbeatTracer, TracingContext.Default, "Last start time for service '{0}' on machine '{1}' is '{2}'.", serviceDisplayName, machineName, eventRecord.TimeCreated, null, "GetLastServiceStartTime", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\RemoteRestartServiceResponder.cs", 159);
						result = eventRecord.TimeCreated;
					}
					else
					{
						WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.HeartbeatTracer, TracingContext.Default, "Last service start time for service '{0}' on machine '{1}' is not available.", serviceDisplayName, machineName, null, "GetLastServiceStartTime", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\RemoteRestartServiceResponder.cs", 164);
					}
				}
			}
			if (result != null)
			{
				result = new DateTime?(result.Value.ToUniversalTime());
			}
			return result;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			string machineName = base.Definition.TargetGroup;
			string serviceName = base.Definition.TargetResource;
			ManagementObject managementObject = null;
			ManagementObject process = null;
			bool flag = this.ReadAttribute("ServiceStartOnly", false);
			try
			{
				TimeSpan timeSpan;
				Utils.Measure<ManagementObject>(() => WmiWrapper.GetServiceObject(machineName, serviceName), out managementObject, out timeSpan);
				base.Result.StateAttribute6 = timeSpan.TotalMilliseconds;
				if (managementObject == null)
				{
					throw new Exception("Service not found.");
				}
				string serviceDisplayName = (string)managementObject["DisplayName"];
				DateTime? lastServiceStartTime = RemoteRestartServiceResponder.GetLastServiceStartTime(machineName, serviceDisplayName);
				if (lastServiceStartTime != null)
				{
					base.Result.StateAttribute1 = lastServiceStartTime.ToString();
				}
				if (((lastServiceStartTime != null) ? (DateTime.UtcNow - lastServiceStartTime.Value) : TimeSpan.FromDays(1.0)).TotalSeconds > (double)base.Definition.WaitIntervalSeconds)
				{
					uint processId = (uint)managementObject["ProcessId"];
					if (processId > 0U)
					{
						TimeSpan timeSpan2;
						Utils.Measure<ManagementObject>(() => WmiWrapper.GetProcessObject(machineName, processId), out process, out timeSpan2);
						base.Result.StateAttribute7 = timeSpan2.TotalMilliseconds;
						if (process != null)
						{
							if (!flag)
							{
								Privilege.RunWithPrivilege("SeDebugPrivilege", true, delegate
								{
									this.InvokeWmiMethod(process, "Terminate");
								});
								this.InvokeWmiMethod(managementObject, "StartService");
							}
						}
						else
						{
							WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.HeartbeatTracer, this.traceContext, "'{0}' service process not found.", serviceName, null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\RemoteRestartServiceResponder.cs", 249);
							this.InvokeWmiMethod(managementObject, "StartService");
						}
					}
					else
					{
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.HeartbeatTracer, this.traceContext, "'{0}' service is not running.", serviceName, null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\RemoteRestartServiceResponder.cs", 256);
						this.InvokeWmiMethod(managementObject, "StartService");
					}
				}
				else
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.HeartbeatTracer, this.traceContext, "'{0}' service has been restarted recently, so not attempting restart now.", serviceName, null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\RemoteRestartServiceResponder.cs", 262);
					base.Result.IsThrottled = true;
					base.Result.RecoveryResult = ServiceRecoveryResult.Skipped;
					base.Result.IsRecoveryAttempted = false;
				}
			}
			finally
			{
				if (managementObject != null)
				{
					managementObject.Dispose();
				}
				if (process != null)
				{
					process.Dispose();
				}
			}
		}

		private void InvokeWmiMethod(ManagementObject obj, string methodName)
		{
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.HeartbeatTracer, this.traceContext, "Invoking WMI method '{0}'.", methodName, null, "InvokeWmiMethod", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\RemoteRestartServiceResponder.cs", 298);
			uint num = (uint)obj.InvokeMethod(methodName, null);
			WTFDiagnostics.TraceDebug<uint>(ExTraceGlobals.HeartbeatTracer, this.traceContext, "Result = '{0}'.", num, null, "InvokeWmiMethod", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\RemoteRestartServiceResponder.cs", 302);
			if (num != 0U)
			{
				throw new Exception(string.Format("Failed to execute method '{0}', result = '{1}'.", methodName, num));
			}
		}

		private const string ServiceStartOnlyAttributeName = "ServiceStartOnly";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(RemoteRestartServiceResponder).FullName;

		private TracingContext traceContext = TracingContext.Default;
	}
}
