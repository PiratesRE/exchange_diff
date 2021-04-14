using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetUMIPGatewayCommand : SyntheticCommandWithPipelineInputNoOutput<UMIPGateway>
	{
		private SetUMIPGatewayCommand() : base("Set-UMIPGateway")
		{
		}

		public SetUMIPGatewayCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetUMIPGatewayCommand SetParameters(SetUMIPGatewayCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetUMIPGatewayCommand SetParameters(SetUMIPGatewayCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter ForceUpgrade
			{
				set
				{
					base.PowerSharpParameters["ForceUpgrade"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual UMSmartHost Address
			{
				set
				{
					base.PowerSharpParameters["Address"] = value;
				}
			}

			public virtual bool OutcallsAllowed
			{
				set
				{
					base.PowerSharpParameters["OutcallsAllowed"] = value;
				}
			}

			public virtual GatewayStatus Status
			{
				set
				{
					base.PowerSharpParameters["Status"] = value;
				}
			}

			public virtual int Port
			{
				set
				{
					base.PowerSharpParameters["Port"] = value;
				}
			}

			public virtual bool Simulator
			{
				set
				{
					base.PowerSharpParameters["Simulator"] = value;
				}
			}

			public virtual IPAddressFamily IPAddressFamily
			{
				set
				{
					base.PowerSharpParameters["IPAddressFamily"] = value;
				}
			}

			public virtual bool DelayedSourcePartyInfoEnabled
			{
				set
				{
					base.PowerSharpParameters["DelayedSourcePartyInfoEnabled"] = value;
				}
			}

			public virtual bool MessageWaitingIndicatorAllowed
			{
				set
				{
					base.PowerSharpParameters["MessageWaitingIndicatorAllowed"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new UMIPGatewayIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter ForceUpgrade
			{
				set
				{
					base.PowerSharpParameters["ForceUpgrade"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual UMSmartHost Address
			{
				set
				{
					base.PowerSharpParameters["Address"] = value;
				}
			}

			public virtual bool OutcallsAllowed
			{
				set
				{
					base.PowerSharpParameters["OutcallsAllowed"] = value;
				}
			}

			public virtual GatewayStatus Status
			{
				set
				{
					base.PowerSharpParameters["Status"] = value;
				}
			}

			public virtual int Port
			{
				set
				{
					base.PowerSharpParameters["Port"] = value;
				}
			}

			public virtual bool Simulator
			{
				set
				{
					base.PowerSharpParameters["Simulator"] = value;
				}
			}

			public virtual IPAddressFamily IPAddressFamily
			{
				set
				{
					base.PowerSharpParameters["IPAddressFamily"] = value;
				}
			}

			public virtual bool DelayedSourcePartyInfoEnabled
			{
				set
				{
					base.PowerSharpParameters["DelayedSourcePartyInfoEnabled"] = value;
				}
			}

			public virtual bool MessageWaitingIndicatorAllowed
			{
				set
				{
					base.PowerSharpParameters["MessageWaitingIndicatorAllowed"] = value;
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
