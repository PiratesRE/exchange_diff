using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Hygiene.Data.Directory;

namespace Microsoft.Forefront.Reporting.OnDemandQuery
{
	internal class RequestBatch
	{
		internal RequestBatch(TenantReportingSession dalSession, OnDemandQueryType[] queryTypes, Guid batchId, IEnumerable<OnDemandQueryRequest> allRequests)
		{
			this.dalSession = dalSession;
			this.BatchID = batchId;
			this.Requests = new Dictionary<OnDemandQueryType, IEnumerable<OnDemandQueryRequest>>();
			for (int i = 0; i < queryTypes.Length; i++)
			{
				OnDemandQueryType type = queryTypes[i];
				IEnumerable<OnDemandQueryRequest> enumerable = from r in allRequests
				where r.QueryType == type
				select r;
				if (enumerable.Any<OnDemandQueryRequest>())
				{
					int num = 1000000 * (int)type + 1;
					int num2 = num;
					foreach (OnDemandQueryRequest onDemandQueryRequest in enumerable)
					{
						onDemandQueryRequest.BatchId = new Guid?(this.BatchID);
						onDemandQueryRequest.InBatchQueryId = num2;
						num2++;
					}
				}
				this.Requests.Add(type, enumerable);
			}
		}

		internal RequestBatch(TenantReportingSession dalSession, List<RequestTracker> requestTrackers, Guid batchId)
		{
			this.dalSession = dalSession;
			this.Requests = new Dictionary<OnDemandQueryType, IEnumerable<OnDemandQueryRequest>>();
			foreach (IGrouping<OnDemandQueryType, RequestTracker> grouping in from tracker in requestTrackers
			group tracker by tracker.QueryType)
			{
				List<OnDemandQueryRequest> list = new List<OnDemandQueryRequest>();
				if (grouping.Count<RequestTracker>() > 0)
				{
					foreach (RequestTracker requestTracker in grouping)
					{
						OnDemandQueryRequest onDemandQueryRequest = this.dalSession.FindOnDemandReportRequests(requestTracker.TenantId, new Guid?(requestTracker.RequestId), null, null, 100).First<OnDemandQueryRequest>();
						onDemandQueryRequest.InBatchQueryId = requestTracker.InBatchQueryId;
						onDemandQueryRequest.BatchId = new Guid?(batchId);
						list.Add(onDemandQueryRequest);
					}
				}
				this.Requests.Add(grouping.Key, list);
			}
		}

		internal Dictionary<OnDemandQueryType, IEnumerable<OnDemandQueryRequest>> Requests { get; private set; }

		internal Guid BatchID { get; private set; }

		internal void UpdateCosmosJobAndStatus(Guid cosmosJobId)
		{
			foreach (OnDemandQueryRequest onDemandQueryRequest in this.GetAllRequests())
			{
				onDemandQueryRequest.RequestStatus = OnDemandQueryRequestStatus.InProgress;
				onDemandQueryRequest.CosmosJobId = new Guid?(cosmosJobId);
				this.dalSession.Save(onDemandQueryRequest);
			}
		}

		internal void UpdateStatus(OnDemandQueryRequestStatus newStatus)
		{
			foreach (OnDemandQueryRequest onDemandQueryRequest in this.GetAllRequests())
			{
				onDemandQueryRequest.RequestStatus = newStatus;
				this.dalSession.Save(onDemandQueryRequest);
			}
		}

		internal void UpdateStatus(OnDemandQueryType queryType, OnDemandQueryRequestStatus newStatus)
		{
			foreach (OnDemandQueryRequest onDemandQueryRequest in this.Requests[queryType])
			{
				onDemandQueryRequest.RequestStatus = newStatus;
				this.dalSession.Save(onDemandQueryRequest);
			}
		}

		internal IEnumerable<OnDemandQueryRequest> GetAllRequests()
		{
			foreach (OnDemandQueryType queryType in this.Requests.Keys)
			{
				foreach (OnDemandQueryRequest request in this.Requests[queryType])
				{
					yield return request;
				}
			}
			yield break;
		}

		internal List<RequestTracker> GetRequestTrackers()
		{
			return (from r in this.GetAllRequests()
			select new RequestTracker(r.TenantId, r.RequestId, r.InBatchQueryId, r.QueryType)).ToList<RequestTracker>();
		}

		private TenantReportingSession dalSession;
	}
}
