using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("New", "MailboxRepairRequest", SupportsShouldProcess = true, DefaultParameterSetName = "Mailbox")]
	public sealed class NewMailboxRepairRequest : NewTaskBase<StoreIntegrityCheckJob>
	{
		internal override IConfigurationSession ConfigurationSession
		{
			get
			{
				return this.readOnlyConfigSession;
			}
		}

		internal IRecipientSession RecipientSession
		{
			get
			{
				return this.readOnlyRecipientSession;
			}
		}

		[Parameter(Mandatory = true, Position = 0, ParameterSetName = "Database", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public DatabaseIdParameter Database
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["Database"];
			}
			set
			{
				base.Fields["Database"] = value;
			}
		}

		[Parameter(Mandatory = false, Position = 1, ParameterSetName = "Database", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public StoreMailboxIdParameter StoreMailbox
		{
			get
			{
				return (StoreMailboxIdParameter)base.Fields["StoreMailbox"];
			}
			set
			{
				base.Fields["StoreMailbox"] = value;
			}
		}

		[Parameter(Mandatory = true, Position = 0, ParameterSetName = "Mailbox", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public MailboxIdParameter Mailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Mailbox")]
		public SwitchParameter Archive
		{
			get
			{
				return (SwitchParameter)(base.Fields["Archive"] ?? false);
			}
			set
			{
				base.Fields["Archive"] = value;
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
		public SwitchParameter DetectOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["DetectOnly"] ?? false);
			}
			set
			{
				base.Fields["DetectOnly"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? false);
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public MailboxCorruptionType[] CorruptionType
		{
			get
			{
				return (MailboxCorruptionType[])base.Fields["CorruptionType"];
			}
			set
			{
				base.Fields["CorruptionType"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewMailboxRepairRequestMailbox(this.databaseObject.Name, this.mailboxGuid.ToString());
			}
		}

		public NewMailboxRepairRequest()
		{
			base.Fields["DetectOnly"] = new SwitchParameter(false);
			base.Fields["Force"] = new SwitchParameter(false);
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || (exception is ServerUnavailableException || exception is RpcException || exception is InvalidIntegrityCheckJobIdentity);
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			this.DataObject = null;
			this.databaseObject = null;
			this.mailboxGuid = Guid.Empty;
			base.InternalStateReset();
			TaskLogger.LogExit();
		}

		protected override IConfigDataProvider CreateSession()
		{
			TaskLogger.LogEnter();
			ADSessionSettings adsessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			this.sessionSettings = ADSessionSettings.RescopeToSubtree(adsessionSettings);
			this.readOnlyRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, this.sessionSettings, 270, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\OnlineIsInteg\\NewMailboxRepairRequest.cs");
			if (this.DomainController == null)
			{
				this.readOnlyRecipientSession.UseGlobalCatalog = true;
			}
			this.readOnlyConfigSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, this.sessionSettings, 281, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\OnlineIsInteg\\NewMailboxRepairRequest.cs");
			TaskLogger.LogExit();
			return this.readOnlyConfigSession;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			this.unlockMoveTarget = false;
			foreach (MailboxCorruptionType mailboxCorruptionType in this.CorruptionType)
			{
				MailboxCorruptionType mailboxCorruptionType2 = mailboxCorruptionType;
				if (mailboxCorruptionType2 == MailboxCorruptionType.LockedMoveTarget)
				{
					this.unlockMoveTarget = true;
				}
			}
			if (this.unlockMoveTarget)
			{
				ADUser aduser = ((ADRecipient)base.GetDataObject<ADRecipient>(this.Mailbox, this.RecipientSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(this.Mailbox.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(this.Mailbox.ToString())))) as ADUser;
				if (this.CorruptionType.Length != 1)
				{
					base.WriteError(new CorruptionTypeParameterIncompatibleException(MailboxCorruptionType.LockedMoveTarget.ToString()), ErrorCategory.InvalidArgument, this.CorruptionType);
				}
				if (UnlockMoveTargetUtil.IsValidLockedStatus(aduser.MailboxMoveStatus))
				{
					base.WriteError(new UnableToUnlockMailboxDueToOutstandingMoveRequestException(this.Mailbox.ToString(), aduser.MailboxMoveStatus.ToString()), ErrorCategory.InvalidArgument, this.Mailbox);
				}
				ActiveManager activeManagerInstance = ActiveManager.GetActiveManagerInstance();
				DatabaseLocationInfo serverForDatabase = activeManagerInstance.GetServerForDatabase(this.databaseObject.Guid);
				if (!UnlockMoveTargetUtil.IsMailboxLocked(serverForDatabase.ServerFqdn, this.databaseObject.Guid, aduser.ExchangeGuid))
				{
					base.WriteError(new MailboxIsNotLockedException(this.Mailbox.ToString()), ErrorCategory.InvalidArgument, this.Mailbox);
				}
			}
			if (!this.databaseObject.IsMailboxDatabase)
			{
				base.WriteError(new OnlineIsIntegException(this.databaseObject.Name, Strings.NotMailboxDatabase, null), ErrorCategory.InvalidArgument, null);
			}
			Guid guid;
			if (this.StoreMailbox != null && !Guid.TryParse(this.StoreMailbox.RawIdentity, out guid))
			{
				throw new ArgumentException("StoreMailbox");
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			DatabaseIdParameter databaseIdParameter = null;
			Guid empty = Guid.Empty;
			if (base.ParameterSetName == "Database")
			{
				databaseIdParameter = this.Database;
				if (this.StoreMailbox != null && !Guid.TryParse(this.StoreMailbox.RawIdentity, out this.mailboxGuid))
				{
					base.WriteError(new OnlineIsIntegException(this.Database.ToString(), Strings.InvalidStoreMailboxIdentity, null), ErrorCategory.InvalidArgument, this.StoreMailbox);
				}
			}
			else if (base.ParameterSetName == "Mailbox")
			{
				ADUser aduser = ((ADRecipient)base.GetDataObject<ADRecipient>(this.Mailbox, this.RecipientSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(this.Mailbox.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(this.Mailbox.ToString())))) as ADUser;
				if (!this.Archive)
				{
					databaseIdParameter = new DatabaseIdParameter(aduser.Database);
					this.mailboxGuid = aduser.ExchangeGuid;
				}
				else if (aduser.ArchiveDomain == null && aduser.ArchiveGuid != Guid.Empty)
				{
					databaseIdParameter = new DatabaseIdParameter(aduser.ArchiveDatabase ?? aduser.Database);
					this.mailboxGuid = aduser.ArchiveGuid;
				}
				else if (aduser.ArchiveDomain != null)
				{
					base.WriteError(new MdbAdminTaskException(Strings.ErrorRemoteArchiveNoStats(aduser.ToString())), (ErrorCategory)1003, this.Mailbox);
				}
				else
				{
					base.WriteError(new MdbAdminTaskException(Strings.ErrorArchiveNotEnabled(aduser.ToString())), ErrorCategory.InvalidArgument, this.Mailbox);
				}
				this.organizationId = aduser.OrganizationId;
			}
			this.databaseObject = (Database)base.GetDataObject<Database>(databaseIdParameter, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(databaseIdParameter.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(databaseIdParameter.ToString())));
			return new StoreIntegrityCheckJob();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.unlockMoveTarget)
			{
				ActiveManager activeManagerInstance = ActiveManager.GetActiveManagerInstance();
				DatabaseLocationInfo serverForDatabase = activeManagerInstance.GetServerForDatabase(this.databaseObject.Guid);
				string serverFqdn = serverForDatabase.ServerFqdn;
				if (!this.DetectOnly)
				{
					UnlockMoveTargetUtil.UnlockMoveTarget(serverFqdn, this.databaseObject.Guid, this.mailboxGuid, this.organizationId);
					this.WriteResult(this.DataObject);
				}
			}
			else
			{
				StoreIntegrityCheckRequestFlags requestFlags = this.GetRequestFlags();
				StoreIntegrityCheckJob storeIntegrityCheckJob = StoreIntegrityCheckAdminRpc.CreateStoreIntegrityCheckJob(this.databaseObject, this.mailboxGuid, requestFlags, this.CorruptionType, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				if (storeIntegrityCheckJob != null)
				{
					this.WriteResult(storeIntegrityCheckJob);
				}
			}
			TaskLogger.LogExit();
		}

		private StoreIntegrityCheckRequestFlags GetRequestFlags()
		{
			StoreIntegrityCheckRequestFlags storeIntegrityCheckRequestFlags = StoreIntegrityCheckRequestFlags.None;
			if (this.DetectOnly)
			{
				storeIntegrityCheckRequestFlags |= StoreIntegrityCheckRequestFlags.DetectOnly;
			}
			if (this.Force)
			{
				storeIntegrityCheckRequestFlags |= StoreIntegrityCheckRequestFlags.Force;
			}
			return storeIntegrityCheckRequestFlags;
		}

		private const string ParameterDetectOnly = "DetectOnly";

		private const string ParameterForce = "Force";

		private IRecipientSession readOnlyRecipientSession;

		private IConfigurationSession readOnlyConfigSession;

		private Database databaseObject;

		private Guid mailboxGuid;

		private OrganizationId organizationId;

		private ADSessionSettings sessionSettings;

		private bool unlockMoveTarget;
	}
}
