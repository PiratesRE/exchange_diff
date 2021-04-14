using System;
using System.ComponentModel;
using System.Configuration;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class AdDriverConfigSchema : ConfigSchemaBase
	{
		public override string Name
		{
			get
			{
				return "AdDriver";
			}
		}

		public override string SectionName
		{
			get
			{
				return "AdDriverConfiguration";
			}
		}

		protected override ExchangeConfigurationSection ScopeSchema
		{
			get
			{
				return AdDriverConfigSchema.scopeSchema;
			}
		}

		[ConfigurationProperty("IsSoftLinkResolutionEnabledForAllProcesses", DefaultValue = true)]
		public bool IsSoftLinkResolutionEnabledForAllProcesses
		{
			get
			{
				return (bool)base["IsSoftLinkResolutionEnabledForAllProcesses"];
			}
			set
			{
				base["IsSoftLinkResolutionEnabledForAllProcesses"] = value;
			}
		}

		[ConfigurationProperty("IsSoftLinkResolutionCacheEnabled", DefaultValue = true)]
		public bool IsSoftLinkResolutionCacheEnabled
		{
			get
			{
				return (bool)base["IsSoftLinkResolutionCacheEnabled"];
			}
			set
			{
				base["IsSoftLinkResolutionCacheEnabled"] = value;
			}
		}

		[ConfigurationProperty("SoftLinkFormatVersion", DefaultValue = 1)]
		public int SoftLinkFormatVersion
		{
			get
			{
				return (int)base["SoftLinkFormatVersion"];
			}
			set
			{
				base["SoftLinkFormatVersion"] = value;
			}
		}

		[ConfigurationProperty("SoftLinkFilterVersion2Enabled", DefaultValue = true)]
		public bool SoftLinkFilterVersion2Enabled
		{
			get
			{
				return (bool)base["SoftLinkFilterVersion2Enabled"];
			}
			set
			{
				base["SoftLinkFilterVersion2Enabled"] = value;
			}
		}

		[ConfigurationProperty("GlsCacheServiceMode", DefaultValue = GlsCacheServiceMode.CacheDisabled)]
		public GlsCacheServiceMode GlsCacheServiceMode
		{
			get
			{
				return (GlsCacheServiceMode)base["GlsCacheServiceMode"];
			}
			set
			{
				base["GlsCacheServiceMode"] = value;
			}
		}

		[ConfigurationProperty("OverrideGlsCacheLoadType", DefaultValue = false)]
		public bool OverrideGlsCacheLoadType
		{
			get
			{
				return (bool)base["OverrideGlsCacheLoadType"];
			}
			set
			{
				base["OverrideGlsCacheLoadType"] = value;
			}
		}

		[ConfigurationProperty("OfflineDataCacheExpirationTimeInMinutes", DefaultValue = 5)]
		public int OfflineDataCacheExpirationTimeInMinutes
		{
			get
			{
				return (int)base["OfflineDataCacheExpirationTimeInMinutes"];
			}
			set
			{
				base["OfflineDataCacheExpirationTimeInMinutes"] = value;
			}
		}

		[TypeConverter(typeof(GlsOverrideCollectionConverter))]
		[ConfigurationProperty("GlsTenantOverrides", DefaultValue = null)]
		public GlsOverrideCollection GlsTenantOverrides
		{
			get
			{
				return (GlsOverrideCollection)base["GlsTenantOverrides"];
			}
			set
			{
				base["GlsTenantOverrides"] = value;
			}
		}

		[ConfigurationProperty("DelayForADWriteThrottlingInMsec", DefaultValue = 0)]
		[IntegerValidator(MinValue = 0, MaxValue = 3000, ExcludeRange = false)]
		public int DelayForADWriteThrottlingInMsec
		{
			get
			{
				return (int)base["DelayForADWriteThrottlingInMsec"];
			}
			set
			{
				base["DelayForADWriteThrottlingInMsec"] = value;
			}
		}

		[ConfigurationProperty("IsADWriteDisabled", DefaultValue = false)]
		public bool IsADWriteDisabled
		{
			get
			{
				return (bool)base["IsADWriteDisabled"];
			}
			set
			{
				base["IsADWriteDisabled"] = value;
			}
		}

		[ConfigurationProperty("MsoEndpointType", DefaultValue = MsoEndpointType.OLD)]
		public MsoEndpointType MsoEndpointType
		{
			get
			{
				return (MsoEndpointType)base["MsoEndpointType"];
			}
			set
			{
				base["MsoEndpointType"] = value;
			}
		}

		[ConfigurationProperty("AccountValidationEnabled", DefaultValue = false)]
		public bool AccountValidationEnabled
		{
			get
			{
				return (bool)base["AccountValidationEnabled"];
			}
			set
			{
				base["AccountValidationEnabled"] = value;
			}
		}

		[ConfigurationProperty("MservEndpoint", DefaultValue = "10.1.25.251")]
		public string MservEndpoint
		{
			get
			{
				return (string)base["MservEndpoint"];
			}
			set
			{
				base["MservEndpoint"] = value;
			}
		}

		[ConfigurationProperty("ConsumerMbxLookupDisabled", DefaultValue = false)]
		public bool ConsumerMbxLookupDisabled
		{
			get
			{
				return (bool)base["ConsumerMbxLookupDisabled"];
			}
			set
			{
				base["ConsumerMbxLookupDisabled"] = value;
			}
		}

		[ConfigurationProperty("ConsumerMailboxScenarioDisabled", DefaultValue = false)]
		public bool ConsumerMailboxScenarioDisabled
		{
			get
			{
				return (bool)base["ConsumerMailboxScenarioDisabled"];
			}
			set
			{
				base["ConsumerMailboxScenarioDisabled"] = value;
			}
		}

		[ConfigurationProperty("TolerateInvalidInputInAggregateSession", DefaultValue = true)]
		public bool TolerateInvalidInputInAggregateSession
		{
			get
			{
				return (bool)base["TolerateInvalidInputInAggregateSession"];
			}
			set
			{
				base["TolerateInvalidInputInAggregateSession"] = value;
			}
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, string>(0L, "Unrecognized configuration attribute {0}={1}", name, value);
			return base.OnDeserializeUnrecognizedAttribute(name, value);
		}

		private static readonly AdDriverConfigSchema.AdDriverScopeSchema scopeSchema = new AdDriverConfigSchema.AdDriverScopeSchema();

		[Serializable]
		public static class Scope
		{
			public const string ForestFqdn = "ForestFqdn";
		}

		public static class Setting
		{
			public const string IsSoftLinkResolutionEnabledForAllProcesses = "IsSoftLinkResolutionEnabledForAllProcesses";

			public const string IsSoftLinkResolutionCacheEnabled = "IsSoftLinkResolutionCacheEnabled";

			public const string SoftLinkFormatVersion = "SoftLinkFormatVersion";

			public const string SoftLinkFilterVersion2Enabled = "SoftLinkFilterVersion2Enabled";

			public const string GlsCacheServiceMode = "GlsCacheServiceMode";

			public const string OverrideGlsCacheLoadType = "OverrideGlsCacheLoadType";

			public const string OfflineDataCacheExpirationTimeInMinutes = "OfflineDataCacheExpirationTimeInMinutes";

			public const string DelayForADWriteThrottlingInMsec = "DelayForADWriteThrottlingInMsec";

			public const string IsADWriteDisabled = "IsADWriteDisabled";

			public const string GlsTenantOverrides = "GlsTenantOverrides";

			public const string MsoEndpointType = "MsoEndpointType";

			public const string AccountValidationEnabled = "AccountValidationEnabled";

			public const string TolerateInvalidInputInAggregateSession = "TolerateInvalidInputInAggregateSession";

			public const string MservEndpoint = "MservEndpoint";

			public const string ConsumerMbxLookupDisabled = "ConsumerMbxLookupDisabled";

			public const string ConsumerMailboxScenarioDisabled = "ConsumerMailboxScenarioDisabled";
		}

		[Serializable]
		private class AdDriverScopeSchema : ExchangeConfigurationSection
		{
			[ConfigurationProperty("ForestFqdn", DefaultValue = "")]
			public string ForestFqdn
			{
				get
				{
					return this.InternalGetConfig<string>("ForestFqdn");
				}
				set
				{
					this.InternalSetConfig<string>(value, "ForestFqdn");
				}
			}
		}
	}
}
