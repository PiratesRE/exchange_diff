using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class MailboxMoveResource : MailboxResource
	{
		public MailboxMoveResource(Guid mailboxGuid) : base(mailboxGuid)
		{
		}

		public override int StaticCapacity
		{
			get
			{
				return 1;
			}
		}

		protected override void ThrowStaticCapacityExceededException()
		{
			using (Dictionary<Guid, ReservationBase>.ValueCollection.Enumerator enumerator = base.Reservations.Values.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ReservationBase reservationBase = enumerator.Current;
					throw new MoveInProgressReservationException(string.Format("{0}({1})", this.ResourceType, this.ResourceName), reservationBase.ClientName);
				}
			}
		}
	}
}
