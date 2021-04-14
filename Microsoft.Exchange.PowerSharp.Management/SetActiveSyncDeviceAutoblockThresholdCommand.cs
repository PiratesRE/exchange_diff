using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetActiveSyncDeviceAutoblockThresholdCommand : SyntheticCommandWithPipelineInputNoOutput<ActiveSyncDeviceAutoblockThreshold>
	{
		private SetActiveSyncDeviceAutoblockThresholdCommand() : base("Set-ActiveSyncDeviceAutoblockThreshold")
		{
		}

		public SetActiveSyncDeviceAutoblockThresholdCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetActiveSyncDeviceAutoblockThresholdCommand SetParameters(SetActiveSyncDeviceAutoblockThresholdCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetActiveSyncDeviceAutoblockThresholdCommand SetParameters(SetActiveSyncDeviceAutoblockThresholdCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual int BehaviorTypeIncidenceLimit
			{
				set
				{
					base.PowerSharpParameters["BehaviorTypeIncidenceLimit"] = value;
				}
			}

			public virtual EnhancedTimeSpan BehaviorTypeIncidenceDuration
			{
				set
				{
					base.PowerSharpParameters["BehaviorTypeIncidenceDuration"] = value;
				}
			}

			public virtual EnhancedTimeSpan DeviceBlockDuration
			{
				set
				{
					base.PowerSharpParameters["DeviceBlockDuration"] = value;
				}
			}

			public virtual string AdminEmailInsert
			{
				set
				{
					base.PowerSharpParameters["AdminEmailInsert"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ActiveSyncDeviceAutoblockThresholdIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual int BehaviorTypeIncidenceLimit
			{
				set
				{
					base.PowerSharpParameters["BehaviorTypeIncidenceLimit"] = value;
				}
			}

			public virtual EnhancedTimeSpan BehaviorTypeIncidenceDuration
			{
				set
				{
					base.PowerSharpParameters["BehaviorTypeIncidenceDuration"] = value;
				}
			}

			public virtual EnhancedTimeSpan DeviceBlockDuration
			{
				set
				{
					base.PowerSharpParameters["DeviceBlockDuration"] = value;
				}
			}

			public virtual string AdminEmailInsert
			{
				set
				{
					base.PowerSharpParameters["AdminEmailInsert"] = value;
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
