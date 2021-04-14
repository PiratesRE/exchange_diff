using System;
using System.Configuration;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UpgradeHandlerConfig : AnchorConfig
	{
		internal UpgradeHandlerConfig() : base("UpgradeHandler")
		{
			base.UpdateConfig<Uri>("WebServiceUri", UpgradeCommon.DefaultSymphonyWebserviceUri);
			base.UpdateConfig<string>("CertificateSubject", UpgradeCommon.DefaultSymphonyCertificateSubject);
			base.UpdateConfig<TimeSpan>("IdleRunDelay", TimeSpan.FromMinutes(15.0));
			base.UpdateConfig<TimeSpan>("ScannerInitialTimeDelay", TimeSpan.FromMinutes(2.0));
			base.UpdateConfig<string>("MonitoringComponentName", ExchangeComponent.MailboxMigration.Name);
			base.UpdateConfig<string>("CacheEntryPoisonNotificationReason", "UpgradeHandlerCacheEntryIsPoisonedNotification");
		}

		[ConfigurationProperty("WebServiceUri")]
		public Uri WebServiceUri
		{
			get
			{
				return this.InternalGetConfig<Uri>("WebServiceUri");
			}
			set
			{
				this.InternalSetConfig<Uri>(value, "WebServiceUri");
			}
		}

		[ConfigurationProperty("CertificateSubject")]
		public string CertificateSubject
		{
			get
			{
				return this.InternalGetConfig<string>("CertificateSubject");
			}
			set
			{
				this.InternalSetConfig<string>(value, "CertificateSubject");
			}
		}

		[IntegerValidator(MinValue = 1, MaxValue = 100, ExcludeRange = false)]
		[ConfigurationProperty("NumberOfSetMailboxAttempts", DefaultValue = 30)]
		public int NumberOfSetMailboxAttempts
		{
			get
			{
				return this.InternalGetConfig<int>("NumberOfSetMailboxAttempts");
			}
			set
			{
				this.InternalSetConfig<int>(value, "NumberOfSetMailboxAttempts");
			}
		}

		[ConfigurationProperty("SetMailboxAttemptIntervalSeconds", DefaultValue = 1)]
		[IntegerValidator(MinValue = 1, MaxValue = 60, ExcludeRange = false)]
		public int SetMailboxAttemptIntervalSeconds
		{
			get
			{
				return this.InternalGetConfig<int>("SetMailboxAttemptIntervalSeconds");
			}
			set
			{
				this.InternalSetConfig<int>(value, "SetMailboxAttemptIntervalSeconds");
			}
		}

		private const string UpgradeHandlerCacheEntryIsPoisonedNotification = "UpgradeHandlerCacheEntryIsPoisonedNotification";
	}
}
