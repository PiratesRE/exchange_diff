using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class StopSetupserviceCommand : SyntheticCommandWithPipelineInputNoOutput<string>
	{
		private StopSetupserviceCommand() : base("Stop-Setupservice")
		{
		}

		public StopSetupserviceCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual StopSetupserviceCommand SetParameters(StopSetupserviceCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string ServiceName
			{
				set
				{
					base.PowerSharpParameters["ServiceName"] = value;
				}
			}

			public virtual bool IgnoreTimeout
			{
				set
				{
					base.PowerSharpParameters["IgnoreTimeout"] = value;
				}
			}

			public virtual string ServiceParameters
			{
				set
				{
					base.PowerSharpParameters["ServiceParameters"] = value;
				}
			}

			public virtual bool FailIfServiceNotInstalled
			{
				set
				{
					base.PowerSharpParameters["FailIfServiceNotInstalled"] = value;
				}
			}

			public virtual Unlimited<EnhancedTimeSpan> MaximumWaitTime
			{
				set
				{
					base.PowerSharpParameters["MaximumWaitTime"] = value;
				}
			}

			public virtual Unlimited<EnhancedTimeSpan> MaxWaitTimeForRunningState
			{
				set
				{
					base.PowerSharpParameters["MaxWaitTimeForRunningState"] = value;
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
