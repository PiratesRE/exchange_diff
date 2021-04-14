using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal interface IThrottlingPolicy
	{
		bool IsFallback { get; }

		ThrottlingPolicyScopeType ThrottlingPolicyScope { get; }

		bool IsServiceAccount { get; }

		bool IsUnthrottled { get; }

		Unlimited<uint> AnonymousMaxConcurrency { get; }

		Unlimited<uint> AnonymousMaxBurst { get; }

		Unlimited<uint> AnonymousRechargeRate { get; }

		Unlimited<uint> AnonymousCutoffBalance { get; }

		Unlimited<uint> EasMaxConcurrency { get; }

		Unlimited<uint> EasMaxBurst { get; }

		Unlimited<uint> EasRechargeRate { get; }

		Unlimited<uint> EasCutoffBalance { get; }

		Unlimited<uint> EasMaxDevices { get; }

		Unlimited<uint> EasMaxDeviceDeletesPerMonth { get; }

		Unlimited<uint> EasMaxInactivityForDeviceCleanup { get; }

		Unlimited<uint> EwsMaxConcurrency { get; }

		Unlimited<uint> EwsMaxBurst { get; }

		Unlimited<uint> EwsRechargeRate { get; }

		Unlimited<uint> EwsCutoffBalance { get; }

		Unlimited<uint> EwsMaxSubscriptions { get; }

		Unlimited<uint> ImapMaxConcurrency { get; }

		Unlimited<uint> ImapMaxBurst { get; }

		Unlimited<uint> ImapRechargeRate { get; }

		Unlimited<uint> ImapCutoffBalance { get; }

		Unlimited<uint> OutlookServiceMaxConcurrency { get; }

		Unlimited<uint> OutlookServiceMaxBurst { get; }

		Unlimited<uint> OutlookServiceRechargeRate { get; }

		Unlimited<uint> OutlookServiceCutoffBalance { get; }

		Unlimited<uint> OutlookServiceMaxSubscriptions { get; }

		Unlimited<uint> OutlookServiceMaxSocketConnectionsPerDevice { get; }

		Unlimited<uint> OutlookServiceMaxSocketConnectionsPerUser { get; }

		Unlimited<uint> OwaMaxConcurrency { get; }

		Unlimited<uint> OwaMaxBurst { get; }

		Unlimited<uint> OwaRechargeRate { get; }

		Unlimited<uint> OwaCutoffBalance { get; }

		Unlimited<uint> OwaVoiceMaxConcurrency { get; }

		Unlimited<uint> OwaVoiceMaxBurst { get; }

		Unlimited<uint> OwaVoiceRechargeRate { get; }

		Unlimited<uint> OwaVoiceCutoffBalance { get; }

		Unlimited<uint> PopMaxConcurrency { get; }

		Unlimited<uint> PopMaxBurst { get; }

		Unlimited<uint> PopRechargeRate { get; }

		Unlimited<uint> PopCutoffBalance { get; }

		Unlimited<uint> RcaMaxConcurrency { get; }

		Unlimited<uint> RcaMaxBurst { get; }

		Unlimited<uint> RcaRechargeRate { get; }

		Unlimited<uint> RcaCutoffBalance { get; }

		Unlimited<uint> CpaMaxConcurrency { get; }

		Unlimited<uint> CpaMaxBurst { get; }

		Unlimited<uint> CpaRechargeRate { get; }

		Unlimited<uint> CpaCutoffBalance { get; }

		Unlimited<uint> PowerShellMaxConcurrency { get; }

		Unlimited<uint> PowerShellMaxBurst { get; }

		Unlimited<uint> PowerShellRechargeRate { get; }

		Unlimited<uint> PowerShellCutoffBalance { get; }

		Unlimited<uint> PowerShellMaxTenantConcurrency { get; }

		Unlimited<uint> PowerShellMaxOperations { get; }

		Unlimited<uint> PowerShellMaxCmdletsTimePeriod { get; }

		Unlimited<uint> PowerShellMaxCmdletQueueDepth { get; }

		Unlimited<uint> ExchangeMaxCmdlets { get; }

		Unlimited<uint> PowerShellMaxDestructiveCmdlets { get; }

		Unlimited<uint> PowerShellMaxDestructiveCmdletsTimePeriod { get; }

		Unlimited<uint> PowerShellMaxCmdlets { get; }

		Unlimited<uint> PowerShellMaxRunspaces { get; }

		Unlimited<uint> PowerShellMaxTenantRunspaces { get; }

		Unlimited<uint> PowerShellMaxRunspacesTimePeriod { get; }

		Unlimited<uint> PswsMaxConcurrency { get; }

		Unlimited<uint> PswsMaxRequest { get; }

		Unlimited<uint> PswsMaxRequestTimePeriod { get; }

		Unlimited<uint> MessageRateLimit { get; }

		Unlimited<uint> RecipientRateLimit { get; }

		Unlimited<uint> ForwardeeLimit { get; }

		Unlimited<uint> DiscoveryMaxConcurrency { get; }

		Unlimited<uint> DiscoveryMaxMailboxes { get; }

		Unlimited<uint> DiscoveryMaxKeywords { get; }

		Unlimited<uint> DiscoveryMaxPreviewSearchMailboxes { get; }

		Unlimited<uint> DiscoveryMaxStatsSearchMailboxes { get; }

		Unlimited<uint> DiscoveryPreviewSearchResultsPageSize { get; }

		Unlimited<uint> DiscoveryMaxKeywordsPerPage { get; }

		Unlimited<uint> DiscoveryMaxRefinerResults { get; }

		Unlimited<uint> DiscoveryMaxSearchQueueDepth { get; }

		Unlimited<uint> DiscoverySearchTimeoutPeriod { get; }

		Unlimited<uint> PushNotificationMaxConcurrency { get; }

		Unlimited<uint> PushNotificationMaxBurst { get; }

		Unlimited<uint> PushNotificationRechargeRate { get; }

		Unlimited<uint> PushNotificationCutoffBalance { get; }

		Unlimited<uint> PushNotificationMaxBurstPerDevice { get; }

		Unlimited<uint> PushNotificationRechargeRatePerDevice { get; }

		Unlimited<uint> PushNotificationSamplingPeriodPerDevice { get; }

		Unlimited<uint> EncryptionSenderMaxConcurrency { get; }

		Unlimited<uint> EncryptionSenderMaxBurst { get; }

		Unlimited<uint> EncryptionSenderRechargeRate { get; }

		Unlimited<uint> EncryptionSenderCutoffBalance { get; }

		Unlimited<uint> EncryptionRecipientMaxConcurrency { get; }

		Unlimited<uint> EncryptionRecipientMaxBurst { get; }

		Unlimited<uint> EncryptionRecipientRechargeRate { get; }

		Unlimited<uint> EncryptionRecipientCutoffBalance { get; }

		Unlimited<uint> ComplianceMaxExpansionDGRecipients { get; }

		Unlimited<uint> ComplianceMaxExpansionNestedDGs { get; }

		string GetIdentityString();

		string GetShortIdentityString();
	}
}
