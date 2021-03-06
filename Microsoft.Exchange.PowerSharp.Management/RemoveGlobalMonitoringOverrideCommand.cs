using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class RemoveGlobalMonitoringOverrideCommand : SyntheticCommandWithPipelineInput<MonitoringOverride, MonitoringOverride>
	{
		private RemoveGlobalMonitoringOverrideCommand() : base("Remove-GlobalMonitoringOverride")
		{
		}

		public RemoveGlobalMonitoringOverrideCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual RemoveGlobalMonitoringOverrideCommand SetParameters(RemoveGlobalMonitoringOverrideCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual MonitoringItemTypeEnum ItemType
			{
				set
				{
					base.PowerSharpParameters["ItemType"] = value;
				}
			}

			public virtual string PropertyName
			{
				set
				{
					base.PowerSharpParameters["PropertyName"] = value;
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
