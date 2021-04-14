using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ComponentData<DataType> : ICustomSerializableBuilder, ICustomSerializable
	{
		protected ComponentData()
		{
		}

		protected ComponentData(DataType data)
		{
			this.data = data;
		}

		public DataType Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		public abstract ushort TypeId { get; set; }

		public ComponentData<DataType> Bind(DataType data)
		{
			this.data = data;
			return this;
		}

		public abstract ICustomSerializable BuildObject();

		public abstract void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool);

		public abstract void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool);

		private DataType data;
	}
}
