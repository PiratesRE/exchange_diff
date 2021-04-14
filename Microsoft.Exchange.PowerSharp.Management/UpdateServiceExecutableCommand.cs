using System;
using System.Management.Automation;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class UpdateServiceExecutableCommand : SyntheticCommandWithPipelineInputNoOutput<string>
	{
		private UpdateServiceExecutableCommand() : base("Update-ServiceExecutable")
		{
		}

		public UpdateServiceExecutableCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual UpdateServiceExecutableCommand SetParameters(UpdateServiceExecutableCommand.DefaultParameters parameters)
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

			public virtual string Executable
			{
				set
				{
					base.PowerSharpParameters["Executable"] = value;
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
