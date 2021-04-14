using System;
using System.Text;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.Server.Storage.MapiDisp
{
	public sealed class ParserConnectionInformation : IConnectionInformation
	{
		public bool ClientSupportsBackoffResult
		{
			get
			{
				return true;
			}
		}

		public bool ClientSupportsBufferTooSmallBreakup
		{
			get
			{
				return false;
			}
		}

		public ushort SessionId
		{
			get
			{
				return 0;
			}
		}

		public Encoding String8Encoding
		{
			get
			{
				return Encoding.ASCII;
			}
		}
	}
}
