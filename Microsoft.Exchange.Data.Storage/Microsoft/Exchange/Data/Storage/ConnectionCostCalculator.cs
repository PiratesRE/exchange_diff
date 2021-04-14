using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ConnectionCostCalculator
	{
		internal ConnectionCostCalculator(int allSitesCount)
		{
			this.allSitesCount = allSitesCount;
		}

		internal bool TryGetConnectionCost(Site site1, Site site2, out int cost)
		{
			if (site1 == null)
			{
				throw new ArgumentNullException("site1");
			}
			if (site2 == null)
			{
				throw new ArgumentNullException("site2");
			}
			int? num;
			if (!this.TryGetKnownCost(site1, site2, out num))
			{
				return this.TryCalculateConnectionCost(site1, site2, out cost);
			}
			if (num != null)
			{
				cost = num.Value;
				return true;
			}
			cost = int.MaxValue;
			return false;
		}

		private bool TryCalculateConnectionCost(Site sourceSite, Site destinationSite, out int sourceToDestinationCost)
		{
			sourceToDestinationCost = int.MaxValue;
			bool flag = false;
			Dictionary<Site, int> dictionary = new Dictionary<Site, int>();
			dictionary[sourceSite] = 0;
			if (sourceSite.Equals(destinationSite))
			{
				sourceToDestinationCost = 0;
				flag = true;
			}
			else
			{
				while (this.allSitesCount > dictionary.Count)
				{
					int num = 0;
					Site site = null;
					foreach (KeyValuePair<Site, int> keyValuePair in dictionary)
					{
						Site key = keyValuePair.Key;
						foreach (SiteLink siteLink in key.SiteLinks)
						{
							foreach (Site site2 in siteLink.Sites)
							{
								if (!dictionary.ContainsKey(site2))
								{
									int num2 = siteLink.Cost + keyValuePair.Value;
									if (site == null || num2 < num || (num2 == num && site2.Equals(destinationSite)))
									{
										site = site2;
										num = num2;
									}
								}
							}
						}
					}
					if (site == null)
					{
						break;
					}
					dictionary[site] = num;
					if (site.Equals(destinationSite))
					{
						sourceToDestinationCost = num;
						flag = true;
						break;
					}
				}
			}
			if (dictionary.Count > 0 || !flag)
			{
				Dictionary<ConnectionCostCalculator.SitePairKey, int?> dictionary2 = this.knownCosts;
				Dictionary<ConnectionCostCalculator.SitePairKey, int?> dictionary3 = new Dictionary<ConnectionCostCalculator.SitePairKey, int?>(dictionary2);
				foreach (KeyValuePair<Site, int> keyValuePair2 in dictionary)
				{
					Site key2 = keyValuePair2.Key;
					ConnectionCostCalculator.SitePairKey key3 = new ConnectionCostCalculator.SitePairKey(sourceSite, key2);
					dictionary3[key3] = new int?(keyValuePair2.Value);
				}
				if (!flag)
				{
					ConnectionCostCalculator.SitePairKey key4 = new ConnectionCostCalculator.SitePairKey(sourceSite, destinationSite);
					dictionary3[key4] = null;
				}
				Interlocked.Exchange<Dictionary<ConnectionCostCalculator.SitePairKey, int?>>(ref this.knownCosts, dictionary3);
			}
			return flag;
		}

		private bool TryGetKnownCost(Site site1, Site site2, out int? cost)
		{
			ConnectionCostCalculator.SitePairKey key = new ConnectionCostCalculator.SitePairKey(site1, site2);
			Dictionary<ConnectionCostCalculator.SitePairKey, int?> dictionary = this.knownCosts;
			return dictionary.TryGetValue(key, out cost);
		}

		private readonly int allSitesCount;

		private Dictionary<ConnectionCostCalculator.SitePairKey, int?> knownCosts = new Dictionary<ConnectionCostCalculator.SitePairKey, int?>();

		private struct SitePairKey : IEquatable<ConnectionCostCalculator.SitePairKey>
		{
			internal SitePairKey(Site site1, Site site2)
			{
				if (site1.CompareTo(site2) < 0)
				{
					this.site1 = site1;
					this.site2 = site2;
					return;
				}
				this.site1 = site2;
				this.site2 = site1;
			}

			public bool Equals(ConnectionCostCalculator.SitePairKey sitePairKey)
			{
				return this.site1.Equals(sitePairKey.site1) && this.site2.Equals(sitePairKey.site2);
			}

			public override bool Equals(object obj)
			{
				return obj is ConnectionCostCalculator.SitePairKey && this.Equals((ConnectionCostCalculator.SitePairKey)obj);
			}

			public override int GetHashCode()
			{
				return this.site1.GetHashCode() ^ this.site2.GetHashCode();
			}

			private readonly Site site1;

			private readonly Site site2;
		}
	}
}
