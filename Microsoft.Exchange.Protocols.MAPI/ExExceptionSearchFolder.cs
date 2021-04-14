using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionSearchFolder : MapiException
	{
		public ExExceptionSearchFolder(LID lid, string message) : base(lid, message, ErrorCodeValue.SearchFolder)
		{
		}

		public ExExceptionSearchFolder(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.SearchFolder, innerException)
		{
		}
	}
}
