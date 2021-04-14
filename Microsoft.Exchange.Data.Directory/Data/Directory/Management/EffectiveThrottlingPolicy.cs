using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class EffectiveThrottlingPolicy : IThrottlingPolicy
	{
		static EffectiveThrottlingPolicy()
		{
			EffectiveThrottlingPolicy.rootOrgConfigSession.SessionSettings.IsSharedConfigChecked = true;
		}

		public EffectiveThrottlingPolicy(ThrottlingPolicy dataObject) : this(dataObject, false)
		{
		}

		public EffectiveThrottlingPolicy(ThrottlingPolicy dataObject, bool useCacheToGetParent)
		{
			if (dataObject == null)
			{
				throw new ArgumentNullException("dataObject");
			}
			this.ThrottlingPolicy = dataObject;
			IThrottlingPolicy parentThrottlingPolicy;
			if (useCacheToGetParent)
			{
				parentThrottlingPolicy = this.GetParentThrottlingPolicyFromCache(dataObject);
			}
			else
			{
				parentThrottlingPolicy = this.GetParentThrottlingPolicyFromAD(dataObject);
			}
			this.MergeValuesFromParentPolicy(parentThrottlingPolicy);
		}

		private static ThrottlingPolicy ReadOrganizationThrottlingPolicyFromAD(OrganizationId orgId)
		{
			SharedConfiguration sharedConfiguration = SharedConfiguration.GetSharedConfiguration(orgId);
			if (sharedConfiguration != null)
			{
				orgId = sharedConfiguration.SharedConfigId;
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(orgId);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, sessionSettings, 224, "ReadOrganizationThrottlingPolicyFromAD", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\throttling\\EffectiveThrottlingPolicy.cs");
			tenantOrTopologyConfigurationSession.SessionSettings.IsSharedConfigChecked = true;
			return tenantOrTopologyConfigurationSession.GetOrganizationThrottlingPolicy(orgId);
		}

		internal static ThrottlingPolicy ReadGlobalThrottlingPolicyFromAD()
		{
			return EffectiveThrottlingPolicy.rootOrgConfigSession.GetGlobalThrottlingPolicy(true);
		}

		private IThrottlingPolicy GetParentThrottlingPolicyFromCache(ThrottlingPolicy policy)
		{
			IThrottlingPolicy result;
			switch (policy.ThrottlingPolicyScope)
			{
			case ThrottlingPolicyScopeType.Regular:
				result = ThrottlingPolicyCache.Singleton.Get(this.ThrottlingPolicy.OrganizationId);
				break;
			case ThrottlingPolicyScopeType.Organization:
				result = ThrottlingPolicyCache.Singleton.GetGlobalThrottlingPolicy();
				break;
			case ThrottlingPolicyScopeType.Global:
				result = FallbackThrottlingPolicy.GetSingleton();
				break;
			default:
				throw new NotSupportedException(string.Format("Unsupported enum value {0}.", this.ThrottlingPolicy.ThrottlingPolicyScope));
			}
			return result;
		}

		private IThrottlingPolicy GetParentThrottlingPolicyFromAD(ThrottlingPolicy policy)
		{
			IThrottlingPolicy result;
			switch (this.ThrottlingPolicy.ThrottlingPolicyScope)
			{
			case ThrottlingPolicyScopeType.Regular:
			{
				ThrottlingPolicy throttlingPolicy = EffectiveThrottlingPolicy.ReadOrganizationThrottlingPolicyFromAD(policy.OrganizationId);
				if (throttlingPolicy == null)
				{
					throttlingPolicy = EffectiveThrottlingPolicy.ReadGlobalThrottlingPolicyFromAD();
				}
				result = throttlingPolicy.GetEffectiveThrottlingPolicy(false);
				break;
			}
			case ThrottlingPolicyScopeType.Organization:
				result = EffectiveThrottlingPolicy.ReadGlobalThrottlingPolicyFromAD().GetEffectiveThrottlingPolicy(false);
				break;
			case ThrottlingPolicyScopeType.Global:
				result = FallbackThrottlingPolicy.GetSingleton();
				break;
			default:
				throw new NotSupportedException(string.Format("Unsupported enum value {0}.", this.ThrottlingPolicy.ThrottlingPolicyScope));
			}
			return result;
		}

		internal ThrottlingPolicy ThrottlingPolicy { get; private set; }

		public ADObjectId Id
		{
			get
			{
				return this.ThrottlingPolicy.Id;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.ThrottlingPolicy.OrganizationId;
			}
		}

		public bool IsFallback
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

		string IThrottlingPolicy.GetIdentityString()
		{
			return this.ThrottlingPolicy.DistinguishedName;
		}

		string IThrottlingPolicy.GetShortIdentityString()
		{
			return this.ThrottlingPolicy.Name;
		}

		public ThrottlingPolicyScopeType ThrottlingPolicyScope
		{
			get
			{
				return this.ThrottlingPolicy.ThrottlingPolicyScope;
			}
		}

		public bool IsServiceAccount
		{
			get
			{
				return this.ThrottlingPolicy.IsServiceAccount;
			}
		}

		public Unlimited<uint> AnonymousMaxConcurrency
		{
			get
			{
				return this.anonymousMaxConcurrency;
			}
		}

		public Unlimited<uint> AnonymousMaxBurst
		{
			get
			{
				return this.anonymousMaxBurst;
			}
		}

		public Unlimited<uint> AnonymousRechargeRate
		{
			get
			{
				return this.anonymousRechargeRate;
			}
		}

		public Unlimited<uint> AnonymousCutoffBalance
		{
			get
			{
				return this.anonymousCutoffBalance;
			}
		}

		public Unlimited<uint> EasMaxConcurrency
		{
			get
			{
				return this.easMaxConcurrency;
			}
		}

		public Unlimited<uint> EasMaxBurst
		{
			get
			{
				return this.easMaxBurst;
			}
		}

		public Unlimited<uint> EasRechargeRate
		{
			get
			{
				return this.easRechargeRate;
			}
		}

		public Unlimited<uint> EasCutoffBalance
		{
			get
			{
				return this.easCutoffBalance;
			}
		}

		public Unlimited<uint> EasMaxDevices
		{
			get
			{
				return this.easMaxDevices;
			}
		}

		public Unlimited<uint> EasMaxDeviceDeletesPerMonth
		{
			get
			{
				return this.easMaxDeviceDeletesPerMonth;
			}
		}

		public Unlimited<uint> EasMaxInactivityForDeviceCleanup
		{
			get
			{
				return this.easMaxInactivityForDeviceCleanup;
			}
		}

		public Unlimited<uint> EwsMaxConcurrency
		{
			get
			{
				return this.ewsMaxConcurrency;
			}
		}

		public Unlimited<uint> EwsMaxBurst
		{
			get
			{
				return this.ewsMaxBurst;
			}
		}

		public Unlimited<uint> EwsRechargeRate
		{
			get
			{
				return this.ewsRechargeRate;
			}
		}

		public Unlimited<uint> EwsCutoffBalance
		{
			get
			{
				return this.ewsCutoffBalance;
			}
		}

		public Unlimited<uint> EwsMaxSubscriptions
		{
			get
			{
				return this.ewsMaxSubscriptions;
			}
		}

		public Unlimited<uint> ImapMaxConcurrency
		{
			get
			{
				return this.imapMaxConcurrency;
			}
		}

		public Unlimited<uint> ImapMaxBurst
		{
			get
			{
				return this.imapMaxBurst;
			}
		}

		public Unlimited<uint> ImapRechargeRate
		{
			get
			{
				return this.imapRechargeRate;
			}
		}

		public Unlimited<uint> ImapCutoffBalance
		{
			get
			{
				return this.imapCutoffBalance;
			}
		}

		public Unlimited<uint> OutlookServiceMaxConcurrency
		{
			get
			{
				return this.outlookServiceMaxConcurrency;
			}
		}

		public Unlimited<uint> OutlookServiceMaxBurst
		{
			get
			{
				return this.outlookServiceMaxBurst;
			}
		}

		public Unlimited<uint> OutlookServiceRechargeRate
		{
			get
			{
				return this.outlookServiceRechargeRate;
			}
		}

		public Unlimited<uint> OutlookServiceCutoffBalance
		{
			get
			{
				return this.outlookServiceCutoffBalance;
			}
		}

		public Unlimited<uint> OutlookServiceMaxSubscriptions
		{
			get
			{
				return this.outlookServiceMaxSubscriptions;
			}
		}

		public Unlimited<uint> OutlookServiceMaxSocketConnectionsPerDevice
		{
			get
			{
				return this.outlookServiceMaxSocketConnectionsPerDevice;
			}
		}

		public Unlimited<uint> OutlookServiceMaxSocketConnectionsPerUser
		{
			get
			{
				return this.outlookServiceMaxSocketConnectionsPerUser;
			}
		}

		public Unlimited<uint> OwaMaxConcurrency
		{
			get
			{
				return this.owaMaxConcurrency;
			}
		}

		public Unlimited<uint> OwaMaxBurst
		{
			get
			{
				return this.owaMaxBurst;
			}
		}

		public Unlimited<uint> OwaRechargeRate
		{
			get
			{
				return this.owaRechargeRate;
			}
		}

		public Unlimited<uint> OwaCutoffBalance
		{
			get
			{
				return this.owaCutoffBalance;
			}
		}

		public Unlimited<uint> OwaVoiceMaxConcurrency
		{
			get
			{
				return this.owaVoiceMaxConcurrency;
			}
		}

		public Unlimited<uint> OwaVoiceMaxBurst
		{
			get
			{
				return this.owaVoiceMaxBurst;
			}
		}

		public Unlimited<uint> OwaVoiceRechargeRate
		{
			get
			{
				return this.owaVoiceRechargeRate;
			}
		}

		public Unlimited<uint> OwaVoiceCutoffBalance
		{
			get
			{
				return this.owaVoiceCutoffBalance;
			}
		}

		public Unlimited<uint> PopMaxConcurrency
		{
			get
			{
				return this.popMaxConcurrency;
			}
		}

		public Unlimited<uint> PopMaxBurst
		{
			get
			{
				return this.popMaxBurst;
			}
		}

		public Unlimited<uint> PopRechargeRate
		{
			get
			{
				return this.popRechargeRate;
			}
		}

		public Unlimited<uint> PopCutoffBalance
		{
			get
			{
				return this.popCutoffBalance;
			}
		}

		public Unlimited<uint> RcaMaxConcurrency
		{
			get
			{
				return this.rcaMaxConcurrency;
			}
		}

		public Unlimited<uint> RcaMaxBurst
		{
			get
			{
				return this.rcaMaxBurst;
			}
		}

		public Unlimited<uint> RcaRechargeRate
		{
			get
			{
				return this.rcaRechargeRate;
			}
		}

		public Unlimited<uint> RcaCutoffBalance
		{
			get
			{
				return this.rcaCutoffBalance;
			}
		}

		public Unlimited<uint> CpaMaxConcurrency
		{
			get
			{
				return this.cpaMaxConcurrency;
			}
		}

		public Unlimited<uint> CpaMaxBurst
		{
			get
			{
				return this.cpaMaxBurst;
			}
		}

		public Unlimited<uint> CpaRechargeRate
		{
			get
			{
				return this.cpaRechargeRate;
			}
		}

		public Unlimited<uint> CpaCutoffBalance
		{
			get
			{
				return this.cpaCutoffBalance;
			}
		}

		public Unlimited<uint> PowerShellMaxConcurrency
		{
			get
			{
				return this.powerShellMaxConcurrency;
			}
		}

		public Unlimited<uint> PowerShellMaxBurst
		{
			get
			{
				return this.powerShellMaxBurst;
			}
		}

		public Unlimited<uint> PowerShellRechargeRate
		{
			get
			{
				return this.powerShellRechargeRate;
			}
		}

		public Unlimited<uint> PowerShellCutoffBalance
		{
			get
			{
				return this.powerShellCutoffBalance;
			}
		}

		public Unlimited<uint> PowerShellMaxTenantConcurrency
		{
			get
			{
				return this.powerShellMaxTenantConcurrency;
			}
		}

		public Unlimited<uint> PowerShellMaxOperations
		{
			get
			{
				return this.powerShellMaxOperations;
			}
		}

		public Unlimited<uint> PowerShellMaxCmdletsTimePeriod
		{
			get
			{
				return this.powerShellMaxCmdletsTimePeriod;
			}
		}

		public Unlimited<uint> PowerShellMaxCmdletQueueDepth
		{
			get
			{
				return this.powerShellMaxCmdletQueueDepth;
			}
		}

		public Unlimited<uint> ExchangeMaxCmdlets
		{
			get
			{
				return this.exchangeMaxCmdlets;
			}
		}

		public Unlimited<uint> PowerShellMaxDestructiveCmdlets
		{
			get
			{
				return this.powerShellMaxDestructiveCmdlets;
			}
		}

		public Unlimited<uint> PowerShellMaxDestructiveCmdletsTimePeriod
		{
			get
			{
				return this.powerShellMaxDestructiveCmdletsTimePeriod;
			}
		}

		public Unlimited<uint> PowerShellMaxCmdlets
		{
			get
			{
				return this.powerShellMaxCmdlets;
			}
		}

		public Unlimited<uint> PowerShellMaxRunspaces
		{
			get
			{
				return this.powerShellMaxRunspaces;
			}
		}

		public Unlimited<uint> PowerShellMaxTenantRunspaces
		{
			get
			{
				return this.powerShellMaxTenantRunspaces;
			}
		}

		public Unlimited<uint> PowerShellMaxRunspacesTimePeriod
		{
			get
			{
				return this.powerShellMaxRunspacesTimePeriod;
			}
		}

		public Unlimited<uint> PswsMaxConcurrency
		{
			get
			{
				return this.pswsMaxConcurrency;
			}
		}

		public Unlimited<uint> PswsMaxRequest
		{
			get
			{
				return this.pswsMaxRequest;
			}
		}

		public Unlimited<uint> PswsMaxRequestTimePeriod
		{
			get
			{
				return this.pswsMaxRequestTimePeriod;
			}
		}

		public Unlimited<uint> MessageRateLimit
		{
			get
			{
				return this.messageRateLimit;
			}
		}

		public Unlimited<uint> RecipientRateLimit
		{
			get
			{
				return this.recipientRateLimit;
			}
		}

		public Unlimited<uint> ForwardeeLimit
		{
			get
			{
				return this.forwardeeLimit;
			}
		}

		public Unlimited<uint> DiscoveryMaxConcurrency
		{
			get
			{
				return this.discoveryMaxConcurrency;
			}
		}

		public Unlimited<uint> DiscoveryMaxMailboxes
		{
			get
			{
				return this.discoveryMaxMailboxes;
			}
		}

		public Unlimited<uint> DiscoveryMaxKeywords
		{
			get
			{
				return this.discoveryMaxKeywords;
			}
		}

		public Unlimited<uint> DiscoveryMaxPreviewSearchMailboxes
		{
			get
			{
				return this.discoveryMaxPreviewSearchMailboxes;
			}
		}

		public Unlimited<uint> DiscoveryMaxStatsSearchMailboxes
		{
			get
			{
				return this.discoveryMaxStatsSearchMailboxes;
			}
		}

		public Unlimited<uint> DiscoveryPreviewSearchResultsPageSize
		{
			get
			{
				return this.discoveryPreviewSearchResultsPageSize;
			}
		}

		public Unlimited<uint> DiscoveryMaxKeywordsPerPage
		{
			get
			{
				return this.discoveryMaxKeywordsPerPage;
			}
		}

		public Unlimited<uint> DiscoveryMaxRefinerResults
		{
			get
			{
				return this.discoveryMaxRefinerResults;
			}
		}

		public Unlimited<uint> DiscoveryMaxSearchQueueDepth
		{
			get
			{
				return this.discoveryMaxSearchQueueDepth;
			}
		}

		public Unlimited<uint> DiscoverySearchTimeoutPeriod
		{
			get
			{
				return this.discoverySearchTimeoutPeriod;
			}
		}

		public Unlimited<uint> PushNotificationMaxConcurrency
		{
			get
			{
				return this.pushNotificationMaxConcurrency;
			}
		}

		public Unlimited<uint> PushNotificationMaxBurst
		{
			get
			{
				return this.pushNotificationMaxBurst;
			}
		}

		public Unlimited<uint> PushNotificationRechargeRate
		{
			get
			{
				return this.pushNotificationRechargeRate;
			}
		}

		public Unlimited<uint> PushNotificationCutoffBalance
		{
			get
			{
				return this.pushNotificationCutoffBalance;
			}
		}

		public Unlimited<uint> PushNotificationMaxBurstPerDevice
		{
			get
			{
				return this.pushNotificationMaxBurstPerDevice;
			}
		}

		public Unlimited<uint> PushNotificationRechargeRatePerDevice
		{
			get
			{
				return this.pushNotificationRechargeRatePerDevice;
			}
		}

		public Unlimited<uint> PushNotificationSamplingPeriodPerDevice
		{
			get
			{
				return this.pushNotificationSamplingPeriodPerDevice;
			}
		}

		public Unlimited<uint> EncryptionSenderMaxConcurrency
		{
			get
			{
				return this.encryptionSenderMaxConcurrency;
			}
		}

		public Unlimited<uint> EncryptionSenderMaxBurst
		{
			get
			{
				return this.encryptionSenderMaxBurst;
			}
		}

		public Unlimited<uint> EncryptionSenderRechargeRate
		{
			get
			{
				return this.encryptionSenderRechargeRate;
			}
		}

		public Unlimited<uint> EncryptionSenderCutoffBalance
		{
			get
			{
				return this.encryptionSenderCutoffBalance;
			}
		}

		public Unlimited<uint> EncryptionRecipientMaxConcurrency
		{
			get
			{
				return this.encryptionRecipientMaxConcurrency;
			}
		}

		public Unlimited<uint> EncryptionRecipientMaxBurst
		{
			get
			{
				return this.encryptionRecipientMaxBurst;
			}
		}

		public Unlimited<uint> EncryptionRecipientRechargeRate
		{
			get
			{
				return this.encryptionRecipientRechargeRate;
			}
		}

		public Unlimited<uint> EncryptionRecipientCutoffBalance
		{
			get
			{
				return this.encryptionRecipientCutoffBalance;
			}
		}

		public Unlimited<uint> ComplianceMaxExpansionDGRecipients
		{
			get
			{
				return this.complianceMaxExpansionDGRecipients;
			}
		}

		public Unlimited<uint> ComplianceMaxExpansionNestedDGs
		{
			get
			{
				return this.complianceMaxExpansionNestedDGs;
			}
		}

		private void MergeValuesFromParentPolicy(IThrottlingPolicy parentThrottlingPolicy)
		{
			this.anonymousMaxConcurrency = (this.ThrottlingPolicy.AnonymousMaxConcurrency ?? parentThrottlingPolicy.AnonymousMaxConcurrency);
			this.anonymousMaxBurst = (this.ThrottlingPolicy.AnonymousMaxBurst ?? parentThrottlingPolicy.AnonymousMaxBurst);
			this.anonymousRechargeRate = (this.ThrottlingPolicy.AnonymousRechargeRate ?? parentThrottlingPolicy.AnonymousRechargeRate);
			this.anonymousCutoffBalance = (this.ThrottlingPolicy.AnonymousCutoffBalance ?? parentThrottlingPolicy.AnonymousCutoffBalance);
			this.easMaxConcurrency = (this.ThrottlingPolicy.EasMaxConcurrency ?? parentThrottlingPolicy.EasMaxConcurrency);
			this.easMaxBurst = (this.ThrottlingPolicy.EasMaxBurst ?? parentThrottlingPolicy.EasMaxBurst);
			this.easRechargeRate = (this.ThrottlingPolicy.EasRechargeRate ?? parentThrottlingPolicy.EasRechargeRate);
			this.easCutoffBalance = (this.ThrottlingPolicy.EasCutoffBalance ?? parentThrottlingPolicy.EasCutoffBalance);
			this.easMaxDevices = (this.ThrottlingPolicy.EasMaxDevices ?? parentThrottlingPolicy.EasMaxDevices);
			this.easMaxDeviceDeletesPerMonth = (this.ThrottlingPolicy.EasMaxDeviceDeletesPerMonth ?? parentThrottlingPolicy.EasMaxDeviceDeletesPerMonth);
			this.easMaxInactivityForDeviceCleanup = (this.ThrottlingPolicy.EasMaxInactivityForDeviceCleanup ?? parentThrottlingPolicy.EasMaxInactivityForDeviceCleanup);
			this.ewsMaxConcurrency = (this.ThrottlingPolicy.EwsMaxConcurrency ?? parentThrottlingPolicy.EwsMaxConcurrency);
			this.ewsMaxBurst = (this.ThrottlingPolicy.EwsMaxBurst ?? parentThrottlingPolicy.EwsMaxBurst);
			this.ewsRechargeRate = (this.ThrottlingPolicy.EwsRechargeRate ?? parentThrottlingPolicy.EwsRechargeRate);
			this.ewsCutoffBalance = (this.ThrottlingPolicy.EwsCutoffBalance ?? parentThrottlingPolicy.EwsCutoffBalance);
			this.ewsMaxSubscriptions = (this.ThrottlingPolicy.EwsMaxSubscriptions ?? parentThrottlingPolicy.EwsMaxSubscriptions);
			this.imapMaxConcurrency = (this.ThrottlingPolicy.ImapMaxConcurrency ?? parentThrottlingPolicy.ImapMaxConcurrency);
			this.imapMaxBurst = (this.ThrottlingPolicy.ImapMaxBurst ?? parentThrottlingPolicy.ImapMaxBurst);
			this.imapRechargeRate = (this.ThrottlingPolicy.ImapRechargeRate ?? parentThrottlingPolicy.ImapRechargeRate);
			this.imapCutoffBalance = (this.ThrottlingPolicy.ImapCutoffBalance ?? parentThrottlingPolicy.ImapCutoffBalance);
			this.outlookServiceMaxConcurrency = (this.ThrottlingPolicy.OutlookServiceMaxConcurrency ?? parentThrottlingPolicy.OutlookServiceMaxConcurrency);
			this.outlookServiceMaxBurst = (this.ThrottlingPolicy.OutlookServiceMaxBurst ?? parentThrottlingPolicy.OutlookServiceMaxBurst);
			this.outlookServiceRechargeRate = (this.ThrottlingPolicy.OutlookServiceRechargeRate ?? parentThrottlingPolicy.OutlookServiceRechargeRate);
			this.outlookServiceCutoffBalance = (this.ThrottlingPolicy.OutlookServiceCutoffBalance ?? parentThrottlingPolicy.OutlookServiceCutoffBalance);
			this.outlookServiceMaxSubscriptions = (this.ThrottlingPolicy.OutlookServiceMaxSubscriptions ?? parentThrottlingPolicy.OutlookServiceMaxSubscriptions);
			this.outlookServiceMaxSocketConnectionsPerDevice = (this.ThrottlingPolicy.OutlookServiceMaxSocketConnectionsPerDevice ?? parentThrottlingPolicy.OutlookServiceMaxSocketConnectionsPerDevice);
			this.outlookServiceMaxSocketConnectionsPerUser = (this.ThrottlingPolicy.OutlookServiceMaxSocketConnectionsPerUser ?? parentThrottlingPolicy.OutlookServiceMaxSocketConnectionsPerUser);
			this.owaMaxConcurrency = (this.ThrottlingPolicy.OwaMaxConcurrency ?? parentThrottlingPolicy.OwaMaxConcurrency);
			this.owaMaxBurst = (this.ThrottlingPolicy.OwaMaxBurst ?? parentThrottlingPolicy.OwaMaxBurst);
			this.owaRechargeRate = (this.ThrottlingPolicy.OwaRechargeRate ?? parentThrottlingPolicy.OwaRechargeRate);
			this.owaCutoffBalance = (this.ThrottlingPolicy.OwaCutoffBalance ?? parentThrottlingPolicy.OwaCutoffBalance);
			this.owaVoiceMaxConcurrency = (this.ThrottlingPolicy.OwaVoiceMaxConcurrency ?? parentThrottlingPolicy.OwaVoiceMaxConcurrency);
			this.owaVoiceMaxBurst = (this.ThrottlingPolicy.OwaVoiceMaxBurst ?? parentThrottlingPolicy.OwaVoiceMaxBurst);
			this.owaVoiceRechargeRate = (this.ThrottlingPolicy.OwaVoiceRechargeRate ?? parentThrottlingPolicy.OwaVoiceRechargeRate);
			this.owaVoiceCutoffBalance = (this.ThrottlingPolicy.OwaVoiceCutoffBalance ?? parentThrottlingPolicy.OwaVoiceCutoffBalance);
			this.popMaxConcurrency = (this.ThrottlingPolicy.PopMaxConcurrency ?? parentThrottlingPolicy.PopMaxConcurrency);
			this.popMaxBurst = (this.ThrottlingPolicy.PopMaxBurst ?? parentThrottlingPolicy.PopMaxBurst);
			this.popRechargeRate = (this.ThrottlingPolicy.PopRechargeRate ?? parentThrottlingPolicy.PopRechargeRate);
			this.popCutoffBalance = (this.ThrottlingPolicy.PopCutoffBalance ?? parentThrottlingPolicy.PopCutoffBalance);
			this.powerShellMaxConcurrency = (this.ThrottlingPolicy.PowerShellMaxConcurrency ?? parentThrottlingPolicy.PowerShellMaxConcurrency);
			this.powerShellMaxBurst = (this.ThrottlingPolicy.PowerShellMaxBurst ?? parentThrottlingPolicy.PowerShellMaxBurst);
			this.powerShellRechargeRate = (this.ThrottlingPolicy.PowerShellRechargeRate ?? parentThrottlingPolicy.PowerShellRechargeRate);
			this.powerShellCutoffBalance = (this.ThrottlingPolicy.PowerShellCutoffBalance ?? parentThrottlingPolicy.PowerShellCutoffBalance);
			this.powerShellMaxTenantConcurrency = (this.ThrottlingPolicy.PowerShellMaxTenantConcurrency ?? parentThrottlingPolicy.PowerShellMaxTenantConcurrency);
			this.powerShellMaxOperations = (this.ThrottlingPolicy.PowerShellMaxOperations ?? parentThrottlingPolicy.PowerShellMaxOperations);
			this.powerShellMaxCmdletsTimePeriod = (this.ThrottlingPolicy.PowerShellMaxCmdletsTimePeriod ?? parentThrottlingPolicy.PowerShellMaxCmdletsTimePeriod);
			this.exchangeMaxCmdlets = (this.ThrottlingPolicy.ExchangeMaxCmdlets ?? parentThrottlingPolicy.ExchangeMaxCmdlets);
			this.powerShellMaxCmdletQueueDepth = (this.ThrottlingPolicy.PowerShellMaxCmdletQueueDepth ?? parentThrottlingPolicy.PowerShellMaxCmdletQueueDepth);
			this.powerShellMaxDestructiveCmdlets = (this.ThrottlingPolicy.PowerShellMaxDestructiveCmdlets ?? parentThrottlingPolicy.PowerShellMaxDestructiveCmdlets);
			this.powerShellMaxDestructiveCmdletsTimePeriod = (this.ThrottlingPolicy.PowerShellMaxDestructiveCmdletsTimePeriod ?? parentThrottlingPolicy.PowerShellMaxDestructiveCmdletsTimePeriod);
			this.powerShellMaxCmdlets = (this.ThrottlingPolicy.PowerShellMaxCmdlets ?? parentThrottlingPolicy.PowerShellMaxCmdlets);
			this.powerShellMaxRunspaces = (this.ThrottlingPolicy.PowerShellMaxRunspaces ?? parentThrottlingPolicy.PowerShellMaxRunspaces);
			this.powerShellMaxTenantRunspaces = (this.ThrottlingPolicy.PowerShellMaxTenantRunspaces ?? parentThrottlingPolicy.PowerShellMaxTenantRunspaces);
			this.powerShellMaxRunspacesTimePeriod = (this.ThrottlingPolicy.PowerShellMaxRunspacesTimePeriod ?? parentThrottlingPolicy.PowerShellMaxRunspacesTimePeriod);
			this.pswsMaxConcurrency = (this.ThrottlingPolicy.PswsMaxConcurrency ?? parentThrottlingPolicy.PswsMaxConcurrency);
			this.pswsMaxRequest = (this.ThrottlingPolicy.PswsMaxRequest ?? parentThrottlingPolicy.PswsMaxRequest);
			this.pswsMaxRequestTimePeriod = (this.ThrottlingPolicy.PswsMaxRequestTimePeriod ?? parentThrottlingPolicy.PswsMaxRequestTimePeriod);
			this.rcaMaxConcurrency = (this.ThrottlingPolicy.RcaMaxConcurrency ?? parentThrottlingPolicy.RcaMaxConcurrency);
			this.rcaMaxBurst = (this.ThrottlingPolicy.RcaMaxBurst ?? parentThrottlingPolicy.RcaMaxBurst);
			this.rcaRechargeRate = (this.ThrottlingPolicy.RcaRechargeRate ?? parentThrottlingPolicy.RcaRechargeRate);
			this.rcaCutoffBalance = (this.ThrottlingPolicy.RcaCutoffBalance ?? parentThrottlingPolicy.RcaCutoffBalance);
			this.cpaMaxConcurrency = (this.ThrottlingPolicy.CpaMaxConcurrency ?? parentThrottlingPolicy.CpaMaxConcurrency);
			this.cpaMaxBurst = (this.ThrottlingPolicy.CpaMaxBurst ?? parentThrottlingPolicy.CpaMaxBurst);
			this.cpaRechargeRate = (this.ThrottlingPolicy.CpaRechargeRate ?? parentThrottlingPolicy.CpaRechargeRate);
			this.cpaCutoffBalance = (this.ThrottlingPolicy.CpaCutoffBalance ?? parentThrottlingPolicy.CpaCutoffBalance);
			this.messageRateLimit = (this.ThrottlingPolicy.MessageRateLimit ?? parentThrottlingPolicy.MessageRateLimit);
			this.recipientRateLimit = (this.ThrottlingPolicy.RecipientRateLimit ?? parentThrottlingPolicy.RecipientRateLimit);
			this.forwardeeLimit = (this.ThrottlingPolicy.ForwardeeLimit ?? parentThrottlingPolicy.ForwardeeLimit);
			this.discoveryMaxConcurrency = (this.ThrottlingPolicy.DiscoveryMaxConcurrency ?? parentThrottlingPolicy.DiscoveryMaxConcurrency);
			this.discoveryMaxMailboxes = (this.ThrottlingPolicy.DiscoveryMaxMailboxes ?? parentThrottlingPolicy.DiscoveryMaxMailboxes);
			this.discoveryMaxKeywords = (this.ThrottlingPolicy.DiscoveryMaxKeywords ?? parentThrottlingPolicy.DiscoveryMaxKeywords);
			this.discoveryMaxPreviewSearchMailboxes = (this.ThrottlingPolicy.DiscoveryMaxPreviewSearchMailboxes ?? parentThrottlingPolicy.DiscoveryMaxPreviewSearchMailboxes);
			this.discoveryMaxStatsSearchMailboxes = (this.ThrottlingPolicy.DiscoveryMaxStatsSearchMailboxes ?? parentThrottlingPolicy.DiscoveryMaxStatsSearchMailboxes);
			this.discoveryPreviewSearchResultsPageSize = (this.ThrottlingPolicy.DiscoveryPreviewSearchResultsPageSize ?? parentThrottlingPolicy.DiscoveryPreviewSearchResultsPageSize);
			this.discoveryMaxKeywordsPerPage = (this.ThrottlingPolicy.DiscoveryMaxKeywordsPerPage ?? parentThrottlingPolicy.DiscoveryMaxKeywordsPerPage);
			this.discoveryMaxRefinerResults = (this.ThrottlingPolicy.DiscoveryMaxRefinerResults ?? parentThrottlingPolicy.DiscoveryMaxRefinerResults);
			this.discoveryMaxSearchQueueDepth = (this.ThrottlingPolicy.DiscoveryMaxSearchQueueDepth ?? parentThrottlingPolicy.DiscoveryMaxSearchQueueDepth);
			this.discoverySearchTimeoutPeriod = (this.ThrottlingPolicy.DiscoverySearchTimeoutPeriod ?? parentThrottlingPolicy.DiscoverySearchTimeoutPeriod);
			this.pushNotificationMaxConcurrency = (this.ThrottlingPolicy.PushNotificationMaxConcurrency ?? parentThrottlingPolicy.PushNotificationMaxConcurrency);
			this.pushNotificationMaxBurst = (this.ThrottlingPolicy.PushNotificationMaxBurst ?? parentThrottlingPolicy.PushNotificationMaxBurst);
			this.pushNotificationRechargeRate = (this.ThrottlingPolicy.PushNotificationRechargeRate ?? parentThrottlingPolicy.PushNotificationRechargeRate);
			this.pushNotificationCutoffBalance = (this.ThrottlingPolicy.PushNotificationCutoffBalance ?? parentThrottlingPolicy.PushNotificationCutoffBalance);
			this.pushNotificationMaxBurstPerDevice = (this.ThrottlingPolicy.PushNotificationMaxBurstPerDevice ?? parentThrottlingPolicy.PushNotificationMaxBurstPerDevice);
			this.pushNotificationRechargeRatePerDevice = (this.ThrottlingPolicy.PushNotificationRechargeRatePerDevice ?? parentThrottlingPolicy.PushNotificationRechargeRatePerDevice);
			this.pushNotificationSamplingPeriodPerDevice = (this.ThrottlingPolicy.PushNotificationSamplingPeriodPerDevice ?? parentThrottlingPolicy.PushNotificationSamplingPeriodPerDevice);
			this.encryptionSenderMaxConcurrency = (this.ThrottlingPolicy.EncryptionSenderMaxConcurrency ?? parentThrottlingPolicy.EncryptionSenderMaxConcurrency);
			this.encryptionSenderMaxBurst = (this.ThrottlingPolicy.EncryptionSenderMaxBurst ?? parentThrottlingPolicy.EncryptionSenderMaxBurst);
			this.encryptionSenderRechargeRate = (this.ThrottlingPolicy.EncryptionSenderRechargeRate ?? parentThrottlingPolicy.EncryptionSenderRechargeRate);
			this.encryptionSenderCutoffBalance = (this.ThrottlingPolicy.EncryptionSenderCutoffBalance ?? parentThrottlingPolicy.EncryptionSenderCutoffBalance);
			this.encryptionRecipientMaxConcurrency = (this.ThrottlingPolicy.EncryptionRecipientMaxConcurrency ?? parentThrottlingPolicy.EncryptionRecipientMaxConcurrency);
			this.encryptionRecipientMaxBurst = (this.ThrottlingPolicy.EncryptionRecipientMaxBurst ?? parentThrottlingPolicy.EncryptionRecipientMaxBurst);
			this.encryptionRecipientRechargeRate = (this.ThrottlingPolicy.EncryptionRecipientRechargeRate ?? parentThrottlingPolicy.EncryptionRecipientRechargeRate);
			this.encryptionRecipientCutoffBalance = (this.ThrottlingPolicy.EncryptionRecipientCutoffBalance ?? parentThrottlingPolicy.EncryptionRecipientCutoffBalance);
			this.complianceMaxExpansionDGRecipients = (this.ThrottlingPolicy.ComplianceMaxExpansionDGRecipients ?? parentThrottlingPolicy.ComplianceMaxExpansionDGRecipients);
			this.complianceMaxExpansionNestedDGs = (this.ThrottlingPolicy.ComplianceMaxExpansionNestedDGs ?? parentThrottlingPolicy.ComplianceMaxExpansionNestedDGs);
		}

		private static ITopologyConfigurationSession rootOrgConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 156, ".cctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\throttling\\EffectiveThrottlingPolicy.cs");

		private Unlimited<uint> anonymousMaxConcurrency;

		private Unlimited<uint> anonymousMaxBurst;

		private Unlimited<uint> anonymousRechargeRate;

		private Unlimited<uint> anonymousCutoffBalance;

		private Unlimited<uint> easMaxConcurrency;

		private Unlimited<uint> easMaxBurst;

		private Unlimited<uint> easRechargeRate;

		private Unlimited<uint> easCutoffBalance;

		private Unlimited<uint> easMaxDevices;

		private Unlimited<uint> easMaxDeviceDeletesPerMonth;

		private Unlimited<uint> easMaxInactivityForDeviceCleanup;

		private Unlimited<uint> ewsMaxConcurrency;

		private Unlimited<uint> ewsMaxBurst;

		private Unlimited<uint> ewsRechargeRate;

		private Unlimited<uint> ewsCutoffBalance;

		private Unlimited<uint> ewsMaxSubscriptions;

		private Unlimited<uint> imapMaxConcurrency;

		private Unlimited<uint> imapMaxBurst;

		private Unlimited<uint> imapRechargeRate;

		private Unlimited<uint> imapCutoffBalance;

		private Unlimited<uint> outlookServiceMaxConcurrency;

		private Unlimited<uint> outlookServiceMaxBurst;

		private Unlimited<uint> outlookServiceRechargeRate;

		private Unlimited<uint> outlookServiceCutoffBalance;

		private Unlimited<uint> outlookServiceMaxSubscriptions;

		private Unlimited<uint> outlookServiceMaxSocketConnectionsPerDevice;

		private Unlimited<uint> outlookServiceMaxSocketConnectionsPerUser;

		private Unlimited<uint> owaMaxConcurrency;

		private Unlimited<uint> owaMaxBurst;

		private Unlimited<uint> owaRechargeRate;

		private Unlimited<uint> owaCutoffBalance;

		private Unlimited<uint> owaVoiceMaxConcurrency;

		private Unlimited<uint> owaVoiceMaxBurst;

		private Unlimited<uint> owaVoiceRechargeRate;

		private Unlimited<uint> owaVoiceCutoffBalance;

		private Unlimited<uint> popMaxConcurrency;

		private Unlimited<uint> popMaxBurst;

		private Unlimited<uint> popRechargeRate;

		private Unlimited<uint> popCutoffBalance;

		private Unlimited<uint> powerShellMaxConcurrency;

		private Unlimited<uint> powerShellMaxBurst;

		private Unlimited<uint> powerShellRechargeRate;

		private Unlimited<uint> powerShellCutoffBalance;

		private Unlimited<uint> powerShellMaxTenantConcurrency;

		private Unlimited<uint> powerShellMaxOperations;

		private Unlimited<uint> powerShellMaxCmdletsTimePeriod;

		private Unlimited<uint> exchangeMaxCmdlets;

		private Unlimited<uint> powerShellMaxCmdletQueueDepth;

		private Unlimited<uint> powerShellMaxDestructiveCmdlets;

		private Unlimited<uint> powerShellMaxDestructiveCmdletsTimePeriod;

		private Unlimited<uint> powerShellMaxCmdlets;

		private Unlimited<uint> powerShellMaxRunspaces;

		private Unlimited<uint> powerShellMaxTenantRunspaces;

		private Unlimited<uint> powerShellMaxRunspacesTimePeriod;

		private Unlimited<uint> pswsMaxConcurrency;

		private Unlimited<uint> pswsMaxRequest;

		private Unlimited<uint> pswsMaxRequestTimePeriod;

		private Unlimited<uint> rcaMaxConcurrency;

		private Unlimited<uint> rcaMaxBurst;

		private Unlimited<uint> rcaRechargeRate;

		private Unlimited<uint> rcaCutoffBalance;

		private Unlimited<uint> cpaMaxConcurrency;

		private Unlimited<uint> cpaMaxBurst;

		private Unlimited<uint> cpaRechargeRate;

		private Unlimited<uint> cpaCutoffBalance;

		private Unlimited<uint> messageRateLimit;

		private Unlimited<uint> recipientRateLimit;

		private Unlimited<uint> forwardeeLimit;

		private Unlimited<uint> discoveryMaxConcurrency;

		private Unlimited<uint> discoveryMaxMailboxes;

		private Unlimited<uint> discoveryMaxKeywords;

		private Unlimited<uint> discoveryMaxPreviewSearchMailboxes;

		private Unlimited<uint> discoveryMaxStatsSearchMailboxes;

		private Unlimited<uint> discoveryPreviewSearchResultsPageSize;

		private Unlimited<uint> discoveryMaxKeywordsPerPage;

		private Unlimited<uint> discoveryMaxRefinerResults;

		private Unlimited<uint> discoveryMaxSearchQueueDepth;

		private Unlimited<uint> discoverySearchTimeoutPeriod;

		private Unlimited<uint> pushNotificationMaxConcurrency;

		private Unlimited<uint> pushNotificationMaxBurst;

		private Unlimited<uint> pushNotificationRechargeRate;

		private Unlimited<uint> pushNotificationCutoffBalance;

		private Unlimited<uint> pushNotificationMaxBurstPerDevice;

		private Unlimited<uint> pushNotificationRechargeRatePerDevice;

		private Unlimited<uint> pushNotificationSamplingPeriodPerDevice;

		private Unlimited<uint> encryptionSenderMaxConcurrency;

		private Unlimited<uint> encryptionSenderMaxBurst;

		private Unlimited<uint> encryptionSenderRechargeRate;

		private Unlimited<uint> encryptionSenderCutoffBalance;

		private Unlimited<uint> encryptionRecipientMaxConcurrency;

		private Unlimited<uint> encryptionRecipientMaxBurst;

		private Unlimited<uint> encryptionRecipientRechargeRate;

		private Unlimited<uint> encryptionRecipientCutoffBalance;

		private Unlimited<uint> complianceMaxExpansionDGRecipients;

		private Unlimited<uint> complianceMaxExpansionNestedDGs;
	}
}
