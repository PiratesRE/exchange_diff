using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class ExportUMCallDataRecordCommand : SyntheticCommandWithPipelineInput<MailboxIdParameter, MailboxIdParameter>
	{
		private ExportUMCallDataRecordCommand() : base("Export-UMCallDataRecord")
		{
		}

		public ExportUMCallDataRecordCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual ExportUMCallDataRecordCommand SetParameters(ExportUMCallDataRecordCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string UMDialPlan
			{
				set
				{
					base.PowerSharpParameters["UMDialPlan"] = ((value != null) ? new UMDialPlanIdParameter(value) : null);
				}
			}

			public virtual string UMIPGateway
			{
				set
				{
					base.PowerSharpParameters["UMIPGateway"] = ((value != null) ? new UMIPGatewayIdParameter(value) : null);
				}
			}

			public virtual ExDateTime Date
			{
				set
				{
					base.PowerSharpParameters["Date"] = value;
				}
			}

			public virtual Stream ClientStream
			{
				set
				{
					base.PowerSharpParameters["ClientStream"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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
