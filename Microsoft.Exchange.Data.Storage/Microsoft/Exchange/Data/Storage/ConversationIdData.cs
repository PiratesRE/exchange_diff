using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationIdData : ComponentData<ConversationId>
	{
		public ConversationIdData()
		{
		}

		public ConversationIdData(ConversationId data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return ConversationIdData.typeId;
			}
			set
			{
				ConversationIdData.typeId = value;
			}
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			byte[] data = null;
			if (base.Data != null)
			{
				data = base.Data.GetBytes();
			}
			componentDataPool.GetByteArrayInstance().Bind(data).SerializeData(writer, componentDataPool);
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			ByteArrayData byteArrayInstance = componentDataPool.GetByteArrayInstance();
			byteArrayInstance.DeserializeData(reader, componentDataPool);
			if (byteArrayInstance.Data == null)
			{
				base.Data = null;
				return;
			}
			base.Data = ConversationId.Create(byteArrayInstance.Data);
		}

		public override ICustomSerializable BuildObject()
		{
			return new ConversationIdData();
		}

		private static ushort typeId;
	}
}
