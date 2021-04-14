using System;
using System.IO;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class MeetingOrganizerEntryData : ComponentData<MeetingOrganizerEntry>
	{
		public MeetingOrganizerEntryData() : base(new MeetingOrganizerEntry())
		{
		}

		public MeetingOrganizerEntryData(MeetingOrganizerEntry data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return MeetingOrganizerEntryData.typeId;
			}
			set
			{
				MeetingOrganizerEntryData.typeId = value;
			}
		}

		public override ICustomSerializable BuildObject()
		{
			return new MeetingOrganizerEntryData();
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
