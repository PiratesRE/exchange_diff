using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Responders
{
	public class CollectAndMergeResponder : RemotePowerShellResponder
	{
		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "CollectAndMergeResponder.DoResponderWork: Attempting to run CollectAndMerge.ps1.", null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Responders\\CollectAndMergeResponder.cs", 40);
			new RecoveryActionRunner(RecoveryActionId.CollectAndMerge, Environment.MachineName, this, true, cancellationToken, null)
			{
				IsIgnoreResourceName = true
			}.Execute(delegate()
			{
				this.PerformRemoteCollectAndMerge();
			});
		}

		private void PerformRemoteCollectAndMerge()
		{
			RecoveryActionHelper.RunAndMeasure(string.Format("CollectAndMerge(WorkitemId={0}, ResultId={1})", base.Id, base.Result.ResultId), false, ManagedAvailabilityCrimsonEvents.MeasureOperation, delegate
			{
				if (base.RemotePowerShell == null)
				{
					base.CreateRunspace();
				}
				IADDatabaseAvailabilityGroup localDAG = CachedAdReader.Instance.LocalDAG;
				string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
				string text = "Administrator@" + domainName;
				if (domainName.EndsWith("prod.outlook.com") || domainName.EndsWith("prod.exchangelabs.com") || domainName.EndsWith("mgd.msft.net"))
				{
					text = "exhalert@microsoft.com";
				}
				PSCommand pscommand = new PSCommand();
				pscommand.AddScript(string.Format("CollectAndMergeEventsFast.ps1 -DatabaseAvailabilityGroup {0} -EventsStartTime {1} -Email {2} -Description 'DataProtectionEscalateResponder collected events for {0} {1}' -Target {3}", new object[]
				{
					localDAG.Name,
					DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(45.0)),
					text,
					Environment.MachineName
				}), false);
				Collection<PSObject> collection = base.RemotePowerShell.InvokePSCommand(pscommand);
				if (collection != null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					foreach (PSObject psobject in collection)
					{
						stringBuilder.AppendLine(psobject.ToString());
					}
					base.Result.StateAttribute5 = "Execution Result=" + stringBuilder.ToString();
				}
				return string.Empty;
			});
		}

		internal static ResponderDefinition CreateDefinition(string responderName, string monitorName, ServiceHealthStatus responderTargetState, string targetServerName, string serviceName = "Exchange", bool enabled = true)
		{
			return new ResponderDefinition
			{
				AssemblyPath = CollectAndMergeResponder.AssemblyPath,
				TypeName = CollectAndMergeResponder.TypeName,
				Name = responderName,
				ServiceName = serviceName,
				AlertTypeId = "*",
				AlertMask = monitorName,
				TimeoutSeconds = 300,
				TargetHealthState = responderTargetState,
				Enabled = enabled,
				TargetResource = targetServerName
			};
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(CollectAndMergeResponder).FullName;
	}
}
