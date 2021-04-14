using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NullableData<T, RawT> : ComponentData<RawT?> where T : ComponentData<RawT>, new() where RawT : struct
	{
		public NullableData()
		{
		}

		public NullableData(RawT? data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return NullableData<T, RawT>.typeId;
			}
			set
			{
				NullableData<T, RawT>.typeId = value;
			}
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write(base.Data == null);
			if (base.Data == null)
			{
				return;
			}
			RawT value = base.Data.Value;
			this.serializableData.Bind(value);
			this.serializableData.SerializeData(writer, componentDataPool);
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			if (reader.ReadBoolean())
			{
				base.Data = null;
				return;
			}
			this.serializableData.DeserializeData(reader, componentDataPool);
			base.Data = new RawT?(this.serializableData.Data);
		}

		public override ICustomSerializable BuildObject()
		{
			return new NullableData<T, RawT>();
		}

		private static ushort typeId;

		private T serializableData = ObjectBuildHelper<T>.Build();
	}
}
