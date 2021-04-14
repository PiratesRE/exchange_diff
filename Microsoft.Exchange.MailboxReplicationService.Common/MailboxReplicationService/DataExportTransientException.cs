using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class DataExportTransientException : MapiFxProxyRetryableException
	{
		public DataExportTransientException(Exception inner) : base(inner)
		{
		}
	}
}
