using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class SystemMailboxHeavyJobs : SystemMailboxScanJobs
	{
		public SystemMailboxHeavyJobs(Guid mdbGuid) : base(mdbGuid)
		{
		}

		protected override void ProcessJobs(MapiStore systemMbx, MapiTable contentsTable, RequestJobNamedPropertySet nps)
		{
			MrsTracer.Service.Debug("Initializing searches...", new object[0]);
			SortOrder sortOrder = new SortOrder(nps.PropTags[17], SortFlags.Descend);
			sortOrder.Add(nps.PropTags[7], SortFlags.Ascend);
			Restriction restriction = Restriction.And(new Restriction[]
			{
				Restriction.EQ(nps.PropTags[4], true),
				Restriction.EQ(nps.PropTags[20], false)
			});
			Restriction restriction2 = Restriction.And(new Restriction[]
			{
				Restriction.BitMaskZero(nps.PropTags[10], 256),
				Restriction.EQ(nps.PropTags[4], false),
				Restriction.EQ(nps.PropTags[20], false),
				Restriction.Or(new Restriction[]
				{
					Restriction.EQ(nps.PropTags[0], RequestStatus.Queued),
					Restriction.EQ(nps.PropTags[0], RequestStatus.InProgress),
					Restriction.EQ(nps.PropTags[0], RequestStatus.CompletionInProgress)
				})
			});
			base.ProcessJobsInBatches(Restriction.Or(new Restriction[]
			{
				restriction2,
				restriction
			}), true, sortOrder, contentsTable, systemMbx, null);
			Restriction restriction3 = Restriction.And(new Restriction[]
			{
				Restriction.BitMaskZero(nps.PropTags[10], 256),
				Restriction.EQ(nps.PropTags[4], false),
				Restriction.EQ(nps.PropTags[20], false),
				Restriction.EQ(nps.PropTags[0], RequestStatus.Synced)
			});
			SortOrder sort = new SortOrder(nps.PropTags[13], SortFlags.Ascend);
			DateTime utcNow = DateTime.UtcNow;
			base.ProcessJobsInBatches(restriction3, false, sort, contentsTable, systemMbx, (MoveJob moveJob) => moveJob.DoNotPickUntilTimestamp > utcNow);
		}
	}
}
