using System;
using System.Globalization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ADAMTransportConfigContainerSchema : ADContainerSchema
	{
		internal const int DefaultShadowHeartbeatRetryCount = 12;

		internal const int DefaultMaxRetriesForRemoteSiteShadow = 4;

		internal const int DefaultMaxRetriesForLocalSiteShadow = 2;

		internal static readonly EnhancedTimeSpan DefaultShadowHeartbeatTimeoutInterval = EnhancedTimeSpan.FromMinutes(15.0);

		internal static readonly EnhancedTimeSpan DefaultShadowMessageAutoDiscardInterval = EnhancedTimeSpan.FromDays(2.0);

		public static readonly ADPropertyDefinition TLSReceiveDomainSecureList = new ADPropertyDefinition("TLSReceiveDomainSecureList", ExchangeObjectVersion.Exchange2007, typeof(SmtpDomain), "msExchTLSReceiveDomainSecureList", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TLSSendDomainSecureList = new ADPropertyDefinition("TLSSendDomainSecureList", ExchangeObjectVersion.Exchange2007, typeof(SmtpDomain), "msExchTLSSendDomainSecureList", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition GenerateCopyOfDSNFor = new ADPropertyDefinition("GenerateCopyOfDSNFor", ExchangeObjectVersion.Exchange2007, typeof(EnhancedStatusCode), "msExchDSNSendCopyToAdmin", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InternalSMTPServers = new ADPropertyDefinition("InternalSMTPServers", ExchangeObjectVersion.Exchange2007, typeof(IPRange), "msExchInternalSMTPServers", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition JournalingReportNdrTo = new ADPropertyDefinition("JournalingReportNdrTo", ExchangeObjectVersion.Exchange2007, typeof(SmtpAddress), "msExchJournalingReportNdrTo", ADPropertyDefinitionFlags.None, SmtpAddress.NullReversePath, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Flags = new ADPropertyDefinition("TransportSettingsFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportSettingsFlags", ADPropertyDefinitionFlags.PersistDefaultValue, TopologyProvider.IsAdamTopology() ? 512 : 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition VerifySecureSubmitEnabled = new ADPropertyDefinition("VerifySecureSubmitEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 32), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 32), null, null);

		public static readonly ADPropertyDefinition KeepCategories = new ADPropertyDefinition("KeepCategories", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 1), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 1), null, null);

		public static readonly ADPropertyDefinition AddressBookPolicyRoutingEnabled = new ADPropertyDefinition("AddressBookPolicyRoutingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 4), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 4), null, null);

		public static readonly ADPropertyDefinition ConvertDisclaimerWrapperToEml = new ADPropertyDefinition("ConvertDisclaimerWrapperToEml", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 16777216), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 16777216), null, null);

		public static readonly ADPropertyDefinition VoicemailJournalingDisabled = new ADPropertyDefinition("VoicemailJournalingDisabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 8), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 8), null, null);

		public static readonly ADPropertyDefinition HeaderPromotionModeSetting = new ADPropertyDefinition("HeaderPromotionModeSetting", ExchangeObjectVersion.Exchange2007, typeof(HeaderPromotionMode), null, ADPropertyDefinitionFlags.Calculated, HeaderPromotionMode.NoCreate, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, new GetterDelegate(TransportConfigContainer.InternalHeaderPromotionModeGetter), new SetterDelegate(TransportConfigContainer.InternalHeaderPromotionModeSetter), null, null);

		public static readonly ADPropertyDefinition PreserveReportBodypart = new ADPropertyDefinition("PreserveReportBodypart", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 131072), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 131072), null, null);

		public static readonly ADPropertyDefinition ConvertReportToMessage = new ADPropertyDefinition("ConvertReportToMessage", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 262144), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 262144), null, null);

		public static readonly ADPropertyDefinition DSNConversionMode = new ADPropertyDefinition("DSNConversionMode", ExchangeObjectVersion.Exchange2007, typeof(DSNConversionOption), null, ADPropertyDefinitionFlags.Calculated, DSNConversionOption.UseExchangeDSNs, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, new GetterDelegate(TransportConfigContainerSchema.DSNConversionModeGetter), new SetterDelegate(TransportConfigContainerSchema.DSNConversionModeSetter), null, null);

		public static readonly ADPropertyDefinition DisableXexch50 = new ADPropertyDefinition("DisableXexch50", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 16), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 16), null, null);

		public static readonly ADPropertyDefinition Rfc2231EncodingEnabled = new ADPropertyDefinition("Rfc2231EncodingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 256), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 256), null, null);

		public static readonly ADPropertyDefinition OpenDomainRoutingEnabled = new ADPropertyDefinition("OpenDomainRoutingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 1024), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 1024), null, null);

		public static readonly ADPropertyDefinition MaxSendSize = new ADPropertyDefinition("MaxSendSize", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.KilobyteQuantifierProvider, "submissionContLength", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2097151UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalDelayDsnDisabled = new ADPropertyDefinition("ExternalDelayDsnDisabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 65536), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 65536), null, null);

		public static readonly ADPropertyDefinition ExternalDsnDefaultLanguageStr = new ADPropertyDefinition("ExternalDSNDefaultLanguageStr", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchTransportExternalDefaultLanguage", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalDsnDefaultLanguage = new ADPropertyDefinition("ExternalDSNDefaultLanguage", ExchangeObjectVersion.Exchange2007, typeof(CultureInfo), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new DelegateConstraint(new ValidationDelegate(ConstraintDelegates.ValidateDsnDefaultCulture))
		}, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.ExternalDsnDefaultLanguageStr
		}, null, new GetterDelegate(TransportConfigContainer.ExternalDsnDefaultLanguageGetter), new SetterDelegate(TransportConfigContainer.ExternalDsnDefaultLanguageSetter), null, null);

		public static readonly ADPropertyDefinition ExternalDsnLanguageDetectionDisabled = new ADPropertyDefinition("ExternalDsnLanguageDetectionDisabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 2048), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 2048), null, null);

		public static readonly ADPropertyDefinition ExternalDsnMaxMessageAttachSize = new ADPropertyDefinition("ExternalDsnMaxMessageAttachSize", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchTransportExternalMaxDSNMessageAttachmentSize", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromMB(10UL), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromBytes(2147483647UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalDsnReportingAuthority = new ADPropertyDefinition("ExternalDsnReportingAuthority", ExchangeObjectVersion.Exchange2007, typeof(SmtpDomain), "msExchTransportExternalDSNReportingAuthority", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalDsnSendHtmlDisabled = new ADPropertyDefinition("ExternalDsnSendHtmlDisabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 4096), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 4096), null, null);

		public static readonly ADPropertyDefinition ExternalPostmasterAddress = new ADPropertyDefinition("ExternalPostmasterAddress", ExchangeObjectVersion.Exchange2007, typeof(SmtpAddress?), "msExchTransportExternalPostmasterAddress", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 320)
		}, null, null);

		public static readonly ADPropertyDefinition InternalDelayDsnDisabled = new ADPropertyDefinition("InternalDelayDsnDisabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 8192), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 8192), null, null);

		public static readonly ADPropertyDefinition InternalDsnDefaultLanguageStr = new ADPropertyDefinition("InternalDSNDefaultLanguageStr", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchTransportInternalDefaultLanguage", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InternalDsnDefaultLanguage = new ADPropertyDefinition("InternalDSNDefaultLanguage", ExchangeObjectVersion.Exchange2007, typeof(CultureInfo), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new DelegateConstraint(new ValidationDelegate(ConstraintDelegates.ValidateDsnDefaultCulture))
		}, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.InternalDsnDefaultLanguageStr
		}, null, new GetterDelegate(TransportConfigContainer.InternalDsnDefaultLanguageGetter), new SetterDelegate(TransportConfigContainer.InternalDsnDefaultLanguageSetter), null, null);

		public static readonly ADPropertyDefinition InternalDsnLanguageDetectionDisabled = new ADPropertyDefinition("InternalDsnLanguageDetectionDisabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 16384), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 16384), null, null);

		public static readonly ADPropertyDefinition InternalDsnMaxMessageAttachSize = new ADPropertyDefinition("InternalDsnMaxMessageAttachSize", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchTransportInternalMaxDSNMessageAttachmentSize", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromMB(10UL), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromBytes(2147483647UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InternalDsnReportingAuthority = new ADPropertyDefinition("InternalDsnReportingAuthority", ExchangeObjectVersion.Exchange2007, typeof(SmtpDomain), "msExchTransportInternalDSNReportingAuthority", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InternalDsnSendHtmlDisabled = new ADPropertyDefinition("InternalDsnSendHtmlDisabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 32768), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 32768), null, null);

		public static readonly ADPropertyDefinition ShadowRedundancyDisabled = new ADPropertyDefinition("ShadowRedundancyDisabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 512), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 512), null, null);

		public static readonly ADPropertyDefinition ShadowHeartbeatTimeoutInterval = new ADPropertyDefinition("ShadowHeartbeatTimeoutInterval", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchTransportShadowHeartbeatTimeoutInterval", ADPropertyDefinitionFlags.PersistDefaultValue, ADAMTransportConfigContainerSchema.DefaultShadowHeartbeatTimeoutInterval, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneSecond, EnhancedTimeSpan.OneDay),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ShadowHeartbeatRetryCount = new ADPropertyDefinition("ShadowHeartbeatRetryCount", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportShadowHeartbeatRetryCount", ADPropertyDefinitionFlags.PersistDefaultValue, 12, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 15)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxRetriesForRemoteSiteShadow = new ADPropertyDefinition("MaxRetriesForRemoteSiteShadow", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportMaxRetriesForRemoteSiteShadow", ADPropertyDefinitionFlags.PersistDefaultValue, 4, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 255)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxRetriesForLocalSiteShadow = new ADPropertyDefinition("MaxRetriesForLocalSiteShadow", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportMaxRetriesForLocalSiteShadow", ADPropertyDefinitionFlags.PersistDefaultValue, 2, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 255)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ShadowMessageAutoDiscardInterval = new ADPropertyDefinition("ShadowMessageAutoDiscardInterval", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchTransportShadowMessageAutoDiscardInterval", ADPropertyDefinitionFlags.PersistDefaultValue, ADAMTransportConfigContainerSchema.DefaultShadowMessageAutoDiscardInterval, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.FromSeconds(5.0), EnhancedTimeSpan.FromDays(90.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HygieneSuite = new ADPropertyDefinition("HygieneSuite", ExchangeObjectVersion.Exchange2007, typeof(HygieneSuiteEnum), "msExchTransportSettingsAVFlags", ADPropertyDefinitionFlags.PersistDefaultValue, HygieneSuiteEnum.Standard, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<HygieneSuiteEnum>(HygieneSuiteEnum.Standard, HygieneSuiteEnum.Premium)
		}, null, null);

		public static readonly ADPropertyDefinition MigrationEnabled = new ADPropertyDefinition("MigrationEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 1048576), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 1048576), null, null);

		public static readonly ADPropertyDefinition LegacyJournalingMigrationEnabled = new ADPropertyDefinition("LegacyJournalingMigrationEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 8388608), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 8388608), null, null);

		public static readonly ADPropertyDefinition LegacyArchiveJournalingEnabled = new ADPropertyDefinition("LegacyArchiveJournalingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 33554432), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 33554432), null, null);

		public static readonly ADPropertyDefinition RejectMessageOnShadowFailure = new ADPropertyDefinition("RejectMessageOnShadowFailure", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 67108864), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 67108864), null, null);

		public static readonly ADPropertyDefinition ShadowMessagePreferenceSetting = new ADPropertyDefinition("ShadowMessagePreferenceSetting", ExchangeObjectVersion.Exchange2007, typeof(ShadowMessagePreference), null, ADPropertyDefinitionFlags.Calculated, ShadowMessagePreference.PreferRemote, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, new GetterDelegate(TransportConfigContainer.InternalShadowMessagePreferenceGetter), new SetterDelegate(TransportConfigContainer.InternalShadowMessagePreferenceSetter), null, null);

		public static readonly ADPropertyDefinition RedirectUnprovisionedUserMessagesForLegacyArchiveJournaling = new ADPropertyDefinition("RedirectUnprovisionedUserMessagesForLegacyArchiveJournaling", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 536870912), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 536870912), null, null);

		public static readonly ADPropertyDefinition RedirectDLMessagesForLegacyArchiveJournaling = new ADPropertyDefinition("RedirectDLMessagesForLegacyArchiveJournaling", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 1073741824), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 1073741824), null, null);

		public static readonly ADPropertyDefinition LegacyArchiveLiveJournalingEnabled = new ADPropertyDefinition("LegacyArchiveLiveJournalingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 2), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 2), null, null);

		public static readonly ADPropertyDefinition JournalReportDLMemberSubstitutionEnabled = new ADPropertyDefinition("JournalReportDLMemberSubstitutionEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 64), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 64), null, null);

		public static readonly ADPropertyDefinition JournalArchivingEnabled = new ADPropertyDefinition("JournalArchivingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, ADObject.FlagGetterDelegate(ADAMTransportConfigContainerSchema.Flags, 128), ADObject.FlagSetterDelegate(ADAMTransportConfigContainerSchema.Flags, 128), null, null);

		public static readonly ADPropertyDefinition SafetyNetHoldTime = new ADPropertyDefinition("SafetyNetHoldTime", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchTransportDumpsterHoldTime", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromDays(7.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.FromSeconds(15.0), EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);
	}
}
