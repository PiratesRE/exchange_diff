using System;
using System.Net;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal sealed class StsUtil
	{
		private StsUtil()
		{
		}

		public static bool IsValidSenderIP(IPAddress senderIP)
		{
			return senderIP != null && !senderIP.Equals(IPAddress.Any) && !senderIP.Equals(IPAddress.Broadcast) && !senderIP.Equals(IPAddress.IPv6Any) && !senderIP.Equals(IPAddress.IPv6Loopback) && !senderIP.Equals(IPAddress.IPv6None) && !senderIP.Equals(IPAddress.Loopback) && !senderIP.Equals(IPAddress.None);
		}

		public static readonly DateTime NoAbsoluteExpiration = DateTime.MaxValue;

		public static readonly TimeSpan NoSlidingExpiration = TimeSpan.Zero;
	}
}
