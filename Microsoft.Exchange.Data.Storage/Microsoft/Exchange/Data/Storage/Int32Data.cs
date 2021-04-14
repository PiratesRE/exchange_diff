using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Int32Data : ComponentData<int>
	{
		public Int32Data()
		{
		}

		public Int32Data(int data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return Int32Data.typeId;
			}
			set
			{
				Int32Data.typeId = value;
			}
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write(base.Data);
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			base.Data = reader.ReadInt32();
		}

		public override ICustomSerializable BuildObject()
		{
			return new Int32Data();
		}

		private static ushort typeId;
	}
}
