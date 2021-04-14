using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class ProxyAddressCollectionComparer : IEqualityComparer<ProxyAddressCollection>
	{
		public bool Equals(ProxyAddressCollection x, ProxyAddressCollection y)
		{
			if (x == y)
			{
				return true;
			}
			foreach (ProxyAddress item in x)
			{
				if (y.Contains(item))
				{
					return true;
				}
			}
			return false;
		}

		public int GetHashCode(ProxyAddressCollection x)
		{
			int num = 0;
			foreach (ProxyAddress proxyAddress in x)
			{
				num ^= proxyAddress.GetHashCode();
			}
			return num;
		}
	}
}
