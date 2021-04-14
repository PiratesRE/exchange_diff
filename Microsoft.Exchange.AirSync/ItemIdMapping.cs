using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class ItemIdMapping : IdMapping, IItemIdMapping, IIdMapping
	{
		public ItemIdMapping()
		{
		}

		public ItemIdMapping(string parentSyncId)
		{
			this.parentSyncId = parentSyncId;
		}

		public override ushort TypeId
		{
			get
			{
				return ItemIdMapping.typeId;
			}
			set
			{
				ItemIdMapping.typeId = value;
			}
		}

		public string Add(ISyncItemId mailboxItemId)
		{
			AirSyncDiagnostics.Assert(mailboxItemId != null);
			if (mailboxItemId != null && mailboxItemId.NativeId != null)
			{
				StoreObjectId storeObjectId = mailboxItemId.NativeId as StoreObjectId;
				if (storeObjectId != null && storeObjectId.ObjectType == StoreObjectType.CalendarItemOccurrence)
				{
					AirSyncDiagnostics.TraceDebug<ISyncItemId>(ExTraceGlobals.RequestsTracer, this, "CalendarItemOccurrence ItemId is being added to the ItemIdMapping! Id:{0}.", mailboxItemId);
				}
			}
			string text;
			if (base.OldIds.ContainsKey(mailboxItemId))
			{
				text = base.OldIds[mailboxItemId];
			}
			else
			{
				text = this.parentSyncId + ":" + base.UniqueCounter.ToString(CultureInfo.InvariantCulture);
			}
			AirSyncDiagnostics.Assert(text.Length <= 64);
			base.Add(mailboxItemId, text);
			return text;
		}

		public new void Add(ISyncItemId itemId, string syncId)
		{
			AirSyncDiagnostics.Assert(itemId != null);
			base.Add(itemId, syncId);
		}

		public override ICustomSerializable BuildObject()
		{
			return new ItemIdMapping();
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			base.DeserializeData(reader, componentDataPool);
			StringData stringDataInstance = componentDataPool.GetStringDataInstance();
			stringDataInstance.DeserializeData(reader, componentDataPool);
			this.parentSyncId = stringDataInstance.Data;
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			base.SerializeData(writer, componentDataPool);
			componentDataPool.GetStringDataInstance().Bind(this.parentSyncId).SerializeData(writer, componentDataPool);
		}

		public void UpdateParent(string newParent)
		{
			this.parentSyncId = newParent;
		}

		private static ushort typeId;

		private string parentSyncId;
	}
}
