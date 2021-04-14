using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal class LogTransactionInformationForTestPurposes : ILogTransactionInformation
	{
		public LogTransactionInformationForTestPurposes()
		{
		}

		public LogTransactionInformationForTestPurposes(byte bufferLength)
		{
			this.buffer = new byte[(int)bufferLength];
			for (byte b = 0; b < bufferLength; b += 1)
			{
				this.buffer[(int)b] = b;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Test block:\nbuffer: ");
			foreach (byte value in this.buffer)
			{
				stringBuilder.Append(value);
			}
			stringBuilder.Append('\n');
			return stringBuilder.ToString();
		}

		public byte Type()
		{
			return 1;
		}

		public int Serialize(byte[] buffer, int offset)
		{
			int num = offset;
			if (buffer != null)
			{
				buffer[offset] = this.Type();
			}
			offset++;
			if (buffer != null)
			{
				buffer[offset] = (byte)this.buffer.Length;
			}
			offset++;
			if (buffer != null)
			{
				this.buffer.CopyTo(buffer, offset);
			}
			offset += this.buffer.Length;
			return offset - num;
		}

		public void Parse(byte[] buffer, ref int offset)
		{
			byte b = buffer[offset++];
			int num = (int)buffer[offset++];
			this.buffer = new byte[num];
			Array.Copy(buffer, offset, this.buffer, 0, num);
			offset += num;
		}

		private byte[] buffer;
	}
}
