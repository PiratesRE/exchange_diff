using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "MailboxRestoreRequest", SupportsShouldProcess = true, DefaultParameterSetName = "MigrationLocalMailboxRestore")]
	public sealed class NewMailboxRestoreRequest : NewRequest<MailboxRestoreRequest>
	{
		public bool IsPublicFolderMailboxRestore
		{
			get
			{
				return this.restoreFlags.HasFlag(MailboxRestoreType.PublicFolderMailbox);
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNull]
		public StoreMailboxIdParameter SourceStoreMailbox
		{
			get
			{
				return (StoreMailboxIdParameter)base.Fields["SourceStoreMailbox"];
			}
			set
			{
				base.Fields["SourceStoreMailbox"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MigrationLocalMailboxRestore")]
		[ValidateNotNull]
		public DatabaseIdParameter SourceDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["SourceDatabase"];
			}
			set
			{
				base.Fields["SourceDatabase"] = value;
			}
		}

		[Parameter(Mandatory = true)]
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

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
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

		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "RemoteMailboxRestore")]
		public Guid RemoteDatabaseGuid
		{
			get
			{
				return (Guid)base.Fields["RemoteDatabaseGuid"];
			}
			set
			{
				base.Fields["RemoteDatabaseGuid"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "RemoteMailboxRestore")]
		public RemoteRestoreType RemoteRestoreType
		{
			get
			{
				return (RemoteRestoreType)base.Fields["RemoteRestoreType"];
			}
			set
			{
				base.Fields["RemoteRestoreType"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "RemoteMailboxRestore")]
		public new Fqdn RemoteHostName
		{
			get
			{
				return base.RemoteHostName;
			}
			set
			{
				base.RemoteHostName = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RemoteMailboxRestore")]
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

		[Parameter(Mandatory = false, ParameterSetName = "RemoteMailboxRestore")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMailboxRestore")]
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

		[Parameter(Mandatory = false, ParameterSetName = "RemoteMailboxRestore")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMailboxRestore")]
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

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMailboxRestore")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoteMailboxRestore")]
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

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMailboxRestore")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoteMailboxRestore")]
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

		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMailboxRestore")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoteMailboxRestore")]
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

		[Parameter(Mandatory = false, ParameterSetName = "RemoteMailboxRestore")]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationLocalMailboxRestore")]
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewMailboxRestoreRequest(base.RequestName);
			}
		}

		protected override void InternalStateReset()
		{
			this.sourceMailboxDN = null;
			this.sourceMailboxGuid = Guid.Empty;
			this.sourceDatabase = null;
			this.restoreFlags = MailboxRestoreType.None;
			this.targetUser = null;
			base.InternalStateReset();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.ValidateRootFolders(this.SourceRootFolder, this.TargetRootFolder);
				bool wildcardedSearch = false;
				if (!string.IsNullOrEmpty(base.Name))
				{
					base.ValidateName();
					base.RequestName = base.Name;
				}
				else
				{
					wildcardedSearch = true;
					base.RequestName = "MailboxRestore";
				}
				this.targetUser = RequestTaskHelper.ResolveADUser(base.RecipSession, base.GCSession, base.ServerSettings, this.TargetMailbox, base.OptionalIdentityData, base.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), true);
				this.CheckForInvalidPublicFolderRestoreParameters();
				if (this.targetUser.HasLocalArchive && this.targetUser.RecipientType == RecipientType.MailUser && this.targetUser.Database == null && !this.TargetIsArchive)
				{
					base.WriteError(new MissingArchiveParameterForRestorePermanentException(this.targetUser.ToString()), ErrorCategory.InvalidArgument, this.TargetMailbox);
				}
				if (this.targetUser.RecipientType != RecipientType.UserMailbox && (!this.TargetIsArchive || this.targetUser.RecipientType != RecipientType.MailUser))
				{
					base.WriteError(new InvalidRecipientTypePermanentException(this.targetUser.ToString(), this.targetUser.RecipientType.ToString()), ErrorCategory.InvalidArgument, this.TargetMailbox);
				}
				if (this.TargetIsArchive && (this.targetUser.ArchiveGuid == Guid.Empty || this.targetUser.ArchiveDatabase == null))
				{
					base.WriteError(new MailboxLacksArchivePermanentException(this.targetUser.ToString()), ErrorCategory.InvalidArgument, this.TargetIsArchive);
				}
				if (!this.TargetIsArchive && this.targetUser.Database == null)
				{
					base.WriteError(new MailboxLacksDatabasePermanentException(this.targetUser.ToString()), ErrorCategory.InvalidArgument, this.TargetMailbox);
				}
				if (base.ParameterSetName.Equals("RemoteMailboxRestore"))
				{
					if (!Guid.TryParse(this.SourceStoreMailbox.RawIdentity, out this.sourceMailboxGuid))
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorParameterValueNotAllowed("SourceStoreMailbox")), ErrorCategory.InvalidArgument, this.SourceStoreMailbox);
					}
					if (!base.Fields.IsModified("AllowLegacyDNMismatch") || !this.AllowLegacyDNMismatch)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorParameterValueNotAllowed("AllowLegacyDNMismatch")), ErrorCategory.InvalidArgument, this.AllowLegacyDNMismatch);
					}
					base.Flags = (RequestFlags.CrossOrg | RequestFlags.Pull);
					switch (this.RemoteRestoreType)
					{
					case RemoteRestoreType.RecoveryDatabase:
						this.restoreFlags |= MailboxRestoreType.Recovery;
						this.restoreFlags |= MailboxRestoreType.SoftDeleted;
						break;
					case RemoteRestoreType.DisconnectedMailbox:
						this.restoreFlags |= MailboxRestoreType.SoftDeleted;
						break;
					case RemoteRestoreType.SoftDeletedRecipient:
						this.restoreFlags |= MailboxRestoreType.SoftDeletedRecipient;
						break;
					default:
						base.WriteError(new RecipientTaskException(Strings.ErrorParameterValueNotAllowed("RemoteRestoreType")), ErrorCategory.InvalidArgument, this.RemoteRestoreType);
						break;
					}
				}
				else
				{
					base.Flags = (RequestFlags.IntraOrg | RequestFlags.Pull);
					string fqdn;
					string serverExchangeLegacyDn;
					ADObjectId adobjectId;
					int num;
					MailboxDatabase mailboxDatabase = base.CheckDatabase<MailboxDatabase>(this.SourceDatabase, NewRequest<MailboxRestoreRequest>.DatabaseSide.Source, this.SourceDatabase, out fqdn, out serverExchangeLegacyDn, out adobjectId, out num);
					if (mailboxDatabase.Recovery)
					{
						this.restoreFlags |= MailboxRestoreType.Recovery;
					}
					this.sourceDatabase = mailboxDatabase.Id;
					this.SourceStoreMailbox.Flags |= 1UL;
					using (MapiSession mapiSession = new MapiAdministrationSession(serverExchangeLegacyDn, Fqdn.Parse(fqdn)))
					{
						using (MailboxStatistics mailboxStatistics = (MailboxStatistics)base.GetDataObject<MailboxStatistics>(this.SourceStoreMailbox, mapiSession, MapiTaskHelper.ConvertDatabaseADObjectToDatabaseId(mailboxDatabase), new LocalizedString?(Strings.ErrorStoreMailboxNotFound(this.SourceStoreMailbox.ToString(), this.SourceDatabase.ToString())), new LocalizedString?(Strings.ErrorStoreMailboxNotUnique(this.SourceStoreMailbox.ToString(), this.SourceDatabase.ToString()))))
						{
							MailboxState? disconnectReason = mailboxStatistics.DisconnectReason;
							if (mailboxStatistics.MailboxType == StoreMailboxType.PublicFolderPrimary || mailboxStatistics.MailboxType == StoreMailboxType.PublicFolderSecondary)
							{
								this.restoreFlags |= MailboxRestoreType.PublicFolderMailbox;
							}
							bool flag = false;
							if (disconnectReason == null && !mailboxDatabase.Recovery)
							{
								mapiSession.Administration.SyncMailboxWithDS(mailboxDatabase.Guid, mailboxStatistics.MailboxGuid);
								using (MailboxStatistics mailboxStatistics2 = (MailboxStatistics)base.GetDataObject<MailboxStatistics>(this.SourceStoreMailbox, mapiSession, MapiTaskHelper.ConvertDatabaseADObjectToDatabaseId(mailboxDatabase), new LocalizedString?(Strings.ErrorStoreMailboxNotFound(this.SourceStoreMailbox.ToString(), this.SourceDatabase.ToString())), new LocalizedString?(Strings.ErrorStoreMailboxNotUnique(this.SourceStoreMailbox.ToString(), this.SourceDatabase.ToString()))))
								{
									disconnectReason = mailboxStatistics2.DisconnectReason;
									if (disconnectReason == null)
									{
										if (this.targetUser.OrganizationId != null && this.targetUser.OrganizationId.OrganizationalUnit != null && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled)
										{
											IRecipientSession recipientSession = CommonUtils.CreateRecipientSession(mailboxStatistics.ExternalDirectoryOrganizationId, null, null);
											ADRecipient adrecipient = this.TargetIsArchive ? recipientSession.FindByExchangeGuidIncludingArchive(mailboxStatistics.MailboxGuid) : recipientSession.FindByExchangeGuid(mailboxStatistics.MailboxGuid);
											flag = (adrecipient != null && adrecipient.RecipientSoftDeletedStatus != 0);
										}
										if (!this.IsPublicFolderMailboxRestore && !flag)
										{
											base.WriteError(new CannotRestoreConnectedMailboxPermanentException(this.SourceStoreMailbox.ToString()), ErrorCategory.InvalidArgument, this.SourceStoreMailbox);
										}
									}
								}
							}
							if (flag)
							{
								this.restoreFlags |= MailboxRestoreType.SoftDeletedRecipient;
							}
							else if (disconnectReason != null)
							{
								if (disconnectReason != MailboxState.SoftDeleted)
								{
									this.restoreFlags |= MailboxRestoreType.Disabled;
								}
								else
								{
									this.restoreFlags |= MailboxRestoreType.SoftDeleted;
								}
							}
							this.sourceMailboxGuid = mailboxStatistics.MailboxGuid;
							this.sourceMailboxDN = mailboxStatistics.LegacyDN;
						}
					}
					if ((this.TargetIsArchive && this.sourceMailboxGuid == this.targetUser.ArchiveGuid && this.sourceDatabase.Equals(this.targetUser.ArchiveDatabase)) || (!this.TargetIsArchive && this.sourceMailboxGuid == this.targetUser.ExchangeGuid && this.sourceDatabase.Equals(this.targetUser.Database)))
					{
						base.WriteError(new CannotRestoreIntoSelfPermanentException(this.targetUser.ToString()), ErrorCategory.InvalidArgument, this.TargetMailbox);
					}
				}
				if (this.restoreFlags.HasFlag(MailboxRestoreType.PublicFolderMailbox))
				{
					if (this.targetUser.RecipientTypeDetails != RecipientTypeDetails.PublicFolderMailbox)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorCannotRestoreFromPublicToPrivateMailbox), ErrorCategory.InvalidArgument, this.SourceStoreMailbox);
					}
				}
				else if (this.targetUser.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorCannotRestoreFromPrivateToPublicMailbox), ErrorCategory.InvalidArgument, this.SourceStoreMailbox);
				}
				base.RescopeToOrgId(this.targetUser.OrganizationId);
				if (base.ParameterSetName.Equals("RemoteMailboxRestore"))
				{
					base.PerRecordReportEntries.Add(new ReportEntry(MrsStrings.ReportRequestAllowedMismatch(base.ExecutingUserIdentity)));
				}
				else
				{
					base.ValidateLegacyDNMatch(this.sourceMailboxDN, this.targetUser, this.TargetMailbox);
				}
				ADObjectId mdbId = null;
				ADObjectId mdbServerSite = null;
				this.LocateAndChooseMdb(null, this.TargetIsArchive ? this.targetUser.ArchiveDatabase : this.targetUser.Database, null, this.TargetMailbox, this.TargetMailbox, out mdbId, out mdbServerSite);
				base.MdbId = mdbId;
				base.MdbServerSite = mdbServerSite;
				base.RequestName = this.CheckRequestNameAvailability(base.RequestName, this.targetUser.Id, true, MRSRequestType.MailboxRestore, this.TargetMailbox, wildcardedSearch);
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
			dataObject.RequestType = MRSRequestType.MailboxRestore;
			if (dataObject.WorkloadType == RequestWorkloadType.None)
			{
				dataObject.WorkloadType = RequestWorkloadType.Local;
			}
			if (this.targetUser != null)
			{
				dataObject.TargetUserId = this.targetUser.Id;
				dataObject.TargetUser = this.targetUser;
			}
			if (!string.IsNullOrEmpty(this.SourceRootFolder))
			{
				dataObject.SourceRootFolder = this.SourceRootFolder;
			}
			dataObject.SourceIsArchive = false;
			dataObject.SourceExchangeGuid = this.sourceMailboxGuid;
			dataObject.SourceDatabase = this.sourceDatabase;
			dataObject.MailboxRestoreFlags = new MailboxRestoreType?(this.restoreFlags);
			if (!string.IsNullOrEmpty(this.TargetRootFolder))
			{
				dataObject.TargetRootFolder = this.TargetRootFolder;
			}
			if (this.TargetIsArchive)
			{
				dataObject.TargetIsArchive = true;
				dataObject.TargetExchangeGuid = this.targetUser.ArchiveGuid;
				dataObject.TargetDatabase = ADObjectIdResolutionHelper.ResolveDN(this.targetUser.ArchiveDatabase);
			}
			else
			{
				dataObject.TargetIsArchive = false;
				dataObject.TargetExchangeGuid = this.targetUser.ExchangeGuid;
				dataObject.TargetDatabase = ADObjectIdResolutionHelper.ResolveDN(this.targetUser.Database);
			}
			dataObject.TargetAlias = this.targetUser.Alias;
			dataObject.AllowedToFinishMove = true;
			if (this.IsPublicFolderMailboxRestore)
			{
				dataObject.SkipFolderRules = true;
			}
			if (base.ParameterSetName.Equals("RemoteMailboxRestore"))
			{
				dataObject.RemoteDatabaseGuid = new Guid?(this.RemoteDatabaseGuid);
				dataObject.RemoteHostName = this.RemoteHostName;
				dataObject.RemoteCredential = RequestTaskHelper.GetNetworkCredential(this.RemoteCredential, new AuthenticationMethod?(AuthenticationMethod.WindowsIntegrated));
			}
		}

		protected override MailboxRestoreRequest ConvertToPresentationObject(TransactionalRequestJob dataObject)
		{
			if (dataObject.IndexEntries != null && dataObject.IndexEntries.Count >= 1)
			{
				return new MailboxRestoreRequest(dataObject.IndexEntries[0]);
			}
			base.WriteError(new RequestIndexEntriesAbsentPermanentException(base.RequestName), ErrorCategory.InvalidArgument, this.TargetMailbox);
			return null;
		}

		protected override bool IsSupportedDatabaseVersion(int serverVersion, NewRequest<MailboxRestoreRequest>.DatabaseSide databaseSide)
		{
			if (databaseSide == NewRequest<MailboxRestoreRequest>.DatabaseSide.Source)
			{
				return serverVersion >= Server.E14MinVersion;
			}
			return base.IsSupportedDatabaseVersion(serverVersion, databaseSide);
		}

		private void CheckForInvalidPublicFolderRestoreParameters()
		{
			if (this.targetUser.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox && base.Fields.IsModified("TargetRootFolder"))
			{
				base.WriteError(new TaskArgumentException(Strings.ErrorInvalidParameterForPublicFolderRestore("TargetRootFolder")), ExchangeErrorCategory.Client, null);
			}
		}

		public const string DefaultMailboxRestoreName = "MailboxRestore";

		public const string TaskNoun = "MailboxRestoreRequest";

		public const string ParameterSetLocalMailboxRestore = "MigrationLocalMailboxRestore";

		public const string ParameterSetRemoteMailboxRestore = "RemoteMailboxRestore";

		private string sourceMailboxDN;

		private Guid sourceMailboxGuid;

		private ADObjectId sourceDatabase;

		private MailboxRestoreType restoreFlags;

		private ADUser targetUser;
	}
}
