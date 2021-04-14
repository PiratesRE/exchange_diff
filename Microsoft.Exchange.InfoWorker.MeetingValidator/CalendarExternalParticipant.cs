using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CalendarExternalParticipant : CalendarParticipant
	{
		internal CalendarExternalParticipant(UserObject userObject, ExDateTime validateFrom, ExDateTime validateUntil) : base(userObject, validateFrom, validateUntil)
		{
		}

		internal override void ValidateMeetings(ref Dictionary<GlobalObjectId, List<Attendee>> organizerRumsSent, Action<long> onItemRepaired)
		{
			foreach (CalendarInstanceContext calendarInstanceContext in base.ItemList.Values)
			{
				calendarInstanceContext.ValidationContext.CalendarInstance = null;
				base.ValidateInstance(calendarInstanceContext, organizerRumsSent, onItemRepaired);
			}
		}
	}
}
