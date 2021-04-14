using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewClientAccessRuleCommand : SyntheticCommandWithPipelineInput<ADClientAccessRule, ADClientAccessRule>
	{
		private NewClientAccessRuleCommand() : base("New-ClientAccessRule")
		{
		}

		public NewClientAccessRuleCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewClientAccessRuleCommand SetParameters(NewClientAccessRuleCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual int Priority
			{
				set
				{
					base.PowerSharpParameters["Priority"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual bool DatacenterAdminsOnly
			{
				set
				{
					base.PowerSharpParameters["DatacenterAdminsOnly"] = value;
				}
			}

			public virtual ClientAccessRulesAction Action
			{
				set
				{
					base.PowerSharpParameters["Action"] = value;
				}
			}

			public virtual MultiValuedProperty<IPRange> AnyOfClientIPAddressesOrRanges
			{
				set
				{
					base.PowerSharpParameters["AnyOfClientIPAddressesOrRanges"] = value;
				}
			}

			public virtual MultiValuedProperty<IPRange> ExceptAnyOfClientIPAddressesOrRanges
			{
				set
				{
					base.PowerSharpParameters["ExceptAnyOfClientIPAddressesOrRanges"] = value;
				}
			}

			public virtual MultiValuedProperty<IntRange> AnyOfSourceTcpPortNumbers
			{
				set
				{
					base.PowerSharpParameters["AnyOfSourceTcpPortNumbers"] = value;
				}
			}

			public virtual MultiValuedProperty<IntRange> ExceptAnyOfSourceTcpPortNumbers
			{
				set
				{
					base.PowerSharpParameters["ExceptAnyOfSourceTcpPortNumbers"] = value;
				}
			}

			public virtual MultiValuedProperty<string> UsernameMatchesAnyOfPatterns
			{
				set
				{
					base.PowerSharpParameters["UsernameMatchesAnyOfPatterns"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ExceptUsernameMatchesAnyOfPatterns
			{
				set
				{
					base.PowerSharpParameters["ExceptUsernameMatchesAnyOfPatterns"] = value;
				}
			}

			public virtual MultiValuedProperty<string> UserIsMemberOf
			{
				set
				{
					base.PowerSharpParameters["UserIsMemberOf"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ExceptUserIsMemberOf
			{
				set
				{
					base.PowerSharpParameters["ExceptUserIsMemberOf"] = value;
				}
			}

			public virtual MultiValuedProperty<ClientAccessAuthenticationMethod> AnyOfAuthenticationTypes
			{
				set
				{
					base.PowerSharpParameters["AnyOfAuthenticationTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<ClientAccessAuthenticationMethod> ExceptAnyOfAuthenticationTypes
			{
				set
				{
					base.PowerSharpParameters["ExceptAnyOfAuthenticationTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<ClientAccessProtocol> AnyOfProtocols
			{
				set
				{
					base.PowerSharpParameters["AnyOfProtocols"] = value;
				}
			}

			public virtual MultiValuedProperty<ClientAccessProtocol> ExceptAnyOfProtocols
			{
				set
				{
					base.PowerSharpParameters["ExceptAnyOfProtocols"] = value;
				}
			}

			public virtual string UserRecipientFilter
			{
				set
				{
					base.PowerSharpParameters["UserRecipientFilter"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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
