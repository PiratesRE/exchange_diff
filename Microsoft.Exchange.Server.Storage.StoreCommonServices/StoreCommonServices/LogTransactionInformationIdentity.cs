using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal class LogTransactionInformationIdentity : ILogTransactionInformation
	{
		public LogTransactionInformationIdentity()
		{
		}

		public LogTransactionInformationIdentity(int mailboxNumber, ClientType clientType)
		{
			this.mailboxNumber = mailboxNumber;
			this.clientType = clientType;
		}

		public int MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public ClientType ClientType
		{
			get
			{
				return this.clientType;
			}
		}

		public override string ToString()
		{
			return string.Format("Identity block:\nmailbox number: {0}\nclientType: {1}\n", this.mailboxNumber, this.clientType);
		}

		public byte Type()
		{
			return 2;
		}

		public int Serialize(byte[] buffer, int offset)
		{
			int num = offset;
			if (buffer != null)
			{
				buffer[offset] = this.Type();
			}
			offset++;
			offset += SerializedValue.SerializeInt32(this.mailboxNumber, buffer, offset);
			if (buffer != null)
			{
				buffer[offset] = (byte)this.clientType;
			}
			offset++;
			return offset - num;
		}

		public void Parse(byte[] buffer, ref int offset)
		{
			byte b = buffer[offset++];
			this.mailboxNumber = SerializedValue.ParseInt32(buffer, ref offset);
			this.clientType = (ClientType)buffer[offset++];
		}

		private int mailboxNumber;

		private ClientType clientType = ClientType.MaxValue;
	}
}
