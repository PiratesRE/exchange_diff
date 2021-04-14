using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Autodiscover.ConfigurationCache
{
	internal class ADServiceConnectionPointCache : SimpleConfigCache<ADServiceConnectionPoint, string>
	{
		internal override IEnumerable<ADServiceConnectionPoint> StartSearch(IConfigurationSession session)
		{
			return session.FindPaged<ADServiceConnectionPoint>(null, QueryScope.SubTree, new TextFilter(ADServiceConnectionPointSchema.Keywords, "77378F46-2C66-4aa9-A6A6-3E7A48B19596", MatchOptions.FullString, MatchFlags.IgnoreCase), null, 0);
		}

		protected override string[] KeysFromConfig(ADServiceConnectionPoint adScp)
		{
			ServerId src = new ServerId(adScp.Id.Parent.Parent.Parent);
			Server configFromSourceObject = ServerConfigurationCache.Singleton.ServerCache.GetConfigFromSourceObject(src);
			if (configFromSourceObject == null)
			{
				return new string[0];
			}
			return new string[]
			{
				configFromSourceObject.Fqdn
			};
		}

		private const string UrlScpGuidString = "77378F46-2C66-4aa9-A6A6-3E7A48B19596";
	}
}
