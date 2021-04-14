using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	public enum OwaUserConfigurationLogMetadata
	{
		[DisplayName("OUC", "AGG")]
		AggregationStats,
		[DisplayName("OUC", "UC")]
		UserCulture,
		[DisplayName("OUC", "UOT")]
		UserOptionsLoadTime,
		[DisplayName("OUC", "UORC")]
		UserOptionsLoadRpcCount,
		[DisplayName("OUC", "UORL")]
		UserOptionsLoadRpcLatency,
		[DisplayName("OUC", "UORLS")]
		UserOptionsLoadRpcLatencyOnStore,
		[DisplayName("OUC", "UOCpu")]
		UserOptionsLoadCPUTime,
		[DisplayName("OUC", "WHT")]
		WorkingHoursTime,
		[DisplayName("OUC", "WHRC")]
		WorkingHoursRpcCount,
		[DisplayName("OUC", "WHRL")]
		WorkingHoursRpcLatency,
		[DisplayName("OUC", "WHRLS")]
		WorkingHoursRpcLatencyOnStore,
		[DisplayName("OUC", "WHCpu")]
		WorkingHoursCPUTime,
		[DisplayName("OUC", "RT")]
		ReminderTime,
		[DisplayName("OUC", "RRC")]
		ReminderRpcCount,
		[DisplayName("OUC", "RRL")]
		ReminderRpcLatency,
		[DisplayName("OUC", "RRLS")]
		ReminderRpcLatencyOnStore,
		[DisplayName("OUC", "RCpu")]
		ReminderCPUTime,
		[DisplayName("OUC", "SST")]
		SessionSettingsMiscTime,
		[DisplayName("OUC", "SSRC")]
		SessionSettingsMiscRpcCount,
		[DisplayName("OUC", "SSRL")]
		SessionSettingsMiscRpcLatency,
		[DisplayName("OUC", "SSRLS")]
		SessionSettingsMiscRpcLatencyOnStore,
		[DisplayName("OUC", "SSCpu")]
		SessionSettingsMiscCPUTime,
		[DisplayName("OUC", "SSmmsT")]
		SessionSettingsMessageSizeTime,
		[DisplayName("OUC", "SSmmsRC")]
		SessionSettingsMessageSizeRpcCount,
		[DisplayName("OUC", "SSmmsRL")]
		SessionSettingsMessageSizeRpcLatency,
		[DisplayName("OUC", "SSmmsRLS")]
		SessionSettingsMessageSizeRpcLatencyOnStore,
		[DisplayName("OUC", "SSmmsCpu")]
		SessionSettingsMessageSizeCPUTime,
		[DisplayName("OUC", "SSplT")]
		SessionSettingsIsPublicLogonTime,
		[DisplayName("OUC", "SSplRC")]
		SessionSettingsPublicLogonRpcCount,
		[DisplayName("OUC", "SSplRL")]
		SessionSettingsPublicLogonRpcLatency,
		[DisplayName("OUC", "SSplRLS")]
		SessionSettingsPublicLogonRpcLatencyOnStore,
		[DisplayName("OUC", "SSplCpu")]
		SessionSettingsPublicLogonCPUTime,
		[DisplayName("OUC", "TMT")]
		TeamMailboxTime,
		[DisplayName("OUC", "TMRC")]
		TeamMailboxRpcCount,
		[DisplayName("OUC", "TMRL")]
		TeamMailboxRpcLatency,
		[DisplayName("OUC", "TMRLS")]
		TeamMailboxRpcLatencyOnStore,
		[DisplayName("OUC", "TMCpu")]
		TeamMailboxCPUTime,
		[DisplayName("OUC", "MRT")]
		MiniRecipientTime,
		[DisplayName("OUC", "MRRC")]
		MiniRecipientRpcCount,
		[DisplayName("OUC", "MRRL")]
		MiniRecipientRpcLatency,
		[DisplayName("OUC", "MRRLS")]
		MiniRecipientRpcLatencyOnStore,
		[DisplayName("OUC", "MRCpu")]
		MiniRecipientCPUTime,
		[DisplayName("OUC", "DFT")]
		DefaultFolderTime,
		[DisplayName("OUC", "DFRC")]
		DefaultFolderRpcCount,
		[DisplayName("OUC", "DFRL")]
		DefaultFolderRpcLatency,
		[DisplayName("OUC", "DFRLS")]
		DefaultFolderRpcLatencyOnStore,
		[DisplayName("OUC", "DFCpu")]
		DefaultFolderCPUTime,
		[DisplayName("OUC", "UMT")]
		UMClientTime,
		[DisplayName("OUC", "UMRC")]
		UMClientRpcCount,
		[DisplayName("OUC", "UMRL")]
		UMClientRpcLatency,
		[DisplayName("OUC", "UMRLS")]
		UMClientRpcLatencyOnStore,
		[DisplayName("OUC", "UMCpu")]
		UMClientCPUTime,
		[DisplayName("OUC", "DCT")]
		IsDatacenterModeTime,
		[DisplayName("OUC", "DCRC")]
		IsDatacenterModeRpcCount,
		[DisplayName("OUC", "DCRL")]
		IsDatacenterModeRpcLatency,
		[DisplayName("OUC", "DCRLS")]
		IsDatacenterModeRpcLatencyOnStore,
		[DisplayName("OUC", "DCCpu")]
		IsDatacenterModeCPUTime,
		[DisplayName("OUC", "VST")]
		ViewStateTime,
		[DisplayName("OUC", "VSRC")]
		ViewStateRpcCount,
		[DisplayName("OUC", "VSRL")]
		ViewStateRpcLatency,
		[DisplayName("OUC", "VSRLS")]
		ViewStateRpcLatencyOnStore,
		[DisplayName("OUC", "VSCpu")]
		ViewStateCPUTime,
		[DisplayName("OUC", "MTT")]
		MailTipsTime,
		[DisplayName("OUC", "MTRC")]
		MailTipsRpcCount,
		[DisplayName("OUC", "MTRL")]
		MailTipsRpcLatency,
		[DisplayName("OUC", "MTRLS")]
		MailTipsRpcLatencyOnStore,
		[DisplayName("OUC", "MTCpu")]
		MailTipsCPUTime,
		[DisplayName("OUC", "RPT")]
		RetentionPolicyTime,
		[DisplayName("OUC", "RPRC")]
		RetentionPolicyRpcCount,
		[DisplayName("OUC", "RPRL")]
		RetentionPolicyRpcLatency,
		[DisplayName("OUC", "RPRLS")]
		RetentionPolicyRpcLatencyOnStore,
		[DisplayName("OUC", "RPCpu")]
		RetentionPolicyCPUTime,
		[DisplayName("OUC", "MCT")]
		MasterCategoryListTime,
		[DisplayName("OUC", "MCRC")]
		MasterCategoryListRpcCount,
		[DisplayName("OUC", "MCRL")]
		MasterCategoryListRpcLatency,
		[DisplayName("OUC", "MCRLS")]
		MasterCategoryListRpcLatencyOnStore,
		[DisplayName("OUC", "MCCpu")]
		MasterCategoryListCPUTime,
		[DisplayName("OUC", "MT")]
		MaxRecipientsTime,
		[DisplayName("OUC", "MRC")]
		MaxRecipientsRpcCount,
		[DisplayName("OUC", "MRL")]
		MaxRecipientsRpcLatency,
		[DisplayName("OUC", "MRLS")]
		MaxRecipientsRpcLatencyOnStore,
		[DisplayName("OUC", "MCpu")]
		MaxRecipientsCPUTime,
		[DisplayName("OUC", "PST")]
		PolicySettingsTime,
		[DisplayName("OUC", "PSRC")]
		PolicySettingsRpcCount,
		[DisplayName("OUC", "PSRL")]
		PolicySettingsRpcLatency,
		[DisplayName("OUC", "PSRLS")]
		PolicySettingsRpcLatencyOnStore,
		[DisplayName("OUC", "PSCpu")]
		PolicySettingsCPUTime,
		[DisplayName("OUC", "SEST")]
		SessionSettingsTime,
		[DisplayName("OUC", "SESRC")]
		SessionSettingsRpcCount,
		[DisplayName("OUC", "SESRL")]
		SessionSettingsRpcLatency,
		[DisplayName("OUC", "SESRLS")]
		SessionSettingsRpcLatencyOnStore,
		[DisplayName("OUC", "SESCpu")]
		SessionSettingsCPUTime,
		[DisplayName("OUC", "CCT")]
		ConfigContextTime,
		[DisplayName("OUC", "CCRC")]
		ConfigContextRpcCount,
		[DisplayName("OUC", "CCRL")]
		ConfigContextRpcLatency,
		[DisplayName("OUC", "CCRLS")]
		ConfigContextRpcLatencyOnStore,
		[DisplayName("OUC", "CCCpu")]
		ConfigContextCPUTime,
		[DisplayName("OUC", "SEGT")]
		SegmentationSettingsTime,
		[DisplayName("OUC", "SEGRC")]
		SegmentationSettingsRpcCount,
		[DisplayName("OUC", "SEGRL")]
		SegmentationSettingsRpcLatency,
		[DisplayName("OUC", "SEGRLS")]
		SegmentationSettingsRpcLatencyOnStore,
		[DisplayName("OUC", "SEGCpu")]
		SegmentationSettingsCPUTime,
		[DisplayName("OUC", "ACHT")]
		AttachmentPolicyTime,
		[DisplayName("OUC", "ACHRC")]
		AttachmentPolicyRpcCount,
		[DisplayName("OUC", "ACHRL")]
		AttachmentPolicyRpcLatency,
		[DisplayName("OUC", "ACHRLS")]
		AttachmentPolicyRpcLatencyOnStore,
		[DisplayName("OUC", "ACHCpu")]
		AttachmentPolicyCPUTime,
		[DisplayName("OUC", "PWT")]
		PlacesWeatherTime,
		[DisplayName("OUC", "PWRC")]
		PlacesWeatherRpcCount,
		[DisplayName("OUC", "PWRL")]
		PlacesWeatherRpcLatency,
		[DisplayName("OUC", "PWRLS")]
		PlacesWeatherRpcLatencyOnStore,
		[DisplayName("OUC", "PWCpu")]
		PlacesWeatherCPUTime,
		[DisplayName("OUC", "GRP")]
		GlobalReadingPanePosition,
		[DisplayName("OUC", "CSO")]
		ConversationSortOrder,
		[DisplayName("OUC", "HDI")]
		HideDeletedItems,
		[DisplayName("OUC", "FFTC")]
		IsFavoritesFolderTreeCollapsed,
		[DisplayName("OUC", "MRTC")]
		IsMailRootFolderTreeCollapsed,
		[DisplayName("OUC", "MFPE")]
		MailFolderPaneExpanded,
		[DisplayName("OUC", "PTLV")]
		ShowPreviewTextInListView,
		[DisplayName("OUC", "SSTLV")]
		ShowSenderOnTopInListView,
		[DisplayName("OUC", "RPFL")]
		ShowReadingPaneOnFirstLoad,
		[DisplayName("OUC", "NPVO")]
		NavigationPaneViewOption,
		[DisplayName("OUC", "EDI")]
		EmptyDeletedItemsOnLogoff,
		[DisplayName("OUC", "IOA")]
		IsOptimizedForAccessibility,
		[DisplayName("OUC", "PIKFC")]
		IsPeopleIKnowFolderTreeCollapsed,
		[DisplayName("OUC", "MRDT")]
		MarkAsReadDelaytime,
		[DisplayName("OUC", "NS")]
		NextSelection,
		[DisplayName("OUC", "PMR")]
		PreviewMarkAsRead,
		[DisplayName("OUC", "PNC")]
		PrimaryNavigationCollapsed,
		[DisplayName("OUC", "RJS")]
		ReportJunkSelected,
		[DisplayName("OUC", "SIUE")]
		ShowInferenceUiElements,
		[DisplayName("OUC", "STIL")]
		ShowTreeInListView,
		[DisplayName("OUC", "PIKU")]
		PeopleIKnowUse,
		[DisplayName("OUC", "SS")]
		SearchScope,
		[DisplayName("OUC", "GFVS")]
		GlobalFolderViewState,
		[DisplayName("OUC", "AFI")]
		ArchiveFolderId,
		[DisplayName("OUC", "RTIC")]
		IsRenewTimeIndexComplete,
		[DisplayName("OUC", "DPFT")]
		DefaultPublicFolderMailboxTime,
		[DisplayName("OUC", "DPFRC")]
		DefaultPublicFolderMailboxRpcCount,
		[DisplayName("OUC", "DPFRL")]
		DefaultPublicFolderMailboxRpcLatency,
		[DisplayName("OUC", "DPFRLS")]
		DefaultPublicFolderMailboxRpcLatencyOnStore,
		[DisplayName("OUC", "DPFCpu")]
		DefaultPublicFolderMailboxCPUTime,
		[DisplayName("OUC", "DPFE")]
		DefaultPublicFolderMailboxError
	}
}
