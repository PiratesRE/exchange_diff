using System;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class MeetingOrganizerEntry
	{
		public MeetingOrganizerEntry()
		{
		}

		public MeetingOrganizerEntry(GlobalObjectId globalObjectId, string organizer, bool? isOrganizer, string subject)
		{
			this.CleanGlobalObjectId = (globalObjectId.IsCleanGlobalObjectId ? globalObjectId : new GlobalObjectId(globalObjectId.CleanGlobalObjectIdBytes));
			this.EntryTime = TimeProvider.UtcNow;
			this.Organizer = organizer;
			this.IsOrganizer = isOrganizer;
			if (!string.IsNullOrEmpty(subject) && subject.Length > 15)
			{
				subject = subject.Substring(0, 15) + "...";
			}
			this.Subject = subject;
		}

		public GlobalObjectId CleanGlobalObjectId { get; set; }

		public DateTime EntryTime { get; set; }

		public string Organizer { get; set; }

		public bool? IsOrganizer { get; set; }

		public string Subject { get; set; }

		public override string ToString()
		{
			return string.Format("Uid:'{0}',EntryTime:{1},Organizer:'{2}',IsOrganizer:{3},Subject:'{4}'", new object[]
			{
				this.CleanGlobalObjectId.Uid,
				this.EntryTime,
				this.Organizer,
				(this.IsOrganizer != null) ? this.IsOrganizer.Value.ToString() : "<unknown>",
				this.Subject
			});
		}

		internal void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			if (this.CleanGlobalObjectId == null)
			{
				throw new CorruptSyncStateException(new LocalizedString("[MeetingOrganizerEntry.SerializeData] CleanGlobalObjectId cannot be null during serialization."), null);
			}
			componentDataPool.GetStringDataInstance().Bind(this.CleanGlobalObjectId.Uid).SerializeData(writer, componentDataPool);
			componentDataPool.GetDateTimeDataInstance().Bind((ExDateTime)this.EntryTime).SerializeData(writer, componentDataPool);
			componentDataPool.GetStringDataInstance().Bind(this.Organizer).SerializeData(writer, componentDataPool);
			NullableData<BooleanData, bool> nullableData = new NullableData<BooleanData, bool>();
			nullableData.Bind(this.IsOrganizer).SerializeData(writer, componentDataPool);
			componentDataPool.GetStringDataInstance().Bind(this.Subject).SerializeData(writer, componentDataPool);
		}

		internal void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			StringData stringDataInstance = componentDataPool.GetStringDataInstance();
			stringDataInstance.DeserializeData(reader, componentDataPool);
			if (string.IsNullOrEmpty(stringDataInstance.Data))
			{
				throw new CorruptSyncStateException(new LocalizedString("[MeetingOrganizerEntry.DeserializeData] deserialized Uid was null or empty."), null);
			}
			this.CleanGlobalObjectId = new GlobalObjectId(stringDataInstance.Data);
			DateTimeData dateTimeDataInstance = componentDataPool.GetDateTimeDataInstance();
			dateTimeDataInstance.DeserializeData(reader, componentDataPool);
			this.EntryTime = (DateTime)dateTimeDataInstance.Data;
			StringData stringDataInstance2 = componentDataPool.GetStringDataInstance();
			stringDataInstance2.DeserializeData(reader, componentDataPool);
			this.Organizer = stringDataInstance2.Data;
			NullableData<BooleanData, bool> nullableData = new NullableData<BooleanData, bool>();
			nullableData.DeserializeData(reader, componentDataPool);
			this.IsOrganizer = nullableData.Data;
			StringData stringDataInstance3 = componentDataPool.GetStringDataInstance();
			stringDataInstance3.DeserializeData(reader, componentDataPool);
			this.Subject = stringDataInstance3.Data;
		}

		private const string formatString = "Uid:'{0}',EntryTime:{1},Organizer:'{2}',IsOrganizer:{3},Subject:'{4}'";
	}
}
