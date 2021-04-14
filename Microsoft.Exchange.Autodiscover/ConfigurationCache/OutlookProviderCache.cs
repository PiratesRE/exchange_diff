using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Autodiscover.ConfigurationCache
{
	internal sealed class OutlookProviderCache : SimpleConfigCache<OutlookProvider, string>
	{
		protected override string[] KeysFromConfig(OutlookProvider config)
		{
			return new string[]
			{
				config.Name
			};
		}

		protected override string KeyFromSourceObject(string id)
		{
			return id;
		}

		internal IEnumerable<OutlookProvider> Providers()
		{
			OutlookProvider op = base.GetConfigFromSourceObject("EXCH");
			if (op != null)
			{
				yield return op;
			}
			op = base.GetConfigFromSourceObject("EXPR");
			if (op != null)
			{
				yield return op;
			}
			foreach (OutlookProvider opc in this.cache.Values)
			{
				if (string.Compare(opc.Name, "EXCH", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(opc.Name, "EXPR", StringComparison.OrdinalIgnoreCase) != 0)
				{
					yield return opc;
				}
			}
			yield break;
		}

		internal const string ExchangeExternalAccess = "EXPR";

		internal const string ExchangeInternalAccess = "EXCH";

		internal const string ExchangeWebAccess = "WEB";
	}
}
