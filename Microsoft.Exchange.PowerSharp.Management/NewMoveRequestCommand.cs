using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.RecipientTasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewMoveRequestCommand : SyntheticCommandWithPipelineInput<TransactionalRequestJob, TransactionalRequestJob>
	{
		private NewMoveRequestCommand() : base("New-MoveRequest")
		{
		}

		public NewMoveRequestCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewMoveRequestCommand SetParameters(NewMoveRequestCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMoveRequestCommand SetParameters(NewMoveRequestCommand.MigrationRemoteLegacyParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMoveRequestCommand SetParameters(NewMoveRequestCommand.MigrationLocalParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMoveRequestCommand SetParameters(NewMoveRequestCommand.MigrationRemoteParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMoveRequestCommand SetParameters(NewMoveRequestCommand.MigrationOutboundParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxOrMailUserIdParameter(value) : null);
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

			public virtual SwitchParameter AcceptLargeDataLoss
			{
				set
				{
					base.PowerSharpParameters["AcceptLargeDataLoss"] = value;
				}
			}

			public virtual SwitchParameter AllowLargeItems
			{
				set
				{
					base.PowerSharpParameters["AllowLargeItems"] = value;
				}
			}

			public virtual SwitchParameter CheckInitialProvisioningSetting
			{
				set
				{
					base.PowerSharpParameters["CheckInitialProvisioningSetting"] = value;
				}
			}

			public virtual string BatchName
			{
				set
				{
					base.PowerSharpParameters["BatchName"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual SwitchParameter Protect
			{
				set
				{
					base.PowerSharpParameters["Protect"] = value;
				}
			}

			public virtual SwitchParameter SuspendWhenReadyToComplete
			{
				set
				{
					base.PowerSharpParameters["SuspendWhenReadyToComplete"] = value;
				}
			}

			public virtual SwitchParameter Suspend
			{
				set
				{
					base.PowerSharpParameters["Suspend"] = value;
				}
			}

			public virtual string SuspendComment
			{
				set
				{
					base.PowerSharpParameters["SuspendComment"] = value;
				}
			}

			public virtual SwitchParameter IgnoreRuleLimitErrors
			{
				set
				{
					base.PowerSharpParameters["IgnoreRuleLimitErrors"] = value;
				}
			}

			public virtual RequestPriority Priority
			{
				set
				{
					base.PowerSharpParameters["Priority"] = value;
				}
			}

			public virtual RequestWorkloadType WorkloadType
			{
				set
				{
					base.PowerSharpParameters["WorkloadType"] = value;
				}
			}

			public virtual Unlimited<EnhancedTimeSpan> CompletedRequestAgeLimit
			{
				set
				{
					base.PowerSharpParameters["CompletedRequestAgeLimit"] = value;
				}
			}

			public virtual SwitchParameter ForceOffline
			{
				set
				{
					base.PowerSharpParameters["ForceOffline"] = value;
				}
			}

			public virtual SwitchParameter PreventCompletion
			{
				set
				{
					base.PowerSharpParameters["PreventCompletion"] = value;
				}
			}

			public virtual SkippableMoveComponent SkipMoving
			{
				set
				{
					base.PowerSharpParameters["SkipMoving"] = value;
				}
			}

			public virtual InternalMrsFlag InternalFlags
			{
				set
				{
					base.PowerSharpParameters["InternalFlags"] = value;
				}
			}

			public virtual DateTime StartAfter
			{
				set
				{
					base.PowerSharpParameters["StartAfter"] = value;
				}
			}

			public virtual DateTime CompleteAfter
			{
				set
				{
					base.PowerSharpParameters["CompleteAfter"] = value;
				}
			}

			public virtual TimeSpan IncrementalSyncInterval
			{
				set
				{
					base.PowerSharpParameters["IncrementalSyncInterval"] = value;
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

		public class MigrationRemoteLegacyParameters : ParametersBase
		{
			public virtual DatabaseIdParameter TargetDatabase
			{
				set
				{
					base.PowerSharpParameters["TargetDatabase"] = value;
				}
			}

			public virtual SwitchParameter IgnoreTenantMigrationPolicies
			{
				set
				{
					base.PowerSharpParameters["IgnoreTenantMigrationPolicies"] = value;
				}
			}

			public virtual string RemoteTargetDatabase
			{
				set
				{
					base.PowerSharpParameters["RemoteTargetDatabase"] = value;
				}
			}

			public virtual PSCredential RemoteCredential
			{
				set
				{
					base.PowerSharpParameters["RemoteCredential"] = value;
				}
			}

			public virtual SwitchParameter RemoteLegacy
			{
				set
				{
					base.PowerSharpParameters["RemoteLegacy"] = value;
				}
			}

			public virtual Fqdn RemoteGlobalCatalog
			{
				set
				{
					base.PowerSharpParameters["RemoteGlobalCatalog"] = value;
				}
			}

			public virtual Fqdn TargetDeliveryDomain
			{
				set
				{
					base.PowerSharpParameters["TargetDeliveryDomain"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxOrMailUserIdParameter(value) : null);
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

			public virtual SwitchParameter AcceptLargeDataLoss
			{
				set
				{
					base.PowerSharpParameters["AcceptLargeDataLoss"] = value;
				}
			}

			public virtual SwitchParameter AllowLargeItems
			{
				set
				{
					base.PowerSharpParameters["AllowLargeItems"] = value;
				}
			}

			public virtual SwitchParameter CheckInitialProvisioningSetting
			{
				set
				{
					base.PowerSharpParameters["CheckInitialProvisioningSetting"] = value;
				}
			}

			public virtual string BatchName
			{
				set
				{
					base.PowerSharpParameters["BatchName"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual SwitchParameter Protect
			{
				set
				{
					base.PowerSharpParameters["Protect"] = value;
				}
			}

			public virtual SwitchParameter SuspendWhenReadyToComplete
			{
				set
				{
					base.PowerSharpParameters["SuspendWhenReadyToComplete"] = value;
				}
			}

			public virtual SwitchParameter Suspend
			{
				set
				{
					base.PowerSharpParameters["Suspend"] = value;
				}
			}

			public virtual string SuspendComment
			{
				set
				{
					base.PowerSharpParameters["SuspendComment"] = value;
				}
			}

			public virtual SwitchParameter IgnoreRuleLimitErrors
			{
				set
				{
					base.PowerSharpParameters["IgnoreRuleLimitErrors"] = value;
				}
			}

			public virtual RequestPriority Priority
			{
				set
				{
					base.PowerSharpParameters["Priority"] = value;
				}
			}

			public virtual RequestWorkloadType WorkloadType
			{
				set
				{
					base.PowerSharpParameters["WorkloadType"] = value;
				}
			}

			public virtual Unlimited<EnhancedTimeSpan> CompletedRequestAgeLimit
			{
				set
				{
					base.PowerSharpParameters["CompletedRequestAgeLimit"] = value;
				}
			}

			public virtual SwitchParameter ForceOffline
			{
				set
				{
					base.PowerSharpParameters["ForceOffline"] = value;
				}
			}

			public virtual SwitchParameter PreventCompletion
			{
				set
				{
					base.PowerSharpParameters["PreventCompletion"] = value;
				}
			}

			public virtual SkippableMoveComponent SkipMoving
			{
				set
				{
					base.PowerSharpParameters["SkipMoving"] = value;
				}
			}

			public virtual InternalMrsFlag InternalFlags
			{
				set
				{
					base.PowerSharpParameters["InternalFlags"] = value;
				}
			}

			public virtual DateTime StartAfter
			{
				set
				{
					base.PowerSharpParameters["StartAfter"] = value;
				}
			}

			public virtual DateTime CompleteAfter
			{
				set
				{
					base.PowerSharpParameters["CompleteAfter"] = value;
				}
			}

			public virtual TimeSpan IncrementalSyncInterval
			{
				set
				{
					base.PowerSharpParameters["IncrementalSyncInterval"] = value;
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

		public class MigrationLocalParameters : ParametersBase
		{
			public virtual DatabaseIdParameter TargetDatabase
			{
				set
				{
					base.PowerSharpParameters["TargetDatabase"] = value;
				}
			}

			public virtual DatabaseIdParameter ArchiveTargetDatabase
			{
				set
				{
					base.PowerSharpParameters["ArchiveTargetDatabase"] = value;
				}
			}

			public virtual SwitchParameter PrimaryOnly
			{
				set
				{
					base.PowerSharpParameters["PrimaryOnly"] = value;
				}
			}

			public virtual SwitchParameter ArchiveOnly
			{
				set
				{
					base.PowerSharpParameters["ArchiveOnly"] = value;
				}
			}

			public virtual SwitchParameter DoNotPreserveMailboxSignature
			{
				set
				{
					base.PowerSharpParameters["DoNotPreserveMailboxSignature"] = value;
				}
			}

			public virtual SwitchParameter ForcePull
			{
				set
				{
					base.PowerSharpParameters["ForcePull"] = value;
				}
			}

			public virtual SwitchParameter ForcePush
			{
				set
				{
					base.PowerSharpParameters["ForcePush"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxOrMailUserIdParameter(value) : null);
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

			public virtual SwitchParameter AcceptLargeDataLoss
			{
				set
				{
					base.PowerSharpParameters["AcceptLargeDataLoss"] = value;
				}
			}

			public virtual SwitchParameter AllowLargeItems
			{
				set
				{
					base.PowerSharpParameters["AllowLargeItems"] = value;
				}
			}

			public virtual SwitchParameter CheckInitialProvisioningSetting
			{
				set
				{
					base.PowerSharpParameters["CheckInitialProvisioningSetting"] = value;
				}
			}

			public virtual string BatchName
			{
				set
				{
					base.PowerSharpParameters["BatchName"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual SwitchParameter Protect
			{
				set
				{
					base.PowerSharpParameters["Protect"] = value;
				}
			}

			public virtual SwitchParameter SuspendWhenReadyToComplete
			{
				set
				{
					base.PowerSharpParameters["SuspendWhenReadyToComplete"] = value;
				}
			}

			public virtual SwitchParameter Suspend
			{
				set
				{
					base.PowerSharpParameters["Suspend"] = value;
				}
			}

			public virtual string SuspendComment
			{
				set
				{
					base.PowerSharpParameters["SuspendComment"] = value;
				}
			}

			public virtual SwitchParameter IgnoreRuleLimitErrors
			{
				set
				{
					base.PowerSharpParameters["IgnoreRuleLimitErrors"] = value;
				}
			}

			public virtual RequestPriority Priority
			{
				set
				{
					base.PowerSharpParameters["Priority"] = value;
				}
			}

			public virtual RequestWorkloadType WorkloadType
			{
				set
				{
					base.PowerSharpParameters["WorkloadType"] = value;
				}
			}

			public virtual Unlimited<EnhancedTimeSpan> CompletedRequestAgeLimit
			{
				set
				{
					base.PowerSharpParameters["CompletedRequestAgeLimit"] = value;
				}
			}

			public virtual SwitchParameter ForceOffline
			{
				set
				{
					base.PowerSharpParameters["ForceOffline"] = value;
				}
			}

			public virtual SwitchParameter PreventCompletion
			{
				set
				{
					base.PowerSharpParameters["PreventCompletion"] = value;
				}
			}

			public virtual SkippableMoveComponent SkipMoving
			{
				set
				{
					base.PowerSharpParameters["SkipMoving"] = value;
				}
			}

			public virtual InternalMrsFlag InternalFlags
			{
				set
				{
					base.PowerSharpParameters["InternalFlags"] = value;
				}
			}

			public virtual DateTime StartAfter
			{
				set
				{
					base.PowerSharpParameters["StartAfter"] = value;
				}
			}

			public virtual DateTime CompleteAfter
			{
				set
				{
					base.PowerSharpParameters["CompleteAfter"] = value;
				}
			}

			public virtual TimeSpan IncrementalSyncInterval
			{
				set
				{
					base.PowerSharpParameters["IncrementalSyncInterval"] = value;
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

		public class MigrationRemoteParameters : ParametersBase
		{
			public virtual DatabaseIdParameter TargetDatabase
			{
				set
				{
					base.PowerSharpParameters["TargetDatabase"] = value;
				}
			}

			public virtual DatabaseIdParameter ArchiveTargetDatabase
			{
				set
				{
					base.PowerSharpParameters["ArchiveTargetDatabase"] = value;
				}
			}

			public virtual SwitchParameter PrimaryOnly
			{
				set
				{
					base.PowerSharpParameters["PrimaryOnly"] = value;
				}
			}

			public virtual SwitchParameter ArchiveOnly
			{
				set
				{
					base.PowerSharpParameters["ArchiveOnly"] = value;
				}
			}

			public virtual SwitchParameter IgnoreTenantMigrationPolicies
			{
				set
				{
					base.PowerSharpParameters["IgnoreTenantMigrationPolicies"] = value;
				}
			}

			public virtual Fqdn RemoteHostName
			{
				set
				{
					base.PowerSharpParameters["RemoteHostName"] = value;
				}
			}

			public virtual string RemoteOrganizationName
			{
				set
				{
					base.PowerSharpParameters["RemoteOrganizationName"] = value;
				}
			}

			public virtual string ArchiveDomain
			{
				set
				{
					base.PowerSharpParameters["ArchiveDomain"] = value;
				}
			}

			public virtual PSCredential RemoteCredential
			{
				set
				{
					base.PowerSharpParameters["RemoteCredential"] = value;
				}
			}

			public virtual SwitchParameter Remote
			{
				set
				{
					base.PowerSharpParameters["Remote"] = value;
				}
			}

			public virtual Fqdn RemoteGlobalCatalog
			{
				set
				{
					base.PowerSharpParameters["RemoteGlobalCatalog"] = value;
				}
			}

			public virtual Fqdn TargetDeliveryDomain
			{
				set
				{
					base.PowerSharpParameters["TargetDeliveryDomain"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxOrMailUserIdParameter(value) : null);
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

			public virtual SwitchParameter AcceptLargeDataLoss
			{
				set
				{
					base.PowerSharpParameters["AcceptLargeDataLoss"] = value;
				}
			}

			public virtual SwitchParameter AllowLargeItems
			{
				set
				{
					base.PowerSharpParameters["AllowLargeItems"] = value;
				}
			}

			public virtual SwitchParameter CheckInitialProvisioningSetting
			{
				set
				{
					base.PowerSharpParameters["CheckInitialProvisioningSetting"] = value;
				}
			}

			public virtual string BatchName
			{
				set
				{
					base.PowerSharpParameters["BatchName"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual SwitchParameter Protect
			{
				set
				{
					base.PowerSharpParameters["Protect"] = value;
				}
			}

			public virtual SwitchParameter SuspendWhenReadyToComplete
			{
				set
				{
					base.PowerSharpParameters["SuspendWhenReadyToComplete"] = value;
				}
			}

			public virtual SwitchParameter Suspend
			{
				set
				{
					base.PowerSharpParameters["Suspend"] = value;
				}
			}

			public virtual string SuspendComment
			{
				set
				{
					base.PowerSharpParameters["SuspendComment"] = value;
				}
			}

			public virtual SwitchParameter IgnoreRuleLimitErrors
			{
				set
				{
					base.PowerSharpParameters["IgnoreRuleLimitErrors"] = value;
				}
			}

			public virtual RequestPriority Priority
			{
				set
				{
					base.PowerSharpParameters["Priority"] = value;
				}
			}

			public virtual RequestWorkloadType WorkloadType
			{
				set
				{
					base.PowerSharpParameters["WorkloadType"] = value;
				}
			}

			public virtual Unlimited<EnhancedTimeSpan> CompletedRequestAgeLimit
			{
				set
				{
					base.PowerSharpParameters["CompletedRequestAgeLimit"] = value;
				}
			}

			public virtual SwitchParameter ForceOffline
			{
				set
				{
					base.PowerSharpParameters["ForceOffline"] = value;
				}
			}

			public virtual SwitchParameter PreventCompletion
			{
				set
				{
					base.PowerSharpParameters["PreventCompletion"] = value;
				}
			}

			public virtual SkippableMoveComponent SkipMoving
			{
				set
				{
					base.PowerSharpParameters["SkipMoving"] = value;
				}
			}

			public virtual InternalMrsFlag InternalFlags
			{
				set
				{
					base.PowerSharpParameters["InternalFlags"] = value;
				}
			}

			public virtual DateTime StartAfter
			{
				set
				{
					base.PowerSharpParameters["StartAfter"] = value;
				}
			}

			public virtual DateTime CompleteAfter
			{
				set
				{
					base.PowerSharpParameters["CompleteAfter"] = value;
				}
			}

			public virtual TimeSpan IncrementalSyncInterval
			{
				set
				{
					base.PowerSharpParameters["IncrementalSyncInterval"] = value;
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

		public class MigrationOutboundParameters : ParametersBase
		{
			public virtual SwitchParameter PrimaryOnly
			{
				set
				{
					base.PowerSharpParameters["PrimaryOnly"] = value;
				}
			}

			public virtual SwitchParameter ArchiveOnly
			{
				set
				{
					base.PowerSharpParameters["ArchiveOnly"] = value;
				}
			}

			public virtual SwitchParameter IgnoreTenantMigrationPolicies
			{
				set
				{
					base.PowerSharpParameters["IgnoreTenantMigrationPolicies"] = value;
				}
			}

			public virtual Fqdn RemoteHostName
			{
				set
				{
					base.PowerSharpParameters["RemoteHostName"] = value;
				}
			}

			public virtual string RemoteTargetDatabase
			{
				set
				{
					base.PowerSharpParameters["RemoteTargetDatabase"] = value;
				}
			}

			public virtual string RemoteArchiveTargetDatabase
			{
				set
				{
					base.PowerSharpParameters["RemoteArchiveTargetDatabase"] = value;
				}
			}

			public virtual string RemoteOrganizationName
			{
				set
				{
					base.PowerSharpParameters["RemoteOrganizationName"] = value;
				}
			}

			public virtual string ArchiveDomain
			{
				set
				{
					base.PowerSharpParameters["ArchiveDomain"] = value;
				}
			}

			public virtual PSCredential RemoteCredential
			{
				set
				{
					base.PowerSharpParameters["RemoteCredential"] = value;
				}
			}

			public virtual SwitchParameter Outbound
			{
				set
				{
					base.PowerSharpParameters["Outbound"] = value;
				}
			}

			public virtual Fqdn RemoteGlobalCatalog
			{
				set
				{
					base.PowerSharpParameters["RemoteGlobalCatalog"] = value;
				}
			}

			public virtual Fqdn TargetDeliveryDomain
			{
				set
				{
					base.PowerSharpParameters["TargetDeliveryDomain"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxOrMailUserIdParameter(value) : null);
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

			public virtual SwitchParameter AcceptLargeDataLoss
			{
				set
				{
					base.PowerSharpParameters["AcceptLargeDataLoss"] = value;
				}
			}

			public virtual SwitchParameter AllowLargeItems
			{
				set
				{
					base.PowerSharpParameters["AllowLargeItems"] = value;
				}
			}

			public virtual SwitchParameter CheckInitialProvisioningSetting
			{
				set
				{
					base.PowerSharpParameters["CheckInitialProvisioningSetting"] = value;
				}
			}

			public virtual string BatchName
			{
				set
				{
					base.PowerSharpParameters["BatchName"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual SwitchParameter Protect
			{
				set
				{
					base.PowerSharpParameters["Protect"] = value;
				}
			}

			public virtual SwitchParameter SuspendWhenReadyToComplete
			{
				set
				{
					base.PowerSharpParameters["SuspendWhenReadyToComplete"] = value;
				}
			}

			public virtual SwitchParameter Suspend
			{
				set
				{
					base.PowerSharpParameters["Suspend"] = value;
				}
			}

			public virtual string SuspendComment
			{
				set
				{
					base.PowerSharpParameters["SuspendComment"] = value;
				}
			}

			public virtual SwitchParameter IgnoreRuleLimitErrors
			{
				set
				{
					base.PowerSharpParameters["IgnoreRuleLimitErrors"] = value;
				}
			}

			public virtual RequestPriority Priority
			{
				set
				{
					base.PowerSharpParameters["Priority"] = value;
				}
			}

			public virtual RequestWorkloadType WorkloadType
			{
				set
				{
					base.PowerSharpParameters["WorkloadType"] = value;
				}
			}

			public virtual Unlimited<EnhancedTimeSpan> CompletedRequestAgeLimit
			{
				set
				{
					base.PowerSharpParameters["CompletedRequestAgeLimit"] = value;
				}
			}

			public virtual SwitchParameter ForceOffline
			{
				set
				{
					base.PowerSharpParameters["ForceOffline"] = value;
				}
			}

			public virtual SwitchParameter PreventCompletion
			{
				set
				{
					base.PowerSharpParameters["PreventCompletion"] = value;
				}
			}

			public virtual SkippableMoveComponent SkipMoving
			{
				set
				{
					base.PowerSharpParameters["SkipMoving"] = value;
				}
			}

			public virtual InternalMrsFlag InternalFlags
			{
				set
				{
					base.PowerSharpParameters["InternalFlags"] = value;
				}
			}

			public virtual DateTime StartAfter
			{
				set
				{
					base.PowerSharpParameters["StartAfter"] = value;
				}
			}

			public virtual DateTime CompleteAfter
			{
				set
				{
					base.PowerSharpParameters["CompleteAfter"] = value;
				}
			}

			public virtual TimeSpan IncrementalSyncInterval
			{
				set
				{
					base.PowerSharpParameters["IncrementalSyncInterval"] = value;
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
