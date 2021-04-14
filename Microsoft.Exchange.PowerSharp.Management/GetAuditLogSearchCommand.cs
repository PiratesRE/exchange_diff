using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetAuditLogSearchCommand : SyntheticCommandWithPipelineInput<AuditLogSearchBase, AuditLogSearchBase>
	{
		private GetAuditLogSearchCommand() : base("Get-AuditLogSearch")
		{
		}

		public GetAuditLogSearchCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetAuditLogSearchCommand SetParameters(GetAuditLogSearchCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string Type
			{
				set
				{
					base.PowerSharpParameters["Type"] = value;
				}
			}

			public virtual ExDateTime? CreatedAfter
			{
				set
				{
					base.PowerSharpParameters["CreatedAfter"] = value;
				}
			}

			public virtual ExDateTime? CreatedBefore
			{
				set
				{
					base.PowerSharpParameters["CreatedBefore"] = value;
				}
			}

			public virtual int ResultSize
			{
				set
				{
					base.PowerSharpParameters["ResultSize"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new AuditLogSearchIdParameter(value) : null);
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
