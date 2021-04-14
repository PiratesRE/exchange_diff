using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class StoreObjectIdData : ComponentData<StoreObjectId>
	{
		public StoreObjectIdData()
		{
		}

		public StoreObjectIdData(StoreObjectId data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return StoreObjectIdData.typeId;
			}
			set
			{
				StoreObjectIdData.typeId = value;
			}
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write(base.Data == null);
			if (base.Data == null)
			{
				return;
			}
			writer.Write(false);
			writer.Write(base.Data.GetByteArrayLength());
			StoreObjectId.Serialize(base.Data, writer);
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			if (reader.ReadBoolean())
			{
				base.Data = null;
				return;
			}
			if (reader.ReadBoolean())
			{
				base.Data = null;
				return;
			}
			int byteArrayLength = reader.ReadInt32();
			base.Data = StoreObjectId.Deserialize(reader, byteArrayLength);
		}

		public override ICustomSerializable BuildObject()
		{
			return new StoreObjectIdData();
		}

		private static ushort typeId;
	}
}
