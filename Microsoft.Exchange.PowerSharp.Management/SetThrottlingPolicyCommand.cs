using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetThrottlingPolicyCommand : SyntheticCommandWithPipelineInputNoOutput<ThrottlingPolicy>
	{
		private SetThrottlingPolicyCommand() : base("Set-ThrottlingPolicy")
		{
		}

		public SetThrottlingPolicyCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetThrottlingPolicyCommand SetParameters(SetThrottlingPolicyCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetThrottlingPolicyCommand SetParameters(SetThrottlingPolicyCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual SwitchParameter IgnoreDehydratedFlag
			{
				set
				{
					base.PowerSharpParameters["IgnoreDehydratedFlag"] = value;
				}
			}

			public virtual SwitchParameter ForceSettingGlobal
			{
				set
				{
					base.PowerSharpParameters["ForceSettingGlobal"] = value;
				}
			}

			public virtual Unlimited<uint>? AnonymousMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["AnonymousMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? AnonymousMaxBurst
			{
				set
				{
					base.PowerSharpParameters["AnonymousMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? AnonymousRechargeRate
			{
				set
				{
					base.PowerSharpParameters["AnonymousRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? AnonymousCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["AnonymousCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? EasMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["EasMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? EasMaxBurst
			{
				set
				{
					base.PowerSharpParameters["EasMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? EasRechargeRate
			{
				set
				{
					base.PowerSharpParameters["EasRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? EasCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["EasCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? EasMaxDevices
			{
				set
				{
					base.PowerSharpParameters["EasMaxDevices"] = value;
				}
			}

			public virtual Unlimited<uint>? EasMaxDeviceDeletesPerMonth
			{
				set
				{
					base.PowerSharpParameters["EasMaxDeviceDeletesPerMonth"] = value;
				}
			}

			public virtual Unlimited<uint>? EasMaxInactivityForDeviceCleanup
			{
				set
				{
					base.PowerSharpParameters["EasMaxInactivityForDeviceCleanup"] = value;
				}
			}

			public virtual Unlimited<uint>? EwsMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["EwsMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? EwsMaxBurst
			{
				set
				{
					base.PowerSharpParameters["EwsMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? EwsRechargeRate
			{
				set
				{
					base.PowerSharpParameters["EwsRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? EwsCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["EwsCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? EwsMaxSubscriptions
			{
				set
				{
					base.PowerSharpParameters["EwsMaxSubscriptions"] = value;
				}
			}

			public virtual Unlimited<uint>? ImapMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["ImapMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? ImapMaxBurst
			{
				set
				{
					base.PowerSharpParameters["ImapMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? ImapRechargeRate
			{
				set
				{
					base.PowerSharpParameters["ImapRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? ImapCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["ImapCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? OutlookServiceMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["OutlookServiceMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? OutlookServiceMaxBurst
			{
				set
				{
					base.PowerSharpParameters["OutlookServiceMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? OutlookServiceRechargeRate
			{
				set
				{
					base.PowerSharpParameters["OutlookServiceRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? OutlookServiceCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["OutlookServiceCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? OutlookServiceMaxSubscriptions
			{
				set
				{
					base.PowerSharpParameters["OutlookServiceMaxSubscriptions"] = value;
				}
			}

			public virtual Unlimited<uint>? OutlookServiceMaxSocketConnectionsPerDevice
			{
				set
				{
					base.PowerSharpParameters["OutlookServiceMaxSocketConnectionsPerDevice"] = value;
				}
			}

			public virtual Unlimited<uint>? OutlookServiceMaxSocketConnectionsPerUser
			{
				set
				{
					base.PowerSharpParameters["OutlookServiceMaxSocketConnectionsPerUser"] = value;
				}
			}

			public virtual Unlimited<uint>? OwaMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["OwaMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? OwaMaxBurst
			{
				set
				{
					base.PowerSharpParameters["OwaMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? OwaRechargeRate
			{
				set
				{
					base.PowerSharpParameters["OwaRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? OwaCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["OwaCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? OwaVoiceMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["OwaVoiceMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? OwaVoiceMaxBurst
			{
				set
				{
					base.PowerSharpParameters["OwaVoiceMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? OwaVoiceRechargeRate
			{
				set
				{
					base.PowerSharpParameters["OwaVoiceRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? OwaVoiceCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["OwaVoiceCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? EncryptionSenderMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["EncryptionSenderMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? EncryptionSenderMaxBurst
			{
				set
				{
					base.PowerSharpParameters["EncryptionSenderMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? EncryptionSenderRechargeRate
			{
				set
				{
					base.PowerSharpParameters["EncryptionSenderRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? EncryptionSenderCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["EncryptionSenderCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? EncryptionRecipientMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["EncryptionRecipientMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? EncryptionRecipientMaxBurst
			{
				set
				{
					base.PowerSharpParameters["EncryptionRecipientMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? EncryptionRecipientRechargeRate
			{
				set
				{
					base.PowerSharpParameters["EncryptionRecipientRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? EncryptionRecipientCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["EncryptionRecipientCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? PopMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["PopMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? PopMaxBurst
			{
				set
				{
					base.PowerSharpParameters["PopMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? PopRechargeRate
			{
				set
				{
					base.PowerSharpParameters["PopRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? PopCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["PopCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxBurst
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellRechargeRate
			{
				set
				{
					base.PowerSharpParameters["PowerShellRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["PowerShellCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxTenantConcurrency
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxTenantConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxOperations
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxOperations"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxCmdletsTimePeriod
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxCmdletsTimePeriod"] = value;
				}
			}

			public virtual Unlimited<uint>? ExchangeMaxCmdlets
			{
				set
				{
					base.PowerSharpParameters["ExchangeMaxCmdlets"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxCmdletQueueDepth
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxCmdletQueueDepth"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxDestructiveCmdlets
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxDestructiveCmdlets"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxDestructiveCmdletsTimePeriod
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxDestructiveCmdletsTimePeriod"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxCmdlets
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxCmdlets"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxRunspaces
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxRunspaces"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxTenantRunspaces
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxTenantRunspaces"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxRunspacesTimePeriod
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxRunspacesTimePeriod"] = value;
				}
			}

			public virtual Unlimited<uint>? PswsMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["PswsMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? PswsMaxRequest
			{
				set
				{
					base.PowerSharpParameters["PswsMaxRequest"] = value;
				}
			}

			public virtual Unlimited<uint>? PswsMaxRequestTimePeriod
			{
				set
				{
					base.PowerSharpParameters["PswsMaxRequestTimePeriod"] = value;
				}
			}

			public virtual Unlimited<uint>? RcaMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["RcaMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? RcaMaxBurst
			{
				set
				{
					base.PowerSharpParameters["RcaMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? RcaRechargeRate
			{
				set
				{
					base.PowerSharpParameters["RcaRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? RcaCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["RcaCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? CpaMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["CpaMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? CpaMaxBurst
			{
				set
				{
					base.PowerSharpParameters["CpaMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? CpaRechargeRate
			{
				set
				{
					base.PowerSharpParameters["CpaRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? CpaCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["CpaCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? MessageRateLimit
			{
				set
				{
					base.PowerSharpParameters["MessageRateLimit"] = value;
				}
			}

			public virtual Unlimited<uint>? RecipientRateLimit
			{
				set
				{
					base.PowerSharpParameters["RecipientRateLimit"] = value;
				}
			}

			public virtual Unlimited<uint>? ForwardeeLimit
			{
				set
				{
					base.PowerSharpParameters["ForwardeeLimit"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoveryMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["DiscoveryMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoveryMaxMailboxes
			{
				set
				{
					base.PowerSharpParameters["DiscoveryMaxMailboxes"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoveryMaxKeywords
			{
				set
				{
					base.PowerSharpParameters["DiscoveryMaxKeywords"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoveryMaxPreviewSearchMailboxes
			{
				set
				{
					base.PowerSharpParameters["DiscoveryMaxPreviewSearchMailboxes"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoveryMaxStatsSearchMailboxes
			{
				set
				{
					base.PowerSharpParameters["DiscoveryMaxStatsSearchMailboxes"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoveryPreviewSearchResultsPageSize
			{
				set
				{
					base.PowerSharpParameters["DiscoveryPreviewSearchResultsPageSize"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoveryMaxKeywordsPerPage
			{
				set
				{
					base.PowerSharpParameters["DiscoveryMaxKeywordsPerPage"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoveryMaxRefinerResults
			{
				set
				{
					base.PowerSharpParameters["DiscoveryMaxRefinerResults"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoveryMaxSearchQueueDepth
			{
				set
				{
					base.PowerSharpParameters["DiscoveryMaxSearchQueueDepth"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoverySearchTimeoutPeriod
			{
				set
				{
					base.PowerSharpParameters["DiscoverySearchTimeoutPeriod"] = value;
				}
			}

			public virtual Unlimited<uint>? ComplianceMaxExpansionDGRecipients
			{
				set
				{
					base.PowerSharpParameters["ComplianceMaxExpansionDGRecipients"] = value;
				}
			}

			public virtual Unlimited<uint>? ComplianceMaxExpansionNestedDGs
			{
				set
				{
					base.PowerSharpParameters["ComplianceMaxExpansionNestedDGs"] = value;
				}
			}

			public virtual Unlimited<uint>? PushNotificationMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["PushNotificationMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? PushNotificationMaxBurst
			{
				set
				{
					base.PowerSharpParameters["PushNotificationMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? PushNotificationRechargeRate
			{
				set
				{
					base.PowerSharpParameters["PushNotificationRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? PushNotificationCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["PushNotificationCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? PushNotificationMaxBurstPerDevice
			{
				set
				{
					base.PowerSharpParameters["PushNotificationMaxBurstPerDevice"] = value;
				}
			}

			public virtual Unlimited<uint>? PushNotificationRechargeRatePerDevice
			{
				set
				{
					base.PowerSharpParameters["PushNotificationRechargeRatePerDevice"] = value;
				}
			}

			public virtual Unlimited<uint>? PushNotificationSamplingPeriodPerDevice
			{
				set
				{
					base.PowerSharpParameters["PushNotificationSamplingPeriodPerDevice"] = value;
				}
			}

			public virtual SwitchParameter IsServiceAccount
			{
				set
				{
					base.PowerSharpParameters["IsServiceAccount"] = value;
				}
			}

			public virtual ThrottlingPolicyScopeType ThrottlingPolicyScope
			{
				set
				{
					base.PowerSharpParameters["ThrottlingPolicyScope"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ThrottlingPolicyIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual SwitchParameter IgnoreDehydratedFlag
			{
				set
				{
					base.PowerSharpParameters["IgnoreDehydratedFlag"] = value;
				}
			}

			public virtual SwitchParameter ForceSettingGlobal
			{
				set
				{
					base.PowerSharpParameters["ForceSettingGlobal"] = value;
				}
			}

			public virtual Unlimited<uint>? AnonymousMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["AnonymousMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? AnonymousMaxBurst
			{
				set
				{
					base.PowerSharpParameters["AnonymousMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? AnonymousRechargeRate
			{
				set
				{
					base.PowerSharpParameters["AnonymousRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? AnonymousCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["AnonymousCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? EasMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["EasMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? EasMaxBurst
			{
				set
				{
					base.PowerSharpParameters["EasMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? EasRechargeRate
			{
				set
				{
					base.PowerSharpParameters["EasRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? EasCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["EasCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? EasMaxDevices
			{
				set
				{
					base.PowerSharpParameters["EasMaxDevices"] = value;
				}
			}

			public virtual Unlimited<uint>? EasMaxDeviceDeletesPerMonth
			{
				set
				{
					base.PowerSharpParameters["EasMaxDeviceDeletesPerMonth"] = value;
				}
			}

			public virtual Unlimited<uint>? EasMaxInactivityForDeviceCleanup
			{
				set
				{
					base.PowerSharpParameters["EasMaxInactivityForDeviceCleanup"] = value;
				}
			}

			public virtual Unlimited<uint>? EwsMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["EwsMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? EwsMaxBurst
			{
				set
				{
					base.PowerSharpParameters["EwsMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? EwsRechargeRate
			{
				set
				{
					base.PowerSharpParameters["EwsRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? EwsCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["EwsCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? EwsMaxSubscriptions
			{
				set
				{
					base.PowerSharpParameters["EwsMaxSubscriptions"] = value;
				}
			}

			public virtual Unlimited<uint>? ImapMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["ImapMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? ImapMaxBurst
			{
				set
				{
					base.PowerSharpParameters["ImapMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? ImapRechargeRate
			{
				set
				{
					base.PowerSharpParameters["ImapRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? ImapCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["ImapCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? OutlookServiceMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["OutlookServiceMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? OutlookServiceMaxBurst
			{
				set
				{
					base.PowerSharpParameters["OutlookServiceMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? OutlookServiceRechargeRate
			{
				set
				{
					base.PowerSharpParameters["OutlookServiceRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? OutlookServiceCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["OutlookServiceCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? OutlookServiceMaxSubscriptions
			{
				set
				{
					base.PowerSharpParameters["OutlookServiceMaxSubscriptions"] = value;
				}
			}

			public virtual Unlimited<uint>? OutlookServiceMaxSocketConnectionsPerDevice
			{
				set
				{
					base.PowerSharpParameters["OutlookServiceMaxSocketConnectionsPerDevice"] = value;
				}
			}

			public virtual Unlimited<uint>? OutlookServiceMaxSocketConnectionsPerUser
			{
				set
				{
					base.PowerSharpParameters["OutlookServiceMaxSocketConnectionsPerUser"] = value;
				}
			}

			public virtual Unlimited<uint>? OwaMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["OwaMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? OwaMaxBurst
			{
				set
				{
					base.PowerSharpParameters["OwaMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? OwaRechargeRate
			{
				set
				{
					base.PowerSharpParameters["OwaRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? OwaCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["OwaCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? OwaVoiceMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["OwaVoiceMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? OwaVoiceMaxBurst
			{
				set
				{
					base.PowerSharpParameters["OwaVoiceMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? OwaVoiceRechargeRate
			{
				set
				{
					base.PowerSharpParameters["OwaVoiceRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? OwaVoiceCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["OwaVoiceCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? EncryptionSenderMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["EncryptionSenderMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? EncryptionSenderMaxBurst
			{
				set
				{
					base.PowerSharpParameters["EncryptionSenderMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? EncryptionSenderRechargeRate
			{
				set
				{
					base.PowerSharpParameters["EncryptionSenderRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? EncryptionSenderCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["EncryptionSenderCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? EncryptionRecipientMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["EncryptionRecipientMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? EncryptionRecipientMaxBurst
			{
				set
				{
					base.PowerSharpParameters["EncryptionRecipientMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? EncryptionRecipientRechargeRate
			{
				set
				{
					base.PowerSharpParameters["EncryptionRecipientRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? EncryptionRecipientCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["EncryptionRecipientCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? PopMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["PopMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? PopMaxBurst
			{
				set
				{
					base.PowerSharpParameters["PopMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? PopRechargeRate
			{
				set
				{
					base.PowerSharpParameters["PopRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? PopCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["PopCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxBurst
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellRechargeRate
			{
				set
				{
					base.PowerSharpParameters["PowerShellRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["PowerShellCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxTenantConcurrency
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxTenantConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxOperations
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxOperations"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxCmdletsTimePeriod
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxCmdletsTimePeriod"] = value;
				}
			}

			public virtual Unlimited<uint>? ExchangeMaxCmdlets
			{
				set
				{
					base.PowerSharpParameters["ExchangeMaxCmdlets"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxCmdletQueueDepth
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxCmdletQueueDepth"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxDestructiveCmdlets
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxDestructiveCmdlets"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxDestructiveCmdletsTimePeriod
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxDestructiveCmdletsTimePeriod"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxCmdlets
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxCmdlets"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxRunspaces
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxRunspaces"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxTenantRunspaces
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxTenantRunspaces"] = value;
				}
			}

			public virtual Unlimited<uint>? PowerShellMaxRunspacesTimePeriod
			{
				set
				{
					base.PowerSharpParameters["PowerShellMaxRunspacesTimePeriod"] = value;
				}
			}

			public virtual Unlimited<uint>? PswsMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["PswsMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? PswsMaxRequest
			{
				set
				{
					base.PowerSharpParameters["PswsMaxRequest"] = value;
				}
			}

			public virtual Unlimited<uint>? PswsMaxRequestTimePeriod
			{
				set
				{
					base.PowerSharpParameters["PswsMaxRequestTimePeriod"] = value;
				}
			}

			public virtual Unlimited<uint>? RcaMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["RcaMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? RcaMaxBurst
			{
				set
				{
					base.PowerSharpParameters["RcaMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? RcaRechargeRate
			{
				set
				{
					base.PowerSharpParameters["RcaRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? RcaCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["RcaCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? CpaMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["CpaMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? CpaMaxBurst
			{
				set
				{
					base.PowerSharpParameters["CpaMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? CpaRechargeRate
			{
				set
				{
					base.PowerSharpParameters["CpaRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? CpaCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["CpaCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? MessageRateLimit
			{
				set
				{
					base.PowerSharpParameters["MessageRateLimit"] = value;
				}
			}

			public virtual Unlimited<uint>? RecipientRateLimit
			{
				set
				{
					base.PowerSharpParameters["RecipientRateLimit"] = value;
				}
			}

			public virtual Unlimited<uint>? ForwardeeLimit
			{
				set
				{
					base.PowerSharpParameters["ForwardeeLimit"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoveryMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["DiscoveryMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoveryMaxMailboxes
			{
				set
				{
					base.PowerSharpParameters["DiscoveryMaxMailboxes"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoveryMaxKeywords
			{
				set
				{
					base.PowerSharpParameters["DiscoveryMaxKeywords"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoveryMaxPreviewSearchMailboxes
			{
				set
				{
					base.PowerSharpParameters["DiscoveryMaxPreviewSearchMailboxes"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoveryMaxStatsSearchMailboxes
			{
				set
				{
					base.PowerSharpParameters["DiscoveryMaxStatsSearchMailboxes"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoveryPreviewSearchResultsPageSize
			{
				set
				{
					base.PowerSharpParameters["DiscoveryPreviewSearchResultsPageSize"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoveryMaxKeywordsPerPage
			{
				set
				{
					base.PowerSharpParameters["DiscoveryMaxKeywordsPerPage"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoveryMaxRefinerResults
			{
				set
				{
					base.PowerSharpParameters["DiscoveryMaxRefinerResults"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoveryMaxSearchQueueDepth
			{
				set
				{
					base.PowerSharpParameters["DiscoveryMaxSearchQueueDepth"] = value;
				}
			}

			public virtual Unlimited<uint>? DiscoverySearchTimeoutPeriod
			{
				set
				{
					base.PowerSharpParameters["DiscoverySearchTimeoutPeriod"] = value;
				}
			}

			public virtual Unlimited<uint>? ComplianceMaxExpansionDGRecipients
			{
				set
				{
					base.PowerSharpParameters["ComplianceMaxExpansionDGRecipients"] = value;
				}
			}

			public virtual Unlimited<uint>? ComplianceMaxExpansionNestedDGs
			{
				set
				{
					base.PowerSharpParameters["ComplianceMaxExpansionNestedDGs"] = value;
				}
			}

			public virtual Unlimited<uint>? PushNotificationMaxConcurrency
			{
				set
				{
					base.PowerSharpParameters["PushNotificationMaxConcurrency"] = value;
				}
			}

			public virtual Unlimited<uint>? PushNotificationMaxBurst
			{
				set
				{
					base.PowerSharpParameters["PushNotificationMaxBurst"] = value;
				}
			}

			public virtual Unlimited<uint>? PushNotificationRechargeRate
			{
				set
				{
					base.PowerSharpParameters["PushNotificationRechargeRate"] = value;
				}
			}

			public virtual Unlimited<uint>? PushNotificationCutoffBalance
			{
				set
				{
					base.PowerSharpParameters["PushNotificationCutoffBalance"] = value;
				}
			}

			public virtual Unlimited<uint>? PushNotificationMaxBurstPerDevice
			{
				set
				{
					base.PowerSharpParameters["PushNotificationMaxBurstPerDevice"] = value;
				}
			}

			public virtual Unlimited<uint>? PushNotificationRechargeRatePerDevice
			{
				set
				{
					base.PowerSharpParameters["PushNotificationRechargeRatePerDevice"] = value;
				}
			}

			public virtual Unlimited<uint>? PushNotificationSamplingPeriodPerDevice
			{
				set
				{
					base.PowerSharpParameters["PushNotificationSamplingPeriodPerDevice"] = value;
				}
			}

			public virtual SwitchParameter IsServiceAccount
			{
				set
				{
					base.PowerSharpParameters["IsServiceAccount"] = value;
				}
			}

			public virtual ThrottlingPolicyScopeType ThrottlingPolicyScope
			{
				set
				{
					base.PowerSharpParameters["ThrottlingPolicyScope"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}
	}
}
