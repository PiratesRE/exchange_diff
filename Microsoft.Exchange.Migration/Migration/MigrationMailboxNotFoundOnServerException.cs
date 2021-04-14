using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationMailboxNotFoundOnServerException : Exception
	{
		public MigrationMailboxNotFoundOnServerException(string hostServer, string expectedServer, string mailboxName) : base(ServerStrings.MigrationMailboxNotFoundOnServerError(mailboxName, expectedServer, hostServer))
		{
			this.hostServer = hostServer;
		}

		public string HostServer
		{
			get
			{
				return this.hostServer;
			}
		}

		private string hostServer;
	}
}
