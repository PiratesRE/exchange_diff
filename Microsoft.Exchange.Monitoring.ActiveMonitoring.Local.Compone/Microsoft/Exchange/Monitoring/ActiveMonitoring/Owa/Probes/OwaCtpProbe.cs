using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Owa.Probes
{
	public class OwaCtpProbe : OwaLocalLogonProbe
	{
		protected override string UserAgent
		{
			get
			{
				return "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1; MSEXCHMON; ACTIVEMONITORING; OWACTP)";
			}
		}

		public override void PopulateDefinition<TDefinition>(TDefinition definition, Dictionary<string, string> propertyBag)
		{
			if (!LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsClientAccessRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaCtpProbe.PopulateDefinition: clientaccess role not found on this server", null, "PopulateDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaCtpProbe.cs", 49);
				throw new ArgumentException(Strings.OwaClientAccessRoleNotInstalled);
			}
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaCtpProbe.PopulateDefinition: endpointManager.MailboxDatabaseEndpoint is null", null, "PopulateDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaCtpProbe.cs", 60);
				throw new ArgumentException(Strings.OwaNoMailboxesAvailable);
			}
			if (instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaCtpProbe.PopulateDefinition: endpointManager.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe is null or the list is empty", null, "PopulateDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaCtpProbe.cs", 70);
				throw new ArgumentException(Strings.OwaNoMailboxesAvailable);
			}
			ProbeDefinition probeDefinition = definition as ProbeDefinition;
			MailboxDatabaseInfo mailboxDatabaseInfo = this.GetMailboxDatabaseInfo(probeDefinition, instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe, propertyBag);
			string valueOrDefault = propertyBag.GetValueOrDefault("Endpoint", "https://localhost/owa/");
			OwaDiscovery.PopulateOwaClientAccessProbeDefinition(probeDefinition, mailboxDatabaseInfo, TimeSpan.Zero, valueOrDefault);
		}

		private MailboxDatabaseInfo GetMailboxDatabaseInfo(ProbeDefinition probeDefinition, ICollection<MailboxDatabaseInfo> mailboxDatabases, Dictionary<string, string> propertyBag)
		{
			string valueOrDefault = propertyBag.GetValueOrDefault("Account", null);
			string valueOrDefault2 = propertyBag.GetValueOrDefault("Password", null);
			if (valueOrDefault != null && valueOrDefault.Contains("@") && valueOrDefault2 != null)
			{
				return new MailboxDatabaseInfo
				{
					MonitoringAccount = valueOrDefault.Substring(0, valueOrDefault.IndexOf('@')),
					MonitoringAccountDomain = valueOrDefault.Substring(valueOrDefault.IndexOf('@') + 1),
					MonitoringAccountPassword = valueOrDefault2,
					MailboxDatabaseName = string.Empty
				};
			}
			return base.GetMailboxDatabaseInfo(probeDefinition, mailboxDatabases);
		}
	}
}
