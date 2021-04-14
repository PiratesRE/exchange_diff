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
	[Cmdlet("Set", "ThrottlingPolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetThrottlingPolicy : SetSystemConfigurationObjectTask<ThrottlingPolicyIdParameter, ThrottlingPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetThrottlingPolicy(this.Identity.ToString(), this.DataObject.ThrottlingPolicyScope.ToString());
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				if (!this.IgnoreDehydratedFlag)
				{
					return SharedTenantConfigurationMode.Static;
				}
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return base.InternalForce;
			}
			set
			{
				base.InternalForce = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IgnoreDehydratedFlag { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter ForceSettingGlobal
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForceSettingGlobal"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ForceSettingGlobal"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? AnonymousMaxConcurrency
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.AnonymousMaxConcurrency);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.AnonymousMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? AnonymousMaxBurst
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.AnonymousMaxBurst);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.AnonymousMaxBurst] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? AnonymousRechargeRate
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.AnonymousRechargeRate);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.AnonymousRechargeRate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? AnonymousCutoffBalance
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.AnonymousCutoffBalance);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.AnonymousCutoffBalance] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EasMaxConcurrency
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EasMaxConcurrency);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EasMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EasMaxBurst
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EasMaxBurst);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EasMaxBurst] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EasRechargeRate
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EasRechargeRate);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EasRechargeRate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EasCutoffBalance
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EasCutoffBalance);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EasCutoffBalance] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EasMaxDevices
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EasMaxDevices);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EasMaxDevices] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EasMaxDeviceDeletesPerMonth
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EasMaxDeviceDeletesPerMonth);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EasMaxDeviceDeletesPerMonth] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EasMaxInactivityForDeviceCleanup
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EasMaxInactivityForDeviceCleanup);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EasMaxInactivityForDeviceCleanup] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EwsMaxConcurrency
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EwsMaxConcurrency);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EwsMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EwsMaxBurst
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EwsMaxBurst);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EwsMaxBurst] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EwsRechargeRate
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EwsRechargeRate);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EwsRechargeRate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EwsCutoffBalance
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EwsCutoffBalance);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EwsCutoffBalance] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EwsMaxSubscriptions
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EwsMaxSubscriptions);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EwsMaxSubscriptions] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? ImapMaxConcurrency
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.ImapMaxConcurrency);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.ImapMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? ImapMaxBurst
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.ImapMaxBurst);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.ImapMaxBurst] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? ImapRechargeRate
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.ImapRechargeRate);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.ImapRechargeRate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? ImapCutoffBalance
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.ImapCutoffBalance);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.ImapCutoffBalance] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OutlookServiceMaxConcurrency
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.OutlookServiceMaxConcurrency);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.OutlookServiceMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OutlookServiceMaxBurst
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.OutlookServiceMaxBurst);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.OutlookServiceMaxBurst] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OutlookServiceRechargeRate
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.OutlookServiceRechargeRate);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.OutlookServiceRechargeRate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OutlookServiceCutoffBalance
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.OutlookServiceCutoffBalance);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.OutlookServiceCutoffBalance] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OutlookServiceMaxSubscriptions
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.OutlookServiceMaxSubscriptions);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.OutlookServiceMaxSubscriptions] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OutlookServiceMaxSocketConnectionsPerDevice
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.OutlookServiceMaxSocketConnectionsPerDevice);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.OutlookServiceMaxSocketConnectionsPerDevice] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OutlookServiceMaxSocketConnectionsPerUser
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.OutlookServiceMaxSocketConnectionsPerUser);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.OutlookServiceMaxSocketConnectionsPerUser] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OwaMaxConcurrency
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.OwaMaxConcurrency);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.OwaMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OwaMaxBurst
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.OwaMaxBurst);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.OwaMaxBurst] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OwaRechargeRate
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.OwaRechargeRate);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.OwaRechargeRate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OwaCutoffBalance
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.OwaCutoffBalance);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.OwaCutoffBalance] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OwaVoiceMaxConcurrency
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.OwaVoiceMaxConcurrency);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.OwaVoiceMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OwaVoiceMaxBurst
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.OwaVoiceMaxBurst);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.OwaVoiceMaxBurst] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OwaVoiceRechargeRate
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.OwaVoiceRechargeRate);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.OwaVoiceRechargeRate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? OwaVoiceCutoffBalance
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.OwaVoiceCutoffBalance);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.OwaVoiceCutoffBalance] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EncryptionSenderMaxConcurrency
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EncryptionSenderMaxConcurrency);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EncryptionSenderMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EncryptionSenderMaxBurst
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EncryptionSenderMaxBurst);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EncryptionSenderMaxBurst] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EncryptionSenderRechargeRate
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EncryptionSenderRechargeRate);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EncryptionSenderRechargeRate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EncryptionSenderCutoffBalance
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EncryptionSenderCutoffBalance);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EncryptionSenderCutoffBalance] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EncryptionRecipientMaxConcurrency
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EncryptionRecipientMaxConcurrency);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EncryptionRecipientMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EncryptionRecipientMaxBurst
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EncryptionRecipientMaxBurst);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EncryptionRecipientMaxBurst] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EncryptionRecipientRechargeRate
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EncryptionRecipientRechargeRate);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EncryptionRecipientRechargeRate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? EncryptionRecipientCutoffBalance
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.EncryptionRecipientCutoffBalance);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.EncryptionRecipientCutoffBalance] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PopMaxConcurrency
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PopMaxConcurrency);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PopMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PopMaxBurst
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PopMaxBurst);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PopMaxBurst] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PopRechargeRate
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PopRechargeRate);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PopRechargeRate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PopCutoffBalance
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PopCutoffBalance);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PopCutoffBalance] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxConcurrency
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PowerShellMaxConcurrency);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PowerShellMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxBurst
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PowerShellMaxBurst);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PowerShellMaxBurst] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellRechargeRate
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PowerShellRechargeRate);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PowerShellRechargeRate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellCutoffBalance
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PowerShellCutoffBalance);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PowerShellCutoffBalance] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxTenantConcurrency
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PowerShellMaxTenantConcurrency);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PowerShellMaxTenantConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxOperations
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PowerShellMaxOperations);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PowerShellMaxOperations] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxCmdletsTimePeriod
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PowerShellMaxCmdletsTimePeriod);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PowerShellMaxCmdletsTimePeriod] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? ExchangeMaxCmdlets
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.ExchangeMaxCmdlets);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.ExchangeMaxCmdlets] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxCmdletQueueDepth
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PowerShellMaxCmdletQueueDepth);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PowerShellMaxCmdletQueueDepth] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxDestructiveCmdlets
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PowerShellMaxDestructiveCmdlets);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PowerShellMaxDestructiveCmdlets] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxDestructiveCmdletsTimePeriod
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PowerShellMaxDestructiveCmdletsTimePeriod);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PowerShellMaxDestructiveCmdletsTimePeriod] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxCmdlets
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PowerShellMaxCmdlets);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PowerShellMaxCmdlets] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxRunspaces
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PowerShellMaxRunspaces);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PowerShellMaxRunspaces] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxTenantRunspaces
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PowerShellMaxTenantRunspaces);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PowerShellMaxTenantRunspaces] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PowerShellMaxRunspacesTimePeriod
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PowerShellMaxRunspacesTimePeriod);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PowerShellMaxRunspacesTimePeriod] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PswsMaxConcurrency
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PswsMaxConcurrency);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PswsMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PswsMaxRequest
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PswsMaxRequest);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PswsMaxRequest] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PswsMaxRequestTimePeriod
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PswsMaxRequestTimePeriod);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PswsMaxRequestTimePeriod] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? RcaMaxConcurrency
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.RcaMaxConcurrency);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.RcaMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? RcaMaxBurst
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.RcaMaxBurst);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.RcaMaxBurst] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? RcaRechargeRate
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.RcaRechargeRate);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.RcaRechargeRate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? RcaCutoffBalance
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.RcaCutoffBalance);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.RcaCutoffBalance] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? CpaMaxConcurrency
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.CpaMaxConcurrency);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.CpaMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? CpaMaxBurst
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.CpaMaxBurst);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.CpaMaxBurst] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? CpaRechargeRate
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.CpaRechargeRate);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.CpaRechargeRate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? CpaCutoffBalance
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.CpaCutoffBalance);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.CpaCutoffBalance] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? MessageRateLimit
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.MessageRateLimit);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.MessageRateLimit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? RecipientRateLimit
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.RecipientRateLimit);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.RecipientRateLimit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? ForwardeeLimit
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.ForwardeeLimit);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.ForwardeeLimit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoveryMaxConcurrency
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.DiscoveryMaxConcurrency);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.DiscoveryMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoveryMaxMailboxes
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.DiscoveryMaxMailboxes);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.DiscoveryMaxMailboxes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoveryMaxKeywords
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.DiscoveryMaxKeywords);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.DiscoveryMaxKeywords] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoveryMaxPreviewSearchMailboxes
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.DiscoveryMaxPreviewSearchMailboxes);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.DiscoveryMaxPreviewSearchMailboxes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoveryMaxStatsSearchMailboxes
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.DiscoveryMaxStatsSearchMailboxes);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.DiscoveryMaxStatsSearchMailboxes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoveryPreviewSearchResultsPageSize
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.DiscoveryPreviewSearchResultsPageSize);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.DiscoveryPreviewSearchResultsPageSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoveryMaxKeywordsPerPage
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.DiscoveryMaxKeywordsPerPage);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.DiscoveryMaxKeywordsPerPage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoveryMaxRefinerResults
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.DiscoveryMaxRefinerResults);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.DiscoveryMaxRefinerResults] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoveryMaxSearchQueueDepth
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.DiscoveryMaxSearchQueueDepth);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.DiscoveryMaxSearchQueueDepth] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? DiscoverySearchTimeoutPeriod
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.DiscoverySearchTimeoutPeriod);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.DiscoverySearchTimeoutPeriod] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? ComplianceMaxExpansionDGRecipients
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.ComplianceMaxExpansionDGRecipients);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.ComplianceMaxExpansionDGRecipients] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? ComplianceMaxExpansionNestedDGs
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.ComplianceMaxExpansionNestedDGs);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.ComplianceMaxExpansionNestedDGs] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PushNotificationMaxConcurrency
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PushNotificationMaxConcurrency);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PushNotificationMaxConcurrency] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PushNotificationMaxBurst
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PushNotificationMaxBurst);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PushNotificationMaxBurst] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PushNotificationRechargeRate
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PushNotificationRechargeRate);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PushNotificationRechargeRate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PushNotificationCutoffBalance
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PushNotificationCutoffBalance);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PushNotificationCutoffBalance] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PushNotificationMaxBurstPerDevice
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PushNotificationMaxBurstPerDevice);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PushNotificationMaxBurstPerDevice] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PushNotificationRechargeRatePerDevice
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PushNotificationRechargeRatePerDevice);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PushNotificationRechargeRatePerDevice] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint>? PushNotificationSamplingPeriodPerDevice
		{
			get
			{
				return this.SafeGetField(ThrottlingPolicySchema.PushNotificationSamplingPeriodPerDevice);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.PushNotificationSamplingPeriodPerDevice] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsServiceAccount
		{
			get
			{
				return (SwitchParameter)(base.Fields[ThrottlingPolicySchema.IsServiceAccount] ?? false);
			}
			set
			{
				base.Fields[ThrottlingPolicySchema.IsServiceAccount] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ThrottlingPolicyScopeType ThrottlingPolicyScope
		{
			get
			{
				return (ThrottlingPolicyScopeType)base.Fields[ThrottlingPolicySchema.ThrottlingPolicyScope];
			}
			set
			{
				base.VerifyValues<ThrottlingPolicyScopeType>(SetThrottlingPolicy.AllowedThrottlingPolicyScopeTypes, value);
				base.Fields[ThrottlingPolicySchema.ThrottlingPolicyScope] = value;
			}
		}

		internal static Unlimited<uint> ConvertToUnlimitedNull(Unlimited<uint>? value)
		{
			if (value != null)
			{
				return value.Value;
			}
			return Unlimited<uint>.UnlimitedValue;
		}

		internal static void VerifyMaxCmdlets(Unlimited<uint> powershellMaxOperations, Unlimited<uint> powershellMaxCmdlets, Unlimited<uint> exchangeMaxCmdlets, out bool haveWarning, out bool haveError)
		{
			haveWarning = false;
			if (powershellMaxCmdlets < exchangeMaxCmdlets || powershellMaxOperations < powershellMaxCmdlets)
			{
				haveError = true;
				return;
			}
			if (!powershellMaxCmdlets.IsUnlimited)
			{
				uint value = powershellMaxCmdlets.Value;
				uint value2 = exchangeMaxCmdlets.Value;
				haveWarning = (value < value2 * 1.2);
			}
			haveError = false;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.DataObject.ThrottlingPolicyScope == ThrottlingPolicyScopeType.Global)
			{
				if (!this.ForceSettingGlobal)
				{
					base.WriteError(new LocalizedException(Strings.ErrorCannotSetGlobalThrottlingPolicyWithoutForceSettingGlobalParameter), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
				this.WriteWarning(Strings.ChangingGlobalPolicy(this.DataObject.Name));
			}
			if (base.Fields.IsModified(ThrottlingPolicySchema.ThrottlingPolicyScope) && this.ThrottlingPolicyScope == ThrottlingPolicyScopeType.Organization)
			{
				this.ConfigurationSession.SessionSettings.IsSharedConfigChecked = true;
				ThrottlingPolicy[] array = this.ConfigurationSession.FindOrganizationThrottlingPolicies(this.DataObject.OrganizationId);
				if (array != null && array.Length > 0 && !array[0].Identity.Equals(this.DataObject.Identity))
				{
					base.WriteError(new LocalizedException(Strings.ErrorOrganizationThrottlingPolicyAlreadyExists(this.DataObject.OrganizationId.ToString())), (ErrorCategory)1000, null);
				}
			}
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
					return;
				}
				if (flag)
				{
					this.WriteWarning(Strings.WarningMaxCmdletsRatioNotSupported(powershellMaxCmdlets.ToString(), exchangeMaxCmdlets.ToString()));
				}
			}
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			base.StampChangesOn(dataObject);
			ThrottlingPolicy throttlingPolicy = dataObject as ThrottlingPolicy;
			if (base.Fields.Contains(ThrottlingPolicySchema.IsServiceAccount))
			{
				throttlingPolicy.IsServiceAccount = this.IsServiceAccount;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.ThrottlingPolicyScope))
			{
				throttlingPolicy.ThrottlingPolicyScope = this.ThrottlingPolicyScope;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.AnonymousMaxConcurrency))
			{
				throttlingPolicy.AnonymousMaxConcurrency = this.AnonymousMaxConcurrency;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.AnonymousMaxBurst))
			{
				throttlingPolicy.AnonymousMaxBurst = this.AnonymousMaxBurst;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.AnonymousRechargeRate))
			{
				throttlingPolicy.AnonymousRechargeRate = this.AnonymousRechargeRate;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.AnonymousCutoffBalance))
			{
				throttlingPolicy.AnonymousCutoffBalance = this.AnonymousCutoffBalance;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EasMaxConcurrency))
			{
				throttlingPolicy.EasMaxConcurrency = this.EasMaxConcurrency;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EasMaxBurst))
			{
				throttlingPolicy.EasMaxBurst = this.EasMaxBurst;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EasRechargeRate))
			{
				throttlingPolicy.EasRechargeRate = this.EasRechargeRate;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EasCutoffBalance))
			{
				throttlingPolicy.EasCutoffBalance = this.EasCutoffBalance;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EasMaxDevices))
			{
				throttlingPolicy.EasMaxDevices = this.EasMaxDevices;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EasMaxDeviceDeletesPerMonth))
			{
				throttlingPolicy.EasMaxDeviceDeletesPerMonth = this.EasMaxDeviceDeletesPerMonth;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EasMaxInactivityForDeviceCleanup))
			{
				throttlingPolicy.EasMaxInactivityForDeviceCleanup = this.EasMaxInactivityForDeviceCleanup;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EwsMaxConcurrency))
			{
				throttlingPolicy.EwsMaxConcurrency = this.EwsMaxConcurrency;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EwsMaxBurst))
			{
				throttlingPolicy.EwsMaxBurst = this.EwsMaxBurst;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EwsRechargeRate))
			{
				throttlingPolicy.EwsRechargeRate = this.EwsRechargeRate;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EwsCutoffBalance))
			{
				throttlingPolicy.EwsCutoffBalance = this.EwsCutoffBalance;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EwsMaxSubscriptions))
			{
				throttlingPolicy.EwsMaxSubscriptions = this.EwsMaxSubscriptions;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.ImapMaxConcurrency))
			{
				throttlingPolicy.ImapMaxConcurrency = this.ImapMaxConcurrency;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.ImapMaxBurst))
			{
				throttlingPolicy.ImapMaxBurst = this.ImapMaxBurst;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.ImapRechargeRate))
			{
				throttlingPolicy.ImapRechargeRate = this.ImapRechargeRate;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.ImapCutoffBalance))
			{
				throttlingPolicy.ImapCutoffBalance = this.ImapCutoffBalance;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.OutlookServiceMaxConcurrency))
			{
				throttlingPolicy.OutlookServiceMaxConcurrency = this.OutlookServiceMaxConcurrency;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.OutlookServiceMaxBurst))
			{
				throttlingPolicy.OutlookServiceMaxBurst = this.OutlookServiceMaxBurst;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.OutlookServiceRechargeRate))
			{
				throttlingPolicy.OutlookServiceRechargeRate = this.OutlookServiceRechargeRate;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.OutlookServiceCutoffBalance))
			{
				throttlingPolicy.OutlookServiceCutoffBalance = this.OutlookServiceCutoffBalance;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.OutlookServiceMaxSubscriptions))
			{
				throttlingPolicy.OutlookServiceMaxSubscriptions = this.OutlookServiceMaxSubscriptions;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.OutlookServiceMaxSocketConnectionsPerDevice))
			{
				throttlingPolicy.OutlookServiceMaxSocketConnectionsPerDevice = this.OutlookServiceMaxSocketConnectionsPerDevice;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.OutlookServiceMaxSocketConnectionsPerUser))
			{
				throttlingPolicy.OutlookServiceMaxSocketConnectionsPerUser = this.OutlookServiceMaxSocketConnectionsPerUser;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.OwaMaxConcurrency))
			{
				throttlingPolicy.OwaMaxConcurrency = this.OwaMaxConcurrency;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.OwaMaxBurst))
			{
				throttlingPolicy.OwaMaxBurst = this.OwaMaxBurst;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.OwaRechargeRate))
			{
				throttlingPolicy.OwaRechargeRate = this.OwaRechargeRate;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.OwaCutoffBalance))
			{
				throttlingPolicy.OwaCutoffBalance = this.OwaCutoffBalance;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.OwaVoiceMaxConcurrency))
			{
				throttlingPolicy.OwaVoiceMaxConcurrency = this.OwaVoiceMaxConcurrency;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.OwaVoiceMaxBurst))
			{
				throttlingPolicy.OwaVoiceMaxBurst = this.OwaVoiceMaxBurst;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.OwaVoiceRechargeRate))
			{
				throttlingPolicy.OwaVoiceRechargeRate = this.OwaVoiceRechargeRate;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.OwaVoiceCutoffBalance))
			{
				throttlingPolicy.OwaVoiceCutoffBalance = this.OwaVoiceCutoffBalance;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EncryptionSenderMaxConcurrency))
			{
				throttlingPolicy.EncryptionSenderMaxConcurrency = this.EncryptionSenderMaxConcurrency;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EncryptionSenderMaxBurst))
			{
				throttlingPolicy.EncryptionSenderMaxBurst = this.EncryptionSenderMaxBurst;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EncryptionSenderRechargeRate))
			{
				throttlingPolicy.EncryptionSenderRechargeRate = this.EncryptionSenderRechargeRate;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EncryptionSenderCutoffBalance))
			{
				throttlingPolicy.EncryptionSenderCutoffBalance = this.EncryptionSenderCutoffBalance;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EncryptionRecipientMaxConcurrency))
			{
				throttlingPolicy.EncryptionRecipientMaxConcurrency = this.EncryptionRecipientMaxConcurrency;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EncryptionRecipientMaxBurst))
			{
				throttlingPolicy.EncryptionRecipientMaxBurst = this.EncryptionRecipientMaxBurst;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EncryptionRecipientRechargeRate))
			{
				throttlingPolicy.EncryptionRecipientRechargeRate = this.EncryptionRecipientRechargeRate;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.EncryptionRecipientCutoffBalance))
			{
				throttlingPolicy.EncryptionRecipientCutoffBalance = this.EncryptionRecipientCutoffBalance;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PopMaxConcurrency))
			{
				throttlingPolicy.PopMaxConcurrency = this.PopMaxConcurrency;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PopMaxBurst))
			{
				throttlingPolicy.PopMaxBurst = this.PopMaxBurst;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PopRechargeRate))
			{
				throttlingPolicy.PopRechargeRate = this.PopRechargeRate;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PopCutoffBalance))
			{
				throttlingPolicy.PopCutoffBalance = this.PopCutoffBalance;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PowerShellMaxConcurrency))
			{
				throttlingPolicy.PowerShellMaxConcurrency = this.PowerShellMaxConcurrency;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PowerShellMaxTenantConcurrency))
			{
				if (throttlingPolicy.ThrottlingPolicyScope != ThrottlingPolicyScopeType.Regular)
				{
					throttlingPolicy.PowerShellMaxTenantConcurrency = this.PowerShellMaxTenantConcurrency;
				}
				else
				{
					base.WriteError(new LocalizedException(Strings.ErrorCannotSetPowerShellMaxTenantConcurrency), (ErrorCategory)1000, null);
				}
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PowerShellMaxBurst))
			{
				throttlingPolicy.PowerShellMaxBurst = this.PowerShellMaxBurst;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PowerShellRechargeRate))
			{
				throttlingPolicy.PowerShellRechargeRate = this.PowerShellRechargeRate;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PowerShellCutoffBalance))
			{
				throttlingPolicy.PowerShellCutoffBalance = this.PowerShellCutoffBalance;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PowerShellMaxOperations))
			{
				throttlingPolicy.PowerShellMaxOperations = this.PowerShellMaxOperations;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PowerShellMaxCmdletsTimePeriod))
			{
				throttlingPolicy.PowerShellMaxCmdletsTimePeriod = this.PowerShellMaxCmdletsTimePeriod;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.ExchangeMaxCmdlets))
			{
				throttlingPolicy.ExchangeMaxCmdlets = this.ExchangeMaxCmdlets;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PowerShellMaxCmdletQueueDepth))
			{
				throttlingPolicy.PowerShellMaxCmdletQueueDepth = this.PowerShellMaxCmdletQueueDepth;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PowerShellMaxDestructiveCmdlets))
			{
				throttlingPolicy.PowerShellMaxDestructiveCmdlets = this.PowerShellMaxDestructiveCmdlets;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PowerShellMaxDestructiveCmdletsTimePeriod))
			{
				throttlingPolicy.PowerShellMaxDestructiveCmdletsTimePeriod = this.PowerShellMaxDestructiveCmdletsTimePeriod;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PowerShellMaxCmdlets))
			{
				throttlingPolicy.PowerShellMaxCmdlets = this.PowerShellMaxCmdlets;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PowerShellMaxRunspaces))
			{
				throttlingPolicy.PowerShellMaxRunspaces = this.PowerShellMaxRunspaces;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PowerShellMaxTenantRunspaces))
			{
				if (throttlingPolicy.ThrottlingPolicyScope != ThrottlingPolicyScopeType.Regular)
				{
					throttlingPolicy.PowerShellMaxTenantRunspaces = this.PowerShellMaxTenantRunspaces;
				}
				else
				{
					base.WriteError(new LocalizedException(Strings.ErrorCannotSetPowerShellMaxTenantRunspaces), (ErrorCategory)1000, null);
				}
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PowerShellMaxRunspacesTimePeriod))
			{
				throttlingPolicy.PowerShellMaxRunspacesTimePeriod = this.PowerShellMaxRunspacesTimePeriod;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PswsMaxConcurrency))
			{
				throttlingPolicy.PswsMaxConcurrency = this.PswsMaxConcurrency;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PswsMaxRequest))
			{
				throttlingPolicy.PswsMaxRequest = this.PswsMaxRequest;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PswsMaxRequestTimePeriod))
			{
				throttlingPolicy.PswsMaxRequestTimePeriod = this.PswsMaxRequestTimePeriod;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.RcaMaxConcurrency))
			{
				throttlingPolicy.RcaMaxConcurrency = this.RcaMaxConcurrency;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.RcaMaxBurst))
			{
				throttlingPolicy.RcaMaxBurst = this.RcaMaxBurst;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.RcaRechargeRate))
			{
				throttlingPolicy.RcaRechargeRate = this.RcaRechargeRate;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.RcaCutoffBalance))
			{
				throttlingPolicy.RcaCutoffBalance = this.RcaCutoffBalance;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.CpaMaxConcurrency))
			{
				throttlingPolicy.CpaMaxConcurrency = this.CpaMaxConcurrency;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.CpaMaxBurst))
			{
				throttlingPolicy.CpaMaxBurst = this.CpaMaxBurst;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.CpaRechargeRate))
			{
				throttlingPolicy.CpaRechargeRate = this.CpaRechargeRate;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.CpaCutoffBalance))
			{
				throttlingPolicy.CpaCutoffBalance = this.CpaCutoffBalance;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.MessageRateLimit))
			{
				throttlingPolicy.MessageRateLimit = this.MessageRateLimit;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.RecipientRateLimit))
			{
				throttlingPolicy.RecipientRateLimit = this.RecipientRateLimit;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.ForwardeeLimit))
			{
				throttlingPolicy.ForwardeeLimit = this.ForwardeeLimit;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.DiscoveryMaxConcurrency))
			{
				throttlingPolicy.DiscoveryMaxConcurrency = this.DiscoveryMaxConcurrency;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.DiscoveryMaxMailboxes))
			{
				throttlingPolicy.DiscoveryMaxMailboxes = this.DiscoveryMaxMailboxes;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.DiscoveryMaxKeywords))
			{
				throttlingPolicy.DiscoveryMaxKeywords = this.DiscoveryMaxKeywords;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.DiscoveryMaxPreviewSearchMailboxes))
			{
				throttlingPolicy.DiscoveryMaxPreviewSearchMailboxes = this.DiscoveryMaxPreviewSearchMailboxes;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.DiscoveryMaxStatsSearchMailboxes))
			{
				throttlingPolicy.DiscoveryMaxStatsSearchMailboxes = this.DiscoveryMaxStatsSearchMailboxes;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.DiscoveryPreviewSearchResultsPageSize))
			{
				throttlingPolicy.DiscoveryPreviewSearchResultsPageSize = this.DiscoveryPreviewSearchResultsPageSize;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.DiscoveryMaxKeywordsPerPage))
			{
				throttlingPolicy.DiscoveryMaxKeywordsPerPage = this.DiscoveryMaxKeywordsPerPage;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.DiscoveryMaxRefinerResults))
			{
				throttlingPolicy.DiscoveryMaxRefinerResults = this.DiscoveryMaxRefinerResults;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.DiscoveryMaxSearchQueueDepth))
			{
				throttlingPolicy.DiscoveryMaxSearchQueueDepth = this.DiscoveryMaxSearchQueueDepth;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.DiscoverySearchTimeoutPeriod))
			{
				throttlingPolicy.DiscoverySearchTimeoutPeriod = this.DiscoverySearchTimeoutPeriod;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.DiscoverySearchTimeoutPeriod))
			{
				throttlingPolicy.DiscoverySearchTimeoutPeriod = this.DiscoverySearchTimeoutPeriod;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PushNotificationMaxConcurrency))
			{
				throttlingPolicy.PushNotificationMaxConcurrency = this.PushNotificationMaxConcurrency;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PushNotificationMaxBurst))
			{
				throttlingPolicy.PushNotificationMaxBurst = this.PushNotificationMaxBurst;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PushNotificationRechargeRate))
			{
				throttlingPolicy.PushNotificationRechargeRate = this.PushNotificationRechargeRate;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PushNotificationCutoffBalance))
			{
				throttlingPolicy.PushNotificationCutoffBalance = this.PushNotificationCutoffBalance;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PushNotificationMaxBurstPerDevice))
			{
				throttlingPolicy.PushNotificationMaxBurstPerDevice = this.PushNotificationMaxBurstPerDevice;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PushNotificationRechargeRatePerDevice))
			{
				throttlingPolicy.PushNotificationRechargeRatePerDevice = this.PushNotificationRechargeRatePerDevice;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.PushNotificationSamplingPeriodPerDevice))
			{
				throttlingPolicy.PushNotificationSamplingPeriodPerDevice = this.PushNotificationSamplingPeriodPerDevice;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.ComplianceMaxExpansionDGRecipients))
			{
				throttlingPolicy.ComplianceMaxExpansionDGRecipients = this.ComplianceMaxExpansionDGRecipients;
			}
			if (base.Fields.Contains(ThrottlingPolicySchema.ComplianceMaxExpansionNestedDGs))
			{
				throttlingPolicy.ComplianceMaxExpansionNestedDGs = this.ComplianceMaxExpansionNestedDGs;
			}
		}

		private Unlimited<uint>? SafeGetField(ADPropertyDefinition key)
		{
			if (!base.Fields.Contains(key))
			{
				return null;
			}
			return (Unlimited<uint>?)base.Fields[key];
		}

		internal static readonly ThrottlingPolicyScopeType[] AllowedThrottlingPolicyScopeTypes = new ThrottlingPolicyScopeType[]
		{
			ThrottlingPolicyScopeType.Regular,
			ThrottlingPolicyScopeType.Organization
		};
	}
}
