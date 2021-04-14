using System;
using System.IO;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class DeviceBehaviorData : ComponentData<DeviceBehavior>
	{
		public DeviceBehaviorData() : base(new DeviceBehavior())
		{
		}

		public DeviceBehaviorData(DeviceBehavior data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return DeviceBehaviorData.typeId;
			}
			set
			{
				DeviceBehaviorData.typeId = value;
			}
		}

		public override ICustomSerializable BuildObject()
		{
			return new DeviceBehaviorData();
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
