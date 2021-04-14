using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolGetUploadedIDsResponseMessage : DataMessageBase
	{
		public FxProxyPoolGetUploadedIDsResponseMessage(List<byte[]> entryIDs)
		{
			this.entryIDs = entryIDs;
		}

		private FxProxyPoolGetUploadedIDsResponseMessage(byte[] blob)
		{
			this.entryIDs = new List<byte[]>();
			using (MemoryStream memoryStream = new MemoryStream(blob))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					int num = CommonUtils.ReadInt(binaryReader);
					for (int i = 0; i < num; i++)
					{
						this.entryIDs.Add(CommonUtils.ReadBlob(binaryReader));
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
					DataMessageOpcode.FxProxyPoolGetUploadedIDsResponse
				};
			}
		}

		public List<byte[]> EntryIDs
		{
			get
			{
				return this.entryIDs;
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return new FxProxyPoolGetUploadedIDsResponseMessage(data);
		}

		protected override int GetSizeInternal()
		{
			int num = 0;
			foreach (byte[] array in this.entryIDs)
			{
				num += ((array != null) ? array.Length : 0);
			}
			return num;
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = DataMessageOpcode.FxProxyPoolGetUploadedIDsResponse;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(this.entryIDs.Count);
					foreach (byte[] blob in this.entryIDs)
					{
						CommonUtils.WriteBlob(binaryWriter, blob);
					}
					binaryWriter.Flush();
					data = memoryStream.ToArray();
				}
			}
		}

		private List<byte[]> entryIDs;
	}
}
