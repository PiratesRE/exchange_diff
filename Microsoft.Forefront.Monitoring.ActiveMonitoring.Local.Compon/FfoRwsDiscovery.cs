using System;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	internal sealed class FfoRwsDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				ExchangeServerRoleEndpoint exchangeServerRoleEndpoint = LocalEndpointManager.Instance.ExchangeServerRoleEndpoint;
				if (exchangeServerRoleEndpoint == null || (!FfoRwsDiscovery.SupportFfoReportingCmdlets(exchangeServerRoleEndpoint) && !FfoRwsDiscovery.SupportFfoReportingWebService(exchangeServerRoleEndpoint)))
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, FfoRwsDiscovery.traceContext, "[FFO RwsDiscovery.DoWork]: This server does not support FFO Reporting cmdlets or FFO Reporting WS.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Rws\\FFORwsDiscovery.cs", 47);
					base.Result.StateAttribute1 = "FFO RwsDiscovery: This server does not support FFO Reporting cmdlets or FFO Reporting WS.";
				}
				else
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, FfoRwsDiscovery.traceContext, "[FFO RwsDiscovery.DoWork]: Discovery Started.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Rws\\FFORwsDiscovery.cs", 57);
					XmlNode definitionNode = GenericWorkItemHelper.GetDefinitionNode("FfoRws.xml", FfoRwsDiscovery.traceContext);
					GenericWorkItemHelper.CreatePerfCounterDefinitions(definitionNode, base.Broker, FfoRwsDiscovery.traceContext, base.Result);
					GenericWorkItemHelper.CreateNTEventDefinitions(definitionNode, base.Broker, FfoRwsDiscovery.traceContext, base.Result);
					if (FfoRwsDiscovery.SupportFfoReportingWebService(exchangeServerRoleEndpoint))
					{
						GenericWorkItemHelper.CreateCustomDefinitions(definitionNode, base.Broker, FfoRwsDiscovery.traceContext, base.Result);
					}
					else
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, FfoRwsDiscovery.traceContext, "[FFO RwsDiscovery.DoWork]: Custom Probes/Monitors/Responders are not generated. This server does not support FFO Reporting WS.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Rws\\FFORwsDiscovery.cs", 80);
					}
					GenericWorkItemHelper.CompleteDiscovery(FfoRwsDiscovery.traceContext);
					WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, FfoRwsDiscovery.traceContext, "[FFO RwsDiscovery.DoWork]: Discovery Completed.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Rws\\FFORwsDiscovery.cs", 89);
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.RWSTracer, FfoRwsDiscovery.traceContext, "[FFO RwsDiscovery.DoWork]: Exception occurred during discovery. {0}", ex.ToString(), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Rws\\FFORwsDiscovery.cs", 96);
				throw;
			}
		}

		private static bool SupportFfoReportingWebService(ExchangeServerRoleEndpoint exchangeEndpoint)
		{
			if (!DatacenterRegistry.IsForefrontForOffice())
			{
				return exchangeEndpoint.IsClientAccessRoleInstalled;
			}
			return FfoLocalEndpointManager.IsWebServiceInstalled;
		}

		private static bool SupportFfoReportingCmdlets(ExchangeServerRoleEndpoint exchangeEndpoint)
		{
			return exchangeEndpoint.IsCafeRoleInstalled;
		}

		private static TracingContext traceContext = new TracingContext();
	}
}
