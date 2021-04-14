using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MissingAttendeeItemRumInfo : UpdateRumInfo
	{
		private MissingAttendeeItemRumInfo(ExDateTime? originalStartTime, IList<Attendee> attendees, CalendarInconsistencyFlag inconsistencyFlag, int? deletedItemVersion) : base(originalStartTime, attendees, inconsistencyFlag)
		{
			this.DeletedItemVersion = deletedItemVersion;
		}

		public static MissingAttendeeItemRumInfo CreateMasterInstance(IList<Attendee> attendees, CalendarInconsistencyFlag inconsistencyFlag, int? deletedItemVersion)
		{
			return new MissingAttendeeItemRumInfo(null, attendees, inconsistencyFlag, deletedItemVersion);
		}

		public new static MissingAttendeeItemRumInfo CreateOccurrenceInstance(ExDateTime originalStartTime, IList<Attendee> attendees, CalendarInconsistencyFlag inconsistencyFlag)
		{
			return new MissingAttendeeItemRumInfo(new ExDateTime?(originalStartTime), attendees, inconsistencyFlag, null);
		}

		public int? DeletedItemVersion { get; private set; }
	}
}
