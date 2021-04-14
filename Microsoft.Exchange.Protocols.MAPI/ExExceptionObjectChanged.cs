using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionObjectChanged : MapiException
	{
		public ExExceptionObjectChanged(LID lid, string message) : base(lid, message, ErrorCodeValue.ObjectChanged)
		{
		}

		public ExExceptionObjectChanged(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.ObjectChanged, innerException)
		{
		}
	}
}
