using System;
using System.IO;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class MeetingOrganizerInfoData : ComponentData<MeetingOrganizerInfo>
	{
		public MeetingOrganizerInfoData() : base(new MeetingOrganizerInfo())
		{
		}

		public MeetingOrganizerInfoData(MeetingOrganizerInfo data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return MeetingOrganizerInfoData.typeId;
			}
			set
			{
				MeetingOrganizerInfoData.typeId = value;
			}
		}

		public override ICustomSerializable BuildObject()
		{
			return new MeetingOrganizerInfoData();
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			base.Data.DeserializeData(reader, componentDataPool);
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			base.Data.SerializeData(writer, componentDataPool);
		}

		private static ushort typeId;
	}
}
