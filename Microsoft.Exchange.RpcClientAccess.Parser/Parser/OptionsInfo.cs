using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal struct OptionsInfo
	{
		public OptionsInfo(byte[] messageData, byte[] recipientData, string helpFileName)
		{
			this.messageData = OptionsInfo.VersionChecked(messageData);
			this.recipientData = OptionsInfo.VersionChecked(recipientData);
			this.helpFileName = helpFileName;
		}

		public byte[] GetBytes()
		{
			byte[] array = new byte[300 + this.messageData.Count + this.recipientData.Count];
			using (BufferWriter bufferWriter = new BufferWriter(array))
			{
				bufferWriter.WriteInt32(this.messageData.Count);
				bufferWriter.WriteInt32(this.recipientData.Count);
				if (this.helpFileName != null)
				{
					bufferWriter.WriteAsciiString("Microsoft MDB ", StringFlags.None);
					int byteCount = CTSGlobals.AsciiEncoding.GetByteCount(this.helpFileName);
					int num = 292 - "Microsoft MDB ".Length - 1;
					string value;
					if (byteCount < num)
					{
						value = this.helpFileName;
					}
					else
					{
						value = this.helpFileName.Substring(num);
					}
					bufferWriter.WriteAsciiString(value, StringFlags.None);
				}
				bufferWriter.Position = (long)OptionsInfo.OptionsInfoHeaderSize;
				bufferWriter.WriteBytes(this.messageData.Array, this.messageData.Offset, this.messageData.Count);
				bufferWriter.WriteBytes(this.recipientData.Array, this.recipientData.Offset, this.recipientData.Count);
			}
			return array;
		}

		private static ArraySegment<byte> VersionChecked(byte[] data)
		{
			if (data == null)
			{
				return default(ArraySegment<byte>);
			}
			if (data.Length < 4)
			{
				throw new RopExecutionException(string.Format("OptionsInfo's data is corrupted due to the data is too small. Length = {0}.", data.Length), (ErrorCode)2147746064U);
			}
			if (data[0] != 2 || data[1] != 0 || data[2] != 0 || data[3] != 0)
			{
				throw new RopExecutionException(string.Format("OptionsInfo's data is corrupted due to the mismatched version. Version = {0}.{1}.{2}.{3}.", new object[]
				{
					data[3],
					data[2],
					data[1],
					data[0]
				}), (ErrorCode)2147746064U);
			}
			return new ArraySegment<byte>(data, 4, data.Length - 4);
		}

		private const string ComponentNamePrefix = "Microsoft MDB ";

		internal const int MaxComponentNameLength = 292;

		private readonly ArraySegment<byte> messageData;

		private readonly ArraySegment<byte> recipientData;

		private readonly string helpFileName;

		internal static int OptionsInfoHeaderSize = 300;
	}
}
