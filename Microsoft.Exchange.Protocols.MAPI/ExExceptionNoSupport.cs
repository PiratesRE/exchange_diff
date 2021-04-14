using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionNoSupport : MapiException
	{
		public ExExceptionNoSupport(LID lid, string message) : base(lid, message, ErrorCodeValue.NotSupported)
		{
		}

		public ExExceptionNoSupport(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.NotSupported, innerException)
		{
		}
	}
}
