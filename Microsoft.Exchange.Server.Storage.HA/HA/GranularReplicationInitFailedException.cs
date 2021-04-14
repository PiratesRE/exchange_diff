using System;

namespace Microsoft.Exchange.Server.Storage.HA
{
	internal class GranularReplicationInitFailedException : Exception
	{
		public GranularReplicationInitFailedException(string message) : base(message)
		{
		}
	}
}
