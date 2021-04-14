using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetBposServiceInstanceInfoCommand : SyntheticCommandWithPipelineInputNoOutput<ServiceInstanceId>
	{
		private GetBposServiceInstanceInfoCommand() : base("Get-BposServiceInstanceInfo")
		{
		}

		public GetBposServiceInstanceInfoCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetBposServiceInstanceInfoCommand SetParameters(GetBposServiceInstanceInfoCommand.DefaultParameters parameters)
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
