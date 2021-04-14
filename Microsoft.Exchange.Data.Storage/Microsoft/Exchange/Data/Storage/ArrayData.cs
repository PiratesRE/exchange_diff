using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ArrayData<T, RawT> : ComponentData<RawT[]> where T : ComponentData<RawT>, new()
	{
		public ArrayData()
		{
		}

		public ArrayData(RawT[] data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return ArrayData<T, RawT>.typeId;
			}
			set
			{
				ArrayData<T, RawT>.typeId = value;
			}
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write(base.Data == null);
			if (base.Data == null)
			{
				return;
			}
			writer.Write(base.Data.Length);
			for (int i = 0; i < base.Data.Length; i++)
			{
				this.serializableData.Bind(base.Data[i]);
				this.serializableData.SerializeData(writer, componentDataPool);
			}
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			if (reader.ReadBoolean())
			{
				base.Data = null;
				return;
			}
			int num = reader.ReadInt32();
			base.Data = new RawT[num];
			for (int i = 0; i < num; i++)
			{
				this.serializableData.DeserializeData(reader, componentDataPool);
				base.Data[i] = this.serializableData.Data;
			}
		}

		public override ICustomSerializable BuildObject()
		{
			return new ArrayData<T, RawT>();
		}

		private static ushort typeId;

		private T serializableData = ObjectBuildHelper<T>.Build();
	}
}
