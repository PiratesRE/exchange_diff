using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DerivedData<RefT> : ComponentData<RefT> where RefT : ICustomSerializableBuilder
	{
		public DerivedData()
		{
		}

		public DerivedData(RefT data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return DerivedData<RefT>.typeId;
			}
			set
			{
				DerivedData<RefT>.typeId = value;
			}
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write(base.Data == null);
			if (base.Data == null)
			{
				return;
			}
			RefT data = base.Data;
			writer.Write(data.TypeId);
			RefT data2 = base.Data;
			data2.SerializeData(writer, componentDataPool);
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			if (reader.ReadBoolean())
			{
				base.Data = default(RefT);
				return;
			}
			ushort num = reader.ReadUInt16();
			if (!SyncStateTypeFactory.DoesTypeExistWithThisId(num))
			{
				throw new CustomSerializationException(ServerStrings.ExSyncStateCorrupted("Type " + num.ToString() + " not registered."));
			}
			base.Data = (RefT)((object)SyncStateTypeFactory.GetInstance().BuildObject(num));
			RefT data = base.Data;
			data.DeserializeData(reader, componentDataPool);
		}

		public override ICustomSerializable BuildObject()
		{
			return new DerivedData<RefT>();
		}

		[Conditional("DEBUG")]
		private void AssertTypeRegisteredWithSyncStateFactory(RefT data)
		{
			if (data == null)
			{
				return;
			}
			if (!SyncStateTypeFactory.IsTypeRegistered(data))
			{
				"Type not registered.  Please call: SyncStateTypeFactory.GetInstance().RegisterBuilder(new " + data.GetType().ToString() + "()) before attempting to serialize this type through DeriveData.";
			}
		}

		private static ushort typeId;
	}
}
