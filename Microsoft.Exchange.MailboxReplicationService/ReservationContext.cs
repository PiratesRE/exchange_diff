using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ReservationContext : DisposeTrackableBase
	{
		public ReservationContext()
		{
			this.Reservations = new List<IReservation>();
		}

		public List<IReservation> Reservations { get; private set; }

		public void AddReservation(IReservation reservation)
		{
			this.Reservations.Add(reservation);
		}

		public IReservation GetReservation(Guid mdbGuid, ReservationFlags flags)
		{
			flags &= (ReservationFlags.Read | ReservationFlags.Write);
			foreach (IReservation reservation in this.Reservations)
			{
				if (reservation.ResourceId == mdbGuid && reservation.Flags.HasFlag(flags))
				{
					return reservation;
				}
			}
			return null;
		}

		public void ReserveResource(Guid mailboxGuid, TenantPartitionHint partitionHint, ADObjectId resourceId, ReservationFlags flags)
		{
			string serverFQDN = null;
			if (!(resourceId.ObjectGuid == MRSResource.Id.ObjectGuid))
			{
				DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(resourceId, null, null, FindServerFlags.None);
				if (databaseInformation.ServerVersion < Server.E15MinVersion)
				{
					return;
				}
				if (!databaseInformation.IsOnThisServer)
				{
					serverFQDN = databaseInformation.ServerFqdn;
				}
			}
			this.AddReservation(ReservationWrapper.CreateReservation(serverFQDN, null, mailboxGuid, partitionHint, resourceId.ObjectGuid, flags));
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				foreach (IReservation reservation in this.Reservations)
				{
					reservation.Dispose();
				}
				this.Reservations.Clear();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ReservationContext>(this);
		}
	}
}
