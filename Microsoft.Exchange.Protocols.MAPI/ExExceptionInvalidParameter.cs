using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionInvalidParameter : MapiException
	{
		public ExExceptionInvalidParameter(LID lid, string message) : base(lid, message, ErrorCodeValue.InvalidParameter)
		{
		}

		public ExExceptionInvalidParameter(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.InvalidParameter, innerException)
		{
		}
	}
}
