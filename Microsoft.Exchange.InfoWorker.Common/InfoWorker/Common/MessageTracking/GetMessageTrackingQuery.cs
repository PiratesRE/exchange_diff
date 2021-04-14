using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.SoapWebClient.AutoDiscover;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal sealed class GetMessageTrackingQuery : Query<GetMessageTrackingQueryResult>
	{
		public GetMessageTrackingQuery(SmtpAddress proxyRecipient, DirectoryContext directoryContext, GetMessageTrackingReportRequestTypeWrapper request, ExchangeVersion minVersionRequested, TimeSpan timeout) : base(directoryContext.ClientContext, null, CasTraceEventType.MessageTracking, GetMessageTrackingApplication.MessageTrackingIOCompletion, InfoWorkerMessageTrackingPerformanceCounters.CurrentRequestDispatcherRequests)
		{
			MessageTrackingReportId messageTrackingReportId = null;
			if (!MessageTrackingReportId.TryParse(request.WrappedRequest.MessageTrackingReportId, out messageTrackingReportId))
			{
				throw new ArgumentException("MessageTrackingReportId invalid");
			}
			this.directoryContext = directoryContext;
			string address = ServerCache.Instance.GetOrgMailboxForDomain(messageTrackingReportId.Domain).ToString();
			this.fakeRecipientQueryResults = MessageTrackingApplication.CreateFakeRecipientQueryResult(address);
			this.proxyRecipient = proxyRecipient;
			this.request = request;
			this.minVersionRequested = minVersionRequested;
			base.Timeout = timeout;
		}

		protected override void ValidateSpecificInputData()
		{
		}

		protected override GetMessageTrackingQueryResult ExecuteInternal()
		{
			base.RequestLogger.AppendToLog<string>("MessageTrackingRequest", "Start");
			GetMessageTrackingApplication getMessageTrackingApplication = new GetMessageTrackingApplication(this.request, this.minVersionRequested);
			GetMessageTrackingQueryResult result;
			using (RequestDispatcher requestDispatcher = new RequestDispatcher(base.RequestLogger))
			{
				try
				{
					IList<RecipientData> recipientQueryResults;
					if (this.proxyRecipient != SmtpAddress.Empty)
					{
						recipientQueryResults = MessageTrackingApplication.CreateRecipientQueryResult(this.directoryContext, this.queryPrepareDeadline, this.proxyRecipient.ToString());
					}
					else
					{
						recipientQueryResults = this.fakeRecipientQueryResults;
					}
					QueryGenerator queryGenerator = new QueryGenerator(getMessageTrackingApplication, base.ClientContext, base.RequestLogger, requestDispatcher, this.queryPrepareDeadline, this.requestProcessingDeadline, recipientQueryResults);
					BaseQuery[] queries = queryGenerator.GetQueries();
					requestDispatcher.Execute(this.requestProcessingDeadline, base.HttpResponse);
					GetMessageTrackingBaseQuery getMessageTrackingBaseQuery = (GetMessageTrackingBaseQuery)queries[0];
					if (getMessageTrackingBaseQuery.Result == null)
					{
						result = null;
					}
					else
					{
						if (getMessageTrackingBaseQuery.Result.ExceptionInfo != null)
						{
							throw getMessageTrackingBaseQuery.Result.ExceptionInfo;
						}
						GetMessageTrackingQueryResult getMessageTrackingQueryResult = new GetMessageTrackingQueryResult();
						getMessageTrackingQueryResult.Response = getMessageTrackingBaseQuery.Result.Response;
						base.RequestLogger.AppendToLog<string>("MessageTrackingRequest", "Exit");
						result = getMessageTrackingQueryResult;
					}
				}
				finally
				{
					requestDispatcher.LogStatistics(base.RequestLogger);
					getMessageTrackingApplication.LogThreadsUsage(base.RequestLogger);
				}
			}
			return result;
		}

		protected override void UpdateCountersAtExecuteEnd(Stopwatch responseTimer)
		{
		}

		protected override void AppendSpecificSpExecuteOperationData(StringBuilder spOperationData)
		{
			spOperationData.AppendFormat("Recipient Processed: {0}", this.proxyRecipient.ToString());
		}

		private DirectoryContext directoryContext;

		private SmtpAddress proxyRecipient;

		private IList<RecipientData> fakeRecipientQueryResults;

		private GetMessageTrackingReportRequestTypeWrapper request;

		private ExchangeVersion minVersionRequested;
	}
}
