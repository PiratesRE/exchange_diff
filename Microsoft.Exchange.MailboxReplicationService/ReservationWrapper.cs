using System;
using System.Net;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ReservationWrapper : WrapperBase<IReservation>, IReservation, IDisposable
	{
		public ReservationWrapper(IReservation reservation) : base(reservation, null)
		{
		}

		Guid IReservation.Id
		{
			get
			{
				return base.WrappedObject.Id;
			}
		}

		ReservationFlags IReservation.Flags
		{
			get
			{
				return base.WrappedObject.Flags;
			}
		}

		Guid IReservation.ResourceId
		{
			get
			{
				return base.WrappedObject.ResourceId;
			}
		}

		public static IReservation CreateReservation(string serverFQDN, NetworkCredential credentials, Guid mailboxGuid, TenantPartitionHint partitionHint, Guid resourceId, ReservationFlags flags)
		{
			if (serverFQDN == null)
			{
				TestIntegration testIntegration = new TestIntegration(false);
				if ((flags.HasFlag(ReservationFlags.Read) && testIntegration.UseRemoteForSource) || (flags.HasFlag(ReservationFlags.Write) && testIntegration.UseRemoteForDestination))
				{
					serverFQDN = CommonUtils.LocalComputerName;
				}
			}
			IReservation result = null;
			ExecutionContext.Create(new DataContext[]
			{
				new OperationDataContext("IReservationManager.ReserveResources", OperationType.None),
				new SimpleValueDataContext("MailboxGuid", mailboxGuid),
				new SimpleValueDataContext("ResourceId", resourceId),
				new SimpleValueDataContext("Flags", flags)
			}).Execute(delegate
			{
				if (serverFQDN == null)
				{
					result = ReservationManager.CreateReservation(mailboxGuid, partitionHint, resourceId, flags, CommonUtils.LocalComputerName);
					return;
				}
				result = RemoteReservation.Create(serverFQDN, credentials, mailboxGuid, partitionHint, resourceId, flags);
			});
			return new ReservationWrapper(result);
		}
	}
}
