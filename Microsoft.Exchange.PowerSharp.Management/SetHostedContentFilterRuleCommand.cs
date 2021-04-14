using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetHostedContentFilterRuleCommand : SyntheticCommandWithPipelineInputNoOutput<HostedContentFilterRule>
	{
		private SetHostedContentFilterRuleCommand() : base("Set-HostedContentFilterRule")
		{
		}

		public SetHostedContentFilterRuleCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetHostedContentFilterRuleCommand SetParameters(SetHostedContentFilterRuleCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetHostedContentFilterRuleCommand SetParameters(SetHostedContentFilterRuleCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual RecipientIdParameter SentTo
			{
				set
				{
					base.PowerSharpParameters["SentTo"] = value;
				}
			}

			public virtual RecipientIdParameter SentToMemberOf
			{
				set
				{
					base.PowerSharpParameters["SentToMemberOf"] = value;
				}
			}

			public virtual Word RecipientDomainIs
			{
				set
				{
					base.PowerSharpParameters["RecipientDomainIs"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfSentTo
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSentTo"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfSentToMemberOf
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSentToMemberOf"] = value;
				}
			}

			public virtual Word ExceptIfRecipientDomainIs
			{
				set
				{
					base.PowerSharpParameters["ExceptIfRecipientDomainIs"] = value;
				}
			}

			public virtual string HostedContentFilterPolicy
			{
				set
				{
					base.PowerSharpParameters["HostedContentFilterPolicy"] = ((value != null) ? new HostedContentFilterPolicyIdParameter(value) : null);
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual int Priority
			{
				set
				{
					base.PowerSharpParameters["Priority"] = value;
				}
			}

			public virtual string Comments
			{
				set
				{
					base.PowerSharpParameters["Comments"] = value;
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

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new RuleIdParameter(value) : null);
				}
			}

			public virtual RecipientIdParameter SentTo
			{
				set
				{
					base.PowerSharpParameters["SentTo"] = value;
				}
			}

			public virtual RecipientIdParameter SentToMemberOf
			{
				set
				{
					base.PowerSharpParameters["SentToMemberOf"] = value;
				}
			}

			public virtual Word RecipientDomainIs
			{
				set
				{
					base.PowerSharpParameters["RecipientDomainIs"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfSentTo
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSentTo"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfSentToMemberOf
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSentToMemberOf"] = value;
				}
			}

			public virtual Word ExceptIfRecipientDomainIs
			{
				set
				{
					base.PowerSharpParameters["ExceptIfRecipientDomainIs"] = value;
				}
			}

			public virtual string HostedContentFilterPolicy
			{
				set
				{
					base.PowerSharpParameters["HostedContentFilterPolicy"] = ((value != null) ? new HostedContentFilterPolicyIdParameter(value) : null);
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual int Priority
			{
				set
				{
					base.PowerSharpParameters["Priority"] = value;
				}
			}

			public virtual string Comments
			{
				set
				{
					base.PowerSharpParameters["Comments"] = value;
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
