using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ReadReservation : MailboxReservation
	{
		protected override IEnumerable<ResourceBase> GetDependentResources()
		{
			if (MapiUtils.FindServerForMdb(base.ResourceId, null, null, FindServerFlags.None).IsOnThisServer)
			{
				yield return DatabaseReadResource.Cache.GetInstance(base.ResourceId, base.WorkloadType);
				yield return LocalServerReadResource.Cache.GetInstance(LocalServerResource.ResourceId, base.WorkloadType);
			}
			if (base.Flags.HasFlag(ReservationFlags.Move))
			{
				yield return MailboxMoveSourceResource.Cache.GetInstance(base.MailboxGuid);
			}
			if (base.Flags.HasFlag(ReservationFlags.Merge))
			{
				yield return MailboxMergeSourceResource.Cache.GetInstance(base.MailboxGuid);
			}
			yield break;
		}
	}
}
