using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class JobSortKey : IComparable<JobSortKey>
	{
		public JobSortKey(RequestPriority priority, JobSortFlags flags, DateTime lastUpdate, Guid requestGuid)
		{
			this.priority = priority;
			this.flags = flags;
			this.lastUpdate = lastUpdate;
			this.requestGuid = requestGuid;
		}

		public int CompareTo(JobSortKey other)
		{
			if (this.priority != other.priority)
			{
				return other.priority.CompareTo(this.priority);
			}
			if (!this.flags.HasFlag(JobSortFlags.IsInteractive) && !other.flags.HasFlag(JobSortFlags.IsInteractive) && this.flags.HasFlag(JobSortFlags.HasReservations) != other.flags.HasFlag(JobSortFlags.HasReservations))
			{
				return other.flags.HasFlag(JobSortFlags.HasReservations).CompareTo(this.flags.HasFlag(JobSortFlags.HasReservations));
			}
			return JobSortKey.CompareTimeAndRequestGuid(this.lastUpdate, other.lastUpdate, this.requestGuid, other.requestGuid);
		}

		private static int CompareTimeAndRequestGuid(DateTime first, DateTime second, Guid firstGuid, Guid secondGuid)
		{
			int num = first.CompareTo(second);
			if (num != 0)
			{
				return num;
			}
			return firstGuid.CompareTo(secondGuid);
		}

		private readonly RequestPriority priority;

		private readonly JobSortFlags flags;

		private readonly DateTime lastUpdate;

		private readonly Guid requestGuid;
	}
}
