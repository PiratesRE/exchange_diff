using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionInvalidObject : MapiException
	{
		public ExExceptionInvalidObject(LID lid, string message) : base(lid, message, ErrorCodeValue.InvalidObject)
		{
		}

		public ExExceptionInvalidObject(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.InvalidObject, innerException)
		{
		}
	}
}
