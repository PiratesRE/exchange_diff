using System;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.CapacityData;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Provisioning
{
	internal interface IHeatMap
	{
		bool IsReady { get; }

		LoadContainer GetLoadTopology();

		HeatMapCapacityData ToCapacityData();

		void UpdateBands(Band[] bands);
	}
}
