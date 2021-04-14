using System;
using System.Management.Automation;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class PreserveAppSettingsCommand : SyntheticCommandWithPipelineInputNoOutput<string>
	{
		private PreserveAppSettingsCommand() : base("Preserve-AppSettings")
		{
		}

		public PreserveAppSettingsCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual PreserveAppSettingsCommand SetParameters(PreserveAppSettingsCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string RoleInstallPath
			{
				set
				{
					base.PowerSharpParameters["RoleInstallPath"] = value;
				}
			}

			public virtual string ConfigFileName
			{
				set
				{
					base.PowerSharpParameters["ConfigFileName"] = value;
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
