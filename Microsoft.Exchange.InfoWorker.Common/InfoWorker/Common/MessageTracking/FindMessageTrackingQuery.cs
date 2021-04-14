using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.SoapWebClient.AutoDiscover;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal sealed class FindMessageTrackingQuery : Query<FindMessageTrackingQueryResult>
	{
		public FindMessageTrackingQuery(SmtpAddress proxyRecipient, string domain, DirectoryContext directoryContext, FindMessageTrackingReportRequestTypeWrapper request, ExchangeVersion minVersionRequested, TimeSpan timeout) : base(directoryContext.ClientContext, null, CasTraceEventType.MessageTracking, FindMessageTrackingApplication.MessageTrackingIOCompletion, InfoWorkerMessageTrackingPerformanceCounters.CurrentRequestDispatcherRequests)
		{
			if (SmtpAddress.Empty.Equals(proxyRecipient))
			{
				string address = ServerCache.Instance.GetOrgMailboxForDomain(domain).ToString();
				this.fakeRecipientQueryResults = MessageTrackingApplication.CreateFakeRecipientQueryResult(address);
			}
			else
			{
				this.proxyRecipient = proxyRecipient;
			}
			this.directoryContext = directoryContext;
			this.request = request;
			this.minVersionRequested = minVersionRequested;
			base.Timeout = timeout;
		}

		protected override void ValidateSpecificInputData()
		{
		}

		protected override FindMessageTrackingQueryResult ExecuteInternal()
		{
			base.RequestLogger.AppendToLog<string>("MessageTrackingRequest", "Start");
			FindMessageTrackingApplication findMessageTrackingApplication = new FindMessageTrackingApplication(this.request, this.minVersionRequested);
			FindMessageTrackingQueryResult result;
			using (RequestDispatcher requestDispatcher = new RequestDispatcher(base.RequestLogger))
			{
				IList<RecipientData> recipientQueryResults;
				if (this.fakeRecipientQueryResults != null)
				{
					recipientQueryResults = this.fakeRecipientQueryResults;
				}
				else
				{
					recipientQueryResults = MessageTrackingApplication.CreateRecipientQueryResult(this.directoryContext, this.queryPrepareDeadline, this.proxyRecipient.ToString());
				}
				QueryGenerator queryGenerator = new QueryGenerator(findMessageTrackingApplication, base.ClientContext, base.RequestLogger, requestDispatcher, this.queryPrepareDeadline, this.requestProcessingDeadline, recipientQueryResults);
				try
				{
					BaseQuery[] queries = queryGenerator.GetQueries();
					requestDispatcher.Execute(this.requestProcessingDeadline, base.HttpResponse);
					FindMessageTrackingBaseQuery findMessageTrackingBaseQuery = (FindMessageTrackingBaseQuery)queries[0];
					if (findMessageTrackingBaseQuery.Result == null)
					{
						result = null;
					}
					else
					{
						if (findMessageTrackingBaseQuery.Result.ExceptionInfo != null)
						{
							throw findMessageTrackingBaseQuery.Result.ExceptionInfo;
						}
						FindMessageTrackingQueryResult findMessageTrackingQueryResult = new FindMessageTrackingQueryResult();
						findMessageTrackingQueryResult.Response = findMessageTrackingBaseQuery.Result.Response;
						base.RequestLogger.AppendToLog<string>("MessageTrackingRequest", "Exit");
						result = findMessageTrackingQueryResult;
					}
				}
				finally
				{
					requestDispatcher.LogStatistics(base.RequestLogger);
					findMessageTrackingApplication.LogThreadsUsage(base.RequestLogger);
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

		internal static readonly PropertyDefinition[] RecipientProperties = new PropertyDefinition[]
		{
			ADObjectSchema.OrganizationId,
			ADRecipientSchema.RecipientType,
			ADRecipientSchema.RecipientDisplayType,
			ADRecipientSchema.ExternalEmailAddress,
			ADRecipientSchema.PrimarySmtpAddress,
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.LegacyExchangeDN,
			ADMailboxRecipientSchema.Database,
			ADMailboxRecipientSchema.ServerLegacyDN,
			ADMailboxRecipientSchema.ExchangeGuid,
			ADMailboxRecipientSchema.Database,
			ADObjectSchema.ExchangeVersion,
			ADObjectSchema.Id
		};

		private DirectoryContext directoryContext;

		private SmtpAddress proxyRecipient;

		private IList<RecipientData> fakeRecipientQueryResults;

		private FindMessageTrackingReportRequestTypeWrapper request;

		private ExchangeVersion minVersionRequested;
	}
}
