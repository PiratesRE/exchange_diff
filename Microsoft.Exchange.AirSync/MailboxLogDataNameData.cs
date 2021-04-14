using System;
using System.IO;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class MailboxLogDataNameData : ComponentData<MailboxLogDataName>
	{
		public MailboxLogDataNameData()
		{
		}

		public MailboxLogDataNameData(MailboxLogDataName data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return MailboxLogDataNameData.typeId;
			}
			set
			{
				MailboxLogDataNameData.typeId = value;
			}
		}

		public override ICustomSerializable BuildObject()
		{
			return new MailboxLogDataNameData();
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			base.Data = (MailboxLogDataName)reader.ReadInt32();
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write((int)base.Data);
		}

		private static ushort typeId;
	}
}
