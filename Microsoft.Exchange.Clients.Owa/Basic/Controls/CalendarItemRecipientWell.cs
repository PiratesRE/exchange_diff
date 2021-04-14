using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public class CalendarItemRecipientWell : ItemRecipientWell
	{
		internal CalendarItemRecipientWell(UserContext userContext, CalendarItemBase calendarItemBase) : base(userContext)
		{
			this.calendarItemBase = calendarItemBase;
		}

		internal override IEnumerator<Participant> GetRecipientCollection(RecipientWellType type)
		{
			if (this.calendarItemBase != null)
			{
				AttendeeType attendeeType;
				switch (type)
				{
				case RecipientWellType.Cc:
					attendeeType = AttendeeType.Optional;
					goto IL_69;
				case RecipientWellType.Bcc:
					attendeeType = AttendeeType.Resource;
					goto IL_69;
				}
				attendeeType = AttendeeType.Required;
				IL_69:
				foreach (Attendee attendee in this.calendarItemBase.AttendeeCollection)
				{
					if (CalendarUtilities.IsExpectedTypeAttendee(attendee, attendeeType))
					{
						yield return attendee.Participant;
					}
				}
			}
			yield break;
		}

		private CalendarItemBase calendarItemBase;
	}
}
