using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GenericListData<T, RawT> : ComponentData<List<RawT>> where T : ComponentData<RawT>, new()
	{
		public GenericListData()
		{
		}

		public GenericListData(List<RawT> data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return GenericListData<T, RawT>.typeId;
			}
			set
			{
				GenericListData<T, RawT>.typeId = value;
			}
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write(base.Data == null);
			if (base.Data == null)
			{
				return;
			}
			writer.Write(base.Data.Count);
			for (int i = 0; i < base.Data.Count; i++)
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
			int num2 = num;
			try
			{
				bool flag = false;
				ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(2902863165U, ref flag);
				if (flag)
				{
					num2 = int.MaxValue;
					throw new OutOfMemoryException();
				}
				base.Data = new List<RawT>(num2);
			}
			catch (OutOfMemoryException ex)
			{
				ex.Data["GenericListDataDeserializationCount"] = num2;
				throw;
			}
			for (int i = 0; i < num; i++)
			{
				this.serializableData.DeserializeData(reader, componentDataPool);
				base.Data.Add(this.serializableData.Data);
			}
		}

		public override ICustomSerializable BuildObject()
		{
			return new GenericListData<T, RawT>();
		}

		private static ushort typeId;

		private T serializableData = ObjectBuildHelper<T>.Build();
	}
}
