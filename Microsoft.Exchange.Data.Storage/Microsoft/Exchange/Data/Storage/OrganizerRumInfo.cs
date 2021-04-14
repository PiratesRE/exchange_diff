using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class OrganizerRumInfo : RumInfo
	{
		private OrganizerRumInfo() : base(RumType.None, null)
		{
		}

		protected OrganizerRumInfo(RumType type, ExDateTime? originalStartTime, IList<Attendee> attendees) : base(type, originalStartTime)
		{
			this.AttendeeList = new List<Attendee>(attendees);
			this.AttendeeRequiredSequenceNumber = int.MinValue;
		}

		public IList<Attendee> AttendeeList { get; private set; }

		public int AttendeeRequiredSequenceNumber { get; set; }
	}
}
