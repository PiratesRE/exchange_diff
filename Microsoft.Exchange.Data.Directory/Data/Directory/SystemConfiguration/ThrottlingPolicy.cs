using System;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class ThrottlingPolicy : ADConfigurationObject
	{
		internal static ThrottlingPolicy GetDefaultOrganizationEffectiveThrottlingPolicy()
		{
			ThrottlingPolicy throttlingPolicy = EffectiveThrottlingPolicy.ReadGlobalThrottlingPolicyFromAD();
			throttlingPolicy.ConvertToEffectiveThrottlingPolicy(false);
			return throttlingPolicy;
		}

		private LegacyThrottlingPolicy LegacyThrottlingPolicy
		{
			get
			{
				if (this.legacyThrottlingPolicy == null)
				{
					this.legacyThrottlingPolicy = LegacyThrottlingPolicy.GetLegacyThrottlingPolicy(this);
				}
				return this.legacyThrottlingPolicy;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal override bool IsShareable
		{
			get
			{
				return true;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ThrottlingPolicy.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ThrottlingPolicy.mostDerivedClass;
			}
		}

		public ThrottlingPolicyScopeType ThrottlingPolicyScope
		{
			get
			{
				return (ThrottlingPolicyScopeType)this[ThrottlingPolicySchema.ThrottlingPolicyScope];
			}
			internal set
			{
				this[ThrottlingPolicySchema.ThrottlingPolicyScope] = value;
			}
		}

		public bool IsServiceAccount
		{
			get
			{
				return (bool)this[ThrottlingPolicySchema.IsServiceAccount];
			}
			internal set
			{
				this[ThrottlingPolicySchema.IsServiceAccount] = value;
			}
		}

		public Unlimited<uint>? AnonymousMaxConcurrency
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.AnonymousMaxConcurrency];
			}
			set
			{
				this[ThrottlingPolicySchema.AnonymousMaxConcurrency] = value;
			}
		}

		public Unlimited<uint>? AnonymousMaxBurst
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.AnonymousMaxBurst];
			}
			set
			{
				this[ThrottlingPolicySchema.AnonymousMaxBurst] = value;
			}
		}

		public Unlimited<uint>? AnonymousRechargeRate
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.AnonymousRechargeRate];
			}
			set
			{
				this[ThrottlingPolicySchema.AnonymousRechargeRate] = value;
			}
		}

		public Unlimited<uint>? AnonymousCutoffBalance
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.AnonymousCutoffBalance];
			}
			set
			{
				this[ThrottlingPolicySchema.AnonymousCutoffBalance] = value;
			}
		}

		public Unlimited<uint>? EasMaxConcurrency
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EasMaxConcurrency];
			}
			set
			{
				this[ThrottlingPolicySchema.EasMaxConcurrency] = value;
			}
		}

		public Unlimited<uint>? EasMaxBurst
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EasMaxBurst];
			}
			set
			{
				this[ThrottlingPolicySchema.EasMaxBurst] = value;
			}
		}

		public Unlimited<uint>? EasRechargeRate
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EasRechargeRate];
			}
			set
			{
				this[ThrottlingPolicySchema.EasRechargeRate] = value;
			}
		}

		public Unlimited<uint>? EasCutoffBalance
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EasCutoffBalance];
			}
			set
			{
				this[ThrottlingPolicySchema.EasCutoffBalance] = value;
			}
		}

		public Unlimited<uint>? EasMaxDevices
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EasMaxDevices];
			}
			set
			{
				this[ThrottlingPolicySchema.EasMaxDevices] = value;
			}
		}

		public Unlimited<uint>? EasMaxDeviceDeletesPerMonth
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EasMaxDeviceDeletesPerMonth];
			}
			set
			{
				this[ThrottlingPolicySchema.EasMaxDeviceDeletesPerMonth] = value;
			}
		}

		public Unlimited<uint>? EasMaxInactivityForDeviceCleanup
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EasMaxInactivityForDeviceCleanup];
			}
			set
			{
				this[ThrottlingPolicySchema.EasMaxInactivityForDeviceCleanup] = value;
			}
		}

		public Unlimited<uint>? EwsMaxConcurrency
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EwsMaxConcurrency];
			}
			set
			{
				this[ThrottlingPolicySchema.EwsMaxConcurrency] = value;
			}
		}

		public Unlimited<uint>? EwsMaxBurst
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EwsMaxBurst];
			}
			set
			{
				this[ThrottlingPolicySchema.EwsMaxBurst] = value;
			}
		}

		public Unlimited<uint>? EwsRechargeRate
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EwsRechargeRate];
			}
			set
			{
				this[ThrottlingPolicySchema.EwsRechargeRate] = value;
			}
		}

		public Unlimited<uint>? EwsCutoffBalance
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EwsCutoffBalance];
			}
			set
			{
				this[ThrottlingPolicySchema.EwsCutoffBalance] = value;
			}
		}

		public Unlimited<uint>? EwsMaxSubscriptions
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EwsMaxSubscriptions];
			}
			set
			{
				this[ThrottlingPolicySchema.EwsMaxSubscriptions] = value;
			}
		}

		public Unlimited<uint>? ImapMaxConcurrency
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.ImapMaxConcurrency];
			}
			set
			{
				this[ThrottlingPolicySchema.ImapMaxConcurrency] = value;
			}
		}

		public Unlimited<uint>? ImapMaxBurst
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.ImapMaxBurst];
			}
			set
			{
				this[ThrottlingPolicySchema.ImapMaxBurst] = value;
			}
		}

		public Unlimited<uint>? ImapRechargeRate
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.ImapRechargeRate];
			}
			set
			{
				this[ThrottlingPolicySchema.ImapRechargeRate] = value;
			}
		}

		public Unlimited<uint>? ImapCutoffBalance
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.ImapCutoffBalance];
			}
			set
			{
				this[ThrottlingPolicySchema.ImapCutoffBalance] = value;
			}
		}

		public Unlimited<uint>? OutlookServiceMaxConcurrency
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.OutlookServiceMaxConcurrency];
			}
			set
			{
				this[ThrottlingPolicySchema.OutlookServiceMaxConcurrency] = value;
			}
		}

		public Unlimited<uint>? OutlookServiceMaxBurst
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.OutlookServiceMaxBurst];
			}
			set
			{
				this[ThrottlingPolicySchema.OutlookServiceMaxBurst] = value;
			}
		}

		public Unlimited<uint>? OutlookServiceRechargeRate
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.OutlookServiceRechargeRate];
			}
			set
			{
				this[ThrottlingPolicySchema.OutlookServiceRechargeRate] = value;
			}
		}

		public Unlimited<uint>? OutlookServiceCutoffBalance
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.OutlookServiceCutoffBalance];
			}
			set
			{
				this[ThrottlingPolicySchema.OutlookServiceCutoffBalance] = value;
			}
		}

		public Unlimited<uint>? OutlookServiceMaxSubscriptions
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.OutlookServiceMaxSubscriptions];
			}
			set
			{
				this[ThrottlingPolicySchema.OutlookServiceMaxSubscriptions] = value;
			}
		}

		public Unlimited<uint>? OutlookServiceMaxSocketConnectionsPerDevice
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.OutlookServiceMaxSocketConnectionsPerDevice];
			}
			set
			{
				this[ThrottlingPolicySchema.OutlookServiceMaxSocketConnectionsPerDevice] = value;
			}
		}

		public Unlimited<uint>? OutlookServiceMaxSocketConnectionsPerUser
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.OutlookServiceMaxSocketConnectionsPerUser];
			}
			set
			{
				this[ThrottlingPolicySchema.OutlookServiceMaxSocketConnectionsPerUser] = value;
			}
		}

		public Unlimited<uint>? OwaMaxConcurrency
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.OwaMaxConcurrency];
			}
			set
			{
				this[ThrottlingPolicySchema.OwaMaxConcurrency] = value;
			}
		}

		public Unlimited<uint>? OwaMaxBurst
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.OwaMaxBurst];
			}
			set
			{
				this[ThrottlingPolicySchema.OwaMaxBurst] = value;
			}
		}

		public Unlimited<uint>? OwaRechargeRate
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.OwaRechargeRate];
			}
			set
			{
				this[ThrottlingPolicySchema.OwaRechargeRate] = value;
			}
		}

		public Unlimited<uint>? OwaCutoffBalance
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.OwaCutoffBalance];
			}
			set
			{
				this[ThrottlingPolicySchema.OwaCutoffBalance] = value;
			}
		}

		public Unlimited<uint>? OwaVoiceMaxConcurrency
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.OwaVoiceMaxConcurrency];
			}
			set
			{
				this[ThrottlingPolicySchema.OwaVoiceMaxConcurrency] = value;
			}
		}

		public Unlimited<uint>? OwaVoiceMaxBurst
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.OwaVoiceMaxBurst];
			}
			set
			{
				this[ThrottlingPolicySchema.OwaVoiceMaxBurst] = value;
			}
		}

		public Unlimited<uint>? OwaVoiceRechargeRate
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.OwaVoiceRechargeRate];
			}
			set
			{
				this[ThrottlingPolicySchema.OwaVoiceRechargeRate] = value;
			}
		}

		public Unlimited<uint>? OwaVoiceCutoffBalance
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.OwaVoiceCutoffBalance];
			}
			set
			{
				this[ThrottlingPolicySchema.OwaVoiceCutoffBalance] = value;
			}
		}

		public Unlimited<uint>? PopMaxConcurrency
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PopMaxConcurrency];
			}
			set
			{
				this[ThrottlingPolicySchema.PopMaxConcurrency] = value;
			}
		}

		public Unlimited<uint>? PopMaxBurst
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PopMaxBurst];
			}
			set
			{
				this[ThrottlingPolicySchema.PopMaxBurst] = value;
			}
		}

		public Unlimited<uint>? PopRechargeRate
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PopRechargeRate];
			}
			set
			{
				this[ThrottlingPolicySchema.PopRechargeRate] = value;
			}
		}

		public Unlimited<uint>? PopCutoffBalance
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PopCutoffBalance];
			}
			set
			{
				this[ThrottlingPolicySchema.PopCutoffBalance] = value;
			}
		}

		public Unlimited<uint>? PowerShellMaxConcurrency
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PowerShellMaxConcurrency];
			}
			set
			{
				this[ThrottlingPolicySchema.PowerShellMaxConcurrency] = value;
			}
		}

		public Unlimited<uint>? PowerShellMaxBurst
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PowerShellMaxBurst];
			}
			set
			{
				this[ThrottlingPolicySchema.PowerShellMaxBurst] = value;
			}
		}

		public Unlimited<uint>? PowerShellRechargeRate
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PowerShellRechargeRate];
			}
			set
			{
				this[ThrottlingPolicySchema.PowerShellRechargeRate] = value;
			}
		}

		public Unlimited<uint>? PowerShellCutoffBalance
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PowerShellCutoffBalance];
			}
			set
			{
				this[ThrottlingPolicySchema.PowerShellCutoffBalance] = value;
			}
		}

		public Unlimited<uint>? PowerShellMaxTenantConcurrency
		{
			get
			{
				if (this.ThrottlingPolicyScope == ThrottlingPolicyScopeType.Regular)
				{
					return null;
				}
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PowerShellMaxTenantConcurrency];
			}
			set
			{
				this[ThrottlingPolicySchema.PowerShellMaxTenantConcurrency] = value;
			}
		}

		public Unlimited<uint>? PowerShellMaxOperations
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PowerShellMaxOperations];
			}
			set
			{
				this[ThrottlingPolicySchema.PowerShellMaxOperations] = value;
			}
		}

		public Unlimited<uint>? PowerShellMaxCmdletsTimePeriod
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PowerShellMaxCmdletsTimePeriod];
			}
			set
			{
				this[ThrottlingPolicySchema.PowerShellMaxCmdletsTimePeriod] = value;
			}
		}

		public Unlimited<uint>? ExchangeMaxCmdlets
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.ExchangeMaxCmdlets];
			}
			set
			{
				this[ThrottlingPolicySchema.ExchangeMaxCmdlets] = value;
			}
		}

		public Unlimited<uint>? PowerShellMaxCmdletQueueDepth
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PowerShellMaxCmdletQueueDepth];
			}
			set
			{
				this[ThrottlingPolicySchema.PowerShellMaxCmdletQueueDepth] = value;
			}
		}

		public Unlimited<uint>? PowerShellMaxDestructiveCmdlets
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PowerShellMaxDestructiveCmdlets];
			}
			set
			{
				this[ThrottlingPolicySchema.PowerShellMaxDestructiveCmdlets] = value;
			}
		}

		public Unlimited<uint>? PowerShellMaxDestructiveCmdletsTimePeriod
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PowerShellMaxDestructiveCmdletsTimePeriod];
			}
			set
			{
				this[ThrottlingPolicySchema.PowerShellMaxDestructiveCmdletsTimePeriod] = value;
			}
		}

		public Unlimited<uint>? PowerShellMaxCmdlets
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PowerShellMaxCmdlets];
			}
			set
			{
				this[ThrottlingPolicySchema.PowerShellMaxCmdlets] = value;
			}
		}

		public Unlimited<uint>? PowerShellMaxRunspaces
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PowerShellMaxRunspaces];
			}
			set
			{
				this[ThrottlingPolicySchema.PowerShellMaxRunspaces] = value;
			}
		}

		public Unlimited<uint>? PowerShellMaxTenantRunspaces
		{
			get
			{
				if (this.ThrottlingPolicyScope == ThrottlingPolicyScopeType.Regular)
				{
					return null;
				}
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PowerShellMaxTenantRunspaces];
			}
			set
			{
				this[ThrottlingPolicySchema.PowerShellMaxTenantRunspaces] = value;
			}
		}

		public Unlimited<uint>? PowerShellMaxRunspacesTimePeriod
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PowerShellMaxRunspacesTimePeriod];
			}
			set
			{
				this[ThrottlingPolicySchema.PowerShellMaxRunspacesTimePeriod] = value;
			}
		}

		public Unlimited<uint>? PswsMaxConcurrency
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PswsMaxConcurrency];
			}
			set
			{
				this[ThrottlingPolicySchema.PswsMaxConcurrency] = value;
			}
		}

		public Unlimited<uint>? PswsMaxRequest
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PswsMaxRequest];
			}
			set
			{
				this[ThrottlingPolicySchema.PswsMaxRequest] = value;
			}
		}

		public Unlimited<uint>? PswsMaxRequestTimePeriod
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PswsMaxRequestTimePeriod];
			}
			set
			{
				this[ThrottlingPolicySchema.PswsMaxRequestTimePeriod] = value;
			}
		}

		public Unlimited<uint>? RcaMaxConcurrency
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.RcaMaxConcurrency];
			}
			set
			{
				this[ThrottlingPolicySchema.RcaMaxConcurrency] = value;
			}
		}

		public Unlimited<uint>? RcaMaxBurst
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.RcaMaxBurst];
			}
			set
			{
				this[ThrottlingPolicySchema.RcaMaxBurst] = value;
			}
		}

		public Unlimited<uint>? RcaRechargeRate
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.RcaRechargeRate];
			}
			set
			{
				this[ThrottlingPolicySchema.RcaRechargeRate] = value;
			}
		}

		public Unlimited<uint>? RcaCutoffBalance
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.RcaCutoffBalance];
			}
			set
			{
				this[ThrottlingPolicySchema.RcaCutoffBalance] = value;
			}
		}

		public Unlimited<uint>? CpaMaxConcurrency
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.CpaMaxConcurrency];
			}
			set
			{
				this[ThrottlingPolicySchema.CpaMaxConcurrency] = value;
			}
		}

		public Unlimited<uint>? CpaMaxBurst
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.CpaMaxBurst];
			}
			set
			{
				this[ThrottlingPolicySchema.CpaMaxBurst] = value;
			}
		}

		public Unlimited<uint>? CpaRechargeRate
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.CpaRechargeRate];
			}
			set
			{
				this[ThrottlingPolicySchema.CpaRechargeRate] = value;
			}
		}

		public Unlimited<uint>? CpaCutoffBalance
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.CpaCutoffBalance];
			}
			set
			{
				this[ThrottlingPolicySchema.CpaCutoffBalance] = value;
			}
		}

		public Unlimited<uint>? MessageRateLimit
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.MessageRateLimit];
			}
			set
			{
				this[ThrottlingPolicySchema.MessageRateLimit] = value;
			}
		}

		public Unlimited<uint>? RecipientRateLimit
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.RecipientRateLimit];
			}
			set
			{
				this[ThrottlingPolicySchema.RecipientRateLimit] = value;
			}
		}

		public Unlimited<uint>? ForwardeeLimit
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.ForwardeeLimit];
			}
			set
			{
				this[ThrottlingPolicySchema.ForwardeeLimit] = value;
			}
		}

		public Unlimited<uint>? DiscoveryMaxConcurrency
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.DiscoveryMaxConcurrency];
			}
			set
			{
				this[ThrottlingPolicySchema.DiscoveryMaxConcurrency] = value;
			}
		}

		public Unlimited<uint>? DiscoveryMaxMailboxes
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.DiscoveryMaxMailboxes];
			}
			set
			{
				this[ThrottlingPolicySchema.DiscoveryMaxMailboxes] = value;
			}
		}

		public Unlimited<uint>? DiscoveryMaxKeywords
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.DiscoveryMaxKeywords];
			}
			set
			{
				this[ThrottlingPolicySchema.DiscoveryMaxKeywords] = value;
			}
		}

		public Unlimited<uint>? DiscoveryMaxPreviewSearchMailboxes
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.DiscoveryMaxPreviewSearchMailboxes];
			}
			set
			{
				this[ThrottlingPolicySchema.DiscoveryMaxPreviewSearchMailboxes] = value;
			}
		}

		public Unlimited<uint>? DiscoveryMaxStatsSearchMailboxes
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.DiscoveryMaxStatsSearchMailboxes];
			}
			set
			{
				this[ThrottlingPolicySchema.DiscoveryMaxStatsSearchMailboxes] = value;
			}
		}

		public Unlimited<uint>? DiscoveryPreviewSearchResultsPageSize
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.DiscoveryPreviewSearchResultsPageSize];
			}
			set
			{
				this[ThrottlingPolicySchema.DiscoveryPreviewSearchResultsPageSize] = value;
			}
		}

		public Unlimited<uint>? DiscoveryMaxKeywordsPerPage
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.DiscoveryMaxKeywordsPerPage];
			}
			set
			{
				this[ThrottlingPolicySchema.DiscoveryMaxKeywordsPerPage] = value;
			}
		}

		public Unlimited<uint>? DiscoveryMaxRefinerResults
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.DiscoveryMaxRefinerResults];
			}
			set
			{
				this[ThrottlingPolicySchema.DiscoveryMaxRefinerResults] = value;
			}
		}

		public Unlimited<uint>? DiscoveryMaxSearchQueueDepth
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.DiscoveryMaxSearchQueueDepth];
			}
			set
			{
				this[ThrottlingPolicySchema.DiscoveryMaxSearchQueueDepth] = value;
			}
		}

		public Unlimited<uint>? DiscoverySearchTimeoutPeriod
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.DiscoverySearchTimeoutPeriod];
			}
			set
			{
				this[ThrottlingPolicySchema.DiscoverySearchTimeoutPeriod] = value;
			}
		}

		public Unlimited<uint>? PushNotificationMaxConcurrency
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PushNotificationMaxConcurrency];
			}
			set
			{
				this[ThrottlingPolicySchema.PushNotificationMaxConcurrency] = value;
			}
		}

		public Unlimited<uint>? PushNotificationMaxBurst
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PushNotificationMaxBurst];
			}
			set
			{
				this[ThrottlingPolicySchema.PushNotificationMaxBurst] = value;
			}
		}

		public Unlimited<uint>? PushNotificationRechargeRate
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PushNotificationRechargeRate];
			}
			set
			{
				this[ThrottlingPolicySchema.PushNotificationRechargeRate] = value;
			}
		}

		public Unlimited<uint>? PushNotificationCutoffBalance
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PushNotificationCutoffBalance];
			}
			set
			{
				this[ThrottlingPolicySchema.PushNotificationCutoffBalance] = value;
			}
		}

		public Unlimited<uint>? PushNotificationMaxBurstPerDevice
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PushNotificationMaxBurstPerDevice];
			}
			set
			{
				this[ThrottlingPolicySchema.PushNotificationMaxBurstPerDevice] = value;
			}
		}

		public Unlimited<uint>? PushNotificationRechargeRatePerDevice
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PushNotificationRechargeRatePerDevice];
			}
			set
			{
				this[ThrottlingPolicySchema.PushNotificationRechargeRatePerDevice] = value;
			}
		}

		public Unlimited<uint>? PushNotificationSamplingPeriodPerDevice
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.PushNotificationSamplingPeriodPerDevice];
			}
			set
			{
				this[ThrottlingPolicySchema.PushNotificationSamplingPeriodPerDevice] = value;
			}
		}

		public Unlimited<uint>? EncryptionSenderMaxConcurrency
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EncryptionSenderMaxConcurrency];
			}
			set
			{
				this[ThrottlingPolicySchema.EncryptionSenderMaxConcurrency] = value;
			}
		}

		public Unlimited<uint>? EncryptionSenderMaxBurst
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EncryptionSenderMaxBurst];
			}
			set
			{
				this[ThrottlingPolicySchema.EncryptionSenderMaxBurst] = value;
			}
		}

		public Unlimited<uint>? EncryptionSenderRechargeRate
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EncryptionSenderRechargeRate];
			}
			set
			{
				this[ThrottlingPolicySchema.EncryptionSenderRechargeRate] = value;
			}
		}

		public Unlimited<uint>? EncryptionSenderCutoffBalance
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EncryptionSenderCutoffBalance];
			}
			set
			{
				this[ThrottlingPolicySchema.EncryptionSenderCutoffBalance] = value;
			}
		}

		public Unlimited<uint>? EncryptionRecipientMaxConcurrency
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EncryptionRecipientMaxConcurrency];
			}
			set
			{
				this[ThrottlingPolicySchema.EncryptionRecipientMaxConcurrency] = value;
			}
		}

		public Unlimited<uint>? EncryptionRecipientMaxBurst
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EncryptionRecipientMaxBurst];
			}
			set
			{
				this[ThrottlingPolicySchema.EncryptionRecipientMaxBurst] = value;
			}
		}

		public Unlimited<uint>? EncryptionRecipientRechargeRate
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EncryptionRecipientRechargeRate];
			}
			set
			{
				this[ThrottlingPolicySchema.EncryptionRecipientRechargeRate] = value;
			}
		}

		public Unlimited<uint>? EncryptionRecipientCutoffBalance
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.EncryptionRecipientCutoffBalance];
			}
			set
			{
				this[ThrottlingPolicySchema.EncryptionRecipientCutoffBalance] = value;
			}
		}

		public Unlimited<uint>? ComplianceMaxExpansionDGRecipients
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.ComplianceMaxExpansionDGRecipients];
			}
			set
			{
				this[ThrottlingPolicySchema.ComplianceMaxExpansionDGRecipients] = value;
			}
		}

		public Unlimited<uint>? ComplianceMaxExpansionNestedDGs
		{
			get
			{
				return (Unlimited<uint>?)this[ThrottlingPolicySchema.ComplianceMaxExpansionNestedDGs];
			}
			set
			{
				this[ThrottlingPolicySchema.ComplianceMaxExpansionNestedDGs] = value;
			}
		}

		public bool IsLegacyDefault
		{
			get
			{
				return this.LegacyThrottlingPolicy.IsDefault && !string.IsNullOrEmpty(this.LegacyThrottlingPolicy.ToString());
			}
		}

		public string Diagnostics
		{
			get
			{
				if (this.diagnostics == null)
				{
					this.diagnostics = this.LegacyThrottlingPolicy.ToString();
					if (this.diagnostics == null)
					{
						this.diagnostics = string.Empty;
					}
				}
				return this.diagnostics;
			}
			set
			{
				this.diagnostics = (value ?? string.Empty);
			}
		}

		internal void CloneThrottlingSettingsFrom(IThrottlingPolicy policy)
		{
			this.AnonymousMaxConcurrency = new Unlimited<uint>?(policy.AnonymousMaxConcurrency);
			this.AnonymousMaxBurst = new Unlimited<uint>?(policy.AnonymousMaxBurst);
			this.AnonymousRechargeRate = new Unlimited<uint>?(policy.AnonymousRechargeRate);
			this.AnonymousCutoffBalance = new Unlimited<uint>?(policy.AnonymousCutoffBalance);
			this.EasMaxConcurrency = new Unlimited<uint>?(policy.EasMaxConcurrency);
			this.EasMaxBurst = new Unlimited<uint>?(policy.EasMaxBurst);
			this.EasRechargeRate = new Unlimited<uint>?(policy.EasRechargeRate);
			this.EasCutoffBalance = new Unlimited<uint>?(policy.EasCutoffBalance);
			this.EasMaxDevices = new Unlimited<uint>?(policy.EasMaxDevices);
			this.EasMaxDeviceDeletesPerMonth = new Unlimited<uint>?(policy.EasMaxDeviceDeletesPerMonth);
			this.EasMaxInactivityForDeviceCleanup = new Unlimited<uint>?(policy.EasMaxInactivityForDeviceCleanup);
			this.EwsMaxConcurrency = new Unlimited<uint>?(policy.EwsMaxConcurrency);
			this.EwsMaxBurst = new Unlimited<uint>?(policy.EwsMaxBurst);
			this.EwsRechargeRate = new Unlimited<uint>?(policy.EwsRechargeRate);
			this.EwsCutoffBalance = new Unlimited<uint>?(policy.EwsCutoffBalance);
			this.EwsMaxSubscriptions = new Unlimited<uint>?(policy.EwsMaxSubscriptions);
			this.ImapMaxConcurrency = new Unlimited<uint>?(policy.ImapMaxConcurrency);
			this.ImapMaxBurst = new Unlimited<uint>?(policy.ImapMaxBurst);
			this.ImapRechargeRate = new Unlimited<uint>?(policy.ImapRechargeRate);
			this.ImapCutoffBalance = new Unlimited<uint>?(policy.ImapCutoffBalance);
			this.OutlookServiceMaxConcurrency = new Unlimited<uint>?(policy.OutlookServiceMaxConcurrency);
			this.OutlookServiceMaxBurst = new Unlimited<uint>?(policy.OutlookServiceMaxBurst);
			this.OutlookServiceRechargeRate = new Unlimited<uint>?(policy.OutlookServiceRechargeRate);
			this.OutlookServiceCutoffBalance = new Unlimited<uint>?(policy.OutlookServiceCutoffBalance);
			this.OutlookServiceMaxSubscriptions = new Unlimited<uint>?(policy.OutlookServiceMaxSubscriptions);
			this.OutlookServiceMaxSocketConnectionsPerDevice = new Unlimited<uint>?(policy.OutlookServiceMaxSocketConnectionsPerDevice);
			this.OutlookServiceMaxSocketConnectionsPerUser = new Unlimited<uint>?(policy.OutlookServiceMaxSocketConnectionsPerUser);
			this.OwaMaxConcurrency = new Unlimited<uint>?(policy.OwaMaxConcurrency);
			this.OwaMaxBurst = new Unlimited<uint>?(policy.OwaMaxBurst);
			this.OwaRechargeRate = new Unlimited<uint>?(policy.OwaRechargeRate);
			this.OwaCutoffBalance = new Unlimited<uint>?(policy.OwaCutoffBalance);
			this.OwaVoiceMaxConcurrency = new Unlimited<uint>?(policy.OwaVoiceMaxConcurrency);
			this.OwaVoiceMaxBurst = new Unlimited<uint>?(policy.OwaVoiceMaxBurst);
			this.OwaVoiceRechargeRate = new Unlimited<uint>?(policy.OwaVoiceRechargeRate);
			this.OwaVoiceCutoffBalance = new Unlimited<uint>?(policy.OwaVoiceCutoffBalance);
			this.PopMaxConcurrency = new Unlimited<uint>?(policy.PopMaxConcurrency);
			this.PopMaxBurst = new Unlimited<uint>?(policy.PopMaxBurst);
			this.PopRechargeRate = new Unlimited<uint>?(policy.PopRechargeRate);
			this.PopCutoffBalance = new Unlimited<uint>?(policy.PopCutoffBalance);
			this.PowerShellMaxConcurrency = new Unlimited<uint>?(policy.PowerShellMaxConcurrency);
			this.PowerShellMaxBurst = new Unlimited<uint>?(policy.PowerShellMaxBurst);
			this.PowerShellRechargeRate = new Unlimited<uint>?(policy.PowerShellRechargeRate);
			this.PowerShellCutoffBalance = new Unlimited<uint>?(policy.PowerShellCutoffBalance);
			this.PowerShellMaxTenantConcurrency = new Unlimited<uint>?(policy.PowerShellMaxTenantConcurrency);
			this.PowerShellMaxOperations = new Unlimited<uint>?(policy.PowerShellMaxOperations);
			this.PowerShellMaxCmdletsTimePeriod = new Unlimited<uint>?(policy.PowerShellMaxCmdletsTimePeriod);
			this.ExchangeMaxCmdlets = new Unlimited<uint>?(policy.ExchangeMaxCmdlets);
			this.PowerShellMaxCmdletQueueDepth = new Unlimited<uint>?(policy.PowerShellMaxCmdletQueueDepth);
			this.PowerShellMaxDestructiveCmdlets = new Unlimited<uint>?(policy.PowerShellMaxDestructiveCmdlets);
			this.PowerShellMaxDestructiveCmdletsTimePeriod = new Unlimited<uint>?(policy.PowerShellMaxDestructiveCmdletsTimePeriod);
			this.PowerShellMaxCmdlets = new Unlimited<uint>?(policy.PowerShellMaxCmdlets);
			this.PowerShellMaxRunspaces = new Unlimited<uint>?(policy.PowerShellMaxRunspaces);
			this.PowerShellMaxTenantRunspaces = new Unlimited<uint>?(policy.PowerShellMaxTenantRunspaces);
			this.PowerShellMaxRunspacesTimePeriod = new Unlimited<uint>?(policy.PowerShellMaxRunspacesTimePeriod);
			this.PswsMaxConcurrency = new Unlimited<uint>?(policy.PswsMaxConcurrency);
			this.PswsMaxRequest = new Unlimited<uint>?(policy.PswsMaxRequest);
			this.PswsMaxRequestTimePeriod = new Unlimited<uint>?(policy.PswsMaxRequestTimePeriod);
			this.MessageRateLimit = new Unlimited<uint>?(policy.MessageRateLimit);
			this.RecipientRateLimit = new Unlimited<uint>?(policy.RecipientRateLimit);
			this.ForwardeeLimit = new Unlimited<uint>?(policy.ForwardeeLimit);
			this.DiscoveryMaxConcurrency = new Unlimited<uint>?(policy.DiscoveryMaxConcurrency);
			this.DiscoveryMaxMailboxes = new Unlimited<uint>?(policy.DiscoveryMaxMailboxes);
			this.DiscoveryMaxKeywords = new Unlimited<uint>?(policy.DiscoveryMaxKeywords);
			this.DiscoveryMaxPreviewSearchMailboxes = new Unlimited<uint>?(policy.DiscoveryMaxPreviewSearchMailboxes);
			this.DiscoveryMaxStatsSearchMailboxes = new Unlimited<uint>?(policy.DiscoveryMaxStatsSearchMailboxes);
			this.DiscoveryPreviewSearchResultsPageSize = new Unlimited<uint>?(policy.DiscoveryPreviewSearchResultsPageSize);
			this.DiscoveryMaxKeywordsPerPage = new Unlimited<uint>?(policy.DiscoveryMaxKeywordsPerPage);
			this.DiscoveryMaxRefinerResults = new Unlimited<uint>?(policy.DiscoveryMaxRefinerResults);
			this.DiscoveryMaxSearchQueueDepth = new Unlimited<uint>?(policy.DiscoveryMaxSearchQueueDepth);
			this.DiscoverySearchTimeoutPeriod = new Unlimited<uint>?(policy.DiscoverySearchTimeoutPeriod);
			this.PushNotificationCutoffBalance = new Unlimited<uint>?(policy.PushNotificationCutoffBalance);
			this.PushNotificationMaxBurst = new Unlimited<uint>?(policy.PushNotificationMaxBurst);
			this.PushNotificationMaxConcurrency = new Unlimited<uint>?(policy.PushNotificationMaxConcurrency);
			this.PushNotificationRechargeRate = new Unlimited<uint>?(policy.PushNotificationRechargeRate);
			this.PushNotificationMaxBurstPerDevice = new Unlimited<uint>?(policy.PushNotificationMaxBurstPerDevice);
			this.PushNotificationRechargeRatePerDevice = new Unlimited<uint>?(policy.PushNotificationRechargeRatePerDevice);
			this.PushNotificationSamplingPeriodPerDevice = new Unlimited<uint>?(policy.PushNotificationSamplingPeriodPerDevice);
			this.RcaMaxConcurrency = new Unlimited<uint>?(policy.RcaMaxConcurrency);
			this.RcaMaxBurst = new Unlimited<uint>?(policy.RcaMaxBurst);
			this.RcaRechargeRate = new Unlimited<uint>?(policy.RcaRechargeRate);
			this.RcaCutoffBalance = new Unlimited<uint>?(policy.RcaCutoffBalance);
			this.CpaMaxConcurrency = new Unlimited<uint>?(policy.CpaMaxConcurrency);
			this.CpaMaxBurst = new Unlimited<uint>?(policy.CpaMaxBurst);
			this.CpaRechargeRate = new Unlimited<uint>?(policy.CpaRechargeRate);
			this.CpaCutoffBalance = new Unlimited<uint>?(policy.CpaCutoffBalance);
			this.EncryptionSenderMaxConcurrency = new Unlimited<uint>?(policy.EncryptionSenderMaxConcurrency);
			this.EncryptionSenderMaxBurst = new Unlimited<uint>?(policy.EncryptionSenderMaxBurst);
			this.EncryptionSenderRechargeRate = new Unlimited<uint>?(policy.EncryptionSenderRechargeRate);
			this.EncryptionSenderCutoffBalance = new Unlimited<uint>?(policy.EncryptionSenderCutoffBalance);
			this.EncryptionRecipientMaxConcurrency = new Unlimited<uint>?(policy.EncryptionRecipientMaxConcurrency);
			this.EncryptionRecipientMaxBurst = new Unlimited<uint>?(policy.EncryptionRecipientMaxBurst);
			this.EncryptionRecipientRechargeRate = new Unlimited<uint>?(policy.EncryptionRecipientRechargeRate);
			this.EncryptionRecipientCutoffBalance = new Unlimited<uint>?(policy.EncryptionRecipientCutoffBalance);
			this.ComplianceMaxExpansionDGRecipients = new Unlimited<uint>?(policy.ComplianceMaxExpansionDGRecipients);
			this.ComplianceMaxExpansionNestedDGs = new Unlimited<uint>?(policy.ComplianceMaxExpansionNestedDGs);
		}

		internal void UpgradeFromLegacyThrottlingPolicy()
		{
			this.LegacyThrottlingPolicy.UpgradeThrottlingSettingsTo(this);
		}

		internal void SaveLegacyThrottlingPolicy(IConfigDataProvider session)
		{
			if (this.LegacyThrottlingPolicy.m_Session == null)
			{
				this.LegacyThrottlingPolicy.m_Session = (IDirectorySession)session;
			}
			session.Save(this.LegacyThrottlingPolicy);
		}

		internal new void SetId(IConfigurationSession session, ADObjectId parent, string cn)
		{
			base.SetId(session, parent, cn);
			this.LegacyThrottlingPolicy.SetIdAndName(this);
		}

		internal EffectiveThrottlingPolicy GetEffectiveThrottlingPolicy(bool useCacheToGetParent = false)
		{
			return new EffectiveThrottlingPolicy(this, useCacheToGetParent);
		}

		internal void ConvertToEffectiveThrottlingPolicy(bool useCacheToGetParent = false)
		{
			IThrottlingPolicy effectiveThrottlingPolicy = this.GetEffectiveThrottlingPolicy(useCacheToGetParent);
			this.CloneThrottlingSettingsFrom(effectiveThrottlingPolicy);
		}

		public const string GlobalThrottlingPolicyNamePrefix = "GlobalThrottlingPolicy_";

		public const string TenantHydrationThrottlingPolicyName = "TenantHydrationThrottlingPolicy";

		public const string PartnerThrottlingPolicyName = "PartnerThrottlingPolicy";

		public const string MSOSyncServiceThrottlingPolicyName = "MSOSyncServiceThrottlingPolicy";

		public const string DiscoveryThrottlingPolicyName = "DiscoveryThrottlingPolicy";

		public const string PushNotificationServiceThrottlingPolicy = "PushNotificationServiceThrottlingPolicy";

		private static ThrottlingPolicySchema schema = ObjectSchema.GetInstance<ThrottlingPolicySchema>();

		private static string mostDerivedClass = "msExchThrottlingPolicy";

		private LegacyThrottlingPolicy legacyThrottlingPolicy;

		private string diagnostics;
	}
}
