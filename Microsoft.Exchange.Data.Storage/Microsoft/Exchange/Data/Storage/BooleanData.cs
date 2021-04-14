using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class BooleanData : ComponentData<bool>
	{
		public BooleanData()
		{
		}

		public BooleanData(bool data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return BooleanData.typeId;
			}
			set
			{
				BooleanData.typeId = value;
			}
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write(base.Data);
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			base.Data = reader.ReadBoolean();
		}

		public override ICustomSerializable BuildObject()
		{
			return new BooleanData();
		}

		private static ushort typeId;
	}
}
