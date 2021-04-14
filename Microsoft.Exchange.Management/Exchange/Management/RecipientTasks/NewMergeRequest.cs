using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "MergeRequest", SupportsShouldProcess = true, DefaultParameterSetName = "MigrationLocalMerge")]
	public sealed class NewMergeRequest : NewRequest<MergeRequest>
	{
		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "MigrationLocalMerge")]
		public MailboxOrMailUserIdParameter SourceMailbox
		{
			get
			{
				return (MailboxOrMailUserIdParameter)base.Fields["SourceMailbox"];
			}
			set
			{
				base.Fields["SourceMailbox"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MigrationLocalMerge")]
		[Parameter(Mandatory = true, ParameterSetName = "MigrationOutlookAnywhereMergePull")]
		[ValidateNotNull]
		public MailboxOrMailUserIdParameter TargetMailbox
		{
			get
			{
				return (MailboxOrMailUserIdParameter)base.Fields["TargetMailbox"];
			}
			set
			{
				base.Fields["TargetMailbox"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMerge")]
		public string SourceRootFolder
		{
			get
			{
				return (string)base.Fields["SourceRootFolder"];
			}
			set
			{
				base.Fields["SourceRootFolder"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMerge")]
		public string TargetRootFolder
		{
			get
			{
				return (string)base.Fields["TargetRootFolder"];
			}
			set
			{
				base.Fields["TargetRootFolder"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMerge")]
		public SwitchParameter SourceIsArchive
		{
			get
			{
				return (SwitchParameter)(base.Fields["SourceIsArchive"] ?? false);
			}
			set
			{
				base.Fields["SourceIsArchive"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMerge")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutlookAnywhereMergePull")]
		public SwitchParameter TargetIsArchive
		{
			get
			{
				return (SwitchParameter)(base.Fields["TargetIsArchive"] ?? false);
			}
			set
			{
				base.Fields["TargetIsArchive"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MigrationOutlookAnywhereMergePull")]
		public string RemoteSourceMailboxLegacyDN
		{
			get
			{
				return (string)base.Fields["RemoteSourceMailboxLegacyDN"];
			}
			set
			{
				base.Fields["RemoteSourceMailboxLegacyDN"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutlookAnywhereMergePull")]
		public string RemoteSourceUserLegacyDN
		{
			get
			{
				return (string)base.Fields["RemoteSourceUserLegacyDN"];
			}
			set
			{
				base.Fields["RemoteSourceUserLegacyDN"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MigrationOutlookAnywhereMergePull")]
		public string RemoteSourceMailboxServerLegacyDN
		{
			get
			{
				return (string)base.Fields["RemoteSourceMailboxServerLegacyDN"];
			}
			set
			{
				base.Fields["RemoteSourceMailboxServerLegacyDN"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutlookAnywhereMergePull")]
		public Fqdn OutlookAnywhereHostName
		{
			get
			{
				return (Fqdn)base.Fields["OutlookAnywhereHostName"];
			}
			set
			{
				base.Fields["OutlookAnywhereHostName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutlookAnywhereMergePull")]
		public bool IsAdministrativeCredential
		{
			get
			{
				return (bool)(base.Fields["IsAdministrativeCredential"] ?? false);
			}
			set
			{
				base.Fields["IsAdministrativeCredential"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutlookAnywhereMergePull")]
		public AuthenticationMethod AuthenticationMethod
		{
			get
			{
				return (AuthenticationMethod)(base.Fields["AuthenticationMethod"] ?? AuthenticationMethod.Basic);
			}
			set
			{
				base.Fields["AuthenticationMethod"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutlookAnywhereMergePull")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMerge")]
		public bool SuspendWhenReadyToComplete
		{
			get
			{
				return (bool)(base.Fields["SuspendWhenReadyToComplete"] ?? false);
			}
			set
			{
				base.Fields["SuspendWhenReadyToComplete"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TimeSpan IncrementalSyncInterval
		{
			get
			{
				return (TimeSpan)(base.Fields["IncrementalSyncInterval"] ?? TimeSpan.Zero);
			}
			set
			{
				base.Fields["IncrementalSyncInterval"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MigrationOutlookAnywhereMergePull")]
		public new PSCredential RemoteCredential
		{
			get
			{
				return base.RemoteCredential;
			}
			set
			{
				base.RemoteCredential = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMerge")]
		public new SwitchParameter AllowLegacyDNMismatch
		{
			get
			{
				return base.AllowLegacyDNMismatch;
			}
			set
			{
				base.AllowLegacyDNMismatch = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMerge")]
		public new string ContentFilter
		{
			get
			{
				return base.ContentFilter;
			}
			set
			{
				base.ContentFilter = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMerge")]
		public new CultureInfo ContentFilterLanguage
		{
			get
			{
				return base.ContentFilterLanguage;
			}
			set
			{
				base.ContentFilterLanguage = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMerge")]
		public new string[] IncludeFolders
		{
			get
			{
				return base.IncludeFolders;
			}
			set
			{
				base.IncludeFolders = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMerge")]
		public new string[] ExcludeFolders
		{
			get
			{
				return base.ExcludeFolders;
			}
			set
			{
				base.ExcludeFolders = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMerge")]
		public new SwitchParameter ExcludeDumpster
		{
			get
			{
				return base.ExcludeDumpster;
			}
			set
			{
				base.ExcludeDumpster = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMerge")]
		public new ConflictResolutionOption ConflictResolutionOption
		{
			get
			{
				return base.ConflictResolutionOption;
			}
			set
			{
				base.ConflictResolutionOption = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMerge")]
		public new FAICopyOption AssociatedMessagesCopyOption
		{
			get
			{
				return base.AssociatedMessagesCopyOption;
			}
			set
			{
				base.AssociatedMessagesCopyOption = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMerge")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutlookAnywhereMergePull")]
		public new DateTime StartAfter
		{
			get
			{
				return base.StartAfter;
			}
			set
			{
				base.StartAfter = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewMergeRequest(base.RequestName);
			}
		}

		protected override void InternalStateReset()
		{
			this.sourceUser = null;
			this.targetUser = null;
			base.InternalStateReset();
		}

		protected override void InternalBeginProcessing()
		{
			if (base.WorkloadType == RequestWorkloadType.None)
			{
				if (base.ParameterSetName.Equals("MigrationLocalMerge"))
				{
					base.WorkloadType = RequestWorkloadType.Local;
				}
				else
				{
					base.WorkloadType = RequestWorkloadType.Onboarding;
				}
			}
			base.InternalBeginProcessing();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.ValidateRootFolders(this.SourceRootFolder, this.TargetRootFolder);
				if (this.SourceMailbox != null)
				{
					this.sourceUser = RequestTaskHelper.ResolveADUser(base.RecipSession, base.GCSession, base.ServerSettings, this.SourceMailbox, base.OptionalIdentityData, base.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), true);
					if (this.SourceIsArchive && (this.sourceUser.ArchiveGuid == Guid.Empty || this.sourceUser.ArchiveDatabase == null))
					{
						base.WriteError(new MailboxLacksArchivePermanentException(this.sourceUser.ToString()), ErrorCategory.InvalidArgument, this.SourceIsArchive);
					}
					if (!this.SourceIsArchive && this.sourceUser.Database == null)
					{
						base.WriteError(new MailboxLacksDatabasePermanentException(this.sourceUser.ToString()), ErrorCategory.InvalidArgument, this.SourceMailbox);
					}
				}
				else
				{
					this.sourceUser = null;
				}
				if (this.TargetMailbox != null)
				{
					this.targetUser = RequestTaskHelper.ResolveADUser(base.RecipSession, base.GCSession, base.ServerSettings, this.TargetMailbox, base.OptionalIdentityData, base.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), true);
					if (this.TargetIsArchive && (this.targetUser.ArchiveGuid == Guid.Empty || this.targetUser.ArchiveDatabase == null))
					{
						base.WriteError(new MailboxLacksArchivePermanentException(this.targetUser.ToString()), ErrorCategory.InvalidArgument, this.TargetIsArchive);
					}
					if (!this.TargetIsArchive && this.targetUser.Database == null)
					{
						base.WriteError(new MailboxLacksDatabasePermanentException(this.targetUser.ToString()), ErrorCategory.InvalidArgument, this.TargetMailbox);
					}
				}
				else
				{
					this.targetUser = null;
				}
				this.DisallowMergeRequestForPublicFolderMailbox();
				bool wildcardedSearch = false;
				if (!string.IsNullOrEmpty(base.Name))
				{
					base.ValidateName();
					base.RequestName = base.Name;
				}
				else
				{
					wildcardedSearch = true;
					base.RequestName = "Merge";
				}
				AuthenticationMethod authenticationMethod = this.AuthenticationMethod;
				switch (authenticationMethod)
				{
				case AuthenticationMethod.Basic:
				case AuthenticationMethod.Ntlm:
					goto IL_282;
				case AuthenticationMethod.Digest:
					break;
				default:
					if (authenticationMethod == AuthenticationMethod.LiveIdBasic)
					{
						goto IL_282;
					}
					break;
				}
				base.WriteError(new UnsupportedAuthMethodPermanentException(this.AuthenticationMethod.ToString()), ErrorCategory.InvalidArgument, this.AuthenticationMethod);
				IL_282:
				if (base.ParameterSetName.Equals("MigrationLocalMerge"))
				{
					if (!object.Equals(this.sourceUser.OrganizationId, this.targetUser.OrganizationId))
					{
						base.WriteError(new UsersNotInSameOrganizationPermanentException(this.sourceUser.ToString(), this.targetUser.ToString()), ErrorCategory.InvalidArgument, this.TargetMailbox);
					}
					base.RescopeToOrgId(this.sourceUser.OrganizationId);
					base.ValidateLegacyDNMatch(this.sourceUser.LegacyExchangeDN, this.targetUser, this.TargetMailbox);
					ADObjectId mdbId = null;
					ADObjectId mdbServerSite = null;
					RequestFlags requestFlags = this.LocateAndChooseMdb(this.SourceIsArchive ? this.sourceUser.ArchiveDatabase : this.sourceUser.Database, this.TargetIsArchive ? this.targetUser.ArchiveDatabase : this.targetUser.Database, this.SourceMailbox, this.TargetMailbox, this.TargetMailbox, out mdbId, out mdbServerSite);
					base.MdbId = mdbId;
					base.MdbServerSite = mdbServerSite;
					base.Flags = (RequestFlags.IntraOrg | requestFlags);
				}
				else
				{
					base.RescopeToOrgId(this.targetUser.OrganizationId);
					ADObjectId mdbId2 = null;
					ADObjectId mdbServerSite2 = null;
					this.LocateAndChooseMdb(null, this.TargetIsArchive ? this.targetUser.ArchiveDatabase : this.targetUser.Database, null, this.TargetMailbox, this.TargetMailbox, out mdbId2, out mdbServerSite2);
					base.MdbId = mdbId2;
					base.MdbServerSite = mdbServerSite2;
					base.Flags = (RequestFlags.CrossOrg | RequestFlags.Pull);
				}
				ADUser aduser;
				if ((base.Flags & RequestFlags.Pull) == RequestFlags.Pull)
				{
					aduser = this.targetUser;
				}
				else
				{
					aduser = this.sourceUser;
				}
				base.RequestName = this.CheckRequestNameAvailability(base.RequestName, aduser.Id, true, MRSRequestType.Merge, this.TargetMailbox, wildcardedSearch);
				if (base.IsFieldSet("IncrementalSyncInterval") && base.IsFieldSet("SuspendWhenReadyToComplete") && this.SuspendWhenReadyToComplete)
				{
					base.WriteError(new SuspendWhenReadyToCompleteCannotBeSetWithIncrementalSyncIntervalException(), ErrorCategory.InvalidArgument, this.SuspendWhenReadyToComplete);
				}
				DateTime utcNow = DateTime.UtcNow;
				if (base.IsFieldSet("StartAfter"))
				{
					RequestTaskHelper.ValidateStartAfterTime(this.StartAfter.ToUniversalTime(), new Task.TaskErrorLoggingDelegate(base.WriteError), utcNow);
				}
				if (base.IsFieldSet("IncrementalSyncInterval"))
				{
					RequestTaskHelper.ValidateIncrementalSyncInterval(this.IncrementalSyncInterval, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
				base.InternalValidate();
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void SetRequestProperties(TransactionalRequestJob dataObject)
		{
			base.SetRequestProperties(dataObject);
			dataObject.RequestType = MRSRequestType.Merge;
			if (this.sourceUser != null)
			{
				dataObject.SourceUserId = this.sourceUser.Id;
				dataObject.SourceUser = this.sourceUser;
				dataObject.SourceIsArchive = this.SourceIsArchive;
				if (this.SourceIsArchive)
				{
					dataObject.SourceExchangeGuid = this.sourceUser.ArchiveGuid;
					dataObject.SourceDatabase = ADObjectIdResolutionHelper.ResolveDN(this.sourceUser.ArchiveDatabase);
				}
				else
				{
					dataObject.SourceExchangeGuid = this.sourceUser.ExchangeGuid;
					dataObject.SourceDatabase = ADObjectIdResolutionHelper.ResolveDN(this.sourceUser.Database);
				}
				dataObject.SourceAlias = this.sourceUser.Alias;
			}
			if (this.targetUser != null)
			{
				dataObject.TargetUserId = this.targetUser.Id;
				dataObject.TargetUser = this.targetUser;
				dataObject.TargetIsArchive = this.TargetIsArchive;
				if (this.TargetIsArchive)
				{
					dataObject.TargetExchangeGuid = this.targetUser.ArchiveGuid;
					dataObject.TargetDatabase = ADObjectIdResolutionHelper.ResolveDN(this.targetUser.ArchiveDatabase);
				}
				else
				{
					dataObject.TargetExchangeGuid = this.targetUser.ExchangeGuid;
					dataObject.TargetDatabase = ADObjectIdResolutionHelper.ResolveDN(this.targetUser.Database);
				}
				dataObject.TargetAlias = this.targetUser.Alias;
			}
			if (base.ParameterSetName.Equals("MigrationLocalMerge"))
			{
				if (!string.IsNullOrEmpty(this.SourceRootFolder))
				{
					dataObject.SourceRootFolder = this.SourceRootFolder;
				}
				if (!string.IsNullOrEmpty(this.TargetRootFolder))
				{
					dataObject.TargetRootFolder = this.TargetRootFolder;
				}
			}
			else
			{
				dataObject.IsAdministrativeCredential = new bool?(this.IsAdministrativeCredential);
				dataObject.AuthenticationMethod = new AuthenticationMethod?(this.AuthenticationMethod);
				dataObject.RemoteMailboxLegacyDN = this.RemoteSourceMailboxLegacyDN;
				dataObject.RemoteUserLegacyDN = this.RemoteSourceUserLegacyDN;
				dataObject.RemoteMailboxServerLegacyDN = this.RemoteSourceMailboxServerLegacyDN;
				dataObject.OutlookAnywhereHostName = this.OutlookAnywhereHostName;
				dataObject.RemoteCredential = RequestTaskHelper.GetNetworkCredential(this.RemoteCredential, new AuthenticationMethod?(this.AuthenticationMethod));
				dataObject.IncludeFolders = this.IncludeListForIncrementalMerge;
				dataObject.ExcludeDumpster = false;
				dataObject.ExcludeFolders = null;
				dataObject.ContentFilter = null;
				dataObject.ConflictResolutionOption = new ConflictResolutionOption?(ConflictResolutionOption.KeepSourceItem);
				dataObject.AssociatedMessagesCopyOption = new FAICopyOption?(FAICopyOption.Copy);
			}
			dataObject.SuspendWhenReadyToComplete = this.SuspendWhenReadyToComplete;
			if (base.IsFieldSet("IncrementalSyncInterval"))
			{
				dataObject.IncrementalSyncInterval = this.IncrementalSyncInterval;
				dataObject.TimeTracker.SetTimestamp(RequestJobTimestamp.CompleteAfter, new DateTime?(DateTime.MaxValue));
				dataObject.JobType = MRSJobType.RequestJobE15_AutoResumeMerges;
			}
			if (this.SuspendWhenReadyToComplete || base.IsFieldSet("IncrementalSyncInterval"))
			{
				dataObject.IncludeFolders = this.IncludeListForIncrementalMerge;
				dataObject.ExcludeDumpster = false;
				dataObject.ExcludeFolders = null;
				dataObject.ContentFilter = null;
				dataObject.ConflictResolutionOption = new ConflictResolutionOption?(ConflictResolutionOption.KeepSourceItem);
				dataObject.AssociatedMessagesCopyOption = new FAICopyOption?(FAICopyOption.Copy);
				dataObject.AllowedToFinishMove = false;
			}
			if (base.IsFieldSet("StartAfter"))
			{
				RequestTaskHelper.SetStartAfter(new DateTime?(this.StartAfter), dataObject, null);
			}
		}

		protected override MergeRequest ConvertToPresentationObject(TransactionalRequestJob dataObject)
		{
			if (dataObject.IndexEntries != null && dataObject.IndexEntries.Count >= 1)
			{
				return new MergeRequest(dataObject.IndexEntries[0]);
			}
			base.WriteError(new RequestIndexEntriesAbsentPermanentException(base.RequestName), ErrorCategory.InvalidArgument, this.TargetMailbox);
			return null;
		}

		private void DisallowMergeRequestForPublicFolderMailbox()
		{
			if (this.sourceUser != null && this.sourceUser.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorDisallowMergeRequestForPublicFolderMailbox(this.sourceUser.Name)), ErrorCategory.InvalidArgument, this.SourceMailbox);
			}
			if (this.targetUser != null && this.targetUser.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorDisallowMergeRequestForPublicFolderMailbox(this.targetUser.Name)), ErrorCategory.InvalidArgument, this.TargetMailbox);
			}
		}

		public const string DefaultMergeName = "Merge";

		public const string TaskNoun = "MergeRequest";

		public const string ParameterSourceMailbox = "SourceMailbox";

		public const string ParameterTargetMailbox = "TargetMailbox";

		public const string ParameterSourceRootFolder = "SourceRootFolder";

		public const string ParameterTargetRootFolder = "TargetRootFolder";

		public const string ParameterSourceIsArchive = "SourceIsArchive";

		public const string ParameterTargetIsArchive = "TargetIsArchive";

		public const string ParameterRemoteSourceMailboxLegacyDN = "RemoteSourceMailboxLegacyDN";

		public const string ParameterRemoteSourceUserLegacyDN = "RemoteSourceUserLegacyDN";

		public const string ParameterRemoteSourceMailboxServerLegacyDN = "RemoteSourceMailboxServerLegacyDN";

		public const string ParameterOutlookAnywhereHostName = "OutlookAnywhereHostName";

		public const string ParameterIsAdministrativeCredential = "IsAdministrativeCredential";

		public const string ParameterAuthenticationMethod = "AuthenticationMethod";

		public const string ParameterSuspendWhenReadyToComplete = "SuspendWhenReadyToComplete";

		public const string ParameterIncrementalSyncInterval = "IncrementalSyncInterval";

		public const string ParameterSetLocalMerge = "MigrationLocalMerge";

		public const string ParameterSetOutlookAnywhereMergePull = "MigrationOutlookAnywhereMergePull";

		private readonly string[] IncludeListForIncrementalMerge = new string[]
		{
			FolderFilterParser.GetAlias(WellKnownFolderType.Root) + "/*"
		};

		private ADUser sourceUser;

		private ADUser targetUser;
	}
}
