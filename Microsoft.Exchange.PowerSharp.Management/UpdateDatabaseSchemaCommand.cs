using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class UpdateDatabaseSchemaCommand : SyntheticCommandWithPipelineInput<Database, Database>
	{
		private UpdateDatabaseSchemaCommand() : base("Update-DatabaseSchema")
		{
		}

		public UpdateDatabaseSchemaCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual UpdateDatabaseSchemaCommand SetParameters(UpdateDatabaseSchemaCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual UpdateDatabaseSchemaCommand SetParameters(UpdateDatabaseSchemaCommand.VersionsParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual UpdateDatabaseSchemaCommand SetParameters(UpdateDatabaseSchemaCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual DatabaseIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool AllowFileRestore
			{
				set
				{
					base.PowerSharpParameters["AllowFileRestore"] = value;
				}
			}

			public virtual bool BackgroundDatabaseMaintenance
			{
				set
				{
					base.PowerSharpParameters["BackgroundDatabaseMaintenance"] = value;
				}
			}

			public virtual EnhancedTimeSpan DeletedItemRetention
			{
				set
				{
					base.PowerSharpParameters["DeletedItemRetention"] = value;
				}
			}

			public virtual Schedule MaintenanceSchedule
			{
				set
				{
					base.PowerSharpParameters["MaintenanceSchedule"] = value;
				}
			}

			public virtual bool MountAtStartup
			{
				set
				{
					base.PowerSharpParameters["MountAtStartup"] = value;
				}
			}

			public virtual Schedule QuotaNotificationSchedule
			{
				set
				{
					base.PowerSharpParameters["QuotaNotificationSchedule"] = value;
				}
			}

			public virtual bool RetainDeletedItemsUntilBackup
			{
				set
				{
					base.PowerSharpParameters["RetainDeletedItemsUntilBackup"] = value;
				}
			}

			public virtual bool AutoDagExcludeFromMonitoring
			{
				set
				{
					base.PowerSharpParameters["AutoDagExcludeFromMonitoring"] = value;
				}
			}

			public virtual AutoDatabaseMountDial AutoDatabaseMountDial
			{
				set
				{
					base.PowerSharpParameters["AutoDatabaseMountDial"] = value;
				}
			}

			public virtual string DatabaseGroup
			{
				set
				{
					base.PowerSharpParameters["DatabaseGroup"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> IssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["IssueWarningQuota"] = value;
				}
			}

			public virtual EnhancedTimeSpan EventHistoryRetentionPeriod
			{
				set
				{
					base.PowerSharpParameters["EventHistoryRetentionPeriod"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual bool CircularLoggingEnabled
			{
				set
				{
					base.PowerSharpParameters["CircularLoggingEnabled"] = value;
				}
			}

			public virtual DataMoveReplicationConstraintParameter DataMoveReplicationConstraint
			{
				set
				{
					base.PowerSharpParameters["DataMoveReplicationConstraint"] = value;
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

		public class VersionsParameters : ParametersBase
		{
			public virtual DatabaseIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual ushort MajorVersion
			{
				set
				{
					base.PowerSharpParameters["MajorVersion"] = value;
				}
			}

			public virtual ushort MinorVersion
			{
				set
				{
					base.PowerSharpParameters["MinorVersion"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool AllowFileRestore
			{
				set
				{
					base.PowerSharpParameters["AllowFileRestore"] = value;
				}
			}

			public virtual bool BackgroundDatabaseMaintenance
			{
				set
				{
					base.PowerSharpParameters["BackgroundDatabaseMaintenance"] = value;
				}
			}

			public virtual EnhancedTimeSpan DeletedItemRetention
			{
				set
				{
					base.PowerSharpParameters["DeletedItemRetention"] = value;
				}
			}

			public virtual Schedule MaintenanceSchedule
			{
				set
				{
					base.PowerSharpParameters["MaintenanceSchedule"] = value;
				}
			}

			public virtual bool MountAtStartup
			{
				set
				{
					base.PowerSharpParameters["MountAtStartup"] = value;
				}
			}

			public virtual Schedule QuotaNotificationSchedule
			{
				set
				{
					base.PowerSharpParameters["QuotaNotificationSchedule"] = value;
				}
			}

			public virtual bool RetainDeletedItemsUntilBackup
			{
				set
				{
					base.PowerSharpParameters["RetainDeletedItemsUntilBackup"] = value;
				}
			}

			public virtual bool AutoDagExcludeFromMonitoring
			{
				set
				{
					base.PowerSharpParameters["AutoDagExcludeFromMonitoring"] = value;
				}
			}

			public virtual AutoDatabaseMountDial AutoDatabaseMountDial
			{
				set
				{
					base.PowerSharpParameters["AutoDatabaseMountDial"] = value;
				}
			}

			public virtual string DatabaseGroup
			{
				set
				{
					base.PowerSharpParameters["DatabaseGroup"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> IssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["IssueWarningQuota"] = value;
				}
			}

			public virtual EnhancedTimeSpan EventHistoryRetentionPeriod
			{
				set
				{
					base.PowerSharpParameters["EventHistoryRetentionPeriod"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual bool CircularLoggingEnabled
			{
				set
				{
					base.PowerSharpParameters["CircularLoggingEnabled"] = value;
				}
			}

			public virtual DataMoveReplicationConstraintParameter DataMoveReplicationConstraint
			{
				set
				{
					base.PowerSharpParameters["DataMoveReplicationConstraint"] = value;
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
			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool AllowFileRestore
			{
				set
				{
					base.PowerSharpParameters["AllowFileRestore"] = value;
				}
			}

			public virtual bool BackgroundDatabaseMaintenance
			{
				set
				{
					base.PowerSharpParameters["BackgroundDatabaseMaintenance"] = value;
				}
			}

			public virtual EnhancedTimeSpan DeletedItemRetention
			{
				set
				{
					base.PowerSharpParameters["DeletedItemRetention"] = value;
				}
			}

			public virtual Schedule MaintenanceSchedule
			{
				set
				{
					base.PowerSharpParameters["MaintenanceSchedule"] = value;
				}
			}

			public virtual bool MountAtStartup
			{
				set
				{
					base.PowerSharpParameters["MountAtStartup"] = value;
				}
			}

			public virtual Schedule QuotaNotificationSchedule
			{
				set
				{
					base.PowerSharpParameters["QuotaNotificationSchedule"] = value;
				}
			}

			public virtual bool RetainDeletedItemsUntilBackup
			{
				set
				{
					base.PowerSharpParameters["RetainDeletedItemsUntilBackup"] = value;
				}
			}

			public virtual bool AutoDagExcludeFromMonitoring
			{
				set
				{
					base.PowerSharpParameters["AutoDagExcludeFromMonitoring"] = value;
				}
			}

			public virtual AutoDatabaseMountDial AutoDatabaseMountDial
			{
				set
				{
					base.PowerSharpParameters["AutoDatabaseMountDial"] = value;
				}
			}

			public virtual string DatabaseGroup
			{
				set
				{
					base.PowerSharpParameters["DatabaseGroup"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> IssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["IssueWarningQuota"] = value;
				}
			}

			public virtual EnhancedTimeSpan EventHistoryRetentionPeriod
			{
				set
				{
					base.PowerSharpParameters["EventHistoryRetentionPeriod"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual bool CircularLoggingEnabled
			{
				set
				{
					base.PowerSharpParameters["CircularLoggingEnabled"] = value;
				}
			}

			public virtual DataMoveReplicationConstraintParameter DataMoveReplicationConstraint
			{
				set
				{
					base.PowerSharpParameters["DataMoveReplicationConstraint"] = value;
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
