using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetServerComponentStateCommand : SyntheticCommandWithPipelineInputNoOutput<ServerIdParameter>
	{
		private SetServerComponentStateCommand() : base("Set-ServerComponentState")
		{
		}

		public SetServerComponentStateCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetServerComponentStateCommand SetParameters(SetServerComponentStateCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual ServerIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual string Component
			{
				set
				{
					base.PowerSharpParameters["Component"] = value;
				}
			}

			public virtual ServiceState State
			{
				set
				{
					base.PowerSharpParameters["State"] = value;
				}
			}

			public virtual string Requester
			{
				set
				{
					base.PowerSharpParameters["Requester"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual SwitchParameter RemoteOnly
			{
				set
				{
					base.PowerSharpParameters["RemoteOnly"] = value;
				}
			}

			public virtual SwitchParameter LocalOnly
			{
				set
				{
					base.PowerSharpParameters["LocalOnly"] = value;
				}
			}

			public virtual int TimeoutInSeconds
			{
				set
				{
					base.PowerSharpParameters["TimeoutInSeconds"] = value;
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
