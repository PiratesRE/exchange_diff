using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class DataExportPermanentException : MapiFxProxyPermanentException
	{
		public DataExportPermanentException(Exception inner) : base(inner)
		{
		}
	}
}
