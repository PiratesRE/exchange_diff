using System;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Provisioning
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ChainedHeatMap : HeatMap
	{
		public ChainedHeatMap(params IHeatMap[] heatMaps)
		{
			AnchorUtil.ThrowOnCollectionEmptyArgument(heatMaps, "heatMaps");
			this.heatMaps = heatMaps;
		}

		public override LoadContainer GetLoadTopology()
		{
			IHeatMap heatMap = this.heatMaps.FirstOrDefault((IHeatMap map) => map.IsReady);
			if (heatMap == null)
			{
				throw new HeatMapNotBuiltException();
			}
			return heatMap.GetLoadTopology();
		}

		public override void UpdateBands(Band[] bands)
		{
			foreach (IHeatMap heatMap in this.heatMaps)
			{
				heatMap.UpdateBands(bands);
			}
		}

		private readonly IHeatMap[] heatMaps;
	}
}
