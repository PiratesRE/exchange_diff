using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetMSOSyncDataCommand : SyntheticCommandWithPipelineInput<ADRawEntry, ADRawEntry>
	{
		private GetMSOSyncDataCommand() : base("Get-MSOSyncData")
		{
		}

		public GetMSOSyncDataCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetMSOSyncDataCommand SetParameters(GetMSOSyncDataCommand.ObjectFullSyncInitialCallParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMSOSyncDataCommand SetParameters(GetMSOSyncDataCommand.IncrementalSyncParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMSOSyncDataCommand SetParameters(GetMSOSyncDataCommand.TenantFullSyncInitialCallParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMSOSyncDataCommand SetParameters(GetMSOSyncDataCommand.ObjectFullSyncInitialCallFromMergeSyncParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMSOSyncDataCommand SetParameters(GetMSOSyncDataCommand.ObjectFullSyncInitialCallFromTenantFullSyncParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMSOSyncDataCommand SetParameters(GetMSOSyncDataCommand.ObjectFullSyncSubsequentCallParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMSOSyncDataCommand SetParameters(GetMSOSyncDataCommand.TenantFullSyncSubsequentCallParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMSOSyncDataCommand SetParameters(GetMSOSyncDataCommand.MergeInitialCallParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMSOSyncDataCommand SetParameters(GetMSOSyncDataCommand.MergeSubsequentCallParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMSOSyncDataCommand SetParameters(GetMSOSyncDataCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class ObjectFullSyncInitialCallParameterSetParameters : ParametersBase
		{
			public virtual byte Cookie
			{
				set
				{
					base.PowerSharpParameters["Cookie"] = value;
				}
			}

			public virtual SyncObjectId ObjectIds
			{
				set
				{
					base.PowerSharpParameters["ObjectIds"] = value;
				}
			}

			public virtual BackSyncOptions SyncOptions
			{
				set
				{
					base.PowerSharpParameters["SyncOptions"] = value;
				}
			}

			public virtual ServiceInstanceId ServiceInstance
			{
				set
				{
					base.PowerSharpParameters["ServiceInstance"] = value;
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

		public class IncrementalSyncParameterSetParameters : ParametersBase
		{
			public virtual byte Cookie
			{
				set
				{
					base.PowerSharpParameters["Cookie"] = value;
				}
			}

			public virtual ServiceInstanceId ServiceInstance
			{
				set
				{
					base.PowerSharpParameters["ServiceInstance"] = value;
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

		public class TenantFullSyncInitialCallParameterSetParameters : ParametersBase
		{
			public virtual byte Cookie
			{
				set
				{
					base.PowerSharpParameters["Cookie"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual ServiceInstanceId ServiceInstance
			{
				set
				{
					base.PowerSharpParameters["ServiceInstance"] = value;
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

		public class ObjectFullSyncInitialCallFromMergeSyncParameterSetParameters : ParametersBase
		{
			public virtual SyncObjectId ObjectIds
			{
				set
				{
					base.PowerSharpParameters["ObjectIds"] = value;
				}
			}

			public virtual BackSyncOptions SyncOptions
			{
				set
				{
					base.PowerSharpParameters["SyncOptions"] = value;
				}
			}

			public virtual byte MergePageTokenContext
			{
				set
				{
					base.PowerSharpParameters["MergePageTokenContext"] = value;
				}
			}

			public virtual ServiceInstanceId ServiceInstance
			{
				set
				{
					base.PowerSharpParameters["ServiceInstance"] = value;
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

		public class ObjectFullSyncInitialCallFromTenantFullSyncParameterSetParameters : ParametersBase
		{
			public virtual SyncObjectId ObjectIds
			{
				set
				{
					base.PowerSharpParameters["ObjectIds"] = value;
				}
			}

			public virtual BackSyncOptions SyncOptions
			{
				set
				{
					base.PowerSharpParameters["SyncOptions"] = value;
				}
			}

			public virtual byte TenantFullSyncPageTokenContext
			{
				set
				{
					base.PowerSharpParameters["TenantFullSyncPageTokenContext"] = value;
				}
			}

			public virtual ServiceInstanceId ServiceInstance
			{
				set
				{
					base.PowerSharpParameters["ServiceInstance"] = value;
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

		public class ObjectFullSyncSubsequentCallParameterSetParameters : ParametersBase
		{
			public virtual byte ObjectFullSyncPageToken
			{
				set
				{
					base.PowerSharpParameters["ObjectFullSyncPageToken"] = value;
				}
			}

			public virtual ServiceInstanceId ServiceInstance
			{
				set
				{
					base.PowerSharpParameters["ServiceInstance"] = value;
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

		public class TenantFullSyncSubsequentCallParameterSetParameters : ParametersBase
		{
			public virtual byte TenantFullSyncPageToken
			{
				set
				{
					base.PowerSharpParameters["TenantFullSyncPageToken"] = value;
				}
			}

			public virtual ServiceInstanceId ServiceInstance
			{
				set
				{
					base.PowerSharpParameters["ServiceInstance"] = value;
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

		public class MergeInitialCallParameterSetParameters : ParametersBase
		{
			public virtual byte MergeTenantFullSyncPageToken
			{
				set
				{
					base.PowerSharpParameters["MergeTenantFullSyncPageToken"] = value;
				}
			}

			public virtual byte MergeIncrementalSyncCookie
			{
				set
				{
					base.PowerSharpParameters["MergeIncrementalSyncCookie"] = value;
				}
			}

			public virtual ServiceInstanceId ServiceInstance
			{
				set
				{
					base.PowerSharpParameters["ServiceInstance"] = value;
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

		public class MergeSubsequentCallParameterSetParameters : ParametersBase
		{
			public virtual byte MergePageToken
			{
				set
				{
					base.PowerSharpParameters["MergePageToken"] = value;
				}
			}

			public virtual ServiceInstanceId ServiceInstance
			{
				set
				{
					base.PowerSharpParameters["ServiceInstance"] = value;
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

		public class DefaultParameters : ParametersBase
		{
			public virtual ServiceInstanceId ServiceInstance
			{
				set
				{
					base.PowerSharpParameters["ServiceInstance"] = value;
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
