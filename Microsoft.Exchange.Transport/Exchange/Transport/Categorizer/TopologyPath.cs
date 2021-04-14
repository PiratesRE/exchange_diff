using System;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal abstract class TopologyPath
	{
		public bool Optimal
		{
			get
			{
				return this.optimal;
			}
			set
			{
				this.optimal = value;
			}
		}

		public abstract int TotalCost { get; }

		public abstract void ReplaceIfBetter(TopologyPath newPrePath, ITopologySiteLink newLink, DateTime timestamp);

		private bool optimal;
	}
}
