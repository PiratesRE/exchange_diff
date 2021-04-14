using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class UnthrottledThrottlingPolicy : IThrottlingPolicy
	{
		public static UnthrottledThrottlingPolicy GetSingleton()
		{
			return UnthrottledThrottlingPolicy.singleton;
		}

		public bool IsFallback
		{
			get
			{
				return false;
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
				return true;
			}
		}

		public Unlimited<uint> EasMaxConcurrency
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EasMaxBurst
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EasRechargeRate
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EasCutoffBalance
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EasMaxDevices
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EasMaxDeviceDeletesPerMonth
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EasMaxInactivityForDeviceCleanup
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EwsMaxConcurrency
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EwsMaxBurst
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EwsRechargeRate
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EwsCutoffBalance
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EwsMaxSubscriptions
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> ImapMaxConcurrency
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> ImapMaxBurst
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> ImapRechargeRate
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> ImapCutoffBalance
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> OutlookServiceMaxConcurrency
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> OutlookServiceMaxBurst
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> OutlookServiceRechargeRate
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> OutlookServiceCutoffBalance
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> OutlookServiceMaxSubscriptions
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> OutlookServiceMaxSocketConnectionsPerDevice
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> OutlookServiceMaxSocketConnectionsPerUser
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> OwaMaxConcurrency
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> OwaMaxBurst
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> OwaRechargeRate
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> OwaCutoffBalance
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> OwaVoiceMaxConcurrency
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> OwaVoiceMaxBurst
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> OwaVoiceRechargeRate
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> OwaVoiceCutoffBalance
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PopMaxConcurrency
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PopMaxBurst
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PopRechargeRate
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PopCutoffBalance
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PowerShellMaxConcurrency
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PowerShellMaxBurst
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PowerShellRechargeRate
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PowerShellCutoffBalance
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PowerShellMaxTenantConcurrency
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PowerShellMaxOperations
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PowerShellMaxCmdletsTimePeriod
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> ExchangeMaxCmdlets
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PowerShellMaxCmdletQueueDepth
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PowerShellMaxDestructiveCmdlets
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PowerShellMaxDestructiveCmdletsTimePeriod
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PowerShellMaxCmdlets
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PowerShellMaxRunspaces
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PowerShellMaxTenantRunspaces
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PowerShellMaxRunspacesTimePeriod
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PswsMaxConcurrency
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PswsMaxRequest
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PswsMaxRequestTimePeriod
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> RcaMaxConcurrency
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> RcaMaxBurst
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> RcaRechargeRate
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> RcaCutoffBalance
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> CpaMaxConcurrency
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> CpaMaxBurst
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> CpaRechargeRate
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> CpaCutoffBalance
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> DiscoveryMaxConcurrency
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> DiscoveryMaxMailboxes
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> DiscoveryMaxKeywords
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> DiscoveryMaxPreviewSearchMailboxes
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> DiscoveryMaxStatsSearchMailboxes
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> DiscoveryPreviewSearchResultsPageSize
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> DiscoveryMaxKeywordsPerPage
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> DiscoveryMaxRefinerResults
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> DiscoveryMaxSearchQueueDepth
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> DiscoverySearchTimeoutPeriod
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> MessageRateLimit
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> RecipientRateLimit
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> ForwardeeLimit
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> ComplianceMaxExpansionDGRecipients
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> ComplianceMaxExpansionNestedDGs
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public string GetIdentityString()
		{
			return "[Unthrottled]";
		}

		public string GetShortIdentityString()
		{
			return "[Unthrottled]";
		}

		public Unlimited<uint> AnonymousMaxConcurrency
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> AnonymousMaxBurst
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> AnonymousRechargeRate
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> AnonymousCutoffBalance
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PushNotificationMaxConcurrency
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PushNotificationMaxBurst
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PushNotificationRechargeRate
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PushNotificationCutoffBalance
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PushNotificationMaxBurstPerDevice
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PushNotificationRechargeRatePerDevice
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> PushNotificationSamplingPeriodPerDevice
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EncryptionSenderMaxConcurrency
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EncryptionSenderMaxBurst
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EncryptionSenderRechargeRate
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EncryptionSenderCutoffBalance
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EncryptionRecipientMaxConcurrency
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EncryptionRecipientMaxBurst
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EncryptionRecipientRechargeRate
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		public Unlimited<uint> EncryptionRecipientCutoffBalance
		{
			get
			{
				return Unlimited<uint>.UnlimitedValue;
			}
		}

		private const string UnthrottledIdentity = "[Unthrottled]";

		private static UnthrottledThrottlingPolicy singleton = new UnthrottledThrottlingPolicy();
	}
}
