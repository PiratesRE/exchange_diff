using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UInt32Data : ComponentData<uint>
	{
		public UInt32Data()
		{
		}

		public UInt32Data(uint data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return UInt32Data.typeId;
			}
			set
			{
				UInt32Data.typeId = value;
			}
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write(base.Data);
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			base.Data = reader.ReadUInt32();
		}

		public override ICustomSerializable BuildObject()
		{
			return new UInt32Data();
		}

		private static ushort typeId;
	}
}
