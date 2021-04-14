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
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class RemoveRequest<TIdentity> : SetRequestBase<TIdentity> where TIdentity : MRSRequestIdParameter
	{
		public RemoveRequest()
		{
			this.mdbGuid = Guid.Empty;
			this.requestCondition = RemoveRequest<TIdentity>.RequestCondition.None;
			this.brokenIndexEntry = null;
			this.indexEntries = new List<IRequestIndexEntry>();
			this.validationMessageString = string.Empty;
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNull]
		public override TIdentity Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
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

		[Parameter(Mandatory = true, ParameterSetName = "MigrationRequestQueue")]
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.DataObject != null)
				{
					this.validationMessageString = this.DataObject.ValidationMessage.ToString();
				}
				switch (this.requestCondition)
				{
				case RemoveRequest<TIdentity>.RequestCondition.FailedValidation:
					return Strings.ConfirmRemovalOfCorruptRequest(this.GenerateIndexEntryString(base.IndexEntry), this.validationMessageString);
				case RemoveRequest<TIdentity>.RequestCondition.IndexEntryMissingData:
					return Strings.ConfirmRemoveIndexEntryMissingADData(this.GenerateIndexEntryString(this.brokenIndexEntry));
				case RemoveRequest<TIdentity>.RequestCondition.Completed:
					return Strings.ConfirmationMessageRemoveCompletedRequest(this.GenerateIndexEntryString(base.IndexEntry));
				case RemoveRequest<TIdentity>.RequestCondition.MdbDown:
					return Strings.ConfirmRemovalOfRequestForInaccessibleDatabase(this.GenerateIndexEntryString(base.IndexEntry), this.mdbGuid);
				}
				if (base.ParameterSetName.Equals("MigrationRequestQueue"))
				{
					return Strings.ConfirmationMessageRemoveRequestDebug(this.RequestGuid.ToString());
				}
				return Strings.ConfirmationMessageRemoveRequest(this.GenerateIndexEntryString(base.IndexEntry));
			}
		}

		internal abstract string GenerateIndexEntryString(IRequestIndexEntry entry);

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			this.mdbGuid = Guid.Empty;
			this.requestCondition = RemoveRequest<TIdentity>.RequestCondition.None;
			this.brokenIndexEntry = null;
			this.indexEntries.Clear();
			this.validationMessageString = string.Empty;
		}

		protected override IConfigurable PrepareDataObject()
		{
			this.brokenIndexEntry = null;
			this.indexEntries.Clear();
			try
			{
				if (base.ParameterSetName.Equals("Identity"))
				{
					TransactionalRequestJob transactionalRequestJob = (TransactionalRequestJob)base.PrepareDataObject();
					if (transactionalRequestJob == null)
					{
						if (this.requestCondition == RemoveRequest<TIdentity>.RequestCondition.None)
						{
							this.requestCondition = RemoveRequest<TIdentity>.RequestCondition.MissingRJ;
							this.indexEntries.Add(base.IndexEntry);
						}
						else
						{
							this.indexEntries.Add(this.brokenIndexEntry);
						}
						MrsTracer.Cmdlet.Warning("Request is missing from the MDB", new object[0]);
						transactionalRequestJob = RequestJobBase.CreateDummyObject<TransactionalRequestJob>();
					}
					else
					{
						this.indexEntries.AddRange(transactionalRequestJob.IndexEntries);
						if (!this.indexEntries.Contains(base.IndexEntry))
						{
							this.indexEntries.Add(base.IndexEntry);
						}
					}
					return transactionalRequestJob;
				}
				base.IndexEntry = null;
				MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.RequestQueue, base.ConfigSession, null, new LocalizedString?(Strings.ErrorMailboxDatabaseNotFound(this.RequestQueue.ToString())), new LocalizedString?(Strings.ErrorMailboxDatabaseNotUnique(this.RequestQueue.ToString())));
				MailboxTaskHelper.VerifyDatabaseIsWithinScopeForRecipientCmdlets(base.CurrentOrgConfigSession.SessionSettings, mailboxDatabase, new Task.ErrorLoggerDelegate(base.WriteError));
				Guid guid = mailboxDatabase.Guid;
				this.requestQueueName = mailboxDatabase.ToString();
				base.RJProvider.AllowInvalid = true;
				TransactionalRequestJob transactionalRequestJob2 = null;
				try
				{
					transactionalRequestJob2 = (TransactionalRequestJob)base.RJProvider.Read<TransactionalRequestJob>(new RequestJobObjectId(this.RequestGuid, guid, null));
					if (transactionalRequestJob2 != null)
					{
						if (transactionalRequestJob2.TargetUser != null && transactionalRequestJob2.TargetUserId != null)
						{
							this.ResolveADUser(transactionalRequestJob2.TargetUserId);
						}
						if (transactionalRequestJob2.SourceUser != null && transactionalRequestJob2.SourceUserId != null)
						{
							this.ResolveADUser(transactionalRequestJob2.SourceUserId);
						}
					}
					TransactionalRequestJob result = transactionalRequestJob2;
					transactionalRequestJob2 = null;
					return result;
				}
				finally
				{
					if (transactionalRequestJob2 != null)
					{
						transactionalRequestJob2.Dispose();
						transactionalRequestJob2 = null;
					}
				}
			}
			catch (MapiExceptionMdbOffline e)
			{
				this.HandleMdbDownException(e);
				this.indexEntries.Add(base.IndexEntry);
			}
			catch (MapiExceptionLogonFailed e2)
			{
				this.HandleMdbDownException(e2);
				this.indexEntries.Add(base.IndexEntry);
			}
			return RequestJobBase.CreateDummyObject<TransactionalRequestJob>();
		}

		protected override void CheckIndexEntry()
		{
			if (base.IndexEntry != null)
			{
				RequestJobObjectId requestJobId = base.IndexEntry.GetRequestJobId();
				if (requestJobId == null || requestJobId.RequestGuid == Guid.Empty || requestJobId.MdbGuid == Guid.Empty)
				{
					this.requestCondition = RemoveRequest<TIdentity>.RequestCondition.IndexEntryMissingData;
					this.brokenIndexEntry = base.IndexEntry;
					base.IndexEntry = null;
				}
			}
		}

		protected override void ValidateRequest(TransactionalRequestJob requestJob)
		{
			if (requestJob.IsFake)
			{
				return;
			}
			base.ValidateRequest(requestJob);
			if (!base.ParameterSetName.Equals("MigrationRequestQueue"))
			{
				base.ValidateRequestProtectionStatus(requestJob);
				if (requestJob.ValidationResult == RequestJobBase.ValidationResultEnum.Valid)
				{
					base.ValidateRequestIsActive(requestJob);
					if (requestJob.RequestJobState == JobProcessingState.InProgress && RequestJobStateNode.RequestStateIs(requestJob.StatusDetail, RequestState.Completion) && !RequestJobStateNode.RequestStateIs(requestJob.StatusDetail, RequestState.IncrementalSync) && requestJob.IdleTime < TimeSpan.FromMinutes(60.0))
					{
						base.WriteError(new CannotModifyCompletingRequestPermanentException(base.IndexEntry.ToString()), ErrorCategory.InvalidArgument, this.Identity);
					}
					if (requestJob.CancelRequest)
					{
						base.WriteVerbose(Strings.RequestAlreadyCanceled);
					}
					if (requestJob.Status == RequestStatus.Completed || requestJob.Status == RequestStatus.CompletedWithWarning)
					{
						if (this.requestCondition != RemoveRequest<TIdentity>.RequestCondition.None && this.requestCondition != RemoveRequest<TIdentity>.RequestCondition.Completed)
						{
							base.WriteError(new CannotRemoveCompletedDuringCancelPermanentException(base.IndexEntry.ToString()), ErrorCategory.InvalidArgument, this.Identity);
						}
						this.requestCondition = RemoveRequest<TIdentity>.RequestCondition.Completed;
						return;
					}
				}
				else
				{
					this.requestCondition = RemoveRequest<TIdentity>.RequestCondition.FailedValidation;
				}
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
				TransactionalRequestJob requestJob = this.DataObject;
				switch (this.requestCondition)
				{
				case RemoveRequest<TIdentity>.RequestCondition.None:
					if (base.ParameterSetName.Equals("MigrationRequestQueue"))
					{
						if (requestJob != null)
						{
							if (requestJob.CheckIfUnderlyingMessageHasChanged())
							{
								base.WriteVerbose(Strings.ReloadingRequest);
								requestJob.Refresh();
								this.ValidateRequest(requestJob);
							}
							base.RJProvider.Delete(requestJob);
							CommonUtils.CatchKnownExceptions(delegate
							{
								ReportData reportData = new ReportData(requestJob.RequestGuid, requestJob.ReportVersion);
								reportData.Delete(this.RJProvider.SystemMailbox);
							}, null);
						}
						else
						{
							base.WriteError(new ManagementObjectNotFoundException(Strings.CouldNotRemoveRequest(this.RequestGuid.ToString())), ErrorCategory.InvalidArgument, this.Identity);
						}
					}
					else
					{
						base.InternalProcessRecord();
						this.CleanupIndexEntries();
					}
					break;
				case RemoveRequest<TIdentity>.RequestCondition.FailedValidation:
					base.WriteVerbose(Strings.RequestFailedValidation(this.validationMessageString));
					this.CleanupIndexEntries();
					break;
				case RemoveRequest<TIdentity>.RequestCondition.IndexEntryMissingData:
					base.WriteVerbose(Strings.IndexEntryIsMissingData);
					this.CleanupIndexEntries();
					break;
				case RemoveRequest<TIdentity>.RequestCondition.MissingRJ:
					base.WriteVerbose(Strings.RequestIsMissing);
					this.CleanupIndexEntries();
					break;
				case RemoveRequest<TIdentity>.RequestCondition.Completed:
					if (requestJob != null && !requestJob.IsFake)
					{
						if (requestJob.CheckIfUnderlyingMessageHasChanged())
						{
							base.WriteVerbose(Strings.ReloadingRequest);
							requestJob.Refresh();
							this.ValidateRequest(requestJob);
						}
						base.RJProvider.Delete(requestJob);
						CommonUtils.CatchKnownExceptions(delegate
						{
							ReportData reportData = new ReportData(requestJob.RequestGuid, requestJob.ReportVersion);
							reportData.Delete(this.RJProvider.SystemMailbox);
						}, null);
						this.CleanupIndexEntries();
					}
					else
					{
						base.WriteError(new ManagementObjectNotFoundException(Strings.CouldNotRemoveCompletedRequest(base.IndexEntry.ToString())), ErrorCategory.InvalidArgument, this.Identity);
					}
					break;
				case RemoveRequest<TIdentity>.RequestCondition.MdbDown:
					base.WriteVerbose(Strings.RequestOnInaccessibleDatabase);
					this.CleanupIndexEntries();
					break;
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void ModifyRequest(TransactionalRequestJob requestJob)
		{
			this.mdbGuid = requestJob.WorkItemQueueMdb.ObjectGuid;
			if (requestJob.TargetUser != null)
			{
				requestJob.DomainControllerToUpdate = requestJob.TargetUser.OriginatingServer;
			}
			else if (requestJob.SourceUser != null)
			{
				requestJob.DomainControllerToUpdate = requestJob.SourceUser.OriginatingServer;
			}
			if (!requestJob.CancelRequest)
			{
				requestJob.CancelRequest = true;
				base.WriteVerbose(Strings.MarkingMoveJobAsCanceled);
				ReportData reportData = new ReportData(requestJob.RequestGuid, requestJob.ReportVersion);
				ConnectivityRec connectivityRec = new ConnectivityRec(ServerKind.Cmdlet, VersionInformation.MRS);
				reportData.Append(MrsStrings.ReportRequestRemoved(base.ExecutingUserIdentity), connectivityRec);
				reportData.Flush(base.RJProvider.SystemMailbox);
			}
		}

		protected override void PostSaveAction()
		{
			if (!base.ParameterSetName.Equals("MigrationRequestQueue") && this.requestCondition == RemoveRequest<TIdentity>.RequestCondition.None && this.DataObject != null)
			{
				RequestTaskHelper.TickleMRS(this.DataObject, MoveRequestNotification.Canceled, this.mdbGuid, base.ConfigSession, base.UnreachableMrsServers);
			}
		}

		protected override ADUser ResolveADUser(ADObjectId userId)
		{
			return RequestTaskHelper.ResolveADUser(base.WriteableSession, base.GCSession, base.ServerSettings, new UserIdParameter(userId), base.OptionalIdentityData, base.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), true);
		}

		private void HandleMdbDownException(Exception e)
		{
			if (base.ParameterSetName.Equals("Identity"))
			{
				MrsTracer.Cmdlet.Error("MailboxDatabase connection error when trying to read Request '{0}'.  Error details: {1}.", new object[]
				{
					this.Identity,
					CommonUtils.FullExceptionMessage(e)
				});
				this.requestCondition = RemoveRequest<TIdentity>.RequestCondition.MdbDown;
			}
			if (base.ParameterSetName.Equals("MigrationRequestQueue"))
			{
				MrsTracer.Cmdlet.Error("MailboxDatabase connection error when trying to read the Request from database '{0}'.  Error details: {1}.", new object[]
				{
					this.requestQueueName,
					CommonUtils.FullExceptionMessage(e)
				});
				base.WriteError(new DatabaseConnectionTransientException(this.requestQueueName), ErrorCategory.InvalidArgument, this.RequestQueue);
			}
		}

		private void CleanupIndexEntries()
		{
			foreach (IRequestIndexEntry requestIndexEntry in this.indexEntries)
			{
				base.WriteVerbose(Strings.RemovingIndexEntry(requestIndexEntry.ToString()));
				base.RJProvider.IndexProvider.Delete(requestIndexEntry);
			}
			TransactionalRequestJob dataObject = this.DataObject;
			if (dataObject != null)
			{
				dataObject.RemoveAsyncNotification();
			}
		}

		public const string ParameterRequestQueue = "RequestQueue";

		public const string ParameterRequestGuid = "RequestGuid";

		public const string RequestQueueSet = "MigrationRequestQueue";

		private Guid mdbGuid;

		private RemoveRequest<TIdentity>.RequestCondition requestCondition;

		private IRequestIndexEntry brokenIndexEntry;

		private List<IRequestIndexEntry> indexEntries;

		private string validationMessageString;

		private string requestQueueName;

		private enum RequestCondition
		{
			None,
			FailedValidation,
			IndexEntryMissingData,
			MissingRJ,
			Completed,
			MdbDown
		}
	}
}
