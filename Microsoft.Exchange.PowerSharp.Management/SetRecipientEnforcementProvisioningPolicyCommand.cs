using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Provisioning;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetRecipientEnforcementProvisioningPolicyCommand : SyntheticCommandWithPipelineInputNoOutput<RecipientEnforcementProvisioningPolicy>
	{
		private SetRecipientEnforcementProvisioningPolicyCommand() : base("Set-RecipientEnforcementProvisioningPolicy")
		{
		}

		public SetRecipientEnforcementProvisioningPolicyCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetRecipientEnforcementProvisioningPolicyCommand SetParameters(SetRecipientEnforcementProvisioningPolicyCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetRecipientEnforcementProvisioningPolicyCommand SetParameters(SetRecipientEnforcementProvisioningPolicyCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter IgnoreDehydratedFlag
			{
				set
				{
					base.PowerSharpParameters["IgnoreDehydratedFlag"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual Unlimited<int> DistributionListCountQuota
			{
				set
				{
					base.PowerSharpParameters["DistributionListCountQuota"] = value;
				}
			}

			public virtual Unlimited<int> MailboxCountQuota
			{
				set
				{
					base.PowerSharpParameters["MailboxCountQuota"] = value;
				}
			}

			public virtual Unlimited<int> MailUserCountQuota
			{
				set
				{
					base.PowerSharpParameters["MailUserCountQuota"] = value;
				}
			}

			public virtual Unlimited<int> ContactCountQuota
			{
				set
				{
					base.PowerSharpParameters["ContactCountQuota"] = value;
				}
			}

			public virtual Unlimited<int> TeamMailboxCountQuota
			{
				set
				{
					base.PowerSharpParameters["TeamMailboxCountQuota"] = value;
				}
			}

			public virtual Unlimited<int> PublicFolderMailboxCountQuota
			{
				set
				{
					base.PowerSharpParameters["PublicFolderMailboxCountQuota"] = value;
				}
			}

			public virtual Unlimited<int> MailPublicFolderCountQuota
			{
				set
				{
					base.PowerSharpParameters["MailPublicFolderCountQuota"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new RecipientEnforcementProvisioningPolicyIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter IgnoreDehydratedFlag
			{
				set
				{
					base.PowerSharpParameters["IgnoreDehydratedFlag"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual Unlimited<int> DistributionListCountQuota
			{
				set
				{
					base.PowerSharpParameters["DistributionListCountQuota"] = value;
				}
			}

			public virtual Unlimited<int> MailboxCountQuota
			{
				set
				{
					base.PowerSharpParameters["MailboxCountQuota"] = value;
				}
			}

			public virtual Unlimited<int> MailUserCountQuota
			{
				set
				{
					base.PowerSharpParameters["MailUserCountQuota"] = value;
				}
			}

			public virtual Unlimited<int> ContactCountQuota
			{
				set
				{
					base.PowerSharpParameters["ContactCountQuota"] = value;
				}
			}

			public virtual Unlimited<int> TeamMailboxCountQuota
			{
				set
				{
					base.PowerSharpParameters["TeamMailboxCountQuota"] = value;
				}
			}

			public virtual Unlimited<int> PublicFolderMailboxCountQuota
			{
				set
				{
					base.PowerSharpParameters["PublicFolderMailboxCountQuota"] = value;
				}
			}

			public virtual Unlimited<int> MailPublicFolderCountQuota
			{
				set
				{
					base.PowerSharpParameters["MailPublicFolderCountQuota"] = value;
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
