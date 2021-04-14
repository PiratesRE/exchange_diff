using System;
using System.Configuration;
using System.IO;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class OwaSettingsLoader : OwaSettingsLoaderBase
	{
		internal OwaSettingsLoader()
		{
		}

		internal override void Load()
		{
			base.Load();
			ResourceHealthMonitorManager.Initialize(ResourceHealthComponent.OWA);
			this.CheckIncompatibleTimeoutValues();
			this.ReadInstantMessageSettings();
			this.ReadNotificationSettings();
			this.InitializeMaxBytesInConversationReadingPane();
			MessagePrefetchConfiguration.InitializeSettings();
			this.ReadAndInitializeIMPerfCounterSettings();
			this.ReadAndInitializeExceptionPerfCounterSettings();
		}

		private void CheckIncompatibleTimeoutValues()
		{
			int sessionTimeout = OwaConfigurationManager.Configuration.SessionTimeout;
			if ((double)(sessionTimeout * 60) < Math.Ceiling(1.25 * (double)this.MaxPendingRequestLifeInSeconds))
			{
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_IncompatibleTimeoutSetting, string.Empty, new object[]
				{
					sessionTimeout,
					this.MaxPendingRequestLifeInSeconds,
					300
				});
				throw new OwaInvalidInputException("The timeout setting values for \"UserContextTimeout\" and \"MaxPendingRequestLife\" chosen by the admin are conflicting", null, 0);
			}
		}

		internal void InitializeMaxBytesInConversationReadingPane()
		{
			long maxBytesInConversationReadingPane = Globals.MaxBytesInConversationReadingPane;
			if (maxBytesInConversationReadingPane != -9223372036854775808L)
			{
				Conversation.MaxBytesForConversation = maxBytesInConversationReadingPane;
			}
		}

		private void ReadNotificationSettings()
		{
			string text = ConfigurationManager.AppSettings["PushNotificationsEnabled"];
			string text2 = ConfigurationManager.AppSettings["PullNotificationsEnabled"];
			string text3 = ConfigurationManager.AppSettings["FolderContentNotificationsEnabled"];
			int num;
			if (!string.IsNullOrEmpty(text) && int.TryParse(text, out num))
			{
				if (num > 0)
				{
					this.isPushNotificationsEnabled = true;
				}
				else
				{
					this.isPushNotificationsEnabled = false;
				}
			}
			int num2;
			if (!string.IsNullOrEmpty(text2) && int.TryParse(text2, out num2))
			{
				if (num2 > 0)
				{
					this.isPullNotificationsEnabled = true;
				}
				else
				{
					this.isPullNotificationsEnabled = false;
				}
			}
			int num3;
			if (!string.IsNullOrEmpty(text3) && int.TryParse(text3, out num3))
			{
				if (num3 > 0)
				{
					this.isFolderContentNotificationsEnabled = true;
					return;
				}
				this.isFolderContentNotificationsEnabled = false;
			}
		}

		private void ReadInstantMessageSettings()
		{
			this.activityBasedPresenceDuration = 300000;
			string text = ConfigurationManager.AppSettings["ActivityBasedPresenceDuration"];
			int num;
			if (!string.IsNullOrEmpty(text) && int.TryParse(text, out num) && 0 < num)
			{
				this.activityBasedPresenceDuration = num * 60 * 1000;
			}
			this.ReadOcsServerSettings();
		}

		private void ReadOcsServerSettings()
		{
			string value = ConfigurationManager.AppSettings["EnableIMForOwaPremium"];
			bool flag = false;
			bool flag2;
			if (!string.IsNullOrWhiteSpace(value) && bool.TryParse(value, out flag2))
			{
				flag = flag2;
			}
			if (!flag)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "Globals.ReadOcsServerSettings. OWA Premium Instant Messaging integration is disabled.");
				return;
			}
			int mtlsPortNumber = -1;
			bool flag3 = true;
			bool flag4 = true;
			string text = string.Empty;
			ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "Globals.ReadOcsServerSettings");
			if (OwaConfigurationManager.Configuration != null)
			{
				this.ocsServerName = OwaConfigurationManager.Configuration.InstantMessagingServerName;
				if (string.IsNullOrEmpty(this.ocsServerName))
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "Instant Messaging Server name is set to null or empty on the OWA Virtual Directory object.");
					flag3 = false;
				}
				text = OwaConfigurationManager.Configuration.InstantMessagingCertificateThumbprint;
				if (string.IsNullOrEmpty(text))
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "Instant Messaging Certificate Thumbprint is null or empty on the OWA Virtual Directory object.");
					flag4 = false;
				}
			}
			if (!flag3 || !flag4)
			{
				if (OwaConfigurationManager.Configuration != null && OwaConfigurationManager.Configuration.InstantMessagingType == InstantMessagingTypeOptions.Ocs)
				{
					if (!flag3)
					{
						OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMServerNameInvalid);
					}
					if (!flag4)
					{
						OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMCertificateThumbprintInvalid);
					}
				}
				return;
			}
			InstantMessageCertUtils.GetIMCertInfo(text, out this.certificateIssuer, out this.certificateSerial);
			if (string.IsNullOrEmpty(this.certificateIssuer))
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "IM Certificate Issuer is null.");
				return;
			}
			if (this.certificateSerial == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "IM Certificate Serial is null.");
				return;
			}
			string text2 = ConfigurationManager.AppSettings["IMPortNumber"];
			int num;
			if (!string.IsNullOrEmpty(text2) && int.TryParse(text2, out num) && num >= 0)
			{
				mtlsPortNumber = num;
			}
			InstantMessageOCSProvider.InitializeEndpointManager(this.certificateIssuer, this.certificateSerial, mtlsPortNumber);
		}

		private void ReadAndInitializeExceptionPerfCounterSettings()
		{
			string text = ConfigurationManager.AppSettings["MailboxOfflineExQueueSize"];
			string text2 = ConfigurationManager.AppSettings["ConnectionFailedTransientExQueueSize"];
			string text3 = ConfigurationManager.AppSettings["StorageTransientExQueueSize"];
			string text4 = ConfigurationManager.AppSettings["StoragePermanentExQueueSize"];
			int num;
			if (!string.IsNullOrEmpty(text) && int.TryParse(text, out num) && num > 0)
			{
				PerformanceCounterManager.MailboxOfflineExResultsQueueSize = num;
			}
			int num2;
			if (!string.IsNullOrEmpty(text2) && int.TryParse(text2, out num2) && num2 > 0)
			{
				PerformanceCounterManager.ConnectionFailedTransientExResultsQueueSize = num2;
			}
			int num3;
			if (!string.IsNullOrEmpty(text4) && int.TryParse(text4, out num3) && num3 > 0)
			{
				PerformanceCounterManager.StoragePermanentExResultsQueueSize = num3;
			}
			int num4;
			if (!string.IsNullOrEmpty(text3) && int.TryParse(text3, out num4) && num4 > 0)
			{
				PerformanceCounterManager.StorageTransientExResultsQueueSize = num4;
			}
			PerformanceCounterManager.InitializeExPerfCountersQueueSizes();
		}

		private void ReadAndInitializeIMPerfCounterSettings()
		{
			int signInFailureQueueSize = 100;
			int sentMessageFailureQueueSize = 100;
			string text = ConfigurationManager.AppSettings["SignInFailureQueueSize"];
			string text2 = ConfigurationManager.AppSettings["SentMessageFailureQueueSize"];
			int num;
			if (!string.IsNullOrEmpty(text) && int.TryParse(text, out num) && num > 0)
			{
				signInFailureQueueSize = num;
			}
			int num2;
			if (!string.IsNullOrEmpty(text2) && int.TryParse(text2, out num2) && num2 > 0)
			{
				sentMessageFailureQueueSize = num2;
			}
			PerformanceCounterManager.InitializeIMQueueSizes(signInFailureQueueSize, sentMessageFailureQueueSize);
		}

		private void IncomingTlsNegotiationFailedHandler(object sender, ErrorEventArgs args)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "Globals.IncomingTlsNegotiationFailedHandler");
			Exception ex = null;
			if (args != null)
			{
				ex = args.GetException();
			}
			OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_FailedToEstablishMTLSConnection, string.Empty, new object[]
			{
				(ex != null && ex.Message != null) ? ex.Message : string.Empty
			});
		}

		private int GetIntSettingFromConfig(string settingName, int defaultValue)
		{
			string text = ConfigurationManager.AppSettings[settingName];
			int result = defaultValue;
			if (string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string, int>(0L, "{0} not set in web.config, using default value of {1}.", settingName, defaultValue);
			}
			else if (!int.TryParse(text, out result))
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string, string, int>(0L, "Invalid {0} set in web.config (value: {1}); using default value of {2}.", settingName, text, defaultValue);
				result = defaultValue;
			}
			return result;
		}

		public override bool IsPreCheckinApp
		{
			get
			{
				bool flag = false;
				bool flag2 = bool.TryParse(ConfigurationManager.AppSettings["IsPreCheckinApp"], out flag);
				return flag2 && flag;
			}
		}

		public override bool IsPushNotificationsEnabled
		{
			get
			{
				return this.isPushNotificationsEnabled;
			}
		}

		public override bool IsPullNotificationsEnabled
		{
			get
			{
				return this.isPullNotificationsEnabled;
			}
		}

		public override bool IsFolderContentNotificationsEnabled
		{
			get
			{
				return this.isFolderContentNotificationsEnabled;
			}
		}

		public override int ConnectionCacheSize
		{
			get
			{
				string text = ConfigurationManager.AppSettings["ConnectionCacheSize"];
				if (string.IsNullOrEmpty(text))
				{
					ExTraceGlobals.CoreTracer.TraceDebug<int>(0L, "ConnectionCacheSize not set in web.config; using default value of {0}.", 100);
					return 100;
				}
				int num;
				if (int.TryParse(text, out num) && 0 < num)
				{
					return num;
				}
				ExTraceGlobals.CoreTracer.TraceDebug<string, int>(0L, "Invalid ConnectionCacheSize set in web.config (value: {0}); using default value of {1}.", text, 100);
				return 100;
			}
		}

		public override bool ListenAdNotifications
		{
			get
			{
				bool flag = false;
				bool flag2 = bool.TryParse(ConfigurationManager.AppSettings["ListenAdNotifications"], out flag);
				return !flag2 || flag;
			}
		}

		public override bool RenderBreadcrumbsInAboutPage
		{
			get
			{
				bool flag = true;
				bool flag2 = bool.TryParse(ConfigurationManager.AppSettings["RenderBreadcrumbsInAboutPage"], out flag);
				return flag2 && flag;
			}
		}

		public override int MaximumTemporaryFilteredViewPerUser
		{
			get
			{
				string text = ConfigurationManager.AppSettings["MaximumTemporaryFilteredViewPerUser"];
				int num;
				if (string.IsNullOrEmpty(text) || !int.TryParse(text, out num) || num < 1)
				{
					return 60;
				}
				return num;
			}
		}

		public override int MaximumFilteredViewInFavoritesPerUser
		{
			get
			{
				string text = ConfigurationManager.AppSettings["MaximumFilteredViewInFavoritesPerUser"];
				int num;
				if (string.IsNullOrEmpty(text) || !int.TryParse(text, out num) || num < 1)
				{
					return 25;
				}
				return num;
			}
		}

		public override bool DisableBreadcrumbs
		{
			get
			{
				bool flag = false;
				bool flag2 = bool.TryParse(ConfigurationManager.AppSettings["DisableBreadcrumbs"], out flag);
				return flag2 && flag;
			}
		}

		public override int MaxBreadcrumbs
		{
			get
			{
				string text = ConfigurationManager.AppSettings["MaxBreadcrumbs"];
				if (string.IsNullOrEmpty(text))
				{
					ExTraceGlobals.CoreTracer.TraceDebug<int>(0L, "MaxBreadcrumbs not set in web.config; using default value of {0}.", 20);
					return 20;
				}
				int num;
				if (int.TryParse(text, out num) && 0 < num)
				{
					return num;
				}
				ExTraceGlobals.CoreTracer.TraceDebug<string, int>(0L, "Invalid defaultMaxBreadcrumbs set in web.config (value: {0}); using default value of {1}.", text, 20);
				return 20;
			}
		}

		public override bool StoreTransientExceptionEventLogEnabled
		{
			get
			{
				string value = ConfigurationManager.AppSettings["StoreTransientExceptionEventLogEnabled"];
				bool flag;
				return !string.IsNullOrEmpty(value) && bool.TryParse(value, out flag) && flag;
			}
		}

		public override int StoreTransientExceptionEventLogThreshold
		{
			get
			{
				string text = ConfigurationManager.AppSettings["StoreTransientExceptionEventLogThreshold"];
				if (string.IsNullOrEmpty(text))
				{
					return 50;
				}
				int num;
				if (!int.TryParse(text, out num))
				{
					return 50;
				}
				if (num <= 0)
				{
					return 50;
				}
				return num;
			}
		}

		public override int StoreTransientExceptionEventLogFrequencyInSeconds
		{
			get
			{
				string text = ConfigurationManager.AppSettings["StoreTransientExceptionEventLogFrequencyInSeconds"];
				if (string.IsNullOrEmpty(text))
				{
					return 1800;
				}
				int num;
				if (!int.TryParse(text, out num))
				{
					return 1800;
				}
				if (num <= 0)
				{
					return 1800;
				}
				return num;
			}
		}

		public override int MaxPendingRequestLifeInSeconds
		{
			get
			{
				string text = ConfigurationManager.AppSettings["MaxPendingRequestLife"];
				if (string.IsNullOrEmpty(text))
				{
					return 300;
				}
				int num = -1;
				int.TryParse(text, out num);
				if (30 > num || num > 1800)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string, int>(0L, "Invalid max pending request life set in web.config (value: {0}); using default value of {1}.", text, 300);
					return 300;
				}
				return num;
			}
		}

		public override int MaxItemsInConversationExpansion
		{
			get
			{
				int result;
				bool flag = int.TryParse(ConfigurationManager.AppSettings["MaxItemsInConversationExpansion"], out result);
				if (flag)
				{
					return result;
				}
				return 499;
			}
		}

		public override int MaxItemsInConversationReadingPane
		{
			get
			{
				int result;
				bool flag = int.TryParse(ConfigurationManager.AppSettings["MaxItemsInConversationReadingPane"], out result);
				if (flag)
				{
					return result;
				}
				return 100;
			}
		}

		public override long MaxBytesInConversationReadingPane
		{
			get
			{
				string text = ConfigurationManager.AppSettings["MaxBytesInConversationReadingPane"];
				if (!string.IsNullOrEmpty(text))
				{
					long result;
					bool flag = long.TryParse(text, out result);
					if (flag)
					{
						return result;
					}
				}
				return long.MinValue;
			}
		}

		public override bool HideDeletedItems
		{
			get
			{
				bool flag2;
				bool flag = bool.TryParse(ConfigurationManager.AppSettings["HideDeletedItems"], out flag2);
				return flag && flag2;
			}
		}

		public override string OCSServerName
		{
			get
			{
				return this.ocsServerName;
			}
		}

		public override int ActivityBasedPresenceDuration
		{
			get
			{
				return this.activityBasedPresenceDuration;
			}
		}

		public override int MailTipsMaxClientCacheSize
		{
			get
			{
				return this.GetIntSettingFromConfig("MailTipsMaxClientCacheSize", 300);
			}
		}

		public override int MailTipsMaxMailboxSourcedRecipientSize
		{
			get
			{
				return this.GetIntSettingFromConfig("MailTipsMaxMailboxSourcedRecipientSize", 300);
			}
		}

		public override int MailTipsClientCacheEntryExpiryInHours
		{
			get
			{
				return this.GetIntSettingFromConfig("MailTipsClientCacheEntryExpiryInHours", 2);
			}
		}

		internal override PhishingLevel MinimumSuspiciousPhishingLevel
		{
			get
			{
				return this.minimumSuspiciousPhishingLevel;
			}
		}

		internal override int UserContextLockTimeout
		{
			get
			{
				if (this.userContextLockTimeout <= 0)
				{
					this.userContextLockTimeout = Math.Min(this.GetIntSettingFromConfig("UserContextTimeout", 3000), 30000);
					if (this.userContextLockTimeout <= 0)
					{
						this.userContextLockTimeout = 3000;
					}
				}
				return this.userContextLockTimeout;
			}
		}

		private const string ConnCacheSize = "ConnectionCacheSize";

		private const int DefaultConnectionCacheSize = 100;

		private const double MinUserTimeoutMaxPendingLifeRatio = 1.25;

		private const int DefaultStoreTransientExceptionEventLogThreshold = 50;

		private const int DefaultStoreTransientExceptionEventLogFrequencyInSeconds = 1800;

		private const int DefaultMaxPendingRequestLifeInSeconds = 300;

		private const int DefaultMaxBreadcrumbs = 20;

		private const bool DefaultDisableBreadcrumbs = false;

		private const bool DefaultBypassOwaXmlAttachmentFiltering = false;

		private const int DefaultMaximumTemporaryFilteredViewPerUser = 60;

		private const int DefaultMaximumFilteredViewInFavoritesPerUser = 25;

		internal const int DefaultUserContextLockTimeout = 3000;

		internal const int MaxUserContextLockTimeout = 30000;

		internal const string UserContextLockTimeoutStr = "UserContextTimeout";

		private bool isPushNotificationsEnabled = true;

		private bool isPullNotificationsEnabled;

		private bool isFolderContentNotificationsEnabled = true;

		private string ocsServerName;

		private string certificateIssuer;

		private byte[] certificateSerial;

		private int activityBasedPresenceDuration;

		private PhishingLevel minimumSuspiciousPhishingLevel = PhishingLevel.Suspicious1;

		private int userContextLockTimeout = -1;
	}
}
