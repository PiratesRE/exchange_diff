using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class EnableUMMailboxPlanCommand : SyntheticCommandWithPipelineInput<MailboxPlanIdParameter, MailboxPlanIdParameter>
	{
		private EnableUMMailboxPlanCommand() : base("Enable-UMMailboxPlan")
		{
		}

		public EnableUMMailboxPlanCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual EnableUMMailboxPlanCommand SetParameters(EnableUMMailboxPlanCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual EnableUMMailboxPlanCommand SetParameters(EnableUMMailboxPlanCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual MultiValuedProperty<string> Extensions
			{
				set
				{
					base.PowerSharpParameters["Extensions"] = value;
				}
			}

			public virtual string UMMailboxPolicy
			{
				set
				{
					base.PowerSharpParameters["UMMailboxPolicy"] = ((value != null) ? new MailboxPolicyIdParameter(value) : null);
				}
			}

			public virtual string SIPResourceIdentifier
			{
				set
				{
					base.PowerSharpParameters["SIPResourceIdentifier"] = value;
				}
			}

			public virtual string Pin
			{
				set
				{
					base.PowerSharpParameters["Pin"] = value;
				}
			}

			public virtual bool PinExpired
			{
				set
				{
					base.PowerSharpParameters["PinExpired"] = value;
				}
			}

			public virtual string NotifyEmail
			{
				set
				{
					base.PowerSharpParameters["NotifyEmail"] = value;
				}
			}

			public virtual string PilotNumber
			{
				set
				{
					base.PowerSharpParameters["PilotNumber"] = value;
				}
			}

			public virtual bool AutomaticSpeechRecognitionEnabled
			{
				set
				{
					base.PowerSharpParameters["AutomaticSpeechRecognitionEnabled"] = value;
				}
			}

			public virtual SwitchParameter ValidateOnly
			{
				set
				{
					base.PowerSharpParameters["ValidateOnly"] = value;
				}
			}

			public virtual SwitchParameter IgnoreDefaultScope
			{
				set
				{
					base.PowerSharpParameters["IgnoreDefaultScope"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxPlanIdParameter(value) : null);
				}
			}

			public virtual MultiValuedProperty<string> Extensions
			{
				set
				{
					base.PowerSharpParameters["Extensions"] = value;
				}
			}

			public virtual string UMMailboxPolicy
			{
				set
				{
					base.PowerSharpParameters["UMMailboxPolicy"] = ((value != null) ? new MailboxPolicyIdParameter(value) : null);
				}
			}

			public virtual string SIPResourceIdentifier
			{
				set
				{
					base.PowerSharpParameters["SIPResourceIdentifier"] = value;
				}
			}

			public virtual string Pin
			{
				set
				{
					base.PowerSharpParameters["Pin"] = value;
				}
			}

			public virtual bool PinExpired
			{
				set
				{
					base.PowerSharpParameters["PinExpired"] = value;
				}
			}

			public virtual string NotifyEmail
			{
				set
				{
					base.PowerSharpParameters["NotifyEmail"] = value;
				}
			}

			public virtual string PilotNumber
			{
				set
				{
					base.PowerSharpParameters["PilotNumber"] = value;
				}
			}

			public virtual bool AutomaticSpeechRecognitionEnabled
			{
				set
				{
					base.PowerSharpParameters["AutomaticSpeechRecognitionEnabled"] = value;
				}
			}

			public virtual SwitchParameter ValidateOnly
			{
				set
				{
					base.PowerSharpParameters["ValidateOnly"] = value;
				}
			}

			public virtual SwitchParameter IgnoreDefaultScope
			{
				set
				{
					base.PowerSharpParameters["IgnoreDefaultScope"] = value;
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
