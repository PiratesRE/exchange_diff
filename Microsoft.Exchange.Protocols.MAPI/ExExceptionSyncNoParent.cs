using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionSyncNoParent : MapiException
	{
		public ExExceptionSyncNoParent(LID lid, string message) : base(lid, message, ErrorCodeValue.SyncNoParent)
		{
		}

		public ExExceptionSyncNoParent(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.SyncNoParent, innerException)
		{
		}
	}
}
