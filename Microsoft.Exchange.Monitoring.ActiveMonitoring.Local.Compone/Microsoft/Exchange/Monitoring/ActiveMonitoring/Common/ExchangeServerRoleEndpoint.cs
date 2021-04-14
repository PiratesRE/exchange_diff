using System;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class ExchangeServerRoleEndpoint : IEndpoint
	{
		public bool IsBridgeheadRoleInstalled
		{
			get
			{
				return this.isBridgeheadRoleInstalled;
			}
		}

		public bool IsGatewayRoleInstalled
		{
			get
			{
				return this.isGatewayRoleInstalled;
			}
		}

		public bool IsClientAccessRoleInstalled
		{
			get
			{
				return this.isClientAccessRoleInstalled;
			}
		}

		public bool IsMailboxRoleInstalled
		{
			get
			{
				return this.isMailboxRoleInstalled;
			}
		}

		public bool IsUnifiedMessagingRoleInstalled
		{
			get
			{
				return this.isUnifiedMessagingRoleInstalled;
			}
		}

		public bool IsFrontendTransportRoleInstalled
		{
			get
			{
				return this.isFrontendTransportRoleInstalled;
			}
		}

		public bool IsAdminToolsRoleInstalled
		{
			get
			{
				return this.isAdminToolsRoleInstalled;
			}
		}

		public bool IsMonitoringRoleInstalled
		{
			get
			{
				return this.isMonitoringRoleInstalled;
			}
		}

		public bool IsCentralAdminRoleInstalled
		{
			get
			{
				return this.isCentralAdminRoleInstalled;
			}
		}

		public bool IsCentralAdminDatabaseRoleInstalled
		{
			get
			{
				return this.isCentralAdminDatabaseRoleInstalled;
			}
		}

		public bool IsCentralAdminFrontEndRoleInstalled
		{
			get
			{
				return this.isCentralAdminFrontEndRoleInstalled;
			}
		}

		public bool IsLanguangePacksRoleInstalled
		{
			get
			{
				return this.isLanguangePacksRoleInstalled;
			}
		}

		public bool IsCafeRoleInstalled
		{
			get
			{
				return this.isCafeRoleInstalled;
			}
		}

		public bool IsFfoWebServiceRoleInstalled
		{
			get
			{
				return this.isFfoWebServiceRoleInstalled;
			}
		}

		public bool RestartOnChange
		{
			get
			{
				return true;
			}
		}

		public Exception Exception { get; set; }

		public void Initialize()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.ExchangeServerRoleEndpointTracer, this.traceContext, "Checking Exchange server role configuration", null, "Initialize", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\ExchangeServerRoleEndpoint.cs", 296);
			this.isBridgeheadRoleInstalled = new BridgeheadRole().IsInstalled;
			this.isGatewayRoleInstalled = new GatewayRole().IsInstalled;
			this.isClientAccessRoleInstalled = new ClientAccessRole().IsInstalled;
			this.isMailboxRoleInstalled = new MailboxRole().IsInstalled;
			this.isUnifiedMessagingRoleInstalled = new UnifiedMessagingRole().IsInstalled;
			this.isFrontendTransportRoleInstalled = new FrontendTransportRole().IsInstalled;
			this.isAdminToolsRoleInstalled = new AdminToolsRole().IsInstalled;
			this.isMonitoringRoleInstalled = new MonitoringRole().IsInstalled;
			this.isCentralAdminRoleInstalled = new CentralAdminRole().IsInstalled;
			this.isCentralAdminDatabaseRoleInstalled = new CentralAdminDatabaseRole().IsInstalled;
			this.isCentralAdminFrontEndRoleInstalled = new CentralAdminFrontEndRole().IsInstalled;
			this.isLanguangePacksRoleInstalled = new LanguagePacksRole().IsInstalled;
			this.isCafeRoleInstalled = new CafeRole().IsInstalled;
			this.isFfoWebServiceRoleInstalled = new FfoWebServiceRole().IsInstalled;
		}

		public bool DetectChange()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.ExchangeServerRoleEndpointTracer, this.traceContext, "Detecting Exchange server role configuration", null, "DetectChange", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\ExchangeServerRoleEndpoint.cs", 319);
			return this.isBridgeheadRoleInstalled != new BridgeheadRole().IsInstalled || this.isGatewayRoleInstalled != new GatewayRole().IsInstalled || this.isClientAccessRoleInstalled != new ClientAccessRole().IsInstalled || this.isMailboxRoleInstalled != new MailboxRole().IsInstalled || this.isUnifiedMessagingRoleInstalled != new UnifiedMessagingRole().IsInstalled || this.isFrontendTransportRoleInstalled != new FrontendTransportRole().IsInstalled || this.isAdminToolsRoleInstalled != new AdminToolsRole().IsInstalled || this.isMonitoringRoleInstalled != new MonitoringRole().IsInstalled || this.isCentralAdminRoleInstalled != new CentralAdminRole().IsInstalled || this.isCentralAdminDatabaseRoleInstalled != new CentralAdminDatabaseRole().IsInstalled || this.isCentralAdminFrontEndRoleInstalled != new CentralAdminFrontEndRole().IsInstalled || this.isLanguangePacksRoleInstalled != new LanguagePacksRole().IsInstalled || this.isCafeRoleInstalled != new CafeRole().IsInstalled || this.isFfoWebServiceRoleInstalled != new FfoWebServiceRole().IsInstalled;
		}

		private bool isBridgeheadRoleInstalled;

		private bool isGatewayRoleInstalled;

		private bool isClientAccessRoleInstalled;

		private bool isMailboxRoleInstalled;

		private bool isUnifiedMessagingRoleInstalled;

		private bool isFrontendTransportRoleInstalled;

		private bool isAdminToolsRoleInstalled;

		private bool isMonitoringRoleInstalled;

		private bool isCentralAdminRoleInstalled;

		private bool isCentralAdminDatabaseRoleInstalled;

		private bool isCentralAdminFrontEndRoleInstalled;

		private bool isLanguangePacksRoleInstalled;

		private bool isCafeRoleInstalled;

		private bool isFfoWebServiceRoleInstalled;

		private TracingContext traceContext = TracingContext.Default;
	}
}
