using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.UM.UMPhoneSession;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class StartUMPhoneSessionCommand : SyntheticCommandWithPipelineInput<UMPhoneSession, UMPhoneSession>
	{
		private StartUMPhoneSessionCommand() : base("Start-UMPhoneSession")
		{
		}

		public StartUMPhoneSessionCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual StartUMPhoneSessionCommand SetParameters(StartUMPhoneSessionCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual StartUMPhoneSessionCommand SetParameters(StartUMPhoneSessionCommand.AwayVoicemailGreetingParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual StartUMPhoneSessionCommand SetParameters(StartUMPhoneSessionCommand.DefaultVoicemailGreetingParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual StartUMPhoneSessionCommand SetParameters(StartUMPhoneSessionCommand.PlayOnPhoneGreetingParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
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

		public class AwayVoicemailGreetingParameters : ParametersBase
		{
			public virtual string UMMailbox
			{
				set
				{
					base.PowerSharpParameters["UMMailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual string PhoneNumber
			{
				set
				{
					base.PowerSharpParameters["PhoneNumber"] = value;
				}
			}

			public virtual SwitchParameter AwayVoicemailGreeting
			{
				set
				{
					base.PowerSharpParameters["AwayVoicemailGreeting"] = value;
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

		public class DefaultVoicemailGreetingParameters : ParametersBase
		{
			public virtual string UMMailbox
			{
				set
				{
					base.PowerSharpParameters["UMMailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual string PhoneNumber
			{
				set
				{
					base.PowerSharpParameters["PhoneNumber"] = value;
				}
			}

			public virtual SwitchParameter DefaultVoicemailGreeting
			{
				set
				{
					base.PowerSharpParameters["DefaultVoicemailGreeting"] = value;
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

		public class PlayOnPhoneGreetingParameters : ParametersBase
		{
			public virtual string PhoneNumber
			{
				set
				{
					base.PowerSharpParameters["PhoneNumber"] = value;
				}
			}

			public virtual string CallAnsweringRuleId
			{
				set
				{
					base.PowerSharpParameters["CallAnsweringRuleId"] = ((value != null) ? new UMCallAnsweringRuleIdParameter(value) : null);
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
