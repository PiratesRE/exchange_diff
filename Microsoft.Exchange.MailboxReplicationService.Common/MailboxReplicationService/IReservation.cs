using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IReservation : IDisposable
	{
		Guid Id { get; }

		ReservationFlags Flags { get; }

		Guid ResourceId { get; }
	}
}
