using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Autodiscover.ConfigurationCache
{
	internal class ServerCache : SimpleConfigCache<Server, ServerId>
	{
		protected override string[] KeysFromConfig(Server server)
		{
			List<string> list = new List<string>(base.KeysFromConfig(server));
			if (!string.IsNullOrEmpty(server.ExchangeLegacyDN))
			{
				list.Add(server.ExchangeLegacyDN);
			}
			return list.ToArray();
		}

		protected override string KeyFromSourceObject(ServerId id)
		{
			return id.Key;
		}
	}
}
