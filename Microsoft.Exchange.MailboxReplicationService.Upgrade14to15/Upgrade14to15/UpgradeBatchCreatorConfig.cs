using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UpgradeBatchCreatorConfig : AnchorConfig
	{
		internal UpgradeBatchCreatorConfig() : base("UpgradeBatchCreator")
		{
			base.UpdateConfig<string>("BatchFileDirectoryPath", UpgradeBatchCreatorConfig.GetDefaultBatchFileDirectoryPath());
			base.UpdateConfig<TimeSpan>("IdleRunDelay", TimeSpan.FromMinutes(15.0));
			base.UpdateConfig<TimeSpan>("ActiveRunDelay", TimeSpan.FromSeconds(10.0));
			base.UpdateConfig<string>("MonitoringComponentName", ExchangeComponent.MailboxMigration.Name);
			base.UpdateConfig<string>("CacheEntryPoisonNotificationReason", "UpgradeBatchCreatorCacheEntryIsPoisonedNotification");
			base.UpdateConfig<long>("LogMaxFileSize", 52428800L);
		}

		[ConfigurationProperty("BatchFileDirectoryPath")]
		public string BatchFileDirectoryPath
		{
			get
			{
				return this.InternalGetConfig<string>("BatchFileDirectoryPath");
			}
			set
			{
				this.InternalSetConfig<string>(value, "BatchFileDirectoryPath");
			}
		}

		[ConfigurationProperty("UpgradeBatchFilenamePrefix", DefaultValue = "E15UpgradeMSEXCH")]
		public string UpgradeBatchFilenamePrefix
		{
			get
			{
				return this.InternalGetConfig<string>("UpgradeBatchFilenamePrefix");
			}
			set
			{
				this.InternalSetConfig<string>(value, "UpgradeBatchFilenamePrefix");
			}
		}

		[ConfigurationProperty("DryRunBatchFilenamePrefix", DefaultValue = "E15UpgradeMSEXCHDryRun")]
		public string DryRunBatchFilenamePrefix
		{
			get
			{
				return this.InternalGetConfig<string>("DryRunBatchFilenamePrefix");
			}
			set
			{
				this.InternalSetConfig<string>(value, "DryRunBatchFilenamePrefix");
			}
		}

		[ConfigurationProperty("MaxBatchSize", DefaultValue = 500)]
		[IntegerValidator(MinValue = 1, MaxValue = 5000, ExcludeRange = false)]
		public int MaxBatchSize
		{
			get
			{
				return this.InternalGetConfig<int>("MaxBatchSize");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxBatchSize");
			}
		}

		[ConfigurationProperty("E14CountUpdateInterval", DefaultValue = "03:00:00")]
		[TimeSpanValidator(MinValueString = "00:15:00", MaxValueString = "1.00:00:00", ExcludeRange = false)]
		public TimeSpan E14CountUpdateIntervalName
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("E14CountUpdateIntervalName");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "E14CountUpdateIntervalName");
			}
		}

		[ConfigurationProperty("DelayUntilCreateNewBatches", DefaultValue = "02:00:00")]
		[TimeSpanValidator(MinValueString = "00:15:00", MaxValueString = "1.00:00:00", ExcludeRange = false)]
		public TimeSpan DelayUntilCreateNewBatches
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("DelayUntilCreateNewBatches");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "DelayUntilCreateNewBatches");
			}
		}

		[ConfigurationProperty("RemoveNonUpgradeMoveRequests", DefaultValue = true)]
		public bool RemoveNonUpgradeMoveRequests
		{
			get
			{
				return this.InternalGetConfig<bool>("RemoveNonUpgradeMoveRequests");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "RemoveNonUpgradeMoveRequests");
			}
		}

		[ConfigurationProperty("ConfigOnly", DefaultValue = true)]
		public bool ConfigOnly
		{
			get
			{
				return this.InternalGetConfig<bool>("ConfigOnly");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "ConfigOnly");
			}
		}

		private static string GetDefaultBatchFileDirectoryPath()
		{
			string text = ExchangeSetupContext.InstallPath;
			if (text == null)
			{
				text = Assembly.GetExecutingAssembly().Location;
				text = Path.GetDirectoryName(text);
			}
			return Path.Combine(text, "UpgradeBatches");
		}

		private const string UpgradeBatchCreatorCacheEntryIsPoisonedNotification = "UpgradeBatchCreatorCacheEntryIsPoisonedNotification";
	}
}
