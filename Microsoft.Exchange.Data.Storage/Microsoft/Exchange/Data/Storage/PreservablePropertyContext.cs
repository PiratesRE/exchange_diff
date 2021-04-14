using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PreservablePropertyContext
	{
		public PreservablePropertyContext(MeetingRequest mtg, CalendarItemBase calItem, ChangeHighlightProperties organizerHighlights)
		{
			this.MeetingRequest = mtg;
			this.CalendarItem = calItem;
			this.OrganizerHighlights = organizerHighlights;
		}

		public MeetingRequest MeetingRequest { get; private set; }

		public CalendarItemBase CalendarItem { get; private set; }

		public ChangeHighlightProperties OrganizerHighlights { get; private set; }
	}
}
