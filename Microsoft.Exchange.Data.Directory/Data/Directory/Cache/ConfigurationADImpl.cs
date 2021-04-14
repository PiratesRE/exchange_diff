using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Caching;
using System.Threading;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	internal class ConfigurationADImpl : ICacheConfiguration, IDisposable
	{
		public bool IsCacheEnabled(string processNameOrProcessAppName)
		{
			if (!Globals.IsDatacenter)
			{
				return false;
			}
			if (!this.Initialize())
			{
				return false;
			}
			this.RefreshIfNecessary();
			return this.isEnabled && !this.exclusiveProcessSet.Contains(processNameOrProcessAppName) && (this.inclusiveProcessSet.Contains(processNameOrProcessAppName) || this.inclusiveProcessSet.Contains("*"));
		}

		public bool IsCacheEnableForCurrentProcess()
		{
			if (!Globals.IsDatacenter)
			{
				return false;
			}
			if (!this.Initialize())
			{
				return false;
			}
			this.RefreshIfNecessary();
			return this.isCurrentProcessEnabled;
		}

		public CacheMode GetCacheMode(string processNameOrProcessAppName)
		{
			if (this.IsCacheEnabled(processNameOrProcessAppName))
			{
				return this.GetCacheMode();
			}
			return CacheMode.Disabled;
		}

		public CacheMode GetCacheModeForCurrentProcess()
		{
			if (this.IsCacheEnableForCurrentProcess())
			{
				return this.GetCacheMode();
			}
			return CacheMode.Disabled;
		}

		public bool IsCacheEnabled(Type objectType)
		{
			if (!Globals.IsDatacenter)
			{
				return false;
			}
			if (!this.Initialize())
			{
				return false;
			}
			this.RefreshIfNecessary();
			return this.isCurrentProcessEnabled && ((this.isWhiteList && (this.objectTypeSet.Contains("*") || this.objectTypeSet.Contains(objectType.Name))) || (!this.isWhiteList && !this.objectTypeSet.Contains(objectType.Name) && !this.objectTypeSet.Contains("*")));
		}

		public bool IsCacheEnabledForInsertOnSave(ADRawEntry rawEntry)
		{
			return this.IsObjectEligibleForRecentlyCreatedBehavior(rawEntry) && this.newObjectBehaviorConfiguration.InsertOnSave;
		}

		public int GetCacheExpirationForObject(ADRawEntry rawEntry)
		{
			if (this.IsObjectEligibleForRecentlyCreatedBehavior(rawEntry))
			{
				return this.newObjectBehaviorConfiguration.TimeToLiveInSeconds;
			}
			return 2147483646;
		}

		public CacheItemPriority GetCachePriorityForObject(ADRawEntry rawEntry)
		{
			if (this.IsObjectEligibleForRecentlyCreatedBehavior(rawEntry) && this.newObjectBehaviorConfiguration.InsertWithHigherPriority)
			{
				return CacheItemPriority.NotRemovable;
			}
			return CacheItemPriority.Default;
		}

		public void Dispose()
		{
			if (this.provider != null)
			{
				this.provider.Dispose();
				this.provider = null;
			}
		}

		private bool Initialize()
		{
			if (this.provider == null)
			{
				if (Globals.ProcessInstanceType == InstanceType.NotInitialized)
				{
					return false;
				}
				lock (this.lockObj)
				{
					if (this.provider == null)
					{
						this.provider = ConfigProvider.CreateADProvider(new ConfigurationADImpl.ADCacheConfigurationSchema(), null);
						this.provider.Initialize();
					}
				}
			}
			return true;
		}

		private string GetProcessOrAppName()
		{
			if (Globals.ProcessName.StartsWith("w3wp", StringComparison.OrdinalIgnoreCase))
			{
				return Globals.ProcessNameAppName;
			}
			return Globals.ProcessName;
		}

		private bool IsObjectEligibleForRecentlyCreatedBehavior(ADRawEntry rawEntry)
		{
			if (rawEntry == null)
			{
				return false;
			}
			if (!this.IsCacheEnabled(rawEntry.GetType()))
			{
				return false;
			}
			RecipientTypeDetails? recipientTypeDetails = rawEntry[ADRecipientSchema.RecipientTypeDetails] as RecipientTypeDetails?;
			if (!this.newObjectBehaviorConfiguration.IsRecipientTypeDetailsEnabled(recipientTypeDetails))
			{
				return false;
			}
			DateTime? dateTime = rawEntry[ADObjectSchema.WhenCreatedUTC] as DateTime?;
			return dateTime != null && this.newObjectBehaviorConfiguration.IsDateTimeWithinInclusionThreshold(dateTime.Value);
		}

		private void RefreshIfNecessary()
		{
			if (this.lastUpdate + TimeSpan.FromMinutes(5.0) >= DateTime.UtcNow)
			{
				return;
			}
			try
			{
				if (Monitor.TryEnter(this.lastUpdateLockObj))
				{
					this.configIsNotFound = !this.provider.TryGetConfig<bool>(ServerSettingsContext.LocalServer, "IsEnabled", out this.isEnabled);
					if (this.configIsNotFound)
					{
						this.isEnabled = ExEnvironment.IsTest;
					}
					this.exclusiveProcessSet = this.GetConfigValues("ExclusiveProcesses");
					this.inclusiveProcessSet = this.GetConfigValues("InclusiveProcesses");
					this.objectTypeSet = this.GetConfigValues("ObjectTypes");
					this.isWhiteList = this.provider.GetConfig<bool>(ServerSettingsContext.LocalServer, "IsWhiteList");
					this.newObjectBehaviorConfiguration = new ConfigurationADImpl.NewObjectCacheBehaviorConfiguration
					{
						EnabledRecipientTypes = this.GetConfigValues("NewObjectCacheRecipientTypes"),
						InclusionThresholdInMinutes = this.provider.GetConfig<int>(ServerSettingsContext.LocalServer, "NewObjectCacheInclusionThresholdMinutes"),
						InsertOnSave = this.provider.GetConfig<bool>(ServerSettingsContext.LocalServer, "NewObjectCacheInsertOnSave"),
						TimeToLiveInMinutes = this.provider.GetConfig<int>(ServerSettingsContext.LocalServer, "NewObjectCacheTimeToLiveMinutes"),
						InsertWithHigherPriority = this.provider.GetConfig<bool>(ServerSettingsContext.LocalServer, "NewObjectCacheWithHighPriority")
					};
					this.isCurrentProcessEnabled = false;
					if (this.isEnabled && Globals.InstanceType != InstanceType.NotInitialized)
					{
						string processOrAppName = this.GetProcessOrAppName();
						if (!"ExSetup.exe".StartsWith(processOrAppName, StringComparison.OrdinalIgnoreCase))
						{
							this.isCurrentProcessEnabled = (!this.exclusiveProcessSet.Contains(processOrAppName) && (this.inclusiveProcessSet.Contains(processOrAppName) || this.inclusiveProcessSet.Contains("*")));
						}
					}
					this.lastUpdate = DateTime.UtcNow;
				}
			}
			finally
			{
				if (Monitor.IsEntered(this.lastUpdateLockObj))
				{
					Monitor.Exit(this.lastUpdateLockObj);
				}
			}
		}

		private HashSet<string> GetConfigValues(string propertyName)
		{
			string config = this.provider.GetConfig<string>(ServerSettingsContext.LocalServer, propertyName);
			if (string.IsNullOrEmpty(config))
			{
				return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			}
			return new HashSet<string>(config.Split(new char[]
			{
				','
			}), StringComparer.OrdinalIgnoreCase);
		}

		private CacheMode GetCacheMode()
		{
			bool flag = false;
			ADDriverContext addriverContext = ADSessionSettings.GetProcessADContext() ?? ADSessionSettings.GetThreadADContext();
			if (addriverContext != null)
			{
				if (ContextMode.Setup == addriverContext.Mode || ContextMode.TopologyService == addriverContext.Mode)
				{
					return CacheMode.Disabled;
				}
				flag = (ContextMode.Cmdlet == addriverContext.Mode);
			}
			if (!flag)
			{
				return CacheMode.Read | CacheMode.SyncWrite;
			}
			return CacheMode.AsyncWrite;
		}

		private const string ExcludedProcess = "ExSetup.exe";

		private const string W3wpProcessName = "w3wp";

		private const int UpdateIntervalInMin = 5;

		private IConfigProvider provider;

		private object lockObj = new object();

		private bool configIsNotFound;

		private bool isEnabled;

		private HashSet<string> exclusiveProcessSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private HashSet<string> inclusiveProcessSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private HashSet<string> objectTypeSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private ConfigurationADImpl.NewObjectCacheBehaviorConfiguration newObjectBehaviorConfiguration = new ConfigurationADImpl.NewObjectCacheBehaviorConfiguration();

		private bool isWhiteList;

		private DateTime lastUpdate;

		private bool isCurrentProcessEnabled;

		private object lastUpdateLockObj = new object();

		internal class NewObjectCacheBehaviorConfiguration
		{
			internal NewObjectCacheBehaviorConfiguration()
			{
				this.EnabledRecipientTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			}

			internal HashSet<string> EnabledRecipientTypes { get; set; }

			internal int InclusionThresholdInMinutes
			{
				get
				{
					return (int)this.inclusionThreshold.TotalMinutes;
				}
				set
				{
					this.inclusionThreshold = TimeSpan.FromMinutes((double)value);
				}
			}

			internal bool InsertOnSave { get; set; }

			internal int TimeToLiveInMinutes
			{
				set
				{
					this.TimeToLiveInSeconds = value * 60;
				}
			}

			internal int TimeToLiveInSeconds { get; private set; }

			internal bool InsertWithHigherPriority { get; set; }

			internal bool IsRecipientTypeDetailsEnabled(RecipientTypeDetails? recipientTypeDetails)
			{
				return recipientTypeDetails != null && this.EnabledRecipientTypes.Contains(recipientTypeDetails.ToString());
			}

			internal bool IsDateTimeWithinInclusionThreshold(DateTime dateTime)
			{
				DateTime t = DateTime.UtcNow.Subtract(this.inclusionThreshold);
				return dateTime > t;
			}

			private TimeSpan inclusionThreshold;
		}

		internal class ADCacheConfigurationSchema : ConfigSchemaBase
		{
			public override string Name
			{
				get
				{
					return "ADCache";
				}
			}

			public override string SectionName
			{
				get
				{
					return "ADCacheConfiguration";
				}
			}

			[ConfigurationProperty("IsEnabled", DefaultValue = false)]
			public bool IsEnabled
			{
				get
				{
					return (bool)base["IsEnabled"];
				}
				set
				{
					base["IsEnabled"] = value;
				}
			}

			[ConfigurationProperty("ExclusiveProcesses", DefaultValue = "Microsoft.Exchange.Management.ForwardSync.exe,Microsoft.Exchange.ServiceHost.exe,wsmprovhost.exe,MSExchangeMailboxReplication.exe,Microsoft.Exchange.Directory.TopologyService.exe")]
			public string ExclusiveProcesses
			{
				get
				{
					return (string)base["ExclusiveProcesses"];
				}
				set
				{
					base["ExclusiveProcesses"] = value;
				}
			}

			[ConfigurationProperty("InclusiveProcesses", DefaultValue = "*")]
			public string InclusiveProcesses
			{
				get
				{
					return (string)base["InclusiveProcesses"];
				}
				set
				{
					base["InclusiveProcesses"] = value;
				}
			}

			[ConfigurationProperty("ObjectTypes", DefaultValue = "")]
			public string ObjectTypes
			{
				get
				{
					return (string)base["ObjectTypes"];
				}
				set
				{
					base["ObjectTypes"] = value;
				}
			}

			[ConfigurationProperty("IsWhiteList", DefaultValue = false)]
			public bool IsWhiteList
			{
				get
				{
					return (bool)base["IsWhiteList"];
				}
				set
				{
					base["IsWhiteList"] = value;
				}
			}

			[ConfigurationProperty("NewObjectCacheRecipientTypes", DefaultValue = "")]
			public string NewObjectCacheRecipientTypes
			{
				get
				{
					return (string)base["NewObjectCacheRecipientTypes"];
				}
				set
				{
					base["NewObjectCacheRecipientTypes"] = value;
				}
			}

			[ConfigurationProperty("NewObjectCacheInclusionThresholdMinutes", DefaultValue = 15)]
			public int NewObjectCacheInclusionThresholdMinutes
			{
				get
				{
					return (int)base["NewObjectCacheInclusionThresholdMinutes"];
				}
				set
				{
					base["NewObjectCacheInclusionThresholdMinutes"] = value;
				}
			}

			[ConfigurationProperty("NewObjectCacheInsertOnSave", DefaultValue = true)]
			public bool NewObjectCacheInsertOnSave
			{
				get
				{
					return (bool)base["NewObjectCacheInsertOnSave"];
				}
				set
				{
					base["NewObjectCacheInsertOnSave"] = value;
				}
			}

			[ConfigurationProperty("NewObjectCacheTimeToLiveMinutes", DefaultValue = 15)]
			public int NewObjectCacheTimeToLiveMinutes
			{
				get
				{
					return (int)base["NewObjectCacheTimeToLiveMinutes"];
				}
				set
				{
					base["NewObjectCacheTimeToLiveMinutes"] = value;
				}
			}

			[ConfigurationProperty("NewObjectCacheWithHighPriority", DefaultValue = true)]
			public bool NewObjectCacheWithHighPriority
			{
				get
				{
					return (bool)base["NewObjectCacheWithHighPriority"];
				}
				set
				{
					base["NewObjectCacheWithHighPriority"] = value;
				}
			}

			protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
			{
				return base.OnDeserializeUnrecognizedAttribute(name, value);
			}

			public static class Constants
			{
				public const string SchemaName = "ADCache";

				public const string IsEnabled = "IsEnabled";

				public const string ExclusiveProcesses = "ExclusiveProcesses";

				public const string InclusiveProcesses = "InclusiveProcesses";

				public const string ObjectTypes = "ObjectTypes";

				public const string IsWhiteList = "IsWhiteList";

				public const string NewObjectCacheRecipientTypes = "NewObjectCacheRecipientTypes";

				public const string NewObjectCacheInclusionThresholdMinutes = "NewObjectCacheInclusionThresholdMinutes";

				public const string NewObjectCacheInsertOnSave = "NewObjectCacheInsertOnSave";

				public const string NewObjectCacheTimeToLiveMinutes = "NewObjectCacheTimeToLiveMinutes";

				public const string NewObjectCacheWithHighPriority = "NewObjectCacheWithHighPriority";
			}
		}
	}
}
