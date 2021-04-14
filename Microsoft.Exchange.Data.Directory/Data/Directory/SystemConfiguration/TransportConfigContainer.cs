using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class TransportConfigContainer : ADContainer
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				if (TopologyProvider.IsAdamTopology())
				{
					return TransportConfigContainer.adamSchema;
				}
				return TransportConfigContainer.adSchema;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateCount(0, 256)]
		public MultiValuedProperty<SmtpDomain> TLSReceiveDomainSecureList
		{
			get
			{
				return (MultiValuedProperty<SmtpDomain>)this[ADAMTransportConfigContainerSchema.TLSReceiveDomainSecureList];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.TLSReceiveDomainSecureList] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateCount(0, 256)]
		public MultiValuedProperty<SmtpDomain> TLSSendDomainSecureList
		{
			get
			{
				return (MultiValuedProperty<SmtpDomain>)this[ADAMTransportConfigContainerSchema.TLSSendDomainSecureList];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.TLSSendDomainSecureList] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<EnhancedStatusCode> GenerateCopyOfDSNFor
		{
			get
			{
				return (MultiValuedProperty<EnhancedStatusCode>)this[ADAMTransportConfigContainerSchema.GenerateCopyOfDSNFor];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.GenerateCopyOfDSNFor] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPRange> InternalSMTPServers
		{
			get
			{
				return (MultiValuedProperty<IPRange>)this[ADAMTransportConfigContainerSchema.InternalSMTPServers];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.InternalSMTPServers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress JournalingReportNdrTo
		{
			get
			{
				return (SmtpAddress)this[ADAMTransportConfigContainerSchema.JournalingReportNdrTo];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.JournalingReportNdrTo] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress OrganizationFederatedMailbox
		{
			get
			{
				return (SmtpAddress)this.GetNonAdamProperty(TransportConfigContainerSchema.OrganizationFederatedMailbox);
			}
			set
			{
				this[TransportConfigContainerSchema.OrganizationFederatedMailbox] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize MaxDumpsterSizePerDatabase
		{
			get
			{
				return (ByteQuantifiedSize)this.GetNonAdamProperty(TransportConfigContainerSchema.MaxDumpsterSizePerDatabase);
			}
			set
			{
				this[TransportConfigContainerSchema.MaxDumpsterSizePerDatabase] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan MaxDumpsterTime
		{
			get
			{
				return (EnhancedTimeSpan)this.GetNonAdamProperty(TransportConfigContainerSchema.MaxDumpsterTime);
			}
			set
			{
				this[TransportConfigContainerSchema.MaxDumpsterTime] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool VerifySecureSubmitEnabled
		{
			get
			{
				return (bool)this[ADAMTransportConfigContainerSchema.VerifySecureSubmitEnabled];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.VerifySecureSubmitEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ClearCategories
		{
			get
			{
				return !(bool)this[ADAMTransportConfigContainerSchema.KeepCategories];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.KeepCategories] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AddressBookPolicyRoutingEnabled
		{
			get
			{
				return (bool)this[ADAMTransportConfigContainerSchema.AddressBookPolicyRoutingEnabled];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.AddressBookPolicyRoutingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ConvertDisclaimerWrapperToEml
		{
			get
			{
				return (bool)this[ADAMTransportConfigContainerSchema.ConvertDisclaimerWrapperToEml];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.ConvertDisclaimerWrapperToEml] = value;
			}
		}

		public bool PreserveReportBodypart
		{
			get
			{
				return (bool)this[ADAMTransportConfigContainerSchema.PreserveReportBodypart];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.PreserveReportBodypart] = value;
			}
		}

		public bool ConvertReportToMessage
		{
			get
			{
				return (bool)this[ADAMTransportConfigContainerSchema.ConvertReportToMessage];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.ConvertReportToMessage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DSNConversionOption DSNConversionMode
		{
			get
			{
				return (DSNConversionOption)this[ADAMTransportConfigContainerSchema.DSNConversionMode];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.DSNConversionMode] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool VoicemailJournalingEnabled
		{
			get
			{
				return !(bool)this[ADAMTransportConfigContainerSchema.VoicemailJournalingDisabled];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.VoicemailJournalingDisabled] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public HeaderPromotionMode HeaderPromotionModeSetting
		{
			get
			{
				return (HeaderPromotionMode)this[ADAMTransportConfigContainerSchema.HeaderPromotionModeSetting];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.HeaderPromotionModeSetting] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Xexch50Enabled
		{
			get
			{
				return !(bool)this[ADAMTransportConfigContainerSchema.DisableXexch50];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.DisableXexch50] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Rfc2231EncodingEnabled
		{
			get
			{
				return (bool)this[ADAMTransportConfigContainerSchema.Rfc2231EncodingEnabled];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.Rfc2231EncodingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OpenDomainRoutingEnabled
		{
			get
			{
				return (bool)this[ADAMTransportConfigContainerSchema.OpenDomainRoutingEnabled];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.OpenDomainRoutingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> MaxReceiveSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this.GetNonAdamProperty(TransportConfigContainerSchema.MaxReceiveSize);
			}
			set
			{
				this[TransportConfigContainerSchema.MaxReceiveSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxRecipientEnvelopeLimit
		{
			get
			{
				return (Unlimited<int>)this.GetNonAdamProperty(TransportConfigContainerSchema.MaxRecipientEnvelopeLimit);
			}
			set
			{
				this[TransportConfigContainerSchema.MaxRecipientEnvelopeLimit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> MaxSendSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ADAMTransportConfigContainerSchema.MaxSendSize];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.MaxSendSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ShadowRedundancyEnabled
		{
			get
			{
				return !(bool)this[ADAMTransportConfigContainerSchema.ShadowRedundancyDisabled];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.ShadowRedundancyDisabled] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ShadowHeartbeatTimeoutInterval
		{
			get
			{
				return (EnhancedTimeSpan)this[ADAMTransportConfigContainerSchema.ShadowHeartbeatTimeoutInterval];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.ShadowHeartbeatTimeoutInterval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int ShadowHeartbeatRetryCount
		{
			get
			{
				return (int)this[ADAMTransportConfigContainerSchema.ShadowHeartbeatRetryCount];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.ShadowHeartbeatRetryCount] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ShadowHeartbeatFrequency
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportConfigContainerSchema.ShadowHeartbeatFrequency];
			}
			set
			{
				this[TransportConfigContainerSchema.ShadowHeartbeatFrequency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ShadowResubmitTimeSpan
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportConfigContainerSchema.ShadowResubmitTimeSpan];
			}
			set
			{
				this[TransportConfigContainerSchema.ShadowResubmitTimeSpan] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ShadowMessageAutoDiscardInterval
		{
			get
			{
				return (EnhancedTimeSpan)this[ADAMTransportConfigContainerSchema.ShadowMessageAutoDiscardInterval];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.ShadowMessageAutoDiscardInterval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExternalDelayDsnEnabled
		{
			get
			{
				return !(bool)this[ADAMTransportConfigContainerSchema.ExternalDelayDsnDisabled];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.ExternalDelayDsnDisabled] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public CultureInfo ExternalDsnDefaultLanguage
		{
			get
			{
				return (CultureInfo)this[ADAMTransportConfigContainerSchema.ExternalDsnDefaultLanguage];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.ExternalDsnDefaultLanguage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExternalDsnLanguageDetectionEnabled
		{
			get
			{
				return !(bool)this[ADAMTransportConfigContainerSchema.ExternalDsnLanguageDetectionDisabled];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.ExternalDsnLanguageDetectionDisabled] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize ExternalDsnMaxMessageAttachSize
		{
			get
			{
				return (ByteQuantifiedSize)this[ADAMTransportConfigContainerSchema.ExternalDsnMaxMessageAttachSize];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.ExternalDsnMaxMessageAttachSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpDomain ExternalDsnReportingAuthority
		{
			get
			{
				return (SmtpDomain)this[ADAMTransportConfigContainerSchema.ExternalDsnReportingAuthority];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.ExternalDsnReportingAuthority] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExternalDsnSendHtml
		{
			get
			{
				return !(bool)this[ADAMTransportConfigContainerSchema.ExternalDsnSendHtmlDisabled];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.ExternalDsnSendHtmlDisabled] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress? ExternalPostmasterAddress
		{
			get
			{
				return (SmtpAddress?)this[ADAMTransportConfigContainerSchema.ExternalPostmasterAddress];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.ExternalPostmasterAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool InternalDelayDsnEnabled
		{
			get
			{
				return !(bool)this[ADAMTransportConfigContainerSchema.InternalDelayDsnDisabled];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.InternalDelayDsnDisabled] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public CultureInfo InternalDsnDefaultLanguage
		{
			get
			{
				return (CultureInfo)this[ADAMTransportConfigContainerSchema.InternalDsnDefaultLanguage];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.InternalDsnDefaultLanguage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool InternalDsnLanguageDetectionEnabled
		{
			get
			{
				return !(bool)this[ADAMTransportConfigContainerSchema.InternalDsnLanguageDetectionDisabled];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.InternalDsnLanguageDetectionDisabled] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize InternalDsnMaxMessageAttachSize
		{
			get
			{
				return (ByteQuantifiedSize)this[ADAMTransportConfigContainerSchema.InternalDsnMaxMessageAttachSize];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.InternalDsnMaxMessageAttachSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpDomain InternalDsnReportingAuthority
		{
			get
			{
				return (SmtpDomain)this[ADAMTransportConfigContainerSchema.InternalDsnReportingAuthority];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.InternalDsnReportingAuthority] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool InternalDsnSendHtml
		{
			get
			{
				return !(bool)this[ADAMTransportConfigContainerSchema.InternalDsnSendHtmlDisabled];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.InternalDsnSendHtmlDisabled] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> SupervisionTags
		{
			get
			{
				return (MultiValuedProperty<string>)this.GetNonAdamProperty(TransportConfigContainerSchema.SupervisionTags);
			}
			set
			{
				this[TransportConfigContainerSchema.SupervisionTags] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public HygieneSuiteEnum HygieneSuite
		{
			get
			{
				return (HygieneSuiteEnum)this[ADAMTransportConfigContainerSchema.HygieneSuite];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.HygieneSuite] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MigrationEnabled
		{
			get
			{
				return (bool)this[ADAMTransportConfigContainerSchema.MigrationEnabled];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.MigrationEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool LegacyJournalingMigrationEnabled
		{
			get
			{
				return (bool)this[ADAMTransportConfigContainerSchema.LegacyJournalingMigrationEnabled];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.LegacyJournalingMigrationEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool LegacyArchiveJournalingEnabled
		{
			get
			{
				return (bool)this[ADAMTransportConfigContainerSchema.LegacyArchiveJournalingEnabled];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.LegacyArchiveJournalingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RedirectDLMessagesForLegacyArchiveJournaling
		{
			get
			{
				return (bool)this[ADAMTransportConfigContainerSchema.RedirectDLMessagesForLegacyArchiveJournaling];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.RedirectDLMessagesForLegacyArchiveJournaling] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RedirectUnprovisionedUserMessagesForLegacyArchiveJournaling
		{
			get
			{
				return (bool)this[ADAMTransportConfigContainerSchema.RedirectUnprovisionedUserMessagesForLegacyArchiveJournaling];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.RedirectUnprovisionedUserMessagesForLegacyArchiveJournaling] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool LegacyArchiveLiveJournalingEnabled
		{
			get
			{
				return (bool)this[ADAMTransportConfigContainerSchema.LegacyArchiveLiveJournalingEnabled];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.LegacyArchiveLiveJournalingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool JournalArchivingEnabled
		{
			get
			{
				return (bool)this[ADAMTransportConfigContainerSchema.JournalArchivingEnabled];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.JournalArchivingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RejectMessageOnShadowFailure
		{
			get
			{
				return (bool)this[ADAMTransportConfigContainerSchema.RejectMessageOnShadowFailure];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.RejectMessageOnShadowFailure] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ShadowMessagePreference ShadowMessagePreferenceSetting
		{
			get
			{
				return (ShadowMessagePreference)this[ADAMTransportConfigContainerSchema.ShadowMessagePreferenceSetting];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.ShadowMessagePreferenceSetting] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxRetriesForLocalSiteShadow
		{
			get
			{
				return (int)this[ADAMTransportConfigContainerSchema.MaxRetriesForLocalSiteShadow];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.MaxRetriesForLocalSiteShadow] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxRetriesForRemoteSiteShadow
		{
			get
			{
				return (int)this[ADAMTransportConfigContainerSchema.MaxRetriesForRemoteSiteShadow];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.MaxRetriesForRemoteSiteShadow] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan SafetyNetHoldTime
		{
			get
			{
				return (EnhancedTimeSpan)this[ADAMTransportConfigContainerSchema.SafetyNetHoldTime];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.SafetyNetHoldTime] = value;
			}
		}

		public MultiValuedProperty<string> TransportRuleConfig
		{
			get
			{
				return (MultiValuedProperty<string>)this[TransportConfigContainerSchema.TransportRuleConfig];
			}
			set
			{
				this[TransportConfigContainerSchema.TransportRuleConfig] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int TransportRuleCollectionAddedRecipientsLimit
		{
			get
			{
				return (int)this[TransportConfigContainerSchema.TransportRuleCollectionAddedRecipientsLimit];
			}
			set
			{
				this[TransportConfigContainerSchema.TransportRuleCollectionAddedRecipientsLimit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int TransportRuleLimit
		{
			get
			{
				return (int)this[TransportConfigContainerSchema.TransportRuleLimit];
			}
			set
			{
				this[TransportConfigContainerSchema.TransportRuleLimit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportRuleCollectionRegexCharsLimit
		{
			get
			{
				return (ByteQuantifiedSize)this[TransportConfigContainerSchema.TransportRuleCollectionRegexCharsLimit];
			}
			set
			{
				this[TransportConfigContainerSchema.TransportRuleCollectionRegexCharsLimit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportRuleSizeLimit
		{
			get
			{
				return (ByteQuantifiedSize)this[TransportConfigContainerSchema.TransportRuleSizeLimit];
			}
			set
			{
				this[TransportConfigContainerSchema.TransportRuleSizeLimit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportRuleAttachmentTextScanLimit
		{
			get
			{
				return (ByteQuantifiedSize)this[TransportConfigContainerSchema.TransportRuleAttachmentTextScanLimit];
			}
			set
			{
				this[TransportConfigContainerSchema.TransportRuleAttachmentTextScanLimit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TransportRuleRegexValidationTimeout
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportConfigContainerSchema.TransportRuleRegexValidationTimeout];
			}
			set
			{
				this[TransportConfigContainerSchema.TransportRuleRegexValidationTimeout] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Version TransportRuleMinProductVersion
		{
			get
			{
				return (Version)this[TransportConfigContainerSchema.TransportRuleMinProductVersion];
			}
			set
			{
				this[TransportConfigContainerSchema.TransportRuleMinProductVersion] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int AnonymousSenderToRecipientRatePerHour
		{
			get
			{
				return (int)this[TransportConfigContainerSchema.AnonymousSenderToRecipientRatePerHour];
			}
			set
			{
				this[TransportConfigContainerSchema.AnonymousSenderToRecipientRatePerHour] = value;
			}
		}

		public EnhancedTimeSpan QueueDiagnosticsAggregationInterval
		{
			get
			{
				return EnhancedTimeSpan.FromTicks((long)this[TransportConfigContainerSchema.QueueDiagnosticsAggregationInterval]);
			}
			set
			{
				this[TransportConfigContainerSchema.QueueDiagnosticsAggregationInterval] = value.Ticks;
			}
		}

		[Parameter(Mandatory = false)]
		public bool JournalReportDLMemberSubstitutionEnabled
		{
			get
			{
				return (bool)this[ADAMTransportConfigContainerSchema.JournalReportDLMemberSubstitutionEnabled];
			}
			set
			{
				this[ADAMTransportConfigContainerSchema.JournalReportDLMemberSubstitutionEnabled] = value;
			}
		}

		public int DiagnosticsAggregationServicePort
		{
			get
			{
				return (int)this[TransportConfigContainerSchema.DiagnosticsAggregationServicePort];
			}
			set
			{
				this[TransportConfigContainerSchema.DiagnosticsAggregationServicePort] = value;
			}
		}

		public bool AgentGeneratedMessageLoopDetectionInSubmissionEnabled
		{
			get
			{
				return (bool)this[TransportConfigContainerSchema.AgentGeneratedMessageLoopDetectionInSubmissionEnabled];
			}
			set
			{
				this[TransportConfigContainerSchema.AgentGeneratedMessageLoopDetectionInSubmissionEnabled] = value;
			}
		}

		public bool AgentGeneratedMessageLoopDetectionInSmtpEnabled
		{
			get
			{
				return (bool)this[TransportConfigContainerSchema.AgentGeneratedMessageLoopDetectionInSmtpEnabled];
			}
			set
			{
				this[TransportConfigContainerSchema.AgentGeneratedMessageLoopDetectionInSmtpEnabled] = value;
			}
		}

		public uint MaxAllowedAgentGeneratedMessageDepth
		{
			get
			{
				return (uint)this[TransportConfigContainerSchema.MaxAllowedAgentGeneratedMessageDepth];
			}
			set
			{
				this[TransportConfigContainerSchema.MaxAllowedAgentGeneratedMessageDepth] = value;
			}
		}

		public uint MaxAllowedAgentGeneratedMessageDepthPerAgent
		{
			get
			{
				return (uint)this[TransportConfigContainerSchema.MaxAllowedAgentGeneratedMessageDepthPerAgent];
			}
			set
			{
				this[TransportConfigContainerSchema.MaxAllowedAgentGeneratedMessageDepthPerAgent] = value;
			}
		}

		internal static object InternalHeaderPromotionModeGetter(IPropertyBag bag)
		{
			TransportSettingFlags transportSettingFlags = (TransportSettingFlags)bag[ADAMTransportConfigContainerSchema.Flags];
			HeaderPromotionMode headerPromotionMode = (HeaderPromotionMode)((transportSettingFlags & TransportSettingFlags.HeaderPromotionModeSetting) >> 21);
			return EnumValidator.IsValidValue<HeaderPromotionMode>(headerPromotionMode) ? headerPromotionMode : HeaderPromotionMode.NoCreate;
		}

		internal static void InternalHeaderPromotionModeSetter(object value, IPropertyBag bag)
		{
			TransportSettingFlags transportSettingFlags = (TransportSettingFlags)bag[ADAMTransportConfigContainerSchema.Flags] & ~TransportSettingFlags.HeaderPromotionModeSetting;
			TransportSettingFlags transportSettingFlags2 = (TransportSettingFlags)((int)value << 21 & 6291456);
			bag[ADAMTransportConfigContainerSchema.Flags] = (int)(transportSettingFlags2 | transportSettingFlags);
		}

		internal static object InternalDsnDefaultLanguageGetter(IPropertyBag propertyBag)
		{
			string cultureString = (string)propertyBag[ADAMTransportConfigContainerSchema.InternalDsnDefaultLanguageStr];
			return TransportConfigContainer.ConvertStringToDefaultDsnCulture(cultureString);
		}

		internal static void InternalDsnDefaultLanguageSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADAMTransportConfigContainerSchema.InternalDsnDefaultLanguageStr] = ((value != null) ? ((CultureInfo)value).ToString() : string.Empty);
		}

		internal static object ExternalDsnDefaultLanguageGetter(IPropertyBag propertyBag)
		{
			string cultureString = (string)propertyBag[ADAMTransportConfigContainerSchema.ExternalDsnDefaultLanguageStr];
			return TransportConfigContainer.ConvertStringToDefaultDsnCulture(cultureString);
		}

		internal static void ExternalDsnDefaultLanguageSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADAMTransportConfigContainerSchema.ExternalDsnDefaultLanguageStr] = ((value != null) ? ((CultureInfo)value).ToString() : string.Empty);
		}

		internal static object InternalShadowMessagePreferenceGetter(IPropertyBag bag)
		{
			TransportSettingFlags transportSettingFlags = (TransportSettingFlags)bag[ADAMTransportConfigContainerSchema.Flags];
			ShadowMessagePreference shadowMessagePreference = (ShadowMessagePreference)((transportSettingFlags & TransportSettingFlags.ShadowMessagePreferenceSetting) >> 27);
			return EnumValidator.IsValidValue<ShadowMessagePreference>(shadowMessagePreference) ? shadowMessagePreference : ShadowMessagePreference.PreferRemote;
		}

		internal static void InternalShadowMessagePreferenceSetter(object value, IPropertyBag bag)
		{
			TransportSettingFlags transportSettingFlags = (TransportSettingFlags)bag[ADAMTransportConfigContainerSchema.Flags] & ~TransportSettingFlags.ShadowMessagePreferenceSetting;
			TransportSettingFlags transportSettingFlags2 = (TransportSettingFlags)((int)value << 27 & 402653184);
			bag[ADAMTransportConfigContainerSchema.Flags] = (int)(transportSettingFlags2 | transportSettingFlags);
		}

		internal static CultureInfo ConvertStringToDefaultDsnCulture(string cultureString)
		{
			if (string.IsNullOrEmpty(cultureString))
			{
				return null;
			}
			CultureInfo result;
			try
			{
				result = CultureInfo.GetCultureInfo(cultureString);
			}
			catch (ArgumentException)
			{
				result = null;
			}
			return result;
		}

		internal bool IsTLSSendSecureDomain(string domainName)
		{
			if (this.tlsSendDomainSecureDictionary == null)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>(this.TLSSendDomainSecureList.Count * 2, StringComparer.OrdinalIgnoreCase);
				foreach (SmtpDomain smtpDomain in this.TLSSendDomainSecureList)
				{
					dictionary.Add(smtpDomain.Domain, null);
				}
				this.tlsSendDomainSecureDictionary = dictionary;
			}
			return this.tlsSendDomainSecureDictionary.Count > 0 && this.tlsSendDomainSecureDictionary.ContainsKey(domainName);
		}

		internal bool IsTLSReceiveSecureDomain(string domainName)
		{
			if (this.tlsReceiveDomainSecureDictionary == null)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>(this.TLSReceiveDomainSecureList.Count * 2, StringComparer.OrdinalIgnoreCase);
				foreach (SmtpDomain smtpDomain in this.TLSReceiveDomainSecureList)
				{
					dictionary.Add(smtpDomain.Domain, null);
				}
				this.tlsReceiveDomainSecureDictionary = dictionary;
			}
			return this.tlsReceiveDomainSecureDictionary.Count > 0 && this.tlsReceiveDomainSecureDictionary.ContainsKey(domainName);
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return TransportConfigContainer.mostDerivedClass;
			}
		}

		private object GetNonAdamProperty(ADPropertyDefinition propertyDefinition)
		{
			object result;
			if (TopologyProvider.IsAdamTopology())
			{
				result = propertyDefinition.DefaultValue;
			}
			else
			{
				result = this[propertyDefinition];
			}
			return result;
		}

		internal const int MaxDomainSecureListCount = 256;

		private static readonly TransportConfigContainerSchema adSchema = ObjectSchema.GetInstance<TransportConfigContainerSchema>();

		private static readonly ADAMTransportConfigContainerSchema adamSchema = ObjectSchema.GetInstance<ADAMTransportConfigContainerSchema>();

		private static string mostDerivedClass = "msExchTransportSettings";

		private Dictionary<string, object> tlsSendDomainSecureDictionary;

		private Dictionary<string, object> tlsReceiveDomainSecureDictionary;
	}
}
