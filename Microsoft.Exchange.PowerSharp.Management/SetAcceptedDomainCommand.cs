using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetAcceptedDomainCommand : SyntheticCommandWithPipelineInputNoOutput<AcceptedDomain>
	{
		private SetAcceptedDomainCommand() : base("Set-AcceptedDomain")
		{
		}

		public SetAcceptedDomainCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetAcceptedDomainCommand SetParameters(SetAcceptedDomainCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetAcceptedDomainCommand SetParameters(SetAcceptedDomainCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual bool MakeDefault
			{
				set
				{
					base.PowerSharpParameters["MakeDefault"] = value;
				}
			}

			public virtual bool IsCoexistenceDomain
			{
				set
				{
					base.PowerSharpParameters["IsCoexistenceDomain"] = value;
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

			public virtual AuthenticationType AuthenticationType
			{
				set
				{
					base.PowerSharpParameters["AuthenticationType"] = value;
				}
			}

			public virtual bool InitialDomain
			{
				set
				{
					base.PowerSharpParameters["InitialDomain"] = value;
				}
			}

			public virtual LiveIdInstanceType LiveIdInstanceType
			{
				set
				{
					base.PowerSharpParameters["LiveIdInstanceType"] = value;
				}
			}

			public virtual bool EnableNego2Authentication
			{
				set
				{
					base.PowerSharpParameters["EnableNego2Authentication"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual AcceptedDomainType DomainType
			{
				set
				{
					base.PowerSharpParameters["DomainType"] = value;
				}
			}

			public virtual bool AddressBookEnabled
			{
				set
				{
					base.PowerSharpParameters["AddressBookEnabled"] = value;
				}
			}

			public virtual bool PendingRemoval
			{
				set
				{
					base.PowerSharpParameters["PendingRemoval"] = value;
				}
			}

			public virtual bool PendingCompletion
			{
				set
				{
					base.PowerSharpParameters["PendingCompletion"] = value;
				}
			}

			public virtual bool DualProvisioningEnabled
			{
				set
				{
					base.PowerSharpParameters["DualProvisioningEnabled"] = value;
				}
			}

			public virtual bool OutboundOnly
			{
				set
				{
					base.PowerSharpParameters["OutboundOnly"] = value;
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
			public virtual AcceptedDomainIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual bool MakeDefault
			{
				set
				{
					base.PowerSharpParameters["MakeDefault"] = value;
				}
			}

			public virtual bool IsCoexistenceDomain
			{
				set
				{
					base.PowerSharpParameters["IsCoexistenceDomain"] = value;
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

			public virtual AuthenticationType AuthenticationType
			{
				set
				{
					base.PowerSharpParameters["AuthenticationType"] = value;
				}
			}

			public virtual bool InitialDomain
			{
				set
				{
					base.PowerSharpParameters["InitialDomain"] = value;
				}
			}

			public virtual LiveIdInstanceType LiveIdInstanceType
			{
				set
				{
					base.PowerSharpParameters["LiveIdInstanceType"] = value;
				}
			}

			public virtual bool EnableNego2Authentication
			{
				set
				{
					base.PowerSharpParameters["EnableNego2Authentication"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual AcceptedDomainType DomainType
			{
				set
				{
					base.PowerSharpParameters["DomainType"] = value;
				}
			}

			public virtual bool AddressBookEnabled
			{
				set
				{
					base.PowerSharpParameters["AddressBookEnabled"] = value;
				}
			}

			public virtual bool PendingRemoval
			{
				set
				{
					base.PowerSharpParameters["PendingRemoval"] = value;
				}
			}

			public virtual bool PendingCompletion
			{
				set
				{
					base.PowerSharpParameters["PendingCompletion"] = value;
				}
			}

			public virtual bool DualProvisioningEnabled
			{
				set
				{
					base.PowerSharpParameters["DualProvisioningEnabled"] = value;
				}
			}

			public virtual bool OutboundOnly
			{
				set
				{
					base.PowerSharpParameters["OutboundOnly"] = value;
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
