using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RehomeJob : LightJobBase
	{
		public RehomeJob(Guid requestGuid, ADObjectId currentQueue, ADObjectId newQueue, MapiStore currentSystemMailbox, byte[] currentMessageId) : base(requestGuid, Guid.Empty, null, null)
		{
			this.CurrentRequestQueue = currentQueue;
			this.NewRequestQueue = newQueue;
			this.CurrentSystemMailbox = currentSystemMailbox;
			this.NewSystemMailbox = null;
			this.CurrentMessageId = currentMessageId;
			this.RehomeFailure = null;
		}

		protected ADObjectId CurrentRequestQueue { get; set; }

		protected ADObjectId NewRequestQueue { get; set; }

		protected MapiStore CurrentSystemMailbox { get; set; }

		protected MapiStore NewSystemMailbox { get; set; }

		protected byte[] CurrentMessageId { get; set; }

		protected Exception RehomeFailure { get; set; }

		protected override Guid RequestQueueGuid
		{
			get
			{
				if (this.RehomeFailure == null)
				{
					return this.NewRequestQueue.ObjectGuid;
				}
				return this.CurrentRequestQueue.ObjectGuid;
			}
			set
			{
				base.RequestQueueGuid = value;
			}
		}

		protected override MapiStore SystemMailbox
		{
			get
			{
				if (this.RehomeFailure == null)
				{
					return this.NewSystemMailbox;
				}
				return this.CurrentSystemMailbox;
			}
			set
			{
				base.SystemMailbox = value;
			}
		}

		protected override byte[] MessageId
		{
			get
			{
				if (this.RehomeFailure == null)
				{
					return null;
				}
				return base.MessageId;
			}
			set
			{
				base.MessageId = value;
			}
		}

		public override void Run()
		{
			Guid currentQueueGuid = this.CurrentRequestQueue.ObjectGuid;
			Guid newQueueGuid = this.NewRequestQueue.ObjectGuid;
			RequestJobObjectId currentId = new RequestJobObjectId(base.RequestGuid, currentQueueGuid, this.MessageId);
			RequestJobObjectId newId = new RequestJobObjectId(base.RequestGuid, newQueueGuid, null);
			CommonUtils.CatchKnownExceptions(delegate
			{
				DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(this.NewRequestQueue, null, null, FindServerFlags.None);
				if (databaseInformation.ServerVersion < Server.E15MinVersion)
				{
					throw new UnsupportedRehomeTargetVersionPermanentException(newQueueGuid.ToString(), new ServerVersion(databaseInformation.ServerVersion).ToString());
				}
				this.NewSystemMailbox = MapiUtils.GetSystemMailbox(newQueueGuid);
				using (RequestJobProvider currentQueueProvider = new RequestJobProvider(currentQueueGuid, this.CurrentSystemMailbox))
				{
					using (RequestJobProvider newQueueProvider = new RequestJobProvider(newQueueGuid, this.NewSystemMailbox))
					{
						using (TransactionalRequestJob requestJob = (TransactionalRequestJob)currentQueueProvider.Read<TransactionalRequestJob>(currentId))
						{
							if (requestJob != null)
							{
								RequestJobProvider origProvider = requestJob.Provider;
								MoveObjectInfo<RequestJobXML> origMO = requestJob.MoveObject;
								ReportData currentReport = new ReportData(requestJob.IdentifyingGuid, requestJob.ReportVersion);
								currentReport.Load(currentQueueProvider.SystemMailbox);
								ReportData newReport = new ReportData(currentReport.IdentifyingGuid, requestJob.ReportVersion);
								newReport.Append(currentReport.Entries);
								try
								{
									requestJob.Provider = null;
									requestJob.MoveObject = null;
									requestJob.RequestQueue = this.NewRequestQueue;
									requestJob.Identity = newId;
									requestJob.OriginatingMDBGuid = Guid.Empty;
									requestJob.RehomeRequest = false;
									if (requestJob.IndexEntries != null)
									{
										foreach (IRequestIndexEntry requestIndexEntry in requestJob.IndexEntries)
										{
											requestIndexEntry.StorageMDB = this.NewRequestQueue;
										}
									}
									newQueueProvider.Save(requestJob);
									CommonUtils.CatchKnownExceptions(delegate
									{
										newReport.Append(MrsStrings.ReportJobRehomed(currentQueueGuid.ToString(), newQueueGuid.ToString()));
										newReport.Flush(newQueueProvider.SystemMailbox);
									}, null);
									CommonUtils.CatchKnownExceptions(delegate
									{
										requestJob.Provider = origProvider;
										requestJob.MoveObject = origMO;
										requestJob.RequestQueue = this.CurrentRequestQueue;
										requestJob.Identity = currentId;
										requestJob.OriginatingMDBGuid = currentQueueGuid;
										requestJob.RehomeRequest = true;
										MapiUtils.RetryOnObjectChanged(delegate
										{
											currentQueueProvider.Delete(requestJob);
										});
										currentReport.Delete(currentQueueProvider.SystemMailbox);
									}, null);
								}
								catch
								{
									requestJob.Provider = origProvider;
									requestJob.MoveObject = origMO;
									requestJob.RequestQueue = this.CurrentRequestQueue;
									requestJob.Identity = currentId;
									requestJob.OriginatingMDBGuid = currentQueueGuid;
									requestJob.RehomeRequest = true;
									throw;
								}
							}
						}
					}
				}
			}, delegate(Exception failure)
			{
				MrsTracer.Service.Warning("Failed to rehome request.", new object[0]);
				this.RehomeFailure = failure;
			});
			base.Run();
		}

		protected override RequestState RelinquishAction(TransactionalRequestJob requestJob, ReportData report)
		{
			if (this.RehomeFailure == null)
			{
				return requestJob.TimeTracker.CurrentState;
			}
			string stackTrace = this.RehomeFailure.StackTrace;
			if (CommonUtils.IsTransientException(this.RehomeFailure))
			{
				this.RehomeFailure = new RehomeRequestTransientException(this.RehomeFailure);
				report.Append(MrsStrings.ReportTransientException(CommonUtils.GetFailureType(this.RehomeFailure), 0, 0), this.RehomeFailure, ReportEntryFlags.None);
				if (requestJob.Status == RequestStatus.Completed || requestJob.Status == RequestStatus.CompletedWithWarning)
				{
					requestJob.RehomeRequest = false;
					return requestJob.TimeTracker.CurrentState;
				}
				requestJob.TimeTracker.SetTimestamp(RequestJobTimestamp.DoNotPickUntil, new DateTime?(DateTime.UtcNow + TimeSpan.FromMinutes(5.0)));
				if (CommonUtils.ExceptionIs(this.RehomeFailure, new WellKnownException[]
				{
					WellKnownException.MapiMdbOffline
				}))
				{
					return RequestState.MDBOffline;
				}
				if (CommonUtils.ExceptionIs(this.RehomeFailure, new WellKnownException[]
				{
					WellKnownException.MapiNetworkError
				}))
				{
					return RequestState.NetworkFailure;
				}
				if (CommonUtils.ExceptionIs(this.RehomeFailure, new WellKnownException[]
				{
					WellKnownException.Mapi
				}))
				{
					return RequestState.TransientFailure;
				}
				return RequestState.TransientFailure;
			}
			else
			{
				if (!(this.RehomeFailure is UnsupportedRehomeTargetVersionPermanentException))
				{
					this.RehomeFailure = new RehomeRequestPermanentException(this.RehomeFailure);
				}
				requestJob.RehomeRequest = false;
				report.Append(MrsStrings.ReportFatalException(CommonUtils.GetFailureType(this.RehomeFailure)), this.RehomeFailure, ReportEntryFlags.Fatal);
				if (requestJob.Status == RequestStatus.Completed || requestJob.Status == RequestStatus.CompletedWithWarning)
				{
					return requestJob.TimeTracker.CurrentState;
				}
				requestJob.Suspend = true;
				requestJob.Status = RequestStatus.Failed;
				requestJob.TimeTracker.SetTimestamp(RequestJobTimestamp.Failure, new DateTime?(DateTime.UtcNow));
				requestJob.FailureCode = new int?(CommonUtils.HrFromException(this.RehomeFailure));
				requestJob.FailureType = CommonUtils.GetFailureType(this.RehomeFailure);
				requestJob.FailureSide = new ExceptionSide?((requestJob.Direction == RequestDirection.Push) ? ExceptionSide.Source : ExceptionSide.Target);
				requestJob.Message = MrsStrings.MoveRequestMessageError(CommonUtils.FullExceptionMessage(this.RehomeFailure));
				if (CommonUtils.ExceptionIs(this.RehomeFailure, new WellKnownException[]
				{
					WellKnownException.MapiMdbOffline
				}))
				{
					return RequestState.FailedOther;
				}
				if (CommonUtils.ExceptionIs(this.RehomeFailure, new WellKnownException[]
				{
					WellKnownException.MapiNetworkError
				}))
				{
					return RequestState.FailedNetwork;
				}
				if (CommonUtils.ExceptionIs(this.RehomeFailure, new WellKnownException[]
				{
					WellKnownException.Mapi
				}))
				{
					return RequestState.FailedMAPI;
				}
				return RequestState.FailedOther;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.NewSystemMailbox != null)
			{
				this.NewSystemMailbox.Dispose();
				this.NewSystemMailbox = null;
			}
			base.InternalDispose(disposing);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<RehomeJob>(this);
		}
	}
}
