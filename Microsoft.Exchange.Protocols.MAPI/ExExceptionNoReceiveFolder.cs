using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionNoReceiveFolder : MapiException
	{
		public ExExceptionNoReceiveFolder(LID lid, string message) : base(lid, message, ErrorCodeValue.NoReceiveFolder)
		{
		}

		public ExExceptionNoReceiveFolder(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.NoReceiveFolder, innerException)
		{
		}
	}
}
