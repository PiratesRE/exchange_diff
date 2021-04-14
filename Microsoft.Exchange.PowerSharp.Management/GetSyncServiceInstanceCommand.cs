using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetSyncServiceInstanceCommand : SyntheticCommandWithPipelineInput<SyncServiceInstance, SyncServiceInstance>
	{
		private GetSyncServiceInstanceCommand() : base("Get-SyncServiceInstance")
		{
		}

		public GetSyncServiceInstanceCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetSyncServiceInstanceCommand SetParameters(GetSyncServiceInstanceCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual ServiceInstanceIdParameter Identity
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
