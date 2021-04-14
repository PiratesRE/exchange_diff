using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MRSReservation : ReservationBase
	{
		public override bool IsActive
		{
			get
			{
				return true;
			}
		}

		protected override IEnumerable<ResourceBase> GetDependentResources()
		{
			yield return MRSResource.Cache.GetInstance(MRSResource.Id.ObjectGuid, base.WorkloadType);
			yield break;
		}
	}
}
