using System;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class RequestDispatcher : BaseRequestDispatcher
	{
		public RequestDispatcher(RequestLogger requestLogger)
		{
			this.requestLogger = requestLogger;
		}

		public bool Execute(DateTime deadline, HttpResponse httpResponse)
		{
			this.ThrowIfClientDisconnected(httpResponse);
			this.requestLogger.CaptureRequestStage("RequestDispatcher.PreQuery");
			RequestDispatcher.RequestRoutingTracer.TraceDebug<object, int>((long)this.GetHashCode(), "{0}: Dispatching all {1} requests", TraceContext.Get(), base.Requests.Count);
			bool flag = false;
			using (ManualResetEvent done = new ManualResetEvent(false))
			{
				this.BeginInvoke(delegate(AsyncTask task)
				{
					try
					{
						done.Set();
					}
					catch (ObjectDisposedException)
					{
					}
				});
				this.requestLogger.CaptureRequestStage("RequestDispatcher.BeginInvoke");
				RequestDispatcher.RequestRoutingTracer.TraceDebug<object, DateTime>((long)this.GetHashCode(), "{0}: Waiting for requests to complete by {1}", TraceContext.Get(), deadline);
				do
				{
					this.ThrowIfClientDisconnected(httpResponse);
					flag = done.WaitOne(RequestDispatcher.DisconnectCheckInterval, false);
				}
				while (DateTime.UtcNow < deadline && !flag);
			}
			this.requestLogger.CaptureRequestStage("RequestDispatcher.Complete");
			if (!flag)
			{
				try
				{
					RequestDispatcher.RequestRoutingTracer.TraceError<object, DateTime>((long)this.GetHashCode(), "{0}: Not all requests did not complete by {1}. Aborting now.", TraceContext.Get(), deadline);
				}
				finally
				{
					this.Abort();
				}
				this.requestLogger.CaptureRequestStage("RequestDispatcher.Abort");
			}
			else
			{
				RequestDispatcher.RequestRoutingTracer.TraceDebug((long)this.GetHashCode(), "{0}: All requests completed in time", new object[]
				{
					TraceContext.Get()
				});
			}
			return flag;
		}

		private void ThrowIfClientDisconnected(HttpResponse httpResponse)
		{
			if (httpResponse != null && !httpResponse.IsClientConnected)
			{
				RequestDispatcher.RequestRoutingTracer.TraceDebug((long)this.GetHashCode(), "{0}: Client has disconnected. Aborting now.", new object[]
				{
					TraceContext.Get()
				});
				throw new ClientDisconnectedException();
			}
		}

		public override string ToString()
		{
			return "RequestDispatcher for " + base.Requests.Count + " requests";
		}

		private RequestLogger requestLogger;

		private static readonly TimeSpan DisconnectCheckInterval = TimeSpan.FromSeconds(1.0);

		private static readonly Trace RequestRoutingTracer = ExTraceGlobals.RequestRoutingTracer;
	}
}
