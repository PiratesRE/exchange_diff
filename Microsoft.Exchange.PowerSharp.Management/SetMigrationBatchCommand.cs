using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Migration;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetMigrationBatchCommand : SyntheticCommandWithPipelineInputNoOutput<MigrationBatch>
	{
		private SetMigrationBatchCommand() : base("Set-MigrationBatch")
		{
		}

		public SetMigrationBatchCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetMigrationBatchCommand SetParameters(SetMigrationBatchCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetMigrationBatchCommand SetParameters(SetMigrationBatchCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual bool? AllowIncrementalSyncs
			{
				set
				{
					base.PowerSharpParameters["AllowIncrementalSyncs"] = value;
				}
			}

			public virtual int? AutoRetryCount
			{
				set
				{
					base.PowerSharpParameters["AutoRetryCount"] = value;
				}
			}

			public virtual byte CSVData
			{
				set
				{
					base.PowerSharpParameters["CSVData"] = value;
				}
			}

			public virtual bool AllowUnknownColumnsInCsv
			{
				set
				{
					base.PowerSharpParameters["AllowUnknownColumnsInCsv"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> NotificationEmails
			{
				set
				{
					base.PowerSharpParameters["NotificationEmails"] = value;
				}
			}

			public virtual Unlimited<int> BadItemLimit
			{
				set
				{
					base.PowerSharpParameters["BadItemLimit"] = value;
				}
			}

			public virtual Unlimited<int> LargeItemLimit
			{
				set
				{
					base.PowerSharpParameters["LargeItemLimit"] = value;
				}
			}

			public virtual DateTime? StartAfter
			{
				set
				{
					base.PowerSharpParameters["StartAfter"] = value;
				}
			}

			public virtual DateTime? CompleteAfter
			{
				set
				{
					base.PowerSharpParameters["CompleteAfter"] = value;
				}
			}

			public virtual TimeSpan? ReportInterval
			{
				set
				{
					base.PowerSharpParameters["ReportInterval"] = value;
				}
			}

			public virtual bool UseAdvancedValidation
			{
				set
				{
					base.PowerSharpParameters["UseAdvancedValidation"] = value;
				}
			}

			public virtual DatabaseIdParameter SourcePublicFolderDatabase
			{
				set
				{
					base.PowerSharpParameters["SourcePublicFolderDatabase"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string Partition
			{
				set
				{
					base.PowerSharpParameters["Partition"] = ((value != null) ? new MailboxIdParameter(value) : null);
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

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MigrationBatchIdParameter(value) : null);
				}
			}

			public virtual bool? AllowIncrementalSyncs
			{
				set
				{
					base.PowerSharpParameters["AllowIncrementalSyncs"] = value;
				}
			}

			public virtual int? AutoRetryCount
			{
				set
				{
					base.PowerSharpParameters["AutoRetryCount"] = value;
				}
			}

			public virtual byte CSVData
			{
				set
				{
					base.PowerSharpParameters["CSVData"] = value;
				}
			}

			public virtual bool AllowUnknownColumnsInCsv
			{
				set
				{
					base.PowerSharpParameters["AllowUnknownColumnsInCsv"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> NotificationEmails
			{
				set
				{
					base.PowerSharpParameters["NotificationEmails"] = value;
				}
			}

			public virtual Unlimited<int> BadItemLimit
			{
				set
				{
					base.PowerSharpParameters["BadItemLimit"] = value;
				}
			}

			public virtual Unlimited<int> LargeItemLimit
			{
				set
				{
					base.PowerSharpParameters["LargeItemLimit"] = value;
				}
			}

			public virtual DateTime? StartAfter
			{
				set
				{
					base.PowerSharpParameters["StartAfter"] = value;
				}
			}

			public virtual DateTime? CompleteAfter
			{
				set
				{
					base.PowerSharpParameters["CompleteAfter"] = value;
				}
			}

			public virtual TimeSpan? ReportInterval
			{
				set
				{
					base.PowerSharpParameters["ReportInterval"] = value;
				}
			}

			public virtual bool UseAdvancedValidation
			{
				set
				{
					base.PowerSharpParameters["UseAdvancedValidation"] = value;
				}
			}

			public virtual DatabaseIdParameter SourcePublicFolderDatabase
			{
				set
				{
					base.PowerSharpParameters["SourcePublicFolderDatabase"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string Partition
			{
				set
				{
					base.PowerSharpParameters["Partition"] = ((value != null) ? new MailboxIdParameter(value) : null);
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
