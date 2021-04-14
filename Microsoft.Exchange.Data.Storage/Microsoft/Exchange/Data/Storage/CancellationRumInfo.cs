using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CancellationRumInfo : OrganizerRumInfo
	{
		private CancellationRumInfo() : this(null, null)
		{
		}

		private CancellationRumInfo(ExDateTime? originalStartTime, IList<Attendee> attendees) : base(RumType.Cancellation, originalStartTime, attendees)
		{
		}

		public static CancellationRumInfo CreateMasterInstance(IList<Attendee> attendees)
		{
			return new CancellationRumInfo(null, attendees);
		}

		public static CancellationRumInfo CreateOccurrenceInstance(ExDateTime originalStartTime, IList<Attendee> attendees)
		{
			return new CancellationRumInfo(new ExDateTime?(originalStartTime), attendees);
		}
	}
}
