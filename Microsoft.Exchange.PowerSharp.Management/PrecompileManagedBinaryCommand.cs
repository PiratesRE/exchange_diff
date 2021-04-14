using System;
using System.Management.Automation;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class PrecompileManagedBinaryCommand : SyntheticCommand<object>
	{
		private PrecompileManagedBinaryCommand() : base("Precompile-ManagedBinary")
		{
		}

		public PrecompileManagedBinaryCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual PrecompileManagedBinaryCommand SetParameters(PrecompileManagedBinaryCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string BinaryName
			{
				set
				{
					base.PowerSharpParameters["BinaryName"] = value;
				}
			}

			public virtual string AppBase
			{
				set
				{
					base.PowerSharpParameters["AppBase"] = value;
				}
			}

			public virtual string Action
			{
				set
				{
					base.PowerSharpParameters["Action"] = value;
				}
			}

			public virtual string Args
			{
				set
				{
					base.PowerSharpParameters["Args"] = value;
				}
			}

			public virtual int Timeout
			{
				set
				{
					base.PowerSharpParameters["Timeout"] = value;
				}
			}

			public virtual int IgnoreExitCode
			{
				set
				{
					base.PowerSharpParameters["IgnoreExitCode"] = value;
				}
			}

			public virtual uint RetryCount
			{
				set
				{
					base.PowerSharpParameters["RetryCount"] = value;
				}
			}

			public virtual uint RetryDelay
			{
				set
				{
					base.PowerSharpParameters["RetryDelay"] = value;
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
