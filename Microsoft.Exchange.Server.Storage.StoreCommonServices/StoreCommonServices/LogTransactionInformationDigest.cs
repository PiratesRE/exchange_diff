using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal class LogTransactionInformationDigest : ILogTransactionInformation
	{
		public LogTransactionInformationDigest()
		{
		}

		public LogTransactionInformationDigest(Dictionary<byte, LogTransactionInformationCollector.Counter> perLogTransactionInformationBlockTypeCounter)
		{
			this.perLogTransactionInformationBlockTypeCounter = perLogTransactionInformationBlockTypeCounter;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Digest:\n");
			foreach (KeyValuePair<byte, LogTransactionInformationCollector.Counter> keyValuePair in this.perLogTransactionInformationBlockTypeCounter)
			{
				stringBuilder.Append(string.Format("Block Type: {0}: counter {1}\n", (LogTransactionInformationBlockType)keyValuePair.Key, keyValuePair.Value.Value));
			}
			return stringBuilder.ToString();
		}

		public byte Type()
		{
			return 6;
		}

		public int Serialize(byte[] buffer, int offset)
		{
			int num = offset;
			if (buffer != null)
			{
				buffer[offset] = this.Type();
			}
			offset++;
			offset += SerializedValue.SerializeInt32(this.perLogTransactionInformationBlockTypeCounter.Count, buffer, offset);
			foreach (KeyValuePair<byte, LogTransactionInformationCollector.Counter> keyValuePair in this.perLogTransactionInformationBlockTypeCounter)
			{
				if (buffer != null)
				{
					buffer[offset] = keyValuePair.Key;
				}
				offset++;
				offset += SerializedValue.SerializeInt32(keyValuePair.Value.Value, buffer, offset);
			}
			return offset - num;
		}

		public void Parse(byte[] buffer, ref int offset)
		{
			byte b = buffer[offset++];
			int num = SerializedValue.ParseInt32(buffer, ref offset);
			this.perLogTransactionInformationBlockTypeCounter = new Dictionary<byte, LogTransactionInformationCollector.Counter>((num > 0 && num < 10) ? num : 10);
			for (int i = 0; i < num; i++)
			{
				byte key = buffer[offset++];
				LogTransactionInformationCollector.Counter counter = new LogTransactionInformationCollector.Counter();
				counter.Value = SerializedValue.ParseInt32(buffer, ref offset);
				this.perLogTransactionInformationBlockTypeCounter.Add(key, counter);
			}
		}

		private Dictionary<byte, LogTransactionInformationCollector.Counter> perLogTransactionInformationBlockTypeCounter;
	}
}
