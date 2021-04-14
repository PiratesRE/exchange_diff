using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Transport
{
	public class CrossPremiseMailFlowProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
				PSSnapInException ex = null;
				runspaceConfiguration.AddPSSnapIn("Microsoft.Exchange.Management.Powershell.E2010", out ex);
				if (ex != null)
				{
					WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.CrossPremiseTracer, base.TraceContext, "Non-fatal error occurred while adding the powerShell snap-in - {0}. Warning: {1}", "Microsoft.Exchange.Management.Powershell.E2010", ex.Message, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Transport\\Probes\\CrossPremiseMailFlowProbe.cs", 52);
				}
				Command command = new Command("Test-Mailflow");
				command.Parameters.Add("MonitoringContext", true);
				command.Parameters.Add("CrossPremises", true);
				using (Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration))
				{
					runspace.Open();
					Pipeline pipeline = runspace.CreatePipeline();
					pipeline.Commands.Add(command);
					Collection<PSObject> collection = pipeline.Invoke();
					StringBuilder aggregateErrorMessages = new StringBuilder();
					foreach (PSObject psobject in collection)
					{
						MonitoringEventCollection monitoringEventCollection = psobject.Properties["Events"].Value as MonitoringEventCollection;
						if (monitoringEventCollection != null)
						{
							monitoringEventCollection.FindAll((MonitoringEvent e) => e.EventType != EventTypeEnumeration.Success).ForEach(delegate(MonitoringEvent e)
							{
								aggregateErrorMessages.AppendLine(e.EventMessage);
							});
						}
					}
					if (aggregateErrorMessages.Length != 0)
					{
						throw new ApplicationException(aggregateErrorMessages.ToString());
					}
				}
			}
			catch (ApplicationException ex2)
			{
				WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CrossPremiseTracer, base.TraceContext, "The Test-Mailflow cmdlet reported an error: {0}", ex2.Message, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Transport\\Probes\\CrossPremiseMailFlowProbe.cs", 91);
				throw;
			}
			catch (Exception ex3)
			{
				WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CrossPremiseTracer, base.TraceContext, "Failed to run the Test-Mailflow cmdlet. Exception: {0}", ex3.ToString(), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Transport\\Probes\\CrossPremiseMailFlowProbe.cs", 100);
				throw;
			}
		}

		private const string SnapIn = "Microsoft.Exchange.Management.Powershell.E2010";
	}
}
