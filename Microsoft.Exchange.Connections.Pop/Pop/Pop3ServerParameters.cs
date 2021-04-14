using System;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Pop
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public sealed class Pop3ServerParameters : ServerParameters
	{
		public Pop3ServerParameters(string server, int port) : base(server, port)
		{
		}

		public Pop3ServerParameters(string server) : base(server, 110)
		{
		}
	}
}
