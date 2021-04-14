using System;
using Microsoft.Exchange.Hygiene.Data.Directory;

namespace Microsoft.Forefront.Reporting.OnDemandQuery
{
	public class RequestTracker
	{
		public RequestTracker()
		{
		}

		public RequestTracker(Guid tenantId, Guid onDemandQueryRequestId, int inBatchQueryId, OnDemandQueryType queryType)
		{
			this.TenantId = tenantId;
			this.RequestId = onDemandQueryRequestId;
			this.InBatchQueryId = inBatchQueryId;
			this.QueryType = queryType;
		}

		public Guid TenantId { get; set; }

		public Guid RequestId { get; set; }

		public int InBatchQueryId { get; set; }

		public OnDemandQueryType QueryType { get; set; }
	}
}
