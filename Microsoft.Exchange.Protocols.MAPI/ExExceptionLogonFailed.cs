using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionLogonFailed : MapiException
	{
		public ExExceptionLogonFailed(LID lid, string message) : base(lid, message, ErrorCodeValue.LogonFailed)
		{
		}

		public ExExceptionLogonFailed(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.LogonFailed, innerException)
		{
		}
	}
}
