using System;
using System.Xml.Linq;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationTestIntegration : TestIntegrationBase
	{
		public MigrationTestIntegration(bool autoRefresh = false) : base("SOFTWARE\\Microsoft\\Exchange_Test\\v15\\MigrationService", autoRefresh)
		{
		}

		public static MigrationTestIntegration Instance
		{
			get
			{
				return MigrationTestIntegration.SingletonInstance;
			}
		}

		public bool IsMigrationNotificationRpcEndpointEnabled
		{
			get
			{
				return base.GetFlagValue("IsMigrationNotificationRpcEndpointEnabled", true);
			}
		}

		public bool IsMigrationServiceRpcEndpointEnabled
		{
			get
			{
				return base.GetFlagValue("SyncMigrationIsMigrationServiceRpcEndpointEnabled", true);
			}
		}

		public bool IsMigrationProxyRpcClientEnabled
		{
			get
			{
				return base.GetFlagValue("MigrationProxyRpcClientEnabled", false);
			}
		}

		public string MigrationAccessorEndpoint
		{
			get
			{
				return base.GetStrValue("SyncMigrationAccessorEndpoint");
			}
		}

		public string MigrationFaultInjectionHandler
		{
			get
			{
				return base.GetStrValue("MigrationFaultInjectionHandler");
			}
		}

		public string ReportMessageEndpoint
		{
			get
			{
				return base.GetStrValue("ReportMessageEndpoint");
			}
		}

		public XElement GetDiagnosticInfo()
		{
			XElement xelement = new XElement("TestIntegration", new XElement("Enabled", base.Enabled));
			if (base.Enabled)
			{
				xelement.Add(new XElement("ReportMessageEndpoint", this.ReportMessageEndpoint));
				xelement.Add(new XElement("MigrationAccessorEndpoint", this.MigrationAccessorEndpoint));
				xelement.Add(new XElement("IsMigrationProxyRpcClientEnabled", this.IsMigrationProxyRpcClientEnabled));
				xelement.Add(new XElement("IsMigrationServiceRpcEndpointEnabled", this.IsMigrationServiceRpcEndpointEnabled));
				xelement.Add(new XElement("IsMigrationNotificationRpcEndpointEnabled", this.IsMigrationNotificationRpcEndpointEnabled));
				xelement.Add(new XElement("MigrationFaultInjectionHandler", this.MigrationFaultInjectionHandler));
			}
			return xelement;
		}

		public const string RegKeyName = "SOFTWARE\\Microsoft\\Exchange_Test\\v15\\MigrationService";

		public const string IsMigrationNotificationRpcEndpointEnabledName = "IsMigrationNotificationRpcEndpointEnabled";

		public const string IsMigrationServiceRpcEndpointEnabledName = "SyncMigrationIsMigrationServiceRpcEndpointEnabled";

		internal const string IsMigrationProxyRpcClientEnabledName = "MigrationProxyRpcClientEnabled";

		internal const string MigrationAccessorEndpointName = "SyncMigrationAccessorEndpoint";

		internal const string MigrationFaultInjectionHandlerName = "MigrationFaultInjectionHandler";

		internal const string ReportMessageEndpointName = "ReportMessageEndpoint";

		private static readonly MigrationTestIntegration SingletonInstance = new MigrationTestIntegration(true);
	}
}
