using System;

namespace Microsoft.Exchange.Servicelets.GlobalLocatorCache
{
	internal class ForestRegionTuple
	{
		public ForestRegionTuple(string forestFqdn)
		{
			this.ForestFqdn = forestFqdn;
			this.Region = ForestRegionTuple.GetRegionFromForestFqdn(forestFqdn);
		}

		public string ForestFqdn { get; private set; }

		public string Region { get; private set; }

		internal static string GetRegionFromForestFqdn(string forestFqdn)
		{
			if (forestFqdn.Equals("prod.exchangelabs.com", StringComparison.OrdinalIgnoreCase))
			{
				return "nam";
			}
			return forestFqdn.Substring(0, 3).ToLower();
		}
	}
}
