using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal class SnapInAliasMap
	{
		internal static void AddAliasMappingForSnapIn(string alias, string snapinName)
		{
			if (SnapInAliasMap.snapInAliasMap.ContainsKey(alias))
			{
				throw new ArgumentException("Mapping is already provided for alias :" + alias);
			}
			SnapInAliasMap.snapInAliasMap[alias] = snapinName;
		}

		internal static string GetSnapInName(string alias)
		{
			if (SnapInAliasMap.snapInAliasMap.Count > 0 && SnapInAliasMap.snapInAliasMap.ContainsKey(alias))
			{
				return SnapInAliasMap.snapInAliasMap[alias];
			}
			return alias;
		}

		private static Dictionary<string, string> snapInAliasMap = new Dictionary<string, string>();
	}
}
