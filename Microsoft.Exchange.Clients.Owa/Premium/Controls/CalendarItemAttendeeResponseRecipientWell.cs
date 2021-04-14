using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class CalendarItemAttendeeResponseRecipientWell : ItemRecipientWell
	{
		internal CalendarItemAttendeeResponseRecipientWell(CalendarItemBase calendarItemBase)
		{
			this.calendarItemBase = calendarItemBase;
		}

		internal CalendarItemAttendeeResponseRecipientWell() : this(null)
		{
		}

		internal override IEnumerator<Participant> GetRecipientsCollection(RecipientWellType type)
		{
			if (this.calendarItemBase != null)
			{
				ResponseType responseType;
				switch (type)
				{
				case RecipientWellType.Cc:
					responseType = ResponseType.Tentative;
					goto IL_69;
				case RecipientWellType.Bcc:
					responseType = ResponseType.Decline;
					goto IL_69;
				}
				responseType = ResponseType.Accept;
				IL_69:
				foreach (Attendee attendee in this.calendarItemBase.AttendeeCollection)
				{
					if (attendee.ResponseType == responseType)
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
