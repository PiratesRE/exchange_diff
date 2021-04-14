using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Exchange.SenderId
{
	internal sealed class CachedSenderIdResults
	{
		public CachedSenderIdResults()
		{
			this.cachedDomains = new Dictionary<string, List<CachedSenderIdResults.IPAddressResultPair>>();
		}

		public SenderIdResult GetCachedResult(string domain, IPAddress ipAddress)
		{
			List<CachedSenderIdResults.IPAddressResultPair> list = null;
			if (this.cachedDomains.TryGetValue(domain, out list))
			{
				foreach (CachedSenderIdResults.IPAddressResultPair ipaddressResultPair in list)
				{
					if (ipaddressResultPair.IpAddress.Equals(ipAddress))
					{
						return ipaddressResultPair.SenderIdResult;
					}
				}
			}
			return null;
		}

		public void SaveResult(string domain, IPAddress ipAddress, SenderIdResult senderIdResult)
		{
			List<CachedSenderIdResults.IPAddressResultPair> list = null;
			if (!this.cachedDomains.TryGetValue(domain, out list))
			{
				list = new List<CachedSenderIdResults.IPAddressResultPair>();
				this.cachedDomains[domain] = list;
			}
			list.Add(new CachedSenderIdResults.IPAddressResultPair(ipAddress, senderIdResult));
		}

		private Dictionary<string, List<CachedSenderIdResults.IPAddressResultPair>> cachedDomains;

		private class IPAddressResultPair
		{
			public IPAddressResultPair(IPAddress ipAddress, SenderIdResult senderIdResult)
			{
				this.IpAddress = ipAddress;
				this.SenderIdResult = senderIdResult;
			}

			public readonly IPAddress IpAddress;

			public readonly SenderIdResult SenderIdResult;
		}
	}
}
