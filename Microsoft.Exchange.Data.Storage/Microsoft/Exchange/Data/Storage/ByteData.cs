using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ByteData : ComponentData<byte>
	{
		public ByteData()
		{
		}

		public ByteData(byte data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return ByteData.typeId;
			}
			set
			{
				ByteData.typeId = value;
			}
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write(base.Data);
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			base.Data = reader.ReadByte();
		}

		public override ICustomSerializable BuildObject()
		{
			return new ByteData();
		}

		private static ushort typeId;
	}
}
