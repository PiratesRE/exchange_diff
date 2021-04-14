using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolGetFolderDataResponseMessage : DataMessageBase
	{
		public FxProxyPoolGetFolderDataResponseMessage(EntryIdMap<byte[]> folderData)
		{
			this.folderData = folderData;
		}

		private FxProxyPoolGetFolderDataResponseMessage(byte[] blob)
		{
			this.folderData = new EntryIdMap<byte[]>();
			using (MemoryStream memoryStream = new MemoryStream(blob))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					int num = CommonUtils.ReadInt(binaryReader);
					for (int i = 0; i < num; i++)
					{
						byte[] array = CommonUtils.ReadBlob(binaryReader);
						byte[] value = CommonUtils.ReadBlob(binaryReader);
						if (array == null || this.folderData.ContainsKey(array))
						{
							throw new InputDataIsInvalidPermanentException();
						}
						this.folderData.Add(array, value);
					}
				}
			}
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.FxProxyPoolGetFolderDataResponse
				};
			}
		}

		public EntryIdMap<byte[]> FolderData
		{
			get
			{
				return this.folderData;
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return new FxProxyPoolGetFolderDataResponseMessage(data);
		}

		protected override int GetSizeInternal()
		{
			int num = 0;
			foreach (KeyValuePair<byte[], byte[]> keyValuePair in this.folderData)
			{
				num += ((keyValuePair.Key != null) ? keyValuePair.Key.Length : 0) + ((keyValuePair.Value != null) ? keyValuePair.Value.Length : 0);
			}
			return num;
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = DataMessageOpcode.FxProxyPoolGetFolderDataResponse;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(this.folderData.Count);
					foreach (KeyValuePair<byte[], byte[]> keyValuePair in this.folderData)
					{
						CommonUtils.WriteBlob(binaryWriter, keyValuePair.Key);
						CommonUtils.WriteBlob(binaryWriter, keyValuePair.Value);
					}
					binaryWriter.Flush();
					data = memoryStream.ToArray();
				}
			}
		}

		private EntryIdMap<byte[]> folderData;
	}
}
