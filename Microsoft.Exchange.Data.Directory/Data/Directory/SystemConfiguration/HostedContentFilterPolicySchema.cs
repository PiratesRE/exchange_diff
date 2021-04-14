using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class HostedContentFilterPolicySchema : ADConfigurationObjectSchema
	{
		private static int AsfSettingsGetterFunc(int slot, ProviderPropertyDefinition propertyDefinition, IPropertyBag bag)
		{
			if (bag[propertyDefinition] == null)
			{
				bag[propertyDefinition] = HostedContentFilterPolicySchema.GetDefaultAsfOptionValues();
			}
			int num = (slot % 2 == 0) ? 0 : 4;
			return (bag[propertyDefinition] as byte[])[slot / 2] >> num & 15;
		}

		private static GetterDelegate AsfSettingsGetterDelegateInt32(int slot, ProviderPropertyDefinition propertyDefinition, Func<int, int> modifier)
		{
			return delegate(IPropertyBag bag)
			{
				int num = HostedContentFilterPolicySchema.AsfSettingsGetterFunc(slot, propertyDefinition, bag);
				if (modifier == null)
				{
					return num;
				}
				return modifier(num);
			};
		}

		private static GetterDelegate AsfSettingsGetterDelegate(int slot, ProviderPropertyDefinition propertyDefinition)
		{
			return (IPropertyBag bag) => (SpamFilteringOption)HostedContentFilterPolicySchema.AsfSettingsGetterFunc(slot, propertyDefinition, bag);
		}

		private static SetterDelegate AsfSettingsSetterDelegate(int slot, ProviderPropertyDefinition propertyDefinition)
		{
			return delegate(object value, IPropertyBag bag)
			{
				if (bag[propertyDefinition] == null)
				{
					bag[propertyDefinition] = HostedContentFilterPolicySchema.GetDefaultAsfOptionValues();
				}
				byte[] array = bag[propertyDefinition] as byte[];
				int num = (slot % 2 == 0) ? 0 : 4;
				int num2 = (num == 0) ? 240 : 15;
				int num3 = (int)array[slot / 2];
				num3 &= num2;
				num3 |= (int)value << num;
				array[slot / 2] = (byte)num3;
				(bag as PropertyBag).MarkAsChanged(propertyDefinition);
			};
		}

		private static byte[] GetDefaultAsfOptionValues()
		{
			byte[] array = new byte[32];
			array[7] = 16;
			return array;
		}

		private static QueryFilter IsDefaultFilterBuilder(SinglePropertyFilter filter)
		{
			return new AndFilter(new QueryFilter[]
			{
				ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(HostedContentFilterPolicySchema.SpamFilteringFlags, 64UL)),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ExchangeVersion, ExchangeObjectVersion.Exchange2012)
			});
		}

		internal const int AsfSettingsByteArrayLengthMinValue = 1;

		internal const int AsfSettingsByteArrayLengthMaxValue = 32;

		internal const int EndUserSpamNotificationFrequencyMinValue = 1;

		internal const int EndUserSpamNotificationFrequencyMaxValue = 15;

		internal const int EndUserSpamNotificationFrequencyDefaultValue = 3;

		internal const int QuarantineRetentionPeriodMinValue = 1;

		internal const int QuarantineRetentionPeriodMaxValue = 15;

		internal const int QuarantineRetentionPeriodDefaultValue = 15;

		internal const int EndUserSpamNotificationLimitDefaultValue = 0;

		internal const int SpamFilteringFlagsDefaultValue = 0;

		internal const int BulkThresholdDefaultValue = 6;

		internal const int BulkThresholdMinValue = 1;

		internal const int BulkThresholdMaxValue = 9;

		internal const int TestModeActionShift = 3;

		internal const int TestModeActionLength = 3;

		internal const int IsDefaultShift = 6;

		internal const int EnableDigestsShift = 7;

		internal const int DownloadLinkShift = 12;

		internal const int HighConfidenceSpamActionShift = 13;

		internal const int HighConfidenceSpamActionLength = 3;

		internal const int SpamActionShift = 21;

		internal const int SpamActionLength = 3;

		internal const int EnableRegionBlockListShift = 25;

		internal const int EnableLanguageBlockListShift = 26;

		public static readonly ADPropertyDefinition AddXHeaderValue = new ADPropertyDefinition("AddXHeaderValue", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchSpamAddHeader", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ModifySubjectValue = new ADPropertyDefinition("ModifySubjectValue", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchSpamModifySubject", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RedirectToRecipients = new ADPropertyDefinition("RedirectToRecipients", ExchangeObjectVersion.Exchange2012, typeof(SmtpAddress), "msExchSpamRedirectAddress", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TestModeBccToRecipients = new ADPropertyDefinition("TestModeBccToRecipients", ExchangeObjectVersion.Exchange2012, typeof(SmtpAddress), "msExchSpamAsfTestBccAddress", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FalsePositiveAdditionalRecipients = new ADPropertyDefinition("FalsePositiveAdditionalRecipients", ExchangeObjectVersion.Exchange2012, typeof(SmtpAddress), "msExchSpamFalsePositiveCc", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SpamFilteringFlags = new ADPropertyDefinition("SpamFilteringFlags", ExchangeObjectVersion.Exchange2012, typeof(int), "msExchSpamFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AsfSettings = new ADPropertyDefinition("AsfSettings", ExchangeObjectVersion.Exchange2012, typeof(byte[]), "msExchSpamAsfSettings", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ByteArrayLengthConstraint(1, 32)
		}, null, null);

		public static readonly ADPropertyDefinition LanguageBlockList = new ADPropertyDefinition("LanguageBlockList", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchSpamLanguageBlockList", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RegionBlockList = new ADPropertyDefinition("RegionBlockList", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchSpamCountryBlockList", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition QuarantineRetentionPeriod = new ADPropertyDefinition("QuarantineRetentionPeriod", ExchangeObjectVersion.Exchange2012, typeof(int), "msExchSpamQuarantineRetention", ADPropertyDefinitionFlags.PersistDefaultValue, 15, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 15)
		}, null, null);

		public static readonly ADPropertyDefinition EndUserSpamNotificationFrequency = new ADPropertyDefinition("EndUserSpamNotificationFrequency", ExchangeObjectVersion.Exchange2012, typeof(int), "msExchSpamDigestFrequency", ADPropertyDefinitionFlags.PersistDefaultValue, 3, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 15)
		}, null, null);

		public static readonly ADPropertyDefinition EndUserSpamNotificationCustomFromAddress = new ADPropertyDefinition("EndUserSpamNotificationCustomFromAddress", ExchangeObjectVersion.Exchange2012, typeof(SmtpAddress), "msExchMalwareFilterConfigInternalSubject", ADPropertyDefinitionFlags.None, SmtpAddress.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EndUserSpamNotificationCustomFromName = new ADPropertyDefinition("EndUserSpamNotificationCustomFromName", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchMalwareFilterConfigFromName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EndUserSpamNotificationCustomSubject = new ADPropertyDefinition("EndUserSpamNotificationCustomSubject", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchMalwareFilterConfigExternalSubject", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EndUserSpamNotificationLimit = new ADPropertyDefinition("EndUserSpamNotificationLimit", ExchangeObjectVersion.Exchange2012, typeof(int), "msExchMalwareFilterConfigFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EndUserSpamNotificationLanguage = new ADPropertyDefinition("EndUserSpamNotificationLanguage", ExchangeObjectVersion.Exchange2012, typeof(EsnLanguage), "msExchMalwareFilteringFlags", ADPropertyDefinitionFlags.PersistDefaultValue, EsnLanguage.Default, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TestModeAction = ADObject.BitfieldProperty("TestModeAction", 3, 3, HostedContentFilterPolicySchema.SpamFilteringFlags);

		public static readonly ADPropertyDefinition IsDefault = new ADPropertyDefinition("IsDefault", ExchangeObjectVersion.Exchange2012, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HostedContentFilterPolicySchema.SpamFilteringFlags
		}, new CustomFilterBuilderDelegate(HostedContentFilterPolicySchema.IsDefaultFilterBuilder), ADObject.FlagGetterDelegate(HostedContentFilterPolicySchema.SpamFilteringFlags, 64), ADObject.FlagSetterDelegate(HostedContentFilterPolicySchema.SpamFilteringFlags, 64), null, null);

		public static readonly ADPropertyDefinition EnableEndUserSpamNotifications = ADObject.BitfieldProperty("EnableEndUserSpamNotifications", 7, HostedContentFilterPolicySchema.SpamFilteringFlags);

		public static readonly ADPropertyDefinition DownloadLink = ADObject.BitfieldProperty("DownloadLink", 12, HostedContentFilterPolicySchema.SpamFilteringFlags);

		public static readonly ADPropertyDefinition HighConfidenceSpamAction = ADObject.BitfieldProperty("HighConfidenceSpamAction", 13, 3, HostedContentFilterPolicySchema.SpamFilteringFlags);

		public static readonly ADPropertyDefinition SpamAction = ADObject.BitfieldProperty("SpamAction", 21, 3, HostedContentFilterPolicySchema.SpamFilteringFlags);

		public static readonly ADPropertyDefinition EnableRegionBlockList = ADObject.BitfieldProperty("EnableRegionBlockList", 25, HostedContentFilterPolicySchema.SpamFilteringFlags);

		public static readonly ADPropertyDefinition EnableLanguageBlockList = ADObject.BitfieldProperty("EnableLanguageBlockList", 26, HostedContentFilterPolicySchema.SpamFilteringFlags);

		public static readonly ADPropertyDefinition IncreaseScoreWithImageLinks = new ADPropertyDefinition("IncreaseScoreWithImageLinks", ExchangeObjectVersion.Exchange2012, typeof(SpamFilteringOption), null, ADPropertyDefinitionFlags.Calculated, SpamFilteringOption.Off, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HostedContentFilterPolicySchema.AsfSettings
		}, null, HostedContentFilterPolicySchema.AsfSettingsGetterDelegate(0, HostedContentFilterPolicySchema.AsfSettings), HostedContentFilterPolicySchema.AsfSettingsSetterDelegate(0, HostedContentFilterPolicySchema.AsfSettings), null, null);

		public static readonly ADPropertyDefinition IncreaseScoreWithNumericIps = new ADPropertyDefinition("IncreaseScoreWithNumericIps", ExchangeObjectVersion.Exchange2012, typeof(SpamFilteringOption), null, ADPropertyDefinitionFlags.Calculated, SpamFilteringOption.Off, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HostedContentFilterPolicySchema.AsfSettings
		}, null, HostedContentFilterPolicySchema.AsfSettingsGetterDelegate(1, HostedContentFilterPolicySchema.AsfSettings), HostedContentFilterPolicySchema.AsfSettingsSetterDelegate(1, HostedContentFilterPolicySchema.AsfSettings), null, null);

		public static readonly ADPropertyDefinition IncreaseScoreWithRedirectToOtherPort = new ADPropertyDefinition("IncreaseScoreWithRedirectToOtherPort", ExchangeObjectVersion.Exchange2012, typeof(SpamFilteringOption), null, ADPropertyDefinitionFlags.Calculated, SpamFilteringOption.Off, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HostedContentFilterPolicySchema.AsfSettings
		}, null, HostedContentFilterPolicySchema.AsfSettingsGetterDelegate(2, HostedContentFilterPolicySchema.AsfSettings), HostedContentFilterPolicySchema.AsfSettingsSetterDelegate(2, HostedContentFilterPolicySchema.AsfSettings), null, null);

		public static readonly ADPropertyDefinition IncreaseScoreWithBizOrInfoUrls = new ADPropertyDefinition("IncreaseScoreWithBizOrInfoUrls", ExchangeObjectVersion.Exchange2012, typeof(SpamFilteringOption), null, ADPropertyDefinitionFlags.Calculated, SpamFilteringOption.Off, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HostedContentFilterPolicySchema.AsfSettings
		}, null, HostedContentFilterPolicySchema.AsfSettingsGetterDelegate(3, HostedContentFilterPolicySchema.AsfSettings), HostedContentFilterPolicySchema.AsfSettingsSetterDelegate(3, HostedContentFilterPolicySchema.AsfSettings), null, null);

		public static readonly ADPropertyDefinition MarkAsSpamEmptyMessages = new ADPropertyDefinition("MarkAsSpamEmptyMessages", ExchangeObjectVersion.Exchange2012, typeof(SpamFilteringOption), null, ADPropertyDefinitionFlags.Calculated, SpamFilteringOption.Off, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HostedContentFilterPolicySchema.AsfSettings
		}, null, HostedContentFilterPolicySchema.AsfSettingsGetterDelegate(4, HostedContentFilterPolicySchema.AsfSettings), HostedContentFilterPolicySchema.AsfSettingsSetterDelegate(4, HostedContentFilterPolicySchema.AsfSettings), null, null);

		public static readonly ADPropertyDefinition MarkAsSpamJavaScriptInHtml = new ADPropertyDefinition("MarkAsSpamJavaScriptInHtml", ExchangeObjectVersion.Exchange2012, typeof(SpamFilteringOption), null, ADPropertyDefinitionFlags.Calculated, SpamFilteringOption.Off, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HostedContentFilterPolicySchema.AsfSettings
		}, null, HostedContentFilterPolicySchema.AsfSettingsGetterDelegate(5, HostedContentFilterPolicySchema.AsfSettings), HostedContentFilterPolicySchema.AsfSettingsSetterDelegate(5, HostedContentFilterPolicySchema.AsfSettings), null, null);

		public static readonly ADPropertyDefinition MarkAsSpamFramesInHtml = new ADPropertyDefinition("MarkAsSpamFramesInHtml", ExchangeObjectVersion.Exchange2012, typeof(SpamFilteringOption), null, ADPropertyDefinitionFlags.Calculated, SpamFilteringOption.Off, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HostedContentFilterPolicySchema.AsfSettings
		}, null, HostedContentFilterPolicySchema.AsfSettingsGetterDelegate(6, HostedContentFilterPolicySchema.AsfSettings), HostedContentFilterPolicySchema.AsfSettingsSetterDelegate(6, HostedContentFilterPolicySchema.AsfSettings), null, null);

		public static readonly ADPropertyDefinition MarkAsSpamObjectTagsInHtml = new ADPropertyDefinition("MarkAsSpamObjectTagsInHtml", ExchangeObjectVersion.Exchange2012, typeof(SpamFilteringOption), null, ADPropertyDefinitionFlags.Calculated, SpamFilteringOption.Off, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HostedContentFilterPolicySchema.AsfSettings
		}, null, HostedContentFilterPolicySchema.AsfSettingsGetterDelegate(7, HostedContentFilterPolicySchema.AsfSettings), HostedContentFilterPolicySchema.AsfSettingsSetterDelegate(7, HostedContentFilterPolicySchema.AsfSettings), null, null);

		public static readonly ADPropertyDefinition MarkAsSpamEmbedTagsInHtml = new ADPropertyDefinition("MarkAsSpamEmbedTagsInHtml", ExchangeObjectVersion.Exchange2012, typeof(SpamFilteringOption), null, ADPropertyDefinitionFlags.Calculated, SpamFilteringOption.Off, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HostedContentFilterPolicySchema.AsfSettings
		}, null, HostedContentFilterPolicySchema.AsfSettingsGetterDelegate(8, HostedContentFilterPolicySchema.AsfSettings), HostedContentFilterPolicySchema.AsfSettingsSetterDelegate(8, HostedContentFilterPolicySchema.AsfSettings), null, null);

		public static readonly ADPropertyDefinition MarkAsSpamFormTagsInHtml = new ADPropertyDefinition("MarkAsSpamFormTagsInHtml", ExchangeObjectVersion.Exchange2012, typeof(SpamFilteringOption), null, ADPropertyDefinitionFlags.Calculated, SpamFilteringOption.Off, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HostedContentFilterPolicySchema.AsfSettings
		}, null, HostedContentFilterPolicySchema.AsfSettingsGetterDelegate(9, HostedContentFilterPolicySchema.AsfSettings), HostedContentFilterPolicySchema.AsfSettingsSetterDelegate(9, HostedContentFilterPolicySchema.AsfSettings), null, null);

		public static readonly ADPropertyDefinition MarkAsSpamWebBugsInHtml = new ADPropertyDefinition("MarkAsSpamWebBugsInHtml", ExchangeObjectVersion.Exchange2012, typeof(SpamFilteringOption), null, ADPropertyDefinitionFlags.Calculated, SpamFilteringOption.Off, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HostedContentFilterPolicySchema.AsfSettings
		}, null, HostedContentFilterPolicySchema.AsfSettingsGetterDelegate(10, HostedContentFilterPolicySchema.AsfSettings), HostedContentFilterPolicySchema.AsfSettingsSetterDelegate(10, HostedContentFilterPolicySchema.AsfSettings), null, null);

		public static readonly ADPropertyDefinition MarkAsSpamSensitiveWordList = new ADPropertyDefinition("MarkAsSpamSensitiveWordList", ExchangeObjectVersion.Exchange2012, typeof(SpamFilteringOption), null, ADPropertyDefinitionFlags.Calculated, SpamFilteringOption.Off, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HostedContentFilterPolicySchema.AsfSettings
		}, null, HostedContentFilterPolicySchema.AsfSettingsGetterDelegate(11, HostedContentFilterPolicySchema.AsfSettings), HostedContentFilterPolicySchema.AsfSettingsSetterDelegate(11, HostedContentFilterPolicySchema.AsfSettings), null, null);

		public static readonly ADPropertyDefinition MarkAsSpamSpfRecordHardFail = new ADPropertyDefinition("MarkAsSpamSpfRecordHardFail", ExchangeObjectVersion.Exchange2012, typeof(SpamFilteringOption), null, ADPropertyDefinitionFlags.Calculated, SpamFilteringOption.Off, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HostedContentFilterPolicySchema.AsfSettings
		}, null, HostedContentFilterPolicySchema.AsfSettingsGetterDelegate(12, HostedContentFilterPolicySchema.AsfSettings), HostedContentFilterPolicySchema.AsfSettingsSetterDelegate(12, HostedContentFilterPolicySchema.AsfSettings), null, null);

		public static readonly ADPropertyDefinition MarkAsSpamFromAddressAuthFail = new ADPropertyDefinition("MarkAsSpamFromAddressAuthFail", ExchangeObjectVersion.Exchange2012, typeof(SpamFilteringOption), null, ADPropertyDefinitionFlags.Calculated, SpamFilteringOption.Off, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HostedContentFilterPolicySchema.AsfSettings
		}, null, HostedContentFilterPolicySchema.AsfSettingsGetterDelegate(13, HostedContentFilterPolicySchema.AsfSettings), HostedContentFilterPolicySchema.AsfSettingsSetterDelegate(13, HostedContentFilterPolicySchema.AsfSettings), null, null);

		public static readonly ADPropertyDefinition MarkAsSpamNdrBackscatter = new ADPropertyDefinition("MarkAsSpamNdrBackscatter", ExchangeObjectVersion.Exchange2012, typeof(SpamFilteringOption), null, ADPropertyDefinitionFlags.Calculated, SpamFilteringOption.Off, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HostedContentFilterPolicySchema.AsfSettings
		}, null, HostedContentFilterPolicySchema.AsfSettingsGetterDelegate(14, HostedContentFilterPolicySchema.AsfSettings), HostedContentFilterPolicySchema.AsfSettingsSetterDelegate(14, HostedContentFilterPolicySchema.AsfSettings), null, null);

		public static readonly ADPropertyDefinition MarkAsSpamBulkMail = new ADPropertyDefinition("MarkAsSpamBulkMail", ExchangeObjectVersion.Exchange2012, typeof(SpamFilteringOption), null, ADPropertyDefinitionFlags.Calculated, SpamFilteringOption.Off, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HostedContentFilterPolicySchema.AsfSettings
		}, null, HostedContentFilterPolicySchema.AsfSettingsGetterDelegate(15, HostedContentFilterPolicySchema.AsfSettings), HostedContentFilterPolicySchema.AsfSettingsSetterDelegate(15, HostedContentFilterPolicySchema.AsfSettings), null, null);

		public static readonly ADPropertyDefinition BulkThreshold = new ADPropertyDefinition("BulkThreshold", ExchangeObjectVersion.Exchange2012, typeof(int), null, ADPropertyDefinitionFlags.Calculated, 6, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 9)
		}, new ProviderPropertyDefinition[]
		{
			HostedContentFilterPolicySchema.AsfSettings
		}, null, HostedContentFilterPolicySchema.AsfSettingsGetterDelegateInt32(16, HostedContentFilterPolicySchema.AsfSettings, delegate(int v)
		{
			if (v == 0)
			{
				return 6;
			}
			return v;
		}), HostedContentFilterPolicySchema.AsfSettingsSetterDelegate(16, HostedContentFilterPolicySchema.AsfSettings), null, null);
	}
}
