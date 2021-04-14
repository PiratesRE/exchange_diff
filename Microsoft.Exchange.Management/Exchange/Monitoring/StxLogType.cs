using System;

namespace Microsoft.Exchange.Monitoring
{
	internal enum StxLogType
	{
		TestLiveIdAuthentication,
		TestNtlmConnectivity,
		TestActiveDirectoryConnectivity,
		TestTopologyService,
		TestGlobalLocatorService,
		TestForwardFullSync,
		TestForwardSyncCookie,
		TestForwardSyncCookieResponder,
		TestForwardSyncCompanyProbe,
		TestForwardSyncCompanyResponder,
		DatabaseAvailability,
		TestRidMonitor,
		TestRidSetMonitor,
		TestActiveDirectorySelfCheck,
		TenantRelocationErrorMonitor,
		SharedConfigurationTenantMonitor,
		TestActivedirectoryConnectivityForConfigDC,
		SyntheticReplicationTransaction,
		SyntheticReplicationMonitor,
		PassiveReplicationMonitor,
		PassiveADReplicationMonitor,
		PassiveReplicationPerfCounterProbe,
		RemoteDomainControllerStateProbe,
		TrustMonitorProbe,
		TestKDCService,
		TestDoMTConnectivity,
		TestOfflineGLS
	}
}
