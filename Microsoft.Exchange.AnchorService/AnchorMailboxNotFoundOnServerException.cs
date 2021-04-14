using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AnchorService
{
	internal class AnchorMailboxNotFoundOnServerException : LocalizedException
	{
		public AnchorMailboxNotFoundOnServerException(string hostServer, string expectedServer, string mailboxName) : base(ServerStrings.MigrationMailboxNotFoundOnServerError(mailboxName, expectedServer, hostServer))
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

		private readonly string hostServer;
	}
}
