using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionObjectDeleted : MapiException
	{
		public ExExceptionObjectDeleted(LID lid, string message) : base(lid, message, ErrorCodeValue.ObjectDeleted)
		{
		}

		public ExExceptionObjectDeleted(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.ObjectDeleted, innerException)
		{
		}
	}
}
