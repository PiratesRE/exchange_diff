using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class UpdateCalendarEventAction : CalendarEventContentAction
	{
		public UpdateCalendarEventAction()
		{
		}

		public UpdateCalendarEventAction(byte[] itemId, byte[] folderId, string watermark, Event theEvent, IList<Event> exceptionalOccurrences = null, IList<string> deletedOccurrences = null) : base(itemId, folderId, watermark, theEvent, exceptionalOccurrences, deletedOccurrences)
		{
		}

		internal override ActionId Id
		{
			get
			{
				return ActionId.UpdateCalendarEvent;
			}
		}
	}
}
