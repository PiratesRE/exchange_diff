using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Migration;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewMigrationBatchCommand : SyntheticCommandWithPipelineInput<MigrationBatch, MigrationBatch>
	{
		private NewMigrationBatchCommand() : base("New-MigrationBatch")
		{
		}

		public NewMigrationBatchCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewMigrationBatchCommand SetParameters(NewMigrationBatchCommand.OnboardingParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMigrationBatchCommand SetParameters(NewMigrationBatchCommand.OffboardingParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMigrationBatchCommand SetParameters(NewMigrationBatchCommand.XO1Parameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMigrationBatchCommand SetParameters(NewMigrationBatchCommand.LocalParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMigrationBatchCommand SetParameters(NewMigrationBatchCommand.LocalPublicFolderParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMigrationBatchCommand SetParameters(NewMigrationBatchCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMigrationBatchCommand SetParameters(NewMigrationBatchCommand.PreexistingUserIdsParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMigrationBatchCommand SetParameters(NewMigrationBatchCommand.PreexistingParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class OnboardingParameters : ParametersBase
		{
			public virtual byte CSVData
			{
				set
				{
					base.PowerSharpParameters["CSVData"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ExcludeFolders
			{
				set
				{
					base.PowerSharpParameters["ExcludeFolders"] = value;
				}
			}

			public virtual SwitchParameter DisallowExistingUsers
			{
				set
				{
					base.PowerSharpParameters["DisallowExistingUsers"] = value;
				}
			}

			public virtual SwitchParameter ArchiveOnly
			{
				set
				{
					base.PowerSharpParameters["ArchiveOnly"] = value;
				}
			}

			public virtual SwitchParameter PrimaryOnly
			{
				set
				{
					base.PowerSharpParameters["PrimaryOnly"] = value;
				}
			}

			public virtual string SourceEndpoint
			{
				set
				{
					base.PowerSharpParameters["SourceEndpoint"] = ((value != null) ? new MigrationEndpointIdParameter(value) : null);
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

			public virtual MultiValuedProperty<string> TargetArchiveDatabases
			{
				set
				{
					base.PowerSharpParameters["TargetArchiveDatabases"] = value;
				}
			}

			public virtual MultiValuedProperty<string> TargetDatabases
			{
				set
				{
					base.PowerSharpParameters["TargetDatabases"] = value;
				}
			}

			public virtual string TargetDeliveryDomain
			{
				set
				{
					base.PowerSharpParameters["TargetDeliveryDomain"] = value;
				}
			}

			public virtual bool AllowUnknownColumnsInCsv
			{
				set
				{
					base.PowerSharpParameters["AllowUnknownColumnsInCsv"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual bool? AllowIncrementalSyncs
			{
				set
				{
					base.PowerSharpParameters["AllowIncrementalSyncs"] = value;
				}
			}

			public virtual ExTimeZoneValue TimeZone
			{
				set
				{
					base.PowerSharpParameters["TimeZone"] = value;
				}
			}

			public virtual SkippableMigrationSteps SkipSteps
			{
				set
				{
					base.PowerSharpParameters["SkipSteps"] = value;
				}
			}

			public virtual CultureInfo Locale
			{
				set
				{
					base.PowerSharpParameters["Locale"] = value;
				}
			}

			public virtual int? AutoRetryCount
			{
				set
				{
					base.PowerSharpParameters["AutoRetryCount"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> NotificationEmails
			{
				set
				{
					base.PowerSharpParameters["NotificationEmails"] = value;
				}
			}

			public virtual SwitchParameter AutoStart
			{
				set
				{
					base.PowerSharpParameters["AutoStart"] = value;
				}
			}

			public virtual SwitchParameter AutoComplete
			{
				set
				{
					base.PowerSharpParameters["AutoComplete"] = value;
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

		public class OffboardingParameters : ParametersBase
		{
			public virtual byte CSVData
			{
				set
				{
					base.PowerSharpParameters["CSVData"] = value;
				}
			}

			public virtual SwitchParameter DisallowExistingUsers
			{
				set
				{
					base.PowerSharpParameters["DisallowExistingUsers"] = value;
				}
			}

			public virtual SwitchParameter ArchiveOnly
			{
				set
				{
					base.PowerSharpParameters["ArchiveOnly"] = value;
				}
			}

			public virtual SwitchParameter PrimaryOnly
			{
				set
				{
					base.PowerSharpParameters["PrimaryOnly"] = value;
				}
			}

			public virtual string TargetEndpoint
			{
				set
				{
					base.PowerSharpParameters["TargetEndpoint"] = ((value != null) ? new MigrationEndpointIdParameter(value) : null);
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

			public virtual MultiValuedProperty<string> TargetArchiveDatabases
			{
				set
				{
					base.PowerSharpParameters["TargetArchiveDatabases"] = value;
				}
			}

			public virtual MultiValuedProperty<string> TargetDatabases
			{
				set
				{
					base.PowerSharpParameters["TargetDatabases"] = value;
				}
			}

			public virtual string TargetDeliveryDomain
			{
				set
				{
					base.PowerSharpParameters["TargetDeliveryDomain"] = value;
				}
			}

			public virtual bool AllowUnknownColumnsInCsv
			{
				set
				{
					base.PowerSharpParameters["AllowUnknownColumnsInCsv"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual bool? AllowIncrementalSyncs
			{
				set
				{
					base.PowerSharpParameters["AllowIncrementalSyncs"] = value;
				}
			}

			public virtual ExTimeZoneValue TimeZone
			{
				set
				{
					base.PowerSharpParameters["TimeZone"] = value;
				}
			}

			public virtual SkippableMigrationSteps SkipSteps
			{
				set
				{
					base.PowerSharpParameters["SkipSteps"] = value;
				}
			}

			public virtual CultureInfo Locale
			{
				set
				{
					base.PowerSharpParameters["Locale"] = value;
				}
			}

			public virtual int? AutoRetryCount
			{
				set
				{
					base.PowerSharpParameters["AutoRetryCount"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> NotificationEmails
			{
				set
				{
					base.PowerSharpParameters["NotificationEmails"] = value;
				}
			}

			public virtual SwitchParameter AutoStart
			{
				set
				{
					base.PowerSharpParameters["AutoStart"] = value;
				}
			}

			public virtual SwitchParameter AutoComplete
			{
				set
				{
					base.PowerSharpParameters["AutoComplete"] = value;
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

		public class XO1Parameters : ParametersBase
		{
			public virtual byte CSVData
			{
				set
				{
					base.PowerSharpParameters["CSVData"] = value;
				}
			}

			public virtual SwitchParameter XO1
			{
				set
				{
					base.PowerSharpParameters["XO1"] = value;
				}
			}

			public virtual bool AllowUnknownColumnsInCsv
			{
				set
				{
					base.PowerSharpParameters["AllowUnknownColumnsInCsv"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual bool? AllowIncrementalSyncs
			{
				set
				{
					base.PowerSharpParameters["AllowIncrementalSyncs"] = value;
				}
			}

			public virtual ExTimeZoneValue TimeZone
			{
				set
				{
					base.PowerSharpParameters["TimeZone"] = value;
				}
			}

			public virtual SkippableMigrationSteps SkipSteps
			{
				set
				{
					base.PowerSharpParameters["SkipSteps"] = value;
				}
			}

			public virtual CultureInfo Locale
			{
				set
				{
					base.PowerSharpParameters["Locale"] = value;
				}
			}

			public virtual int? AutoRetryCount
			{
				set
				{
					base.PowerSharpParameters["AutoRetryCount"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> NotificationEmails
			{
				set
				{
					base.PowerSharpParameters["NotificationEmails"] = value;
				}
			}

			public virtual SwitchParameter AutoStart
			{
				set
				{
					base.PowerSharpParameters["AutoStart"] = value;
				}
			}

			public virtual SwitchParameter AutoComplete
			{
				set
				{
					base.PowerSharpParameters["AutoComplete"] = value;
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

		public class LocalParameters : ParametersBase
		{
			public virtual byte CSVData
			{
				set
				{
					base.PowerSharpParameters["CSVData"] = value;
				}
			}

			public virtual SwitchParameter DisallowExistingUsers
			{
				set
				{
					base.PowerSharpParameters["DisallowExistingUsers"] = value;
				}
			}

			public virtual SwitchParameter ArchiveOnly
			{
				set
				{
					base.PowerSharpParameters["ArchiveOnly"] = value;
				}
			}

			public virtual SwitchParameter Local
			{
				set
				{
					base.PowerSharpParameters["Local"] = value;
				}
			}

			public virtual SwitchParameter PrimaryOnly
			{
				set
				{
					base.PowerSharpParameters["PrimaryOnly"] = value;
				}
			}

			public virtual Unlimited<int> BadItemLimit
			{
				set
				{
					base.PowerSharpParameters["BadItemLimit"] = value;
				}
			}

			public virtual MultiValuedProperty<string> TargetArchiveDatabases
			{
				set
				{
					base.PowerSharpParameters["TargetArchiveDatabases"] = value;
				}
			}

			public virtual MultiValuedProperty<string> TargetDatabases
			{
				set
				{
					base.PowerSharpParameters["TargetDatabases"] = value;
				}
			}

			public virtual bool AllowUnknownColumnsInCsv
			{
				set
				{
					base.PowerSharpParameters["AllowUnknownColumnsInCsv"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual bool? AllowIncrementalSyncs
			{
				set
				{
					base.PowerSharpParameters["AllowIncrementalSyncs"] = value;
				}
			}

			public virtual ExTimeZoneValue TimeZone
			{
				set
				{
					base.PowerSharpParameters["TimeZone"] = value;
				}
			}

			public virtual SkippableMigrationSteps SkipSteps
			{
				set
				{
					base.PowerSharpParameters["SkipSteps"] = value;
				}
			}

			public virtual CultureInfo Locale
			{
				set
				{
					base.PowerSharpParameters["Locale"] = value;
				}
			}

			public virtual int? AutoRetryCount
			{
				set
				{
					base.PowerSharpParameters["AutoRetryCount"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> NotificationEmails
			{
				set
				{
					base.PowerSharpParameters["NotificationEmails"] = value;
				}
			}

			public virtual SwitchParameter AutoStart
			{
				set
				{
					base.PowerSharpParameters["AutoStart"] = value;
				}
			}

			public virtual SwitchParameter AutoComplete
			{
				set
				{
					base.PowerSharpParameters["AutoComplete"] = value;
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

		public class LocalPublicFolderParameters : ParametersBase
		{
			public virtual byte CSVData
			{
				set
				{
					base.PowerSharpParameters["CSVData"] = value;
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

			public virtual DatabaseIdParameter SourcePublicFolderDatabase
			{
				set
				{
					base.PowerSharpParameters["SourcePublicFolderDatabase"] = value;
				}
			}

			public virtual bool AllowUnknownColumnsInCsv
			{
				set
				{
					base.PowerSharpParameters["AllowUnknownColumnsInCsv"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual bool? AllowIncrementalSyncs
			{
				set
				{
					base.PowerSharpParameters["AllowIncrementalSyncs"] = value;
				}
			}

			public virtual ExTimeZoneValue TimeZone
			{
				set
				{
					base.PowerSharpParameters["TimeZone"] = value;
				}
			}

			public virtual SkippableMigrationSteps SkipSteps
			{
				set
				{
					base.PowerSharpParameters["SkipSteps"] = value;
				}
			}

			public virtual CultureInfo Locale
			{
				set
				{
					base.PowerSharpParameters["Locale"] = value;
				}
			}

			public virtual int? AutoRetryCount
			{
				set
				{
					base.PowerSharpParameters["AutoRetryCount"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> NotificationEmails
			{
				set
				{
					base.PowerSharpParameters["NotificationEmails"] = value;
				}
			}

			public virtual SwitchParameter AutoStart
			{
				set
				{
					base.PowerSharpParameters["AutoStart"] = value;
				}
			}

			public virtual SwitchParameter AutoComplete
			{
				set
				{
					base.PowerSharpParameters["AutoComplete"] = value;
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

		public class DefaultParameters : ParametersBase
		{
			public virtual bool AllowUnknownColumnsInCsv
			{
				set
				{
					base.PowerSharpParameters["AllowUnknownColumnsInCsv"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual bool? AllowIncrementalSyncs
			{
				set
				{
					base.PowerSharpParameters["AllowIncrementalSyncs"] = value;
				}
			}

			public virtual ExTimeZoneValue TimeZone
			{
				set
				{
					base.PowerSharpParameters["TimeZone"] = value;
				}
			}

			public virtual SkippableMigrationSteps SkipSteps
			{
				set
				{
					base.PowerSharpParameters["SkipSteps"] = value;
				}
			}

			public virtual CultureInfo Locale
			{
				set
				{
					base.PowerSharpParameters["Locale"] = value;
				}
			}

			public virtual int? AutoRetryCount
			{
				set
				{
					base.PowerSharpParameters["AutoRetryCount"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> NotificationEmails
			{
				set
				{
					base.PowerSharpParameters["NotificationEmails"] = value;
				}
			}

			public virtual SwitchParameter AutoStart
			{
				set
				{
					base.PowerSharpParameters["AutoStart"] = value;
				}
			}

			public virtual SwitchParameter AutoComplete
			{
				set
				{
					base.PowerSharpParameters["AutoComplete"] = value;
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

		public class PreexistingUserIdsParameters : ParametersBase
		{
			public virtual SwitchParameter DisableOnCopy
			{
				set
				{
					base.PowerSharpParameters["DisableOnCopy"] = value;
				}
			}

			public virtual MultiValuedProperty<MigrationUserIdParameter> UserIds
			{
				set
				{
					base.PowerSharpParameters["UserIds"] = value;
				}
			}

			public virtual bool AllowUnknownColumnsInCsv
			{
				set
				{
					base.PowerSharpParameters["AllowUnknownColumnsInCsv"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual bool? AllowIncrementalSyncs
			{
				set
				{
					base.PowerSharpParameters["AllowIncrementalSyncs"] = value;
				}
			}

			public virtual ExTimeZoneValue TimeZone
			{
				set
				{
					base.PowerSharpParameters["TimeZone"] = value;
				}
			}

			public virtual SkippableMigrationSteps SkipSteps
			{
				set
				{
					base.PowerSharpParameters["SkipSteps"] = value;
				}
			}

			public virtual CultureInfo Locale
			{
				set
				{
					base.PowerSharpParameters["Locale"] = value;
				}
			}

			public virtual int? AutoRetryCount
			{
				set
				{
					base.PowerSharpParameters["AutoRetryCount"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> NotificationEmails
			{
				set
				{
					base.PowerSharpParameters["NotificationEmails"] = value;
				}
			}

			public virtual SwitchParameter AutoStart
			{
				set
				{
					base.PowerSharpParameters["AutoStart"] = value;
				}
			}

			public virtual SwitchParameter AutoComplete
			{
				set
				{
					base.PowerSharpParameters["AutoComplete"] = value;
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

		public class PreexistingParameters : ParametersBase
		{
			public virtual SwitchParameter DisableOnCopy
			{
				set
				{
					base.PowerSharpParameters["DisableOnCopy"] = value;
				}
			}

			public virtual MultiValuedProperty<MigrationUser> Users
			{
				set
				{
					base.PowerSharpParameters["Users"] = value;
				}
			}

			public virtual bool AllowUnknownColumnsInCsv
			{
				set
				{
					base.PowerSharpParameters["AllowUnknownColumnsInCsv"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual bool? AllowIncrementalSyncs
			{
				set
				{
					base.PowerSharpParameters["AllowIncrementalSyncs"] = value;
				}
			}

			public virtual ExTimeZoneValue TimeZone
			{
				set
				{
					base.PowerSharpParameters["TimeZone"] = value;
				}
			}

			public virtual SkippableMigrationSteps SkipSteps
			{
				set
				{
					base.PowerSharpParameters["SkipSteps"] = value;
				}
			}

			public virtual CultureInfo Locale
			{
				set
				{
					base.PowerSharpParameters["Locale"] = value;
				}
			}

			public virtual int? AutoRetryCount
			{
				set
				{
					base.PowerSharpParameters["AutoRetryCount"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> NotificationEmails
			{
				set
				{
					base.PowerSharpParameters["NotificationEmails"] = value;
				}
			}

			public virtual SwitchParameter AutoStart
			{
				set
				{
					base.PowerSharpParameters["AutoStart"] = value;
				}
			}

			public virtual SwitchParameter AutoComplete
			{
				set
				{
					base.PowerSharpParameters["AutoComplete"] = value;
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
