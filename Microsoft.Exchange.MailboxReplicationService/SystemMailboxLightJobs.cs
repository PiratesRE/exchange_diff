using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class SystemMailboxLightJobs : SystemMailboxScanJobs
	{
		public SystemMailboxLightJobs(Guid mdbGuid) : base(mdbGuid)
		{
		}

		protected override void ProcessJobs(MapiStore systemMbx, MapiTable contentsTable, RequestJobNamedPropertySet nps)
		{
			SortOrder sortOrder = new SortOrder(nps.PropTags[17], SortFlags.Descend);
			sortOrder.Add(nps.PropTags[7], SortFlags.Ascend);
			MrsTracer.Service.Debug("Searching for MRs to Rehome...", new object[0]);
			Restriction restriction = Restriction.And(new Restriction[]
			{
				Restriction.EQ(nps.PropTags[4], false),
				Restriction.EQ(nps.PropTags[20], true)
			});
			MrsTracer.Service.Debug("Searching for MRs to Suspend...", new object[0]);
			base.ProcessJobsInBatches(restriction, false, sortOrder, contentsTable, systemMbx, null);
			Restriction restriction2 = Restriction.And(new Restriction[]
			{
				Restriction.BitMaskNonZero(nps.PropTags[10], 256),
				Restriction.EQ(nps.PropTags[4], false),
				Restriction.EQ(nps.PropTags[20], false),
				Restriction.NE(nps.PropTags[0], RequestStatus.Failed),
				Restriction.NE(nps.PropTags[0], RequestStatus.Suspended),
				Restriction.NE(nps.PropTags[0], RequestStatus.AutoSuspended),
				Restriction.NE(nps.PropTags[0], RequestStatus.Completed),
				Restriction.NE(nps.PropTags[0], RequestStatus.CompletedWithWarning)
			});
			base.ProcessJobsInBatches(restriction2, false, sortOrder, contentsTable, systemMbx, null);
			MrsTracer.Service.Debug("Searching for MRs to Resume...", new object[0]);
			Restriction restriction3 = Restriction.And(new Restriction[]
			{
				Restriction.BitMaskZero(nps.PropTags[10], 256),
				Restriction.EQ(nps.PropTags[4], false),
				Restriction.EQ(nps.PropTags[20], false),
				Restriction.Or(new Restriction[]
				{
					Restriction.EQ(nps.PropTags[0], RequestStatus.Failed),
					Restriction.EQ(nps.PropTags[0], RequestStatus.Suspended),
					Restriction.EQ(nps.PropTags[0], RequestStatus.AutoSuspended)
				})
			});
			base.ProcessJobsInBatches(restriction3, false, sortOrder, contentsTable, systemMbx, null);
			SortOrder sort = new SortOrder(nps.PropTags[13], SortFlags.Ascend);
			DateTime utcNow = DateTime.UtcNow;
			MrsTracer.Service.Debug("Searching for Completed MRs to clean up...", new object[0]);
			Restriction restriction4 = Restriction.And(new Restriction[]
			{
				Restriction.EQ(nps.PropTags[20], false),
				Restriction.EQ(nps.PropTags[4], false),
				Restriction.EQ(nps.PropTags[0], RequestStatus.Completed),
				Restriction.NE(nps.PropTags[13], SystemMailboxLightJobs.defaultDoNotPickUntil)
			});
			base.ProcessJobsInBatches(restriction4, false, sort, contentsTable, systemMbx, (MoveJob moveJob) => moveJob.DoNotPickUntilTimestamp > utcNow);
		}

		private const int DateTimeMinValue_SerializedAs = 0;

		private static DateTime defaultDoNotPickUntil = DateTime.FromFileTimeUtc(0L);
	}
}
