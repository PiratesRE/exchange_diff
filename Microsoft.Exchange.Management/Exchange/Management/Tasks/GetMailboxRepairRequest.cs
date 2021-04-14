using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "MailboxRepairRequest", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxRepairRequest : GetObjectWithIdentityTaskBase<StoreIntegrityCheckJobIdParameter, StoreIntegrityCheckJob>
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter Detailed
		{
			get
			{
				return (SwitchParameter)(base.Fields["Detailed"] ?? false);
			}
			set
			{
				base.Fields["Detailed"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override StoreIntegrityCheckJobIdParameter Identity
		{
			get
			{
				return (StoreIntegrityCheckJobIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
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

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || (exception is ServerUnavailableException || exception is RpcException || exception is InvalidIntegrityCheckJobIdentity);
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
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
			this.readOnlyRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, this.sessionSettings, 235, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\OnlineIsInteg\\GetMailboxRepairRequest.cs");
			if (this.DomainController == null)
			{
				this.readOnlyRecipientSession.UseGlobalCatalog = true;
			}
			this.readOnlyConfigSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, this.sessionSettings, 246, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\OnlineIsInteg\\GetMailboxRepairRequest.cs");
			TaskLogger.LogExit();
			return this.readOnlyConfigSession;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			DatabaseIdParameter databaseIdParameter = null;
			Guid empty = Guid.Empty;
			if (base.ParameterSetName == "Identity")
			{
				if (this.Identity != null)
				{
					StoreIntegrityCheckJobIdentity storeIntegrityCheckJobIdentity = new StoreIntegrityCheckJobIdentity(this.Identity.RawIdentity);
					databaseIdParameter = new DatabaseIdParameter(new ADObjectId(storeIntegrityCheckJobIdentity.DatabaseGuid));
					this.requestGuid = storeIntegrityCheckJobIdentity.RequestGuid;
					this.jobGuid = storeIntegrityCheckJobIdentity.JobGuid;
				}
				else
				{
					base.WriteError(new ArgumentNullException("identity"), ErrorCategory.InvalidArgument, null);
				}
			}
			else if (base.ParameterSetName == "Database")
			{
				databaseIdParameter = this.Database;
				if (this.StoreMailbox != null)
				{
					Guid.TryParse(this.StoreMailbox.RawIdentity, out this.mailboxGuid);
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
			}
			this.databaseObject = (Database)base.GetDataObject<Database>(databaseIdParameter, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(databaseIdParameter.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(databaseIdParameter.ToString())));
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			IntegrityCheckQueryFlags flags = IntegrityCheckQueryFlags.None;
			Guid guid = this.requestGuid;
			if (this.jobGuid != Guid.Empty)
			{
				flags = IntegrityCheckQueryFlags.QueryJob;
				guid = this.jobGuid;
			}
			bool details = true;
			if (base.ParameterSetName == "Identity" && this.Detailed == false)
			{
				details = false;
			}
			List<StoreIntegrityCheckJob> storeIntegrityCheckJob = StoreIntegrityCheckAdminRpc.GetStoreIntegrityCheckJob(this.databaseObject, this.mailboxGuid, guid, flags, details, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			if (storeIntegrityCheckJob != null)
			{
				this.WriteResult<StoreIntegrityCheckJob>(storeIntegrityCheckJob);
			}
			TaskLogger.LogExit();
		}

		private IRecipientSession readOnlyRecipientSession;

		private IConfigurationSession readOnlyConfigSession;

		private Database databaseObject;

		private Guid mailboxGuid;

		private Guid requestGuid;

		private Guid jobGuid;

		private ADSessionSettings sessionSettings;
	}
}
