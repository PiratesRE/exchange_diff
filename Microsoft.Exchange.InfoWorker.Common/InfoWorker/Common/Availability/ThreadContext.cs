using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal static class ThreadContext
	{
		public static void SetWithExceptionHandling(string label, ThreadCounter threadCounter, ClientContext clientContext, RequestLogger requestLogger, ThreadContext.ExecuteDelegate executeDelegate)
		{
			ThreadContext.Set(label, threadCounter, clientContext, requestLogger, delegate()
			{
				try
				{
					GrayException.MapAndReportGrayExceptions(delegate()
					{
						executeDelegate();
					});
				}
				catch (GrayException arg)
				{
					string arg2 = (clientContext != null) ? clientContext.IdentityForFilteredTracing : "none";
					ThreadContext.Tracer.TraceError<string, GrayException>(0L, "{0}: failed with exception: {1}", arg2, arg);
				}
			});
		}

		public static T Set<T>(string label, ThreadCounter threadCounter, ClientContext clientContext, RequestLogger requestLogger, ThreadContext.ExecuteDelegate<T> executeDelegate)
		{
			T result = default(T);
			ThreadContext.Set(label, threadCounter, clientContext, requestLogger, delegate()
			{
				result = executeDelegate();
			});
			return result;
		}

		private static void Set(string label, ThreadCounter threadCounter, ClientContext clientContext, RequestLogger requestLogger, ThreadContext.ExecuteDelegate executeDelegate)
		{
			string text = (clientContext != null) ? clientContext.IdentityForFilteredTracing : "none";
			RequestStatisticsForThread requestStatisticsForThread = RequestStatisticsForThread.Begin();
			threadCounter.Increment();
			ThreadContext.Tracer.TraceDebug<string, string, string>(0L, "{0}: Thread entered {1}. MessageId={2}", text, label, (clientContext != null) ? (clientContext.MessageId ?? "<null>") : "none");
			try
			{
				using (new ASTraceFilter(null, text))
				{
					TraceContext.Set(text);
					try
					{
						executeDelegate();
					}
					finally
					{
						TraceContext.Reset();
					}
				}
			}
			finally
			{
				threadCounter.Decrement();
				RequestStatistics requestStatistics = requestStatisticsForThread.End(RequestStatisticsType.ThreadCPULongPole, label);
				if (requestStatistics != null && requestLogger != null)
				{
					requestLogger.Add(requestStatistics);
				}
				ThreadContext.Tracer.TraceDebug<string, string>(0L, "{0}: Thread exited {1}", text, label);
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.RequestRoutingTracer;

		public delegate void ExecuteDelegate();

		public delegate T ExecuteDelegate<T>();
	}
}
