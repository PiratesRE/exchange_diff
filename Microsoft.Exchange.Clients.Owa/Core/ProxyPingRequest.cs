using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class ProxyPingRequest : DisposeTrackableBase
	{
		internal void BeginSend(OwaContext owaContext, AsyncCallback callback, object extraData)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyPingRequest.BeginSend");
			Uri uri = new UriBuilder(owaContext.SecondCasUri.Uri)
			{
				Path = OwaUrl.ProxyPing.GetExplicitUrl(owaContext)
			}.Uri;
			HttpWebRequest httpWebRequest = ProxyUtilities.CreateHttpWebRequestForProxying(owaContext, uri);
			httpWebRequest.Method = "GET";
			httpWebRequest.UserAgent = "OwaProxy";
			this.request = httpWebRequest;
			this.asyncResult = new OwaAsyncResult(callback, extraData);
			this.requestTimedOut = false;
			IAsyncResult asyncResult = ProxyUtilities.BeginGetResponse(this.request, new AsyncCallback(this.GetResponseCallback), this, out this.requestClock);
			this.timeoutWaitHandle = ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, new WaitOrTimerCallback(this.RequestTimeoutCallback), asyncResult, (long)owaContext.HttpContext.Server.ScriptTimeout * 1000L, true);
		}

		internal HttpWebResponse EndSend(IAsyncResult asyncResult)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyPingRequest.EndSend");
			HttpWebResponse result;
			try
			{
				OwaAsyncResult owaAsyncResult = (OwaAsyncResult)asyncResult;
				if (owaAsyncResult.Exception != null)
				{
					throw new OwaAsyncOperationException("ProxyPingRequest async operation failed", owaAsyncResult.Exception);
				}
				HttpWebResponse httpWebResponse = this.response;
				this.response = null;
				result = httpWebResponse;
			}
			finally
			{
				this.Dispose();
			}
			return result;
		}

		private void GetResponseCallback(IAsyncResult asyncResult)
		{
			lock (this)
			{
				if (!this.requestTimedOut)
				{
					try
					{
						if (this.timeoutWaitHandle != null)
						{
							this.timeoutWaitHandle.Unregister(null);
							this.timeoutWaitHandle = null;
						}
						this.response = ProxyUtilities.EndGetResponse(this.request, asyncResult, this.requestClock);
					}
					catch (Exception exception)
					{
						this.asyncResult.CompleteRequest(false, exception);
						return;
					}
					this.asyncResult.CompleteRequest(false);
				}
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				if (this.response != null)
				{
					this.response.Close();
					this.response = null;
				}
				if (this.timeoutWaitHandle != null)
				{
					this.timeoutWaitHandle.Unregister(null);
					this.timeoutWaitHandle = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ProxyPingRequest>(this);
		}

		private void RequestTimeoutCallback(object state, bool timedOut)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug(0L, "ProxyEventHandler.RequestTimeoutCallback");
			if (!timedOut)
			{
				ExTraceGlobals.ProxyTracer.TraceDebug(0L, "Request is completed, aborting timeout");
				return;
			}
			lock (this)
			{
				if (this.asyncResult.IsCompleted)
				{
					ExTraceGlobals.ProxyTracer.TraceDebug(0L, "Request is completed, aborting timeout");
				}
				else
				{
					ExTraceGlobals.ProxyTracer.TraceDebug(0L, "Async request timed out");
					this.requestTimedOut = true;
					this.request.Abort();
					this.asyncResult.CompleteRequest(false, new OwaAsyncRequestTimeoutException("ProxyPingRequest request timeout", null));
				}
			}
		}

		private HttpWebRequest request;

		private HttpWebResponse response;

		private bool requestTimedOut;

		private RegisteredWaitHandle timeoutWaitHandle;

		private OwaAsyncResult asyncResult;

		private Stopwatch requestClock;
	}
}
