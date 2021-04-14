using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Threading;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal abstract class BaseAsyncOmexCommand : BaseAsyncCommand
	{
		public BaseAsyncOmexCommand(OmexWebServiceUrlsCache urlsCache, string scenario) : base(scenario)
		{
			this.urlsCache = urlsCache;
		}

		private void TimeoutCallback(object state, bool timedOut)
		{
			if (timedOut)
			{
				BaseAsyncCommand.Tracer.TraceError<Uri>(0L, "BaseAsyncOmexCommand.TimeoutCallback: Request timed out: {0}", this.uri);
				try
				{
					if (this.request != null)
					{
						this.request.Abort();
					}
				}
				catch (WebException exception)
				{
					this.InternalFailureCallback(exception, null);
					return;
				}
				this.asyncResult = null;
			}
		}

		protected void InternalExecute(Uri uri)
		{
			this.uri = uri;
			this.request = null;
			try
			{
				this.request = (HttpWebRequest)WebRequest.Create(uri);
				this.request.CachePolicy = BaseAsyncOmexCommand.NoCachePolicy;
				this.request.Method = "GET";
				this.PrepareRequest(this.request);
				this.asyncResult = this.request.BeginGetResponse(new AsyncCallback(this.EndGetResponseCallback), null);
				ThreadPool.RegisterWaitForSingleObject(this.asyncResult.AsyncWaitHandle, new WaitOrTimerCallback(this.TimeoutCallback), null, 60000, true);
			}
			catch (WebException exception)
			{
				this.InternalFailureCallback(exception, null);
			}
		}

		protected virtual void PrepareRequest(HttpWebRequest request)
		{
		}

		protected virtual void ProcessResponse(HttpWebResponse response)
		{
		}

		protected virtual long GetMaxResponseBufferSize()
		{
			return 32768L;
		}

		private void EndGetResponseCallback(IAsyncResult asyncResult)
		{
			lock (this.request)
			{
				if (this.requestFailedDueToMisMatchAsyncResult)
				{
					BaseAsyncCommand.Tracer.TraceError(0L, "EndGetResponseCallback called after request already failed. Ignoring callback.");
					return;
				}
				if (this.asyncResult != asyncResult && !asyncResult.CompletedSynchronously)
				{
					this.requestFailedDueToMisMatchAsyncResult = true;
				}
			}
			if (!this.requestFailedDueToMisMatchAsyncResult)
			{
				base.ExecuteWithExceptionHandler(delegate
				{
					try
					{
						HttpWebResponse httpWebResponse = this.request.EndGetResponse(asyncResult) as HttpWebResponse;
						this.ProcessResponse(httpWebResponse);
						this.responseStream = httpWebResponse.GetResponseStream();
						this.responseBuffer = new byte[this.GetMaxResponseBufferSize()];
						this.responseBufferIndex = 0;
						this.BeginResponseStreamRead();
					}
					catch (WebException exception)
					{
						this.InternalFailureCallback(exception, null);
					}
					finally
					{
						this.asyncResult = null;
					}
				});
				return;
			}
			this.InternalFailureCallback(null, "asyncResult in callback does not match the asyncResult from BeginGetResponse. ignoring " + this.uri);
		}

		private void BeginResponseStreamRead()
		{
			try
			{
				if (this.responseBufferIndex >= this.responseBuffer.Length)
				{
					this.responseStream.Close();
					this.InternalFailureCallback(null, "BaseAsyncOmexCommand.BeginResponseStreamRead: Response buffer too small for request: " + this.uri);
					ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_ResponseExceedsBufferSize, this.uri.AbsolutePath, new object[]
					{
						this.scenario,
						this.requestId,
						base.GetLoggedMailboxIdentifier(),
						this.uri,
						this.GetMaxResponseBufferSize()
					});
				}
				else
				{
					this.responseStream.BeginRead(this.responseBuffer, this.responseBufferIndex, this.responseBuffer.Length - this.responseBufferIndex, new AsyncCallback(this.EndResponseStreamReadCallback), null);
				}
			}
			catch (IOException exception)
			{
				this.InternalFailureCallback(exception, null);
			}
			catch (WebException exception2)
			{
				this.InternalFailureCallback(exception2, null);
			}
		}

		private void EndResponseStreamReadCallback(IAsyncResult asyncResult)
		{
			base.ExecuteWithExceptionHandler(delegate
			{
				try
				{
					int num = this.responseStream.EndRead(asyncResult);
					this.responseBufferIndex += num;
					if (num != 0)
					{
						this.BeginResponseStreamRead();
					}
					else
					{
						BaseAsyncCommand.Tracer.Information(0L, "The web service response was received.");
						this.ParseResponse(this.responseBuffer, this.responseBufferIndex);
					}
				}
				catch (IOException exception)
				{
					this.InternalFailureCallback(exception, null);
				}
				catch (WebException exception2)
				{
					this.InternalFailureCallback(exception2, null);
				}
			});
		}

		protected void LogResponseParsed()
		{
			ExtensionDiagnostics.LogToDatacenterOnly(ApplicationLogicEventLogConstants.Tuple_OmexWebServiceResponseParsed, null, new object[]
			{
				this.scenario,
				this.requestId,
				base.GetLoggedMailboxIdentifier(),
				this.uri
			});
		}

		protected abstract void ParseResponse(byte[] responseBuffer, int responseBufferSize);

		private const int RequestTimeout = 60000;

		private const string GetRequestMethod = "GET";

		private const long defaultMaxBufferSize = 32768L;

		private static HttpRequestCachePolicy NoCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);

		private HttpWebRequest request;

		private IAsyncResult asyncResult;

		private bool requestFailedDueToMisMatchAsyncResult;

		private Stream responseStream;

		private byte[] responseBuffer;

		private int responseBufferIndex;

		protected OmexWebServiceUrlsCache urlsCache;
	}
}
