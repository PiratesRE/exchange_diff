using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum UpdateGroupMailboxMetadata
	{
		[DisplayName("UGM", "Guid")]
		ExchangeGuid,
		[DisplayName("UGM", "ExtId")]
		ExternalDirectoryObjectId,
		[DisplayName("UGM", "ADSCT")]
		ADSessionCreateTime,
		[DisplayName("UGM", "AMC")]
		AddedMembersCount,
		[DisplayName("UGM", "RMC")]
		RemovedMembersCount,
		[DisplayName("UGM", "APMC")]
		AddedPendingMembersCount,
		[DisplayName("UGM", "RPMC")]
		RemovedPendingMembersCount,
		[DisplayName("UGM", "FC")]
		ForceConfigurationActionValue,
		[DisplayName("UGM", "GADT")]
		GroupAdLookupTime,
		[DisplayName("UGM", "EUT")]
		ExecutingUserLookupTime,
		[DisplayName("UGM", "LT")]
		MailboxLogonTime,
		[DisplayName("UGM", "RMT")]
		ResolveMembersTime,
		[DisplayName("UGM", "MT")]
		SetMembershipTime,
		[DisplayName("UGM", "ADUserCached")]
		IsPopulateADUserInCacheSuccessful,
		[DisplayName("UGM", "MiniRecipCached")]
		IsPopulateMiniRecipientInCacheSuccessful,
		[DisplayName("UGM", "GPUT")]
		GroupPhotoUploadTime,
		[DisplayName("UGM", "ERPT")]
		ExchangeResourcePublishTime,
		[DisplayName("UGM", "CFGExe")]
		IsConfigurationExecuted,
		[DisplayName("UGM", "CFGWarn")]
		ConfigurationWarnings,
		[DisplayName("UGM", "SRST")]
		SetRegionalSettingsTime,
		[DisplayName("UGM", "DFT")]
		CreateDefaultFoldersTime,
		[DisplayName("UGM", "DFTC")]
		CreateDefaultFoldersCount,
		[DisplayName("UGM", "ACL")]
		SetFolderPermissionsTime,
		[DisplayName("UGM", "ACLC")]
		SetFolderPermissionsCount,
		[DisplayName("UGM", "CALT")]
		ConfigureCalendarTime,
		[DisplayName("UGM", "WMT")]
		SendWelcomeMessageTime,
		[DisplayName("UGM", "CFG")]
		AdditionalConfigurationDetails
	}
}
