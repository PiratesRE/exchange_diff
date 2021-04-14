using System;
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
	public abstract class GetRequest<TIdentity, TDataObject> : GetObjectWithIdentityTaskBase<TIdentity, TDataObject> where TIdentity : MRSRequestIdParameter where TDataObject : RequestBase, new()
	{
		public GetRequest()
		{
			this.userId = null;
			this.queueId = null;
			this.GCSession = null;
			this.RecipSession = null;
			this.ConfigSession = null;
			this.CurrentOrgConfigSession = null;
			this.indexProvider = null;
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		public RequestStatus Status
		{
			get
			{
				return (RequestStatus)(base.Fields["Status"] ?? RequestStatus.None);
			}
			set
			{
				base.Fields["Status"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		public bool Suspend
		{
			get
			{
				return (bool)(base.Fields["Suspend"] ?? false);
			}
			set
			{
				base.Fields["Suspend"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		public bool HighPriority
		{
			get
			{
				return (bool)(base.Fields["HighPriority"] ?? false);
			}
			set
			{
				base.Fields["HighPriority"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		public DatabaseIdParameter RequestQueue
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["RequestQueue"];
			}
			set
			{
				base.Fields["RequestQueue"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		[ValidateNotNull]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNull]
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
		public Unlimited<uint> ResultSize
		{
			get
			{
				return base.InternalResultSize;
			}
			set
			{
				base.InternalResultSize = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[Parameter(Mandatory = false, ParameterSetName = "Filtering", ValueFromPipeline = true)]
		[ValidateNotNull]
		public AccountPartitionIdParameter AccountPartition
		{
			get
			{
				return (AccountPartitionIdParameter)base.Fields["AccountPartition"];
			}
			set
			{
				base.Fields["AccountPartition"] = value;
			}
		}

		internal MailboxOrMailUserIdParameter InternalMailbox
		{
			get
			{
				return (MailboxOrMailUserIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		internal IRecipientSession GCSession { get; set; }

		internal IRecipientSession RecipSession { get; set; }

		internal IConfigurationSession CurrentOrgConfigSession { get; set; }

		internal ITopologyConfigurationSession ConfigSession { get; set; }

		protected virtual RequestIndexId DefaultRequestIndexId
		{
			get
			{
				return new RequestIndexId(RequestIndexLocation.AD);
			}
		}

		protected abstract MRSRequestType RequestType { get; }

		protected override ObjectId RootId
		{
			get
			{
				if (this.AccountPartition == null)
				{
					return ADHandler.GetRootId(this.CurrentOrgConfigSession, this.RequestType);
				}
				return null;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return this.AccountPartition != null;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				return this.InternalFilterBuilder();
			}
		}

		protected virtual RequestIndexEntryQueryFilter InternalFilterBuilder()
		{
			RequestIndexEntryQueryFilter requestIndexEntryQueryFilter = new RequestIndexEntryQueryFilter();
			if (this.IsFieldSet("BatchName"))
			{
				requestIndexEntryQueryFilter.BatchName = (this.BatchName ?? string.Empty);
			}
			if (this.IsFieldSet("Name"))
			{
				requestIndexEntryQueryFilter.RequestName = (this.Name ?? string.Empty);
			}
			if (this.IsFieldSet("Status"))
			{
				requestIndexEntryQueryFilter.Status = this.Status;
			}
			if (this.IsFieldSet("Suspend"))
			{
				if (this.Suspend)
				{
					requestIndexEntryQueryFilter.Flags |= RequestFlags.Suspend;
				}
				else
				{
					requestIndexEntryQueryFilter.NotFlags |= RequestFlags.Suspend;
				}
			}
			if (this.IsFieldSet("HighPriority"))
			{
				if (this.HighPriority)
				{
					requestIndexEntryQueryFilter.Flags |= RequestFlags.HighPriority;
				}
				else
				{
					requestIndexEntryQueryFilter.NotFlags |= RequestFlags.HighPriority;
				}
			}
			if (this.queueId != null)
			{
				requestIndexEntryQueryFilter.RequestQueueId = this.queueId;
			}
			if (this.userId != null)
			{
				requestIndexEntryQueryFilter.MailboxId = this.userId;
				requestIndexEntryQueryFilter.LooseMailboxSearch = true;
			}
			return requestIndexEntryQueryFilter;
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (this.Organization != null)
			{
				IConfigurationSession session = RequestTaskHelper.CreateOrganizationFindingSession(base.CurrentOrganizationId, base.ExecutingUserOrganizationId);
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, session, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
				base.CurrentOrganizationId = adorganizationalUnit.OrganizationId;
			}
		}

		protected override void InternalStateReset()
		{
			this.userId = null;
			this.queueId = null;
			base.InternalStateReset();
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADSessionSettings adsessionSettings;
			if (this.AccountPartition != null)
			{
				PartitionId partitionId = RecipientTaskHelper.ResolvePartitionId(this.AccountPartition, new Task.TaskErrorLoggingDelegate(base.WriteError));
				adsessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
			}
			else
			{
				adsessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			}
			if (MapiTaskHelper.IsDatacenter || MapiTaskHelper.IsDatacenterDedicated)
			{
				adsessionSettings.IncludeSoftDeletedObjects = true;
				adsessionSettings.IncludeInactiveMailbox = true;
			}
			this.CurrentOrgConfigSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, null, adsessionSettings, 479, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\RequestBase\\GetRequest.cs");
			if (this.AccountPartition == null)
			{
				adsessionSettings = ADSessionSettings.RescopeToSubtree(adsessionSettings);
			}
			this.GCSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, adsessionSettings, 493, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\RequestBase\\GetRequest.cs");
			this.RecipSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, adsessionSettings, 500, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\RequestBase\\GetRequest.cs");
			this.ConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromRootOrgScopeSet(), 506, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\RequestBase\\GetRequest.cs");
			if (this.indexProvider != null)
			{
				this.indexProvider = null;
			}
			this.indexProvider = new RequestIndexEntryProvider(this.GCSession, this.CurrentOrgConfigSession);
			return this.indexProvider;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.InternalMailbox != null)
			{
				ADUser aduser = RequestTaskHelper.ResolveADUser(this.RecipSession, this.GCSession, base.ServerSettings, this.InternalMailbox, base.OptionalIdentityData, this.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), false);
				this.userId = aduser.Id;
			}
			if (this.RequestQueue != null)
			{
				MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.RequestQueue, this.ConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.RequestQueue.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.RequestQueue.ToString())));
				this.queueId = mailboxDatabase.Id;
			}
			if (this.Organization != null && this.AccountPartition != null)
			{
				base.WriteError(new TaskException(Strings.ErrorIncompatibleParameters("Organization", "AccountPartition")), ErrorCategory.InvalidArgument, this.Identity);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				if (this.Identity != null)
				{
					TIdentity identity = this.Identity;
					if (identity.OrganizationId != null)
					{
						IDirectorySession recipSession = this.RecipSession;
						TIdentity identity2 = this.Identity;
						if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(recipSession, identity2.OrganizationId))
						{
							IDirectorySession recipSession2 = this.RecipSession;
							TIdentity identity3 = this.Identity;
							this.RecipSession = (IRecipientSession)TaskHelper.UnderscopeSessionToOrganization(recipSession2, identity3.OrganizationId, true);
						}
						IDirectorySession currentOrgConfigSession = this.CurrentOrgConfigSession;
						TIdentity identity4 = this.Identity;
						if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(currentOrgConfigSession, identity4.OrganizationId))
						{
							IDirectorySession currentOrgConfigSession2 = this.CurrentOrgConfigSession;
							TIdentity identity5 = this.Identity;
							this.CurrentOrgConfigSession = (ITenantConfigurationSession)TaskHelper.UnderscopeSessionToOrganization(currentOrgConfigSession2, identity5.OrganizationId, true);
							this.indexProvider.ConfigSession = this.CurrentOrgConfigSession;
						}
					}
					TIdentity identity6 = this.Identity;
					if (!string.IsNullOrEmpty(identity6.MailboxName))
					{
						IRecipientSession recipSession3 = this.RecipSession;
						IRecipientSession gcsession = this.GCSession;
						ADServerSettings serverSettings = base.ServerSettings;
						TIdentity identity7 = this.Identity;
						ADUser aduser = RequestTaskHelper.ResolveADUser(recipSession3, gcsession, serverSettings, new MailboxOrMailUserIdParameter(identity7.MailboxName), base.OptionalIdentityData, this.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), false);
						if (aduser != null)
						{
							TIdentity identity8 = this.Identity;
							identity8.MailboxId = aduser.Id;
							if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(this.RecipSession, aduser))
							{
								this.RecipSession = (IRecipientSession)TaskHelper.UnderscopeSessionToOrganization(this.RecipSession, aduser.OrganizationId, true);
							}
							if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(this.CurrentOrgConfigSession, aduser))
							{
								this.CurrentOrgConfigSession = (ITenantConfigurationSession)TaskHelper.UnderscopeSessionToOrganization(this.CurrentOrgConfigSession, aduser.OrganizationId, true);
								this.indexProvider.ConfigSession = this.CurrentOrgConfigSession;
							}
						}
					}
					TIdentity identity9 = this.Identity;
					if (!string.IsNullOrEmpty(identity9.OrganizationName))
					{
						IConfigurationSession configurationSession = RequestTaskHelper.CreateOrganizationFindingSession(base.CurrentOrganizationId, base.ExecutingUserOrganizationId);
						TIdentity identity10 = this.Identity;
						IIdentityParameter id = new OrganizationIdParameter(identity10.OrganizationName);
						IConfigDataProvider session = configurationSession;
						ObjectId rootID = null;
						TIdentity identity11 = this.Identity;
						LocalizedString? notFoundError = new LocalizedString?(Strings.ErrorOrganizationNotFound(identity11.OrganizationName));
						TIdentity identity12 = this.Identity;
						ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(id, session, rootID, notFoundError, new LocalizedString?(Strings.ErrorOrganizationNotUnique(identity12.OrganizationName)));
						if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(this.RecipSession, adorganizationalUnit))
						{
							this.RecipSession = (IRecipientSession)TaskHelper.UnderscopeSessionToOrganization(this.RecipSession, adorganizationalUnit.OrganizationId, true);
						}
						if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(this.CurrentOrgConfigSession, adorganizationalUnit))
						{
							this.CurrentOrgConfigSession = (ITenantConfigurationSession)TaskHelper.UnderscopeSessionToOrganization(this.CurrentOrgConfigSession, adorganizationalUnit.OrganizationId, true);
							this.indexProvider.ConfigSession = this.CurrentOrgConfigSession;
						}
					}
					TIdentity identity13 = this.Identity;
					identity13.SetDefaultIndex(this.DefaultRequestIndexId);
				}
				base.InternalProcessRecord();
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return RequestTaskHelper.IsKnownExceptionHandler(exception, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose)) || base.IsKnownException(exception);
		}

		protected bool IsFieldSet(string fieldName)
		{
			return base.Fields.IsChanged(fieldName) || base.Fields.IsModified(fieldName);
		}

		public const string ParameterStatus = "Status";

		public const string ParameterBatchName = "BatchName";

		public const string ParameterName = "Name";

		public const string ParameterMailbox = "Mailbox";

		public const string ParameterRequestQueue = "RequestQueue";

		public const string ParameterSuspend = "Suspend";

		public const string ParameterHighPriority = "HighPriority";

		public const string ParameterOrganization = "Organization";

		public const string ParameterAccountPartition = "AccountPartition";

		public const string FiltersSet = "Filtering";

		private ADObjectId userId;

		private ADObjectId queueId;

		private RequestIndexEntryProvider indexProvider;
	}
}
