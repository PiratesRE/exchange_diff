using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class LegacyThrottlingPolicySchema : ADConfigurationObjectSchema
	{
		private static ADPropertyDefinition BuildSettingsProperty(string name, string ldapDisplayName)
		{
			return new ADPropertyDefinition(name, ExchangeObjectVersion.Exchange2010, typeof(LegacyThrottlingPolicySettings), ldapDisplayName, ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);
		}

		private static ADPropertyDefinition BuildCalculatedReadOnlySettingProperty(string name, string key, ADPropertyDefinition parentProperty)
		{
			return new ADPropertyDefinition(name, ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
			{
				parentProperty
			}, null, delegate(IPropertyBag propertyBag)
			{
				LegacyThrottlingPolicySettings legacyThrottlingPolicySettings = (LegacyThrottlingPolicySettings)propertyBag[parentProperty];
				string result;
				if (legacyThrottlingPolicySettings != null && legacyThrottlingPolicySettings.TryGetValue(key, out result))
				{
					return result;
				}
				return null;
			}, null, null, null);
		}

		private const string MaxConcurrencyKey = "con";

		public static readonly ADPropertyDefinition IsDefaultPolicy = new ADPropertyDefinition("IsDefaultPolicy", ExchangeObjectVersion.Exchange2010, typeof(bool), "msExchThrottlingIsDefaultPolicy", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AnonymousThrottlingPolicyStateSettings = LegacyThrottlingPolicySchema.BuildSettingsProperty("AnonymousThrottlingPolicyStateSettings", "msExchAnonymousThrottlingPolicyState");

		public static readonly ADPropertyDefinition EasThrottlingPolicyStateSettings = LegacyThrottlingPolicySchema.BuildSettingsProperty("EasThrottlingPolicyStateSettings", "msExchEasThrottlingPolicyState");

		public static readonly ADPropertyDefinition EwsThrottlingPolicyStateSettings = LegacyThrottlingPolicySchema.BuildSettingsProperty("EwsThrottlingPolicyStateSettings", "msExchEwsThrottlingPolicyState");

		public static readonly ADPropertyDefinition ImapThrottlingPolicyStateSettings = LegacyThrottlingPolicySchema.BuildSettingsProperty("ImapThrottlingPolicyStateSettings", "msExchImapThrottlingPolicyState");

		public static readonly ADPropertyDefinition OwaThrottlingPolicyStateSettings = LegacyThrottlingPolicySchema.BuildSettingsProperty("OwaThrottlingPolicyStateSettings", "msExchOwaThrottlingPolicyState");

		public static readonly ADPropertyDefinition PopThrottlingPolicyStateSettings = LegacyThrottlingPolicySchema.BuildSettingsProperty("PopThrottlingPolicyStateSettings", "msExchPopThrottlingPolicyState");

		public static readonly ADPropertyDefinition PowershellThrottlingPolicyStateSettings = LegacyThrottlingPolicySchema.BuildSettingsProperty("PowershellThrottlingPolicyStateSettings", "msExchPowershellThrottlingPolicyState");

		public static readonly ADPropertyDefinition RcaThrottlingPolicyStateSettings = LegacyThrottlingPolicySchema.BuildSettingsProperty("RcaThrottlingPolicyStateSettings", "msExchRcaThrottlingPolicyState");

		public static readonly ADPropertyDefinition GeneralThrottlingPolicyStateSettings = LegacyThrottlingPolicySchema.BuildSettingsProperty("GeneralThrottlingPolicyStateSettings", "msExchGeneralThrottlingPolicyState");

		public static readonly ADPropertyDefinition AnonymousMaxConcurrency = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("AnonymousMaxConcurrency", "con", LegacyThrottlingPolicySchema.AnonymousThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition EasMaxConcurrency = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("EasMaxConcurrency", "con", LegacyThrottlingPolicySchema.EasThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition EasMaxDevices = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("EasMaxDevices", "md", LegacyThrottlingPolicySchema.EasThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition EasMaxDeviceDeletesPerMonth = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("EasMaxDeviceDeletesPerMonth", "mddpm", LegacyThrottlingPolicySchema.EasThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition EwsMaxConcurrency = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("EwsMaxConcurrency", "con", LegacyThrottlingPolicySchema.EwsThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition EwsMaxSubscriptions = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("EwsMaxSubscriptions", "sub", LegacyThrottlingPolicySchema.EwsThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition ImapMaxConcurrency = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("ImapMaxConcurrency", "con", LegacyThrottlingPolicySchema.ImapThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition OwaMaxConcurrency = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("OwaMaxConcurrency", "con", LegacyThrottlingPolicySchema.OwaThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition PopMaxConcurrency = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("PopMaxConcurrency", "con", LegacyThrottlingPolicySchema.PopThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition PowershellMaxConcurrency = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("PowershellMaxConcurrency", "con", LegacyThrottlingPolicySchema.PowershellThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition PowershellMaxTenantConcurrency = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("PowershellMaxTenantConcurrency", "ten", LegacyThrottlingPolicySchema.PowershellThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition PowerShellMaxCmdlets = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("PowerShellMaxCmdlets", "cmds", LegacyThrottlingPolicySchema.PowershellThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition PowershellMaxCmdletsTimePeriod = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("PowershellMaxCmdletsTimePeriod", "per", LegacyThrottlingPolicySchema.PowershellThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition PowershellMaxCmdletQueueDepth = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("PowershellMaxCmdletQueueDepth", "que", LegacyThrottlingPolicySchema.PowershellThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition ExchangeMaxCmdlets = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("ExchangeMaxCmdlets", "excmds", LegacyThrottlingPolicySchema.PowershellThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition PowershellMaxDestructiveCmdlets = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("PowershellMaxDestructiveCmdlets", "dscmds", LegacyThrottlingPolicySchema.PowershellThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition PowershellMaxDestructiveCmdletsTimePeriod = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("PowershellMaxDestructiveCmdletsTimePeriod", "dsper", LegacyThrottlingPolicySchema.PowershellThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition RcaMaxConcurrency = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("RcaMaxConcurrency", "con", LegacyThrottlingPolicySchema.RcaThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition CpaMaxConcurrency = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("CpaMaxConcurrency", "xcon", LegacyThrottlingPolicySchema.RcaThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition MessageRateLimit = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("MessageRateLimit", "mrl", LegacyThrottlingPolicySchema.GeneralThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition RecipientRateLimit = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("RecipientRateLimit", "rrl", LegacyThrottlingPolicySchema.GeneralThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition ForwardeeLimit = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("ForwardeeLimit", "fl", LegacyThrottlingPolicySchema.GeneralThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition DiscoveryMaxConcurrency = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("DiscoveryMaxConcurrency", "dmc", LegacyThrottlingPolicySchema.GeneralThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition DiscoveryMaxMailboxes = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("DiscoveryMaxMailboxes", "dmm", LegacyThrottlingPolicySchema.GeneralThrottlingPolicyStateSettings);

		public static readonly ADPropertyDefinition DiscoveryMaxKeywords = LegacyThrottlingPolicySchema.BuildCalculatedReadOnlySettingProperty("DiscoveryMaxKeywords", "dmk", LegacyThrottlingPolicySchema.GeneralThrottlingPolicyStateSettings);
	}
}
