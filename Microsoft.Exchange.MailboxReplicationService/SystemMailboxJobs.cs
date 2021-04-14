using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class SystemMailboxJobs
	{
		protected abstract void ProcessJobs(MapiStore systemMbx, MapiTable contentsTable, RequestJobNamedPropertySet nps);

		protected abstract void PerformPickupAccounting(RequestStatus status, JobPickupRec jobPickupRec);

		protected abstract void ProcessPickupResults(JobPickupRec jobPickupRec);

		protected SystemMailboxJobs(Guid mdbGuid)
		{
			this.MdbGuid = mdbGuid;
		}

		public Guid MdbGuid { get; private set; }

		public void PickupJobs(out string failure)
		{
			string dbName = null;
			string scanFailure = null;
			failure = null;
			using (new DatabaseSettingsContext(this.MdbGuid, null).Activate())
			{
				CommonUtils.CatchKnownExceptions(delegate
				{
					DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(this.MdbGuid, null, null, FindServerFlags.None);
					dbName = databaseInformation.DatabaseName;
					if (!databaseInformation.IsOnThisServer)
					{
						scanFailure = string.Format("MDB is mounted on remote server {0}", databaseInformation.ServerFqdn);
						MRSQueue.RemoveQueue(this.MdbGuid);
						return;
					}
					MrsTracer.Service.Debug("Picking up jobs from '{0}' ({1})", new object[]
					{
						dbName,
						this.MdbGuid
					});
					using (MapiStore systemMailbox = MapiUtils.GetSystemMailbox(this.MdbGuid, false))
					{
						using (MapiFolder requestJobsFolder = RequestJobXML.GetRequestJobsFolder(systemMailbox))
						{
							using (MapiTable contentsTable = requestJobsFolder.GetContentsTable(ContentsTableFlags.DeferredErrors))
							{
								if (contentsTable.GetRowCount() > 0)
								{
									RequestJobNamedPropertySet requestJobNamedPropertySet = RequestJobNamedPropertySet.Get(systemMailbox);
									contentsTable.SetColumns(requestJobNamedPropertySet.PropTags);
									this.ProcessJobs(systemMailbox, contentsTable, requestJobNamedPropertySet);
								}
							}
						}
					}
					MrsTracer.Service.Debug("Pick up jobs from Mdb: '{0}' - complete.", new object[]
					{
						dbName
					});
				}, delegate(Exception f)
				{
					if (dbName == null)
					{
						dbName = MrsStrings.MissingDatabaseName(this.MdbGuid).ToString();
					}
					MrsTracer.Service.Debug("PickupJobs() failed for mdb '{0}'. Error: {1}", new object[]
					{
						dbName,
						CommonUtils.FullExceptionMessage(f)
					});
					scanFailure = CommonUtils.FullExceptionMessage(f, true);
					MRSService.LogEvent(MRSEventLogConstants.Tuple_UnableToProcessJobsInDatabase, new object[]
					{
						dbName,
						CommonUtils.FullExceptionMessage(f)
					});
				});
			}
			failure = scanFailure;
		}

		public void ProcessJobsInBatches(Restriction restriction, bool applyManualSort, SortOrder sort, MapiTable contentsTable, MapiStore systemMbx, Func<MoveJob, bool> stoppingCondition)
		{
			List<MoveJob> allMoveJobs = SystemMailboxJobs.GetAllMoveJobs(restriction, sort, contentsTable, this.MdbGuid, stoppingCondition);
			if (allMoveJobs != null)
			{
				if (applyManualSort)
				{
					allMoveJobs.Sort();
				}
				MrsTracer.Throttling.Debug("Sorted jobs for Mdb: {0}", new object[]
				{
					this.MdbGuid
				});
				SystemMailboxJobs.TraceJobs(allMoveJobs);
				using (List<MoveJob>.Enumerator enumerator = allMoveJobs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MoveJob moveJob = enumerator.Current;
						try
						{
							using (SettingsContextBase.ActivateContext(moveJob))
							{
								JobPickupRec pickupResult = null;
								CommonUtils.CatchKnownExceptions(delegate
								{
									pickupResult = moveJob.AttemptToPick(systemMbx);
									this.PerformPickupAccounting(moveJob.Status, pickupResult);
								}, delegate(Exception failure)
								{
									LocalizedString localizedString = CommonUtils.FullExceptionMessage(failure);
									pickupResult = new JobPickupRec(moveJob, JobPickupResult.PickupFailure, DateTime.UtcNow + MoveJob.JobPickupRetryInterval, localizedString, null);
									MrsTracer.Service.Error("Unexpected failure occurred trying to pick up MoveJob '{0}' from database '{1}', skipping it. {2}", new object[]
									{
										moveJob.RequestGuid,
										this.MdbGuid,
										localizedString
									});
									MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_UnableToProcessRequest, new object[]
									{
										moveJob.RequestGuid.ToString(),
										this.MdbGuid.ToString(),
										localizedString
									});
								});
								this.ProcessPickupResults(pickupResult);
							}
						}
						catch (Exception exception)
						{
							BaseJob.PerformCrashingFailureActions(moveJob.IdentifyingGuid, moveJob.RequestGuid, exception, RequestState.None, SyncStage.None);
							throw;
						}
					}
				}
			}
		}

		public static List<MoveJob> GetAllMoveJobs(Restriction restriction, SortOrder sort, MapiTable contentsTable, Guid mdbGuid, Func<MoveJob, bool> stoppingCondition)
		{
			List<MoveJob> list = null;
			bool flag = false;
			if (restriction != null)
			{
				contentsTable.Restrict(restriction, RestrictFlags.Batch);
			}
			if (sort != null)
			{
				contentsTable.SortTable(sort, SortTableFlags.Batch);
			}
			contentsTable.SeekRow(BookMark.Beginning, 0);
			do
			{
				PropValue[][] array = contentsTable.QueryRows(1000);
				if (array == null || array.Length <= 0)
				{
					break;
				}
				if (list == null)
				{
					list = new List<MoveJob>();
				}
				foreach (PropValue[] properties in array)
				{
					MoveJob moveJob = new MoveJob(properties, mdbGuid);
					if (stoppingCondition != null && stoppingCondition(moveJob))
					{
						flag = true;
						break;
					}
					list.Add(moveJob);
				}
			}
			while (!flag);
			return list;
		}

		private static void TraceJobs(List<MoveJob> jobs)
		{
			foreach (MoveJob moveJob in jobs)
			{
				MrsTracer.Throttling.Debug("Priority: {0}, Status: {1}, Cancel: {2}, Rehome: {3}, IdleTime: {4}, RequestGuid: {5}", new object[]
				{
					moveJob.Priority,
					moveJob.Status,
					moveJob.CancelRequest,
					moveJob.RehomeRequest,
					moveJob.IdleTime,
					moveJob.RequestGuid
				});
			}
		}
	}
}
