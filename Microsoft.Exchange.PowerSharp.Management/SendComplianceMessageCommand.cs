using System;
using System.Management.Automation;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SendComplianceMessageCommand : SyntheticCommandWithPipelineInput<bool, bool>
	{
		private SendComplianceMessageCommand() : base("Send-ComplianceMessage")
		{
		}

		public SendComplianceMessageCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SendComplianceMessageCommand SetParameters(SendComplianceMessageCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual byte SerializedComplianceMessage
			{
				set
				{
					base.PowerSharpParameters["SerializedComplianceMessage"] = value;
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
