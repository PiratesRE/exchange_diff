using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Autodiscover.ConfigurationCache
{
	internal class ClientAccessArrayCache : SimpleConfigCache<ClientAccessArray, ServerId>
	{
		protected override string[] KeysFromConfig(ClientAccessArray array)
		{
			List<string> list = new List<string>(base.KeysFromConfig(array));
			if (!string.IsNullOrEmpty(array.ExchangeLegacyDN))
			{
				list.Add(array.ExchangeLegacyDN);
			}
			return list.ToArray();
		}

		protected override string KeyFromSourceObject(ServerId id)
		{
			return id.Key;
		}
	}
}
