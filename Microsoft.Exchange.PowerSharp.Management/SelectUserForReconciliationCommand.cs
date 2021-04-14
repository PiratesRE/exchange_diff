using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SelectUserForReconciliationCommand : SyntheticCommandWithPipelineInputNoOutput<ADObjectId>
	{
		private SelectUserForReconciliationCommand() : base("Select-UserForReconciliation")
		{
		}

		public SelectUserForReconciliationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SelectUserForReconciliationCommand SetParameters(SelectUserForReconciliationCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual ADObjectId User
			{
				set
				{
					base.PowerSharpParameters["User"] = value;
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
