using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class ProxyProtocolRequest : DisposeTrackableBase
	{
		internal void BeginSend(OwaContext owaContext, HttpRequest originalRequest, string targetUrl, string proxyRequestBody, AsyncCallback callback, object extraData)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyProtocolRequest.BeginSend");
			Uri uri = new UriBuilder(owaContext.SecondCasUri.Uri)
			{
				Path = targetUrl
			}.Uri;
			HttpWebRequest proxyRequestInstance = ProxyUtilities.GetProxyRequestInstance(originalRequest, owaContext, uri);
			proxyRequestInstance.ContentLength = (long)((proxyRequestBody != null) ? Encoding.UTF8.GetByteCount(proxyRequestBody) : 0);
			proxyRequestInstance.Method = "POST";
			this.proxyRequest = proxyRequestInstance;
			this.proxyRequestBody = proxyRequestBody;
			this.owaContext = owaContext;
			this.asyncResult = new OwaAsyncResult(callback, extraData);
			proxyRequestInstance.BeginGetRequestStream(new AsyncCallback(this.GetRequestStreamCallback), this);
		}

		internal HttpWebResponse EndSend(IAsyncResult asyncResult)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyProtocolRequest.EndSend");
			HttpWebResponse result;
			try
			{
				OwaAsyncResult owaAsyncResult = (OwaAsyncResult)asyncResult;
				if (owaAsyncResult.Exception != null)
				{
					throw new OwaAsyncOperationException("ProxyProtocolRequest async operation failed", owaAsyncResult.Exception);
				}
				HttpWebResponse httpWebResponse = this.proxyResponse;
				this.proxyResponse = null;
				result = httpWebResponse;
			}
			finally
			{
				this.Dispose();
			}
			return result;
		}

		private void GetRequestStreamCallback(IAsyncResult asyncResult)
		{
			try
			{
				this.proxyRequestStream = this.proxyRequest.EndGetRequestStream(asyncResult);
				this.proxyStreamCopy = new ProxyStreamCopy(this.proxyRequestBody, this.proxyRequestStream, StreamCopyMode.SyncReadAsyncWrite);
				this.proxyStreamCopy.Encoding = Encoding.UTF8;
				this.proxyStreamCopy.BeginCopy(new AsyncCallback(this.CopyRequestStreamCallback), this);
			}
			catch (Exception exception)
			{
				this.asyncResult.CompleteRequest(false, exception);
			}
		}

		private void CopyRequestStreamCallback(IAsyncResult result)
		{
			try
			{
				this.proxyStreamCopy.EndCopy(result);
				this.requestTimedOut = false;
				IAsyncResult asyncResult = ProxyUtilities.BeginGetResponse(this.proxyRequest, new AsyncCallback(this.GetResponseCallback), this, out this.requestClock);
				this.timeoutWaitHandle = ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, new WaitOrTimerCallback(this.RequestTimeoutCallback), asyncResult, (long)this.owaContext.HttpContext.Server.ScriptTimeout * 1000L, true);
			}
			catch (Exception exception)
			{
				this.asyncResult.CompleteRequest(false, exception);
			}
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
						this.proxyResponse = ProxyUtilities.EndGetResponse(this.proxyRequest, asyncResult, this.requestClock);
						ProxyUtilities.UpdateProxyUserContextIdFromResponse(this.proxyResponse, this.owaContext.UserContext);
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
				if (this.proxyResponse != null)
				{
					this.proxyResponse.Close();
					this.proxyResponse = null;
				}
				if (this.proxyResponseStream != null)
				{
					this.proxyResponseStream.Close();
					this.proxyResponseStream = null;
				}
				if (this.proxyRequestStream != null)
				{
					this.proxyRequestStream.Close();
					this.proxyRequestStream = null;
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
			return DisposeTracker.Get<ProxyProtocolRequest>(this);
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
					this.proxyRequest.Abort();
					this.asyncResult.CompleteRequest(false, new OwaAsyncRequestTimeoutException("ProxyProtocolRequest request timeout", null));
				}
			}
		}

		private HttpWebRequest proxyRequest;

		private HttpWebResponse proxyResponse;

		private Stream proxyRequestStream;

		private Stream proxyResponseStream;

		private string proxyRequestBody;

		private OwaAsyncResult asyncResult;

		private ProxyStreamCopy proxyStreamCopy;

		private Stopwatch requestClock;

		private OwaContext owaContext;

		private bool requestTimedOut;

		private RegisteredWaitHandle timeoutWaitHandle;
	}
}
