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
	public abstract class GetRequestStatistics<TIdentity, TDataObject> : GetTaskBase<TDataObject> where TIdentity : MRSRequestIdParameter where TDataObject : RequestStatisticsBase, new()
	{
		protected GetRequestStatistics()
		{
			this.fromMdb = null;
			this.gcSession = null;
			this.recipSession = null;
			this.configSession = null;
			this.currentOrgConfigSession = null;
			this.rjProvider = null;
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNull]
		public TIdentity Identity
		{
			get
			{
				return (TIdentity)((object)base.Fields["Identity"]);
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeReport
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeReport"] ?? false);
			}
			set
			{
				base.Fields["IncludeReport"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MigrationRequestQueue")]
		[ValidateNotNull]
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

		[Parameter(Mandatory = false, ParameterSetName = "MigrationRequestQueue")]
		public Guid RequestGuid
		{
			get
			{
				return (Guid)(base.Fields["RequestGuid"] ?? Guid.Empty);
			}
			set
			{
				base.Fields["RequestGuid"] = value;
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
		public SwitchParameter Diagnostic
		{
			get
			{
				return (SwitchParameter)(base.Fields["Diagnostic"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Diagnostic"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateLength(1, 1048576)]
		[ValidateNotNull]
		public string DiagnosticArgument
		{
			get
			{
				return (string)base.Fields["DiagnosticArgument"];
			}
			set
			{
				base.Fields["DiagnosticArgument"] = value;
			}
		}

		protected virtual RequestIndexId DefaultRequestIndexId
		{
			get
			{
				return new RequestIndexId(RequestIndexLocation.AD);
			}
		}

		protected MRSRequestType RequestType
		{
			get
			{
				return GetRequestStatistics<TIdentity, TDataObject>.requestType;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return ADHandler.GetRootId(this.currentOrgConfigSession, this.RequestType);
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				if (base.ParameterSetName.Equals("MigrationRequestQueue"))
				{
					return new RequestJobQueryFilter(this.RequestGuid, this.fromMdb.ObjectGuid, this.RequestType);
				}
				return null;
			}
		}

		internal virtual void CheckIndexEntry(IRequestIndexEntry index)
		{
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADObjectId rootOrgContainerId = ADSystemConfigurationSession.GetRootOrgContainerId(this.DomainController, null);
			ADSessionSettings adsessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, rootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			this.currentOrgConfigSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, null, adsessionSettings, 313, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\RequestBase\\GetRequestStatistics.cs");
			adsessionSettings = ADSessionSettings.RescopeToSubtree(adsessionSettings);
			if (MapiTaskHelper.IsDatacenter || MapiTaskHelper.IsDatacenterDedicated)
			{
				adsessionSettings.IncludeSoftDeletedObjects = true;
				adsessionSettings.IncludeInactiveMailbox = true;
			}
			this.gcSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, adsessionSettings, 330, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\RequestBase\\GetRequestStatistics.cs");
			this.recipSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, adsessionSettings, 337, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\RequestBase\\GetRequestStatistics.cs");
			this.configSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromRootOrgScopeSet(), 343, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\RequestBase\\GetRequestStatistics.cs");
			if (this.rjProvider != null)
			{
				this.rjProvider.Dispose();
				this.rjProvider = null;
			}
			if (base.ParameterSetName.Equals("MigrationRequestQueue"))
			{
				MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.RequestQueue, this.configSession, null, new LocalizedString?(Strings.ErrorMailboxDatabaseNotFound(this.RequestQueue.ToString())), new LocalizedString?(Strings.ErrorMailboxDatabaseNotUnique(this.RequestQueue.ToString())));
				this.rjProvider = new RequestJobProvider(mailboxDatabase.Guid);
			}
			else
			{
				this.rjProvider = new RequestJobProvider(this.gcSession, this.currentOrgConfigSession);
			}
			this.rjProvider.LoadReport = this.IncludeReport;
			return this.rjProvider;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.rjProvider != null)
			{
				this.rjProvider.Dispose();
				this.rjProvider = null;
			}
			base.Dispose(disposing);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				if (base.ParameterSetName.Equals("Identity"))
				{
					TIdentity identity = this.Identity;
					if (identity.OrganizationId != null)
					{
						IDirectorySession dataSession = this.recipSession;
						TIdentity identity2 = this.Identity;
						if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(dataSession, identity2.OrganizationId))
						{
							IDirectorySession session = this.recipSession;
							TIdentity identity3 = this.Identity;
							this.recipSession = (IRecipientSession)TaskHelper.UnderscopeSessionToOrganization(session, identity3.OrganizationId, true);
						}
						IDirectorySession dataSession2 = this.currentOrgConfigSession;
						TIdentity identity4 = this.Identity;
						if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(dataSession2, identity4.OrganizationId))
						{
							IDirectorySession session2 = this.currentOrgConfigSession;
							TIdentity identity5 = this.Identity;
							this.currentOrgConfigSession = (ITenantConfigurationSession)TaskHelper.UnderscopeSessionToOrganization(session2, identity5.OrganizationId, true);
							this.rjProvider.IndexProvider.ConfigSession = this.currentOrgConfigSession;
						}
					}
					ADUser aduser = null;
					TIdentity identity6 = this.Identity;
					if (!string.IsNullOrEmpty(identity6.MailboxName))
					{
						IRecipientSession dataSession3 = this.recipSession;
						IRecipientSession globalCatalogSession = this.gcSession;
						ADServerSettings serverSettings = base.ServerSettings;
						TIdentity identity7 = this.Identity;
						aduser = RequestTaskHelper.ResolveADUser(dataSession3, globalCatalogSession, serverSettings, new MailboxOrMailUserIdParameter(identity7.MailboxName), base.OptionalIdentityData, this.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), false);
						if (aduser != null)
						{
							TIdentity identity8 = this.Identity;
							identity8.MailboxId = aduser.Id;
							if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(this.recipSession, aduser))
							{
								this.recipSession = (IRecipientSession)TaskHelper.UnderscopeSessionToOrganization(this.recipSession, aduser.OrganizationId, true);
							}
							if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(this.currentOrgConfigSession, aduser))
							{
								this.currentOrgConfigSession = (ITenantConfigurationSession)TaskHelper.UnderscopeSessionToOrganization(this.currentOrgConfigSession, aduser.OrganizationId, true);
								this.rjProvider.IndexProvider.ConfigSession = this.currentOrgConfigSession;
							}
						}
					}
					TIdentity identity9 = this.Identity;
					if (!string.IsNullOrEmpty(identity9.OrganizationName))
					{
						IConfigurationSession configurationSession = RequestTaskHelper.CreateOrganizationFindingSession(base.CurrentOrganizationId, base.ExecutingUserOrganizationId);
						TIdentity identity10 = this.Identity;
						IIdentityParameter id = new OrganizationIdParameter(identity10.OrganizationName);
						IConfigDataProvider session3 = configurationSession;
						ObjectId rootID = null;
						TIdentity identity11 = this.Identity;
						LocalizedString? notFoundError = new LocalizedString?(Strings.ErrorOrganizationNotFound(identity11.OrganizationName));
						TIdentity identity12 = this.Identity;
						ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(id, session3, rootID, notFoundError, new LocalizedString?(Strings.ErrorOrganizationNotUnique(identity12.OrganizationName)));
						if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(this.recipSession, adorganizationalUnit))
						{
							this.recipSession = (IRecipientSession)TaskHelper.UnderscopeSessionToOrganization(this.recipSession, adorganizationalUnit.OrganizationId, true);
						}
						if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(this.currentOrgConfigSession, adorganizationalUnit))
						{
							this.currentOrgConfigSession = (ITenantConfigurationSession)TaskHelper.UnderscopeSessionToOrganization(this.currentOrgConfigSession, adorganizationalUnit.OrganizationId, true);
							this.rjProvider.IndexProvider.ConfigSession = this.currentOrgConfigSession;
						}
					}
					TIdentity identity13 = this.Identity;
					identity13.SetDefaultIndex(this.DefaultRequestIndexId);
					IRequestIndexEntry entry = this.GetEntry();
					RequestJobObjectId requestJobId = entry.GetRequestJobId();
					if (entry.TargetUserId != null)
					{
						if (aduser != null && aduser.Id.Equals(entry.TargetUserId))
						{
							requestJobId.TargetUser = aduser;
						}
						else
						{
							requestJobId.TargetUser = RequestTaskHelper.ResolveADUser(this.recipSession, this.gcSession, base.ServerSettings, new MailboxOrMailUserIdParameter(entry.TargetUserId), base.OptionalIdentityData, this.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), false);
						}
					}
					if (entry.SourceUserId != null)
					{
						if (aduser != null && aduser.Id.Equals(entry.SourceUserId))
						{
							requestJobId.SourceUser = aduser;
						}
						else
						{
							requestJobId.SourceUser = RequestTaskHelper.ResolveADUser(this.recipSession, this.gcSession, base.ServerSettings, new MailboxOrMailUserIdParameter(entry.SourceUserId), base.OptionalIdentityData, this.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), false);
						}
					}
					this.CheckIndexEntry(entry);
					TDataObject tdataObject = (TDataObject)((object)this.rjProvider.Read<TDataObject>(requestJobId));
					if (tdataObject == null || tdataObject.Status == RequestStatus.None)
					{
						TIdentity identity14 = this.Identity;
						base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorCouldNotFindRequest(identity14.ToString())), ErrorCategory.InvalidArgument, this.Identity);
					}
					else if (tdataObject.RequestType != this.RequestType)
					{
						base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorNotEnoughInformationToFindRequest), ErrorCategory.InvalidArgument, this.Identity);
					}
					else
					{
						this.WriteResult(tdataObject);
					}
				}
				else if (base.ParameterSetName.Equals("MigrationRequestQueue"))
				{
					if (this.RequestQueue != null)
					{
						MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.RequestQueue, this.configSession, null, new LocalizedString?(Strings.ErrorMailboxDatabaseNotFound(this.RequestQueue.ToString())), new LocalizedString?(Strings.ErrorMailboxDatabaseNotUnique(this.RequestQueue.ToString())));
						this.fromMdb = mailboxDatabase.Id;
					}
					this.rjProvider.AllowInvalid = true;
					base.InternalProcessRecord();
				}
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

		protected override void TranslateException(ref Exception e, out ErrorCategory category)
		{
			LocalizedException ex = RequestTaskHelper.TranslateExceptionHandler(e);
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

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject
			});
			TDataObject tdataObject = (TDataObject)((object)dataObject);
			try
			{
				RequestTaskHelper.GetUpdatedMRSRequestInfo(tdataObject, this.Diagnostic, this.DiagnosticArgument);
				if (tdataObject.Status == RequestStatus.Queued)
				{
					tdataObject.PositionInQueue = this.rjProvider.ComputePositionInQueue(tdataObject.RequestGuid);
				}
				base.WriteResult(tdataObject);
				if (tdataObject.ValidationResult != RequestJobBase.ValidationResultEnum.Valid)
				{
					this.WriteWarning(Strings.ErrorInvalidRequest(tdataObject.Identity.ToString(), tdataObject.ValidationMessage));
				}
				if (tdataObject.PoisonCount > 5)
				{
					this.WriteWarning(Strings.WarningJobIsPoisoned(tdataObject.Identity.ToString(), tdataObject.PoisonCount));
				}
				if (base.ParameterSetName.Equals("MigrationRequestQueue"))
				{
					base.WriteVerbose(Strings.RawRequestJobDump(CommonUtils.ConfigurableObjectToString(tdataObject)));
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected bool IsFieldSet(string fieldName)
		{
			return base.Fields.IsChanged(fieldName) || base.Fields.IsModified(fieldName);
		}

		internal void CheckIndexEntryLocalUserNotNull(IRequestIndexEntry index)
		{
			if (index.SourceUserId == null && index.TargetUserId == null)
			{
				base.WriteError(new ManagementObjectNotFoundException(Strings.RequestIndexEntryIsMissingLocalUserData(index.ToString())), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		internal void CheckIndexEntryTargetUserNotNull(IRequestIndexEntry index)
		{
			if (index.TargetUserId == null)
			{
				base.WriteError(new ManagementObjectNotFoundException(Strings.RequestIndexEntryIsMissingLocalUserData(index.ToString())), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private IRequestIndexEntry GetEntry()
		{
			LocalizedString? notFoundError = new LocalizedString?(Strings.ErrorNotEnoughInformationToFindRequest);
			LocalizedString? multipleFoundError = new LocalizedString?(Strings.ErrorNotEnoughInformationToFindUniqueRequest);
			TIdentity identity = this.Identity;
			if (identity.IndexToUse == null)
			{
				base.WriteError(new UnknownRequestIndexPermanentException(null), ErrorCategory.InvalidArgument, this.Identity);
				return null;
			}
			TIdentity identity2 = this.Identity;
			if (identity2.IndexToUse.RequestIndexEntryType == null)
			{
				return null;
			}
			TIdentity identity3 = this.Identity;
			if (identity3.IndexToUse.RequestIndexEntryType == typeof(MRSRequestWrapper))
			{
				IIdentityParameter id = this.Identity;
				IConfigDataProvider indexProvider = this.rjProvider.IndexProvider;
				IConfigurationSession configurationSession = this.currentOrgConfigSession;
				TIdentity identity4 = this.Identity;
				return (IRequestIndexEntry)base.GetDataObject<MRSRequestWrapper>(id, indexProvider, ADHandler.GetRootId(configurationSession, identity4.RequestType), notFoundError, multipleFoundError);
			}
			TIdentity identity5 = this.Identity;
			if (identity5.IndexToUse.RequestIndexEntryType == typeof(MRSRequestMailboxEntry))
			{
				IIdentityParameter id2 = this.Identity;
				IConfigDataProvider indexProvider2 = this.rjProvider.IndexProvider;
				IConfigurationSession configurationSession2 = this.currentOrgConfigSession;
				TIdentity identity6 = this.Identity;
				return (IRequestIndexEntry)base.GetDataObject<MRSRequestMailboxEntry>(id2, indexProvider2, ADHandler.GetRootId(configurationSession2, identity6.RequestType), notFoundError, multipleFoundError);
			}
			TIdentity identity7 = this.Identity;
			base.WriteError(new UnknownRequestIndexPermanentException(identity7.IndexToUse.ToString()), ErrorCategory.InvalidArgument, this.Identity);
			return null;
		}

		public const string ParameterIncludeReport = "IncludeReport";

		public const string ParameterRequestQueue = "RequestQueue";

		public const string ParameterRequestGuid = "RequestGuid";

		public const string ParameterIdentity = "Identity";

		public const string RequestQueueSet = "MigrationRequestQueue";

		public const string ParameterDiagnostic = "Diagnostic";

		public const string ParameterDiagnosticArgument = "DiagnosticArgument";

		private static readonly MRSRequestType requestType = MRSRequestIdParameter.GetRequestType<TIdentity>();

		private ADObjectId fromMdb;

		private IRecipientSession recipSession;

		private IRecipientSession gcSession;

		private IConfigurationSession currentOrgConfigSession;

		private ITopologyConfigurationSession configSession;

		private RequestJobProvider rjProvider;
	}
}
