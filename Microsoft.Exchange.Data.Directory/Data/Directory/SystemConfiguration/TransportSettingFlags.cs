using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum TransportSettingFlags
	{
		Empty = 0,
		KeepCategories = 1,
		LegacyArchiveLiveJournalingEnabled = 2,
		AddressBookPolicyRoutingEnabled = 4,
		VoicemailJournalingDisabled = 8,
		DisableXexch50 = 16,
		VerifySecureSubmitEnabled = 32,
		JournalReportDLMemberSubstitutionEnabled = 64,
		JournalArchivingEnabled = 128,
		Rfc2231EncodingEnabled = 256,
		ShadowRedundancyDisabled = 512,
		OpenDomainRoutingEnabled = 1024,
		ExternalDsnLanguageDetectionDisabled = 2048,
		ExternalDsnSendHtmlDisabled = 4096,
		InternalDelayDsnDisabled = 8192,
		InternalDsnLanguageDetectionDisabled = 16384,
		InternalDsnSendHtmlDisabled = 32768,
		ExternalDelayDsnDisabled = 65536,
		PreserveReportBodypart = 131072,
		ConvertReportToMessage = 262144,
		MigrationEnabled = 1048576,
		HeaderPromotionModeSetting = 6291456,
		LegacyJournalingMigrationEnabled = 8388608,
		ConvertDisclaimerWrapperToEml = 16777216,
		LegacyArchiveJournalingEnabled = 33554432,
		RejectMessageOnShadowFailure = 67108864,
		RedirectUnprovisionedUserMessagesForLegacyArchiveJournaling = 536870912,
		ShadowMessagePreferenceSetting = 402653184,
		RedirectDLMessagesForLegacyArchiveJournaling = 1073741824,
		All = 2146959359
	}
}
