using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal interface IAppConfiguration
	{
		bool IsWriteToPickupFolderEnabled { get; }

		TimeSpan HangDetectionInterval { get; }

		TimeSpan SmtpOutWaitTimeOut { get; }

		int MaxConcurrentSubmissions { get; }

		bool ShouldLogTemporaryFailures { get; }

		bool ShouldLogNotifyEvents { get; }

		bool UseLocalHubOnly { get; }

		bool EnableCalendarHeaderCreation { get; }

		bool EnableMailboxQuarantine { get; }

		int MailboxQuarantineCrashCountThreshold { get; }

		TimeSpan MailboxQuarantineCrashCountWindow { get; }

		TimeSpan MailboxQuarantineSpan { get; }

		TimeSpan PoisonRegistryEntryExpiryWindow { get; }

		string PoisonRegistryEntryLocation { get; }

		int PoisonRegistryEntryMaxCount { get; }

		bool SenderRateDeprioritizationEnabled { get; }

		bool EnableSendNdrForPoisonMessage { get; }

		int SenderRateDeprioritizationThreshold { get; }

		bool SenderRateThrottlingEnabled { get; }

		int SenderRateThrottlingThreshold { get; }

		TimeSpan SenderRateThrottlingRetryInterval { get; }

		bool EnableSeriesMessageProcessing { get; }

		TimeSpan ServiceHeartbeatPeriodicLoggingInterval { get; }

		bool EnableUnparkedMessageRestoring { get; }

		void AddDiagnosticInfo(XElement parent);
	}
}
