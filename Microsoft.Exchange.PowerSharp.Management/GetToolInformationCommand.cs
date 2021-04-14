using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tools;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetToolInformationCommand : SyntheticCommandWithPipelineInputNoOutput<ToolId>
	{
		private GetToolInformationCommand() : base("Get-ToolInformation")
		{
		}

		public GetToolInformationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetToolInformationCommand SetParameters(GetToolInformationCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual ToolId Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual Version Version
			{
				set
				{
					base.PowerSharpParameters["Version"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
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
