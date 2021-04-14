using System;
using System.Collections.Generic;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Provisioning
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class HeatMapDatabaseSelector : DatabaseSelector
	{
		public HeatMapDatabaseSelector(IHeatMap heatMap, ILogger logger) : base(logger)
		{
			this.heatMap = heatMap;
		}

		protected override IEnumerable<LoadContainer> GetAvailableDatabases()
		{
			DatabaseCollector databaseCollector = new DatabaseCollector();
			this.heatMap.GetLoadTopology().Accept(databaseCollector);
			return databaseCollector.Databases;
		}

		private readonly IHeatMap heatMap;
	}
}
