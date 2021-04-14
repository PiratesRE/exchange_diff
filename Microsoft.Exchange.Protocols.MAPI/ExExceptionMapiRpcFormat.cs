using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionMapiRpcFormat : MapiException
	{
		public ExExceptionMapiRpcFormat(LID lid, string message) : base(lid, message, ErrorCodeValue.RpcFormat)
		{
		}

		public ExExceptionMapiRpcFormat(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.RpcFormat, innerException)
		{
		}
	}
}
