using System;
using System.Configuration;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TenantDataCollectorConfig : AnchorConfig
	{
		internal TenantDataCollectorConfig() : base("TenantDataCollector")
		{
			base.UpdateConfig<Uri>("WebServiceUri", UpgradeCommon.DefaultSymphonyWebserviceUri);
			base.UpdateConfig<string>("CertificateSubject", UpgradeCommon.DefaultSymphonyCertificateSubject);
			base.UpdateConfig<TimeSpan>("IdleRunDelay", TimeSpan.FromHours(24.0));
			base.UpdateConfig<TimeSpan>("ActiveRunDelay", TimeSpan.FromHours(24.0));
			base.UpdateConfig<string>("MonitoringComponentName", ExchangeComponent.MailboxMigration.Name);
			base.UpdateConfig<string>("CacheEntryPoisonNotificationReason", "TenantDataCollectorCacheEntryIsPoisonedNotification");
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

		[ConfigurationProperty("E14DataDirectory", DefaultValue = "C$\\Program Files\\Microsoft\\Exchange Server\\V14\\logging\\CompleteMailboxStats")]
		public string E14DataDirectory
		{
			get
			{
				return this.InternalGetConfig<string>("E14DataDirectory");
			}
			set
			{
				this.InternalSetConfig<string>(value, "E14DataDirectory");
			}
		}

		[ConfigurationProperty("E15DataDirectory", DefaultValue = "C$\\Program Files\\Microsoft\\Exchange Server\\V15\\logging\\CompleteMailboxStats")]
		public string E15DataDirectory
		{
			get
			{
				return this.InternalGetConfig<string>("E15DataDirectory");
			}
			set
			{
				this.InternalSetConfig<string>(value, "E15DataDirectory");
			}
		}

		[ConfigurationProperty("UpgradeUnitsConversionFactor", DefaultValue = 50)]
		[IntegerValidator(MinValue = 0, MaxValue = 1000, ExcludeRange = false)]
		public int UpgradeUnitsConversionFactor
		{
			get
			{
				return this.InternalGetConfig<int>("UpgradeUnitsConversionFactor");
			}
			set
			{
				this.InternalSetConfig<int>(value, "UpgradeUnitsConversionFactor");
			}
		}

		[ConfigurationProperty("CheckAllAccountPartitions", DefaultValue = false)]
		public bool CheckAllAccountPartitions
		{
			get
			{
				return this.InternalGetConfig<bool>("CheckAllAccountPartitions");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "CheckAllAccountPartitions");
			}
		}

		[ConfigurationProperty("ValidateMailboxVersions", DefaultValue = true)]
		public bool ValidateMailboxVersions
		{
			get
			{
				return this.InternalGetConfig<bool>("ValidateMailboxVersions");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "ValidateMailboxVersions");
			}
		}

		[ConfigurationProperty("UploadToSymphony", DefaultValue = true)]
		public bool UploadToSymphony
		{
			get
			{
				return this.InternalGetConfig<bool>("UploadToSymphony");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "UploadToSymphony");
			}
		}

		private const string TenantDataCollectorCacheEntryIsPoisonedNotification = "TenantDataCollectorCacheEntryIsPoisonedNotification";
	}
}
