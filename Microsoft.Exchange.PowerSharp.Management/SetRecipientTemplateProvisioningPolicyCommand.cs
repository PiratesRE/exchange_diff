using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Provisioning;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetRecipientTemplateProvisioningPolicyCommand : SyntheticCommandWithPipelineInputNoOutput<RecipientTemplateProvisioningPolicy>
	{
		private SetRecipientTemplateProvisioningPolicyCommand() : base("Set-RecipientTemplateProvisioningPolicy")
		{
		}

		public SetRecipientTemplateProvisioningPolicyCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetRecipientTemplateProvisioningPolicyCommand SetParameters(SetRecipientTemplateProvisioningPolicyCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetRecipientTemplateProvisioningPolicyCommand SetParameters(SetRecipientTemplateProvisioningPolicyCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string DefaultDistributionListOU
			{
				set
				{
					base.PowerSharpParameters["DefaultDistributionListOU"] = ((value != null) ? new OrganizationalUnitIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultMaxSendSize
			{
				set
				{
					base.PowerSharpParameters["DefaultMaxSendSize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultMaxReceiveSize
			{
				set
				{
					base.PowerSharpParameters["DefaultMaxReceiveSize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultProhibitSendQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultProhibitSendQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultProhibitSendReceiveQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultProhibitSendReceiveQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultIssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultIssueWarningQuota"] = value;
				}
			}

			public virtual ByteQuantifiedSize? DefaultRulesQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultRulesQuota"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ProvisioningPolicyIdParameter(value) : null);
				}
			}

			public virtual string DefaultDistributionListOU
			{
				set
				{
					base.PowerSharpParameters["DefaultDistributionListOU"] = ((value != null) ? new OrganizationalUnitIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultMaxSendSize
			{
				set
				{
					base.PowerSharpParameters["DefaultMaxSendSize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultMaxReceiveSize
			{
				set
				{
					base.PowerSharpParameters["DefaultMaxReceiveSize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultProhibitSendQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultProhibitSendQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultProhibitSendReceiveQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultProhibitSendReceiveQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultIssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultIssueWarningQuota"] = value;
				}
			}

			public virtual ByteQuantifiedSize? DefaultRulesQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultRulesQuota"] = value;
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
