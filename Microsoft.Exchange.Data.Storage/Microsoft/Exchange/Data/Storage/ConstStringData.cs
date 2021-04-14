using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConstStringData : ComponentData<string>
	{
		public ConstStringData()
		{
		}

		public ConstStringData(string data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return ConstStringData.typeId;
			}
			set
			{
				ConstStringData.typeId = value;
			}
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write(base.Data == null);
			if (base.Data == null)
			{
				return;
			}
			writer.Write(base.Data);
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			if (reader.ReadBoolean())
			{
				base.Data = null;
				return;
			}
			base.Data = StaticStringPool.Instance.GetData(reader, componentDataPool);
			if (base.Data == null)
			{
				componentDataPool.ConstStringDataReader.BaseStream.Seek(0L, SeekOrigin.Begin);
				base.Data = componentDataPool.ConstStringDataReader.ReadString();
			}
		}

		public override ICustomSerializable BuildObject()
		{
			return new ConstStringData();
		}

		private static ushort typeId;
	}
}
