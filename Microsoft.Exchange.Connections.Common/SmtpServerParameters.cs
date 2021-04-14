using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public sealed class SmtpServerParameters : ServerParameters
	{
		public SmtpServerParameters(string server, int port) : base(server, port)
		{
		}

		public SmtpServerParameters(string server) : base(server, 465)
		{
		}

		private const int DefaultPort = 465;
	}
}
