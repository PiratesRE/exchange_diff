using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetHostedOutboundSpamFilterPolicyCommand : SyntheticCommandWithPipelineInputNoOutput<HostedOutboundSpamFilterPolicy>
	{
		private SetHostedOutboundSpamFilterPolicyCommand() : base("Set-HostedOutboundSpamFilterPolicy")
		{
		}

		public SetHostedOutboundSpamFilterPolicyCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetHostedOutboundSpamFilterPolicyCommand SetParameters(SetHostedOutboundSpamFilterPolicyCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetHostedOutboundSpamFilterPolicyCommand SetParameters(SetHostedOutboundSpamFilterPolicyCommand.IdentityParameters parameters)
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

			public virtual string AdminDisplayName
			{
				set
				{
					base.PowerSharpParameters["AdminDisplayName"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> NotifyOutboundSpamRecipients
			{
				set
				{
					base.PowerSharpParameters["NotifyOutboundSpamRecipients"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> BccSuspiciousOutboundAdditionalRecipients
			{
				set
				{
					base.PowerSharpParameters["BccSuspiciousOutboundAdditionalRecipients"] = value;
				}
			}

			public virtual bool BccSuspiciousOutboundMail
			{
				set
				{
					base.PowerSharpParameters["BccSuspiciousOutboundMail"] = value;
				}
			}

			public virtual bool NotifyOutboundSpam
			{
				set
				{
					base.PowerSharpParameters["NotifyOutboundSpam"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new HostedOutboundSpamFilterPolicyIdParameter(value) : null);
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

			public virtual string AdminDisplayName
			{
				set
				{
					base.PowerSharpParameters["AdminDisplayName"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> NotifyOutboundSpamRecipients
			{
				set
				{
					base.PowerSharpParameters["NotifyOutboundSpamRecipients"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> BccSuspiciousOutboundAdditionalRecipients
			{
				set
				{
					base.PowerSharpParameters["BccSuspiciousOutboundAdditionalRecipients"] = value;
				}
			}

			public virtual bool BccSuspiciousOutboundMail
			{
				set
				{
					base.PowerSharpParameters["BccSuspiciousOutboundMail"] = value;
				}
			}

			public virtual bool NotifyOutboundSpam
			{
				set
				{
					base.PowerSharpParameters["NotifyOutboundSpam"] = value;
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
