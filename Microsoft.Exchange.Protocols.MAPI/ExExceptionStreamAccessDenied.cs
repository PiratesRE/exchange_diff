using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionStreamAccessDenied : MapiException
	{
		public ExExceptionStreamAccessDenied(LID lid, string message) : base(lid, message, ErrorCodeValue.StreamAccessDenied)
		{
		}

		public ExExceptionStreamAccessDenied(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.StreamAccessDenied, innerException)
		{
		}
	}
}
