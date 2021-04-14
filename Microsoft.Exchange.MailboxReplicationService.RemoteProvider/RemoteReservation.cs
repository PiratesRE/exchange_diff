using System;
using System.Net;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RemoteReservation : DisposeTrackableBase, IReservation, IDisposable
	{
		public RemoteReservation(Guid reservationID, string serverFQDN, NetworkCredential credentials, Guid mailboxGuid, Guid mdbGuid, ReservationFlags flags)
		{
			this.reservationId = reservationID;
			this.serverFQDN = serverFQDN;
			this.credentials = credentials;
			this.mailboxGuid = mailboxGuid;
			this.mdbGuid = mdbGuid;
			this.flags = flags;
		}

		Guid IReservation.Id
		{
			get
			{
				return this.reservationId;
			}
		}

		ReservationFlags IReservation.Flags
		{
			get
			{
				return this.flags;
			}
		}

		Guid IReservation.ResourceId
		{
			get
			{
				return this.mdbGuid;
			}
		}

		public static RemoteReservation Create(string serverFQDN, NetworkCredential credentials, Guid mailboxGuid, TenantPartitionHint partitionHint, Guid mdbGuid, ReservationFlags flags)
		{
			RemoteReservation result;
			using (MailboxReplicationProxyClient mailboxReplicationProxyClient = MailboxReplicationProxyClient.CreateWithoutThrottling(serverFQDN, credentials, mailboxGuid, mdbGuid))
			{
				Guid reservationID;
				if (mailboxReplicationProxyClient.ServerVersion[37])
				{
					byte[] partitionHintBytes = (partitionHint != null) ? partitionHint.GetPersistablePartitionHint() : null;
					reservationID = ((IMailboxReplicationProxyService)mailboxReplicationProxyClient).IReservationManager_ReserveResources(mailboxGuid, partitionHintBytes, mdbGuid, (int)flags);
				}
				else if (mailboxReplicationProxyClient.ServerVersion[28])
				{
					reservationID = Guid.NewGuid();
					LegacyReservationStatus legacyReservationStatus = (LegacyReservationStatus)((IMailboxReplicationProxyService)mailboxReplicationProxyClient).IMailbox_ReserveResources(reservationID, mdbGuid, (int)RemoteReservation.ConvertReservationFlagsToLegacy(flags, true));
					if (legacyReservationStatus != LegacyReservationStatus.Success)
					{
						throw new CapacityExceededReservationException(string.Format("{0}:{1}:{2}", serverFQDN, mdbGuid, flags), 1);
					}
				}
				else
				{
					reservationID = RemoteReservation.DownlevelReservationId;
				}
				result = new RemoteReservation(reservationID, serverFQDN, credentials, mailboxGuid, mdbGuid, flags);
			}
			return result;
		}

		public void ConfirmLegacyReservation(MailboxReplicationProxyClient client)
		{
			if (client.ServerVersion[28])
			{
				LegacyReservationStatus legacyReservationStatus = (LegacyReservationStatus)((IMailboxReplicationProxyService)client).IMailbox_ReserveResources(this.reservationId, this.mdbGuid, (int)RemoteReservation.ConvertReservationFlagsToLegacy(this.flags, false));
				if (legacyReservationStatus != LegacyReservationStatus.Success)
				{
					throw new CapacityExceededReservationException(string.Format("{0}:{1}:{2}", this.serverFQDN, this.mdbGuid, this.flags), 1);
				}
			}
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.reservationId != Guid.Empty)
			{
				if (this.reservationId == RemoteReservation.DownlevelReservationId)
				{
					return;
				}
				CommonUtils.CatchKnownExceptions(delegate
				{
					using (MailboxReplicationProxyClient mailboxReplicationProxyClient = MailboxReplicationProxyClient.CreateWithoutThrottling(this.serverFQDN, this.credentials, this.mailboxGuid, this.mdbGuid))
					{
						if (mailboxReplicationProxyClient.ServerVersion[37])
						{
							((IMailboxReplicationProxyService)mailboxReplicationProxyClient).IReservationManager_ReleaseResources(this.reservationId);
						}
						else if (mailboxReplicationProxyClient.ServerVersion[28])
						{
							((IMailboxReplicationProxyService)mailboxReplicationProxyClient).IMailbox_ReserveResources(this.reservationId, Guid.Empty, 3);
						}
					}
				}, null);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<RemoteReservation>(this);
		}

		private static LegacyReservationType ConvertReservationFlagsToLegacy(ReservationFlags flags, bool expiring)
		{
			LegacyReservationType result;
			if (flags.HasFlag(ReservationFlags.Read))
			{
				if (expiring)
				{
					result = LegacyReservationType.ExpiredRead;
				}
				else
				{
					result = LegacyReservationType.Read;
				}
			}
			else if (expiring)
			{
				result = LegacyReservationType.ExpiredWrite;
			}
			else
			{
				result = LegacyReservationType.Write;
			}
			return result;
		}

		private static readonly Guid DownlevelReservationId = new Guid("c83d3976-43cc-4fbc-afe9-ed5bce7f3acb");

		private readonly Guid reservationId;

		private readonly Guid mailboxGuid;

		private readonly Guid mdbGuid;

		private readonly ReservationFlags flags;

		private readonly string serverFQDN;

		private readonly NetworkCredential credentials;
	}
}
