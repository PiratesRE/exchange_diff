using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Int16Data : ComponentData<short>
	{
		public override ushort TypeId
		{
			get
			{
				return Int16Data.typeId;
			}
			set
			{
				Int16Data.typeId = value;
			}
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write(base.Data);
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			base.Data = reader.ReadInt16();
		}

		public override ICustomSerializable BuildObject()
		{
			return new Int16Data();
		}

		private static ushort typeId;
	}
}
