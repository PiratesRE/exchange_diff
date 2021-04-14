using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class CalendarItemRecipientWell : ItemRecipientWell
	{
		internal CalendarItemRecipientWell(CalendarItemBase calendarItemBase)
		{
			this.calendarItemBase = calendarItemBase;
		}

		internal CalendarItemRecipientWell() : this(null)
		{
		}

		internal override IEnumerator<Participant> GetRecipientsCollection(RecipientWellType type)
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
					if (attendee.AttendeeType == attendeeType && !attendee.IsOrganizer)
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
