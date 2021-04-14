using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublicFolderFreeBusyAppointment
	{
		public PublicFolderFreeBusyAppointment(ExDateTime startTime, ExDateTime endTime, BusyType busyType)
		{
			this.StartTime = startTime;
			this.EndTime = endTime;
			if (EnumValidator.IsValidValue<BusyType>(busyType))
			{
				this.BusyType = busyType;
				return;
			}
			this.BusyType = BusyType.Free;
		}

		public ExDateTime StartTime { get; private set; }

		public ExDateTime EndTime { get; private set; }

		public BusyType BusyType { get; private set; }
	}
}
