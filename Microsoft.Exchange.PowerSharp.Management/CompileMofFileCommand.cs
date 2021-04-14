using System;
using System.Management.Automation;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class CompileMofFileCommand : SyntheticCommandWithPipelineInputNoOutput<string>
	{
		private CompileMofFileCommand() : base("Compile-MofFile")
		{
		}

		public CompileMofFileCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual CompileMofFileCommand SetParameters(CompileMofFileCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string MofFilePath
			{
				set
				{
					base.PowerSharpParameters["MofFilePath"] = value;
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
