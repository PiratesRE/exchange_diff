using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class MailboxSyncerJobs
	{
		public static void StopScheduling()
		{
			List<JobScheduler> list = new List<JobScheduler>(MailboxSyncerJobs.schedulers.Count);
			lock (MailboxSyncerJobs.syncRoot)
			{
				foreach (JobScheduler item in MailboxSyncerJobs.schedulers.Values)
				{
					list.Add(item);
				}
				MailboxSyncerJobs.schedulers.Clear();
			}
			if (list.Count > 0)
			{
				foreach (JobScheduler jobScheduler in list)
				{
					jobScheduler.Stop();
				}
			}
			if (SystemWorkloadManager.Status != WorkloadExecutionStatus.NotInitialized)
			{
				SystemWorkloadManager.Shutdown();
				MrsAndProxyActivityLogger.Stop();
			}
		}

		public static BaseJob ConstructJob(TransactionalRequestJob requestJob)
		{
			BaseJob result;
			lock (MailboxSyncerJobs.syncRoot)
			{
				switch (requestJob.RequestType)
				{
				case MRSRequestType.Move:
					if (requestJob.RequestStyle == RequestStyle.IntraOrg)
					{
						return new LocalMoveJob();
					}
					return new RemoteMoveJob();
				case MRSRequestType.Merge:
					if (requestJob.IncrementalSyncInterval > TimeSpan.Zero || requestJob.SuspendWhenReadyToComplete || !requestJob.AllowedToFinishMove)
					{
						return new IncrementalMergeJob();
					}
					return new MergeJob();
				case MRSRequestType.MailboxImport:
					return new ImportJob();
				case MRSRequestType.MailboxExport:
					return new ExportJob();
				case MRSRequestType.MailboxRestore:
					return new RestoreJob();
				case MRSRequestType.PublicFolderMove:
					return new PublicFolderMoveJob();
				case MRSRequestType.PublicFolderMigration:
					return new PublicFolderMigrationJob();
				case MRSRequestType.Sync:
					if (requestJob.SyncProtocol == SyncProtocol.Olc)
					{
						return new OlcMigrationJob();
					}
					return new SyncJob();
				case MRSRequestType.MailboxRelocation:
					return new LocalMoveJob();
				case MRSRequestType.FolderMove:
					return new AuxFolderMoveJob();
				case MRSRequestType.PublicFolderMailboxMigration:
					return new PublicFolderMailboxMigrationJob();
				}
				result = null;
			}
			return result;
		}

		public static void CreateJob(TransactionalRequestJob requestJob, ReservationContext reservation)
		{
			BaseJob baseJob;
			lock (MailboxSyncerJobs.syncRoot)
			{
				Guid guid = (requestJob.RequestType == MRSRequestType.Move) ? requestJob.ExchangeGuid : requestJob.RequestGuid;
				baseJob = MailboxSyncerJobs.FindJob(guid, false);
				if (baseJob != null)
				{
					MrsTracer.Service.Error("Request {0} is already being worked on.", new object[]
					{
						guid
					});
					return;
				}
				baseJob = MailboxSyncerJobs.ConstructJob(requestJob);
				if (baseJob == null)
				{
					MrsTracer.Service.Error("Don't know how to process '{0}' request", new object[]
					{
						requestJob.RequestType
					});
					return;
				}
				baseJob.Initialize(requestJob);
				baseJob.Reservation = reservation;
				MailboxSyncerJobs.activeJobs.Add(guid, baseJob);
				MailboxSyncerJobs.activeJobsIsEmpty.Reset();
			}
			JobScheduler.ScheduleJob(baseJob);
		}

		public static XElement GetJobsDiagnosticInfo(MRSDiagnosticArgument arguments)
		{
			int num = arguments.GetArgumentOrDefault<int>("maxsize", int.MaxValue);
			Guid empty = Guid.Empty;
			string text = null;
			string argument = arguments.GetArgument<string>("job");
			if (!string.IsNullOrEmpty(argument) && !Guid.TryParse(argument, out empty))
			{
				text = argument;
			}
			MRSRequestType? argumentOrDefault = arguments.GetArgumentOrDefault<MRSRequestType?>("requesttype", null);
			XElement xelement = new XElement("Jobs");
			lock (MailboxSyncerJobs.syncRoot)
			{
				using (Dictionary<Guid, BaseJob>.ValueCollection.Enumerator enumerator = MailboxSyncerJobs.activeJobs.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BaseJob baseJob = enumerator.Current;
						if (num == 0)
						{
							break;
						}
						if ((empty == Guid.Empty || empty == baseJob.RequestJobGuid) && (argumentOrDefault == null || argumentOrDefault == baseJob.CachedRequestJob.RequestType) && (string.IsNullOrEmpty(text) || CommonUtils.IsValueInWildcardedList(baseJob.RequestJobStoringMDB.ToString(), text)))
						{
							xelement.Add(arguments.RunDiagnosticOperation(() => baseJob.GetJobDiagnosticInfo(arguments)));
							num--;
						}
					}
				}
			}
			return xelement;
		}

		public static BaseJob FindJob(Guid mailboxGuid, bool mustExist)
		{
			BaseJob result = null;
			lock (MailboxSyncerJobs.syncRoot)
			{
				if (!MailboxSyncerJobs.activeJobs.TryGetValue(mailboxGuid, out result) && mustExist)
				{
					MrsTracer.Service.Error("Job for mailbox {0} was not found.", new object[]
					{
						mailboxGuid
					});
					throw new MailboxNotSyncedPermanentException(mailboxGuid);
				}
			}
			return result;
		}

		public static bool ContainsJob(Guid identifyingGuid)
		{
			bool result;
			lock (MailboxSyncerJobs.syncRoot)
			{
				result = MailboxSyncerJobs.activeJobs.ContainsKey(identifyingGuid);
			}
			return result;
		}

		public static bool ProcessJob(Guid mailboxGuid, bool mustExist, MailboxSyncerJobs.JobProcessingDelegate del)
		{
			bool result;
			lock (MailboxSyncerJobs.syncRoot)
			{
				BaseJob job = null;
				if (!MailboxSyncerJobs.activeJobs.TryGetValue(mailboxGuid, out job))
				{
					if (mustExist)
					{
						MrsTracer.Service.Error("Job for mailbox {0} was not found.", new object[]
						{
							mailboxGuid
						});
						throw new MailboxNotSyncedPermanentException(mailboxGuid);
					}
					result = false;
				}
				else
				{
					del(job);
					result = true;
				}
			}
			return result;
		}

		public static void WaitForAllJobsToComplete()
		{
			if (!MailboxSyncerJobs.activeJobsIsEmpty.WaitOne(TimeSpan.FromMinutes(5.0), true))
			{
				MrsTracer.Service.Error("{0} jobs did not finish within the alloted time period.", new object[]
				{
					MailboxSyncerJobs.activeJobs.Count
				});
			}
		}

		public static void StartScheduling()
		{
			SystemWorkloadManager.Initialize(MrsAndProxyActivityLogger.Start());
			MailboxSyncerJobs.GetScheduler(WorkloadType.MailboxReplicationServiceHighPriority);
			MailboxSyncerJobs.GetScheduler(WorkloadType.MailboxReplicationServiceInteractive);
			MailboxSyncerJobs.GetScheduler(WorkloadType.MailboxReplicationServiceInternalMaintenance);
			MailboxSyncerJobs.GetScheduler(WorkloadType.MailboxReplicationService);
		}

		private static JobScheduler GetScheduler(WorkloadType workloadType)
		{
			JobScheduler jobScheduler;
			if (!MailboxSyncerJobs.schedulers.TryGetValue(workloadType, out jobScheduler))
			{
				jobScheduler = new JobScheduler(workloadType);
				jobScheduler.Start();
				jobScheduler.JobStateChanged += MailboxSyncerJobs.JobStateChangedHandler;
				MailboxSyncerJobs.schedulers.Add(workloadType, jobScheduler);
			}
			return jobScheduler;
		}

		private static void JobStateChangedHandler(object sender, JobEventArgs args)
		{
			BaseJob baseJob = args.Job as BaseJob;
			if (baseJob != null)
			{
				Guid objectGuid = baseJob.RequestJobStoringMDB.ObjectGuid;
				switch (args.State)
				{
				case JobState.Failed:
				case JobState.Finished:
					baseJob.Disconnect();
					baseJob.JobCompletedCallback();
					lock (MailboxSyncerJobs.syncRoot)
					{
						MailboxSyncerJobs.activeJobs.Remove(baseJob.GetRequestKeyGuid());
						((IDisposable)baseJob).Dispose();
						if (MailboxSyncerJobs.activeJobs.Count == 0)
						{
							MailboxSyncerJobs.activeJobsIsEmpty.Set();
						}
					}
					MRSService.Tickle(baseJob.RequestJobGuid, baseJob.RequestJobStoringMDB.ObjectGuid, MoveRequestNotification.Canceled);
					break;
				case JobState.Waiting:
					break;
				default:
					return;
				}
			}
		}

		private static readonly object syncRoot = new object();

		private static readonly Dictionary<WorkloadType, JobScheduler> schedulers = new Dictionary<WorkloadType, JobScheduler>();

		private static readonly Dictionary<Guid, BaseJob> activeJobs = new Dictionary<Guid, BaseJob>();

		private static readonly ManualResetEvent activeJobsIsEmpty = new ManualResetEvent(true);

		public delegate void JobProcessingDelegate(BaseJob job);
	}
}
