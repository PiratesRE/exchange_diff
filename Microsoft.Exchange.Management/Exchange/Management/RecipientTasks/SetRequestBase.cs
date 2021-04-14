using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class SetRequestBase<TIdentity> : SetTaskBase<TransactionalRequestJob> where TIdentity : MRSRequestIdParameter
	{
		public SetRequestBase()
		{
			this.WriteableSession = null;
			this.RJProvider = null;
			this.UnreachableMrsServers = new List<string>(5);
			this.RequestName = null;
			this.ConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromRootOrgScopeSet(), 84, ".ctor", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\RequestBase\\SetRequestBase.cs");
		}

		public virtual TIdentity Identity
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

		internal IRequestIndexEntry IndexEntry { get; set; }

		internal IRecipientSession GCSession { get; private set; }

		internal IRecipientSession WriteableSession { get; private set; }

		internal IConfigurationSession CurrentOrgConfigSession { get; private set; }

		internal ITopologyConfigurationSession ConfigSession { get; private set; }

		internal RequestJobProvider RJProvider { get; private set; }

		internal string RequestName { get; private set; }

		protected virtual RequestIndexId DefaultRequestIndexId
		{
			get
			{
				return new RequestIndexId(RequestIndexLocation.AD);
			}
		}

		private protected List<string> UnreachableMrsServers { protected get; private set; }

		protected string ExecutingUserIdentity
		{
			get
			{
				return base.ExecutingUserIdentityName;
			}
		}

		internal void ValidateRequestIsActive(RequestJobBase requestJob)
		{
			if (requestJob == null || requestJob.Status == RequestStatus.None)
			{
				TIdentity identity = this.Identity;
				base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorCouldNotFindRequest(identity.ToString())), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (requestJob.ValidationResult != RequestJobBase.ValidationResultEnum.Valid)
			{
				TIdentity identity2 = this.Identity;
				base.WriteError(new InvalidRequestPermanentException(identity2.ToString(), requestJob.ValidationMessage), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		internal void ValidateRequestProtectionStatus(RequestJobBase requestJob)
		{
			if (requestJob.Protect && RequestTaskHelper.CheckUserOrgIdIsTenant(base.ExecutingUserOrganizationId))
			{
				base.WriteError(new RequestIsProtectedPermanentException(requestJob.Name), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		internal void ValidateRequestIsRunnable(RequestJobBase requestJob)
		{
			if (requestJob.Status == RequestStatus.Completed || requestJob.Status == RequestStatus.CompletedWithWarning)
			{
				base.WriteError(new CannotModifyCompletedRequestPermanentException(requestJob.Name), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (requestJob.RehomeRequest)
			{
				base.WriteError(new CannotModifyRehomingRequestTransientException(requestJob.Name), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		internal void ValidateRequestIsNotCancelled(RequestJobBase requestJob)
		{
			if (requestJob.CancelRequest)
			{
				base.WriteError(new CannotSetCancelledRequestPermanentException(requestJob.Name), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		protected void ValidateRequestType(TransactionalRequestJob requestJob)
		{
			if (requestJob.RequestType != SetRequestBase<TIdentity>.RequestType)
			{
				base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorNotEnoughInformationToFindRequestOfCorrectType), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.DataObject != null)
				{
					((IDisposable)this.DataObject).Dispose();
					this.DataObject = null;
				}
				if (this.RJProvider != null)
				{
					this.RJProvider.Dispose();
					this.RJProvider = null;
				}
			}
			base.Dispose(disposing);
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			if (this.DataObject != null)
			{
				this.DataObject.Dispose();
				this.DataObject = null;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			if (MapiTaskHelper.IsDatacenter || MapiTaskHelper.IsDatacenterDedicated)
			{
				adsessionSettings.IncludeSoftDeletedObjects = true;
				adsessionSettings.IncludeInactiveMailbox = true;
			}
			this.CurrentOrgConfigSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, null, adsessionSettings, 375, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\RequestBase\\SetRequestBase.cs");
			adsessionSettings = ADSessionSettings.RescopeToSubtree(adsessionSettings);
			this.GCSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, adsessionSettings, 386, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\RequestBase\\SetRequestBase.cs");
			adsessionSettings.IncludeInactiveMailbox = true;
			this.WriteableSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, adsessionSettings, 394, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\RequestBase\\SetRequestBase.cs");
			if (base.CurrentTaskContext.CanBypassRBACScope)
			{
				this.WriteableSession.EnforceDefaultScope = false;
			}
			if (this.DataObject != null)
			{
				this.DataObject.Dispose();
				this.DataObject = null;
			}
			if (this.RJProvider != null)
			{
				this.RJProvider.Dispose();
				this.RJProvider = null;
			}
			this.RJProvider = new RequestJobProvider(this.WriteableSession, this.CurrentOrgConfigSession);
			return this.RJProvider;
		}

		protected override IConfigurable PrepareDataObject()
		{
			TIdentity identity = this.Identity;
			if (identity.OrganizationId != null)
			{
				IDirectorySession writeableSession = this.WriteableSession;
				TIdentity identity2 = this.Identity;
				if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(writeableSession, identity2.OrganizationId))
				{
					IDirectorySession writeableSession2 = this.WriteableSession;
					TIdentity identity3 = this.Identity;
					this.WriteableSession = (IRecipientSession)TaskHelper.UnderscopeSessionToOrganization(writeableSession2, identity3.OrganizationId, true);
				}
				IDirectorySession currentOrgConfigSession = this.CurrentOrgConfigSession;
				TIdentity identity4 = this.Identity;
				if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(currentOrgConfigSession, identity4.OrganizationId))
				{
					IDirectorySession currentOrgConfigSession2 = this.CurrentOrgConfigSession;
					TIdentity identity5 = this.Identity;
					this.CurrentOrgConfigSession = (ITenantConfigurationSession)TaskHelper.UnderscopeSessionToOrganization(currentOrgConfigSession2, identity5.OrganizationId, true);
					this.RJProvider.IndexProvider.ConfigSession = this.CurrentOrgConfigSession;
				}
			}
			TIdentity identity6 = this.Identity;
			if (!string.IsNullOrEmpty(identity6.MailboxName))
			{
				IRecipientSession writeableSession3 = this.WriteableSession;
				IRecipientSession gcsession = this.GCSession;
				ADServerSettings serverSettings = base.ServerSettings;
				TIdentity identity7 = this.Identity;
				ADUser aduser = RequestTaskHelper.ResolveADUser(writeableSession3, gcsession, serverSettings, new UserIdParameter(identity7.MailboxName), base.OptionalIdentityData, this.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), true);
				if (aduser != null)
				{
					TIdentity identity8 = this.Identity;
					identity8.MailboxId = aduser.Id;
					if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(this.WriteableSession, aduser))
					{
						this.WriteableSession = (IRecipientSession)TaskHelper.UnderscopeSessionToOrganization(this.WriteableSession, aduser.OrganizationId, true);
					}
					if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(this.CurrentOrgConfigSession, aduser))
					{
						this.CurrentOrgConfigSession = (ITenantConfigurationSession)TaskHelper.UnderscopeSessionToOrganization(this.CurrentOrgConfigSession, aduser.OrganizationId, true);
						this.RJProvider.IndexProvider.ConfigSession = this.CurrentOrgConfigSession;
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
				if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(this.WriteableSession, adorganizationalUnit))
				{
					this.WriteableSession = (IRecipientSession)TaskHelper.UnderscopeSessionToOrganization(this.WriteableSession, adorganizationalUnit.OrganizationId, true);
				}
				if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(this.CurrentOrgConfigSession, adorganizationalUnit))
				{
					this.CurrentOrgConfigSession = (ITenantConfigurationSession)TaskHelper.UnderscopeSessionToOrganization(this.CurrentOrgConfigSession, adorganizationalUnit.OrganizationId, true);
					this.RJProvider.IndexProvider.ConfigSession = this.CurrentOrgConfigSession;
				}
			}
			TIdentity identity13 = this.Identity;
			identity13.SetDefaultIndex(this.DefaultRequestIndexId);
			this.IndexEntry = this.GetEntry();
			this.CheckIndexEntry();
			if (this.IndexEntry == null)
			{
				return null;
			}
			RequestJobObjectId requestJobId = this.IndexEntry.GetRequestJobId();
			if (this.IndexEntry.TargetUserId != null)
			{
				requestJobId.TargetUser = this.ResolveADUser(this.IndexEntry.TargetUserId);
			}
			if (this.IndexEntry.SourceUserId != null)
			{
				requestJobId.SourceUser = this.ResolveADUser(this.IndexEntry.SourceUserId);
			}
			return (TransactionalRequestJob)this.RJProvider.Read<TransactionalRequestJob>(requestJobId);
		}

		protected virtual void CheckIndexEntry()
		{
			if (this.IndexEntry.SourceUserId == null && this.IndexEntry.TargetUserId == null)
			{
				base.WriteError(new ManagementObjectNotFoundException(Strings.RequestIndexEntryIsMissingLocalUserData(this.IndexEntry.Name)), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			try
			{
				using (new ADSessionSettingsFactory.InactiveMailboxVisibilityEnabler())
				{
					base.InternalValidate();
				}
				TransactionalRequestJob dataObject = this.DataObject;
				this.RequestName = dataObject.Name;
				this.ValidateRequest(dataObject);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected virtual void ValidateRequest(TransactionalRequestJob requestJob)
		{
			this.ValidateRequestType(requestJob);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			try
			{
				base.WriteVerbose(Strings.SettingRequest);
				TransactionalRequestJob dataObject = this.DataObject;
				int num = 1;
				for (;;)
				{
					if (dataObject.CheckIfUnderlyingMessageHasChanged())
					{
						base.WriteVerbose(Strings.ReloadingRequest);
						dataObject.Refresh();
						this.ValidateRequest(dataObject);
					}
					this.ModifyRequest(dataObject);
					try
					{
						base.InternalProcessRecord();
						RequestJobLog.Write(dataObject);
					}
					catch (MapiExceptionObjectChanged)
					{
						if (num >= 5 || base.Stopping)
						{
							throw;
						}
						num++;
						continue;
					}
					break;
				}
				CommonUtils.CatchKnownExceptions(delegate
				{
					this.PostSaveAction();
				}, delegate(Exception ex)
				{
					this.WriteWarning(MrsStrings.PostSaveActionFailed(CommonUtils.FullExceptionMessage(ex)));
				});
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected virtual void ModifyRequest(TransactionalRequestJob requestJob)
		{
		}

		protected virtual void PostSaveAction()
		{
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

		protected virtual ADUser ResolveADUser(ADObjectId userId)
		{
			return RequestTaskHelper.ResolveADUser(this.WriteableSession, this.GCSession, base.ServerSettings, new MailboxOrMailUserIdParameter(userId), base.OptionalIdentityData, this.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), true);
		}

		protected bool IsFieldSet(string fieldName)
		{
			return base.Fields.IsChanged(fieldName) || base.Fields.IsModified(fieldName);
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
				IConfigDataProvider indexProvider = this.RJProvider.IndexProvider;
				IConfigurationSession currentOrgConfigSession = this.CurrentOrgConfigSession;
				TIdentity identity4 = this.Identity;
				return (IRequestIndexEntry)base.GetDataObject<MRSRequestWrapper>(id, indexProvider, ADHandler.GetRootId(currentOrgConfigSession, identity4.RequestType), notFoundError, multipleFoundError);
			}
			TIdentity identity5 = this.Identity;
			if (identity5.IndexToUse.RequestIndexEntryType == typeof(MRSRequestMailboxEntry))
			{
				IIdentityParameter id2 = this.Identity;
				IConfigDataProvider indexProvider2 = this.RJProvider.IndexProvider;
				IConfigurationSession currentOrgConfigSession2 = this.CurrentOrgConfigSession;
				TIdentity identity6 = this.Identity;
				return (IRequestIndexEntry)base.GetDataObject<MRSRequestMailboxEntry>(id2, indexProvider2, ADHandler.GetRootId(currentOrgConfigSession2, identity6.RequestType), notFoundError, multipleFoundError);
			}
			TIdentity identity7 = this.Identity;
			base.WriteError(new UnknownRequestIndexPermanentException(identity7.IndexToUse.ToString()), ErrorCategory.InvalidArgument, this.Identity);
			return null;
		}

		public const string ParameterIdentity = "Identity";

		private static readonly MRSRequestType RequestType = MRSRequestIdParameter.GetRequestType<TIdentity>();
	}
}
