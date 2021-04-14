using System;
using System.Diagnostics.Eventing.Reader;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class LocalDataAccessMetaData
	{
		internal LocalDataAccessMetaData(EventRecord record)
		{
			if (record != null)
			{
				this.RecordId = record.RecordId.Value;
				this.TimeStamp = record.TimeCreated.Value;
				this.Bookmark = record.Bookmark;
				this.EventId = record.Id;
			}
		}

		public long RecordId { get; private set; }

		public DateTime TimeStamp { get; private set; }

		public EventBookmark Bookmark { get; private set; }

		public int EventId { get; private set; }
	}
}
