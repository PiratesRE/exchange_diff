using System;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class Event : Item
	{
		public DateTimeOffset Start
		{
			get
			{
				return (DateTimeOffset)base[EventSchema.Start];
			}
			set
			{
				base[EventSchema.Start] = value;
			}
		}

		public DateTimeOffset End
		{
			get
			{
				return (DateTimeOffset)base[EventSchema.End];
			}
			set
			{
				base[EventSchema.End] = value;
			}
		}

		public Location Location
		{
			get
			{
				return (Location)base[EventSchema.Location];
			}
			set
			{
				base[EventSchema.Location] = value;
			}
		}

		public FreeBusyStatus ShowAs
		{
			get
			{
				return (FreeBusyStatus)base[EventSchema.ShowAs];
			}
			set
			{
				base[EventSchema.ShowAs] = value;
			}
		}

		public bool IsAllDay
		{
			get
			{
				return (bool)base[EventSchema.IsAllDay];
			}
			set
			{
				base[EventSchema.IsAllDay] = value;
			}
		}

		public bool IsCancelled
		{
			get
			{
				return (bool)base[EventSchema.IsCancelled];
			}
			set
			{
				base[EventSchema.IsCancelled] = value;
			}
		}

		public bool IsOrganizer
		{
			get
			{
				return (bool)base[EventSchema.IsOrganizer];
			}
			set
			{
				base[EventSchema.IsOrganizer] = value;
			}
		}

		public bool ResponseRequested
		{
			get
			{
				return (bool)base[EventSchema.ResponseRequested];
			}
			set
			{
				base[EventSchema.ResponseRequested] = value;
			}
		}

		public EventType Type
		{
			get
			{
				return (EventType)base[EventSchema.Type];
			}
			set
			{
				base[EventSchema.Type] = value;
			}
		}

		public string SeriesId
		{
			get
			{
				return (string)base[EventSchema.SeriesId];
			}
			set
			{
				base[EventSchema.SeriesId] = value;
			}
		}

		public string SeriesMasterId
		{
			get
			{
				return (string)base[EventSchema.SeriesMasterId];
			}
			set
			{
				base[EventSchema.SeriesMasterId] = value;
			}
		}

		public Attendee[] Attendees
		{
			get
			{
				return (Attendee[])base[EventSchema.Attendees];
			}
			set
			{
				base[EventSchema.Attendees] = value;
			}
		}

		public PatternedRecurrence Recurrence
		{
			get
			{
				return (PatternedRecurrence)base[EventSchema.Recurrence];
			}
			set
			{
				base[EventSchema.Recurrence] = value;
			}
		}

		public Recipient Organizer
		{
			get
			{
				return (Recipient)base[EventSchema.Organizer];
			}
			set
			{
				base[EventSchema.Organizer] = value;
			}
		}

		public Calendar Calendar
		{
			get
			{
				return (Calendar)base[EventSchema.Calendar];
			}
			set
			{
				base[EventSchema.Calendar] = value;
			}
		}

		internal override EntitySchema Schema
		{
			get
			{
				return EventSchema.SchemaInstance;
			}
		}

		internal new static readonly EdmEntityType EdmEntityType = new EdmEntityType(typeof(Event).Namespace, typeof(Event).Name, Microsoft.Exchange.Services.OData.Model.Item.EdmEntityType);
	}
}
