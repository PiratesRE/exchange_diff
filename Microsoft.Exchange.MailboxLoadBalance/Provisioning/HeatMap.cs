using System;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.CapacityData;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Provisioning
{
	internal abstract class HeatMap : IHeatMap
	{
		public virtual bool IsReady
		{
			get
			{
				return true;
			}
		}

		public abstract LoadContainer GetLoadTopology();

		public HeatMapCapacityData ToCapacityData()
		{
			LoadContainer loadTopology = this.GetLoadTopology();
			return loadTopology.ToCapacityData();
		}

		public abstract void UpdateBands(Band[] bands);
	}
}
