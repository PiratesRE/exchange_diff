using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Band
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BandDataAggregator : ILoadEntityVisitor
	{
		public BandDataAggregator(Band band)
		{
			this.band = band;
		}

		public IEnumerable<BandData> BandData
		{
			get
			{
				return this.bandData;
			}
		}

		public bool Visit(LoadContainer container)
		{
			if (container.ContainerType != ContainerType.Database)
			{
				return container.CanAcceptRegularLoad;
			}
			this.AggregateDatabase(container);
			return false;
		}

		public bool Visit(LoadEntity entity)
		{
			return false;
		}

		private void AggregateDatabase(LoadContainer database)
		{
			BandData bandData = database.ConsumedLoad.GetBandData(this.band);
			bandData.Database = database;
			this.bandData.Add(bandData);
		}

		private readonly Band band;

		private readonly List<BandData> bandData = new List<BandData>();
	}
}
