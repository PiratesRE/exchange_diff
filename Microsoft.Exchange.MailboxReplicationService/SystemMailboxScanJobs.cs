using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class SystemMailboxScanJobs : SystemMailboxJobs
	{
		protected SystemMailboxScanJobs(Guid mdbGuid) : base(mdbGuid)
		{
		}

		public string ScanFailure { get; private set; }

		public List<JobPickupRec> ScanResults { get; private set; }

		public DateTime RecommendedNextScan { get; private set; }

		public int QueuedJobsCount { get; private set; }

		public int InProgressJobsCount { get; private set; }

		public void PickupJobs()
		{
			this.ScanFailure = null;
			this.QueuedJobsCount = 0;
			this.InProgressJobsCount = 0;
			this.RecommendedNextScan = DateTime.MaxValue;
			this.ScanResults = new List<JobPickupRec>();
			string scanFailure;
			base.PickupJobs(out scanFailure);
			this.ScanFailure = scanFailure;
		}

		protected override void PerformPickupAccounting(RequestStatus status, JobPickupRec jobPickupRec)
		{
			switch (status)
			{
			case RequestStatus.Queued:
				if (jobPickupRec.PickupResult != JobPickupResult.CompletedJobCleanedUp && jobPickupRec.PickupResult != JobPickupResult.CompletedJobSkipped && jobPickupRec.PickupResult != JobPickupResult.InvalidJob)
				{
					this.QueuedJobsCount++;
					return;
				}
				break;
			case RequestStatus.InProgress:
				if (jobPickupRec.PickupResult == JobPickupResult.JobPickedUp || jobPickupRec.PickupResult == JobPickupResult.JobAlreadyActive)
				{
					this.InProgressJobsCount++;
				}
				if (jobPickupRec.PickupResult == JobPickupResult.JobIsPostponed)
				{
					this.QueuedJobsCount++;
				}
				break;
			default:
				return;
			}
		}

		protected override void ProcessPickupResults(JobPickupRec jobPickupRec)
		{
			this.ScanResults.Add(jobPickupRec);
			if (jobPickupRec.NextRecommendedPickup < this.RecommendedNextScan)
			{
				this.RecommendedNextScan = jobPickupRec.NextRecommendedPickup;
			}
		}
	}
}
