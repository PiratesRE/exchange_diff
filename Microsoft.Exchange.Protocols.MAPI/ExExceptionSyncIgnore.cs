using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionSyncIgnore : MapiException
	{
		public ExExceptionSyncIgnore(LID lid, string message) : base(lid, message, ErrorCodeValue.SyncIgnore)
		{
		}

		public ExExceptionSyncIgnore(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.SyncIgnore, innerException)
		{
		}
	}
}
