using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GenericListData<T> : ComponentData<List<T>> where T : ICustomSerializable, new()
	{
		public GenericListData()
		{
		}

		public GenericListData(List<T> data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return GenericListData<T>.typeId;
			}
			set
			{
				GenericListData<T>.typeId = value;
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
				T t = base.Data[i];
				t.SerializeData(writer, componentDataPool);
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
				base.Data = new List<T>(num2);
			}
			catch (OutOfMemoryException ex)
			{
				ex.Data["GenericListDataDeserializationCount"] = num2;
				throw;
			}
			for (int i = 0; i < num; i++)
			{
				T item = ObjectBuildHelper<T>.Build();
				item.DeserializeData(reader, componentDataPool);
				base.Data.Add(item);
			}
		}

		public override ICustomSerializable BuildObject()
		{
			return new GenericListData<T>();
		}

		internal const uint LidChangeListSizeForOutOfMemoryException = 2902863165U;

		public const string GenericListDataDeserializationCount = "GenericListDataDeserializationCount";

		private static ushort typeId;
	}
}
