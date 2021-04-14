using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ByteArrayData : ComponentData<byte[]>
	{
		public ByteArrayData()
		{
		}

		public ByteArrayData(byte[] data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return ByteArrayData.typeId;
			}
			set
			{
				ByteArrayData.typeId = value;
			}
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write(base.Data == null);
			if (base.Data == null)
			{
				return;
			}
			writer.Write(base.Data.Length);
			writer.Write(base.Data, 0, base.Data.Length);
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			if (reader.ReadBoolean())
			{
				base.Data = null;
				return;
			}
			int num = reader.ReadInt32();
			base.Data = new byte[num];
			reader.Read(base.Data, 0, num);
		}

		public override ICustomSerializable BuildObject()
		{
			return new ByteArrayData();
		}

		private static ushort typeId;
	}
}
