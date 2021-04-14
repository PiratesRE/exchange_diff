using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Autodiscover.ConfigurationSettings
{
	internal class SiteCostComparer<T> : IComparer<T> where T : Service
	{
		public SiteCostComparer(ServiceTopology serviceTopology, Site source)
		{
			this.serviceTopology = serviceTopology;
			this.mbxServerSite = source;
		}

		public int Compare(T x, T y)
		{
			if (this.mbxServerSite == null)
			{
				return 0;
			}
			int maxValue;
			if (!this.serviceTopology.TryGetConnectionCost(this.mbxServerSite, x.Site, out maxValue, "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\SiteCostComparer.cs", "Compare", 68))
			{
				maxValue = int.MaxValue;
			}
			int maxValue2;
			if (!this.serviceTopology.TryGetConnectionCost(this.mbxServerSite, y.Site, out maxValue2, "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\SiteCostComparer.cs", "Compare", 75))
			{
				maxValue2 = int.MaxValue;
			}
			if (maxValue < maxValue2)
			{
				return -1;
			}
			if (maxValue > maxValue2)
			{
				return 1;
			}
			return 0;
		}

		private ServiceTopology serviceTopology;

		private Site mbxServerSite;
	}
}
