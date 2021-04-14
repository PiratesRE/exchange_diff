using System;
using System.Management.Automation;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetInstallPathInAppConfigCommand : SyntheticCommandWithPipelineInputNoOutput<string>
	{
		private SetInstallPathInAppConfigCommand() : base("Set-InstallPathInAppConfig")
		{
		}

		public SetInstallPathInAppConfigCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetInstallPathInAppConfigCommand SetParameters(SetInstallPathInAppConfigCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string ExchangeInstallPath
			{
				set
				{
					base.PowerSharpParameters["ExchangeInstallPath"] = value;
				}
			}

			public virtual string ReplacementString
			{
				set
				{
					base.PowerSharpParameters["ReplacementString"] = value;
				}
			}

			public virtual string ConfigFileAbsolutePath
			{
				set
				{
					base.PowerSharpParameters["ConfigFileAbsolutePath"] = value;
				}
			}

			public virtual string ConfigFileRelativePath
			{
				set
				{
					base.PowerSharpParameters["ConfigFileRelativePath"] = value;
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
