using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Management.Tasks.UM;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "MoveRequest", SupportsShouldProcess = true, DefaultParameterSetName = "MigrationLocal")]
	public sealed class NewMoveRequest : NewTaskBase<TransactionalRequestJob>
	{
		public NewMoveRequest()
		{
			this.mrsClient = null;
			this.adSession = null;
			this.mrProvider = null;
			this.BadItemLimit = 0;
			this.specifiedTargetMDB = null;
			this.specifiedArchiveTargetMDB = null;
			this.targetDatabaseForMailbox = null;
			this.targetDatabaseForMailboxArchive = null;
			this.sourceMbxInfo = null;
			this.sourceArchiveInfo = null;
			this.targetMbxInfo = null;
			this.targetArchiveInfo = null;
			this.adUser = null;
			this.moveFlags = RequestFlags.None;
			this.unreachableMrsServers = new List<string>();
			TestIntegration.Instance.ForceRefresh();
			this.globalConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromRootOrgScopeSet(), 255, ".ctor", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\MoveRequest\\NewMoveRequest.cs");
		}

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public MailboxOrMailUserIdParameter Identity
		{
			get
			{
				return (MailboxOrMailUserIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationRemoteLegacy")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocal")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationRemote")]
		public DatabaseIdParameter TargetDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["TargetDatabase"];
			}
			set
			{
				base.Fields["TargetDatabase"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationRemote")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocal")]
		public DatabaseIdParameter ArchiveTargetDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["ArchiveTargetDatabase"];
			}
			set
			{
				base.Fields["ArchiveTargetDatabase"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationRemote")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutbound")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocal")]
		public SwitchParameter PrimaryOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["PrimaryOnly"] ?? false);
			}
			set
			{
				base.Fields["PrimaryOnly"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutbound")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationRemote")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocal")]
		public SwitchParameter ArchiveOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["ArchiveOnly"] ?? false);
			}
			set
			{
				base.Fields["ArchiveOnly"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> BadItemLimit
		{
			get
			{
				return (Unlimited<int>)(base.Fields["BadItemLimit"] ?? RequestTaskHelper.UnlimitedZero);
			}
			set
			{
				base.Fields["BadItemLimit"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> LargeItemLimit
		{
			get
			{
				return (Unlimited<int>)(base.Fields["LargeItemLimit"] ?? RequestTaskHelper.UnlimitedZero);
			}
			set
			{
				base.Fields["LargeItemLimit"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AcceptLargeDataLoss
		{
			get
			{
				return (SwitchParameter)(base.Fields["AcceptLargeDataLoss"] ?? false);
			}
			set
			{
				base.Fields["AcceptLargeDataLoss"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AllowLargeItems
		{
			get
			{
				bool value = this.MoveRequestIs(RequestFlags.IntraOrg);
				return (SwitchParameter)(base.Fields["AllowLargeItems"] ?? value);
			}
			set
			{
				base.Fields["AllowLargeItems"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationRemote")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationRemoteLegacy")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutbound")]
		public SwitchParameter IgnoreTenantMigrationPolicies
		{
			get
			{
				return (SwitchParameter)(base.Fields["IgnoreTenantMigrationPolicies"] ?? false);
			}
			set
			{
				base.Fields["IgnoreTenantMigrationPolicies"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter CheckInitialProvisioningSetting
		{
			get
			{
				return (SwitchParameter)(base.Fields["CheckInitialProvisioningSetting"] ?? false);
			}
			set
			{
				base.Fields["CheckInitialProvisioningSetting"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MigrationOutbound")]
		[Parameter(Mandatory = true, ParameterSetName = "MigrationRemote")]
		public Fqdn RemoteHostName
		{
			get
			{
				return (Fqdn)base.Fields["RemoteHostName"];
			}
			set
			{
				base.Fields["RemoteHostName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutbound")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationRemoteLegacy")]
		public string RemoteTargetDatabase
		{
			get
			{
				return (string)base.Fields["RemoteTargetDatabase"];
			}
			set
			{
				base.Fields["RemoteTargetDatabase"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutbound")]
		public string RemoteArchiveTargetDatabase
		{
			get
			{
				return (string)base.Fields["RemoteArchiveTargetDatabase"];
			}
			set
			{
				base.Fields["RemoteArchiveTargetDatabase"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationRemote")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutbound")]
		public string RemoteOrganizationName
		{
			get
			{
				return (string)base.Fields["RemoteOrganizationName"];
			}
			set
			{
				base.Fields["RemoteOrganizationName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationRemote")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutbound")]
		public string ArchiveDomain
		{
			get
			{
				return (string)base.Fields["ArchiveDomain"];
			}
			set
			{
				base.Fields["ArchiveDomain"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string BatchName
		{
			get
			{
				return (string)base.Fields["BatchName"];
			}
			set
			{
				base.Fields["BatchName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutbound")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationRemote")]
		[Parameter(Mandatory = true, ParameterSetName = "MigrationRemoteLegacy")]
		public PSCredential RemoteCredential
		{
			get
			{
				return (PSCredential)base.Fields["RemoteCredential"];
			}
			set
			{
				base.Fields["RemoteCredential"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MigrationRemote")]
		public SwitchParameter Remote
		{
			get
			{
				return (SwitchParameter)(base.Fields["Remote"] ?? false);
			}
			set
			{
				base.Fields["Remote"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MigrationOutbound")]
		public SwitchParameter Outbound
		{
			get
			{
				return (SwitchParameter)(base.Fields["Outbound"] ?? false);
			}
			set
			{
				base.Fields["Outbound"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MigrationRemoteLegacy")]
		public SwitchParameter RemoteLegacy
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoteLegacy"] ?? false);
			}
			set
			{
				base.Fields["RemoteLegacy"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutbound")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationRemote")]
		[Parameter(Mandatory = true, ParameterSetName = "MigrationRemoteLegacy")]
		public Fqdn RemoteGlobalCatalog
		{
			get
			{
				return (Fqdn)base.Fields["RemoteGlobalCatalog"];
			}
			set
			{
				base.Fields["RemoteGlobalCatalog"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationRemote")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationRemoteLegacy")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutbound")]
		public Fqdn TargetDeliveryDomain
		{
			get
			{
				return (Fqdn)base.Fields["TargetDeliveryDomain"];
			}
			set
			{
				base.Fields["TargetDeliveryDomain"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Protect
		{
			get
			{
				return (SwitchParameter)(base.Fields["Protect"] ?? false);
			}
			set
			{
				base.Fields["Protect"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SuspendWhenReadyToComplete
		{
			get
			{
				return (SwitchParameter)(base.Fields["SuspendWhenReadyToComplete"] ?? false);
			}
			set
			{
				base.Fields["SuspendWhenReadyToComplete"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Suspend
		{
			get
			{
				return (SwitchParameter)(base.Fields["Suspend"] ?? false);
			}
			set
			{
				base.Fields["Suspend"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SuspendComment
		{
			get
			{
				return (string)base.Fields["SuspendComment"];
			}
			set
			{
				base.Fields["SuspendComment"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IgnoreRuleLimitErrors
		{
			get
			{
				return (SwitchParameter)(base.Fields["IgnoreRuleLimitErrors"] ?? false);
			}
			set
			{
				base.Fields["IgnoreRuleLimitErrors"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocal")]
		public SwitchParameter DoNotPreserveMailboxSignature
		{
			get
			{
				return (SwitchParameter)(base.Fields["DoNotPreserveMailboxSignature"] ?? false);
			}
			set
			{
				base.Fields["DoNotPreserveMailboxSignature"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RequestPriority Priority
		{
			get
			{
				return (RequestPriority)(base.Fields["Priority"] ?? RequestPriority.Normal);
			}
			set
			{
				base.Fields["Priority"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RequestWorkloadType WorkloadType
		{
			get
			{
				return (RequestWorkloadType)(base.Fields["WorkloadType"] ?? RequestWorkloadType.None);
			}
			set
			{
				base.Fields["WorkloadType"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<EnhancedTimeSpan> CompletedRequestAgeLimit
		{
			get
			{
				return (Unlimited<EnhancedTimeSpan>)(base.Fields["CompletedRequestAgeLimit"] ?? RequestTaskHelper.DefaultCompletedRequestAgeLimit);
			}
			set
			{
				base.Fields["CompletedRequestAgeLimit"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ForceOffline
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForceOffline"] ?? false);
			}
			set
			{
				base.Fields["ForceOffline"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter PreventCompletion
		{
			get
			{
				return (SwitchParameter)(base.Fields["PreventCompletion"] ?? false);
			}
			set
			{
				base.Fields["PreventCompletion"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SkippableMoveComponent[] SkipMoving
		{
			get
			{
				return (SkippableMoveComponent[])(base.Fields["SkipMoving"] ?? null);
			}
			set
			{
				base.Fields["SkipMoving"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public InternalMrsFlag[] InternalFlags
		{
			get
			{
				return (InternalMrsFlag[])(base.Fields["InternalFlags"] ?? null);
			}
			set
			{
				base.Fields["InternalFlags"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime StartAfter
		{
			get
			{
				return (DateTime)base.Fields["StartAfter"];
			}
			set
			{
				base.Fields["StartAfter"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime CompleteAfter
		{
			get
			{
				return (DateTime)base.Fields["CompleteAfter"];
			}
			set
			{
				base.Fields["CompleteAfter"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TimeSpan IncrementalSyncInterval
		{
			get
			{
				return (TimeSpan)(base.Fields["IncrementalSyncInterval"] ?? NewMoveRequest.defaultIncrementalSyncIntervalForMove);
			}
			set
			{
				base.Fields["IncrementalSyncInterval"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocal")]
		public SwitchParameter ForcePull
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForcePull"] ?? false);
			}
			set
			{
				base.Fields["ForcePull"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocal")]
		public SwitchParameter ForcePush
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForcePush"] ?? false);
			}
			set
			{
				base.Fields["ForcePush"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Microsoft.Exchange.Management.Tasks.Strings.ConfirmationMessageNewMoveRequest(this.adUser.Id.ToString());
			}
		}

		private bool PrimaryIsMoving
		{
			get
			{
				return !this.MoveRequestIs(RequestFlags.MoveOnlyArchiveMailbox);
			}
		}

		private bool ArchiveIsMoving
		{
			get
			{
				return this.MoveRequestIs(RequestFlags.MoveOnlyArchiveMailbox) || (!this.MoveRequestIs(RequestFlags.MoveOnlyPrimaryMailbox) && this.SourceHasArchive);
			}
		}

		private bool SourceIsRemote
		{
			get
			{
				return this.MoveRequestIs(RequestFlags.CrossOrg | RequestFlags.Pull);
			}
		}

		private bool SourceIsLocal
		{
			get
			{
				return !this.SourceIsRemote;
			}
		}

		private bool TargetIsRemote
		{
			get
			{
				return this.MoveRequestIs(RequestFlags.CrossOrg | RequestFlags.Push);
			}
		}

		private bool TargetIsLocal
		{
			get
			{
				return !this.TargetIsRemote;
			}
		}

		private ADUser SourceUser
		{
			get
			{
				if (!this.SourceIsRemote)
				{
					return this.adUser;
				}
				return this.remoteADUser;
			}
		}

		private ADUser TargetUser
		{
			get
			{
				if (!this.TargetIsRemote)
				{
					return this.adUser;
				}
				return this.remoteADUser;
			}
		}

		private bool MailboxHasArchive
		{
			get
			{
				return this.adUser.ArchiveGuid != Guid.Empty;
			}
		}

		private bool SourceHasPrimary
		{
			get
			{
				return this.SourceUser.Database != null;
			}
		}

		private bool SourceHasArchive
		{
			get
			{
				return this.SourceUser.HasLocalArchive;
			}
		}

		private bool TargetHasPrimary
		{
			get
			{
				return this.TargetUser.Database != null;
			}
		}

		private bool TargetHasArchive
		{
			get
			{
				return this.TargetUser.HasLocalArchive;
			}
		}

		private bool IsSplitPrimaryAndArchiveScenario
		{
			get
			{
				return this.PrimaryOnly || this.ArchiveOnly || (this.MailboxHasArchive && (!this.SourceHasArchive || this.sourceMbxInfo.MdbGuid != this.sourceArchiveInfo.MdbGuid || this.targetMbxInfo.MdbGuid != this.targetArchiveInfo.MdbGuid));
			}
		}

		private string ExecutingUserIdentity
		{
			get
			{
				return base.ExecutingUserIdentityName;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.mrProvider != null)
				{
					this.mrProvider.Dispose();
					this.mrProvider = null;
				}
				if (this.mrsClient != null)
				{
					this.mrsClient.Dispose();
					this.mrsClient = null;
				}
			}
			base.Dispose(disposing);
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (base.ParameterSetName.Equals("MigrationRemote"))
			{
				this.moveFlags |= (RequestFlags.CrossOrg | RequestFlags.Pull);
				if (this.WorkloadType == RequestWorkloadType.None)
				{
					this.WorkloadType = RequestWorkloadType.Onboarding;
				}
			}
			else if (base.ParameterSetName.Equals("MigrationOutbound"))
			{
				this.moveFlags |= (RequestFlags.CrossOrg | RequestFlags.Push);
				if (this.WorkloadType == RequestWorkloadType.None)
				{
					this.WorkloadType = RequestWorkloadType.Offboarding;
				}
			}
			else if (base.ParameterSetName.Equals("MigrationRemoteLegacy"))
			{
				this.moveFlags |= (RequestFlags.CrossOrg | RequestFlags.Pull | RequestFlags.RemoteLegacy);
				if (this.WorkloadType == RequestWorkloadType.None)
				{
					this.WorkloadType = RequestWorkloadType.Onboarding;
				}
			}
			else
			{
				this.moveFlags |= RequestFlags.IntraOrg;
				if (this.WorkloadType == RequestWorkloadType.None)
				{
					this.WorkloadType = RequestWorkloadType.Local;
				}
			}
			if (this.ArchiveOnly)
			{
				this.moveFlags |= RequestFlags.MoveOnlyArchiveMailbox;
			}
			if (this.PrimaryOnly)
			{
				this.moveFlags |= RequestFlags.MoveOnlyPrimaryMailbox;
			}
			if (this.ArchiveOnly)
			{
				if (this.PrimaryOnly)
				{
					base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorIncompatibleParameters("PrimaryOnly", "ArchiveOnly")), ErrorCategory.InvalidArgument, this.Identity);
				}
				if (this.TargetDatabase != null)
				{
					base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorIncompatibleParameters("TargetDatabase", "ArchiveOnly")), ErrorCategory.InvalidArgument, this.Identity);
				}
				if (!string.IsNullOrEmpty(this.RemoteTargetDatabase))
				{
					base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorIncompatibleParameters("RemoteTargetDatabase", "ArchiveOnly")), ErrorCategory.InvalidArgument, this.Identity);
				}
			}
			if (this.PrimaryOnly)
			{
				if (this.ArchiveTargetDatabase != null)
				{
					base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorIncompatibleParameters("ArchiveTargetDatabase", "PrimaryOnly")), ErrorCategory.InvalidArgument, this.Identity);
				}
				if (!string.IsNullOrEmpty(this.RemoteArchiveTargetDatabase))
				{
					base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorIncompatibleParameters("RemoteArchiveTargetDatabase", "PrimaryOnly")), ErrorCategory.InvalidArgument, this.Identity);
				}
			}
			if (!string.IsNullOrEmpty(this.RemoteOrganizationName) && this.RemoteCredential != null)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorIncompatibleParameters("RemoteOrganizationName", "RemoteCredential")), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (this.Outbound)
			{
				if (this.ArchiveOnly)
				{
					if (string.IsNullOrEmpty(this.RemoteArchiveTargetDatabase))
					{
					}
				}
				else if (string.IsNullOrEmpty(this.RemoteTargetDatabase))
				{
					base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorMissingDependentParameter("RemoteTargetDatabase", "Outbound")), ErrorCategory.InvalidArgument, this.Identity);
				}
			}
			if (this.MoveRequestIs(RequestFlags.IntraOrg) && !this.IsFieldSet("AllowLargeItems") && this.IsFieldSet("LargeItemLimit"))
			{
				this.AllowLargeItems = false;
			}
			if (this.AllowLargeItems && this.IsFieldSet("LargeItemLimit"))
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorIncompatibleParameters("AllowLargeItems", "LargeItemLimit")), ErrorCategory.InvalidArgument, this.Identity);
			}
			RequestTaskHelper.ValidateItemLimits(this.BadItemLimit, this.LargeItemLimit, this.AcceptLargeDataLoss, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), this.ExecutingUserIdentity);
			if (this.SuspendComment != null && !this.Suspend)
			{
				base.WriteError(new SuspendCommentWithoutSuspendPermanentException(), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (!string.IsNullOrEmpty(this.SuspendComment) && this.SuspendComment.Length > 4096)
			{
				base.WriteError(new ParameterLengthExceededPermanentException("SuspendComment", 4096), ErrorCategory.InvalidArgument, this.SuspendComment);
			}
			if (!string.IsNullOrEmpty(this.BatchName) && this.BatchName.Length > 255)
			{
				base.WriteError(new ParameterLengthExceededPermanentException("BatchName", 255), ErrorCategory.InvalidArgument, this.BatchName);
			}
			if (this.ForcePush && this.ForcePull)
			{
				base.WriteError(new IncompatibleParametersException("ForcePull", "ForcePush"), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (this.ForceOffline && this.SuspendWhenReadyToComplete)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorIncompatibleParameters("ForceOffline", "SuspendWhenReadyToComplete")), ErrorCategory.InvalidArgument, this.ForceOffline);
			}
			if (this.ForceOffline && this.PreventCompletion)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorIncompatibleParameters("ForceOffline", "PreventCompletion")), ErrorCategory.InvalidArgument, this.ForceOffline);
			}
			if (this.MoveRequestIs(RequestFlags.IntraOrg) && SetMoveRequestBase.CheckUserOrgIdIsTenant(base.ExecutingUserOrganizationId))
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorLocalNotForTenantAdmins), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (this.TargetDatabase != null)
			{
				int num;
				this.specifiedTargetMDB = this.LocateAndVerifyMdb(this.TargetDatabase, false, out num);
			}
			if (this.ArchiveTargetDatabase != null)
			{
				int num;
				this.specifiedArchiveTargetMDB = this.LocateAndVerifyMdb(this.ArchiveTargetDatabase, false, out num);
			}
			DateTime utcNow = DateTime.UtcNow;
			if (this.IsFieldSet("StartAfter"))
			{
				RequestTaskHelper.ValidateStartAfterTime(this.StartAfter.ToUniversalTime(), new Task.TaskErrorLoggingDelegate(base.WriteError), utcNow);
			}
			if ((this.IsFieldSet("StartAfter") || this.IsFieldSet("CompleteAfter")) && this.IsFieldSet("SuspendWhenReadyToComplete"))
			{
				DateTime? startAfter = this.IsFieldSet("StartAfter") ? new DateTime?(this.StartAfter) : null;
				DateTime? completeAfter = this.IsFieldSet("CompleteAfter") ? new DateTime?(this.CompleteAfter) : null;
				RequestTaskHelper.ValidateStartAfterCompleteAfterWithSuspendWhenReadyToComplete(startAfter, completeAfter, this.SuspendWhenReadyToComplete.ToBool(), new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (this.IsFieldSet("IncrementalSyncInterval") && this.IsFieldSet("SuspendWhenReadyToComplete") && this.SuspendWhenReadyToComplete.ToBool())
			{
				base.WriteError(new SuspendWhenReadyToCompleteCannotBeSetWithIncrementalSyncIntervalException(), ErrorCategory.InvalidArgument, this.SuspendWhenReadyToComplete);
			}
			if (this.IsFieldSet("IncrementalSyncInterval"))
			{
				RequestTaskHelper.ValidateIncrementalSyncInterval(this.IncrementalSyncInterval, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (this.IsFieldSet("StartAfter") && this.IsFieldSet("CompleteAfter"))
			{
				RequestTaskHelper.ValidateStartAfterComesBeforeCompleteAfter(new DateTime?(this.StartAfter.ToUniversalTime()), new DateTime?(this.CompleteAfter.ToUniversalTime()), new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			this.sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			ADSessionSettings adsessionSettings = ADSessionSettings.RescopeToSubtree(this.sessionSettings);
			if (this.MoveRequestIs(RequestFlags.IntraOrg) && (MapiTaskHelper.IsDatacenter || MapiTaskHelper.IsDatacenterDedicated))
			{
				adsessionSettings.IncludeSoftDeletedObjects = true;
			}
			this.gcSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, adsessionSettings, 1235, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\MoveRequest\\NewMoveRequest.cs");
			this.adSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, adsessionSettings, 1243, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\MoveRequest\\NewMoveRequest.cs");
			this.tenantLocalConfigSession = null;
			if (this.mrProvider != null)
			{
				this.mrProvider.Dispose();
				this.mrProvider = null;
			}
			this.mrProvider = new RequestJobProvider(this.adSession, this.globalConfigSession);
			return this.mrProvider;
		}

		protected override IConfigurable PrepareDataObject()
		{
			TransactionalRequestJob transactionalRequestJob = null;
			IConfigurable result;
			try
			{
				TransactionalRequestJob transactionalRequestJob2 = (TransactionalRequestJob)base.PrepareDataObject();
				transactionalRequestJob = transactionalRequestJob2;
				this.adUser = RequestTaskHelper.ResolveADUser(this.adSession, this.gcSession, base.ServerSettings, this.Identity, base.OptionalIdentityData, this.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), true);
				this.adSession = (IRecipientSession)TaskHelper.UnderscopeSessionToOrganization(this.adSession, this.adUser.OrganizationId, true);
				base.CurrentOrganizationId = this.adUser.OrganizationId;
				this.mrProvider.IndexProvider.RecipientSession = this.adSession;
				this.tenantLocalConfigSession = RecipientTaskHelper.GetTenantLocalConfigSession(this.adUser.OrganizationId, base.ExecutingUserOrganizationId, base.RootOrgContainerId);
				transactionalRequestJob2.UserId = this.adUser.Id;
				transactionalRequestJob2.DistinguishedName = this.adUser.DistinguishedName;
				transactionalRequestJob = null;
				result = transactionalRequestJob2;
			}
			finally
			{
				if (transactionalRequestJob != null)
				{
					transactionalRequestJob.Dispose();
				}
			}
			return result;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalValidate();
				RequestTaskHelper.SetSkipMoving(this.SkipMoving, this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError), false);
				RequestTaskHelper.SetInternalFlags(this.InternalFlags, this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
				if (this.SkipMoving == null)
				{
					this.DataObject.SkipKnownCorruptions = ConfigBase<MRSConfigSchema>.GetConfig<bool>("SkipKnownCorruptionsDefault");
				}
				if (this.IsFieldSet("InternalFlags"))
				{
					RequestTaskHelper.SetInternalFlags(this.InternalFlags, this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
				if (this.MoveRequestIs(RequestFlags.RemoteLegacy))
				{
					this.moveFlags &= ~(RequestFlags.Push | RequestFlags.Pull);
					if (this.adUser.RecipientType == RecipientType.MailUser)
					{
						this.moveFlags |= RequestFlags.Pull;
						if (!string.IsNullOrEmpty(this.RemoteTargetDatabase))
						{
							base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorParameterValueNotAllowed("RemoteTargetDatabase")), ErrorCategory.InvalidArgument, this.Identity);
						}
					}
					else if (this.adUser.RecipientType == RecipientType.UserMailbox)
					{
						this.moveFlags |= RequestFlags.Push;
						if (string.IsNullOrEmpty(this.RemoteTargetDatabase))
						{
							base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorParameterValueRequired("RemoteTargetDatabase")), ErrorCategory.InvalidArgument, this.Identity);
						}
						if (this.TargetDatabase != null)
						{
							base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorParameterValueNotAllowed("TargetDatabase")), ErrorCategory.InvalidArgument, this.Identity);
						}
					}
					else
					{
						base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorInvalidRecipientType(this.adUser.ToString(), this.adUser.RecipientType.ToString())), ErrorCategory.InvalidArgument, this.Identity);
					}
				}
				this.CheckTenantMigrationPolicySettings();
				this.EnsureUserNotAlreadyBeingMoved();
				this.CheckRequiredPropertiesSetOnUser();
				if (this.adUser.MailboxProvisioningConstraint == null && VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.LegacyRegCodeSupport.Enabled)
				{
					string dedicatedMailboxPlansCustomAttributeName = AppSettings.Current.DedicatedMailboxPlansCustomAttributeName;
					string customAttribute = ADRecipient.GetCustomAttribute(this.adUser, dedicatedMailboxPlansCustomAttributeName);
					if (!string.IsNullOrEmpty(customAttribute))
					{
						string text = null;
						MailboxProvisioningConstraint mailboxProvisioningConstraint = null;
						if (!ADRecipient.TryParseMailboxProvisioningData(customAttribute, out text, out mailboxProvisioningConstraint))
						{
							base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.Error_InvalidLegacyRegionCode(customAttribute)), ExchangeErrorCategory.Client, null);
						}
					}
				}
				if (this.TargetDatabase != null)
				{
					this.EnsureDatabaseAttributesMatchUser(this.specifiedTargetMDB);
				}
				if (this.ArchiveTargetDatabase != null)
				{
					this.EnsureDatabaseAttributesMatchUser(this.specifiedArchiveTargetMDB);
				}
				this.ChooseTargetMDBs();
				if (this.MoveRequestIs(RequestFlags.IntraOrg))
				{
					int num = 0;
					ADObjectId adObjectId = (this.ArchiveOnly && this.adUser.ArchiveDatabase != null) ? this.adUser.ArchiveDatabase : this.adUser.Database;
					MailboxDatabase sourceDatabase = this.LocateAndVerifyMdb(new DatabaseIdParameter(adObjectId), true, out num);
					if (base.ExchangeRunspaceConfig != null && base.ExchangeRunspaceConfig.IsDedicatedTenantAdmin)
					{
						this.DisallowLocalMoveSubmittedByTenantAdmins(this.ArchiveOnly ? this.ArchiveTargetDatabase : this.TargetDatabase, sourceDatabase);
					}
				}
				this.remoteOrgName = this.RemoteOrganizationName;
				if (string.IsNullOrEmpty(this.remoteOrgName) && this.SourceIsRemote && this.RemoteCredential == null)
				{
					if (this.adUser.ExternalEmailAddress == null)
					{
						base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorParameterValueRequired("RemoteOrganizationName")), ErrorCategory.InvalidArgument, this.Identity);
					}
					SmtpAddress smtpAddress = new SmtpAddress(this.adUser.ExternalEmailAddress.AddressString);
					this.remoteOrgName = smtpAddress.Domain;
				}
				this.mrsClient = MailboxReplicationServiceClient.Create(CommonUtils.LocalComputerName, MRSJobType.RequestJobE15_TenantHint);
				this.remoteCred = RequestTaskHelper.GetNetworkCredential(this.RemoteCredential, null);
				this.RetrieveSourceMailboxesInfo();
				this.RetrieveTargetMailboxesInfo();
				if (this.MoveRequestIs(RequestFlags.IntraOrg))
				{
					this.ComputeLocalPushPull();
				}
				ADObjectId mdbQueue = this.GetMdbQueue();
				ADObjectId adobjectId = null;
				this.VerifyMdbIsCurrentVersion(mdbQueue, out adobjectId);
				this.mrProvider.AttachToMDB(mdbQueue.ObjectGuid);
				this.CheckForDuplicateMR(true, null);
				this.mrsClient.Dispose();
				this.mrsClient = null;
				this.mrsClient = MailboxReplicationServiceClient.Create(this.globalConfigSession, MRSJobType.RequestJobE15_TenantHint, mdbQueue.ObjectGuid, this.unreachableMrsServers);
				this.RetrieveNonMovingTargetMailboxes();
				this.EnsurePrimaryOnlyIsPresentWhenOnboardingWithCloudArchive();
				this.ValidateMailboxes();
				this.ValidateRecipients();
				this.CheckHalfMailboxOnlyMovingFromOrToDatacenter();
				this.CheckPrimaryCannotBeInDatacenterWhenArchiveIsOnPremise();
				this.CheckOnlineMoveSupported();
				this.DisallowPrimaryOnlyCrossForestMovesWhenMailboxHasNoArchive();
				this.CheckVersionsForArchiveSeparation();
				this.DisallowUsersOnLitigationHoldToPreE14();
				this.CheckDifferentSourceAndTargets();
				this.DisallowRemoteLegacyWithE15Plus();
				this.DisallowArbitrationMailboxesToPreE14();
				this.DisallowArbitrationMailboxesCrossForest();
				this.DisallowDiscoveryMailboxesToPreE14();
				this.DisallowPublicFolderMailboxesToPreE15();
				this.DisallowPublicFolderMailboxesCrossForestOrCrossPremise();
				this.DisallowPublicFolderMailboxesMoveDuringFinalization();
				this.DisallowTeamMailboxesToPreE15();
				this.CheckTeamMailboxesCrossForestOrCrossPremise();
				this.DisallowDiscoveryMailboxesCrossForest();
				this.DisallowUCSMigratedMailboxesToPreE15();
				this.DisallowMailboxMovesWithInPlaceHoldToPreE15();
				this.DisallowMailboxMoveWithSubscriptions();
				if (this.PrimaryIsMoving)
				{
					this.VerifyMailboxQuota(this.sourceMbxInfo, this.targetMbxInfo, false);
				}
				if (this.ArchiveIsMoving)
				{
					this.VerifyMailboxQuota(this.sourceArchiveInfo, this.targetArchiveInfo, true);
				}
				if (this.PrimaryIsMoving && this.targetMbxInfo.ServerVersion < Server.E2007MinVersion && this.sourceMbxInfo.RulesSize > 32768)
				{
					if (this.IgnoreRuleLimitErrors)
					{
						this.WriteWarning(Microsoft.Exchange.Management.Tasks.Strings.WarningRulesWillNotBeCopied(this.sourceMbxInfo.MailboxIdentity));
					}
					else
					{
						base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorRulesSizeOverLimit(this.sourceMbxInfo.MailboxIdentity)), ErrorCategory.InvalidArgument, this.Identity);
					}
				}
				if (this.MoveRequestIs(RequestFlags.CrossOrg) && this.PrimaryIsMoving)
				{
					if (this.TargetDeliveryDomain == null)
					{
						base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorParameterValueRequired("TargetDeliveryDomain")), ErrorCategory.InvalidArgument, this.Identity);
					}
					ADUser aduser = ConfigurableObjectXML.Deserialize<ADUser>(this.targetMbxInfo.UserDataXML);
					if (aduser == null)
					{
						base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorMRSIsDownlevel(this.mrsClient.ServerVersion.ComputerName, this.mrsClient.ServerVersion.ToString())), ErrorCategory.InvalidArgument, this.Identity);
					}
					CommonUtils.ValidateTargetDeliveryDomain(aduser.EmailAddresses, this.TargetDeliveryDomain);
					if (this.SourceIsRemote && VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
					{
						RecipientTaskHelper.ValidateSmtpAddress(this.tenantLocalConfigSession, this.TargetUser.EmailAddresses, this.TargetUser, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache, true);
					}
				}
				if (this.specifiedTargetMDB != null)
				{
					MailboxTaskHelper.VerifyDatabaseIsWithinScopeForRecipientCmdlets(this.sessionSettings, this.specifiedTargetMDB, new Task.ErrorLoggerDelegate(base.WriteError));
				}
				if (this.specifiedArchiveTargetMDB != null)
				{
					MailboxTaskHelper.VerifyDatabaseIsWithinScopeForRecipientCmdlets(this.sessionSettings, this.specifiedArchiveTargetMDB, new Task.ErrorLoggerDelegate(base.WriteError));
				}
				if (this.SourceIsRemote)
				{
					this.ValidateUMSettings();
				}
				RequestTaskHelper.ValidateNotImplicitSplit(this.moveFlags, this.SourceUser, new Task.TaskErrorLoggingDelegate(base.WriteError), this.Identity);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			try
			{
				TransactionalRequestJob dataObject = this.DataObject;
				DateTime utcNow = DateTime.UtcNow;
				dataObject.TimeTracker.SetTimestamp(RequestJobTimestamp.Creation, new DateTime?(utcNow));
				dataObject.TimeTracker.CurrentState = RequestState.Queued;
				if (this.PreventCompletion || this.SuspendWhenReadyToComplete)
				{
					dataObject.JobType = MRSJobType.RequestJobE15_TenantHint;
				}
				else
				{
					dataObject.JobType = MRSJobType.RequestJobE15_AutoResume;
				}
				dataObject.WorkloadType = this.WorkloadType;
				dataObject.RequestGuid = Guid.NewGuid();
				dataObject.UserId = this.adUser.Id;
				dataObject.ExchangeGuid = this.adUser.ExchangeGuid;
				RequestTaskHelper.ApplyOrganization(dataObject, this.adUser.OrganizationId ?? OrganizationId.ForestWideOrgId);
				if (this.IsFieldSet("StartAfter"))
				{
					RequestTaskHelper.SetStartAfter(new DateTime?(this.StartAfter), dataObject, null);
				}
				if (this.IsFieldSet("CompleteAfter"))
				{
					RequestTaskHelper.SetCompleteAfter(new DateTime?(this.CompleteAfter), dataObject, null);
				}
				dataObject.TimeTracker.SetTimestamp(RequestJobTimestamp.LastSuccessfulSourceConnection, new DateTime?(utcNow));
				dataObject.TimeTracker.SetTimestamp(RequestJobTimestamp.LastSuccessfulTargetConnection, new DateTime?(utcNow));
				dataObject.IncrementalSyncInterval = this.IncrementalSyncInterval;
				if (this.SourceUser.ArchiveGuid != Guid.Empty && this.SourceUser.ArchiveDomain == null)
				{
					dataObject.ArchiveGuid = new Guid?(this.SourceUser.ArchiveGuid);
				}
				dataObject.UserOrgName = ((this.adUser.OrganizationId != null && this.adUser.OrganizationId.OrganizationalUnit != null) ? this.adUser.OrganizationId.OrganizationalUnit.Name : this.adUser.Id.DomainId.ToString());
				dataObject.DistinguishedName = this.adUser.DistinguishedName;
				dataObject.DisplayName = this.adUser.DisplayName;
				dataObject.Alias = this.adUser.Alias;
				dataObject.User = this.adUser;
				dataObject.Flags = this.moveFlags;
				if (this.MoveRequestIs(RequestFlags.IntraOrg))
				{
					if (this.PrimaryIsMoving)
					{
						dataObject.SourceDatabase = ADObjectIdResolutionHelper.ResolveDN(this.adUser.Database);
						dataObject.TargetDatabase = this.targetDatabaseForMailbox;
					}
					if (this.ArchiveIsMoving)
					{
						dataObject.SourceArchiveDatabase = ADObjectIdResolutionHelper.ResolveDN(this.adUser.ArchiveDatabase ?? this.adUser.Database);
						dataObject.TargetArchiveDatabase = this.targetDatabaseForMailboxArchive;
					}
					dataObject.PreserveMailboxSignature = !this.DoNotPreserveMailboxSignature;
				}
				else if (this.MoveRequestIs(RequestFlags.CrossOrg))
				{
					dataObject.RemoteCredential = this.remoteCred;
					dataObject.TargetDeliveryDomain = this.TargetDeliveryDomain;
					dataObject.PreserveMailboxSignature = false;
					if (this.MoveRequestIs(RequestFlags.Push))
					{
						if (this.PrimaryIsMoving)
						{
							dataObject.SourceDatabase = ADObjectIdResolutionHelper.ResolveDN(this.adUser.Database);
							dataObject.RemoteDatabaseName = this.targetMbxInfo.MdbName;
							dataObject.RemoteDatabaseGuid = new Guid?(this.targetMbxInfo.MdbGuid);
						}
						if (this.ArchiveIsMoving)
						{
							dataObject.SourceArchiveDatabase = ADObjectIdResolutionHelper.ResolveDN(this.adUser.ArchiveDatabase ?? this.adUser.Database);
							dataObject.RemoteArchiveDatabaseName = this.targetArchiveInfo.MdbName;
							dataObject.RemoteArchiveDatabaseGuid = new Guid?(this.targetArchiveInfo.MdbGuid);
						}
						dataObject.TargetDatabase = null;
						dataObject.TargetArchiveDatabase = null;
						if (this.MoveRequestIs(RequestFlags.RemoteLegacy))
						{
							dataObject.TargetDCName = this.RemoteGlobalCatalog;
							dataObject.TargetCredential = this.remoteCred;
						}
					}
					else
					{
						dataObject.SourceDatabase = null;
						dataObject.SourceArchiveDatabase = null;
						if (this.PrimaryIsMoving)
						{
							dataObject.TargetDatabase = this.targetDatabaseForMailbox;
							dataObject.RemoteDatabaseName = this.sourceMbxInfo.MdbName;
							dataObject.RemoteDatabaseGuid = new Guid?(this.sourceMbxInfo.MdbGuid);
						}
						if (this.ArchiveIsMoving)
						{
							dataObject.TargetArchiveDatabase = this.targetDatabaseForMailboxArchive;
							dataObject.RemoteArchiveDatabaseName = this.sourceArchiveInfo.MdbName;
							dataObject.RemoteArchiveDatabaseGuid = new Guid?(this.sourceArchiveInfo.MdbGuid);
						}
						if (this.MoveRequestIs(RequestFlags.RemoteLegacy))
						{
							dataObject.SourceDCName = this.RemoteGlobalCatalog;
							dataObject.SourceCredential = this.remoteCred;
						}
					}
					if (!this.MoveRequestIs(RequestFlags.RemoteLegacy))
					{
						dataObject.RemoteHostName = this.RemoteHostName;
						dataObject.RemoteOrgName = this.remoteOrgName;
						if (!string.IsNullOrEmpty(this.RemoteGlobalCatalog))
						{
							dataObject.RemoteDomainControllerToUpdate = this.RemoteGlobalCatalog;
						}
						if (this.Outbound && !this.ArchiveIsMoving && this.MailboxHasArchive)
						{
							ExAssert.RetailAssert(MapiTaskHelper.IsDatacenter, "Splitting archive is only supported from datacenter.");
							if (string.IsNullOrEmpty(this.ArchiveDomain))
							{
								base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorParameterRequired("ArchiveDomain")), ErrorCategory.InvalidArgument, this.Identity);
							}
						}
						dataObject.ArchiveDomain = (this.ArchiveDomain ?? this.TargetDeliveryDomain);
					}
				}
				if (this.PrimaryIsMoving)
				{
					dataObject.SourceVersion = this.sourceMbxInfo.ServerVersion;
					dataObject.SourceServer = ((this.sourceMbxInfo.ServerInformation != null) ? this.sourceMbxInfo.ServerInformation.MailboxServerName : null);
					dataObject.TargetVersion = this.targetMbxInfo.ServerVersion;
					dataObject.TargetServer = ((this.targetMbxInfo.ServerInformation != null) ? this.targetMbxInfo.ServerInformation.MailboxServerName : null);
					dataObject.TotalMailboxItemCount = this.sourceMbxInfo.MailboxItemCount;
					dataObject.TotalMailboxSize = this.sourceMbxInfo.MailboxSize;
					dataObject.TargetContainerGuid = this.adUser.MailboxContainerGuid;
				}
				if (this.ArchiveIsMoving)
				{
					dataObject.SourceArchiveVersion = this.sourceArchiveInfo.ServerVersion;
					dataObject.SourceArchiveServer = ((this.sourceArchiveInfo.ServerInformation != null) ? this.sourceArchiveInfo.ServerInformation.MailboxServerName : null);
					dataObject.TargetArchiveVersion = this.targetArchiveInfo.ServerVersion;
					dataObject.TargetArchiveServer = ((this.targetArchiveInfo.ServerInformation != null) ? this.targetArchiveInfo.ServerInformation.MailboxServerName : null);
					dataObject.TotalArchiveItemCount = new ulong?(this.sourceArchiveInfo.MailboxItemCount);
					dataObject.TotalArchiveSize = new ulong?(this.sourceArchiveInfo.MailboxSize);
				}
				dataObject.BatchName = this.BatchName;
				dataObject.IgnoreRuleLimitErrors = this.IgnoreRuleLimitErrors;
				dataObject.Priority = this.ComputePriority();
				dataObject.CompletedRequestAgeLimit = this.CompletedRequestAgeLimit;
				dataObject.RequestCreator = this.ExecutingUserIdentity;
				dataObject.BadItemLimit = this.BadItemLimit;
				dataObject.LargeItemLimit = this.LargeItemLimit;
				dataObject.AllowLargeItems = this.AllowLargeItems;
				if (this.Protect)
				{
					dataObject.Flags |= RequestFlags.Protected;
				}
				if (this.ForceOffline)
				{
					dataObject.ForceOfflineMove = true;
					dataObject.Flags |= RequestFlags.Offline;
				}
				if (this.PreventCompletion)
				{
					dataObject.PreventCompletion = true;
				}
				RequestTaskHelper.SetSkipMoving(this.SkipMoving, dataObject, new Task.TaskErrorLoggingDelegate(base.WriteError), true);
				RequestTaskHelper.SetInternalFlags(this.InternalFlags, dataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
				if (this.SkipMoving == null)
				{
					dataObject.SkipKnownCorruptions = ConfigBase<MRSConfigSchema>.GetConfig<bool>("SkipKnownCorruptionsDefault");
				}
				bool flag = this.SourceUser.IsFromDatacenter && this.TargetUser.IsFromDatacenter && (!dataObject.SkipMailboxReleaseCheck && !dataObject.BlockFinalization) && !dataObject.PreventCompletion;
				if (flag)
				{
					this.CheckMailboxRelease();
				}
				dataObject.Status = RequestStatus.Queued;
				dataObject.Suspend = this.Suspend;
				dataObject.SuspendWhenReadyToComplete = this.SuspendWhenReadyToComplete;
				if (!string.IsNullOrEmpty(this.SuspendComment))
				{
					dataObject.Message = MrsStrings.MoveRequestMessageInformational(new LocalizedString(this.SuspendComment));
				}
				dataObject.DomainControllerToUpdate = this.adUser.OriginatingServer;
				dataObject.RequestJobState = JobProcessingState.Ready;
				dataObject.Identity = new RequestJobObjectId(dataObject.ExchangeGuid, dataObject.WorkItemQueueMdb.ObjectGuid, null);
				dataObject.RequestQueue = ADObjectIdResolutionHelper.ResolveDN(dataObject.WorkItemQueueMdb);
				if (!base.Stopping)
				{
					ReportData reportData = new ReportData(dataObject.ExchangeGuid, dataObject.ReportVersion);
					reportData.Delete(this.mrProvider.SystemMailbox);
					ConnectivityRec connectivityRec = new ConnectivityRec(ServerKind.Cmdlet, VersionInformation.MRS);
					reportData.Append(MrsStrings.ReportMoveRequestCreated(this.ExecutingUserIdentity), connectivityRec);
					if (this.AcceptLargeDataLoss)
					{
						reportData.Append(MrsStrings.ReportLargeAmountOfDataLossAccepted2(this.BadItemLimit.ToString(), this.LargeItemLimit.ToString(), this.ExecutingUserIdentity));
					}
					reportData.Flush(this.mrProvider.SystemMailbox);
					base.InternalProcessRecord();
					RequestJobLog.Write(dataObject);
					this.CheckForDuplicateMR(false, dataObject);
					if (this.mrsClient.ServerVersion[3])
					{
						this.mrsClient.RefreshMoveRequest2(dataObject.ExchangeGuid, dataObject.WorkItemQueueMdb.ObjectGuid, (int)dataObject.Flags, MoveRequestNotification.Created);
					}
					else
					{
						this.mrsClient.RefreshMoveRequest(dataObject.ExchangeGuid, dataObject.WorkItemQueueMdb.ObjectGuid, MoveRequestNotification.Created);
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalStateReset()
		{
			this.adUser = null;
			this.remoteADUser = null;
			if (this.DataObject != null)
			{
				this.DataObject.Dispose();
				this.DataObject = null;
			}
			base.InternalStateReset();
			if (this.mrsClient != null)
			{
				this.mrsClient.Dispose();
				this.mrsClient = null;
			}
			this.sourceMbxInfo = null;
			this.sourceArchiveInfo = null;
			this.targetMbxInfo = null;
			this.targetArchiveInfo = null;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return SetMoveRequestBase.IsKnownExceptionHandler(exception, new WriteVerboseDelegate(base.WriteVerbose)) || base.IsKnownException(exception);
		}

		protected override void TranslateException(ref Exception e, out ErrorCategory category)
		{
			LocalizedException ex = SetMoveRequestBase.TranslateExceptionHandler(e);
			if (ex == null)
			{
				ErrorCategory errorCategory;
				base.TranslateException(ref e, out errorCategory);
				category = errorCategory;
				return;
			}
			e = ex;
			category = ErrorCategory.ResourceUnavailable;
		}

		protected override void WriteResult()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Identity
			});
			base.WriteVerbose(TaskVerboseStringHelper.GetReadObjectVerboseString(this.DataObject.Identity, base.DataSession, typeof(TransactionalRequestJob)));
			IConfigurable configurable = null;
			try
			{
				try
				{
					configurable = base.DataSession.Read<TransactionalRequestJob>(this.DataObject.Identity);
				}
				finally
				{
					base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
				}
				if (configurable == null)
				{
					base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.ResolveIdentityString(this.DataObject.Identity), typeof(TransactionalRequestJob).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), (ErrorCategory)1003, this.DataObject.Identity);
				}
				this.WriteResult(configurable);
			}
			finally
			{
				IDisposable disposable = configurable as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
					configurable = null;
				}
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			MoveRequestStatistics moveRequestStatistics = new MoveRequestStatistics((TransactionalRequestJob)dataObject);
			if (this.adUser != null)
			{
				moveRequestStatistics.DisplayName = this.adUser.DisplayName;
				moveRequestStatistics.Alias = this.adUser.Alias;
			}
			base.WriteResult(moveRequestStatistics);
		}

		private void CheckRequiredPropertiesSetOnUser()
		{
			if (this.adUser.ExchangeGuid == Guid.Empty)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorInvalidObjectMissingCriticalProperty(this.adUser.RecipientType.ToString(), this.adUser.ToString(), ADMailboxRecipientSchema.ExchangeGuid.Name)), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (string.IsNullOrEmpty(this.adUser.LegacyExchangeDN))
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorInvalidObjectMissingCriticalProperty(this.adUser.RecipientType.ToString(), this.adUser.ToString(), ADRecipientSchema.LegacyExchangeDN.Name)), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (string.IsNullOrEmpty(this.adUser.Alias))
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorInvalidObjectMissingCriticalProperty(this.adUser.RecipientType.ToString(), this.adUser.ToString(), ADRecipientSchema.Alias.Name)), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (this.SourceIsLocal)
			{
				if (this.PrimaryIsMoving && this.adUser.Database == null)
				{
					base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorSourcePrimaryMailboxDoesNotExist(this.adUser.ToString())), ErrorCategory.InvalidArgument, this.Identity);
				}
				if (this.ArchiveOnly && !this.adUser.HasLocalArchive)
				{
					base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorSourceArchiveMailboxDoesNotExist(this.adUser.ToString())), ErrorCategory.InvalidArgument, this.Identity);
				}
			}
			if (this.SourceIsRemote && this.TargetIsLocal)
			{
				if (this.PrimaryIsMoving && this.adUser.Database != null)
				{
					base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorTargetUserAlreadyHasPrimaryMailbox(this.adUser.ToString())), ErrorCategory.InvalidArgument, this.Identity);
				}
				if (this.ArchiveOnly && this.adUser.HasLocalArchive)
				{
					base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorTargetUserAlreadyHasArchiveMailbox(this.adUser.ToString())), ErrorCategory.InvalidArgument, this.Identity);
				}
			}
		}

		private void DisallowLocalMoveSubmittedByTenantAdmins(DatabaseIdParameter specifiedTargetMDB, MailboxDatabase sourceDatabase)
		{
			if (specifiedTargetMDB == null && (this.adUser.MailboxProvisioningConstraint == null || this.adUser.MailboxProvisioningConstraint.IsMatch(sourceDatabase.MailboxProvisioningAttributes)))
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorLocalNotForTenantAdmins), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void DisallowPrimaryOnlyCrossForestMovesWhenMailboxHasNoArchive()
		{
			if (this.PrimaryOnly && this.MoveRequestIs(RequestFlags.CrossOrg) && !this.MailboxHasArchive)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorPrimaryOnlyCrossForestMovesWithoutArchive(this.adUser.Name)), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void CheckTenantMigrationPolicySettings()
		{
			if (this.IgnoreTenantMigrationPolicies)
			{
				return;
			}
			if (!this.CheckOnboardingOffboarding())
			{
				return;
			}
			ADObjectId organizationalUnit = this.adUser.OrganizationId.OrganizationalUnit;
			ADObjectId configurationUnit = this.adUser.OrganizationId.ConfigurationUnit;
			Organization organization = this.GetOrganization();
			if (organization == null)
			{
				base.WriteError(new ManagementObjectNotFoundException(Microsoft.Exchange.Management.Tasks.Strings.ErrorOrganizationNotFound(configurationUnit.ToString())), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (this.SourceIsRemote && organization.IsExcludedFromOnboardMigration)
			{
				base.WriteError(new OnboardingDisabledException(), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (this.TargetIsRemote && organization.IsExcludedFromOffboardMigration)
			{
				base.WriteError(new OffboardingDisabledException(), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (organization.MaxConcurrentMigrations.IsUnlimited)
			{
				return;
			}
			int value = organization.MaxConcurrentMigrations.Value;
			if (value > 1000)
			{
				return;
			}
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ExistsFilter(ADUserSchema.MailboxMoveStatus),
				new ComparisonFilter(ComparisonOperator.NotEqual, ADUserSchema.MailboxMoveStatus, RequestStatus.None),
				new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox),
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.MailUser)
				}),
				new BitMaskAndFilter(ADUserSchema.MailboxMoveFlags, 1UL)
			});
			MrsTracer.Cmdlet.Debug("Querying for up to {0} remote MoveRequests...", new object[]
			{
				value
			});
			ADRawEntry[] array = this.adSession.Find(organizationalUnit, QueryScope.OneLevel, filter, null, value, NewMoveRequest.simpleSearchResult);
			MrsTracer.Cmdlet.Debug("Found {0} remote MoveRequests.", new object[]
			{
				(array == null) ? "null" : array.Length.ToString()
			});
			if (array != null && array.Length >= value)
			{
				base.WriteError(new MaxConcurrentMigrationsExceededException(value), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void EnsureUserNotAlreadyBeingMoved()
		{
			if (this.adUser.MailboxMoveStatus == RequestStatus.Completed || this.adUser.MailboxMoveStatus == RequestStatus.CompletedWithWarning)
			{
				base.WriteError(new ManagementObjectAlreadyExistsException(Microsoft.Exchange.Management.Tasks.Strings.ErrorCompletedMoveMustBeCleared(this.adUser.ToString())), ErrorCategory.InvalidArgument, this.Identity);
				return;
			}
			if (this.adUser.MailboxMoveStatus != RequestStatus.None)
			{
				string target;
				if (this.adUser.MailboxMoveTargetMDB != null)
				{
					target = this.adUser.MailboxMoveTargetMDB.ToString();
				}
				else if (this.adUser.MailboxMoveTargetArchiveMDB != null)
				{
					target = this.adUser.MailboxMoveTargetArchiveMDB.ToString();
				}
				else
				{
					target = this.adUser.MailboxMoveRemoteHostName;
				}
				base.WriteError(new ManagementObjectAlreadyExistsException(Microsoft.Exchange.Management.Tasks.Strings.ErrorUserIsAlreadyBeingMoved(this.adUser.ToString(), target)), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void ChooseTargetMDBs()
		{
			if (this.TargetIsLocal)
			{
				MailboxDatabase mailboxDatabase = null;
				if (!this.ArchiveOnly)
				{
					if (this.specifiedTargetMDB != null)
					{
						this.targetDatabaseForMailbox = this.specifiedTargetMDB.Id;
						mailboxDatabase = this.specifiedTargetMDB;
					}
					else
					{
						mailboxDatabase = this.ChooseTargetMDB(this.adUser.Database);
						this.targetDatabaseForMailbox = ADObjectIdResolutionHelper.ResolveDN(mailboxDatabase.Id);
					}
					if (CommonUtils.ShouldHonorProvisioningSettings() && (this.MoveRequestIs(RequestFlags.IntraOrg) || this.MoveRequestIs(RequestFlags.Pull)) && !this.InternalFlagsContains(InternalMrsFlag.SkipProvisioningCheck) && mailboxDatabase.IsExcludedFromProvisioning)
					{
						base.WriteError(new DatabaseExcludedFromProvisioningException(mailboxDatabase.Name), ErrorCategory.InvalidArgument, this.Identity);
					}
				}
				if (!this.PrimaryOnly)
				{
					MailboxDatabase mailboxDatabase2;
					if (this.specifiedArchiveTargetMDB != null)
					{
						this.targetDatabaseForMailboxArchive = this.specifiedArchiveTargetMDB.Id;
						mailboxDatabase2 = this.specifiedArchiveTargetMDB;
					}
					else if (this.ArchiveOnly)
					{
						mailboxDatabase2 = this.ChooseTargetMDB(this.adUser.ArchiveDatabase ?? this.adUser.Database);
						this.targetDatabaseForMailboxArchive = ADObjectIdResolutionHelper.ResolveDN(mailboxDatabase2.Id);
					}
					else
					{
						this.targetDatabaseForMailboxArchive = this.targetDatabaseForMailbox;
						mailboxDatabase2 = mailboxDatabase;
					}
					if (CommonUtils.ShouldHonorProvisioningSettings() && (this.MoveRequestIs(RequestFlags.IntraOrg) || this.MoveRequestIs(RequestFlags.Pull)) && !this.InternalFlagsContains(InternalMrsFlag.SkipProvisioningCheck) && mailboxDatabase2.IsExcludedFromProvisioning)
					{
						base.WriteError(new DatabaseExcludedFromProvisioningException(mailboxDatabase2.Name), ErrorCategory.InvalidArgument, this.Identity);
					}
				}
			}
		}

		private void ComputeLocalPushPull()
		{
			this.moveFlags &= ~(RequestFlags.Push | RequestFlags.Pull);
			if (this.ForcePush || this.ForcePull)
			{
				this.moveFlags |= (this.ForcePush ? RequestFlags.Push : RequestFlags.Pull);
				return;
			}
			MailboxInformation mailboxInformation = this.ArchiveOnly ? this.sourceArchiveInfo : this.sourceMbxInfo;
			MailboxInformation mailboxInformation2 = this.ArchiveOnly ? this.targetArchiveInfo : this.targetMbxInfo;
			float num = mailboxInformation.MrsVersion;
			float num2 = mailboxInformation2.MrsVersion;
			if (num == 0f && mailboxInformation.ServerVersion < Server.E15MinVersion)
			{
				num = -1f;
			}
			if (num2 == 0f && mailboxInformation2.ServerVersion < Server.E15MinVersion)
			{
				num2 = -1f;
			}
			bool flag = num > num2;
			this.moveFlags |= (flag ? RequestFlags.Push : RequestFlags.Pull);
			if (this.PrimaryIsMoving && this.ArchiveIsMoving)
			{
				bool flag2 = this.sourceArchiveInfo.MrsVersion > this.targetArchiveInfo.MrsVersion;
				if (flag2 != flag)
				{
					base.WriteError(new CannotMovePrimaryAndArchiveToOpposingMrsVersions(mailboxInformation.MrsVersion.ToString(), mailboxInformation2.MrsVersion.ToString(), this.sourceArchiveInfo.MrsVersion.ToString(), this.targetArchiveInfo.MrsVersion.ToString()), ErrorCategory.InvalidArgument, this.Identity);
				}
			}
		}

		private void VerifyMdbIsCurrentVersion(ADObjectId mdbQueue, out ADObjectId mdbqServerSite)
		{
			DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(mdbQueue.ObjectGuid, null, null, FindServerFlags.None);
			ServerVersion serverVersion = new ServerVersion(databaseInformation.ServerVersion);
			ServerVersion serverVersion2 = new ServerVersion(Server.E15MinVersion);
			if (serverVersion.Major < serverVersion2.Major)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorMailboxDatabaseNotOnCurrentCmdletVersion2(databaseInformation.DatabaseName)), ErrorCategory.InvalidArgument, this.Identity);
			}
			mdbqServerSite = databaseInformation.ServerSite;
		}

		private bool MoveRequestIs(RequestFlags requiredFlags)
		{
			return (this.moveFlags & requiredFlags) == requiredFlags;
		}

		private void RetrieveSourceMailboxesInfo()
		{
			TenantPartitionHint partitionHint = null;
			if (this.adUser.OrganizationId != null && (this.SourceIsLocal || TestIntegration.Instance.RemoteExchangeGuidOverride != Guid.Empty || this.DataObject.CrossResourceForest))
			{
				partitionHint = TenantPartitionHint.FromOrganizationId(this.adUser.OrganizationId);
			}
			if (this.PrimaryIsMoving)
			{
				MrsTracer.Cmdlet.Debug("Loading source mailbox info", new object[0]);
				this.sourceMbxInfo = this.mrsClient.GetMailboxInformation(this.DataObject, this.adUser.ExchangeGuid, this.adUser.ExchangeGuid, partitionHint, Guid.Empty, null, this.SourceIsRemote ? this.RemoteHostName : null, this.SourceIsRemote ? this.remoteOrgName : null, this.SourceIsRemote ? this.RemoteGlobalCatalog : null, this.SourceIsRemote ? this.remoteCred : null);
				MrsTracer.Cmdlet.Debug("Source mailbox info:\n{0}", new object[]
				{
					this.sourceMbxInfo.ToString()
				});
				if (this.SourceIsRemote)
				{
					this.remoteADUser = ConfigurableObjectXML.Deserialize<ADUser>(this.sourceMbxInfo.UserDataXML);
				}
			}
			if (this.ArchiveIsMoving)
			{
				if (!this.MailboxHasArchive)
				{
					base.WriteError(new ArchiveDisabledPermanentException(), ErrorCategory.InvalidArgument, this.Identity);
				}
				MrsTracer.Cmdlet.Debug("Loading source archive info", new object[0]);
				this.sourceArchiveInfo = this.mrsClient.GetMailboxInformation(this.DataObject, this.adUser.ExchangeGuid, this.adUser.ArchiveGuid, partitionHint, Guid.Empty, null, this.SourceIsRemote ? this.RemoteHostName : null, this.SourceIsRemote ? this.remoteOrgName : null, this.SourceIsRemote ? this.RemoteGlobalCatalog : null, this.SourceIsRemote ? this.remoteCred : null);
				MrsTracer.Cmdlet.Debug("Source archive info:\n{0}", new object[]
				{
					this.sourceArchiveInfo.ToString()
				});
				if (this.SourceIsRemote && !this.PrimaryIsMoving)
				{
					this.remoteADUser = ConfigurableObjectXML.Deserialize<ADUser>(this.sourceArchiveInfo.UserDataXML);
				}
			}
			if (this.PrimaryOnly)
			{
				RequestTaskHelper.ValidatePrimaryOnlyMoveArchiveDatabase(this.SourceUser, delegate(Exception exception, ErrorCategory category)
				{
					base.WriteError(exception, category, this.Identity);
				});
			}
		}

		private void RetrieveTargetMailboxesInfo()
		{
			TenantPartitionHint partitionHint = (this.TargetIsLocal || TestIntegration.Instance.RemoteExchangeGuidOverride != Guid.Empty) ? TenantPartitionHint.FromOrganizationId(this.adUser.OrganizationId) : null;
			if (this.PrimaryIsMoving)
			{
				MrsTracer.Cmdlet.Debug("Loading target mailbox info", new object[0]);
				this.targetMbxInfo = this.mrsClient.GetMailboxInformation(this.DataObject, this.adUser.ExchangeGuid, this.adUser.ExchangeGuid, partitionHint, this.TargetIsRemote ? Guid.Empty : this.targetDatabaseForMailbox.ObjectGuid, this.TargetIsRemote ? this.RemoteTargetDatabase : null, this.TargetIsRemote ? this.RemoteHostName : null, this.TargetIsRemote ? this.remoteOrgName : null, this.TargetIsRemote ? this.RemoteGlobalCatalog : null, this.TargetIsRemote ? this.remoteCred : null);
				MrsTracer.Cmdlet.Debug("Target mailbox info:\n{0}", new object[]
				{
					this.targetMbxInfo.ToString()
				});
				if (this.TargetIsRemote)
				{
					this.remoteADUser = ConfigurableObjectXML.Deserialize<ADUser>(this.targetMbxInfo.UserDataXML);
				}
			}
			if (this.ArchiveIsMoving)
			{
				MrsTracer.Cmdlet.Debug("Loading target archive info", new object[0]);
				string text = this.RemoteArchiveTargetDatabase ?? this.RemoteTargetDatabase;
				if (this.TargetIsRemote && string.IsNullOrEmpty(text))
				{
					if (this.SourceHasPrimary)
					{
						base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorMissingDependentParameter("RemoteArchiveTargetDatabase", "Outbound")), ErrorCategory.InvalidArgument, this.Identity);
					}
					MrsTracer.Cmdlet.Debug("Loading target primary mailbox info", new object[0]);
					MailboxInformation mailboxInformation = this.mrsClient.GetMailboxInformation(this.DataObject, this.adUser.ExchangeGuid, this.adUser.ExchangeGuid, partitionHint, Guid.Empty, null, this.RemoteHostName, this.remoteOrgName, this.RemoteGlobalCatalog, this.remoteCred);
					text = mailboxInformation.MdbName;
					MrsTracer.Cmdlet.Debug("RemoteArchiveTargetDatabase was not specified. It was defaulted to the HomeMDB '{0}' of the target primary mailbox.", new object[]
					{
						mailboxInformation.MdbName
					});
				}
				this.targetArchiveInfo = this.mrsClient.GetMailboxInformation(this.DataObject, this.adUser.ExchangeGuid, this.adUser.ArchiveGuid, partitionHint, this.TargetIsRemote ? Guid.Empty : this.targetDatabaseForMailboxArchive.ObjectGuid, this.TargetIsRemote ? text : null, this.TargetIsRemote ? this.RemoteHostName : null, this.TargetIsRemote ? this.remoteOrgName : null, this.TargetIsRemote ? this.RemoteGlobalCatalog : null, this.TargetIsRemote ? this.remoteCred : null);
				MrsTracer.Cmdlet.Debug("Target archive info:\n{0}", new object[]
				{
					this.targetArchiveInfo.ToString()
				});
				if (this.TargetIsRemote && !this.PrimaryIsMoving)
				{
					this.remoteADUser = ConfigurableObjectXML.Deserialize<ADUser>(this.targetArchiveInfo.UserDataXML);
				}
			}
		}

		private void RetrieveNonMovingTargetMailboxes()
		{
			TenantPartitionHint partitionHint = (this.TargetIsLocal || TestIntegration.Instance.RemoteExchangeGuidOverride != Guid.Empty) ? TenantPartitionHint.FromOrganizationId(this.adUser.OrganizationId) : null;
			if (this.targetMbxInfo == null && this.TargetHasPrimary)
			{
				MrsTracer.Cmdlet.Debug("Loading target mailbox info for non-moving primary", new object[0]);
				this.targetMbxInfo = this.mrsClient.GetMailboxInformation(this.DataObject, this.adUser.ExchangeGuid, this.adUser.ExchangeGuid, partitionHint, this.TargetUser.Database.ObjectGuid, this.TargetUser.Database.Name, this.TargetIsRemote ? this.RemoteHostName : null, this.TargetIsRemote ? this.remoteOrgName : null, this.TargetIsRemote ? this.RemoteGlobalCatalog : null, this.TargetIsRemote ? this.remoteCred : null);
				MrsTracer.Cmdlet.Debug("Target mailbox info for non-moving primary:\n{0}", new object[]
				{
					this.targetMbxInfo.ToString()
				});
			}
			if (this.targetArchiveInfo == null && this.TargetHasArchive)
			{
				this.targetArchiveInfo = this.mrsClient.GetMailboxInformation(this.DataObject, this.adUser.ExchangeGuid, this.adUser.ArchiveGuid, partitionHint, this.TargetUser.ArchiveDatabase.ObjectGuid, this.TargetUser.ArchiveDatabase.Name, this.TargetIsRemote ? this.RemoteHostName : null, this.TargetIsRemote ? this.remoteOrgName : null, this.TargetIsRemote ? this.RemoteGlobalCatalog : null, this.TargetIsRemote ? this.remoteCred : null);
				MrsTracer.Cmdlet.Debug("Target archive info for non-moving archive:\n{0}", new object[]
				{
					this.targetArchiveInfo.ToString()
				});
			}
		}

		private void ValidateMailboxes()
		{
			if (this.SourceUser.ArchiveGuid != this.TargetUser.ArchiveGuid && TestIntegration.Instance.RemoteArchiveGuidOverride == Guid.Empty)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorArchiveGuidsDontMatch(this.SourceUser.ToString(), this.TargetUser.ToString(), this.SourceUser.ArchiveGuid, this.TargetUser.ArchiveGuid)), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (!this.ArchiveOnly && !this.SourceHasPrimary)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorSourcePrimaryMailboxDoesNotExist(this.SourceUser.ToString())), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (this.ArchiveOnly && !this.SourceHasArchive)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorSourceArchiveMailboxDoesNotExist(this.SourceUser.ToString())), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (this.MoveRequestIs(RequestFlags.CrossOrg))
			{
				if (this.PrimaryIsMoving && this.TargetHasPrimary)
				{
					base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorTargetUserAlreadyHasPrimaryMailbox(this.TargetUser.ToString())), ErrorCategory.InvalidArgument, this.Identity);
				}
				if (this.ArchiveIsMoving && this.TargetHasArchive)
				{
					base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorTargetUserAlreadyHasArchiveMailbox(this.TargetUser.ToString())), ErrorCategory.InvalidArgument, this.Identity);
				}
			}
			if (this.PrimaryIsMoving)
			{
				this.EnsureSupportedServerVersion(this.sourceMbxInfo.ServerVersion, true);
				this.EnsureSupportedServerVersion(this.targetMbxInfo.ServerVersion, false);
			}
			if (this.ArchiveIsMoving)
			{
				this.EnsureSupportedServerVersionForArchiveScenarios(this.sourceArchiveInfo.ServerVersion, true);
				this.EnsureSupportedServerVersionForArchiveScenarios(this.targetArchiveInfo.ServerVersion, false);
			}
			ServerVersion serverVersion = new ServerVersion(Server.E15MinVersion);
			if (this.PrimaryIsMoving && this.sourceMbxInfo.ServerVersion < Server.E15MinVersion && this.targetMbxInfo.ServerVersion < Server.E15MinVersion)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorAtLeastOneSideMustBeCurrentProductVersion(serverVersion.ToString())), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (this.ArchiveIsMoving && this.sourceArchiveInfo.ServerVersion < Server.E15MinVersion && this.targetArchiveInfo.ServerVersion < Server.E15MinVersion)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorAtLeastOneSideMustBeCurrentProductVersion(serverVersion.ToString())), ErrorCategory.InvalidArgument, this.Identity);
			}
			ServerVersion serverVersion2 = null;
			ServerVersion serverVersion3 = null;
			if (this.PrimaryIsMoving || this.TargetHasPrimary)
			{
				serverVersion2 = new ServerVersion(this.targetMbxInfo.ServerVersion);
			}
			if (this.ArchiveIsMoving || this.TargetHasArchive)
			{
				serverVersion3 = new ServerVersion(this.targetArchiveInfo.ServerVersion);
			}
			if (serverVersion2 != null && serverVersion3 != null && serverVersion2.Major != serverVersion3.Major)
			{
				base.WriteError(new PrimaryAndArchiveNotOnSameVersionPermanentException(serverVersion2.ToString(), serverVersion3.ToString()), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void ValidateRecipients()
		{
			if (this.MoveRequestIs(RequestFlags.CrossOrg))
			{
				if (this.PrimaryIsMoving && this.targetMbxInfo.ServerVersion >= Server.E2007MinVersion && this.TargetUser.RecipientType != RecipientType.MailUser)
				{
					base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorPrimaryTargetIsNotAnMEU(this.TargetUser.ToString())), ErrorCategory.InvalidArgument, this.Identity);
				}
				if (this.ArchiveIsMoving && !this.TargetHasPrimary && this.targetArchiveInfo.ServerVersion >= Server.E14SP1MinVersion && this.TargetUser.RecipientType != RecipientType.MailUser)
				{
					base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorArchiveTargetIsNotAnMEU(this.TargetUser.ToString())), ErrorCategory.InvalidArgument, this.Identity);
				}
			}
		}

		private void CheckHalfMailboxOnlyMovingFromOrToDatacenter()
		{
			if (this.MoveRequestIs(RequestFlags.CrossOrg) && (this.PrimaryOnly || this.ArchiveOnly) && !this.SourceUser.IsFromDatacenter && !this.TargetUser.IsFromDatacenter && !TestIntegration.Instance.AllowRemoteArchivesInEnt)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorCannotMoveHalfMailboxBetweenTwoNonDatacenterForests), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void CheckPrimaryCannotBeInDatacenterWhenArchiveIsOnPremise()
		{
			if (this.MoveRequestIs(RequestFlags.CrossOrg) && this.SourceHasPrimary && this.SourceHasArchive)
			{
				if (!this.SourceUser.IsFromDatacenter && this.TargetUser.IsFromDatacenter && this.PrimaryIsMoving && !this.ArchiveIsMoving)
				{
					base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorCannotHavePrimaryInDatacenterAndArchiveOnPremise), ErrorCategory.InvalidArgument, this.Identity);
				}
				if (this.SourceUser.IsFromDatacenter && !this.TargetUser.IsFromDatacenter && this.ArchiveIsMoving && !this.PrimaryIsMoving)
				{
					base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorCannotHavePrimaryInDatacenterAndArchiveOnPremise), ErrorCategory.InvalidArgument, this.Identity);
				}
			}
		}

		private void CheckOnlineMoveSupported()
		{
			if (this.SuspendWhenReadyToComplete && ((this.PrimaryIsMoving && !this.SupportsOnline(this.sourceMbxInfo.ServerVersion, this.targetMbxInfo.ServerVersion)) || (this.ArchiveIsMoving && !this.SupportsOnline(this.sourceArchiveInfo.ServerVersion, this.targetArchiveInfo.ServerVersion))))
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorSuspendWRTCForOnlineOnly), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void CheckVersionsForArchiveSeparation()
		{
			if (!this.IsSplitPrimaryAndArchiveScenario)
			{
				return;
			}
			int e14SP1MinVersion = Server.E14SP1MinVersion;
			string serverName;
			string serverVersion;
			if (this.mrsClient.ServerVersion[2])
			{
				if (this.PrimaryIsMoving)
				{
					if (this.sourceMbxInfo.ServerVersion < e14SP1MinVersion)
					{
						MrsTracer.Cmdlet.Warning("Source primary MDB is not E14R5 or above", new object[0]);
						serverName = this.sourceMbxInfo.ServerInformation.MailboxServerName;
						serverVersion = new ServerVersion(this.sourceMbxInfo.ServerInformation.MailboxServerVersion).ToString();
						goto IL_39B;
					}
					if (this.targetMbxInfo.ServerVersion < e14SP1MinVersion)
					{
						MrsTracer.Cmdlet.Warning("Target primary MDB is not E14R5 or above", new object[0]);
						serverName = this.targetMbxInfo.ServerInformation.MailboxServerName;
						serverVersion = new ServerVersion(this.targetMbxInfo.ServerInformation.MailboxServerVersion).ToString();
						goto IL_39B;
					}
					if (this.SourceIsRemote && !this.MoveRequestIs(RequestFlags.RemoteLegacy) && !this.sourceMbxInfo.ServerInformation.ProxyServerVersion[9])
					{
						MrsTracer.Cmdlet.Warning("Source MRSProxy does not support archive separation", new object[0]);
						serverName = this.sourceMbxInfo.ServerInformation.ProxyServerVersion.ComputerName;
						serverVersion = this.sourceMbxInfo.ServerInformation.ProxyServerVersion.ComputerName.ToString();
						goto IL_39B;
					}
					if (this.TargetIsRemote && !this.MoveRequestIs(RequestFlags.RemoteLegacy) && !this.targetMbxInfo.ServerInformation.ProxyServerVersion[9])
					{
						MrsTracer.Cmdlet.Warning("Target MRSProxy does not support archive separation", new object[0]);
						serverName = this.targetMbxInfo.ServerInformation.ProxyServerVersion.ComputerName;
						serverVersion = this.targetMbxInfo.ServerInformation.ProxyServerVersion.ComputerName.ToString();
						goto IL_39B;
					}
				}
				if (this.ArchiveIsMoving)
				{
					if (this.sourceArchiveInfo.ServerVersion < e14SP1MinVersion)
					{
						MrsTracer.Cmdlet.Warning("Source archive MDB is not E14R5 or above", new object[0]);
						serverName = this.sourceArchiveInfo.ServerInformation.MailboxServerName;
						serverVersion = new ServerVersion(this.sourceArchiveInfo.ServerInformation.MailboxServerVersion).ToString();
						goto IL_39B;
					}
					if (this.targetArchiveInfo.ServerVersion < e14SP1MinVersion)
					{
						MrsTracer.Cmdlet.Warning("Target archive MDB is not E14R5 or above", new object[0]);
						serverName = this.targetArchiveInfo.ServerInformation.MailboxServerName;
						serverVersion = new ServerVersion(this.targetArchiveInfo.ServerInformation.MailboxServerVersion).ToString();
						goto IL_39B;
					}
					if (this.SourceIsRemote && !this.MoveRequestIs(RequestFlags.RemoteLegacy) && !this.sourceArchiveInfo.ServerInformation.ProxyServerVersion[9])
					{
						MrsTracer.Cmdlet.Warning("Source MRSProxy does not support archive separation", new object[0]);
						serverName = this.sourceArchiveInfo.ServerInformation.ProxyServerVersion.ComputerName;
						serverVersion = this.sourceArchiveInfo.ServerInformation.ProxyServerVersion.ComputerName.ToString();
						goto IL_39B;
					}
					if (this.TargetIsRemote && !this.MoveRequestIs(RequestFlags.RemoteLegacy) && !this.targetArchiveInfo.ServerInformation.ProxyServerVersion[9])
					{
						MrsTracer.Cmdlet.Warning("Target MRSProxy does not support archive separation", new object[0]);
						serverName = this.targetArchiveInfo.ServerInformation.ProxyServerVersion.ComputerName;
						serverVersion = this.targetArchiveInfo.ServerInformation.ProxyServerVersion.ComputerName.ToString();
						goto IL_39B;
					}
				}
				return;
			}
			MrsTracer.Cmdlet.Warning("MRS does not support archive separation", new object[0]);
			serverName = this.mrsClient.ServerVersion.ComputerName;
			serverVersion = this.mrsClient.ServerVersion.ToString();
			IL_39B:
			base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorCannotSeparatePrimaryFromArchiveBecauseServerIsDownlevel(serverName, serverVersion)), ErrorCategory.InvalidArgument, this.Identity);
		}

		private void DisallowUsersOnLitigationHoldToPreE14()
		{
			if (this.PrimaryIsMoving && this.SourceUser.LitigationHoldEnabled && this.targetMbxInfo.ServerVersion < Server.E14MinVersion)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorCannotMoveLitigationHoldToPreE14(this.adUser.Name)), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void CheckDifferentSourceAndTargets()
		{
			if (this.PrimaryIsMoving && this.sourceMbxInfo.MdbGuid == this.targetMbxInfo.MdbGuid)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorUserAlreadyInTargetDatabase(this.sourceMbxInfo.MailboxIdentity, this.sourceMbxInfo.MdbName)), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (this.ArchiveIsMoving && this.sourceArchiveInfo.MdbGuid == this.targetArchiveInfo.MdbGuid)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorArchiveAlreadyInTargetArchiveDatabase(this.sourceArchiveInfo.MailboxIdentity, this.sourceArchiveInfo.MdbName)), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void DisallowRemoteLegacyWithE15Plus()
		{
			if (TestIntegration.Instance.AllowRemoteLegacyMovesWithE15)
			{
				return;
			}
			int num = this.PrimaryIsMoving ? this.sourceMbxInfo.ServerVersion : this.sourceArchiveInfo.ServerVersion;
			int num2 = this.PrimaryIsMoving ? this.targetMbxInfo.ServerVersion : this.targetArchiveInfo.ServerVersion;
			bool flag = num >= Server.E14MinVersion && num < Server.E15MinVersion;
			bool flag2 = num2 >= Server.E14MinVersion && num2 < Server.E15MinVersion;
			bool flag3 = num >= Server.E14MinVersion;
			bool flag4 = num2 >= Server.E14MinVersion;
			if (this.MoveRequestIs(RequestFlags.RemoteLegacy) && flag3 && flag4 && (!flag || !flag2))
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorRemoteLegacyWithE15NotAllowed), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void DisallowArbitrationMailboxesToPreE14()
		{
			if (this.PrimaryIsMoving && this.sourceMbxInfo.RecipientDisplayType == 10 && this.targetMbxInfo.ServerVersion < Server.E14MinVersion)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorCannotMoveArbitrationMailboxesDownlevel(this.adUser.Name)), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void DisallowArbitrationMailboxesCrossForest()
		{
			if (this.MoveRequestIs(RequestFlags.CrossOrg) && this.PrimaryIsMoving && this.sourceMbxInfo.RecipientDisplayType == 10)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorCannotMoveArbitrationMailboxesCrossForest(this.adUser.Name)), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void DisallowDiscoveryMailboxesToPreE14()
		{
			if (this.PrimaryIsMoving && this.sourceMbxInfo.RecipientTypeDetailsLong == 536870912L && this.targetMbxInfo.ServerVersion < Server.E14MinVersion)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorCannotMoveDiscoveryMailboxesDownlevel(this.adUser.Name)), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void DisallowMailboxMovesWithInPlaceHoldToPreE15()
		{
			if (this.PrimaryIsMoving && this.targetMbxInfo.ServerVersion < Server.E15MinVersion && this.sourceMbxInfo.ServerVersion >= Server.E15MinVersion && this.SourceUser.InPlaceHolds != null && this.SourceUser.InPlaceHolds.Count > 0)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorCannotMoveMailboxesWithInPlaceHoldDownlevel(this.adUser.Name)), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void DisallowPublicFolderMailboxesToPreE15()
		{
			if (this.PrimaryIsMoving && this.sourceMbxInfo.RecipientTypeDetailsLong == 68719476736L && this.targetMbxInfo.ServerVersion < Server.E15MinVersion)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorCannotMovePublicFolderMailboxesDownlevel(this.adUser.Name)), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void DisallowPublicFolderMailboxesCrossForestOrCrossPremise()
		{
			if (this.MoveRequestIs(RequestFlags.CrossOrg) && this.PrimaryIsMoving && this.sourceMbxInfo.RecipientTypeDetailsLong == 68719476736L)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorCannotMovePublicFolderMailboxesCrossForestOrCrossPremise(this.adUser.Name)), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void DisallowPublicFolderMailboxesMoveDuringFinalization()
		{
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Mrs.PublicFolderMailboxesMigration.Enabled && this.PrimaryIsMoving && this.sourceMbxInfo.RecipientTypeDetailsLong == 68719476736L && CommonUtils.IsPublicFolderMailboxesLockedForNewConnectionsFlagSet(base.CurrentOrganizationId))
			{
				base.WriteError(new RecipientTaskException(new LocalizedString(ServerStrings.PublicFolderMailboxesCannotBeMovedDuringMigration)), ErrorCategory.InvalidOperation, this.Identity);
			}
		}

		private void DisallowTeamMailboxesToPreE15()
		{
			if (this.PrimaryIsMoving && this.sourceMbxInfo.RecipientTypeDetailsLong == 137438953472L && this.targetMbxInfo.ServerVersion < Server.E15MinVersion)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorCannotMoveTeamMailboxesDownlevel(this.adUser.Name)), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void CheckTeamMailboxesCrossForestOrCrossPremise()
		{
			if (this.MoveRequestIs(RequestFlags.CrossOrg) && this.PrimaryIsMoving && this.sourceMbxInfo.RecipientTypeDetailsLong == 137438953472L)
			{
				this.WriteWarning(Microsoft.Exchange.Management.Tasks.Strings.WarningMovingTeamMailboxesCrossForestOrCrossPremise(this.adUser.Name));
			}
		}

		private void DisallowDiscoveryMailboxesCrossForest()
		{
			if (this.MoveRequestIs(RequestFlags.CrossOrg) && this.PrimaryIsMoving && this.sourceMbxInfo.RecipientTypeDetailsLong == 536870912L)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorCannotMoveDiscoveryMailboxesCrossForest(this.adUser.Name)), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void DisallowUCSMigratedMailboxesToPreE15()
		{
			if (this.PrimaryIsMoving && this.targetMbxInfo.ServerVersion < Server.E15MinVersion && (bool)this.SourceUser[ADRecipientSchema.UCSImListMigrationCompleted])
			{
				base.WriteError(new UnableToMoveUCSMigratedMailboxToDownlevelException(this.adUser.Name), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void DisallowMailboxMoveWithSubscriptions()
		{
			if (this.PrimaryIsMoving && this.CheckOnboardingOffboarding() && this.TargetIsRemote && this.sourceMbxInfo.ContentAggregationFlags == 1)
			{
				base.WriteError(new UnableToMoveMailboxWithSubscriptionsException(this.adUser.Name), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void EnsureSupportedServerVersion(int serverVersion, bool isSourceDatabase)
		{
			bool flag = serverVersion >= Server.E15MinVersion;
			bool flag2 = serverVersion >= Server.E14MinVersion && serverVersion < Server.E15MinVersion;
			bool flag3 = serverVersion >= Server.E2007SP2MinVersion && serverVersion < Server.E14MinVersion;
			bool flag4 = serverVersion >= Server.E2k3SP2MinVersion && serverVersion < Server.E2007MinVersion;
			bool flag5;
			if (isSourceDatabase)
			{
				flag5 = (flag || flag2 || flag3 || (MapiTaskHelper.IsDatacenter && flag4));
			}
			else
			{
				flag5 = (flag || flag2 || flag3);
			}
			if (!flag5)
			{
				if (isSourceDatabase)
				{
					base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorMovingOldExchangeUsersUnsupported), ErrorCategory.InvalidArgument, this.Identity);
					return;
				}
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorMovingToOldExchangeDatabaseUnsupported), ErrorCategory.InvalidArgument, this.TargetDatabase);
			}
		}

		private void EnsureSupportedServerVersionForArchiveScenarios(int serverVersion, bool isSourceDatabase)
		{
			if (serverVersion < Server.E14MinVersion)
			{
				if (isSourceDatabase)
				{
					base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorMovingUnsupportedArchiveUser), ErrorCategory.InvalidArgument, this.Identity);
					return;
				}
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorCannotMoveArchiveMailboxesDownlevel(this.adUser.Name)), ErrorCategory.InvalidArgument, this.TargetDatabase);
			}
		}

		private bool SupportsOnline(int sourceVersion, int targetVersion)
		{
			int e15MinVersion = Server.E15MinVersion;
			if (sourceVersion >= Server.E14MinVersion)
			{
				int e15MinVersion2 = Server.E15MinVersion;
			}
			if (sourceVersion >= Server.E2007MinVersion)
			{
				int e14MinVersion = Server.E14MinVersion;
			}
			bool flag = sourceVersion >= Server.E2k3MinVersion && sourceVersion < Server.E2007MinVersion;
			int e15MinVersion3 = Server.E15MinVersion;
			if (targetVersion >= Server.E14MinVersion)
			{
				int e15MinVersion4 = Server.E15MinVersion;
			}
			bool flag2 = targetVersion >= Server.E2007MinVersion && targetVersion < Server.E14MinVersion;
			bool flag3 = targetVersion >= Server.E2k3MinVersion && targetVersion < Server.E2007MinVersion;
			bool flag4 = flag2 || flag || flag3;
			return !flag4;
		}

		private MailboxDatabase LocateAndVerifyMdb(DatabaseIdParameter databaseId, bool isSourceDatabase, out int serverVersion)
		{
			ITopologyConfigurationSession configSessionForDatabase = RequestTaskHelper.GetConfigSessionForDatabase(this.globalConfigSession, databaseId.InternalADObjectId);
			MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(databaseId, configSessionForDatabase, null, new LocalizedString?(Microsoft.Exchange.Management.Tasks.Strings.ErrorMailboxDatabaseNotFound(databaseId.ToString())), new LocalizedString?(Microsoft.Exchange.Management.Tasks.Strings.ErrorMailboxDatabaseNotUnique(databaseId.ToString())));
			serverVersion = MapiUtils.FindServerForMdb(mailboxDatabase.Id, null, null, FindServerFlags.None).ServerVersion;
			this.EnsureSupportedServerVersion(serverVersion, isSourceDatabase);
			if (!isSourceDatabase && mailboxDatabase.Recovery)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorTargetDatabaseIsRecovery(mailboxDatabase.ToString())), ErrorCategory.InvalidArgument, this.Identity);
			}
			return mailboxDatabase;
		}

		private void EnsureDatabaseAttributesMatchUser(MailboxDatabase targetMdb)
		{
			if (this.adUser == null || this.adUser.MailboxProvisioningConstraint == null)
			{
				return;
			}
			if (targetMdb.MailboxProvisioningAttributes == null || !this.adUser.MailboxProvisioningConstraint.IsMatch(targetMdb.MailboxProvisioningAttributes))
			{
				base.WriteError(new MailboxConstraintsMismatchException(this.adUser.ToString(), targetMdb.Name, this.adUser.MailboxProvisioningConstraint.Value), ErrorCategory.InvalidData, this.Identity);
			}
		}

		private MailboxDatabase ChooseTargetMDB(ADObjectId sourceMDB)
		{
			bool checkInitialProvisioningSetting = this.CheckInitialProvisioningSetting;
			if (!this.IsFieldSet("CheckInitialProvisioningSetting") && VariantConfiguration.InvariantNoFlightingSnapshot.Mrs.UseDefaultValueForCheckInitialProvisioningForMovesParameter.Enabled)
			{
				checkInitialProvisioningSetting = TestIntegration.Instance.CheckInitialProvisioningForMoves;
			}
			return RequestTaskHelper.ChooseTargetMDB(new ADObjectId[]
			{
				sourceMDB
			}, checkInitialProvisioningSetting, this.adUser, this.DomainController, base.ScopeSet, new Action<LocalizedString>(base.WriteVerbose), new Action<LocalizedException, ExchangeErrorCategory, object>(base.WriteError), new Action<Exception, ErrorCategory, object>(base.WriteError), this.Identity);
		}

		private ADObjectId GetMdbQueue()
		{
			ADObjectId adobjectId;
			if (this.MoveRequestIs(RequestFlags.Pull))
			{
				if (this.ArchiveOnly)
				{
					adobjectId = this.targetDatabaseForMailboxArchive;
				}
				else
				{
					adobjectId = this.targetDatabaseForMailbox;
				}
			}
			else if (this.ArchiveOnly)
			{
				adobjectId = this.adUser.ArchiveDatabase;
			}
			else
			{
				adobjectId = this.adUser.Database;
			}
			if (adobjectId == null)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorUnableToDetermineMdbQueue(this.Identity.ToString())), ErrorCategory.InvalidArgument, this.Identity);
			}
			return adobjectId;
		}

		private void CheckForDuplicateMR(bool verifyOnly, TransactionalRequestJob newMR)
		{
			RequestJobQueryFilter filter = new RequestJobQueryFilter(this.adUser.ExchangeGuid, this.mrProvider.MdbGuid, MRSRequestType.Move);
			IEnumerable<MoveRequestStatistics> enumerable = this.mrProvider.FindPaged<MoveRequestStatistics>(filter, null, true, null, 100);
			RequestJobObjectId requestJobObjectId = null;
			bool flag = false;
			using (IEnumerator<MoveRequestStatistics> enumerator = enumerable.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					for (;;)
					{
						MoveRequestStatistics moveRequestStatistics = enumerator.Current;
						if (!moveRequestStatistics.CancelRequest)
						{
							goto IL_CC;
						}
						if (!verifyOnly)
						{
							using (TransactionalRequestJob transactionalRequestJob = (TransactionalRequestJob)this.mrProvider.Read<TransactionalRequestJob>(moveRequestStatistics.Identity))
							{
								if (transactionalRequestJob != null && !transactionalRequestJob.IsFake)
								{
									MrsTracer.Cmdlet.Warning("Removing existing canceled move request.", new object[0]);
									this.mrProvider.Delete(transactionalRequestJob);
								}
								goto IL_11F;
							}
							goto IL_CC;
						}
						MrsTracer.Cmdlet.Warning("Canceled MoveRequest will be overwritten.", new object[0]);
						IL_11F:
						if (!enumerator.MoveNext())
						{
							break;
						}
						continue;
						IL_CC:
						if (newMR != null && moveRequestStatistics.RequestGuid == newMR.RequestGuid)
						{
							requestJobObjectId = new RequestJobObjectId(moveRequestStatistics.RequestGuid, moveRequestStatistics.RequestQueue.ObjectGuid, null);
							if (flag)
							{
								break;
							}
							goto IL_11F;
						}
						else
						{
							if (verifyOnly)
							{
								goto IL_11F;
							}
							MrsTracer.Cmdlet.Warning("Someone managed to create another MoveRequest.  The MoveRequest we just created will be removed.", new object[0]);
							flag = true;
							if (requestJobObjectId == null)
							{
								goto IL_11F;
							}
							break;
						}
					}
					if (flag && requestJobObjectId != null)
					{
						using (TransactionalRequestJob transactionalRequestJob2 = (TransactionalRequestJob)this.mrProvider.Read<TransactionalRequestJob>(requestJobObjectId))
						{
							if (transactionalRequestJob2 != null && !transactionalRequestJob2.IsFake)
							{
								MrsTracer.Cmdlet.Warning("Removing our newly created move request because someone managed to create another one.", new object[0]);
								this.mrProvider.Delete(transactionalRequestJob2);
							}
						}
						base.WriteError(new ManagementObjectAlreadyExistsException(Microsoft.Exchange.Management.Tasks.Strings.ErrorMoveRequestAlreadyExistsInMDBQueue(this.GetMdbQueue().ToString(), this.adUser.ToString(), this.adUser.ExchangeGuid)), ErrorCategory.InvalidArgument, this.Identity);
					}
				}
			}
		}

		private void GetMailboxQuotas(MailboxInformation mbxInfo, bool isArchive, bool isTarget, out ulong mbxQuota, out ulong dumpsterQuota)
		{
			if (isTarget && this.SourceIsRemote && this.PrimaryIsMoving && ConfigBase<MRSConfigSchema>.GetConfig<bool>("CheckMailUserPlanQuotasForOnboarding"))
			{
				MailboxInformation mailboxInformation = new MailboxInformation();
				mailboxInformation.MdbQuota = mbxInfo.MdbQuota;
				ADUser aduser = (ADUser)this.TargetUser.Clone();
				ADUser aduser2 = (ADUser)this.SourceUser.Clone();
				MrsTracer.Cmdlet.Debug("Getting target mailbox quotas from in-memory application of mailbox plan to AD user...", new object[0]);
				if (base.IsProvisioningLayerAvailable)
				{
					aduser.RecipientTypeDetails = RecipientTypeDetails.UserMailbox;
					ProvisioningLayer.UpdateAffectedIConfigurable(this, this.ConvertDataObjectToPresentationObject(aduser), false);
				}
				MailboxTaskHelper.ApplyMbxPlanSettingsInTargetForest(aduser, (ADObjectId mbxPlanId) => (ADUser)base.GetDataObject<ADUser>(new MailboxPlanIdParameter(mbxPlanId), base.TenantGlobalCatalogSession, null, new LocalizedString?(Microsoft.Exchange.Management.Tasks.Strings.ErrorMailboxPlanNotFound(mbxPlanId.ToString())), new LocalizedString?(Microsoft.Exchange.Management.Tasks.Strings.ErrorMailboxPlanNotUnique(mbxPlanId.ToString()))), ApplyMailboxPlanFlags.PreservePreviousExplicitlySetValues);
				mailboxInformation.MailboxQuota = (aduser.ProhibitSendReceiveQuota.IsUnlimited ? null : new ulong?(aduser.ProhibitSendReceiveQuota.Value.ToBytes()));
				mailboxInformation.MailboxArchiveQuota = (aduser.ArchiveQuota.IsUnlimited ? null : new ulong?(aduser.ArchiveQuota.Value.ToBytes()));
				mailboxInformation.MailboxDumpsterQuota = (aduser.RecoverableItemsQuota.IsUnlimited ? null : new ulong?(aduser.RecoverableItemsQuota.Value.ToBytes()));
				base.WriteVerbose(Microsoft.Exchange.Management.Tasks.Strings.MailboxQuotaValues("TargetUser-original", this.TargetUser.ProhibitSendReceiveQuota.IsUnlimited ? new ByteQuantifiedSize(ulong.MaxValue).ToString() : this.TargetUser.ProhibitSendReceiveQuota.Value.ToString(), this.TargetUser.ArchiveQuota.IsUnlimited ? new ByteQuantifiedSize(ulong.MaxValue).ToString() : this.TargetUser.ArchiveQuota.Value.ToString(), this.TargetUser.RecoverableItemsQuota.IsUnlimited ? new ByteQuantifiedSize(ulong.MaxValue).ToString() : this.TargetUser.RecoverableItemsQuota.Value.ToString()));
				mbxInfo = mailboxInformation;
			}
			ulong? num;
			ulong? num2;
			if (isArchive)
			{
				num = mbxInfo.MailboxArchiveQuota;
				num2 = mbxInfo.MailboxDumpsterQuota;
			}
			else if (mbxInfo.UseMdbQuotaDefaults != null && mbxInfo.UseMdbQuotaDefaults == true)
			{
				num = mbxInfo.MdbQuota;
				num2 = mbxInfo.MdbDumpsterQuota;
			}
			else
			{
				num = mbxInfo.MailboxQuota;
				num2 = mbxInfo.MailboxDumpsterQuota;
			}
			base.WriteVerbose(Microsoft.Exchange.Management.Tasks.Strings.MailboxQuotaValues(isTarget ? "Effective-Target" : "Effective-Source", (num == null) ? new ByteQuantifiedSize(ulong.MaxValue).ToString() : new ByteQuantifiedSize(num.Value).ToString(), "N.A.", (num2 == null) ? new ByteQuantifiedSize(ulong.MaxValue).ToString() : new ByteQuantifiedSize(num2.Value).ToString()));
			mbxQuota = ((num != null) ? num.Value : ulong.MaxValue);
			dumpsterQuota = ((num2 != null) ? num2.Value : ulong.MaxValue);
		}

		private void VerifyMailboxQuota(MailboxInformation sourceInfo, MailboxInformation targetInfo, bool isArchive)
		{
			ulong num;
			ulong num2;
			this.GetMailboxQuotas(targetInfo, isArchive, true, out num, out num2);
			ulong num3;
			ulong num4;
			this.GetMailboxQuotas(sourceInfo, isArchive, false, out num3, out num4);
			ulong num5 = (sourceInfo.MailboxSize > sourceInfo.RegularDeletedItemsSize) ? (sourceInfo.MailboxSize - sourceInfo.RegularDeletedItemsSize) : 0UL;
			if (num5 > num)
			{
				ByteQuantifiedSize byteQuantifiedSize = new ByteQuantifiedSize(num);
				ByteQuantifiedSize byteQuantifiedSize2 = new ByteQuantifiedSize(num5);
				LocalizedString message = isArchive ? Microsoft.Exchange.Management.Tasks.Strings.ErrorArchiveExceedsTargetQuota(byteQuantifiedSize2.ToString(), byteQuantifiedSize.ToString()) : Microsoft.Exchange.Management.Tasks.Strings.ErrorMailboxExceedsTargetQuota(byteQuantifiedSize2.ToString(), byteQuantifiedSize.ToString());
				base.WriteError(new RecipientTaskException(message), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (sourceInfo.ServerVersion >= Server.E14MinVersion && num4 > num2)
			{
				ulong regularDeletedItemsSize = sourceInfo.RegularDeletedItemsSize;
				if (regularDeletedItemsSize > num2)
				{
					ByteQuantifiedSize byteQuantifiedSize3 = new ByteQuantifiedSize(num2);
					ByteQuantifiedSize byteQuantifiedSize4 = new ByteQuantifiedSize(regularDeletedItemsSize);
					LocalizedString message2 = isArchive ? Microsoft.Exchange.Management.Tasks.Strings.ErrorArchiveDumpsterExceedsTargetQuota(byteQuantifiedSize4.ToString(), byteQuantifiedSize3.ToString()) : Microsoft.Exchange.Management.Tasks.Strings.ErrorMailboxDumpsterExceedsTargetQuota(byteQuantifiedSize4.ToString(), byteQuantifiedSize3.ToString());
					base.WriteError(new RecipientTaskException(message2), ErrorCategory.InvalidArgument, this.Identity);
				}
			}
		}

		private void CheckMailboxRelease()
		{
			MailboxRelease requiredMailboxRelease = MailboxTaskHelper.ComputeRequiredMailboxRelease(this.ConfigurationSession, this.adUser, (ExchangeConfigurationUnit)this.orgConfig, new Task.ErrorLoggerDelegate(base.WriteError));
			if (this.PrimaryIsMoving)
			{
				MailboxRelease targetServerMailboxRelease = MailboxRelease.None;
				Enum.TryParse<MailboxRelease>(this.targetMbxInfo.ServerMailboxRelease, true, out targetServerMailboxRelease);
				MailboxTaskHelper.ValidateMailboxRelease(targetServerMailboxRelease, requiredMailboxRelease, this.adUser.Id.ToString(), this.targetMbxInfo.MdbName, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			if (this.ArchiveIsMoving)
			{
				MailboxRelease targetServerMailboxRelease2 = MailboxRelease.None;
				Enum.TryParse<MailboxRelease>(this.targetArchiveInfo.ServerMailboxRelease, true, out targetServerMailboxRelease2);
				MailboxTaskHelper.ValidateMailboxRelease(targetServerMailboxRelease2, requiredMailboxRelease, this.adUser.Id.ToString(), this.targetArchiveInfo.MdbName, new Task.ErrorLoggerDelegate(base.WriteError));
			}
		}

		private void ValidateUMSettings()
		{
			IRecipientSession tenantLocalRecipientSession = RecipientTaskHelper.GetTenantLocalRecipientSession(this.TargetUser.OrganizationId, base.ExecutingUserOrganizationId, base.RootOrgContainerId);
			try
			{
				MigrationHelper.ValidateTargetUserCanBeEnabledForUM(tenantLocalRecipientSession, this.tenantLocalConfigSession, Datacenter.IsMicrosoftHostedOnly(true), this.SourceUser, this.TargetUser);
			}
			catch (LocalizedException ex)
			{
				base.WriteError(new RecipientTaskException(Microsoft.Exchange.Management.Tasks.Strings.ErrorCannotUMEnableInTarget(this.SourceUser.ToString(), ex.LocalizedString), ex), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private bool CheckOnboardingOffboarding()
		{
			return Datacenter.IsMicrosoftHostedOnly(true) && this.adUser != null && this.adUser.IsFromDatacenter && this.adUser.OrganizationId != null && !this.adUser.OrganizationId.Equals(OrganizationId.ForestWideOrgId) && (this.SourceIsRemote || this.TargetIsRemote);
		}

		private Organization GetOrganization()
		{
			if (this.orgConfig == null)
			{
				if (!this.CheckOnboardingOffboarding())
				{
					return null;
				}
				ADObjectId organizationalUnit = this.adUser.OrganizationId.OrganizationalUnit;
				ADObjectId configurationUnit = this.adUser.OrganizationId.ConfigurationUnit;
				this.orgConfig = this.tenantLocalConfigSession.Read<ExchangeConfigurationUnit>(configurationUnit);
			}
			return this.orgConfig;
		}

		private RequestPriority ComputePriority()
		{
			RequestPriority result = RequestPriority.Normal;
			if (base.Fields.IsModified("Priority"))
			{
				result = this.Priority;
			}
			else
			{
				Organization organization = this.GetOrganization();
				if (organization != null && organization.DefaultMovePriority > 0)
				{
					result = (RequestPriority)organization.DefaultMovePriority;
				}
			}
			return result;
		}

		private void EnsurePrimaryOnlyIsPresentWhenOnboardingWithCloudArchive()
		{
			if (this.TargetHasArchive && !this.PrimaryOnly && this.SourceIsRemote)
			{
				LocalizedString message = MrsStrings.CompositeDataContext(Microsoft.Exchange.Management.Tasks.Strings.ErrorParameterValueRequired("PrimaryOnly"), Microsoft.Exchange.Management.Tasks.Strings.ErrorTargetUserAlreadyHasArchiveMailbox(this.adUser.ToString()));
				base.WriteError(new RecipientTaskException(message), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private bool IsFieldSet(string fieldName)
		{
			return base.Fields.IsChanged(fieldName) || base.Fields.IsModified(fieldName);
		}

		private bool InternalFlagsContains(InternalMrsFlag flag)
		{
			if (this.InternalFlags == null)
			{
				return false;
			}
			foreach (InternalMrsFlag internalMrsFlag in this.InternalFlags)
			{
				if (internalMrsFlag == flag)
				{
					return true;
				}
			}
			return false;
		}

		public const string ParameterSetRemote = "MigrationRemote";

		public const string ParameterSetOutbound = "MigrationOutbound";

		public const string ParameterSetRemoteLegacy = "MigrationRemoteLegacy";

		public const string ParameterSetLocal = "MigrationLocal";

		private static ADPropertyDefinition[] simpleSearchResult = new ADPropertyDefinition[]
		{
			ADObjectSchema.Guid
		};

		private static TimeSpan defaultIncrementalSyncIntervalForMove = TimeSpan.FromDays(1.0);

		private MailboxDatabase specifiedTargetMDB;

		private MailboxDatabase specifiedArchiveTargetMDB;

		private ADUser adUser;

		private ADUser remoteADUser;

		private MailboxInformation sourceMbxInfo;

		private MailboxInformation sourceArchiveInfo;

		private MailboxInformation targetMbxInfo;

		private MailboxInformation targetArchiveInfo;

		private string remoteOrgName;

		private RequestFlags moveFlags;

		private ADObjectId targetDatabaseForMailbox;

		private ADObjectId targetDatabaseForMailboxArchive;

		private MailboxReplicationServiceClient mrsClient;

		private IRecipientSession gcSession;

		private IRecipientSession adSession;

		private ITopologyConfigurationSession globalConfigSession;

		private IConfigurationSession tenantLocalConfigSession;

		private RequestJobProvider mrProvider;

		private List<string> unreachableMrsServers;

		private NetworkCredential remoteCred;

		private ADSessionSettings sessionSettings;

		private Organization orgConfig;
	}
}
