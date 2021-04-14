using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MRSQueue
	{
		public MRSQueue(Guid mdbGuid)
		{
			this.MdbGuid = mdbGuid;
			this.MdbDiscoveryTimestamp = DateTime.UtcNow;
		}

		public Guid MdbGuid { get; private set; }

		public DateTime MdbDiscoveryTimestamp { get; private set; }

		public DateTime LastScanTimestamp { get; private set; }

		public TimeSpan LastScanDuration { get; private set; }

		public DateTime NextRecommendedScan { get; private set; }

		public DateTime NextRecommendedLightScan { get; private set; }

		public DateTime LastJobPickup { get; private set; }

		public DateTime LastInteractiveJobPickup { get; private set; }

		public int QueuedJobsCount { get; private set; }

		public int InProgressJobsCount { get; private set; }

		public List<JobPickupRec> LastScanResults { get; private set; }

		public string LastScanFailure { get; private set; }

		public DateTime LastActiveJobFinishTime { get; set; }

		public Guid LastActiveJobFinished { get; set; }

		public static MRSQueue Get(Guid mdbGuid)
		{
			MRSQueue mrsqueue;
			lock (MRSQueue.locker)
			{
				if (!MRSQueue.queues.TryGetValue(mdbGuid, out mrsqueue))
				{
					mrsqueue = new MRSQueue(mdbGuid);
					MRSQueue.queues.TryInsertSliding(mdbGuid, mrsqueue, MRSQueue.CacheTimeout);
				}
			}
			return mrsqueue;
		}

		public static XElement GetDiagnosticInfo(MRSDiagnosticArgument arguments)
		{
			XElement xelement = new XElement("Queues");
			lock (MRSQueue.locker)
			{
				using (List<MRSQueue>.Enumerator enumerator = MRSQueue.queues.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MRSQueue queue = enumerator.Current;
						xelement.Add(arguments.RunDiagnosticOperation(() => queue.GetQueueDiagnosticInfo(arguments)));
					}
				}
			}
			return xelement;
		}

		public static List<Guid> GetQueuesToScan(MRSQueue.ScanType scanType)
		{
			List<Guid> list = null;
			DateTime utcNow = DateTime.UtcNow;
			lock (MRSQueue.locker)
			{
				foreach (MRSQueue mrsqueue in MRSQueue.queues.Values)
				{
					DateTime t = (scanType == MRSQueue.ScanType.Light) ? mrsqueue.NextRecommendedLightScan : mrsqueue.NextRecommendedScan;
					if (t <= utcNow)
					{
						if (list == null)
						{
							list = new List<Guid>();
						}
						list.Add(mrsqueue.MdbGuid);
					}
				}
			}
			if (list != null)
			{
				list = CommonUtils.RandomizeSequence<Guid>(list);
			}
			return list;
		}

		public static List<Guid> GetQueues()
		{
			List<Guid> list = new List<Guid>(MRSQueue.queues.Count);
			lock (MRSQueue.locker)
			{
				foreach (MRSQueue mrsqueue in MRSQueue.queues.Values)
				{
					list.Add(mrsqueue.MdbGuid);
				}
			}
			return list;
		}

		public static LocalizedString GetJobPickupFailureMessageForRequest(Guid requestGuid)
		{
			LocalizedString result = LocalizedString.Empty;
			List<Guid> list = MRSQueue.GetQueues();
			if (!list.IsNullOrEmpty())
			{
				foreach (Guid mdbGuid in list)
				{
					MRSQueue mrsqueue = MRSQueue.Get(mdbGuid);
					List<JobPickupRec> lastScanResults = mrsqueue.LastScanResults;
					if (!lastScanResults.IsNullOrEmpty())
					{
						JobPickupRec jobPickupRec = lastScanResults.Find((JobPickupRec x) => x.RequestGuid == requestGuid);
						if (jobPickupRec != null)
						{
							result = jobPickupRec.GetPickupFailureMessage();
							break;
						}
					}
				}
			}
			return result;
		}

		public static void RemoveQueue(Guid mdbGuid)
		{
			lock (MRSQueue.locker)
			{
				MRSQueue.queues.Remove(mdbGuid);
			}
		}

		public void Tickle(MRSQueue.ScanType scanType)
		{
			switch (scanType)
			{
			case MRSQueue.ScanType.Light:
				this.NextRecommendedLightScan = DateTime.UtcNow;
				return;
			case MRSQueue.ScanType.Heavy:
				this.NextRecommendedScan = DateTime.UtcNow;
				return;
			default:
				return;
			}
		}

		public void PickupLightJobs()
		{
			DateTime utcNow = DateTime.UtcNow;
			SystemMailboxLightJobs systemMailboxLightJobs = new SystemMailboxLightJobs(this.MdbGuid);
			systemMailboxLightJobs.PickupJobs();
			if (systemMailboxLightJobs.ScanFailure != null)
			{
				this.NextRecommendedLightScan = DateTime.UtcNow + MRSQueue.ScanRetryInterval;
				return;
			}
			if (this.NextRecommendedLightScan <= utcNow)
			{
				this.NextRecommendedLightScan = systemMailboxLightJobs.RecommendedNextScan;
			}
		}

		public void PickupHeavyJobs(out bool mdbIsUnreachable)
		{
			SystemMailboxHeavyJobs systemMailboxHeavyJobs = new SystemMailboxHeavyJobs(this.MdbGuid);
			this.LastScanTimestamp = DateTime.UtcNow;
			MailboxReplicationServicePerMdbPerformanceCountersInstance perfCounter = MDBPerfCounterHelperCollection.GetMDBHelper(this.MdbGuid, true).PerfCounter;
			perfCounter.LastScanTime.RawValue = CommonUtils.TimestampToPerfcounterLong(this.LastScanTimestamp);
			systemMailboxHeavyJobs.PickupJobs();
			mdbIsUnreachable = (systemMailboxHeavyJobs.ScanFailure != null);
			this.LastScanDuration = DateTime.UtcNow - this.LastScanTimestamp;
			perfCounter.LastScanDuration.RawValue = (long)this.LastScanDuration.TotalMilliseconds;
			perfCounter.LastScanFailure.RawValue = (mdbIsUnreachable ? 1L : 0L);
			this.LastScanFailure = systemMailboxHeavyJobs.ScanFailure;
			this.LastScanResults = systemMailboxHeavyJobs.ScanResults;
			if (systemMailboxHeavyJobs.ScanResults != null)
			{
				foreach (JobPickupRec jobPickupRec in systemMailboxHeavyJobs.ScanResults)
				{
					if (jobPickupRec.PickupResult == JobPickupResult.JobPickedUp && jobPickupRec.Timestamp > this.LastJobPickup)
					{
						this.LastJobPickup = jobPickupRec.Timestamp;
					}
				}
			}
			if (mdbIsUnreachable)
			{
				this.NextRecommendedScan = DateTime.UtcNow + MRSQueue.ScanRetryInterval;
				return;
			}
			if (this.NextRecommendedScan <= this.LastScanTimestamp)
			{
				this.NextRecommendedScan = ((systemMailboxHeavyJobs.RecommendedNextScan < MRSService.NextFullScanTime) ? systemMailboxHeavyJobs.RecommendedNextScan : MRSService.NextFullScanTime);
				perfCounter.MdbQueueQueued.RawValue = (long)systemMailboxHeavyJobs.QueuedJobsCount;
				perfCounter.MdbQueueInProgress.RawValue = (long)systemMailboxHeavyJobs.InProgressJobsCount;
				this.QueuedJobsCount = systemMailboxHeavyJobs.QueuedJobsCount;
				this.InProgressJobsCount = systemMailboxHeavyJobs.InProgressJobsCount;
			}
		}

		private XElement GetQueueDiagnosticInfo(MRSDiagnosticArgument arguments)
		{
			DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(this.MdbGuid, null, null, FindServerFlags.AllowMissing);
			string argument = arguments.GetArgument<string>("queues");
			if (!string.IsNullOrEmpty(argument) && (databaseInformation.IsMissing || !CommonUtils.IsValueInWildcardedList(databaseInformation.DatabaseName, argument)))
			{
				return null;
			}
			MRSQueueDiagnosticInfoXML mrsqueueDiagnosticInfoXML = new MRSQueueDiagnosticInfoXML
			{
				MdbGuid = this.MdbGuid,
				MdbName = databaseInformation.DatabaseName,
				LastJobPickup = this.LastJobPickup,
				LastInteractiveJobPickup = this.LastInteractiveJobPickup,
				QueuedJobsCount = this.QueuedJobsCount,
				InProgressJobsCount = this.InProgressJobsCount,
				LastScanFailure = this.LastScanFailure,
				MdbDiscoveryTimestamp = this.MdbDiscoveryTimestamp,
				LastScanTimestamp = this.LastScanTimestamp,
				LastScanDurationMs = (int)this.LastScanDuration.TotalMilliseconds,
				NextRecommendedScan = this.NextRecommendedScan,
				LastActiveJobFinishTime = this.LastActiveJobFinishTime,
				LastActiveJobFinished = this.LastActiveJobFinished
			};
			if (arguments.HasArgument("pickupresults"))
			{
				mrsqueueDiagnosticInfoXML.LastScanResults = this.LastScanResults;
			}
			return mrsqueueDiagnosticInfoXML.ToDiagnosticInfo(null);
		}

		private static readonly TimeSpan CacheTimeout = TimeSpan.FromMinutes(30.0);

		private static readonly TimeSpan ScanRetryInterval = TimeSpan.FromMinutes(1.0);

		private static readonly ExactTimeoutCache<Guid, MRSQueue> queues = new ExactTimeoutCache<Guid, MRSQueue>(null, null, null, 1000, false);

		private static readonly object locker = new object();

		public enum ScanType
		{
			Light = 1,
			Heavy
		}
	}
}
