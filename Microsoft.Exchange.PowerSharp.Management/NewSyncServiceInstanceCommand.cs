using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewSyncServiceInstanceCommand : SyntheticCommandWithPipelineInput<SyncServiceInstance, SyncServiceInstance>
	{
		private NewSyncServiceInstanceCommand() : base("New-SyncServiceInstance")
		{
		}

		public NewSyncServiceInstanceCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewSyncServiceInstanceCommand SetParameters(NewSyncServiceInstanceCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual ServiceInstanceId Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual AccountPartitionIdParameter AccountPartition
			{
				set
				{
					base.PowerSharpParameters["AccountPartition"] = value;
				}
			}

			public virtual Version MinVersion
			{
				set
				{
					base.PowerSharpParameters["MinVersion"] = value;
				}
			}

			public virtual Version MaxVersion
			{
				set
				{
					base.PowerSharpParameters["MaxVersion"] = value;
				}
			}

			public virtual int ActiveInstanceSleepInterval
			{
				set
				{
					base.PowerSharpParameters["ActiveInstanceSleepInterval"] = value;
				}
			}

			public virtual int PassiveInstanceSleepInterval
			{
				set
				{
					base.PowerSharpParameters["PassiveInstanceSleepInterval"] = value;
				}
			}

			public virtual bool IsEnabled
			{
				set
				{
					base.PowerSharpParameters["IsEnabled"] = value;
				}
			}

			public virtual Version NewTenantMinVersion
			{
				set
				{
					base.PowerSharpParameters["NewTenantMinVersion"] = value;
				}
			}

			public virtual Version NewTenantMaxVersion
			{
				set
				{
					base.PowerSharpParameters["NewTenantMaxVersion"] = value;
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
