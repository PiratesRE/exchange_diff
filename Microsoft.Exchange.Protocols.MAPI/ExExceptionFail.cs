using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionFail : MapiException
	{
		public ExExceptionFail(LID lid, string message) : base(lid, message, ErrorCodeValue.CallFailed)
		{
		}

		public ExExceptionFail(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.CallFailed, innerException)
		{
		}
	}
}
