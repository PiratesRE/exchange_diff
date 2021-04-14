using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal abstract class Query<ReturnType>
	{
		public Query(ClientContext clientContext, HttpResponse httpResponse, CasTraceEventType casTraceEventType, ThreadCounter threadCounter, ExPerformanceCounter currentRequestsCounter) : this(casTraceEventType, threadCounter, currentRequestsCounter)
		{
			this.ClientContext = clientContext;
			this.HttpResponse = httpResponse;
		}

		public Query(CasTraceEventType casTraceEventType, ThreadCounter threadCounter, ExPerformanceCounter currentRequestsCounter)
		{
			this.casTraceEventType = casTraceEventType;
			this.threadCounter = threadCounter;
			this.currentRequestsCounter = currentRequestsCounter;
			this.Timeout = Configuration.WebRequestTimeoutInSeconds;
			this.requestProcessingDeadline = DateTime.UtcNow + this.Timeout;
			this.queryPrepareDeadline = this.requestProcessingDeadline;
			this.RequestLogger = new RequestLogger();
			ConfigurationReader.Start(this.RequestLogger);
		}

		public ClientContext ClientContext { get; set; }

		public HttpResponse HttpResponse { get; set; }

		public RequestLogger RequestLogger { get; protected set; }

		public string ServerName { get; set; }

		public TimeSpan Timeout { get; set; }

		public ReturnType Execute()
		{
			this.ValidateInputData();
			return ThreadContext.Set<ReturnType>(string.Format(CultureInfo.InvariantCulture, "{0}.Execute", new object[]
			{
				base.GetType().Name
			}), this.threadCounter, this.ClientContext, this.RequestLogger, new ThreadContext.ExecuteDelegate<ReturnType>(this.ExecuteWithPerformanceMeasurement));
		}

		internal static string GetCurrentHttpRequestServerName()
		{
			if (HttpContext.Current != null && HttpContext.Current.Server != null)
			{
				return HttpContext.Current.Server.MachineName;
			}
			return string.Empty;
		}

		protected abstract void ValidateSpecificInputData();

		protected abstract ReturnType ExecuteInternal();

		protected abstract void UpdateCountersAtExecuteEnd(Stopwatch responseTimer);

		protected abstract void AppendSpecificSpExecuteOperationData(StringBuilder spOperationData);

		protected virtual void LogExpensiveRequests(RequestStatistics threadStatistics, RequestStatistics mapiStatistics, RequestStatistics adStatistics)
		{
		}

		protected int LogFailures(ReturnType result, IDictionary<string, int> exceptionStatistics)
		{
			int num = 0;
			if (result != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("<");
				foreach (string text in exceptionStatistics.Keys)
				{
					num += exceptionStatistics[text];
					stringBuilder.AppendFormat("{0}={1}|", text, exceptionStatistics[text]);
				}
				stringBuilder.Append(">");
				this.RequestLogger.AppendToLog<int>("Failures", num);
				this.RequestLogger.AppendToLog<string>("EXP", stringBuilder.ToString());
			}
			else
			{
				this.RequestLogger.AppendToLog<string>("RequestFailed", "true");
			}
			return num;
		}

		protected StringBuilder GetSpExecuteOperationData()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Mailboxes Processed: {0}", this.individualMailboxesProcessed);
			this.AppendSpecificSpExecuteOperationData(stringBuilder);
			return stringBuilder;
		}

		private ReturnType ExecuteWithPerformanceMeasurement()
		{
			Query<ReturnType>.Tracer.TraceDebug<object, string>((long)this.GetHashCode(), "{0}: {1}.Execute: enter", TraceContext.Get(), base.GetType().Name);
			RequestStatisticsForThread requestStatisticsForThread = RequestStatisticsForThread.Begin();
			RequestStatisticsForMapi requestStatisticsForMapi = RequestStatisticsForMapi.Begin();
			RequestStatisticsForAD requestStatisticsForAD = RequestStatisticsForAD.Begin();
			Stopwatch stopwatch = Stopwatch.StartNew();
			Guid serverRequestId = Microsoft.Exchange.Diagnostics.Trace.TraceCasStart(this.casTraceEventType);
			this.currentRequestsCounter.Increment();
			ReturnType result;
			try
			{
				result = this.ExecuteInternal();
			}
			finally
			{
				stopwatch.Stop();
				this.TraceExecuteInternalStop(serverRequestId);
				this.UpdateCountersAtExecuteEnd(stopwatch);
				this.currentRequestsCounter.Decrement();
				Query<ReturnType>.Tracer.TraceDebug<object, string>((long)this.GetHashCode(), "{0}: {1}.Execute: exit", TraceContext.Get(), base.GetType().Name);
				RequestStatistics requestStatistics = requestStatisticsForMapi.End(RequestStatisticsType.MailboxRPC);
				requestStatistics.Log(this.RequestLogger);
				RequestStatistics requestStatistics2 = requestStatisticsForAD.End(RequestStatisticsType.AD);
				requestStatistics2.Log(this.RequestLogger);
				RequestStatistics requestStatistics3 = requestStatisticsForThread.End(RequestStatisticsType.RequestCPUMain);
				if (requestStatistics3 != null)
				{
					requestStatistics3.Log(this.RequestLogger);
				}
				this.RequestLogger.Log();
				this.LogExpensiveRequests(requestStatistics3, requestStatistics, requestStatistics2);
			}
			return result;
		}

		private void ValidateInputData()
		{
			if (this.ClientContext == null)
			{
				throw new MissingArgumentException(Strings.descMissingArgument("ClientContext"));
			}
			this.ClientContext.ValidateContext();
			this.ValidateSpecificInputData();
		}

		private void TraceExecuteInternalStop(Guid serverRequestId)
		{
			if (ETWTrace.ShouldTraceCasStop(serverRequestId))
			{
				StringBuilder spExecuteOperationData = this.GetSpExecuteOperationData();
				if (string.IsNullOrEmpty(this.ServerName))
				{
					this.ServerName = Query<ReturnType>.GetCurrentHttpRequestServerName();
				}
				Microsoft.Exchange.Diagnostics.Trace.TraceCasStop(this.casTraceEventType, serverRequestId, 0, 0, this.ServerName, TraceContext.Get(), string.Format(CultureInfo.InvariantCulture, "{0}::ExecuteInternal", new object[]
				{
					base.GetType().Name
				}), spExecuteOperationData, string.Empty);
			}
		}

		private static readonly Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.RequestRoutingTracer;

		protected DateTime requestProcessingDeadline;

		protected DateTime queryPrepareDeadline;

		protected CasTraceEventType casTraceEventType;

		protected int individualMailboxesProcessed;

		private ThreadCounter threadCounter;

		private ExPerformanceCounter currentRequestsCounter;
	}
}
