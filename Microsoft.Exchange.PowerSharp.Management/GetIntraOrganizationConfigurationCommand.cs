using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetIntraOrganizationConfigurationCommand : SyntheticCommandWithPipelineInput<Server, Server>
	{
		private GetIntraOrganizationConfigurationCommand() : base("Get-IntraOrganizationConfiguration")
		{
		}

		public GetIntraOrganizationConfigurationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetIntraOrganizationConfigurationCommand SetParameters(GetIntraOrganizationConfigurationCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual OnPremisesOrganizationIdParameter OrganizationGuid
			{
				set
				{
					base.PowerSharpParameters["OrganizationGuid"] = value;
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
