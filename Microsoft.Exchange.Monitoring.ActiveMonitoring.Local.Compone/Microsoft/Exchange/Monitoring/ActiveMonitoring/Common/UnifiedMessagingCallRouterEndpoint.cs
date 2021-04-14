using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class UnifiedMessagingCallRouterEndpoint : UnifiedMessagingEndpoint
	{
		public override bool DetectChange()
		{
			Server server = base.TopologyConfigurationSession.FindServerByName(Environment.MachineName);
			if (server != null && server.IsCafeServer)
			{
				WTFDiagnostics.TraceFunction(ExTraceGlobals.UnifiedMessagingEndpointTracer, base.TraceContext, "Detecting Changes on UMCallRouter configuration", null, "DetectChange", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\UnifiedMessagingCallRouterEndpoint.cs", 39);
				return base.HasAnyUMPropertyChanged(server);
			}
			return false;
		}

		public override void Initialize()
		{
			base.TopologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 53, "Initialize", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\UnifiedMessagingCallRouterEndpoint.cs");
			base.Server = base.TopologyConfigurationSession.FindServerByName(Environment.MachineName);
			if (base.Server == null || !base.Server.IsCafeServer)
			{
				return;
			}
			UMStartupMode startupMode;
			int sipTcpListeningPort;
			int sipTlsListeningPort;
			string certificateThumbprint;
			this.GetUMPropertiesFromAD(base.Server, out startupMode, out sipTcpListeningPort, out sipTlsListeningPort, out certificateThumbprint);
			base.StartupMode = startupMode;
			base.SipTcpListeningPort = sipTcpListeningPort;
			base.SipTlsListeningPort = sipTlsListeningPort;
			base.CertificateThumbprint = certificateThumbprint;
			base.CertificateSubjectName = base.GetCertificateSubjectNameFromThumbprint(certificateThumbprint);
		}

		protected override void GetUMPropertiesFromAD(Server server, out UMStartupMode startupMode, out int sipTcpListeningPort, out int sipTlsListeningPort, out string certificateThumbprint)
		{
			sipTcpListeningPort = 0;
			sipTlsListeningPort = 0;
			certificateThumbprint = null;
			startupMode = UMStartupMode.TCP;
			if (server != null && server.IsCafeServer)
			{
				SIPFEServerConfiguration sipfeserverConfiguration = SIPFEServerConfiguration.Find();
				sipTcpListeningPort = sipfeserverConfiguration.SipTcpListeningPort;
				sipTlsListeningPort = sipfeserverConfiguration.SipTlsListeningPort;
				startupMode = sipfeserverConfiguration.UMStartupMode;
				certificateThumbprint = sipfeserverConfiguration.UMCertificateThumbprint;
			}
		}
	}
}
