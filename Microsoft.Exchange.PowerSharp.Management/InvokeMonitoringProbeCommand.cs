using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class InvokeMonitoringProbeCommand : SyntheticCommandWithPipelineInputNoOutput<string>
	{
		private InvokeMonitoringProbeCommand() : base("Invoke-MonitoringProbe")
		{
		}

		public InvokeMonitoringProbeCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual InvokeMonitoringProbeCommand SetParameters(InvokeMonitoringProbeCommand.DefaultParameters parameters)
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

			public virtual ServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual string ItemTargetExtension
			{
				set
				{
					base.PowerSharpParameters["ItemTargetExtension"] = value;
				}
			}

			public virtual string Account
			{
				set
				{
					base.PowerSharpParameters["Account"] = value;
				}
			}

			public virtual string Password
			{
				set
				{
					base.PowerSharpParameters["Password"] = value;
				}
			}

			public virtual string Endpoint
			{
				set
				{
					base.PowerSharpParameters["Endpoint"] = value;
				}
			}

			public virtual string SecondaryAccount
			{
				set
				{
					base.PowerSharpParameters["SecondaryAccount"] = value;
				}
			}

			public virtual string SecondaryPassword
			{
				set
				{
					base.PowerSharpParameters["SecondaryPassword"] = value;
				}
			}

			public virtual string SecondaryEndpoint
			{
				set
				{
					base.PowerSharpParameters["SecondaryEndpoint"] = value;
				}
			}

			public virtual string TimeOutSeconds
			{
				set
				{
					base.PowerSharpParameters["TimeOutSeconds"] = value;
				}
			}

			public virtual string PropertyOverride
			{
				set
				{
					base.PowerSharpParameters["PropertyOverride"] = value;
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
