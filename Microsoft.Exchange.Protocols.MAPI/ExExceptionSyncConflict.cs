using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionSyncConflict : MapiException
	{
		public ExExceptionSyncConflict(LID lid, string message) : base(lid, message, ErrorCodeValue.SyncConflict)
		{
		}

		public ExExceptionSyncConflict(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.SyncConflict, innerException)
		{
		}
	}
}
