using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetBposServiceInstanceInfoCommand : SyntheticCommandWithPipelineInputNoOutput<ServiceInstanceId>
	{
		private SetBposServiceInstanceInfoCommand() : base("Set-BposServiceInstanceInfo")
		{
		}

		public SetBposServiceInstanceInfoCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetBposServiceInstanceInfoCommand SetParameters(SetBposServiceInstanceInfoCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual ServiceInstanceId Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual Uri BackSyncUrl
			{
				set
				{
					base.PowerSharpParameters["BackSyncUrl"] = value;
				}
			}

			public virtual bool SupportsAuthorityTransfer
			{
				set
				{
					base.PowerSharpParameters["SupportsAuthorityTransfer"] = value;
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
