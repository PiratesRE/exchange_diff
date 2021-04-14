using System;
using System.Configuration;
using System.Xml.Linq;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal sealed class AppConfig : IAppConfiguration
	{
		internal AppConfig()
		{
		}

		public int MaxConcurrentSubmissions { get; private set; }

		public bool IsWriteToPickupFolderEnabled { get; private set; }

		public TimeSpan HangDetectionInterval { get; private set; }

		public TimeSpan SmtpOutWaitTimeOut { get; private set; }

		public bool ShouldLogTemporaryFailures { get; private set; }

		public bool ShouldLogNotifyEvents { get; private set; }

		public bool UseLocalHubOnly { get; private set; }

		public bool EnableCalendarHeaderCreation { get; private set; }

		public bool EnableSeriesMessageProcessing { get; private set; }

		public bool EnableUnparkedMessageRestoring { get; private set; }

		public bool EnableMailboxQuarantine { get; private set; }

		public int MailboxQuarantineCrashCountThreshold { get; private set; }

		public TimeSpan MailboxQuarantineCrashCountWindow { get; private set; }

		public TimeSpan MailboxQuarantineSpan { get; private set; }

		public string PoisonRegistryEntryLocation { get; private set; }

		public TimeSpan PoisonRegistryEntryExpiryWindow { get; private set; }

		public int PoisonRegistryEntryMaxCount { get; private set; }

		public bool SenderRateDeprioritizationEnabled { get; private set; }

		public int SenderRateDeprioritizationThreshold { get; private set; }

		public bool SenderRateThrottlingEnabled { get; private set; }

		public int SenderRateThrottlingThreshold { get; private set; }

		public TimeSpan SenderRateThrottlingRetryInterval { get; private set; }

		public bool EnableSendNdrForPoisonMessage { get; private set; }

		public TimeSpan ServiceHeartbeatPeriodicLoggingInterval { get; private set; }

		public static IAppConfiguration Load()
		{
			return new AppConfig
			{
				MaxConcurrentSubmissions = AppConfig.GetConfigInt(AppConfig.maxConcurrentSubmissions, 1, int.MaxValue, 5),
				IsWriteToPickupFolderEnabled = AppConfig.GetConfigBool(AppConfig.writeToPickupFolderEnabled, false),
				HangDetectionInterval = AppConfig.GetConfigTimeSpan(AppConfig.hangDetectionInterval, TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromMinutes(5.0)),
				SmtpOutWaitTimeOut = AppConfig.GetConfigTimeSpan(AppConfig.smtpOutWaitTimeOut, TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromMinutes(15.0)),
				ShouldLogTemporaryFailures = AppConfig.GetConfigBool(AppConfig.logTemporaryFailures, true),
				ShouldLogNotifyEvents = AppConfig.GetConfigBool(AppConfig.logNotifyEvents, true),
				UseLocalHubOnly = AppConfig.GetConfigBool(AppConfig.useLocalHubOnly, false),
				EnableCalendarHeaderCreation = AppConfig.GetConfigBool(AppConfig.enableCalendarHeaderCreation, true),
				EnableSeriesMessageProcessing = AppConfig.GetConfigBool(AppConfig.enableSeriesMessageProcessing, true),
				EnableUnparkedMessageRestoring = AppConfig.GetConfigBool(AppConfig.enableUnparkedMessageRestoring, true),
				EnableMailboxQuarantine = AppConfig.GetConfigBool(AppConfig.enableMailboxQuarantine, false),
				MailboxQuarantineCrashCountThreshold = AppConfig.GetConfigInt(AppConfig.mailboxQuarantineCrashCountThreshold, 1, int.MaxValue, 8),
				MailboxQuarantineCrashCountWindow = AppConfig.GetConfigTimeSpan(AppConfig.mailboxQuarantineCrashCountWindow, TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.FromHours(4.0)),
				MailboxQuarantineSpan = AppConfig.GetConfigTimeSpan(AppConfig.mailboxQuarantineSpan, TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.FromHours(24.0)),
				PoisonRegistryEntryLocation = AppConfig.GetConfigString(AppConfig.poisonRegistryEntryLocation, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\StoreDriver\\Submission"),
				PoisonRegistryEntryExpiryWindow = AppConfig.GetConfigTimeSpan(AppConfig.poisonRegistryEntryExpiryWindow, TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.FromDays(7.0)),
				PoisonRegistryEntryMaxCount = AppConfig.GetConfigInt(AppConfig.poisonRegistryEntryMaxCount, 1, int.MaxValue, 100),
				SenderRateDeprioritizationEnabled = AppConfig.GetConfigBool("SenderRateDeprioritizationEnabled", true),
				SenderRateDeprioritizationThreshold = AppConfig.GetConfigInt("SenderRateDeprioritizationThreshold", 2, 100000, 30),
				SenderRateThrottlingEnabled = AppConfig.GetConfigBool("SenderRateThrottlingEnabled", true),
				SenderRateThrottlingThreshold = AppConfig.GetConfigInt("SenderRateThrottlingThreshold", 2, 100000, 30),
				SenderRateThrottlingRetryInterval = AppConfig.GetConfigTimeSpan("SenderRateThrottlingRetryInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromHours(12.0), TimeSpan.FromSeconds(30.0)),
				EnableSendNdrForPoisonMessage = AppConfig.GetConfigBool(AppConfig.enableSendNdrForPoisonMessage, true),
				ServiceHeartbeatPeriodicLoggingInterval = AppConfig.GetConfigTimeSpan(AppConfig.serviceHeartbeatPeriodicLoggingInterval, TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.FromMinutes(5.0))
			};
		}

		public void AddDiagnosticInfo(XElement parent)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			parent.Add(new object[]
			{
				new XElement(AppConfig.hangDetectionInterval, this.HangDetectionInterval),
				new XElement(AppConfig.writeToPickupFolderEnabled, this.IsWriteToPickupFolderEnabled),
				new XElement(AppConfig.logNotifyEvents, this.ShouldLogNotifyEvents),
				new XElement(AppConfig.logTemporaryFailures, this.ShouldLogTemporaryFailures),
				new XElement(AppConfig.maxConcurrentSubmissions, this.MaxConcurrentSubmissions),
				new XElement(AppConfig.useLocalHubOnly, this.UseLocalHubOnly),
				new XElement(AppConfig.smtpOutWaitTimeOut, this.SmtpOutWaitTimeOut),
				new XElement(AppConfig.enableCalendarHeaderCreation, this.EnableCalendarHeaderCreation),
				new XElement(AppConfig.enableMailboxQuarantine, this.EnableMailboxQuarantine),
				new XElement(AppConfig.poisonRegistryEntryExpiryWindow, this.PoisonRegistryEntryExpiryWindow),
				new XElement(AppConfig.poisonRegistryEntryMaxCount, this.PoisonRegistryEntryMaxCount),
				new XElement(AppConfig.senderRateDeprioritizationEnabled, this.SenderRateDeprioritizationEnabled),
				new XElement(AppConfig.senderRateDeprioritizationThreshold, this.SenderRateDeprioritizationThreshold),
				new XElement(AppConfig.senderRateThrottlingEnabled, this.SenderRateThrottlingEnabled),
				new XElement(AppConfig.senderRateThrottlingThreshold, this.SenderRateThrottlingThreshold),
				new XElement(AppConfig.senderRateThrottlingRetryInterval, this.SenderRateThrottlingRetryInterval),
				new XElement(AppConfig.serviceHeartbeatPeriodicLoggingInterval, this.ServiceHeartbeatPeriodicLoggingInterval),
				new XElement(AppConfig.poisonRegistryEntryLocation, this.PoisonRegistryEntryLocation),
				new XElement(AppConfig.enableSendNdrForPoisonMessage, this.EnableSendNdrForPoisonMessage),
				new XElement(AppConfig.mailboxQuarantineCrashCountThreshold, this.MailboxQuarantineCrashCountThreshold),
				new XElement(AppConfig.mailboxQuarantineCrashCountWindow, this.MailboxQuarantineCrashCountWindow),
				new XElement(AppConfig.mailboxQuarantineSpan, this.MailboxQuarantineSpan)
			});
		}

		private static bool GetConfigBool(string label, bool defaultValue)
		{
			bool result;
			try
			{
				result = TransportAppConfig.GetConfigBool(label, defaultValue);
			}
			catch (ConfigurationErrorsException)
			{
				result = defaultValue;
			}
			return result;
		}

		private static int GetConfigInt(string label, int minimumValue, int maximumValue, int defaultValue)
		{
			int result;
			try
			{
				result = TransportAppConfig.GetConfigInt(label, minimumValue, maximumValue, defaultValue);
			}
			catch (ConfigurationErrorsException)
			{
				result = defaultValue;
			}
			return result;
		}

		private static string GetConfigString(string label, string defaultValue)
		{
			string result;
			try
			{
				result = TransportAppConfig.GetConfigString(label, defaultValue);
			}
			catch (ConfigurationErrorsException)
			{
				result = defaultValue;
			}
			return result;
		}

		private static TimeSpan GetConfigTimeSpan(string label, TimeSpan min, TimeSpan max, TimeSpan defaultValue)
		{
			TimeSpan result;
			try
			{
				result = TransportAppConfig.GetConfigValue<TimeSpan>(label, min, max, defaultValue, new TransportAppConfig.TryParse<TimeSpan>(TimeSpan.TryParse));
			}
			catch (ConfigurationErrorsException)
			{
				result = defaultValue;
			}
			return result;
		}

		private static string writeToPickupFolderEnabled = "WriteToPickupFolderEnabled";

		private static string hangDetectionInterval = "HangDetectionInterval";

		private static string smtpOutWaitTimeOut = "SmtpOutWaitTimeOut";

		private static string logTemporaryFailures = "LogTemporaryFailures";

		private static string logNotifyEvents = "LogNotifyEvents";

		private static string maxConcurrentSubmissions = "MaxConcurrentSubmissions";

		private static string useLocalHubOnly = "UseLocalHubOnly";

		private static string enableCalendarHeaderCreation = "EnableCalendarHeaderCreation";

		private static string enableSeriesMessageProcessing = "EnableSeriesMessageProcessing";

		private static string enableUnparkedMessageRestoring = "EnableUnparkedMessageRestoring";

		private static string enableMailboxQuarantine = "EnableMailboxQuarantine";

		private static string mailboxQuarantineCrashCountThreshold = "MailboxQuarantineCrashCountThreshold";

		private static string mailboxQuarantineCrashCountWindow = "MailboxQuarantineCrashCountWindow";

		private static string mailboxQuarantineSpan = "MailboxQuarantineSpan";

		private static string poisonRegistryEntryLocation = "PoisonRegistryEntryLocation";

		private static string poisonRegistryEntryExpiryWindow = "PoisonRegistryEntryExpiryWindow";

		private static string poisonRegistryEntryMaxCount = "PoisonRegistryEntryMaxCount";

		private static string senderRateDeprioritizationEnabled = "SenderRateDeprioritizationEnabled";

		private static string senderRateDeprioritizationThreshold = "SenderRateDeprioritizationThreshold";

		private static string senderRateThrottlingEnabled = "SenderRateThrottlingEnabled";

		private static string senderRateThrottlingThreshold = "SenderRateThrottlingThreshold";

		private static string senderRateThrottlingRetryInterval = "SenderRateThrottlingRetryInterval";

		private static string enableSendNdrForPoisonMessage = "EnableSendNdrForPoisonMessage";

		private static string serviceHeartbeatPeriodicLoggingInterval = "ServiceHeartbeatPeriodicLoggingInterval";
	}
}
