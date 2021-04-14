using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal interface IUMAggregatedData
	{
		DateTime WaterMark { get; }

		void AddCDR(CDRData cdrData);

		void Cleanup(OrganizationId orgId);

		UMReportRawCounters[] QueryAggregatedData(Guid dialPlanGuid, Guid gatewayGuid, GroupBy groupBy);
	}
}
