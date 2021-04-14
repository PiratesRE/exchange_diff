using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetMSOFullSyncObjectRequestCommand : SyntheticCommandWithPipelineInput<FullSyncObjectRequest, FullSyncObjectRequest>
	{
		private GetMSOFullSyncObjectRequestCommand() : base("Get-MSOFullSyncObjectRequest")
		{
		}

		public GetMSOFullSyncObjectRequestCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetMSOFullSyncObjectRequestCommand SetParameters(GetMSOFullSyncObjectRequestCommand.IdentityParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMSOFullSyncObjectRequestCommand SetParameters(GetMSOFullSyncObjectRequestCommand.ServiceInstanceIdParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class IdentityParameterSetParameters : ParametersBase
		{
			public virtual SyncObjectId Identity
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

		public class ServiceInstanceIdParameterSetParameters : ParametersBase
		{
			public virtual ServiceInstanceId ServiceInstanceId
			{
				set
				{
					base.PowerSharpParameters["ServiceInstanceId"] = value;
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
