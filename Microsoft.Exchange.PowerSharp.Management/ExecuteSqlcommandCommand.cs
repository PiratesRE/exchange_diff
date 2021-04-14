using System;
using System.Collections;
using System.Management.Automation;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class ExecuteSqlcommandCommand : SyntheticCommandWithPipelineInputNoOutput<string>
	{
		private ExecuteSqlcommandCommand() : base("Execute-Sqlcommand")
		{
		}

		public ExecuteSqlcommandCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual ExecuteSqlcommandCommand SetParameters(ExecuteSqlcommandCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Command
			{
				set
				{
					base.PowerSharpParameters["Command"] = value;
				}
			}

			public virtual SwitchParameter ExecuteScalar
			{
				set
				{
					base.PowerSharpParameters["ExecuteScalar"] = value;
				}
			}

			public virtual SwitchParameter ExecuteScript
			{
				set
				{
					base.PowerSharpParameters["ExecuteScript"] = value;
				}
			}

			public virtual Hashtable Arguments
			{
				set
				{
					base.PowerSharpParameters["Arguments"] = value;
				}
			}

			public virtual int Timeout
			{
				set
				{
					base.PowerSharpParameters["Timeout"] = value;
				}
			}

			public virtual string DatabaseName
			{
				set
				{
					base.PowerSharpParameters["DatabaseName"] = value;
				}
			}

			public virtual string ServerName
			{
				set
				{
					base.PowerSharpParameters["ServerName"] = value;
				}
			}

			public virtual string MirrorServerName
			{
				set
				{
					base.PowerSharpParameters["MirrorServerName"] = value;
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
