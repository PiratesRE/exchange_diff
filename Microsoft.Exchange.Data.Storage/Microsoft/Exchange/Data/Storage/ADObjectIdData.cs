using System;
using System.IO;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ADObjectIdData : ComponentData<ADObjectId>
	{
		public ADObjectIdData()
		{
		}

		public ADObjectIdData(ADObjectId data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return ADObjectIdData.typeId;
			}
			set
			{
				ADObjectIdData.typeId = value;
			}
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write(base.Data == null);
			if (base.Data == null)
			{
				return;
			}
			componentDataPool.GetByteArrayInstance().Bind(base.Data.GetBytes()).SerializeData(writer, componentDataPool);
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			if (reader.ReadBoolean())
			{
				base.Data = null;
				return;
			}
			ByteArrayData byteArrayInstance = componentDataPool.GetByteArrayInstance();
			byteArrayInstance.DeserializeData(reader, componentDataPool);
			if (byteArrayInstance.Data == null)
			{
				base.Data = null;
				return;
			}
			base.Data = new ADObjectId(byteArrayInstance.Data);
		}

		public override ICustomSerializable BuildObject()
		{
			return new ADObjectIdData();
		}

		private static ushort typeId;
	}
}
