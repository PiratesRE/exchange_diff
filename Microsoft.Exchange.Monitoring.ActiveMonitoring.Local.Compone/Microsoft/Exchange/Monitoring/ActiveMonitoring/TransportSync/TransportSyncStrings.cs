using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync
{
	internal static class TransportSyncStrings
	{
		internal const string DatabaseConsistencyProbeName = "TransportSyncManager.DatabaseConsistency.Probe";

		internal const string DatabaseConsistencyMonitorName = "TransportSyncManager.DatabaseConsistency.Monitor";

		internal const string DatabaseConsistencyRestartResponderName = "TransportSyncManager.DatabaseConsistency.Restart";

		internal const string DatabaseConsistencyEscalateResponderName = "TransportSyncManager.DatabaseConsistency.Escalate";

		internal const string ServiceAvailabilityProbeName = "TransportSyncManager.Started.Probe";

		internal const string ServiceAvailabilityMonitorName = "TransportSyncManager.Started.Monitor";

		internal const string ServiceAvailabilityEscalateResponderName = "TransportSyncManager.Service.Escalate";

		internal const string ServiceAvailabilityRestartResponderName = "TransportSyncManager.Service.Restart";

		internal const string SubscriptionSlaMissedMonitorName = "TransportSync.NotDispatchingWithin1HourSla.Monitor";

		internal const string SubscriptionSlaMissedResponderName = "TransportSync.NotDispatchingWithin1HourSla.Escalate";

		internal const string DeltaSyncEndpointUnreachableMonitorName = "DeltaSync.EndpointUnreachable.Monitor";

		internal const string DeltaSyncEndpointUnreachableResponderName = "DeltaSync.EndpointUnreachable.Escalate";

		internal const string DeltaSyncPartnerAuthenticationFailedMonitorName = "DeltaSync.PartnerAuthentication.Failed.Monitor";

		internal const string DeltaSyncPartnerAuthenticationFailedResponderName = "DeltaSync.PartnerAuthentication.Failed.Escalate";

		internal const string DeltaSyncServiceEndpointsLoadFailedMonitorName = "DeltaSync.ServiceEndpointsLoad.Failed.Monitor";

		internal const string DeltaSyncServiceEndpointsLoadFailedResponderName = "DeltaSync.ServiceEndpointsLoad.Failed.Escalate";

		internal const string RegistryAccessDeniedMonitorName = "Registry.AccessDenied.Monitor";

		internal const string RegistryAccessDeniedResponderName = "Registry.AccessDenied.Escalate";
	}
}
