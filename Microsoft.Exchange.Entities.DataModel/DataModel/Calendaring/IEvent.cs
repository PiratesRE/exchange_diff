using System;
using System.Collections.Generic;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public interface IEvent : IItem, IStorageEntity, IEntity, IPropertyChangeTracker<PropertyDefinition>, IVersioned
	{
		IList<Attendee> Attendees { get; set; }

		Calendar Calendar { get; set; }

		string ClientId { get; set; }

		bool DisallowNewTimeProposal { get; set; }

		ExDateTime End { get; set; }

		IList<string> ExceptionalProperties { get; set; }

		bool HasAttendees { get; set; }

		string IntendedEndTimeZoneId { get; set; }

		string IntendedStartTimeZoneId { get; set; }

		FreeBusyStatus IntendedStatus { get; set; }

		bool IsAllDay { get; set; }

		bool IsCancelled { get; set; }

		bool IsDraft { get; set; }

		bool IsOnlineMeeting { get; set; }

		bool IsOrganizer { get; set; }

		Location Location { get; set; }

		IList<Event> Occurrences { get; set; }

		string OnlineMeetingConfLink { get; set; }

		string OnlineMeetingExternalLink { get; set; }

		Organizer Organizer { get; set; }

		PatternedRecurrence PatternedRecurrence { get; set; }

		bool ResponseRequested { get; set; }

		ResponseStatus ResponseStatus { get; set; }

		string SeriesId { get; set; }

		Event SeriesMaster { get; set; }

		string SeriesMasterId { get; set; }

		FreeBusyStatus ShowAs { get; set; }

		ExDateTime Start { get; set; }

		EventType Type { get; set; }

		IList<EventPopupReminderSetting> PopupReminderSettings { get; set; }
	}
}
