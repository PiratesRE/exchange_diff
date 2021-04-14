using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionVersionMismatch : MapiException
	{
		public ExExceptionVersionMismatch(LID lid, string message) : base(lid, message, ErrorCodeValue.VersionMismatch)
		{
		}

		public ExExceptionVersionMismatch(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.VersionMismatch, innerException)
		{
		}
	}
}
