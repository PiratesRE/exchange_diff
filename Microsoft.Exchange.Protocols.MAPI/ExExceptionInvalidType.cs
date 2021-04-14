using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionInvalidType : MapiException
	{
		public ExExceptionInvalidType(LID lid, string message) : base(lid, message, ErrorCodeValue.InvalidType)
		{
		}

		public ExExceptionInvalidType(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.InvalidType, innerException)
		{
		}
	}
}
