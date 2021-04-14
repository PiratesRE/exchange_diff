using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MoveJob : IComparable<MoveJob>, ISettingsContextProvider
	{
		public MoveJob(PropValue[] properties, Guid requestQueueGuid)
		{
			this.JobType = MapiUtils.GetValue<MRSJobType>(properties[9], MRSJobType.Unknown);
			if (!RequestJobXML.IsKnownJobType(this.JobType))
			{
				MrsTracer.Service.Debug("Skipping unknown jobType {0}", new object[]
				{
					(int)this.JobType
				});
				return;
			}
			this.RequestGuid = MapiUtils.GetValue<Guid>(properties[26], Guid.Empty);
			this.ExchangeGuid = MapiUtils.GetValue<Guid>(properties[5], Guid.Empty);
			this.ArchiveGuid = MapiUtils.GetValue<Guid>(properties[6], Guid.Empty);
			this.CancelRequest = MapiUtils.GetValue<bool>(properties[4], false);
			this.MrsServerName = MapiUtils.GetValue<string>(properties[2], null);
			this.Status = MapiUtils.GetValue<RequestStatus>(properties[0], RequestStatus.None);
			this.JobState = MapiUtils.GetValue<JobProcessingState>(properties[1], JobProcessingState.NotReady);
			this.LastUpdateTimeStamp = MapiUtils.GetValue<DateTime>(properties[7], DateTime.MinValue);
			this.Flags = MapiUtils.GetValue<RequestFlags>(properties[10], RequestFlags.None);
			this.SourceDatabaseGuid = MapiUtils.GetValue<Guid>(properties[11], Guid.Empty);
			this.TargetDatabaseGuid = MapiUtils.GetValue<Guid>(properties[12], Guid.Empty);
			this.SourceArchiveDatabaseGuid = MapiUtils.GetValue<Guid>(properties[15], Guid.Empty);
			this.TargetArchiveDatabaseGuid = MapiUtils.GetValue<Guid>(properties[16], Guid.Empty);
			this.Priority = MapiUtils.GetValue<int>(properties[17], -1);
			this.DoNotPickUntilTimestamp = MapiUtils.GetValue<DateTime>(properties[13], DateTime.MinValue);
			this.RequestType = MapiUtils.GetValue<MRSRequestType>(properties[14], MRSRequestType.Move);
			this.MessageID = MapiUtils.GetValue<byte[]>(properties[27], null);
			this.SourceExchangeGuid = MapiUtils.GetValue<Guid>(properties[18], Guid.Empty);
			this.TargetExchangeGuid = MapiUtils.GetValue<Guid>(properties[19], Guid.Empty);
			this.RehomeRequest = MapiUtils.GetValue<bool>(properties[20], false);
			this.InternalFlags = MapiUtils.GetValue<RequestJobInternalFlags>(properties[21], RequestJobInternalFlags.None);
			this.PoisonCount = MapiUtils.GetValue<int>(properties[23], 0);
			this.FailureType = MapiUtils.GetValue<string>(properties[24], null);
			this.WorkloadType = MapiUtils.GetValue<RequestWorkloadType>(properties[25], RequestWorkloadType.None);
			byte[] value = MapiUtils.GetValue<byte[]>(properties[22], null);
			this.PartitionHint = ((value != null && value.Length > 0) ? TenantPartitionHint.FromPersistablePartitionHint(value) : null);
			this.RequestQueueGuid = requestQueueGuid;
			this.IsActiveOnThisMRSInstance = MRSService.JobIsActive(this.RequestGuid);
			this.isInteractive = MoveJob.IsInteractive(this.RequestType, this.WorkloadType);
		}

		public static bool IsInteractive(MRSRequestType requestType, RequestWorkloadType workloadType)
		{
			return ConfigBase<MRSConfigSchema>.GetConfig<bool>("AllAggregationSyncJobsInteractive") && requestType == MRSRequestType.Sync && workloadType == RequestWorkloadType.SyncAggregation;
		}

		public static bool CacheJobQueues
		{
			get
			{
				return MoveJob.cacheJobQueues.Value;
			}
		}

		internal Guid IdentifyingGuid
		{
			get
			{
				if (this.RequestType != MRSRequestType.Move)
				{
					return this.RequestGuid;
				}
				return this.ExchangeGuid;
			}
		}

		public Guid RequestGuid { get; private set; }

		public Guid ExchangeGuid { get; private set; }

		public Guid ArchiveGuid { get; private set; }

		public Guid SourceDatabaseGuid { get; private set; }

		public Guid TargetDatabaseGuid { get; private set; }

		public Guid SourceArchiveDatabaseGuid { get; private set; }

		public Guid TargetArchiveDatabaseGuid { get; private set; }

		public int Priority { get; private set; }

		public Guid RequestQueueGuid { get; private set; }

		public RequestFlags Flags { get; private set; }

		public RequestStatus Status { get; private set; }

		public Guid SourceExchangeGuid { get; private set; }

		public Guid TargetExchangeGuid { get; private set; }

		public bool RehomeRequest { get; private set; }

		public JobProcessingState JobState { get; private set; }

		public DateTime LastUpdateTimeStamp { get; private set; }

		public TimeSpan IdleTime
		{
			get
			{
				return (DateTime)ExDateTime.UtcNow - this.LastUpdateTimeStamp;
			}
		}

		public MRSJobType JobType { get; private set; }

		public MRSRequestType RequestType { get; private set; }

		public RequestWorkloadType WorkloadType { get; private set; }

		public string MrsServerName { get; private set; }

		public bool CancelRequest { get; private set; }

		public DateTime DoNotPickUntilTimestamp { get; private set; }

		public bool IsActiveOnThisMRSInstance { get; private set; }

		public byte[] MessageID { get; private set; }

		public RequestJobInternalFlags InternalFlags { get; private set; }

		public TenantPartitionHint PartitionHint { get; private set; }

		public int PoisonCount { get; private set; }

		public string FailureType { get; private set; }

		public bool Suspend
		{
			get
			{
				return (this.Flags & RequestFlags.Suspend) != RequestFlags.None;
			}
		}

		public bool SourceIsLocal
		{
			get
			{
				return (this.Flags & RequestFlags.IntraOrg) != RequestFlags.None || (this.Flags & RequestFlags.Push) != RequestFlags.None;
			}
		}

		public bool TargetIsLocal
		{
			get
			{
				return (this.Flags & RequestFlags.IntraOrg) != RequestFlags.None || (this.Flags & RequestFlags.Pull) != RequestFlags.None;
			}
		}

		public bool ToBeCanceled
		{
			get
			{
				return this.CancelRequest && !this.RehomeRequest;
			}
		}

		public bool ToBeContinued
		{
			get
			{
				return !this.Suspend && !this.CancelRequest && !this.RehomeRequest && (this.Status == RequestStatus.InProgress || this.Status == RequestStatus.CompletionInProgress);
			}
		}

		public bool ToBeStartedFromScratch
		{
			get
			{
				return !this.Suspend && !this.CancelRequest && !this.RehomeRequest && this.Status == RequestStatus.Queued;
			}
		}

		public bool IsCrossOrg
		{
			get
			{
				return (this.Flags & RequestFlags.CrossOrg) != RequestFlags.None;
			}
		}

		public bool DoNotPickUntilHasElapsed
		{
			get
			{
				return DateTime.UtcNow >= this.DoNotPickUntilTimestamp || (MoveJob.CacheJobQueues && this.DoNotPickUntilTimestamp < MRSService.NextFullScanTime);
			}
		}

		public bool IsLightRequest
		{
			get
			{
				return this.RehomeRequest || (this.Suspend && this.Status != RequestStatus.Failed && this.Status != RequestStatus.Suspended && this.Status != RequestStatus.AutoSuspended && this.Status != RequestStatus.Completed && this.Status != RequestStatus.CompletedWithWarning) || (!this.Suspend && (this.Status == RequestStatus.Failed || this.Status == RequestStatus.Suspended || this.Status == RequestStatus.AutoSuspended)) || this.Status == RequestStatus.Completed || QuarantinedJobs.Contains(this.IdentifyingGuid);
			}
		}

		public static void ReserveLocalForestResources(ReservationContext reservation, WorkloadType workloadType, MRSRequestType requestType, RequestFlags requestFlags, Guid archiveGuid, Guid exchangeGuid, Guid sourceExchangeGuid, Guid targetExchangeGuid, TenantPartitionHint partitionHint, ADObjectId sourceDatabase, ADObjectId sourceArchiveDatabase, ADObjectId targetDatabase, ADObjectId targetArchiveDatabase, Guid sourceDatabaseGuid, Guid sourceArchiveDatabaseGuid, Guid targetDatabaseGuid, Guid targetArchiveDatabaseGuid)
		{
			ReservationFlags reservationFlags;
			Guid guid;
			Guid guid2;
			if (requestType == MRSRequestType.Move || requestType == MRSRequestType.MailboxRelocation)
			{
				reservationFlags = ReservationFlags.Move;
				if (requestFlags.HasFlag(RequestFlags.MoveOnlyArchiveMailbox) && archiveGuid != Guid.Empty)
				{
					guid = archiveGuid;
				}
				else
				{
					guid = exchangeGuid;
				}
				guid2 = guid;
			}
			else
			{
				reservationFlags = ReservationFlags.Merge;
				guid = sourceExchangeGuid;
				guid2 = targetExchangeGuid;
			}
			if (workloadType != Microsoft.Exchange.WorkloadManagement.WorkloadType.MailboxReplicationServiceHighPriority)
			{
				switch (workloadType)
				{
				case Microsoft.Exchange.WorkloadManagement.WorkloadType.MailboxReplicationServiceInternalMaintenance:
					reservationFlags |= ReservationFlags.InternalMaintenance;
					break;
				case Microsoft.Exchange.WorkloadManagement.WorkloadType.MailboxReplicationServiceInteractive:
					reservationFlags |= ReservationFlags.Interactive;
					break;
				}
			}
			else
			{
				reservationFlags |= ReservationFlags.HighPriority;
			}
			reservation.ReserveResource((guid2 == Guid.Empty) ? guid : guid2, partitionHint, MRSResource.Id, reservationFlags);
			if (targetDatabaseGuid != Guid.Empty)
			{
				reservation.ReserveResource(guid2, partitionHint, targetDatabase, reservationFlags | ReservationFlags.Write);
			}
			if (targetArchiveDatabaseGuid != Guid.Empty && targetArchiveDatabaseGuid != targetDatabaseGuid && archiveGuid != Guid.Empty)
			{
				reservation.ReserveResource(archiveGuid, partitionHint, targetArchiveDatabase, reservationFlags | ReservationFlags.Write | ReservationFlags.Archive);
			}
			if (sourceDatabaseGuid != Guid.Empty)
			{
				reservation.ReserveResource(guid, partitionHint, sourceDatabase, reservationFlags | ReservationFlags.Read);
			}
			if (sourceArchiveDatabaseGuid != Guid.Empty && sourceArchiveDatabaseGuid != sourceDatabaseGuid && archiveGuid != Guid.Empty)
			{
				reservation.ReserveResource(archiveGuid, partitionHint, sourceArchiveDatabase, reservationFlags | ReservationFlags.Read | ReservationFlags.Archive);
			}
		}

		public static void AddRemoteHostInProxyBackoff(string remoteHostName, DateTime nextCheckTime)
		{
			if (TestIntegration.Instance.DisableRemoteHostNameBlacklisting)
			{
				return;
			}
			if (string.IsNullOrEmpty(remoteHostName))
			{
				return;
			}
			remoteHostName = remoteHostName.ToLowerInvariant();
			DateTime dateTime;
			if (MoveJob.RemoteHostsInProxyBackoff.TryGetValue(remoteHostName, out dateTime))
			{
				return;
			}
			MoveJob.RemoteHostsInProxyBackoff.InsertAbsolute(remoteHostName, nextCheckTime, nextCheckTime, null);
			MrsTracer.Service.Debug("RemoteHostName {0} is added to proxy backoff blacklist until {1}.", new object[]
			{
				remoteHostName,
				nextCheckTime.ToLocalTime()
			});
		}

		public JobPickupRec AttemptToPick(MapiStore systemMailbox)
		{
			if (!RequestJobXML.IsKnownRequestType(this.RequestType))
			{
				return new JobPickupRec(this, JobPickupResult.UnknownJobType, DateTime.MaxValue, MrsStrings.PickupStatusRequestTypeNotSupported(this.RequestType.ToString()), null);
			}
			if (!RequestJobXML.IsKnownJobType(this.JobType))
			{
				return new JobPickupRec(this, JobPickupResult.UnknownJobType, DateTime.MaxValue, MrsStrings.PickupStatusJobTypeNotSupported(this.JobType.ToString()), null);
			}
			if (this.PoisonCount >= ConfigBase<MRSConfigSchema>.GetConfig<int>("HardPoisonLimit"))
			{
				return new JobPickupRec(this, JobPickupResult.PoisonedJob, DateTime.MaxValue, MrsStrings.PickupStatusJobPoisoned(this.PoisonCount), null);
			}
			if (this.IsActiveOnThisMRSInstance)
			{
				return new JobPickupRec(this, JobPickupResult.JobAlreadyActive, DateTime.MaxValue, LocalizedString.Empty, null);
			}
			DateTime utcNow = DateTime.UtcNow;
			if (this.Status == RequestStatus.Completed && (!this.DoNotPickUntilHasElapsed || this.DoNotPickUntilTimestamp == DateTime.MinValue) && !this.RehomeRequest)
			{
				return new JobPickupRec(this, JobPickupResult.CompletedJobSkipped, (this.DoNotPickUntilTimestamp == DateTime.MinValue) ? DateTime.MaxValue : this.DoNotPickUntilTimestamp, LocalizedString.Empty, null);
			}
			if (this.CancelRequest && !this.DoNotPickUntilHasElapsed)
			{
				return new JobPickupRec(this, JobPickupResult.PostponeCancel, this.DoNotPickUntilTimestamp, LocalizedString.Empty, null);
			}
			if (!this.isInteractive && !this.IsLightRequest && !this.DoNotPickUntilHasElapsed)
			{
				MrsTracer.Service.Debug("Ignoring MoveJob '{0}' on queue '{1}' having DoNotPickUntilTimestamp of {2}.", new object[]
				{
					this.RequestGuid,
					this.RequestQueueGuid,
					this.DoNotPickUntilTimestamp.ToLocalTime()
				});
				return new JobPickupRec(this, JobPickupResult.JobIsPostponed, this.DoNotPickUntilTimestamp, LocalizedString.Empty, null);
			}
			if (this.InternalFlags.HasFlag(RequestJobInternalFlags.ExecutedByTransportSync))
			{
				MrsTracer.Service.Debug("Ignoring MoveJob '{0}' since Tranport Sync Owns Execution of the job.", new object[]
				{
					this.RequestGuid
				});
				return new JobPickupRec(this, JobPickupResult.JobOwnedByTransportSync, DateTime.MaxValue, LocalizedString.Empty, null);
			}
			JobPickupRec result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				using (RequestJobProvider requestJobProvider = new RequestJobProvider(this.RequestQueueGuid, systemMailbox))
				{
					using (TransactionalRequestJob transactionalRequestJob = (TransactionalRequestJob)requestJobProvider.Read<TransactionalRequestJob>(new RequestJobObjectId(this.RequestGuid, this.RequestQueueGuid, this.MessageID)))
					{
						if (transactionalRequestJob == null)
						{
							result = new JobPickupRec(this, JobPickupResult.InvalidJob, DateTime.MaxValue, MrsStrings.PickupStatusCorruptJob, null);
						}
						else if (!transactionalRequestJob.IsSupported())
						{
							result = new JobPickupRec(this, JobPickupResult.UnknownJobType, DateTime.MaxValue, MrsStrings.PickupStatusSubTypeNotSupported(transactionalRequestJob.RequestType.ToString()), null);
						}
						else if (transactionalRequestJob.ValidationResult != RequestJobBase.ValidationResultEnum.Valid)
						{
							this.ProcessInvalidJob(transactionalRequestJob, requestJobProvider);
							result = new JobPickupRec(this, JobPickupResult.InvalidJob, DateTime.MaxValue, MrsStrings.PickupStatusInvalidJob(transactionalRequestJob.ValidationResult.ToString(), transactionalRequestJob.ValidationMessage), null);
						}
						else if (transactionalRequestJob.Status == RequestStatus.Completed && !transactionalRequestJob.RehomeRequest)
						{
							this.CleanupCompletedJob(transactionalRequestJob, requestJobProvider);
							result = new JobPickupRec(this, JobPickupResult.CompletedJobCleanedUp, DateTime.MaxValue, MrsStrings.PickupStatusCompletedJob, null);
						}
						else if (!transactionalRequestJob.ShouldProcessJob())
						{
							result = new JobPickupRec(this, JobPickupResult.DisabledJobPickup, DateTime.MaxValue, MrsStrings.PickupStatusDisabled, null);
						}
						else
						{
							ReservationContext reservationContext = null;
							if (!this.IsLightRequest && !MoveJob.CacheJobQueues)
							{
								reservationContext = new ReservationContext();
								disposeGuard.Add<ReservationContext>(reservationContext);
								try
								{
									this.ReserveLocalForestResources(reservationContext, transactionalRequestJob);
								}
								catch (LocalizedException ex)
								{
									if (CommonUtils.ExceptionIs(ex, new WellKnownException[]
									{
										WellKnownException.ResourceReservation
									}))
									{
										return new JobPickupRec(this, JobPickupResult.ReservationFailure, utcNow + MoveJob.JobPickupRetryInterval, MrsStrings.PickupStatusReservationFailure(CommonUtils.FullExceptionMessage(ex)), ex as ResourceReservationException);
									}
									throw;
								}
							}
							if (!TestIntegration.Instance.DisableRemoteHostNameBlacklisting && transactionalRequestJob.RequestType == MRSRequestType.Move && (transactionalRequestJob.Flags & RequestFlags.CrossOrg) != RequestFlags.None && (transactionalRequestJob.Flags & RequestFlags.RemoteLegacy) == RequestFlags.None && !string.IsNullOrEmpty(transactionalRequestJob.RemoteHostName))
							{
								string key = transactionalRequestJob.RemoteHostName.ToLowerInvariant();
								DateTime nextRecommendedPickup;
								if (MoveJob.RemoteHostsInProxyBackoff.TryGetValue(key, out nextRecommendedPickup))
								{
									return new JobPickupRec(this, JobPickupResult.ProxyBackoff, nextRecommendedPickup, MrsStrings.PickupStatusProxyBackoff(transactionalRequestJob.RemoteHostName), null);
								}
							}
							MrsTracer.Service.Debug("Attempting to take over MoveJob '{0}' on queue '{1}', priority={2}", new object[]
							{
								transactionalRequestJob,
								this.RequestQueueGuid,
								transactionalRequestJob.Priority
							});
							transactionalRequestJob.RequestJobState = JobProcessingState.InProgress;
							transactionalRequestJob.MRSServerName = CommonUtils.LocalComputerName;
							if (!this.IsLightRequest)
							{
								transactionalRequestJob.PoisonCount++;
								transactionalRequestJob.LastPickupTime = new DateTime?(DateTime.UtcNow);
							}
							if (!transactionalRequestJob.Suspend && !transactionalRequestJob.RehomeRequest && transactionalRequestJob.Status != RequestStatus.Suspended && transactionalRequestJob.Status != RequestStatus.AutoSuspended && transactionalRequestJob.Status != RequestStatus.Failed && transactionalRequestJob.Status != RequestStatus.Completed && transactionalRequestJob.Status != RequestStatus.CompletedWithWarning)
							{
								transactionalRequestJob.Status = ((reservationContext == null) ? RequestStatus.Queued : RequestStatus.InProgress);
							}
							requestJobProvider.Save(transactionalRequestJob);
							this.Status = transactionalRequestJob.Status;
							JobPickupRec jobPickupRec;
							if (this.IsLightRequest)
							{
								jobPickupRec = new JobPickupRec(this, JobPickupResult.JobPickedUp, DateTime.MaxValue, MrsStrings.PickupStatusLightJob(transactionalRequestJob.Suspend, transactionalRequestJob.RehomeRequest, transactionalRequestJob.Priority.ToString()), null);
								MoveJob.PerformLightJobAction(requestJobProvider.SystemMailbox, RequestJobProvider.CreateRequestStatistics(transactionalRequestJob));
							}
							else
							{
								MailboxSyncerJobs.CreateJob(transactionalRequestJob, reservationContext);
								jobPickupRec = new JobPickupRec(this, JobPickupResult.JobPickedUp, DateTime.MaxValue, MrsStrings.PickupStatusCreateJob(transactionalRequestJob.SyncStage.ToString(), transactionalRequestJob.CancelRequest, transactionalRequestJob.Priority.ToString()), null);
							}
							disposeGuard.Success();
							result = jobPickupRec;
						}
					}
				}
			}
			return result;
		}

		public int CompareTo(MoveJob other)
		{
			if (object.ReferenceEquals(this, other) || this.RequestGuid.Equals(other.RequestGuid))
			{
				return 0;
			}
			if (this.Priority > other.Priority)
			{
				return -1;
			}
			if (this.Priority < other.Priority)
			{
				return 1;
			}
			if (this.ToBeCanceled && !other.ToBeCanceled)
			{
				return -1;
			}
			if (!this.ToBeCanceled && other.ToBeCanceled)
			{
				return 1;
			}
			if (this.ToBeCanceled && other.ToBeCanceled)
			{
				return this.CompareLastUpdateTimestamps(this, other);
			}
			if (this.ToBeContinued && !other.ToBeContinued)
			{
				return -1;
			}
			if (!this.ToBeContinued && other.ToBeContinued)
			{
				return 1;
			}
			if (this.ToBeContinued && other.ToBeContinued)
			{
				return this.CompareLastUpdateTimestamps(this, other);
			}
			if (this.ToBeStartedFromScratch && !other.ToBeStartedFromScratch)
			{
				return -1;
			}
			if (!this.ToBeStartedFromScratch && other.ToBeStartedFromScratch)
			{
				return 1;
			}
			if (this.ToBeStartedFromScratch && other.ToBeStartedFromScratch)
			{
				return this.CompareLastUpdateTimestamps(this, other);
			}
			int num = this.CompareLastUpdateTimestamps(this, other);
			if (num != 0)
			{
				return num;
			}
			return this.RequestGuid.CompareTo(other.RequestGuid);
		}

		private static void PerformLightJobAction(MapiStore systemMailbox, RequestStatisticsBase requestJobStats)
		{
			CommonUtils.CatchKnownExceptions(delegate
			{
				bool flag = false;
				LightJobBase lightJobBase;
				if (QuarantinedJobs.Contains(requestJobStats.IdentifyingGuid))
				{
					lightJobBase = new QuarantineJob(requestJobStats.IdentifyingGuid, requestJobStats.WorkItemQueueMdb.ObjectGuid, systemMailbox, requestJobStats.MessageId);
				}
				else if (requestJobStats.ShouldRehomeRequest)
				{
					lightJobBase = new RehomeJob(requestJobStats.IdentifyingGuid, requestJobStats.RequestQueue, requestJobStats.OptimalRequestQueue, systemMailbox, requestJobStats.MessageId);
				}
				else if (requestJobStats.ShouldClearRehomeRequest)
				{
					lightJobBase = new ClearRehomeJob(requestJobStats.IdentifyingGuid, requestJobStats.WorkItemQueueMdb.ObjectGuid, systemMailbox, requestJobStats.MessageId);
				}
				else if (requestJobStats.ShouldSuspendRequest)
				{
					lightJobBase = new SuspendJob(requestJobStats.IdentifyingGuid, requestJobStats.WorkItemQueueMdb.ObjectGuid, systemMailbox, requestJobStats.MessageId);
				}
				else
				{
					lightJobBase = new ResumeJob(requestJobStats.IdentifyingGuid, requestJobStats.WorkItemQueueMdb.ObjectGuid, systemMailbox, requestJobStats.MessageId);
					flag = true;
				}
				using (lightJobBase)
				{
					lightJobBase.Run();
					if (flag)
					{
						MRSService.Tickle(requestJobStats.IdentifyingGuid, requestJobStats.WorkItemQueueMdb.ObjectGuid, MoveRequestNotification.Created);
					}
				}
			}, delegate(Exception failure)
			{
				LocalizedString localizedString = CommonUtils.FullExceptionMessage(failure);
				MrsTracer.Service.Debug("Unexpected failure occurred trying to perform a light pipe action on MoveJob '{0}' from queue '{1}', skipping it. {2}", new object[]
				{
					requestJobStats.RequestGuid,
					requestJobStats.RequestQueue,
					localizedString
				});
				MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_UnableToProcessRequest, new object[]
				{
					requestJobStats.RequestGuid.ToString(),
					requestJobStats.WorkItemQueueMdb.ObjectGuid.ToString(),
					localizedString
				});
			});
		}

		private void ReserveLocalForestResources(ReservationContext reservation, TransactionalRequestJob requestJob)
		{
			MoveJob.ReserveLocalForestResources(reservation, CommonUtils.ComputeWlmWorkloadType(this.Priority, this.isInteractive, ConfigBase<MRSConfigSchema>.GetConfig<WorkloadType>("WlmWorkloadType")), this.RequestType, this.Flags, this.ArchiveGuid, this.ExchangeGuid, this.SourceExchangeGuid, this.TargetExchangeGuid, this.PartitionHint, requestJob.SourceDatabase, requestJob.SourceArchiveDatabase, requestJob.TargetDatabase, requestJob.TargetArchiveDatabase, this.SourceDatabaseGuid, this.SourceArchiveDatabaseGuid, this.TargetDatabaseGuid, this.TargetArchiveDatabaseGuid);
		}

		private void ProcessInvalidJob(TransactionalRequestJob requestJob, RequestJobProvider rjProvider)
		{
			MrsTracer.Service.Warning("MoveJob '{0}' on queue '{1}' failed validation: {2}.", new object[]
			{
				requestJob,
				this.RequestQueueGuid,
				requestJob.ValidationMessage
			});
			if (requestJob.IdleTime < MoveJob.MaxADReplicationWaitTime)
			{
				MrsTracer.Service.Warning("MoveJob '{0}' on queue '{1}' appears invalid.  Waiting for {2} for AD Replication.  Already have waited {3}...", new object[]
				{
					requestJob,
					this.RequestQueueGuid,
					MoveJob.MaxADReplicationWaitTime,
					requestJob.IdleTime
				});
				return;
			}
			if (requestJob.ValidationResult == RequestJobBase.ValidationResultEnum.Orphaned)
			{
				MrsTracer.Service.Warning("MoveJob '{0}' on queue '{1}' is orphaned, removing it.", new object[]
				{
					requestJob,
					this.RequestQueueGuid
				});
				rjProvider.Delete(requestJob);
				CommonUtils.CatchKnownExceptions(delegate
				{
					ReportData reportData2 = new ReportData(requestJob.IdentifyingGuid, requestJob.ReportVersion);
					reportData2.Delete(rjProvider.SystemMailbox);
				}, null);
				requestJob.RemoveAsyncNotification();
				MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_RemovedOrphanedMoveRequest, new object[]
				{
					this.RequestQueueGuid.ToString(),
					this.RequestGuid.ToString(),
					requestJob.ToString(),
					requestJob.ValidationMessage
				});
				return;
			}
			ReportData reportData = new ReportData(requestJob.IdentifyingGuid, requestJob.ReportVersion);
			reportData.Append(MrsStrings.ReportFailingInvalidMoveRequest(requestJob.ValidationMessage));
			reportData.Flush(rjProvider.SystemMailbox);
			requestJob.Status = RequestStatus.Failed;
			requestJob.FailureCode = new int?(-2147024809);
			requestJob.FailureType = "InvalidRequest";
			requestJob.FailureSide = new ExceptionSide?(ExceptionSide.None);
			requestJob.Message = MrsStrings.MoveRequestMessageError(MrsStrings.MoveRequestDataIsCorrupt(requestJob.ValidationMessage));
			requestJob.TimeTracker.SetTimestamp(RequestJobTimestamp.Failure, new DateTime?(DateTime.UtcNow));
			requestJob.TimeTracker.CurrentState = RequestState.Failed;
			rjProvider.Save(requestJob);
			requestJob.UpdateAsyncNotification(reportData);
			MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_FailedInvalidRequest, new object[]
			{
				this.RequestQueueGuid.ToString(),
				this.RequestGuid.ToString(),
				requestJob.ToString(),
				requestJob.ValidationMessage
			});
		}

		private void CleanupCompletedJob(TransactionalRequestJob requestJob, RequestJobProvider rjProvider)
		{
			DateTime t = requestJob.TimeTracker.GetTimestamp(RequestJobTimestamp.DoNotPickUntil) ?? DateTime.MaxValue;
			if (DateTime.UtcNow < t)
			{
				return;
			}
			MrsTracer.Service.Debug("Cleaning up expired completed job '{0}' on queue '{1}'", new object[]
			{
				requestJob,
				this.RequestQueueGuid
			});
			rjProvider.Delete(requestJob);
			CommonUtils.CatchKnownExceptions(delegate
			{
				ReportData reportData = new ReportData(requestJob.IdentifyingGuid, requestJob.ReportVersion);
				reportData.Delete(rjProvider.SystemMailbox);
			}, null);
			CommonUtils.CatchKnownExceptions(delegate
			{
				rjProvider.DeleteIndexEntries(requestJob);
			}, null);
			requestJob.RemoveAsyncNotification();
			MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_RemovedCompletedRequest, new object[]
			{
				requestJob.ToString(),
				this.RequestGuid.ToString(),
				requestJob.WorkItemQueueMdbName
			});
		}

		private int CompareLastUpdateTimestamps(MoveJob job1, MoveJob job2)
		{
			if (job1.IdleTime > job2.IdleTime)
			{
				return -1;
			}
			if (job1.IdleTime < job2.IdleTime)
			{
				return 1;
			}
			return 0;
		}

		ISettingsContext ISettingsContextProvider.GetSettingsContext()
		{
			Guid guid = this.ExchangeGuid;
			if (guid == Guid.Empty)
			{
				guid = ((this.TargetExchangeGuid != Guid.Empty) ? this.TargetExchangeGuid : this.SourceExchangeGuid);
			}
			return CommonUtils.CreateConfigContext(guid, this.RequestQueueGuid, null, this.WorkloadType, this.RequestType, SyncProtocol.None);
		}

		public static readonly TimeSpan JobPickupRetryInterval = TimeSpan.FromMinutes(1.0);

		private static readonly TimeSpan MaxADReplicationWaitTime = TimeSpan.FromDays(10.0);

		private static readonly TimeoutCache<string, DateTime> RemoteHostsInProxyBackoff = new TimeoutCache<string, DateTime>(16, 1024, false);

		private static readonly Lazy<bool> cacheJobQueues = new Lazy<bool>(() => ConfigBase<MRSConfigSchema>.GetConfig<bool>("CacheJobQueues"));

		private readonly bool isInteractive;
	}
}
