using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetAdminAuditLogConfigCommand : SyntheticCommandWithPipelineInputNoOutput<AdminAuditLogConfig>
	{
		private SetAdminAuditLogConfigCommand() : base("Set-AdminAuditLogConfig")
		{
		}

		public SetAdminAuditLogConfigCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetAdminAuditLogConfigCommand SetParameters(SetAdminAuditLogConfigCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetAdminAuditLogConfigCommand SetParameters(SetAdminAuditLogConfigCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter IgnoreDehydratedFlag
			{
				set
				{
					base.PowerSharpParameters["IgnoreDehydratedFlag"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool AdminAuditLogEnabled
			{
				set
				{
					base.PowerSharpParameters["AdminAuditLogEnabled"] = value;
				}
			}

			public virtual AuditLogLevel LogLevel
			{
				set
				{
					base.PowerSharpParameters["LogLevel"] = value;
				}
			}

			public virtual bool TestCmdletLoggingEnabled
			{
				set
				{
					base.PowerSharpParameters["TestCmdletLoggingEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AdminAuditLogCmdlets
			{
				set
				{
					base.PowerSharpParameters["AdminAuditLogCmdlets"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AdminAuditLogParameters
			{
				set
				{
					base.PowerSharpParameters["AdminAuditLogParameters"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AdminAuditLogExcludedCmdlets
			{
				set
				{
					base.PowerSharpParameters["AdminAuditLogExcludedCmdlets"] = value;
				}
			}

			public virtual EnhancedTimeSpan AdminAuditLogAgeLimit
			{
				set
				{
					base.PowerSharpParameters["AdminAuditLogAgeLimit"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter IgnoreDehydratedFlag
			{
				set
				{
					base.PowerSharpParameters["IgnoreDehydratedFlag"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool AdminAuditLogEnabled
			{
				set
				{
					base.PowerSharpParameters["AdminAuditLogEnabled"] = value;
				}
			}

			public virtual AuditLogLevel LogLevel
			{
				set
				{
					base.PowerSharpParameters["LogLevel"] = value;
				}
			}

			public virtual bool TestCmdletLoggingEnabled
			{
				set
				{
					base.PowerSharpParameters["TestCmdletLoggingEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AdminAuditLogCmdlets
			{
				set
				{
					base.PowerSharpParameters["AdminAuditLogCmdlets"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AdminAuditLogParameters
			{
				set
				{
					base.PowerSharpParameters["AdminAuditLogParameters"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AdminAuditLogExcludedCmdlets
			{
				set
				{
					base.PowerSharpParameters["AdminAuditLogExcludedCmdlets"] = value;
				}
			}

			public virtual EnhancedTimeSpan AdminAuditLogAgeLimit
			{
				set
				{
					base.PowerSharpParameters["AdminAuditLogAgeLimit"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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
