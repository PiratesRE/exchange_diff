using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class SetMoveRequestBase : SetTaskBase<TransactionalRequestJob>
	{
		public SetMoveRequestBase()
		{
			this.LocalADUser = null;
			this.WriteableSession = null;
			this.MRProvider = null;
			this.UnreachableMrsServers = new List<string>(5);
			this.ConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromRootOrgScopeSet(), 82, ".ctor", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\MoveRequest\\SetMoveRequestBase.cs");
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public MoveRequestIdParameter Identity
		{
			get
			{
				return (MoveRequestIdParameter)base.Fields["Identity"];
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

		internal ADUser LocalADUser { get; set; }

		internal IRecipientSession GCSession { get; private set; }

		internal IRecipientSession WriteableSession { get; private set; }

		internal ITopologyConfigurationSession ConfigSession { get; private set; }

		internal RequestJobProvider MRProvider { get; private set; }

		internal ADSessionSettings SessionSettings { get; private set; }

		private protected List<string> UnreachableMrsServers { protected get; private set; }

		protected string ExecutingUserIdentity
		{
			get
			{
				return base.ExecutingUserIdentityName;
			}
		}

		internal static LocalizedException TranslateExceptionHandler(Exception e)
		{
			if (e is LocalizedException)
			{
				if (!(e is MapiRetryableException))
				{
					if (!(e is MapiPermanentException))
					{
						goto IL_42;
					}
				}
				try
				{
					LocalizedException ex = StorageGlobals.TranslateMapiException(Strings.UnableToCommunicate, (LocalizedException)e, null, null, string.Empty, new object[0]);
					if (ex != null)
					{
						return ex;
					}
				}
				catch (ArgumentException)
				{
				}
			}
			IL_42:
			return null;
		}

		internal static bool IsKnownExceptionHandler(Exception exception, WriteVerboseDelegate writeVerbose)
		{
			if (exception is MapiRetryableException || exception is MapiPermanentException)
			{
				return true;
			}
			if (exception is MailboxReplicationPermanentException || exception is MailboxReplicationTransientException)
			{
				writeVerbose(CommonUtils.FullExceptionMessage(exception));
				return true;
			}
			return false;
		}

		internal static bool CheckUserOrgIdIsTenant(OrganizationId userOrgId)
		{
			return !userOrgId.Equals(OrganizationId.ForestWideOrgId);
		}

		internal void ValidateMoveRequestIsActive(RequestJobBase moveRequest)
		{
			if (moveRequest == null || moveRequest.Status == RequestStatus.None)
			{
				base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorUserNotBeingMoved(this.DataObject.ToString())), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (moveRequest.ValidationResult != RequestJobBase.ValidationResultEnum.Valid)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorInvalidMoveRequest(this.LocalADUser.ToString(), moveRequest.ValidationMessage.ToString())), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		internal void ValidateMoveRequestProtectionStatus(RequestJobBase moveRequest)
		{
			if (moveRequest.Protect && SetMoveRequestBase.CheckUserOrgIdIsTenant(base.ExecutingUserOrganizationId))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorMoveRequestIsProtected(this.DataObject.ToString())), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		internal void ValidateMoveRequestIsSettable(RequestJobBase moveRequest)
		{
			if (moveRequest.Status == RequestStatus.Completed || moveRequest.Status == RequestStatus.CompletedWithWarning)
			{
				base.WriteError(new CannotModifyCompletedRequestPermanentException(this.LocalADUser.ToString()), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (moveRequest.CancelRequest)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorMoveAlreadyCanceled(this.LocalADUser.ToString())), ErrorCategory.InvalidArgument, this.Identity);
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
				if (this.MRProvider != null)
				{
					this.MRProvider.Dispose();
					this.MRProvider = null;
				}
			}
			base.Dispose(disposing);
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			this.LocalADUser = null;
			if (this.DataObject != null)
			{
				this.DataObject.Dispose();
				this.DataObject = null;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADObjectId rootOrgContainerId = ADSystemConfigurationSession.GetRootOrgContainerId(this.DomainController, null);
			this.SessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, rootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			ADSessionSettings adsessionSettings = ADSessionSettings.RescopeToSubtree(this.SessionSettings);
			if (MapiTaskHelper.IsDatacenter || MapiTaskHelper.IsDatacenterDedicated)
			{
				adsessionSettings.IncludeSoftDeletedObjects = true;
			}
			this.GCSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, adsessionSettings, 413, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\MoveRequest\\SetMoveRequestBase.cs");
			this.WriteableSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, adsessionSettings, 419, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\MoveRequest\\SetMoveRequestBase.cs");
			if (base.CurrentTaskContext.CanBypassRBACScope)
			{
				this.WriteableSession.EnforceDefaultScope = false;
			}
			if (this.DataObject != null)
			{
				this.DataObject.Dispose();
				this.DataObject = null;
			}
			if (this.MRProvider != null)
			{
				this.MRProvider.Dispose();
				this.MRProvider = null;
			}
			this.MRProvider = new RequestJobProvider(this.WriteableSession, this.ConfigSession);
			return this.MRProvider;
		}

		protected override IConfigurable PrepareDataObject()
		{
			this.LocalADUser = (ADUser)RecipientTaskHelper.ResolveDataObject<ADUser>(this.WriteableSession, this.GCSession, base.ServerSettings, this.Identity, null, base.OptionalIdentityData, this.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
			this.CheckADUser();
			if (this.LocalADUser == null)
			{
				return null;
			}
			this.WriteableSession = (IRecipientSession)TaskHelper.UnderscopeSessionToOrganization(this.WriteableSession, this.LocalADUser.OrganizationId, true);
			base.CurrentOrganizationId = this.LocalADUser.OrganizationId;
			this.MRProvider.IndexProvider.RecipientSession = this.WriteableSession;
			return (TransactionalRequestJob)this.MRProvider.Read<TransactionalRequestJob>(new RequestJobObjectId(this.LocalADUser));
		}

		protected virtual void CheckADUser()
		{
			if (this.LocalADUser.MailboxMoveStatus == RequestStatus.None)
			{
				base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorUserNotBeingMoved(this.LocalADUser.ToString())), ErrorCategory.InvalidArgument, this.Identity);
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
				base.InternalValidate();
				TransactionalRequestJob dataObject = this.DataObject;
				this.ValidateMoveRequest(dataObject);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected virtual void ValidateMoveRequest(TransactionalRequestJob moveRequest)
		{
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			try
			{
				base.WriteVerbose(Strings.SettingMoveRequest);
				TransactionalRequestJob dataObject = this.DataObject;
				int num = 1;
				for (;;)
				{
					if (dataObject.CheckIfUnderlyingMessageHasChanged())
					{
						base.WriteVerbose(Strings.ReloadingMoveRequest);
						dataObject.Refresh();
						this.ValidateMoveRequest(dataObject);
					}
					this.ModifyMoveRequest(dataObject);
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

		protected virtual void ModifyMoveRequest(TransactionalRequestJob moveRequest)
		{
		}

		protected virtual void PostSaveAction()
		{
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

		protected bool IsFieldSet(string fieldName)
		{
			return base.Fields.IsChanged(fieldName) || base.Fields.IsModified(fieldName);
		}

		public const string TaskNoun = "MoveRequest";

		public const string ParameterIdentity = "Identity";
	}
}
