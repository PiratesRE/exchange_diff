using System;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Data
{
	internal static class ThrottlingPolicyDefaults
	{
		static ThrottlingPolicyDefaults()
		{
			bool flag = Datacenter.IsMultiTenancyEnabled();
			if (flag)
			{
				ThrottlingPolicyDefaults.SetAnonymousDefaults(1U, 120000U, 420000U, 720000U);
				ThrottlingPolicyDefaults.SetEasDefaults(4U, 480000U, 1800000U, 600000U, 100U, 20U, 180U);
				ThrottlingPolicyDefaults.SetEwsDefaults(27U, 300000U, 900000U, 3000000U, 20U);
				ThrottlingPolicyDefaults.SetImapDefaults(20U, 3600000U, 600000U, 600000U);
				ThrottlingPolicyDefaults.SetOutlookServiceDefaults(27U, 300000U, 900000U, 3000000U, 20U, 4U, 12U);
				ThrottlingPolicyDefaults.SetOwaDefaults(20U, 480000U, 1800000U, Unlimited<uint>.UnlimitedValue, 3U, 75000U, 375000U, 525000U);
				ThrottlingPolicyDefaults.SetPopDefaults(20U, 3600000U, 600000U, 600000U);
				ThrottlingPolicyDefaults.SetPowerShellDefaults(3U, 9U, 600000U, 1800000U, 3000000U, 400U, 5U, 25U, 50U, 120U, 60U, 200U, 5U, 18U, 60U);
				ThrottlingPolicyDefaults.SetPswsDefaults(3U, 25U, 5U);
				ThrottlingPolicyDefaults.SetRcaDefaults(40U, 150000U, 900000U, Unlimited<uint>.UnlimitedValue);
				ThrottlingPolicyDefaults.SetCpaDefaults(20U, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue);
				ThrottlingPolicyDefaults.SetDiscoveryDefaults(2U, 5000U, 500U, 5000U, 100U, 200U, 25U, 10U, 32U, 10U);
				ThrottlingPolicyDefaults.SetGeneralDefaults(30U, 10000U, 10U);
				ThrottlingPolicyDefaults.SetPushNotificationDefaults(10U, 30000U, 120000U, 240000U, 10U, 6U, 600000U);
				ThrottlingPolicyDefaults.SetE4eSenderDefaults(200U, 4800000U, 18000000U, Unlimited<uint>.UnlimitedValue);
				ThrottlingPolicyDefaults.SetE4eRecipientDefaults(20U, 480000U, 1800000U, Unlimited<uint>.UnlimitedValue);
				ThrottlingPolicyDefaults.SetComplianceDefaults(10000U, 25U);
				return;
			}
			ThrottlingPolicyDefaults.SetAnonymousDefaults(1U, 120000U, 420000U, 720000U);
			ThrottlingPolicyDefaults.SetEasDefaults(10U, 480000U, 1800000U, 600000U, 100U, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue);
			ThrottlingPolicyDefaults.SetEwsDefaults(27U, 300000U, 900000U, 3000000U, 5000U);
			ThrottlingPolicyDefaults.SetImapDefaults(Unlimited<uint>.UnlimitedValue, 3600000U, 600000U, Unlimited<uint>.UnlimitedValue);
			ThrottlingPolicyDefaults.SetOutlookServiceDefaults(27U, 300000U, 900000U, 3000000U, 5000U, 4U, 12U);
			ThrottlingPolicyDefaults.SetOwaDefaults(20U, 480000U, 1800000U, Unlimited<uint>.UnlimitedValue, 3U, 75000U, 375000U, 525000U);
			ThrottlingPolicyDefaults.SetPopDefaults(20U, 3600000U, 600000U, Unlimited<uint>.UnlimitedValue);
			ThrottlingPolicyDefaults.SetPowerShellDefaults(18U, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue);
			ThrottlingPolicyDefaults.SetPswsDefaults(18U, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue);
			ThrottlingPolicyDefaults.SetRcaDefaults(40U, 150000U, 900000U, Unlimited<uint>.UnlimitedValue);
			ThrottlingPolicyDefaults.SetCpaDefaults(20U, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue);
			ThrottlingPolicyDefaults.SetDiscoveryDefaults(2U, 5000U, 500U, 5000U, 100U, 200U, 25U, 10U, 32U, 10U);
			ThrottlingPolicyDefaults.SetGeneralDefaults(Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue);
			ThrottlingPolicyDefaults.SetPushNotificationDefaults(20U, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue, Unlimited<uint>.UnlimitedValue, 10U, 6U, 600000U);
			ThrottlingPolicyDefaults.SetE4eSenderDefaults(200U, 4800000U, 18000000U, Unlimited<uint>.UnlimitedValue);
			ThrottlingPolicyDefaults.SetE4eRecipientDefaults(20U, 480000U, 1800000U, Unlimited<uint>.UnlimitedValue);
			ThrottlingPolicyDefaults.SetComplianceDefaults(10000U, 25U);
		}

		private static void SetEasDefaults(Unlimited<uint> maxConcurrency, Unlimited<uint> maxBurst, Unlimited<uint> rechargeRate, Unlimited<uint> cutoffBalance, Unlimited<uint> maxDevices, Unlimited<uint> maxDeviceDeletesPerMonth, Unlimited<uint> maxInactivityForDeviceCleanup)
		{
			ThrottlingPolicyDefaults.EasMaxConcurrency = maxConcurrency;
			ThrottlingPolicyDefaults.EasMaxBurst = maxBurst;
			ThrottlingPolicyDefaults.EasRechargeRate = rechargeRate;
			ThrottlingPolicyDefaults.EasCutoffBalance = cutoffBalance;
			ThrottlingPolicyDefaults.EasMaxDevices = maxDevices;
			ThrottlingPolicyDefaults.EasMaxDeviceDeletesPerMonth = maxDeviceDeletesPerMonth;
			ThrottlingPolicyDefaults.EasMaxInactivityForDeviceCleanup = maxInactivityForDeviceCleanup;
		}

		private static void SetEwsDefaults(Unlimited<uint> maxConcurrency, Unlimited<uint> maxBurst, Unlimited<uint> rechargeRate, Unlimited<uint> cutoffBalance, Unlimited<uint> maxSubscriptions)
		{
			ThrottlingPolicyDefaults.EwsMaxConcurrency = maxConcurrency;
			ThrottlingPolicyDefaults.EwsMaxBurst = maxBurst;
			ThrottlingPolicyDefaults.EwsRechargeRate = rechargeRate;
			ThrottlingPolicyDefaults.EwsCutoffBalance = cutoffBalance;
			ThrottlingPolicyDefaults.EwsMaxSubscriptions = maxSubscriptions;
		}

		private static void SetImapDefaults(Unlimited<uint> maxConcurrency, Unlimited<uint> maxBurst, Unlimited<uint> rechargeRate, Unlimited<uint> cutoffBalance)
		{
			ThrottlingPolicyDefaults.ImapMaxConcurrency = maxConcurrency;
			ThrottlingPolicyDefaults.ImapMaxBurst = maxBurst;
			ThrottlingPolicyDefaults.ImapRechargeRate = rechargeRate;
			ThrottlingPolicyDefaults.ImapCutoffBalance = cutoffBalance;
		}

		private static void SetOutlookServiceDefaults(Unlimited<uint> maxConcurrency, Unlimited<uint> maxBurst, Unlimited<uint> rechargeRate, Unlimited<uint> cutoffBalance, Unlimited<uint> maxSubscriptions, Unlimited<uint> maxSocketConnectionsPerDevice, Unlimited<uint> maxSocketConnectionsPerUser)
		{
			ThrottlingPolicyDefaults.OutlookServiceMaxConcurrency = maxConcurrency;
			ThrottlingPolicyDefaults.OutlookServiceMaxBurst = maxBurst;
			ThrottlingPolicyDefaults.OutlookServiceRechargeRate = rechargeRate;
			ThrottlingPolicyDefaults.OutlookServiceCutoffBalance = cutoffBalance;
			ThrottlingPolicyDefaults.OutlookServiceMaxSubscriptions = maxSubscriptions;
			ThrottlingPolicyDefaults.OutlookServiceMaxSocketConnectionsPerDevice = maxSocketConnectionsPerDevice;
			ThrottlingPolicyDefaults.OutlookServiceMaxSocketConnectionsPerUser = maxSocketConnectionsPerUser;
		}

		private static void SetOwaDefaults(Unlimited<uint> maxConcurrency, Unlimited<uint> maxBurst, Unlimited<uint> rechargeRate, Unlimited<uint> cutoffBalance, Unlimited<uint> voiceMaxConcurrency, Unlimited<uint> voiceMaxBurst, Unlimited<uint> voiceRechargeRate, Unlimited<uint> voiceCutoffBalance)
		{
			ThrottlingPolicyDefaults.OwaMaxConcurrency = maxConcurrency;
			ThrottlingPolicyDefaults.OwaMaxBurst = maxBurst;
			ThrottlingPolicyDefaults.OwaRechargeRate = rechargeRate;
			ThrottlingPolicyDefaults.OwaCutoffBalance = cutoffBalance;
			ThrottlingPolicyDefaults.OwaVoiceMaxConcurrency = voiceMaxConcurrency;
			ThrottlingPolicyDefaults.OwaVoiceMaxBurst = voiceMaxBurst;
			ThrottlingPolicyDefaults.OwaVoiceRechargeRate = voiceRechargeRate;
			ThrottlingPolicyDefaults.OwaVoiceCutoffBalance = voiceCutoffBalance;
		}

		private static void SetPopDefaults(Unlimited<uint> maxConcurrency, Unlimited<uint> maxBurst, Unlimited<uint> rechargeRate, Unlimited<uint> cutoffBalance)
		{
			ThrottlingPolicyDefaults.PopMaxConcurrency = maxConcurrency;
			ThrottlingPolicyDefaults.PopMaxBurst = maxBurst;
			ThrottlingPolicyDefaults.PopRechargeRate = rechargeRate;
			ThrottlingPolicyDefaults.PopCutoffBalance = cutoffBalance;
		}

		private static void SetPowerShellDefaults(Unlimited<uint> maxConcurrency, Unlimited<uint> maxTenantConcurrency, Unlimited<uint> maxBurst, Unlimited<uint> rechargeRate, Unlimited<uint> cutoffBalance, Unlimited<uint> maxOperations, Unlimited<uint> maxCmdletsTimePeriod, Unlimited<uint> exchangeMaxCmdlets, Unlimited<uint> maxCmdletQueueDepth, Unlimited<uint> maxDestructiveCmdlets, Unlimited<uint> maxDestructiveCmdletsTimePeriod, Unlimited<uint> maxCmdlets, Unlimited<uint> maxRunspaces, Unlimited<uint> maxTenantRunspaces, Unlimited<uint> maxRunspacesTimePeriod)
		{
			ThrottlingPolicyDefaults.PowerShellMaxConcurrency = maxConcurrency;
			ThrottlingPolicyDefaults.PowerShellMaxTenantConcurrency = maxTenantConcurrency;
			ThrottlingPolicyDefaults.PowerShellMaxBurst = maxBurst;
			ThrottlingPolicyDefaults.PowerShellRechargeRate = rechargeRate;
			ThrottlingPolicyDefaults.PowerShellCutoffBalance = cutoffBalance;
			ThrottlingPolicyDefaults.PowerShellMaxOperations = maxOperations;
			ThrottlingPolicyDefaults.PowerShellMaxCmdletsTimePeriod = maxCmdletsTimePeriod;
			ThrottlingPolicyDefaults.ExchangeMaxCmdlets = exchangeMaxCmdlets;
			ThrottlingPolicyDefaults.PowerShellMaxCmdletQueueDepth = maxCmdletQueueDepth;
			ThrottlingPolicyDefaults.PowerShellMaxDestructiveCmdlets = maxDestructiveCmdlets;
			ThrottlingPolicyDefaults.PowerShellMaxDestructiveCmdletsTimePeriod = maxDestructiveCmdletsTimePeriod;
			ThrottlingPolicyDefaults.PowerShellMaxCmdlets = maxCmdlets;
			ThrottlingPolicyDefaults.PowerShellMaxRunspaces = maxRunspaces;
			ThrottlingPolicyDefaults.PowerShellMaxTenantRunspaces = maxTenantRunspaces;
			ThrottlingPolicyDefaults.PowerShellMaxRunspacesTimePeriod = maxRunspacesTimePeriod;
		}

		private static void SetPswsDefaults(Unlimited<uint> maxConcurrency, Unlimited<uint> maxRequest, Unlimited<uint> maxRequestTimePeriod)
		{
			ThrottlingPolicyDefaults.PswsMaxConcurrency = maxConcurrency;
			ThrottlingPolicyDefaults.PswsMaxRequest = maxRequest;
			ThrottlingPolicyDefaults.PswsMaxRequestTimePeriod = maxRequestTimePeriod;
		}

		private static void SetAnonymousDefaults(Unlimited<uint> maxConcurrency, Unlimited<uint> maxBurst, Unlimited<uint> rechargeRate, Unlimited<uint> cutoffBalance)
		{
			ThrottlingPolicyDefaults.AnonymousMaxConcurrency = maxConcurrency;
			ThrottlingPolicyDefaults.AnonymousMaxBurst = maxBurst;
			ThrottlingPolicyDefaults.AnonymousRechargeRate = rechargeRate;
			ThrottlingPolicyDefaults.AnonymousCutoffBalance = cutoffBalance;
		}

		private static void SetRcaDefaults(Unlimited<uint> maxConcurrency, Unlimited<uint> maxBurst, Unlimited<uint> rechargeRate, Unlimited<uint> cutoffBalance)
		{
			ThrottlingPolicyDefaults.RcaMaxConcurrency = maxConcurrency;
			ThrottlingPolicyDefaults.RcaMaxBurst = maxBurst;
			ThrottlingPolicyDefaults.RcaRechargeRate = rechargeRate;
			ThrottlingPolicyDefaults.RcaCutoffBalance = cutoffBalance;
		}

		private static void SetCpaDefaults(Unlimited<uint> maxConcurrency, Unlimited<uint> maxBurst, Unlimited<uint> rechargeRate, Unlimited<uint> cutoffBalance)
		{
			ThrottlingPolicyDefaults.CpaMaxConcurrency = maxConcurrency;
			ThrottlingPolicyDefaults.CpaMaxBurst = maxBurst;
			ThrottlingPolicyDefaults.CpaRechargeRate = rechargeRate;
			ThrottlingPolicyDefaults.CpaCutoffBalance = cutoffBalance;
		}

		private static void SetGeneralDefaults(Unlimited<uint> messageRateLimit, Unlimited<uint> recipientRateLimit, Unlimited<uint> forwardeeLimit)
		{
			ThrottlingPolicyDefaults.MessageRateLimit = messageRateLimit;
			ThrottlingPolicyDefaults.RecipientRateLimit = recipientRateLimit;
			ThrottlingPolicyDefaults.ForwardeeLimit = forwardeeLimit;
		}

		private static void SetDiscoveryDefaults(Unlimited<uint> discoveryMaxConcurrency, Unlimited<uint> discoveryMaxMailboxes, Unlimited<uint> discoveryMaxKeywords, Unlimited<uint> discoveryMaxPreviewSearchMailboxes, Unlimited<uint> discoveryMaxStatsSearchMailboxes, Unlimited<uint> discoveryPreviewSearchResultsPageSize, Unlimited<uint> discoveryMaxKeywordsPerPage, Unlimited<uint> discoveryMaxRefinerResults, Unlimited<uint> discoveryMaxSearchQueueDepth, Unlimited<uint> discoverySearchTimeoutPeriod)
		{
			ThrottlingPolicyDefaults.DiscoveryMaxConcurrency = discoveryMaxConcurrency;
			ThrottlingPolicyDefaults.DiscoveryMaxMailboxes = discoveryMaxMailboxes;
			ThrottlingPolicyDefaults.DiscoveryMaxKeywords = discoveryMaxKeywords;
			ThrottlingPolicyDefaults.DiscoveryMaxPreviewSearchMailboxes = discoveryMaxPreviewSearchMailboxes;
			ThrottlingPolicyDefaults.DiscoveryMaxStatsSearchMailboxes = discoveryMaxStatsSearchMailboxes;
			ThrottlingPolicyDefaults.DiscoveryPreviewSearchResultsPageSize = discoveryPreviewSearchResultsPageSize;
			ThrottlingPolicyDefaults.DiscoveryMaxKeywordsPerPage = discoveryMaxKeywordsPerPage;
			ThrottlingPolicyDefaults.DiscoveryMaxRefinerResults = discoveryMaxRefinerResults;
			ThrottlingPolicyDefaults.DiscoveryMaxSearchQueueDepth = discoveryMaxSearchQueueDepth;
			ThrottlingPolicyDefaults.DiscoverySearchTimeoutPeriod = discoverySearchTimeoutPeriod;
		}

		private static void SetPushNotificationDefaults(Unlimited<uint> maxConcurrency, Unlimited<uint> maxBurst, Unlimited<uint> rechargeRate, Unlimited<uint> cutoffBalance, Unlimited<uint> maxBurstPerDevice, Unlimited<uint> rechargeRatePerDevice, Unlimited<uint> samplingPeriodPerDevice)
		{
			ThrottlingPolicyDefaults.PushNotificationMaxConcurrency = maxConcurrency;
			ThrottlingPolicyDefaults.PushNotificationMaxBurst = maxBurst;
			ThrottlingPolicyDefaults.PushNotificationRechargeRate = rechargeRate;
			ThrottlingPolicyDefaults.PushNotificationCutoffBalance = cutoffBalance;
			ThrottlingPolicyDefaults.PushNotificationMaxBurstPerDevice = maxBurstPerDevice;
			ThrottlingPolicyDefaults.PushNotificationRechargeRatePerDevice = rechargeRatePerDevice;
			ThrottlingPolicyDefaults.PushNotificationSamplingPeriodPerDevice = samplingPeriodPerDevice;
		}

		private static void SetE4eSenderDefaults(Unlimited<uint> maxConcurrency, Unlimited<uint> maxBurst, Unlimited<uint> rechargeRate, Unlimited<uint> cutoffBalance)
		{
			ThrottlingPolicyDefaults.EncryptionSenderMaxConcurrency = maxConcurrency;
			ThrottlingPolicyDefaults.EncryptionSenderMaxBurst = maxBurst;
			ThrottlingPolicyDefaults.EncryptionSenderRechargeRate = rechargeRate;
			ThrottlingPolicyDefaults.EncryptionSenderCutoffBalance = cutoffBalance;
		}

		private static void SetE4eRecipientDefaults(Unlimited<uint> maxConcurrency, Unlimited<uint> maxBurst, Unlimited<uint> rechargeRate, Unlimited<uint> cutoffBalance)
		{
			ThrottlingPolicyDefaults.EncryptionRecipientMaxConcurrency = maxConcurrency;
			ThrottlingPolicyDefaults.EncryptionRecipientMaxBurst = maxBurst;
			ThrottlingPolicyDefaults.EncryptionRecipientRechargeRate = rechargeRate;
			ThrottlingPolicyDefaults.EncryptionRecipientCutoffBalance = cutoffBalance;
		}

		private static void SetComplianceDefaults(Unlimited<uint> complianceMaxExpansionDGRecipients, Unlimited<uint> complianceMaxExpansionNestedDGs)
		{
			ThrottlingPolicyDefaults.ComplianceMaxExpansionDGRecipients = complianceMaxExpansionDGRecipients;
			ThrottlingPolicyDefaults.ComplianceMaxExpansionNestedDGs = complianceMaxExpansionNestedDGs;
		}

		public static Unlimited<uint> EasMaxConcurrency { get; private set; }

		public static Unlimited<uint> EasMaxBurst { get; private set; }

		public static Unlimited<uint> EasRechargeRate { get; private set; }

		public static Unlimited<uint> EasCutoffBalance { get; private set; }

		public static Unlimited<uint> EasMaxDevices { get; private set; }

		public static Unlimited<uint> EasMaxDeviceDeletesPerMonth { get; private set; }

		public static Unlimited<uint> EasMaxInactivityForDeviceCleanup { get; private set; }

		public static Unlimited<uint> EwsMaxConcurrency { get; private set; }

		public static Unlimited<uint> EwsMaxBurst { get; private set; }

		public static Unlimited<uint> EwsRechargeRate { get; private set; }

		public static Unlimited<uint> EwsCutoffBalance { get; private set; }

		public static Unlimited<uint> EwsMaxSubscriptions { get; private set; }

		public static Unlimited<uint> ImapMaxConcurrency { get; private set; }

		public static Unlimited<uint> ImapMaxBurst { get; private set; }

		public static Unlimited<uint> ImapRechargeRate { get; private set; }

		public static Unlimited<uint> ImapCutoffBalance { get; private set; }

		public static Unlimited<uint> OutlookServiceMaxConcurrency { get; private set; }

		public static Unlimited<uint> OutlookServiceMaxBurst { get; private set; }

		public static Unlimited<uint> OutlookServiceRechargeRate { get; private set; }

		public static Unlimited<uint> OutlookServiceCutoffBalance { get; private set; }

		public static Unlimited<uint> OutlookServiceMaxSubscriptions { get; private set; }

		public static Unlimited<uint> OutlookServiceMaxSocketConnectionsPerDevice { get; private set; }

		public static Unlimited<uint> OutlookServiceMaxSocketConnectionsPerUser { get; private set; }

		public static Unlimited<uint> OwaMaxConcurrency { get; private set; }

		public static Unlimited<uint> OwaMaxBurst { get; private set; }

		public static Unlimited<uint> OwaRechargeRate { get; private set; }

		public static Unlimited<uint> OwaCutoffBalance { get; private set; }

		public static Unlimited<uint> OwaVoiceMaxConcurrency { get; private set; }

		public static Unlimited<uint> OwaVoiceMaxBurst { get; private set; }

		public static Unlimited<uint> OwaVoiceRechargeRate { get; private set; }

		public static Unlimited<uint> OwaVoiceCutoffBalance { get; private set; }

		public static Unlimited<uint> PopMaxConcurrency { get; private set; }

		public static Unlimited<uint> PopMaxBurst { get; private set; }

		public static Unlimited<uint> PopRechargeRate { get; private set; }

		public static Unlimited<uint> PopCutoffBalance { get; private set; }

		public static Unlimited<uint> PowerShellMaxConcurrency { get; private set; }

		public static Unlimited<uint> PowerShellMaxTenantConcurrency { get; private set; }

		public static Unlimited<uint> PowerShellMaxBurst { get; private set; }

		public static Unlimited<uint> PowerShellRechargeRate { get; private set; }

		public static Unlimited<uint> PowerShellCutoffBalance { get; private set; }

		public static Unlimited<uint> PowerShellMaxOperations { get; private set; }

		public static Unlimited<uint> PowerShellMaxCmdletsTimePeriod { get; private set; }

		public static Unlimited<uint> ExchangeMaxCmdlets { get; private set; }

		public static Unlimited<uint> PowerShellMaxCmdletQueueDepth { get; private set; }

		public static Unlimited<uint> PowerShellMaxDestructiveCmdlets { get; private set; }

		public static Unlimited<uint> PowerShellMaxDestructiveCmdletsTimePeriod { get; private set; }

		public static Unlimited<uint> PowerShellMaxCmdlets { get; private set; }

		public static Unlimited<uint> PowerShellMaxRunspaces { get; private set; }

		public static Unlimited<uint> PowerShellMaxTenantRunspaces { get; private set; }

		public static Unlimited<uint> PowerShellMaxRunspacesTimePeriod { get; private set; }

		public static Unlimited<uint> PswsMaxConcurrency { get; private set; }

		public static Unlimited<uint> PswsMaxRequest { get; private set; }

		public static Unlimited<uint> PswsMaxRequestTimePeriod { get; private set; }

		public static Unlimited<uint> AnonymousMaxConcurrency { get; private set; }

		public static Unlimited<uint> AnonymousMaxBurst { get; private set; }

		public static Unlimited<uint> AnonymousRechargeRate { get; private set; }

		public static Unlimited<uint> AnonymousCutoffBalance { get; private set; }

		public static Unlimited<uint> RcaMaxConcurrency { get; private set; }

		public static Unlimited<uint> RcaMaxBurst { get; private set; }

		public static Unlimited<uint> RcaRechargeRate { get; private set; }

		public static Unlimited<uint> RcaCutoffBalance { get; private set; }

		public static Unlimited<uint> CpaMaxConcurrency { get; private set; }

		public static Unlimited<uint> CpaMaxBurst { get; private set; }

		public static Unlimited<uint> CpaRechargeRate { get; private set; }

		public static Unlimited<uint> CpaCutoffBalance { get; private set; }

		public static Unlimited<uint> MessageRateLimit { get; private set; }

		public static Unlimited<uint> RecipientRateLimit { get; private set; }

		public static Unlimited<uint> ForwardeeLimit { get; private set; }

		public static Unlimited<uint> DiscoveryMaxConcurrency { get; private set; }

		public static Unlimited<uint> DiscoveryMaxMailboxes { get; private set; }

		public static Unlimited<uint> DiscoveryMaxKeywords { get; private set; }

		public static Unlimited<uint> DiscoveryMaxPreviewSearchMailboxes { get; private set; }

		public static Unlimited<uint> DiscoveryMaxStatsSearchMailboxes { get; private set; }

		public static Unlimited<uint> DiscoveryPreviewSearchResultsPageSize { get; private set; }

		public static Unlimited<uint> DiscoveryMaxKeywordsPerPage { get; private set; }

		public static Unlimited<uint> DiscoveryMaxRefinerResults { get; private set; }

		public static Unlimited<uint> DiscoveryMaxSearchQueueDepth { get; private set; }

		public static Unlimited<uint> DiscoverySearchTimeoutPeriod { get; private set; }

		public static Unlimited<uint> ComplianceMaxExpansionDGRecipients { get; private set; }

		public static Unlimited<uint> ComplianceMaxExpansionNestedDGs { get; private set; }

		public static Unlimited<uint> PushNotificationMaxConcurrency { get; private set; }

		public static Unlimited<uint> PushNotificationMaxBurst { get; private set; }

		public static Unlimited<uint> PushNotificationRechargeRate { get; private set; }

		public static Unlimited<uint> PushNotificationCutoffBalance { get; private set; }

		public static Unlimited<uint> PushNotificationMaxBurstPerDevice { get; private set; }

		public static Unlimited<uint> PushNotificationRechargeRatePerDevice { get; private set; }

		public static Unlimited<uint> PushNotificationSamplingPeriodPerDevice { get; private set; }

		public static Unlimited<uint> ServiceAccountEwsMaxConcurrency
		{
			get
			{
				return 27U;
			}
		}

		public static Unlimited<uint> ServiceAccountImapMaxConcurrency
		{
			get
			{
				return 50U;
			}
		}

		public static Unlimited<uint> ServiceAccountOutlookServiceMaxConcurrency
		{
			get
			{
				return 27U;
			}
		}

		public static Unlimited<uint> ServiceAccountRcaMaxConcurrency
		{
			get
			{
				return 60U;
			}
		}

		public static Unlimited<uint> EncryptionSenderMaxConcurrency { get; private set; }

		public static Unlimited<uint> EncryptionSenderMaxBurst { get; private set; }

		public static Unlimited<uint> EncryptionSenderRechargeRate { get; private set; }

		public static Unlimited<uint> EncryptionSenderCutoffBalance { get; private set; }

		public static Unlimited<uint> EncryptionRecipientMaxConcurrency { get; private set; }

		public static Unlimited<uint> EncryptionRecipientMaxBurst { get; private set; }

		public static Unlimited<uint> EncryptionRecipientRechargeRate { get; private set; }

		public static Unlimited<uint> EncryptionRecipientCutoffBalance { get; private set; }
	}
}
