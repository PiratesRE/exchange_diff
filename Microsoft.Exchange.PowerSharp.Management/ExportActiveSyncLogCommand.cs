using System;
using System.Management.Automation;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class ExportActiveSyncLogCommand : SyntheticCommandWithPipelineInputNoOutput<string>
	{
		private ExportActiveSyncLogCommand() : base("Export-ActiveSyncLog")
		{
		}

		public ExportActiveSyncLogCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual ExportActiveSyncLogCommand SetParameters(ExportActiveSyncLogCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Filename
			{
				set
				{
					base.PowerSharpParameters["Filename"] = value;
				}
			}

			public virtual DateTime StartDate
			{
				set
				{
					base.PowerSharpParameters["StartDate"] = value;
				}
			}

			public virtual DateTime EndDate
			{
				set
				{
					base.PowerSharpParameters["EndDate"] = value;
				}
			}

			public virtual SwitchParameter UseGMT
			{
				set
				{
					base.PowerSharpParameters["UseGMT"] = value;
				}
			}

			public virtual string OutputPrefix
			{
				set
				{
					base.PowerSharpParameters["OutputPrefix"] = value;
				}
			}

			public virtual string OutputPath
			{
				set
				{
					base.PowerSharpParameters["OutputPath"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
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

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}
	}
}
