using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetSyncServiceInstanceCommand : SyntheticCommandWithPipelineInputNoOutput<SyncServiceInstance>
	{
		private SetSyncServiceInstanceCommand() : base("Set-SyncServiceInstance")
		{
		}

		public SetSyncServiceInstanceCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetSyncServiceInstanceCommand SetParameters(SetSyncServiceInstanceCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetSyncServiceInstanceCommand SetParameters(SetSyncServiceInstanceCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual AccountPartitionIdParameter AccountPartition
			{
				set
				{
					base.PowerSharpParameters["AccountPartition"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

			public virtual bool UseCentralConfig
			{
				set
				{
					base.PowerSharpParameters["UseCentralConfig"] = value;
				}
			}

			public virtual bool IsHalted
			{
				set
				{
					base.PowerSharpParameters["IsHalted"] = value;
				}
			}

			public virtual bool IsHaltRecoveryDisabled
			{
				set
				{
					base.PowerSharpParameters["IsHaltRecoveryDisabled"] = value;
				}
			}

			public virtual bool IsMultiObjectCookieEnabled
			{
				set
				{
					base.PowerSharpParameters["IsMultiObjectCookieEnabled"] = value;
				}
			}

			public virtual bool IsNewCookieBlocked
			{
				set
				{
					base.PowerSharpParameters["IsNewCookieBlocked"] = value;
				}
			}

			public virtual bool IsUsedForTenantToServiceInstanceAssociation
			{
				set
				{
					base.PowerSharpParameters["IsUsedForTenantToServiceInstanceAssociation"] = value;
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

			public virtual Version TargetServerMinVersion
			{
				set
				{
					base.PowerSharpParameters["TargetServerMinVersion"] = value;
				}
			}

			public virtual Version TargetServerMaxVersion
			{
				set
				{
					base.PowerSharpParameters["TargetServerMaxVersion"] = value;
				}
			}

			public virtual string ForwardSyncConfigurationXML
			{
				set
				{
					base.PowerSharpParameters["ForwardSyncConfigurationXML"] = value;
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

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
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

			public virtual AccountPartitionIdParameter AccountPartition
			{
				set
				{
					base.PowerSharpParameters["AccountPartition"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

			public virtual bool UseCentralConfig
			{
				set
				{
					base.PowerSharpParameters["UseCentralConfig"] = value;
				}
			}

			public virtual bool IsHalted
			{
				set
				{
					base.PowerSharpParameters["IsHalted"] = value;
				}
			}

			public virtual bool IsHaltRecoveryDisabled
			{
				set
				{
					base.PowerSharpParameters["IsHaltRecoveryDisabled"] = value;
				}
			}

			public virtual bool IsMultiObjectCookieEnabled
			{
				set
				{
					base.PowerSharpParameters["IsMultiObjectCookieEnabled"] = value;
				}
			}

			public virtual bool IsNewCookieBlocked
			{
				set
				{
					base.PowerSharpParameters["IsNewCookieBlocked"] = value;
				}
			}

			public virtual bool IsUsedForTenantToServiceInstanceAssociation
			{
				set
				{
					base.PowerSharpParameters["IsUsedForTenantToServiceInstanceAssociation"] = value;
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

			public virtual Version TargetServerMinVersion
			{
				set
				{
					base.PowerSharpParameters["TargetServerMinVersion"] = value;
				}
			}

			public virtual Version TargetServerMaxVersion
			{
				set
				{
					base.PowerSharpParameters["TargetServerMaxVersion"] = value;
				}
			}

			public virtual string ForwardSyncConfigurationXML
			{
				set
				{
					base.PowerSharpParameters["ForwardSyncConfigurationXML"] = value;
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

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}
	}
}
