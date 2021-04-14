using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class PerfCounterRecentPartnerTranscriptionFailedMonitorAndResponder : IUMLocalMonitoringMonitorAndResponder
	{
		public void InitializeMonitorAndResponder(IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			UMPerfCounterUtils.InstantiatePerfCountersBasedUrgentAlerts(50, 2, "MSExchangeUMAvailability\\% of Partner Voice Message Transcription Failures Over the Last Hour", PerfCounterRecentPartnerTranscriptionFailedMonitorAndResponder.RecentPartnerTranscriptionFailedMonitorName, PerfCounterRecentPartnerTranscriptionFailedMonitorAndResponder.RecentPartnerTranscriptionFailedResponderName, ExchangeComponent.UMProtocol, PerfCounterRecentPartnerTranscriptionFailedMonitorAndResponder.RecentPartnerTranscriptionFailedEscalationMessageString, 0, broker, traceContext, NotificationServiceClass.Scheduled);
		}

		private const int RecentPartnerTranscriptionFailedPercentThreshold = 50;

		private const int RecentPartnerTranscriptionFailedNumberOfSamples = 2;

		private const string RecentPartnerTranscriptionFailedPerfCounterName = "MSExchangeUMAvailability\\% of Partner Voice Message Transcription Failures Over the Last Hour";

		private const int RecentPartnerTranscriptionFailedTransitionToUnhealthySecs = 0;

		private static readonly string RecentPartnerTranscriptionFailedMonitorName = "UMServiceRecentPartnerTranscriptionFailedMonitor";

		private static readonly string RecentPartnerTranscriptionFailedResponderName = "UMServiceRecentPartnerTranscriptionFailedEscalate";

		private static readonly string RecentPartnerTranscriptionFailedEscalationMessageString = Strings.UMRecentPartnerTranscriptionFailedEscalationMessageString(50);
	}
}
