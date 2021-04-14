using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Rws.Probes;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rws
{
	internal sealed class RwsInstrumentationDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsInstrumentation.DoWork: enter", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationDiscovery.cs", 52);
			if (!this.ShouldDoDeployment())
			{
				return;
			}
			string text = base.Definition.Attributes["TargetReportNames"];
			string[] array = text.Split(new char[]
			{
				';'
			});
			foreach (string targetReportName in array)
			{
				this.CreateProbeDefinitions(targetReportName);
			}
		}

		private bool ShouldDoDeployment()
		{
			if (!LocalEndpointManager.IsDataCenter)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, "Skip RWS monitoring on non datacenter environment", null, "ShouldDoDeployment", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationDiscovery.cs", 78);
				return false;
			}
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Datamining\\Monitoring"))
			{
				if (registryKey != null && (int)registryKey.GetValue("ActiveMonitoringEnabled", 0) != 0)
				{
					if (ExEnvironment.IsTest)
					{
						return true;
					}
					if (ExEnvironment.IsSdfDomain && Regex.IsMatch(Environment.MachineName.Trim(), "^[a-zA-Z0-9]{3}SM01MS\\d{3}$", RegexOptions.IgnoreCase))
					{
						return false;
					}
					if (Regex.IsMatch(Environment.MachineName.Trim(), "^[a-zA-Z0-9]{3}MG(01|T03)MS\\d{3}$", RegexOptions.IgnoreCase))
					{
						string text = base.Definition.Attributes["DeploymentBlackDCList"];
						string[] source = text.Split(new char[]
						{
							';'
						});
						string value = Environment.MachineName.Substring(0, 3);
						if (source.Contains(value))
						{
							return false;
						}
						return true;
					}
				}
				else
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, "Skip RWS monitoring on the server which doesn't have AM enabled", null, "ShouldDoDeployment", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationDiscovery.cs", 122);
				}
			}
			return false;
		}

		private void CreateProbeDefinitions(string TargetReportName)
		{
			string text = string.Format("{0}.{1}", TargetReportName, base.Definition.Name);
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsInstrumentationDiscovery.CreateProbe: Creating probe {0}", text, null, "CreateProbeDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationDiscovery.cs", 139);
			string value = Path.Combine(base.Definition.Attributes["UploaderDetailFileDir"], TargetReportName);
			ProbeDefinition probeDefinition = new ProbeDefinition
			{
				AssemblyPath = RwsInstrumentationDiscovery.assemblyPath,
				TypeName = RwsInstrumentationDiscovery.probeTypeName,
				Name = text,
				ServiceName = base.Definition.ServiceName,
				RecurrenceIntervalSeconds = Convert.ToInt32(base.Definition.Attributes["ProbeRecurrenceIntervalSeconds"]),
				TimeoutSeconds = Convert.ToInt32(base.Definition.Attributes["ProbeTimeoutSeconds"]),
				MaxRetryAttempts = Convert.ToInt32(base.Definition.Attributes["ProbeMaxRetryAttempts"]),
				Enabled = Convert.ToBoolean(base.Definition.Attributes["ProbeEnabled"]),
				WorkItemVersion = RwsInstrumentationDiscovery.assemblyVersion
			};
			probeDefinition.Attributes["TargetServer"] = base.Definition.Attributes["TargetServer"];
			probeDefinition.Attributes["TargetDatabase"] = base.Definition.Attributes["TargetDatabase"];
			probeDefinition.Attributes["TargetSummaryTable"] = base.Definition.Attributes["TargetSummaryTable"];
			probeDefinition.Attributes["TargetDetailTable"] = base.Definition.Attributes["TargetDetailTable"];
			probeDefinition.Attributes["TargetReportName"] = TargetReportName;
			probeDefinition.Attributes["SummaryConnectionString"] = base.Definition.Attributes["SummaryConnectionString"];
			probeDefinition.Attributes["UploaderDetailFileDir"] = value;
			probeDefinition.Attributes["UploaderSummaryFilePath"] = base.Definition.Attributes["UploaderSummaryFilePath"];
			probeDefinition.Attributes["ExeFileName"] = base.Definition.Attributes["ExeFileName"];
			probeDefinition.Attributes["ExeArguments"] = base.Definition.Attributes["ExeArguments"];
			probeDefinition.Attributes["ExeSuccessCopyNumberPattern"] = base.Definition.Attributes["ExeSuccessCopyNumberPattern"];
			probeDefinition.Attributes["OutputFieldSeperator"] = base.Definition.Attributes["OutputFieldSeperator"];
			probeDefinition.Attributes["ConfigurationDir"] = base.Definition.Attributes["ConfigurationDir"];
			probeDefinition.Attributes["AlertSource"] = base.Definition.Attributes["AlertSource"];
			probeDefinition.Attributes["ToAddresses"] = base.Definition.Attributes["ToAddresses"];
			probeDefinition.Attributes["CCAddresses"] = base.Definition.Attributes["CCAddresses"];
			probeDefinition.Attributes["FromAddress"] = base.Definition.Attributes["FromAddress"];
			probeDefinition.Attributes["AlertSubject"] = base.Definition.Attributes["AlertSubject"];
			probeDefinition.Attributes["AlertBody"] = base.Definition.Attributes["AlertBody"];
			probeDefinition.Attributes["CheckDataExistHour"] = base.Definition.Attributes["CheckDataExistHour"];
			probeDefinition.Attributes["DumpRecoveryDays"] = base.Definition.Attributes["DumpRecoveryDays"];
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext).Wait();
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsInstrumentationDiscovery.CreateProbe: Created probe {0}", text, null, "CreateProbeDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationDiscovery.cs", 185);
		}

		private static string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

		private static string assemblyPath = Assembly.GetExecutingAssembly().Location;

		private static string probeTypeName = typeof(RwsInstrumentationProbe).FullName;
	}
}
