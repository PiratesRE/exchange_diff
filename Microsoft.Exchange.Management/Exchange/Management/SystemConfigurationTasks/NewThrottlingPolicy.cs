using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "ThrottlingPolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class NewThrottlingPolicy : NewMultitenancySystemConfigurationObjectTask<ThrottlingPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewThrottlingPolicy(base.Name.ToString(), this.ThrottlingPolicyScope.ToString());
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		[Parameter(Mandatory = false)]
		public override SwitchParameter IgnoreDehydratedFlag { get; set; }

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? AnonymousMaxConcurrency
		{
			get
			{
				return this.DataObject.AnonymousMaxConcurrency;
			}
			set
			{
				this.DataObject.AnonymousMaxConcurrency = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? AnonymousMaxBurst
		{
			get
			{
				return this.DataObject.AnonymousMaxBurst;
			}
			set
			{
				this.DataObject.AnonymousMaxBurst = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? AnonymousRechargeRate
		{
			get
			{
				return this.DataObject.AnonymousRechargeRate;
			}
			set
			{
				this.DataObject.AnonymousRechargeRate = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? AnonymousCutoffBalance
		{
			get
			{
				return this.DataObject.AnonymousCutoffBalance;
			}
			set
			{
				this.DataObject.AnonymousCutoffBalance = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EasMaxConcurrency
		{
			get
			{
				return this.DataObject.EasMaxConcurrency;
			}
			set
			{
				this.DataObject.EasMaxConcurrency = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EasMaxBurst
		{
			get
			{
				return this.DataObject.EasMaxBurst;
			}
			set
			{
				this.DataObject.EasMaxBurst = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EasRechargeRate
		{
			get
			{
				return this.DataObject.EasRechargeRate;
			}
			set
			{
				this.DataObject.EasRechargeRate = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EasCutoffBalance
		{
			get
			{
				return this.DataObject.EasCutoffBalance;
			}
			set
			{
				this.DataObject.EasCutoffBalance = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EasMaxDevices
		{
			get
			{
				return this.DataObject.EasMaxDevices;
			}
			set
			{
				this.DataObject.EasMaxDevices = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EasMaxDeviceDeletesPerMonth
		{
			get
			{
				return this.DataObject.EasMaxDeviceDeletesPerMonth;
			}
			set
			{
				this.DataObject.EasMaxDeviceDeletesPerMonth = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EasMaxInactivityForDeviceCleanup
		{
			get
			{
				return this.DataObject.EasMaxInactivityForDeviceCleanup;
			}
			set
			{
				this.DataObject.EasMaxInactivityForDeviceCleanup = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EwsMaxConcurrency
		{
			get
			{
				return this.DataObject.EwsMaxConcurrency;
			}
			set
			{
				this.DataObject.EwsMaxConcurrency = value;
				base.Fields[ThrottlingPolicySchema.EwsMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EwsMaxBurst
		{
			get
			{
				return this.DataObject.EwsMaxBurst;
			}
			set
			{
				this.DataObject.EwsMaxBurst = value;
				base.Fields[ThrottlingPolicySchema.EwsMaxBurst] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EwsRechargeRate
		{
			get
			{
				return this.DataObject.EwsRechargeRate;
			}
			set
			{
				this.DataObject.EwsRechargeRate = value;
				base.Fields[ThrottlingPolicySchema.EwsRechargeRate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EwsCutoffBalance
		{
			get
			{
				return this.DataObject.EwsCutoffBalance;
			}
			set
			{
				this.DataObject.EwsCutoffBalance = value;
				base.Fields[ThrottlingPolicySchema.EwsCutoffBalance] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EwsMaxSubscriptions
		{
			get
			{
				return this.DataObject.EwsMaxSubscriptions;
			}
			set
			{
				this.DataObject.EwsMaxSubscriptions = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? ImapMaxConcurrency
		{
			get
			{
				return this.DataObject.ImapMaxConcurrency;
			}
			set
			{
				this.DataObject.ImapMaxConcurrency = value;
				base.Fields[ThrottlingPolicySchema.ImapMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? ImapMaxBurst
		{
			get
			{
				return this.DataObject.ImapMaxBurst;
			}
			set
			{
				this.DataObject.ImapMaxBurst = value;
				base.Fields[ThrottlingPolicySchema.ImapMaxBurst] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? ImapRechargeRate
		{
			get
			{
				return this.DataObject.ImapRechargeRate;
			}
			set
			{
				this.DataObject.ImapRechargeRate = value;
				base.Fields[ThrottlingPolicySchema.ImapRechargeRate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? ImapCutoffBalance
		{
			get
			{
				return this.DataObject.ImapCutoffBalance;
			}
			set
			{
				this.DataObject.ImapCutoffBalance = value;
				base.Fields[ThrottlingPolicySchema.ImapCutoffBalance] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OutlookServiceMaxConcurrency
		{
			get
			{
				return this.DataObject.OutlookServiceMaxConcurrency;
			}
			set
			{
				this.DataObject.OutlookServiceMaxConcurrency = value;
				base.Fields[ThrottlingPolicySchema.OutlookServiceMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OutlookServiceMaxBurst
		{
			get
			{
				return this.DataObject.OutlookServiceMaxBurst;
			}
			set
			{
				this.DataObject.OutlookServiceMaxBurst = value;
				base.Fields[ThrottlingPolicySchema.OutlookServiceMaxBurst] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OutlookServiceRechargeRate
		{
			get
			{
				return this.DataObject.OutlookServiceRechargeRate;
			}
			set
			{
				this.DataObject.OutlookServiceRechargeRate = value;
				base.Fields[ThrottlingPolicySchema.OutlookServiceRechargeRate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OutlookServiceCutoffBalance
		{
			get
			{
				return this.DataObject.OutlookServiceCutoffBalance;
			}
			set
			{
				this.DataObject.OutlookServiceCutoffBalance = value;
				base.Fields[ThrottlingPolicySchema.OutlookServiceCutoffBalance] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OutlookServiceMaxSubscriptions
		{
			get
			{
				return this.DataObject.OutlookServiceMaxSubscriptions;
			}
			set
			{
				this.DataObject.OutlookServiceMaxSubscriptions = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OutlookServiceMaxSocketConnectionsPerDevice
		{
			get
			{
				return this.DataObject.OutlookServiceMaxSocketConnectionsPerDevice;
			}
			set
			{
				this.DataObject.OutlookServiceMaxSocketConnectionsPerDevice = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OutlookServiceMaxSocketConnectionsPerUser
		{
			get
			{
				return this.DataObject.OutlookServiceMaxSocketConnectionsPerUser;
			}
			set
			{
				this.DataObject.OutlookServiceMaxSocketConnectionsPerUser = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OwaMaxConcurrency
		{
			get
			{
				return this.DataObject.OwaMaxConcurrency;
			}
			set
			{
				this.DataObject.OwaMaxConcurrency = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OwaMaxBurst
		{
			get
			{
				return this.DataObject.OwaMaxBurst;
			}
			set
			{
				this.DataObject.OwaMaxBurst = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OwaRechargeRate
		{
			get
			{
				return this.DataObject.OwaRechargeRate;
			}
			set
			{
				this.DataObject.OwaRechargeRate = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OwaCutoffBalance
		{
			get
			{
				return this.DataObject.OwaCutoffBalance;
			}
			set
			{
				this.DataObject.OwaCutoffBalance = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OwaVoiceMaxConcurrency
		{
			get
			{
				return this.DataObject.OwaVoiceMaxConcurrency;
			}
			set
			{
				this.DataObject.OwaVoiceMaxConcurrency = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OwaVoiceMaxBurst
		{
			get
			{
				return this.DataObject.OwaVoiceMaxBurst;
			}
			set
			{
				this.DataObject.OwaVoiceMaxBurst = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OwaVoiceRechargeRate
		{
			get
			{
				return this.DataObject.OwaVoiceRechargeRate;
			}
			set
			{
				this.DataObject.OwaVoiceRechargeRate = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OwaVoiceCutoffBalance
		{
			get
			{
				return this.DataObject.OwaVoiceCutoffBalance;
			}
			set
			{
				this.DataObject.OwaVoiceCutoffBalance = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EncryptionSenderMaxConcurrency
		{
			get
			{
				return this.DataObject.EncryptionSenderMaxConcurrency;
			}
			set
			{
				this.DataObject.EncryptionSenderMaxConcurrency = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EncryptionSenderMaxBurst
		{
			get
			{
				return this.DataObject.EncryptionSenderMaxBurst;
			}
			set
			{
				this.DataObject.EncryptionSenderMaxBurst = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EncryptionSenderRechargeRate
		{
			get
			{
				return this.DataObject.EncryptionSenderRechargeRate;
			}
			set
			{
				this.DataObject.EncryptionSenderRechargeRate = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EncryptionSenderCutoffBalance
		{
			get
			{
				return this.DataObject.EncryptionSenderCutoffBalance;
			}
			set
			{
				this.DataObject.EncryptionSenderCutoffBalance = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EncryptionRecipientMaxConcurrency
		{
			get
			{
				return this.DataObject.EncryptionRecipientMaxConcurrency;
			}
			set
			{
				this.DataObject.EncryptionRecipientMaxConcurrency = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EncryptionRecipientMaxBurst
		{
			get
			{
				return this.DataObject.EncryptionRecipientMaxBurst;
			}
			set
			{
				this.DataObject.EncryptionRecipientMaxBurst = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EncryptionRecipientRechargeRate
		{
			get
			{
				return this.DataObject.EncryptionRecipientRechargeRate;
			}
			set
			{
				this.DataObject.EncryptionRecipientRechargeRate = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EncryptionRecipientCutoffBalance
		{
			get
			{
				return this.DataObject.EncryptionRecipientCutoffBalance;
			}
			set
			{
				this.DataObject.EncryptionRecipientCutoffBalance = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PopMaxConcurrency
		{
			get
			{
				return this.DataObject.PopMaxConcurrency;
			}
			set
			{
				this.DataObject.PopMaxConcurrency = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PopMaxBurst
		{
			get
			{
				return this.DataObject.PopMaxBurst;
			}
			set
			{
				this.DataObject.PopMaxBurst = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PopRechargeRate
		{
			get
			{
				return this.DataObject.PopRechargeRate;
			}
			set
			{
				this.DataObject.PopRechargeRate = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PopCutoffBalance
		{
			get
			{
				return this.DataObject.PopCutoffBalance;
			}
			set
			{
				this.DataObject.PopCutoffBalance = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxConcurrency
		{
			get
			{
				return this.DataObject.PowerShellMaxConcurrency;
			}
			set
			{
				this.DataObject.PowerShellMaxConcurrency = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxBurst
		{
			get
			{
				return this.DataObject.PowerShellMaxBurst;
			}
			set
			{
				this.DataObject.PowerShellMaxBurst = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellRechargeRate
		{
			get
			{
				return this.DataObject.PowerShellRechargeRate;
			}
			set
			{
				this.DataObject.PowerShellRechargeRate = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellCutoffBalance
		{
			get
			{
				return this.DataObject.PowerShellCutoffBalance;
			}
			set
			{
				this.DataObject.PowerShellCutoffBalance = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxTenantConcurrency
		{
			get
			{
				return this.DataObject.PowerShellMaxTenantConcurrency;
			}
			set
			{
				this.DataObject.PowerShellMaxTenantConcurrency = value;
				base.Fields[ThrottlingPolicySchema.PowerShellMaxTenantConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxOperations
		{
			get
			{
				return this.DataObject.PowerShellMaxOperations;
			}
			set
			{
				this.DataObject.PowerShellMaxOperations = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? ExchangeMaxCmdlets
		{
			get
			{
				return this.DataObject.ExchangeMaxCmdlets;
			}
			set
			{
				this.DataObject.ExchangeMaxCmdlets = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxCmdletsTimePeriod
		{
			get
			{
				return this.DataObject.PowerShellMaxCmdletsTimePeriod;
			}
			set
			{
				this.DataObject.PowerShellMaxCmdletsTimePeriod = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxCmdletQueueDepth
		{
			get
			{
				return this.DataObject.PowerShellMaxCmdletQueueDepth;
			}
			set
			{
				this.DataObject.PowerShellMaxCmdletQueueDepth = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxDestructiveCmdlets
		{
			get
			{
				return this.DataObject.PowerShellMaxDestructiveCmdlets;
			}
			set
			{
				this.DataObject.PowerShellMaxDestructiveCmdlets = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxDestructiveCmdletsTimePeriod
		{
			get
			{
				return this.DataObject.PowerShellMaxDestructiveCmdletsTimePeriod;
			}
			set
			{
				this.DataObject.PowerShellMaxDestructiveCmdletsTimePeriod = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxCmdlets
		{
			get
			{
				return this.DataObject.PowerShellMaxCmdlets;
			}
			set
			{
				this.DataObject.PowerShellMaxCmdlets = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxRunspaces
		{
			get
			{
				return this.DataObject.PowerShellMaxRunspaces;
			}
			set
			{
				this.DataObject.PowerShellMaxRunspaces = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxTenantRunspaces
		{
			get
			{
				return this.DataObject.PowerShellMaxTenantRunspaces;
			}
			set
			{
				this.DataObject.PowerShellMaxTenantRunspaces = value;
				base.Fields[ThrottlingPolicySchema.PowerShellMaxTenantRunspaces] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxRunspacesTimePeriod
		{
			get
			{
				return this.DataObject.PowerShellMaxRunspacesTimePeriod;
			}
			set
			{
				this.DataObject.PowerShellMaxRunspacesTimePeriod = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PswsMaxConcurrency
		{
			get
			{
				return this.DataObject.PswsMaxConcurrency;
			}
			set
			{
				this.DataObject.PswsMaxConcurrency = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PswsMaxRequest
		{
			get
			{
				return this.DataObject.PswsMaxRequest;
			}
			set
			{
				this.DataObject.PswsMaxRequest = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PswsMaxRequestTimePeriod
		{
			get
			{
				return this.DataObject.PswsMaxRequestTimePeriod;
			}
			set
			{
				this.DataObject.PswsMaxRequestTimePeriod = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? RcaMaxConcurrency
		{
			get
			{
				return this.DataObject.RcaMaxConcurrency;
			}
			set
			{
				this.DataObject.RcaMaxConcurrency = value;
				base.Fields[ThrottlingPolicySchema.RcaMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? RcaMaxBurst
		{
			get
			{
				return this.DataObject.RcaMaxBurst;
			}
			set
			{
				this.DataObject.RcaMaxBurst = value;
				base.Fields[ThrottlingPolicySchema.RcaMaxBurst] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? RcaRechargeRate
		{
			get
			{
				return this.DataObject.RcaRechargeRate;
			}
			set
			{
				this.DataObject.RcaRechargeRate = value;
				base.Fields[ThrottlingPolicySchema.RcaRechargeRate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? RcaCutoffBalance
		{
			get
			{
				return this.DataObject.RcaCutoffBalance;
			}
			set
			{
				this.DataObject.RcaCutoffBalance = value;
				base.Fields[ThrottlingPolicySchema.RcaCutoffBalance] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? CpaMaxConcurrency
		{
			get
			{
				return this.DataObject.CpaMaxConcurrency;
			}
			set
			{
				this.DataObject.CpaMaxConcurrency = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? CpaMaxBurst
		{
			get
			{
				return this.DataObject.CpaMaxBurst;
			}
			set
			{
				this.DataObject.CpaMaxBurst = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? CpaRechargeRate
		{
			get
			{
				return this.DataObject.CpaRechargeRate;
			}
			set
			{
				this.DataObject.CpaRechargeRate = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? CpaCutoffBalance
		{
			get
			{
				return this.DataObject.CpaCutoffBalance;
			}
			set
			{
				this.DataObject.CpaCutoffBalance = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? MessageRateLimit
		{
			get
			{
				return this.DataObject.MessageRateLimit;
			}
			set
			{
				this.DataObject.MessageRateLimit = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? RecipientRateLimit
		{
			get
			{
				return this.DataObject.RecipientRateLimit;
			}
			set
			{
				this.DataObject.RecipientRateLimit = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? ForwardeeLimit
		{
			get
			{
				return this.DataObject.ForwardeeLimit;
			}
			set
			{
				this.DataObject.ForwardeeLimit = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoveryMaxConcurrency
		{
			get
			{
				return this.DataObject.DiscoveryMaxConcurrency;
			}
			set
			{
				this.DataObject.DiscoveryMaxConcurrency = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoveryMaxMailboxes
		{
			get
			{
				return this.DataObject.DiscoveryMaxMailboxes;
			}
			set
			{
				this.DataObject.DiscoveryMaxMailboxes = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoveryMaxKeywords
		{
			get
			{
				return this.DataObject.DiscoveryMaxKeywords;
			}
			set
			{
				this.DataObject.DiscoveryMaxKeywords = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoveryMaxPreviewSearchMailboxes
		{
			get
			{
				return this.DataObject.DiscoveryMaxPreviewSearchMailboxes;
			}
			set
			{
				this.DataObject.DiscoveryMaxPreviewSearchMailboxes = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoveryMaxStatsSearchMailboxes
		{
			get
			{
				return this.DataObject.DiscoveryMaxStatsSearchMailboxes;
			}
			set
			{
				this.DataObject.DiscoveryMaxStatsSearchMailboxes = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoveryPreviewSearchResultsPageSize
		{
			get
			{
				return this.DataObject.DiscoveryPreviewSearchResultsPageSize;
			}
			set
			{
				this.DataObject.DiscoveryPreviewSearchResultsPageSize = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoveryMaxKeywordsPerPage
		{
			get
			{
				return this.DataObject.DiscoveryMaxKeywordsPerPage;
			}
			set
			{
				this.DataObject.DiscoveryMaxKeywordsPerPage = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoveryMaxRefinerResults
		{
			get
			{
				return this.DataObject.DiscoveryMaxRefinerResults;
			}
			set
			{
				this.DataObject.DiscoveryMaxRefinerResults = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoveryMaxSearchQueueDepth
		{
			get
			{
				return this.DataObject.DiscoveryMaxSearchQueueDepth;
			}
			set
			{
				this.DataObject.DiscoveryMaxSearchQueueDepth = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoverySearchTimeoutPeriod
		{
			get
			{
				return this.DataObject.DiscoverySearchTimeoutPeriod;
			}
			set
			{
				this.DataObject.DiscoverySearchTimeoutPeriod = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? ComplianceMaxExpansionDGRecipients
		{
			get
			{
				return this.DataObject.ComplianceMaxExpansionDGRecipients;
			}
			set
			{
				this.DataObject.ComplianceMaxExpansionDGRecipients = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? ComplianceMaxExpansionNestedDGs
		{
			get
			{
				return this.DataObject.ComplianceMaxExpansionNestedDGs;
			}
			set
			{
				this.DataObject.ComplianceMaxExpansionNestedDGs = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PushNotificationMaxConcurrency
		{
			get
			{
				return this.DataObject.PushNotificationMaxConcurrency;
			}
			set
			{
				this.DataObject.PushNotificationMaxConcurrency = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PushNotificationMaxBurst
		{
			get
			{
				return this.DataObject.PushNotificationMaxBurst;
			}
			set
			{
				this.DataObject.PushNotificationMaxBurst = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PushNotificationRechargeRate
		{
			get
			{
				return this.DataObject.PushNotificationRechargeRate;
			}
			set
			{
				this.DataObject.PushNotificationRechargeRate = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PushNotificationCutoffBalance
		{
			get
			{
				return this.DataObject.PushNotificationCutoffBalance;
			}
			set
			{
				this.DataObject.PushNotificationCutoffBalance = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PushNotificationMaxBurstPerDevice
		{
			get
			{
				return this.DataObject.PushNotificationMaxBurstPerDevice;
			}
			set
			{
				this.DataObject.PushNotificationMaxBurstPerDevice = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PushNotificationRechargeRatePerDevice
		{
			get
			{
				return this.DataObject.PushNotificationRechargeRatePerDevice;
			}
			set
			{
				this.DataObject.PushNotificationRechargeRatePerDevice = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PushNotificationSamplingPeriodPerDevice
		{
			get
			{
				return this.DataObject.PushNotificationSamplingPeriodPerDevice;
			}
			set
			{
				this.DataObject.PushNotificationSamplingPeriodPerDevice = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsServiceAccount
		{
			get
			{
				return this.DataObject.IsServiceAccount;
			}
			set
			{
				this.DataObject.IsServiceAccount = value.ToBool();
			}
		}

		[Parameter(Mandatory = false)]
		public ThrottlingPolicyScopeType ThrottlingPolicyScope
		{
			get
			{
				return this.DataObject.ThrottlingPolicyScope;
			}
			set
			{
				base.VerifyValues<ThrottlingPolicyScopeType>(NewThrottlingPolicy.AllowedThrottlingPolicyScopeTypes, value);
				this.DataObject.ThrottlingPolicyScope = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ThrottlingPolicy throttlingPolicy = (ThrottlingPolicy)base.PrepareDataObject();
			IConfigurationSession session = base.DataSession as IConfigurationSession;
			throttlingPolicy.SetId(session, new ADObjectId("CN=Global Settings"), base.Name);
			if (this.IsServiceAccount)
			{
				this.StampServiceAccountValue(throttlingPolicy, ThrottlingPolicySchema.EwsMaxConcurrency, ThrottlingPolicyDefaults.ServiceAccountEwsMaxConcurrency);
				this.StampServiceAccountValue(throttlingPolicy, ThrottlingPolicySchema.EwsMaxBurst, Unlimited<uint>.UnlimitedValue);
				this.StampServiceAccountValue(throttlingPolicy, ThrottlingPolicySchema.EwsRechargeRate, Unlimited<uint>.UnlimitedValue);
				this.StampServiceAccountValue(throttlingPolicy, ThrottlingPolicySchema.EwsCutoffBalance, Unlimited<uint>.UnlimitedValue);
				this.StampServiceAccountValue(throttlingPolicy, ThrottlingPolicySchema.ImapMaxConcurrency, ThrottlingPolicyDefaults.ServiceAccountImapMaxConcurrency);
				this.StampServiceAccountValue(throttlingPolicy, ThrottlingPolicySchema.ImapMaxBurst, Unlimited<uint>.UnlimitedValue);
				this.StampServiceAccountValue(throttlingPolicy, ThrottlingPolicySchema.ImapRechargeRate, Unlimited<uint>.UnlimitedValue);
				this.StampServiceAccountValue(throttlingPolicy, ThrottlingPolicySchema.ImapCutoffBalance, Unlimited<uint>.UnlimitedValue);
				this.StampServiceAccountValue(throttlingPolicy, ThrottlingPolicySchema.OutlookServiceMaxConcurrency, ThrottlingPolicyDefaults.ServiceAccountOutlookServiceMaxConcurrency);
				this.StampServiceAccountValue(throttlingPolicy, ThrottlingPolicySchema.OutlookServiceMaxBurst, Unlimited<uint>.UnlimitedValue);
				this.StampServiceAccountValue(throttlingPolicy, ThrottlingPolicySchema.OutlookServiceRechargeRate, Unlimited<uint>.UnlimitedValue);
				this.StampServiceAccountValue(throttlingPolicy, ThrottlingPolicySchema.OutlookServiceCutoffBalance, Unlimited<uint>.UnlimitedValue);
				this.StampServiceAccountValue(throttlingPolicy, ThrottlingPolicySchema.RcaMaxConcurrency, ThrottlingPolicyDefaults.ServiceAccountRcaMaxConcurrency);
				this.StampServiceAccountValue(throttlingPolicy, ThrottlingPolicySchema.RcaMaxBurst, Unlimited<uint>.UnlimitedValue);
				this.StampServiceAccountValue(throttlingPolicy, ThrottlingPolicySchema.RcaRechargeRate, Unlimited<uint>.UnlimitedValue);
				this.StampServiceAccountValue(throttlingPolicy, ThrottlingPolicySchema.RcaCutoffBalance, Unlimited<uint>.UnlimitedValue);
			}
			TaskLogger.LogExit();
			return throttlingPolicy;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			base.InternalValidate();
			if (this.ExchangeMaxCmdlets != null || this.PowerShellMaxCmdlets != null || this.PowerShellMaxOperations != null)
			{
				IThrottlingPolicy effectiveThrottlingPolicy = this.DataObject.GetEffectiveThrottlingPolicy(false);
				Unlimited<uint> powershellMaxOperations = SetThrottlingPolicy.ConvertToUnlimitedNull((this.PowerShellMaxOperations == null) ? new Unlimited<uint>?(effectiveThrottlingPolicy.PowerShellMaxOperations) : this.PowerShellMaxOperations);
				Unlimited<uint> powershellMaxCmdlets = SetThrottlingPolicy.ConvertToUnlimitedNull((this.PowerShellMaxCmdlets == null) ? new Unlimited<uint>?(effectiveThrottlingPolicy.PowerShellMaxCmdlets) : this.PowerShellMaxCmdlets);
				Unlimited<uint> exchangeMaxCmdlets = SetThrottlingPolicy.ConvertToUnlimitedNull((this.ExchangeMaxCmdlets == null) ? new Unlimited<uint>?(effectiveThrottlingPolicy.ExchangeMaxCmdlets) : this.ExchangeMaxCmdlets);
				bool flag;
				bool flag2;
				SetThrottlingPolicy.VerifyMaxCmdlets(powershellMaxOperations, powershellMaxCmdlets, exchangeMaxCmdlets, out flag, out flag2);
				if (flag2)
				{
					base.WriteError(new LocalizedException(Strings.ErrorMaxCmdletsNotSupported(powershellMaxOperations.ToString(), powershellMaxCmdlets.ToString(), exchangeMaxCmdlets.ToString())), (ErrorCategory)1000, null);
				}
				else if (flag)
				{
					this.WriteWarning(Strings.WarningMaxCmdletsRatioNotSupported(powershellMaxCmdlets.ToString(), exchangeMaxCmdlets.ToString()));
				}
			}
			if (this.DataObject.ThrottlingPolicyScope == ThrottlingPolicyScopeType.Organization)
			{
				ThrottlingPolicy[] array = this.ConfigurationSession.FindOrganizationThrottlingPolicies(this.DataObject.OrganizationId);
				if (array != null && array.Length > 0)
				{
					base.WriteError(new LocalizedException(Strings.ErrorOrganizationThrottlingPolicyAlreadyExists(base.OrganizationId.ToString())), (ErrorCategory)1000, null);
				}
			}
			if (this.DataObject.ThrottlingPolicyScope != ThrottlingPolicyScopeType.Organization)
			{
				if (base.Fields.Contains(ThrottlingPolicySchema.PowerShellMaxTenantConcurrency))
				{
					base.WriteError(new LocalizedException(Strings.ErrorCannotSetPowerShellMaxTenantConcurrency), (ErrorCategory)1000, null);
				}
				if (base.Fields.Contains(ThrottlingPolicySchema.PowerShellMaxTenantRunspaces))
				{
					base.WriteError(new LocalizedException(Strings.ErrorCannotSetPowerShellMaxTenantRunspaces), (ErrorCategory)1000, null);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			ThrottlingPolicy throttlingPolicy = (ThrottlingPolicy)dataObject;
			try
			{
				throttlingPolicy.ConvertToEffectiveThrottlingPolicy(false);
			}
			catch (GlobalThrottlingPolicyNotFoundException)
			{
				base.WriteError(new ManagementObjectNotFoundException(DirectoryStrings.GlobalThrottlingPolicyNotFoundException), ErrorCategory.ObjectNotFound, null);
			}
			catch (GlobalThrottlingPolicyAmbiguousException)
			{
				base.WriteError(new ManagementObjectAmbiguousException(DirectoryStrings.GlobalThrottlingPolicyAmbiguousException), ErrorCategory.InvalidResult, null);
			}
			throttlingPolicy.Diagnostics = null;
			base.WriteResult(throttlingPolicy);
		}

		private void StampServiceAccountValue(ThrottlingPolicy dataObject, ADPropertyDefinition key, Unlimited<uint> serviceAccountDefaultValue)
		{
			if (!base.Fields.Contains(key))
			{
				dataObject[key] = serviceAccountDefaultValue;
			}
		}

		internal static readonly ThrottlingPolicyScopeType[] AllowedThrottlingPolicyScopeTypes = new ThrottlingPolicyScopeType[]
		{
			ThrottlingPolicyScopeType.Regular,
			ThrottlingPolicyScopeType.Organization
		};
	}
}
