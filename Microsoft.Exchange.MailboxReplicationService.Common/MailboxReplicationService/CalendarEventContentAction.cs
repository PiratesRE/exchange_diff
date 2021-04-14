using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal abstract class CalendarEventContentAction : ReplayAction
	{
		protected CalendarEventContentAction()
		{
		}

		[DataMember]
		public string EventData { get; set; }

		[DataMember]
		public IList<string> ExceptionalEventsData { get; set; }

		[DataMember]
		public IList<string> DeletedOccurrences { get; set; }

		protected CalendarEventContentAction(byte[] itemId, byte[] folderId, string watermark, Event theEvent, IList<Event> exceptionalOccurrences = null, IList<string> deletedOccurrences = null) : base(watermark)
		{
			base.ItemId = itemId;
			base.FolderId = folderId;
			this.EventData = EntitySerializer.Serialize<Event>(theEvent);
			this.DeletedOccurrences = deletedOccurrences;
			if (exceptionalOccurrences != null)
			{
				List<string> list = new List<string>();
				foreach (Event obj in exceptionalOccurrences)
				{
					list.Add(EntitySerializer.Serialize<Event>(obj));
				}
				this.ExceptionalEventsData = list;
			}
		}

		public Event Event
		{
			get
			{
				if (this.deserializedEvent == null && this.EventData != null)
				{
					this.deserializedEvent = EntitySerializer.Deserialize<Event>(this.EventData);
				}
				return this.deserializedEvent;
			}
		}

		public IList<Event> ExceptionalEvents
		{
			get
			{
				if (this.deserializedExceptionalEvents == null && this.ExceptionalEventsData != null)
				{
					List<Event> list = new List<Event>(this.ExceptionalEventsData.Count);
					foreach (string serializedObject in this.ExceptionalEventsData)
					{
						list.Add(EntitySerializer.Deserialize<Event>(serializedObject));
					}
					this.deserializedExceptionalEvents = list;
				}
				return this.deserializedExceptionalEvents;
			}
		}

		public override string ToString()
		{
			return base.ToString() + ", EntryId: " + TraceUtils.DumpEntryId(base.ItemId);
		}

		private Event deserializedEvent;

		private IList<Event> deserializedExceptionalEvents;
	}
}
