using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Int64Data : ComponentData<long>
	{
		public Int64Data()
		{
		}

		public Int64Data(long data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return Int64Data.typeId;
			}
			set
			{
				Int64Data.typeId = value;
			}
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write(base.Data);
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			base.Data = reader.ReadInt64();
		}

		public override ICustomSerializable BuildObject()
		{
			return new Int64Data();
		}

		private static ushort typeId;
	}
}
