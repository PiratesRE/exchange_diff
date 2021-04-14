using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Net.MonitoringWebClient;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Owa.Probes
{
	public class OwaHealthCheckProbe : OwaBaseProbe
	{
		protected override string UserAgent
		{
			get
			{
				return "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1; MSEXCHMON; ACTIVEMONITORING; OWASELFTEST)";
			}
		}

		internal static ITestStep CreateScenario(ProbeDefinition probeDefinition, Uri targetUri)
		{
			ITestFactory testFactory = new TestFactory();
			return testFactory.CreateOwaHealthCheckScenario(targetUri, testFactory);
		}

		internal override ITestStep CreateScenario(Uri targetUri)
		{
			return OwaHealthCheckProbe.CreateScenario(base.Definition, targetUri);
		}

		public override void PopulateDefinition<TDefinition>(TDefinition definition, Dictionary<string, string> propertyBag)
		{
			if (!LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaHealthCheckProbe.PopulateDefinition: mailbox role not found on this server", null, "PopulateDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaHealthCheckProbe.cs", 74);
				throw new ArgumentException(Strings.OwaMailboxRoleNotInstalled);
			}
			OwaDiscovery.PopulateOwaProtocolProbeDefinition(definition as ProbeDefinition);
			definition.RecurrenceIntervalSeconds = 0;
		}
	}
}
