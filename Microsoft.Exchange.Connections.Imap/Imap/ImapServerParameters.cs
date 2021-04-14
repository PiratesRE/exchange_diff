using System;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Imap
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public sealed class ImapServerParameters : ServerParameters
	{
		public ImapServerParameters(string server, int port) : base(server, port)
		{
		}

		public ImapServerParameters(string server) : base(server, 143)
		{
		}
	}
}
