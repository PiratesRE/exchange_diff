using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DateTimeData : ComponentData<ExDateTime>
	{
		public DateTimeData()
		{
		}

		public DateTimeData(ExDateTime data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return DateTimeData.typeId;
			}
			set
			{
				DateTimeData.typeId = value;
			}
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			long value = base.Data.ToBinary();
			writer.Write(value);
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			long dateData = reader.ReadInt64();
			base.Data = ExDateTime.FromBinary(dateData);
		}

		public override ICustomSerializable BuildObject()
		{
			return new DateTimeData();
		}

		private static ushort typeId;
	}
}
