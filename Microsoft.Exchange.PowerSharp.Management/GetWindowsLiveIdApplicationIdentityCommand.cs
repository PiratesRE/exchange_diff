using System;
using System.Management.Automation;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetWindowsLiveIdApplicationIdentityCommand : SyntheticCommandWithPipelineInputNoOutput<string>
	{
		private GetWindowsLiveIdApplicationIdentityCommand() : base("Get-WindowsLiveIdApplicationIdentity")
		{
		}

		public GetWindowsLiveIdApplicationIdentityCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetWindowsLiveIdApplicationIdentityCommand SetParameters(GetWindowsLiveIdApplicationIdentityCommand.UriParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetWindowsLiveIdApplicationIdentityCommand SetParameters(GetWindowsLiveIdApplicationIdentityCommand.AppIDParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class UriParameters : ParametersBase
		{
			public virtual string Uri
			{
				set
				{
					base.PowerSharpParameters["Uri"] = value;
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

		public class AppIDParameters : ParametersBase
		{
			public virtual string AppId
			{
				set
				{
					base.PowerSharpParameters["AppId"] = value;
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
