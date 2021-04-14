using System;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal class LogTransactionInformationMapi : ILogTransactionInformation
	{
		public LogTransactionInformationMapi()
		{
		}

		public LogTransactionInformationMapi(RopId ropId)
		{
			this.ropId = ropId;
		}

		public RopId RopId
		{
			get
			{
				return this.ropId;
			}
		}

		public override string ToString()
		{
			return string.Format("MAPI RPC:\nIdentifier: {0}\n", this.ropId);
		}

		public byte Type()
		{
			return 4;
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
				buffer[offset] = (byte)this.ropId;
			}
			offset++;
			return offset - num;
		}

		public void Parse(byte[] buffer, ref int offset)
		{
			byte b = buffer[offset++];
			this.ropId = (RopId)buffer[offset++];
		}

		private RopId ropId;
	}
}
