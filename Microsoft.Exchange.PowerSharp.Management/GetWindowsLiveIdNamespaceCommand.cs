using System;
using System.Management.Automation;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetWindowsLiveIdNamespaceCommand : SyntheticCommandWithPipelineInputNoOutput<string>
	{
		private GetWindowsLiveIdNamespaceCommand() : base("Get-WindowsLiveIdNamespace")
		{
		}

		public GetWindowsLiveIdNamespaceCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetWindowsLiveIdNamespaceCommand SetParameters(GetWindowsLiveIdNamespaceCommand.NamespaceParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class NamespaceParameters : ParametersBase
		{
			public virtual string Namespace
			{
				set
				{
					base.PowerSharpParameters["Namespace"] = value;
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
