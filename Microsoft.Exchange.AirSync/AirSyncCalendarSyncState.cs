using System;
using System.IO;
using Microsoft.Exchange.Data.ApplicationLogic.SyncCalendar;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class AirSyncCalendarSyncState : CalendarSyncState, ICustomSerializableBuilder, ICustomSerializable
	{
		public AirSyncCalendarSyncState() : base(null, null, null)
		{
		}

		public AirSyncCalendarSyncState(string base64IcsSyncState, CalendarViewQueryResumptionPoint queryResumptionPoint, ExDateTime? oldWindowEndTime) : base(base64IcsSyncState, queryResumptionPoint, oldWindowEndTime)
		{
		}

		public ushort TypeId
		{
			get
			{
				return AirSyncCalendarSyncState.typeId;
			}
			set
			{
				AirSyncCalendarSyncState.typeId = value;
			}
		}

		public override IFolderSyncState CreateFolderSyncState(StoreObjectId folderObjectId, ISyncProvider syncProvider)
		{
			return new SyncCalendarFolderSyncState(folderObjectId, syncProvider, base.IcsSyncState);
		}

		public ICustomSerializable BuildObject()
		{
			return new AirSyncCalendarSyncState(null, null, null);
		}

		public void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			string text = reader.ReadString();
			if (string.IsNullOrEmpty(text))
			{
				throw new CorruptSyncStateException("AirSyncCalendarSyncState.DeserializeData.EmptyString", null);
			}
			string[] array = text.Split(new char[]
			{
				','
			});
			if (array.Length != 7 || array[0] != "v2")
			{
				throw new CorruptSyncStateException("AirSyncCalendarSyncState.DeserializeData.InvalidVersion", null);
			}
			base.IcsSyncState = array[1];
			base.OldWindowEnd = CalendarSyncState.SafeGetDateTimeValue(array[6]);
			base.QueryResumptionPoint = CalendarSyncState.SafeGetResumptionPoint(array[2], array[3], array[4], array[5]);
		}

		public void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write(this.ToString());
		}

		public static readonly AirSyncCalendarSyncState Empty = new AirSyncCalendarSyncState(null, null, null);

		private static ushort typeId;
	}
}
