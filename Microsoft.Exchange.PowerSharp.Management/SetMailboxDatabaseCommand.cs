using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetMailboxDatabaseCommand : SyntheticCommandWithPipelineInputNoOutput<MailboxDatabase>
	{
		private SetMailboxDatabaseCommand() : base("Set-MailboxDatabase")
		{
		}

		public SetMailboxDatabaseCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetMailboxDatabaseCommand SetParameters(SetMailboxDatabaseCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetMailboxDatabaseCommand SetParameters(SetMailboxDatabaseCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual DatabaseIdParameter PublicFolderDatabase
			{
				set
				{
					base.PowerSharpParameters["PublicFolderDatabase"] = value;
				}
			}

			public virtual OfflineAddressBookIdParameter OfflineAddressBook
			{
				set
				{
					base.PowerSharpParameters["OfflineAddressBook"] = value;
				}
			}

			public virtual string JournalRecipient
			{
				set
				{
					base.PowerSharpParameters["JournalRecipient"] = ((value != null) ? new RecipientIdParameter(value) : null);
				}
			}

			public virtual MailboxProvisioningAttributes MailboxProvisioningAttributes
			{
				set
				{
					base.PowerSharpParameters["MailboxProvisioningAttributes"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual EnhancedTimeSpan MailboxRetention
			{
				set
				{
					base.PowerSharpParameters["MailboxRetention"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ProhibitSendReceiveQuota
			{
				set
				{
					base.PowerSharpParameters["ProhibitSendReceiveQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ProhibitSendQuota
			{
				set
				{
					base.PowerSharpParameters["ProhibitSendQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> RecoverableItemsQuota
			{
				set
				{
					base.PowerSharpParameters["RecoverableItemsQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> RecoverableItemsWarningQuota
			{
				set
				{
					base.PowerSharpParameters["RecoverableItemsWarningQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> CalendarLoggingQuota
			{
				set
				{
					base.PowerSharpParameters["CalendarLoggingQuota"] = value;
				}
			}

			public virtual bool IndexEnabled
			{
				set
				{
					base.PowerSharpParameters["IndexEnabled"] = value;
				}
			}

			public virtual bool IsExcludedFromProvisioning
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromProvisioning"] = value;
				}
			}

			public virtual bool IsExcludedFromProvisioningBySchemaVersionMonitoring
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromProvisioningBySchemaVersionMonitoring"] = value;
				}
			}

			public virtual bool IsExcludedFromInitialProvisioning
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromInitialProvisioning"] = value;
				}
			}

			public virtual bool IsSuspendedFromProvisioning
			{
				set
				{
					base.PowerSharpParameters["IsSuspendedFromProvisioning"] = value;
				}
			}

			public virtual bool IsExcludedFromProvisioningBySpaceMonitoring
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromProvisioningBySpaceMonitoring"] = value;
				}
			}

			public virtual ByteQuantifiedSize? MailboxLoadBalanceMaximumEdbFileSize
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceMaximumEdbFileSize"] = value;
				}
			}

			public virtual int? MailboxLoadBalanceRelativeLoadCapacity
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceRelativeLoadCapacity"] = value;
				}
			}

			public virtual int? MailboxLoadBalanceOverloadedThreshold
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceOverloadedThreshold"] = value;
				}
			}

			public virtual int? MailboxLoadBalanceUnderloadedThreshold
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceUnderloadedThreshold"] = value;
				}
			}

			public virtual bool? MailboxLoadBalanceEnabled
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceEnabled"] = value;
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

		public class IdentityParameters : ParametersBase
		{
			public virtual DatabaseIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual DatabaseIdParameter PublicFolderDatabase
			{
				set
				{
					base.PowerSharpParameters["PublicFolderDatabase"] = value;
				}
			}

			public virtual OfflineAddressBookIdParameter OfflineAddressBook
			{
				set
				{
					base.PowerSharpParameters["OfflineAddressBook"] = value;
				}
			}

			public virtual string JournalRecipient
			{
				set
				{
					base.PowerSharpParameters["JournalRecipient"] = ((value != null) ? new RecipientIdParameter(value) : null);
				}
			}

			public virtual MailboxProvisioningAttributes MailboxProvisioningAttributes
			{
				set
				{
					base.PowerSharpParameters["MailboxProvisioningAttributes"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual EnhancedTimeSpan MailboxRetention
			{
				set
				{
					base.PowerSharpParameters["MailboxRetention"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ProhibitSendReceiveQuota
			{
				set
				{
					base.PowerSharpParameters["ProhibitSendReceiveQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ProhibitSendQuota
			{
				set
				{
					base.PowerSharpParameters["ProhibitSendQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> RecoverableItemsQuota
			{
				set
				{
					base.PowerSharpParameters["RecoverableItemsQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> RecoverableItemsWarningQuota
			{
				set
				{
					base.PowerSharpParameters["RecoverableItemsWarningQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> CalendarLoggingQuota
			{
				set
				{
					base.PowerSharpParameters["CalendarLoggingQuota"] = value;
				}
			}

			public virtual bool IndexEnabled
			{
				set
				{
					base.PowerSharpParameters["IndexEnabled"] = value;
				}
			}

			public virtual bool IsExcludedFromProvisioning
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromProvisioning"] = value;
				}
			}

			public virtual bool IsExcludedFromProvisioningBySchemaVersionMonitoring
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromProvisioningBySchemaVersionMonitoring"] = value;
				}
			}

			public virtual bool IsExcludedFromInitialProvisioning
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromInitialProvisioning"] = value;
				}
			}

			public virtual bool IsSuspendedFromProvisioning
			{
				set
				{
					base.PowerSharpParameters["IsSuspendedFromProvisioning"] = value;
				}
			}

			public virtual bool IsExcludedFromProvisioningBySpaceMonitoring
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromProvisioningBySpaceMonitoring"] = value;
				}
			}

			public virtual ByteQuantifiedSize? MailboxLoadBalanceMaximumEdbFileSize
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceMaximumEdbFileSize"] = value;
				}
			}

			public virtual int? MailboxLoadBalanceRelativeLoadCapacity
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceRelativeLoadCapacity"] = value;
				}
			}

			public virtual int? MailboxLoadBalanceOverloadedThreshold
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceOverloadedThreshold"] = value;
				}
			}

			public virtual int? MailboxLoadBalanceUnderloadedThreshold
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceUnderloadedThreshold"] = value;
				}
			}

			public virtual bool? MailboxLoadBalanceEnabled
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceEnabled"] = value;
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
