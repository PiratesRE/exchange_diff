using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal sealed class UMDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			try
			{
				ExAssert.RetailAssert(instance.ExchangeServerRoleEndpoint != null, "Error: ExchangeServerRoleEndpoint should not be null");
				if (instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
				{
					UMSipOptionsDiscoveryUtils.InstantiateSipOptionsMonitoringForUMCallRouterService(base.Broker, base.TraceContext);
					this.InitializeUMMonitorsAndResponders(UMDiscovery.umCallRouterMonitorsAndResponders);
				}
				if (instance.ExchangeServerRoleEndpoint.IsUnifiedMessagingRoleInstalled)
				{
					UMSipOptionsDiscoveryUtils.InstantiateSipOptionsMonitoringForUMService(base.Broker, base.TraceContext);
					this.InitializeUMMonitorsAndResponders(UMDiscovery.umServiceMonitorsAndResponders);
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, base.TraceContext, "UMDiscovery.DoWork: EndpointManagerEndpointUninitializedException is caught.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\UM\\Discovery\\UMDiscovery.cs", 92);
			}
		}

		private void InitializeUMMonitorsAndResponders(IUMLocalMonitoringMonitorAndResponder[] umAlertTypes)
		{
			foreach (IUMLocalMonitoringMonitorAndResponder iumlocalMonitoringMonitorAndResponder in umAlertTypes)
			{
				iumlocalMonitoringMonitorAndResponder.InitializeMonitorAndResponder(base.Broker, base.TraceContext);
			}
		}

		private static IUMLocalMonitoringMonitorAndResponder[] umCallRouterMonitorsAndResponders = new IUMLocalMonitoringMonitorAndResponder[]
		{
			new PerfCounterRecentMissedCallNotificationProxyFailedMonitorAndResponder(),
			new UMCallRouterCertificateNearExpiryMonitorAndResponder()
		};

		private static IUMLocalMonitoringMonitorAndResponder[] umServiceMonitorsAndResponders = new IUMLocalMonitoringMonitorAndResponder[]
		{
			new UMPipelineFullMonitorAndResponder(),
			new MediaEstablishedStatusFailedMonitorAndResponder(),
			new MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedMonitorAndResponder(),
			new MediaEdgeResourceAllocationFailedMonitorAndResponder(),
			new PerfCounterUMPipelineSLAMonitorAndResponder(),
			new UMCertificateNearExpiryMonitorAndResponder(),
			new UMProtectedVoiceMessageEncryptDecryptFailedMonitorAndResponder(),
			new PerfCounterRecentPartnerTranscriptionFailedMonitorAndResponder(),
			new UMGrammarUsageMonitorAndResponder(),
			new UMTranscriptionThrottledMonitorAndResponder()
		};
	}
}
