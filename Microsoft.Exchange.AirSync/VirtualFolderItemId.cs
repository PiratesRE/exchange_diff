using System;
using System.IO;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class VirtualFolderItemId : ISyncItemId, ICustomSerializableBuilder, ICustomSerializable
	{
		public VirtualFolderItemId()
		{
		}

		public VirtualFolderItemId(string nativeId)
		{
			this.nativeId = nativeId;
		}

		public ushort TypeId
		{
			get
			{
				return VirtualFolderItemId.typeId;
			}
			set
			{
				VirtualFolderItemId.typeId = value;
			}
		}

		public object NativeId
		{
			get
			{
				return this.nativeId;
			}
		}

		public void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			StringData stringDataInstance = componentDataPool.GetStringDataInstance();
			stringDataInstance.DeserializeData(reader, componentDataPool);
			this.nativeId = stringDataInstance.Data;
		}

		public void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			componentDataPool.GetStringDataInstance().Bind(this.nativeId).SerializeData(writer, componentDataPool);
		}

		public ICustomSerializable BuildObject()
		{
			return new VirtualFolderItemId(null);
		}

		private static ushort typeId;

		private string nativeId;
	}
}
