using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class FallbackThrottlingPolicy : IThrottlingPolicy
	{
		private FallbackThrottlingPolicy()
		{
		}

		public static FallbackThrottlingPolicy GetSingleton()
		{
			return FallbackThrottlingPolicy.singleton;
		}

		public bool IsFallback
		{
			get
			{
				return true;
			}
		}

		public ThrottlingPolicyScopeType ThrottlingPolicyScope
		{
			get
			{
				return ThrottlingPolicyScopeType.Regular;
			}
		}

		public bool IsServiceAccount
		{
			get
			{
				return false;
			}
		}

		public bool IsUnthrottled
		{
			get
			{
				return false;
			}
		}

		public Unlimited<uint> AnonymousMaxConcurrency
		{
			get
			{
				return ThrottlingPolicyDefaults.AnonymousMaxConcurrency;
			}
		}

		public Unlimited<uint> AnonymousMaxBurst
		{
			get
			{
				return ThrottlingPolicyDefaults.AnonymousMaxBurst;
			}
		}

		public Unlimited<uint> AnonymousRechargeRate
		{
			get
			{
				return ThrottlingPolicyDefaults.AnonymousRechargeRate;
			}
		}

		public Unlimited<uint> AnonymousCutoffBalance
		{
			get
			{
				return ThrottlingPolicyDefaults.AnonymousCutoffBalance;
			}
		}

		public Unlimited<uint> EasMaxConcurrency
		{
			get
			{
				return ThrottlingPolicyDefaults.EasMaxConcurrency;
			}
		}

		public Unlimited<uint> EasMaxBurst
		{
			get
			{
				return ThrottlingPolicyDefaults.EasMaxBurst;
			}
		}

		public Unlimited<uint> EasRechargeRate
		{
			get
			{
				return ThrottlingPolicyDefaults.EasRechargeRate;
			}
		}

		public Unlimited<uint> EasCutoffBalance
		{
			get
			{
				return ThrottlingPolicyDefaults.EasCutoffBalance;
			}
		}

		public Unlimited<uint> EasMaxDevices
		{
			get
			{
				return ThrottlingPolicyDefaults.EasMaxDevices;
			}
		}

		public Unlimited<uint> EasMaxDeviceDeletesPerMonth
		{
			get
			{
				return ThrottlingPolicyDefaults.EasMaxDeviceDeletesPerMonth;
			}
		}

		public Unlimited<uint> EasMaxInactivityForDeviceCleanup
		{
			get
			{
				return ThrottlingPolicyDefaults.EasMaxInactivityForDeviceCleanup;
			}
		}

		public Unlimited<uint> EwsMaxConcurrency
		{
			get
			{
				return ThrottlingPolicyDefaults.EwsMaxConcurrency;
			}
		}

		public Unlimited<uint> EwsMaxBurst
		{
			get
			{
				return ThrottlingPolicyDefaults.EwsMaxBurst;
			}
		}

		public Unlimited<uint> EwsRechargeRate
		{
			get
			{
				return ThrottlingPolicyDefaults.EwsRechargeRate;
			}
		}

		public Unlimited<uint> EwsCutoffBalance
		{
			get
			{
				return ThrottlingPolicyDefaults.EwsCutoffBalance;
			}
		}

		public Unlimited<uint> EwsMaxSubscriptions
		{
			get
			{
				return ThrottlingPolicyDefaults.EwsMaxSubscriptions;
			}
		}

		public Unlimited<uint> ImapMaxConcurrency
		{
			get
			{
				return ThrottlingPolicyDefaults.ImapMaxConcurrency;
			}
		}

		public Unlimited<uint> ImapMaxBurst
		{
			get
			{
				return ThrottlingPolicyDefaults.ImapMaxBurst;
			}
		}

		public Unlimited<uint> ImapRechargeRate
		{
			get
			{
				return ThrottlingPolicyDefaults.ImapRechargeRate;
			}
		}

		public Unlimited<uint> ImapCutoffBalance
		{
			get
			{
				return ThrottlingPolicyDefaults.ImapCutoffBalance;
			}
		}

		public Unlimited<uint> OutlookServiceMaxConcurrency
		{
			get
			{
				return ThrottlingPolicyDefaults.OutlookServiceMaxConcurrency;
			}
		}

		public Unlimited<uint> OutlookServiceMaxBurst
		{
			get
			{
				return ThrottlingPolicyDefaults.OutlookServiceMaxBurst;
			}
		}

		public Unlimited<uint> OutlookServiceRechargeRate
		{
			get
			{
				return ThrottlingPolicyDefaults.OutlookServiceRechargeRate;
			}
		}

		public Unlimited<uint> OutlookServiceCutoffBalance
		{
			get
			{
				return ThrottlingPolicyDefaults.OutlookServiceCutoffBalance;
			}
		}

		public Unlimited<uint> OutlookServiceMaxSubscriptions
		{
			get
			{
				return ThrottlingPolicyDefaults.OutlookServiceMaxSubscriptions;
			}
		}

		public Unlimited<uint> OutlookServiceMaxSocketConnectionsPerDevice
		{
			get
			{
				return ThrottlingPolicyDefaults.OutlookServiceMaxSocketConnectionsPerDevice;
			}
		}

		public Unlimited<uint> OutlookServiceMaxSocketConnectionsPerUser
		{
			get
			{
				return ThrottlingPolicyDefaults.OutlookServiceMaxSocketConnectionsPerUser;
			}
		}

		public Unlimited<uint> OwaMaxConcurrency
		{
			get
			{
				return ThrottlingPolicyDefaults.OwaMaxConcurrency;
			}
		}

		public Unlimited<uint> OwaMaxBurst
		{
			get
			{
				return ThrottlingPolicyDefaults.OwaMaxBurst;
			}
		}

		public Unlimited<uint> OwaRechargeRate
		{
			get
			{
				return ThrottlingPolicyDefaults.OwaRechargeRate;
			}
		}

		public Unlimited<uint> OwaCutoffBalance
		{
			get
			{
				return ThrottlingPolicyDefaults.OwaCutoffBalance;
			}
		}

		public Unlimited<uint> OwaVoiceMaxConcurrency
		{
			get
			{
				return ThrottlingPolicyDefaults.OwaVoiceMaxConcurrency;
			}
		}

		public Unlimited<uint> OwaVoiceMaxBurst
		{
			get
			{
				return ThrottlingPolicyDefaults.OwaVoiceMaxBurst;
			}
		}

		public Unlimited<uint> OwaVoiceRechargeRate
		{
			get
			{
				return ThrottlingPolicyDefaults.OwaVoiceRechargeRate;
			}
		}

		public Unlimited<uint> OwaVoiceCutoffBalance
		{
			get
			{
				return ThrottlingPolicyDefaults.OwaVoiceCutoffBalance;
			}
		}

		public Unlimited<uint> PopMaxConcurrency
		{
			get
			{
				return ThrottlingPolicyDefaults.PopMaxConcurrency;
			}
		}

		public Unlimited<uint> PopMaxBurst
		{
			get
			{
				return ThrottlingPolicyDefaults.PopMaxBurst;
			}
		}

		public Unlimited<uint> PopRechargeRate
		{
			get
			{
				return ThrottlingPolicyDefaults.PopRechargeRate;
			}
		}

		public Unlimited<uint> PopCutoffBalance
		{
			get
			{
				return ThrottlingPolicyDefaults.PopCutoffBalance;
			}
		}

		public Unlimited<uint> PowerShellMaxConcurrency
		{
			get
			{
				return ThrottlingPolicyDefaults.PowerShellMaxConcurrency;
			}
		}

		public Unlimited<uint> PowerShellMaxBurst
		{
			get
			{
				return ThrottlingPolicyDefaults.PowerShellMaxBurst;
			}
		}

		public Unlimited<uint> PowerShellRechargeRate
		{
			get
			{
				return ThrottlingPolicyDefaults.PowerShellRechargeRate;
			}
		}

		public Unlimited<uint> PowerShellCutoffBalance
		{
			get
			{
				return ThrottlingPolicyDefaults.PowerShellCutoffBalance;
			}
		}

		public Unlimited<uint> PowerShellMaxTenantConcurrency
		{
			get
			{
				return ThrottlingPolicyDefaults.PowerShellMaxTenantConcurrency;
			}
		}

		public Unlimited<uint> PowerShellMaxOperations
		{
			get
			{
				return ThrottlingPolicyDefaults.PowerShellMaxOperations;
			}
		}

		public Unlimited<uint> PowerShellMaxCmdletsTimePeriod
		{
			get
			{
				return ThrottlingPolicyDefaults.PowerShellMaxCmdletsTimePeriod;
			}
		}

		public Unlimited<uint> ExchangeMaxCmdlets
		{
			get
			{
				return ThrottlingPolicyDefaults.ExchangeMaxCmdlets;
			}
		}

		public Unlimited<uint> PowerShellMaxCmdletQueueDepth
		{
			get
			{
				return ThrottlingPolicyDefaults.PowerShellMaxCmdletQueueDepth;
			}
		}

		public Unlimited<uint> PowerShellMaxDestructiveCmdlets
		{
			get
			{
				return ThrottlingPolicyDefaults.PowerShellMaxDestructiveCmdlets;
			}
		}

		public Unlimited<uint> PowerShellMaxDestructiveCmdletsTimePeriod
		{
			get
			{
				return ThrottlingPolicyDefaults.PowerShellMaxDestructiveCmdletsTimePeriod;
			}
		}

		public Unlimited<uint> PowerShellMaxCmdlets
		{
			get
			{
				return ThrottlingPolicyDefaults.PowerShellMaxCmdlets;
			}
		}

		public Unlimited<uint> PowerShellMaxRunspaces
		{
			get
			{
				return ThrottlingPolicyDefaults.PowerShellMaxRunspaces;
			}
		}

		public Unlimited<uint> PowerShellMaxTenantRunspaces
		{
			get
			{
				return ThrottlingPolicyDefaults.PowerShellMaxTenantRunspaces;
			}
		}

		public Unlimited<uint> PowerShellMaxRunspacesTimePeriod
		{
			get
			{
				return ThrottlingPolicyDefaults.PowerShellMaxRunspacesTimePeriod;
			}
		}

		public Unlimited<uint> PswsMaxConcurrency
		{
			get
			{
				return ThrottlingPolicyDefaults.PswsMaxConcurrency;
			}
		}

		public Unlimited<uint> PswsMaxRequest
		{
			get
			{
				return ThrottlingPolicyDefaults.PswsMaxRequest;
			}
		}

		public Unlimited<uint> PswsMaxRequestTimePeriod
		{
			get
			{
				return ThrottlingPolicyDefaults.PswsMaxRequestTimePeriod;
			}
		}

		public Unlimited<uint> RcaMaxConcurrency
		{
			get
			{
				return ThrottlingPolicyDefaults.RcaMaxConcurrency;
			}
		}

		public Unlimited<uint> RcaMaxBurst
		{
			get
			{
				return ThrottlingPolicyDefaults.RcaMaxBurst;
			}
		}

		public Unlimited<uint> RcaRechargeRate
		{
			get
			{
				return ThrottlingPolicyDefaults.RcaRechargeRate;
			}
		}

		public Unlimited<uint> RcaCutoffBalance
		{
			get
			{
				return ThrottlingPolicyDefaults.RcaCutoffBalance;
			}
		}

		public Unlimited<uint> CpaMaxConcurrency
		{
			get
			{
				return ThrottlingPolicyDefaults.CpaMaxConcurrency;
			}
		}

		public Unlimited<uint> CpaMaxBurst
		{
			get
			{
				return ThrottlingPolicyDefaults.CpaMaxBurst;
			}
		}

		public Unlimited<uint> CpaRechargeRate
		{
			get
			{
				return ThrottlingPolicyDefaults.CpaRechargeRate;
			}
		}

		public Unlimited<uint> CpaCutoffBalance
		{
			get
			{
				return ThrottlingPolicyDefaults.CpaCutoffBalance;
			}
		}

		public Unlimited<uint> MessageRateLimit
		{
			get
			{
				return ThrottlingPolicyDefaults.MessageRateLimit;
			}
		}

		public Unlimited<uint> RecipientRateLimit
		{
			get
			{
				return ThrottlingPolicyDefaults.RecipientRateLimit;
			}
		}

		public Unlimited<uint> ForwardeeLimit
		{
			get
			{
				return ThrottlingPolicyDefaults.ForwardeeLimit;
			}
		}

		public Unlimited<uint> DiscoveryMaxConcurrency
		{
			get
			{
				return ThrottlingPolicyDefaults.DiscoveryMaxConcurrency;
			}
		}

		public Unlimited<uint> DiscoveryMaxMailboxes
		{
			get
			{
				return ThrottlingPolicyDefaults.DiscoveryMaxMailboxes;
			}
		}

		public Unlimited<uint> DiscoveryMaxKeywords
		{
			get
			{
				return ThrottlingPolicyDefaults.DiscoveryMaxKeywords;
			}
		}

		public Unlimited<uint> DiscoveryMaxStatsSearchMailboxes
		{
			get
			{
				return ThrottlingPolicyDefaults.DiscoveryMaxStatsSearchMailboxes;
			}
		}

		public Unlimited<uint> DiscoveryMaxPreviewSearchMailboxes
		{
			get
			{
				return ThrottlingPolicyDefaults.DiscoveryMaxPreviewSearchMailboxes;
			}
		}

		public Unlimited<uint> DiscoveryPreviewSearchResultsPageSize
		{
			get
			{
				return ThrottlingPolicyDefaults.DiscoveryPreviewSearchResultsPageSize;
			}
		}

		public Unlimited<uint> DiscoveryMaxKeywordsPerPage
		{
			get
			{
				return ThrottlingPolicyDefaults.DiscoveryMaxKeywordsPerPage;
			}
		}

		public Unlimited<uint> DiscoveryMaxRefinerResults
		{
			get
			{
				return ThrottlingPolicyDefaults.DiscoveryMaxRefinerResults;
			}
		}

		public Unlimited<uint> DiscoveryMaxSearchQueueDepth
		{
			get
			{
				return ThrottlingPolicyDefaults.DiscoveryMaxSearchQueueDepth;
			}
		}

		public Unlimited<uint> DiscoverySearchTimeoutPeriod
		{
			get
			{
				return ThrottlingPolicyDefaults.DiscoverySearchTimeoutPeriod;
			}
		}

		public Unlimited<uint> PushNotificationMaxConcurrency
		{
			get
			{
				return ThrottlingPolicyDefaults.PushNotificationMaxConcurrency;
			}
		}

		public Unlimited<uint> PushNotificationMaxBurst
		{
			get
			{
				return ThrottlingPolicyDefaults.PushNotificationMaxBurst;
			}
		}

		public Unlimited<uint> PushNotificationRechargeRate
		{
			get
			{
				return ThrottlingPolicyDefaults.PushNotificationRechargeRate;
			}
		}

		public Unlimited<uint> PushNotificationCutoffBalance
		{
			get
			{
				return ThrottlingPolicyDefaults.PushNotificationCutoffBalance;
			}
		}

		public Unlimited<uint> PushNotificationMaxBurstPerDevice
		{
			get
			{
				return ThrottlingPolicyDefaults.PushNotificationMaxBurstPerDevice;
			}
		}

		public Unlimited<uint> PushNotificationRechargeRatePerDevice
		{
			get
			{
				return ThrottlingPolicyDefaults.PushNotificationRechargeRatePerDevice;
			}
		}

		public Unlimited<uint> PushNotificationSamplingPeriodPerDevice
		{
			get
			{
				return ThrottlingPolicyDefaults.PushNotificationSamplingPeriodPerDevice;
			}
		}

		public Unlimited<uint> EncryptionSenderMaxConcurrency
		{
			get
			{
				return ThrottlingPolicyDefaults.EncryptionSenderMaxConcurrency;
			}
		}

		public Unlimited<uint> EncryptionSenderMaxBurst
		{
			get
			{
				return ThrottlingPolicyDefaults.EncryptionSenderMaxBurst;
			}
		}

		public Unlimited<uint> EncryptionSenderRechargeRate
		{
			get
			{
				return ThrottlingPolicyDefaults.EncryptionSenderRechargeRate;
			}
		}

		public Unlimited<uint> EncryptionSenderCutoffBalance
		{
			get
			{
				return ThrottlingPolicyDefaults.EncryptionSenderCutoffBalance;
			}
		}

		public Unlimited<uint> EncryptionRecipientMaxConcurrency
		{
			get
			{
				return ThrottlingPolicyDefaults.EncryptionRecipientMaxConcurrency;
			}
		}

		public Unlimited<uint> EncryptionRecipientMaxBurst
		{
			get
			{
				return ThrottlingPolicyDefaults.EncryptionRecipientMaxBurst;
			}
		}

		public Unlimited<uint> EncryptionRecipientRechargeRate
		{
			get
			{
				return ThrottlingPolicyDefaults.EncryptionRecipientRechargeRate;
			}
		}

		public Unlimited<uint> EncryptionRecipientCutoffBalance
		{
			get
			{
				return ThrottlingPolicyDefaults.EncryptionRecipientCutoffBalance;
			}
		}

		public Unlimited<uint> ComplianceMaxExpansionDGRecipients
		{
			get
			{
				return ThrottlingPolicyDefaults.ComplianceMaxExpansionDGRecipients;
			}
		}

		public Unlimited<uint> ComplianceMaxExpansionNestedDGs
		{
			get
			{
				return ThrottlingPolicyDefaults.ComplianceMaxExpansionNestedDGs;
			}
		}

		public string GetIdentityString()
		{
			return "[Fallback]";
		}

		public string GetShortIdentityString()
		{
			return "[Fallback]";
		}

		private const string FallbackIdentity = "[Fallback]";

		private static FallbackThrottlingPolicy singleton = new FallbackThrottlingPolicy();
	}
}
