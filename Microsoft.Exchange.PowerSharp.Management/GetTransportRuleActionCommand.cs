using System;
using System.Management.Automation;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetTransportRuleActionCommand : SyntheticCommandWithPipelineInput<TransportRuleAction, TransportRuleAction>
	{
		private GetTransportRuleActionCommand() : base("Get-TransportRuleAction")
		{
		}

		public GetTransportRuleActionCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetTransportRuleActionCommand SetParameters(GetTransportRuleActionCommand.DefaultParameters parameters)
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
		}
	}
}
