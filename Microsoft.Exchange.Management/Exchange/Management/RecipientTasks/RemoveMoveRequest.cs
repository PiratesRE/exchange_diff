using System;
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
	[Cmdlet("Remove", "MoveRequest", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class RemoveMoveRequest : SetMoveRequestBase
	{
		public RemoveMoveRequest()
		{
			this.mdbGuid = Guid.Empty;
			this.mrCondition = RemoveMoveRequest.MoveRequestCondition.None;
			this.brokenADUser = null;
			this.validationMessageString = string.Empty;
		}

		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "MigrationMoveRequestQueue")]
		public DatabaseIdParameter MoveRequestQueue
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["MoveRequestQueue"];
			}
			set
			{
				base.Fields["MoveRequestQueue"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MigrationMoveRequestQueue")]
		public Guid MailboxGuid
		{
			get
			{
				return (Guid)(base.Fields["MailboxGuid"] ?? Guid.Empty);
			}
			set
			{
				base.Fields["MailboxGuid"] = value;
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
				switch (this.mrCondition)
				{
				case RemoveMoveRequest.MoveRequestCondition.FailedValidation:
					return Strings.ConfirmRemovalOfCorruptMoveRequest(base.LocalADUser.ToString(), this.validationMessageString);
				case RemoveMoveRequest.MoveRequestCondition.MdbDown:
					return Strings.ConfirmOrphanCannotConnectToMailboxDatabase(base.LocalADUser.ToString(), base.MRProvider.MdbGuid.ToString());
				case RemoveMoveRequest.MoveRequestCondition.AdUserMissingMoveData:
					return Strings.ConfirmOrphanUserMissingADData(this.brokenADUser.ToString());
				case RemoveMoveRequest.MoveRequestCondition.MoveCompleted:
					return Strings.ConfirmationMessageRemoveCompletedMoveRequest(base.LocalADUser.ToString());
				}
				if (base.ParameterSetName.Equals("MigrationMoveRequestQueue"))
				{
					return Strings.ConfirmationMessageRemoveMoveRequestDebug(this.MailboxGuid.ToString());
				}
				return Strings.ConfirmationMessageRemoveMoveRequest(base.Identity.ToString());
			}
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			this.mdbGuid = Guid.Empty;
			this.mrCondition = RemoveMoveRequest.MoveRequestCondition.None;
			this.brokenADUser = null;
			this.validationMessageString = string.Empty;
		}

		protected override IConfigurable PrepareDataObject()
		{
			try
			{
				if (base.ParameterSetName.Equals("Identity"))
				{
					TransactionalRequestJob transactionalRequestJob = (TransactionalRequestJob)base.PrepareDataObject();
					if (transactionalRequestJob == null)
					{
						if (this.mrCondition == RemoveMoveRequest.MoveRequestCondition.None)
						{
							this.mrCondition = RemoveMoveRequest.MoveRequestCondition.MissingMR;
						}
						MrsTracer.Cmdlet.Warning("Move Request is missing in the MDB", new object[0]);
						transactionalRequestJob = RequestJobBase.CreateDummyObject<TransactionalRequestJob>();
					}
					return transactionalRequestJob;
				}
				base.LocalADUser = null;
				MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.MoveRequestQueue, base.ConfigSession, null, new LocalizedString?(Strings.ErrorMailboxDatabaseNotFound(this.MoveRequestQueue.ToString())), new LocalizedString?(Strings.ErrorMailboxDatabaseNotUnique(this.MoveRequestQueue.ToString())));
				MailboxTaskHelper.VerifyDatabaseIsWithinScopeForRecipientCmdlets(base.SessionSettings, mailboxDatabase, new Task.ErrorLoggerDelegate(base.WriteError));
				Guid guid = mailboxDatabase.Guid;
				this.moveRequestQueueName = mailboxDatabase.ToString();
				base.MRProvider.AllowInvalid = true;
				return (TransactionalRequestJob)base.MRProvider.Read<TransactionalRequestJob>(new RequestJobObjectId(this.MailboxGuid, guid, null));
			}
			catch (MapiExceptionMdbOffline e)
			{
				this.HandleMdbDownException(e);
			}
			catch (MapiExceptionLogonFailed e2)
			{
				this.HandleMdbDownException(e2);
			}
			return RequestJobBase.CreateDummyObject<TransactionalRequestJob>();
		}

		protected override void CheckADUser()
		{
			base.CheckADUser();
			Guid a;
			Guid a2;
			RequestIndexEntryProvider.GetMoveGuids(base.LocalADUser, out a, out a2);
			if (a == Guid.Empty || a2 == Guid.Empty)
			{
				this.mrCondition = RemoveMoveRequest.MoveRequestCondition.AdUserMissingMoveData;
				this.brokenADUser = base.LocalADUser;
				base.LocalADUser = null;
			}
		}

		protected override void ValidateMoveRequest(TransactionalRequestJob moveRequest)
		{
			if (moveRequest.IsFake)
			{
				return;
			}
			if (!base.ParameterSetName.Equals("MigrationMoveRequestQueue"))
			{
				base.ValidateMoveRequestProtectionStatus(moveRequest);
				if (moveRequest.ValidationResult == RequestJobBase.ValidationResultEnum.Valid)
				{
					base.ValidateMoveRequestIsActive(moveRequest);
					if (moveRequest.RequestJobState == JobProcessingState.InProgress && RequestJobStateNode.RequestStateIs(moveRequest.StatusDetail, RequestState.Cleanup) && moveRequest.IdleTime < TimeSpan.FromMinutes(60.0))
					{
						base.WriteError(new CannotModifyCompletingRequestPermanentException(base.LocalADUser.ToString()), ErrorCategory.InvalidArgument, base.Identity);
					}
					if (moveRequest.CancelRequest)
					{
						base.WriteVerbose(Strings.MoveAlreadyCanceled);
					}
					if (moveRequest.Status == RequestStatus.Completed || moveRequest.Status == RequestStatus.CompletedWithWarning)
					{
						this.mrCondition = RemoveMoveRequest.MoveRequestCondition.MoveCompleted;
						return;
					}
				}
				else
				{
					this.mrCondition = RemoveMoveRequest.MoveRequestCondition.FailedValidation;
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
				TransactionalRequestJob moveRequest = this.DataObject;
				switch (this.mrCondition)
				{
				case RemoveMoveRequest.MoveRequestCondition.None:
					if (base.ParameterSetName.Equals("MigrationMoveRequestQueue"))
					{
						if (moveRequest != null)
						{
							if (moveRequest.CheckIfUnderlyingMessageHasChanged())
							{
								base.WriteVerbose(Strings.ReloadingMoveRequest);
								moveRequest.Refresh();
								this.ValidateMoveRequest(moveRequest);
							}
							base.MRProvider.Delete(moveRequest);
							CommonUtils.CatchKnownExceptions(delegate
							{
								ReportData reportData = new ReportData(moveRequest.ExchangeGuid, moveRequest.ReportVersion);
								reportData.Delete(this.MRProvider.SystemMailbox);
							}, null);
						}
						else
						{
							base.WriteError(new ManagementObjectNotFoundException(Strings.CouldNotRemoveMoveRequest(this.MailboxGuid.ToString())), ErrorCategory.InvalidArgument, base.Identity);
						}
					}
					else
					{
						base.InternalProcessRecord();
						this.CleanupADEntry(base.LocalADUser.Id, base.LocalADUser);
					}
					break;
				case RemoveMoveRequest.MoveRequestCondition.FailedValidation:
					base.WriteVerbose(Strings.MoveFailedValidation(this.validationMessageString));
					this.CleanupADEntry(base.LocalADUser.Id, base.LocalADUser);
					break;
				case RemoveMoveRequest.MoveRequestCondition.MdbDown:
					base.WriteVerbose(Strings.MailboxDatabaseIsDown);
					this.CleanupADEntry(base.LocalADUser.Id, base.LocalADUser);
					break;
				case RemoveMoveRequest.MoveRequestCondition.AdUserMissingMoveData:
					base.WriteVerbose(Strings.ADUserIsMissingData);
					this.CleanupADEntry(this.brokenADUser.Id, this.brokenADUser);
					break;
				case RemoveMoveRequest.MoveRequestCondition.MissingMR:
					base.WriteVerbose(Strings.MoveRequestIsMissing);
					this.CleanupADEntry(base.LocalADUser.Id, base.LocalADUser);
					break;
				case RemoveMoveRequest.MoveRequestCondition.MoveCompleted:
					if (moveRequest != null && !moveRequest.IsFake)
					{
						if (moveRequest.CheckIfUnderlyingMessageHasChanged())
						{
							base.WriteVerbose(Strings.ReloadingMoveRequest);
							moveRequest.Refresh();
							this.ValidateMoveRequest(moveRequest);
						}
						base.MRProvider.Delete(moveRequest);
						CommonUtils.CatchKnownExceptions(delegate
						{
							ReportData reportData = new ReportData(moveRequest.ExchangeGuid, moveRequest.ReportVersion);
							reportData.Delete(this.MRProvider.SystemMailbox);
						}, null);
						this.CleanupADEntry(base.LocalADUser.Id, base.LocalADUser);
					}
					else
					{
						base.WriteError(new ManagementObjectNotFoundException(Strings.CouldNotRemoveCompletedMoveRequest(base.LocalADUser.ToString())), ErrorCategory.InvalidArgument, base.Identity);
					}
					break;
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void ModifyMoveRequest(TransactionalRequestJob moveRequest)
		{
			this.mdbGuid = moveRequest.WorkItemQueueMdb.ObjectGuid;
			if (base.LocalADUser != null)
			{
				moveRequest.DomainControllerToUpdate = base.LocalADUser.OriginatingServer;
			}
			if (!moveRequest.CancelRequest)
			{
				moveRequest.CancelRequest = true;
				moveRequest.TimeTracker.SetTimestamp(RequestJobTimestamp.RequestCanceled, new DateTime?(DateTime.UtcNow));
				base.WriteVerbose(Strings.MarkingMoveJobAsCanceled);
				ReportData reportData = new ReportData(moveRequest.ExchangeGuid, moveRequest.ReportVersion);
				ConnectivityRec connectivityRec = new ConnectivityRec(ServerKind.Cmdlet, VersionInformation.MRS);
				reportData.Append(MrsStrings.ReportMoveRequestRemoved(base.ExecutingUserIdentity), connectivityRec);
				reportData.Flush(base.MRProvider.SystemMailbox);
			}
		}

		protected override void PostSaveAction()
		{
			if (!base.ParameterSetName.Equals("MigrationMoveRequestQueue") && this.mrCondition == RemoveMoveRequest.MoveRequestCondition.None && this.DataObject != null)
			{
				TransactionalRequestJob dataObject = this.DataObject;
				using (MailboxReplicationServiceClient mailboxReplicationServiceClient = dataObject.CreateMRSClient(base.ConfigSession, this.mdbGuid, base.UnreachableMrsServers))
				{
					if (mailboxReplicationServiceClient.ServerVersion[3])
					{
						mailboxReplicationServiceClient.RefreshMoveRequest2(this.DataObject.ExchangeGuid, this.mdbGuid, (int)dataObject.Flags, MoveRequestNotification.Canceled);
					}
					else
					{
						mailboxReplicationServiceClient.RefreshMoveRequest(this.DataObject.ExchangeGuid, this.mdbGuid, MoveRequestNotification.Canceled);
					}
				}
			}
		}

		private void CleanupADEntry(ADObjectId userId, ADUser adUser)
		{
			base.WriteVerbose(Strings.RemovingMoveJobFromAd(userId.ToString()));
			try
			{
				CommonUtils.CleanupMoveRequestInAD(base.WriteableSession, userId, adUser);
			}
			catch (UnableToReadADUserException)
			{
				base.WriteVerbose(Strings.UserNotInAd);
			}
		}

		private void HandleMdbDownException(Exception e)
		{
			if (base.ParameterSetName.Equals("Identity"))
			{
				MrsTracer.Cmdlet.Error("MailboxDatabase connection error when trying to read MoveRequest '{0}'.  Error details: {1}.", new object[]
				{
					base.Identity,
					CommonUtils.FullExceptionMessage(e)
				});
				this.mrCondition = RemoveMoveRequest.MoveRequestCondition.MdbDown;
			}
			if (base.ParameterSetName.Equals("MigrationMoveRequestQueue"))
			{
				MrsTracer.Cmdlet.Error("MailboxDatabase connection error when trying to read all MoveRequests from database '{0}'.  Error details: {1}.", new object[]
				{
					this.moveRequestQueueName,
					CommonUtils.FullExceptionMessage(e)
				});
				base.WriteError(new DatabaseConnectionTransientException(this.moveRequestQueueName), ErrorCategory.InvalidArgument, this.MoveRequestQueue);
			}
		}

		public const string ParameterMoveRequestQueue = "MoveRequestQueue";

		public const string ParameterMailboxGuid = "MailboxGuid";

		public const string MoveRequestQueueSet = "MigrationMoveRequestQueue";

		private Guid mdbGuid;

		private RemoveMoveRequest.MoveRequestCondition mrCondition;

		private ADUser brokenADUser;

		private string validationMessageString;

		private string moveRequestQueueName;

		private enum MoveRequestCondition
		{
			None,
			FailedValidation,
			MdbDown,
			AdUserMissingMoveData,
			MissingMR,
			MoveCompleted
		}
	}
}
