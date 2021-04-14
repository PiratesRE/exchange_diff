using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class WriteReservation : MailboxReservation
	{
		protected override IEnumerable<ResourceBase> GetDependentResources()
		{
			if (MapiUtils.FindServerForMdb(base.ResourceId, null, null, FindServerFlags.None).IsOnThisServer)
			{
				yield return DatabaseWriteResource.Cache.GetInstance(base.ResourceId, base.WorkloadType);
				yield return LocalServerWriteResource.Cache.GetInstance(LocalServerResource.ResourceId, base.WorkloadType);
			}
			if (base.Flags.HasFlag(ReservationFlags.Move))
			{
				yield return MailboxMoveTargetResource.Cache.GetInstance(base.MailboxGuid);
			}
			if (base.Flags.HasFlag(ReservationFlags.Merge))
			{
				yield return MailboxMergeTargetResource.Cache.GetInstance(base.MailboxGuid);
			}
			yield break;
		}
	}
}
