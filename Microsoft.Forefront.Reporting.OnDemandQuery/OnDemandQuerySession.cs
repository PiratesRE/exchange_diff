using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Hygiene.Data.Directory;
using Microsoft.Forefront.Reporting.Common;

namespace Microsoft.Forefront.Reporting.OnDemandQuery
{
	public class OnDemandQuerySession
	{
		public OnDemandQuerySession()
		{
			this.dalSession = new TenantReportingSession();
		}

		internal OnDemandQueryRequest NewQueryRequest(Guid tenantID, OnDemandQueryType queryType, string queryTitle, string queryDefinition, OnDemandQueryCallerType callerType, string notificationEmail = null, string resultLocale = null)
		{
			if (string.IsNullOrWhiteSpace(queryDefinition))
			{
				throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.EmptySearchDefinition);
			}
			if (resultLocale != null)
			{
				CultureInfo.GetCultureInfo(resultLocale);
			}
			OnDemandQueryRequest onDemandQueryRequest = new OnDemandQueryRequest(tenantID, Guid.NewGuid())
			{
				SubmissionTime = DateTime.UtcNow,
				QueryType = queryType,
				QueryPriority = OnDemandQueryPriority.Normal,
				QueryDefinition = queryDefinition,
				CallerType = callerType,
				RequestStatus = OnDemandQueryRequestStatus.NotStarted,
				QuerySubject = queryTitle,
				NotificationEmail = notificationEmail,
				ResultLocale = resultLocale
			};
			if ((from request in this.dalSession.FindOnDemandReportRequests(tenantID, null, null, new DateTime?(DateTime.UtcNow.AddDays(-1.0)), 100)
			where request.CallerType == OnDemandQueryCallerType.Customer
			where request.RequestStatus != OnDemandQueryRequestStatus.Failed && request.RequestStatus != OnDemandQueryRequestStatus.UserCancel
			select request).Count<OnDemandQueryRequest>() > 50)
			{
				throw new OverTenantQuotaException();
			}
			this.dalSession.Save(onDemandQueryRequest);
			return onDemandQueryRequest;
		}

		internal OnDemandQueryRequest GetQueryRequest(Guid tenantID, Guid requestID)
		{
			OnDemandQueryRequest onDemandQueryRequest = (from r in this.dalSession.FindOnDemandReportRequests(tenantID, new Guid?(requestID), null, null, 100)
			where r.RequestStatus != OnDemandQueryRequestStatus.OverSystemQuota && r.RequestStatus != OnDemandQueryRequestStatus.OverTenantQuota
			select OnDemandQuerySession.MarkRequestIfExpired(r) into r
			where r.RequestStatus != OnDemandQueryRequestStatus.Expired
			select r).FirstOrDefault<OnDemandQueryRequest>();
			if (onDemandQueryRequest == null)
			{
				throw new QueryIdNotFoundException();
			}
			return onDemandQueryRequest;
		}

		internal OnDemandQueryRequest ViewQueryResult(Guid tenantID, Guid requestID)
		{
			OnDemandQueryRequest onDemandQueryRequest = (from r in this.dalSession.FindOnDemandReportRequests(tenantID, new Guid?(requestID), null, null, 100)
			where r.RequestStatus != OnDemandQueryRequestStatus.OverSystemQuota && r.RequestStatus != OnDemandQueryRequestStatus.OverTenantQuota
			select OnDemandQuerySession.MarkRequestIfExpired(r)).FirstOrDefault<OnDemandQueryRequest>();
			if (onDemandQueryRequest == null)
			{
				throw new QueryIdNotFoundException();
			}
			onDemandQueryRequest.ViewCounts++;
			this.dalSession.Save(onDemandQueryRequest);
			OnDemandQueryLogger.Log(onDemandQueryRequest, OnDemandQueryLogEvent.View, null);
			return onDemandQueryRequest;
		}

		internal IList<OnDemandQueryRequest> GetQueryRequests(Guid tenantID, DateTime startDate)
		{
			return (from r in this.dalSession.FindOnDemandReportRequests(tenantID, null, null, new DateTime?(startDate), 100)
			where r.RequestStatus != OnDemandQueryRequestStatus.OverSystemQuota && r.RequestStatus != OnDemandQueryRequestStatus.OverTenantQuota
			select OnDemandQuerySession.MarkRequestIfExpired(r)).ToList<OnDemandQueryRequest>();
		}

		internal IList<OnDemandQueryRequest> GetQueryRequests(Guid tenantID, DateTime startDate, OnDemandQueryRequestStatus status)
		{
			return (from r in this.dalSession.FindOnDemandReportRequests(tenantID, null, null, new DateTime?(startDate), 100)
			where r.RequestStatus != OnDemandQueryRequestStatus.OverSystemQuota && r.RequestStatus != OnDemandQueryRequestStatus.OverTenantQuota && r.RequestStatus == status
			select OnDemandQuerySession.MarkRequestIfExpired(r)).ToList<OnDemandQueryRequest>();
		}

		internal OnDemandQueryRequest CancelQueryRequest(Guid tenantID, Guid requestID)
		{
			OnDemandQueryRequest onDemandQueryRequest = (from r in this.dalSession.FindOnDemandReportRequests(tenantID, new Guid?(requestID), null, null, 100)
			where r.RequestStatus != OnDemandQueryRequestStatus.OverSystemQuota && r.RequestStatus != OnDemandQueryRequestStatus.OverTenantQuota
			select r).FirstOrDefault<OnDemandQueryRequest>();
			if (onDemandQueryRequest == null)
			{
				throw new QueryIdNotFoundException();
			}
			onDemandQueryRequest.RequestStatus = OnDemandQueryRequestStatus.UserCancel;
			this.dalSession.Save(onDemandQueryRequest);
			return onDemandQueryRequest;
		}

		internal Dictionary<Guid, RequestBatch> CreateRequestBatches(Dictionary<Guid, OnDemandQueryType[]> queryTypes, IEnumerable<OnDemandQueryRequestStatus> status, int batchSize, ref string pageCookie)
		{
			HashSet<OnDemandQueryType> queryTypes2 = new HashSet<OnDemandQueryType>(queryTypes.SelectMany((KeyValuePair<Guid, OnDemandQueryType[]> t) => t.Value));
			bool flag = false;
			IEnumerable<OnDemandQueryRequest> allRequests = Enumerable.Empty<OnDemandQueryRequest>();
			while (!flag && allRequests.Count<OnDemandQueryRequest>() < batchSize)
			{
				allRequests = allRequests.Concat(this.dalSession.FindOnDemandReportsForScheduling(queryTypes2, status, ref pageCookie, out flag, batchSize));
			}
			return queryTypes.ToDictionary((KeyValuePair<Guid, OnDemandQueryType[]> pair) => pair.Key, (KeyValuePair<Guid, OnDemandQueryType[]> pair) => new RequestBatch(this.dalSession, pair.Value, pair.Key, allRequests));
		}

		internal RequestBatch GetRequestBatch(List<RequestTracker> requests, Guid batchId)
		{
			return new RequestBatch(this.dalSession, requests, batchId);
		}

		internal void UpdateRequest(OnDemandQueryRequest request)
		{
			this.dalSession.Save(request);
		}

		internal void UpdateRequestStatus(List<RequestTracker> requestTrackers, OnDemandQueryRequestStatus newStatus)
		{
			foreach (RequestTracker requestTracker in requestTrackers)
			{
				this.dalSession.Save(new OnDemandQueryRequest(requestTracker.TenantId, requestTracker.RequestId)
				{
					RequestStatus = newStatus
				});
			}
		}

		private static bool CheckIfOverSystemThrottle(OnDemandQueryRequest systemCountKeepingRequest)
		{
			switch (systemCountKeepingRequest.QueryType)
			{
			case OnDemandQueryType.MTSummary:
				if (systemCountKeepingRequest.ViewCounts > 40000)
				{
					return true;
				}
				break;
			case OnDemandQueryType.MTDetail:
				if (systemCountKeepingRequest.ViewCounts > 20000)
				{
					return true;
				}
				break;
			case OnDemandQueryType.DLP:
			case OnDemandQueryType.Rule:
			case OnDemandQueryType.AntiSpam:
			case OnDemandQueryType.AntiVirus:
				if (systemCountKeepingRequest.ViewCounts > 10000)
				{
					return true;
				}
				break;
			default:
				throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.InvalidQueryType);
			}
			return false;
		}

		private static OnDemandQueryRequest MarkRequestIfExpired(OnDemandQueryRequest request)
		{
			if (request.SubmissionTime < DateTime.UtcNow.AddDays(-10.0))
			{
				request.RequestStatus = OnDemandQueryRequestStatus.Expired;
			}
			return request;
		}

		private OnDemandQueryRequest GetSystemCountKeepingRequest(OnDemandQueryType queryType)
		{
			OnDemandQueryRequest onDemandQueryRequest = (from r in this.dalSession.FindOnDemandReportRequests(Constants.TenantIDForSystemCount, null, null, new DateTime?(DateTime.UtcNow.AddDays(-1.0)), 100)
			where r.QueryType == queryType
			select r).FirstOrDefault<OnDemandQueryRequest>();
			if (onDemandQueryRequest == null)
			{
				return new OnDemandQueryRequest(Constants.TenantIDForSystemCount, Guid.NewGuid())
				{
					QueryType = queryType,
					RequestStatus = OnDemandQueryRequestStatus.OverSystemQuota,
					ViewCounts = 1
				};
			}
			return onDemandQueryRequest;
		}

		public const int ResultExpirationDays = 10;

		public const int PerTenantDailyThreshold = 50;

		public const int SystemDailyThresholdMTSummary = 40000;

		public const int SystemDailyThresholdMTDetail = 20000;

		public const int SystemDailyThresholdRuleHit = 10000;

		private TenantReportingSession dalSession;
	}
}
