using System;
using Microsoft.Exchange.Conversion;

namespace Microsoft.Exchange.Diagnostics
{
	public class ReconciliationMessageProcessingDataEvent : IPerfEventData
	{
		public ReconciliationMessageProcessingDataEvent()
		{
		}

		public ReconciliationMessageProcessingDataEvent(string mailbox, DateTime sentTime, string messageId)
		{
			this.mailbox = mailbox;
			this.sentTime = sentTime.ToFileTime();
			this.messageId = messageId;
		}

		public void FromBytes(byte[] data)
		{
			int num = 0;
			this.sentTime = BitConverter.ToInt64(data, num);
			num += 8;
			this.messageId = ExBitConverter.ReadAsciiString(data, num);
			num += this.messageId.Length + 1;
			this.mailbox = ExBitConverter.ReadAsciiString(data, num);
		}

		public byte[] ToBytes()
		{
			byte[] array = new byte[this.mailbox.Length + this.messageId.Length + 10];
			int num = 0;
			num += ExBitConverter.Write(this.sentTime, array, num);
			num += ExBitConverter.Write(this.messageId, false, array, num);
			num += ExBitConverter.Write(this.mailbox, false, array, num);
			return array;
		}

		public string[] ToCsvRecord()
		{
			return new string[]
			{
				DateTime.FromFileTimeUtc(this.sentTime).ToString(),
				this.messageId.ToString(),
				this.mailbox.ToString()
			};
		}

		private long sentTime;

		private string mailbox;

		private string messageId;
	}
}
