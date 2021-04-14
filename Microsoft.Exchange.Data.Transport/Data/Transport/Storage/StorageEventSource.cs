using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Data.Transport.Storage
{
	public abstract class StorageEventSource
	{
		internal StorageEventSource()
		{
		}

		public abstract void Delete(string sourceContext);

		public abstract void DeleteWithNdr(SmtpResponse response, string sourceContext);
	}
}
