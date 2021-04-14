using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewAcceptedDomainCommand : SyntheticCommandWithPipelineInput<AcceptedDomain, AcceptedDomain>
	{
		private NewAcceptedDomainCommand() : base("New-AcceptedDomain")
		{
		}

		public NewAcceptedDomainCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewAcceptedDomainCommand SetParameters(NewAcceptedDomainCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SmtpDomainWithSubdomains DomainName
			{
				set
				{
					base.PowerSharpParameters["DomainName"] = value;
				}
			}

			public virtual AcceptedDomainType DomainType
			{
				set
				{
					base.PowerSharpParameters["DomainType"] = value;
				}
			}

			public virtual AuthenticationType AuthenticationType
			{
				set
				{
					base.PowerSharpParameters["AuthenticationType"] = value;
				}
			}

			public virtual LiveIdInstanceType LiveIdInstanceType
			{
				set
				{
					base.PowerSharpParameters["LiveIdInstanceType"] = value;
				}
			}

			public virtual string CatchAllRecipient
			{
				set
				{
					base.PowerSharpParameters["CatchAllRecipient"] = ((value != null) ? new RecipientIdParameter(value) : null);
				}
			}

			public virtual bool MatchSubDomains
			{
				set
				{
					base.PowerSharpParameters["MatchSubDomains"] = value;
				}
			}

			public virtual string MailFlowPartner
			{
				set
				{
					base.PowerSharpParameters["MailFlowPartner"] = ((value != null) ? new MailFlowPartnerIdParameter(value) : null);
				}
			}

			public virtual bool OutboundOnly
			{
				set
				{
					base.PowerSharpParameters["OutboundOnly"] = value;
				}
			}

			public virtual bool InitialDomain
			{
				set
				{
					base.PowerSharpParameters["InitialDomain"] = value;
				}
			}

			public virtual SwitchParameter SkipDnsProvisioning
			{
				set
				{
					base.PowerSharpParameters["SkipDnsProvisioning"] = value;
				}
			}

			public virtual SwitchParameter SkipDomainNameValidation
			{
				set
				{
					base.PowerSharpParameters["SkipDomainNameValidation"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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
