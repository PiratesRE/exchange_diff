using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GenericDictionaryData<K, RawK, V> : ComponentData<Dictionary<RawK, V>> where K : ComponentData<RawK>, new() where V : ICustomSerializable, new()
	{
		public GenericDictionaryData()
		{
		}

		public GenericDictionaryData(Dictionary<RawK, V> data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return GenericDictionaryData<K, RawK, V>.typeId;
			}
			set
			{
				GenericDictionaryData<K, RawK, V>.typeId = value;
			}
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write(base.Data == null);
			if (base.Data == null)
			{
				return;
			}
			List<RawK> data = new List<RawK>(base.Data.Keys);
			List<V> data2 = new List<V>(base.Data.Values);
			GenericListData<K, RawK> genericListData = new GenericListData<K, RawK>(data);
			GenericListData<V> genericListData2 = new GenericListData<V>(data2);
			genericListData.SerializeData(writer, componentDataPool);
			genericListData2.SerializeData(writer, componentDataPool);
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			if (reader.ReadBoolean())
			{
				base.Data = null;
				return;
			}
			GenericListData<K, RawK> genericListData = new GenericListData<K, RawK>();
			GenericListData<V> genericListData2 = new GenericListData<V>();
			genericListData.DeserializeData(reader, componentDataPool);
			genericListData2.DeserializeData(reader, componentDataPool);
			if (genericListData.Data == null)
			{
				base.Data = null;
				return;
			}
			base.Data = new Dictionary<RawK, V>(genericListData.Data.Count);
			for (int i = 0; i < genericListData.Data.Count; i++)
			{
				base.Data[genericListData.Data[i]] = genericListData2.Data[i];
			}
		}

		public override ICustomSerializable BuildObject()
		{
			return new GenericDictionaryData<K, RawK, V>();
		}

		private static ushort typeId;
	}
}
