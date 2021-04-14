using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionNotFound : MapiException
	{
		public ExExceptionNotFound(LID lid, string message) : base(lid, message, ErrorCodeValue.NotFound)
		{
		}

		public ExExceptionNotFound(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.NotFound, innerException)
		{
		}
	}
}
