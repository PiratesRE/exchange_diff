using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class EnhancedDnsTargetHost : TargetHost
	{
		public EnhancedDnsTargetHost(string targetName, List<IPAddress> addresses, TimeSpan timeToLive, ushort port) : base(targetName, addresses, timeToLive)
		{
			this.port = port;
		}

		public ushort Port
		{
			get
			{
				return this.port;
			}
		}

		private readonly ushort port;
	}
}
