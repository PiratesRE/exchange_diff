using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	internal class LogTransactionInformationAdmin : ILogTransactionInformation
	{
		public LogTransactionInformationAdmin()
		{
		}

		public LogTransactionInformationAdmin(AdminMethod methodId)
		{
			this.methodId = methodId;
		}

		public AdminMethod MethodId
		{
			get
			{
				return this.methodId;
			}
		}

		public override string ToString()
		{
			return string.Format("Admin RPC:\nIdentifier: {0}\n", this.methodId);
		}

		public byte Type()
		{
			return 3;
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
				buffer[offset] = (byte)this.methodId;
			}
			offset++;
			return offset - num;
		}

		public void Parse(byte[] buffer, ref int offset)
		{
			byte b = buffer[offset++];
			this.methodId = (AdminMethod)buffer[offset++];
		}

		private AdminMethod methodId;
	}
}
