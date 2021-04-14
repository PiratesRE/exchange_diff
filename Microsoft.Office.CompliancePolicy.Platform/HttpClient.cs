using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;

namespace Microsoft.Office.CompliancePolicy
{
	internal class HttpClient : IDisposable
	{
		public HttpClient(ExecutionLog protocolLog = null)
		{
			this.responseCallBack = this.WrapCallbackWithUnhandledExceptionRedirect(new WaitCallback(this.ResponseCallBack));
			this.requestCallBack = this.WrapCallbackWithUnhandledExceptionRedirect(new WaitCallback(this.RequestCallBack));
			this.readResponseCallBack = this.WrapCallbackWithUnhandledExceptionRedirect(new WaitCallback(this.ReadResponseCallBack));
			this.writeRequestCallBack = this.WrapCallbackWithUnhandledExceptionRedirect(new WaitCallback(this.WriteRequestCallBack));
			this.writeResponseCallBack = new AsyncCallback(this.WriteResponseCallBack);
			this.readRequestCallBack = new AsyncCallback(this.ReadRequestCallBack);
			this.breadcrumbs = new Breadcrumbs<HttpClient.Breadcrumbs>(64);
			this.timeoutTimer = new Timer(new TimerCallback(this.TimeoutCallback), null, -1, -1);
			this.protocolLog = protocolLog;
		}

		~HttpClient()
		{
			this.Dispose(false);
		}

		public event EventHandler<HttpWebRequestEventArgs> SendingRequest;

		public event EventHandler<HttpWebResponseEventArgs> ResponseReceived;

		public event EventHandler<DownloadCompleteEventArgs> DownloadCompleted;

		public Uri LastKnownRequestedUri
		{
			get
			{
				this.CheckDisposed();
				return this.lastKnownRequestedUri;
			}
		}

		internal bool CompletedSynchronously
		{
			get
			{
				this.CheckDisposed();
				return this.completedSynchronously;
			}
		}

		private byte[] Buffer
		{
			get
			{
				if (this.buffer == null)
				{
					this.buffer = new byte[4096];
				}
				return this.buffer;
			}
		}

		public ICancelableAsyncResult BeginDownload(Uri url, AsyncCallback requestCallback, object state)
		{
			return this.BeginDownload(url, new HttpSessionConfig(), requestCallback, state);
		}

		public ICancelableAsyncResult BeginDownload(Uri url, int timeoutInterval, AsyncCallback requestCallback, object state)
		{
			return this.BeginDownload(url, new HttpSessionConfig(timeoutInterval), requestCallback, state);
		}

		public ICancelableAsyncResult BeginDownload(Uri url, HttpSessionConfig sessionConfig, AsyncCallback requestCallback, object state)
		{
			this.CheckDisposed();
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.EnterBeginDownload);
			this.InitializeState(sessionConfig);
			Exception exception = null;
			CancelableHttpAsyncResult cancelableHttpAsyncResult = new CancelableHttpAsyncResult(requestCallback, state, this);
			this.lastKnownRequestedUri = url;
			try
			{
				exception = this.BeginDownloadAction(url, cancelableHttpAsyncResult);
			}
			catch (WebException ex)
			{
				exception = ex;
			}
			catch (SecurityException ex2)
			{
				exception = ex2;
			}
			catch (IOException ex3)
			{
				exception = ex3;
			}
			catch (HttpWebRequestException ex4)
			{
				exception = ex4;
			}
			this.HandleException(cancelableHttpAsyncResult, exception);
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.LeaveBeginDownload);
			return cancelableHttpAsyncResult;
		}

		public DownloadResult EndDownload(ICancelableAsyncResult asyncResult)
		{
			this.CheckDisposed();
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.EnterEndDownload);
			CancelableHttpAsyncResult cancelableHttpAsyncResult = (CancelableHttpAsyncResult)asyncResult;
			HttpClient.SetEndCalled(cancelableHttpAsyncResult);
			if (!cancelableHttpAsyncResult.IsCompleted)
			{
				cancelableHttpAsyncResult.AsyncWaitHandle.WaitOne();
			}
			if (this.DownloadCompleted != null)
			{
				this.DownloadCompleted(this, new DownloadCompleteEventArgs(this.bytesReceived, this.bytesUploaded));
			}
			DownloadResult downloadResult = new DownloadResult(cancelableHttpAsyncResult.Exception);
			if (downloadResult.IsSucceeded || this.sessionConfig.ReadWebExceptionResponseStream)
			{
				if (this.responseStream != null)
				{
					this.responseStream.Seek(0L, SeekOrigin.Begin);
					downloadResult.ResponseStream = this.responseStream;
					this.responseStream = null;
				}
				downloadResult.LastModified = this.lastModified;
				downloadResult.ETag = this.eTag;
				downloadResult.BytesDownloaded = this.bytesReceived;
				downloadResult.ResponseUri = this.responseUri;
				downloadResult.StatusCode = this.statusCode;
				downloadResult.ResponseHeaders = this.responseHeaders;
				downloadResult.Cookies = this.cookies;
			}
			downloadResult.LastKnownRequestedUri = this.lastKnownRequestedUri;
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.LeaveEndDownload);
			this.LogDebug("Download IsSucceeded {0}", new object[]
			{
				downloadResult.IsSucceeded
			});
			return downloadResult;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal static string GetDisconnectReason(Exception exception)
		{
			if (exception == null)
			{
				return "Operation Completed";
			}
			return string.Format(CultureInfo.InvariantCulture, "Download Failure: {0},{1}", new object[]
			{
				exception.GetType(),
				exception.Message
			});
		}

		internal bool TryClose(string reason)
		{
			this.CheckDisposed();
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.EnterTryClose);
			lock (this.syncRoot)
			{
				if (this.sessionClosing)
				{
					this.LogDebug("Session Close already initiated ...");
					return false;
				}
				this.sessionClosing = true;
				if (this.httpWebRequest != null)
				{
					this.httpWebRequest.Abort();
					this.httpWebRequest = null;
				}
				if (this.httpRequestStream != null)
				{
					try
					{
						this.httpRequestStream.Flush();
						this.httpRequestStream.Dispose();
						this.httpRequestStream = null;
					}
					catch (WebException ex)
					{
						this.httpRequestStream = null;
						this.LogError("this.httpRequestStream.Dispose() hit exception: {0}", new object[]
						{
							ex
						});
					}
				}
				this.requestStream = null;
				if (this.httpResponseStream != null)
				{
					this.httpResponseStream.Flush();
					this.httpResponseStream.Dispose();
					this.httpResponseStream = null;
					this.LogDebug("HttpResponseStream is Closed");
				}
				if (!string.Equals(reason, "Operation Completed", StringComparison.OrdinalIgnoreCase) && this.responseStream != null)
				{
					this.responseStream.Flush();
					this.responseStream.Dispose();
					this.responseStream = null;
					this.LogDebug("ResponseStream is Closed");
				}
				this.UnRegisterTimeout();
				this.LogDisconnect(reason);
			}
			this.LogDebug("Session Closed Successfully");
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.LeaveTryClose);
			return true;
		}

		internal string GetBreadcrumbsSnapshot()
		{
			this.CheckDisposed();
			return this.breadcrumbs.ToString();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			lock (this.syncRoot)
			{
				if (disposing)
				{
					this.TryClose("Client Disposed");
					this.timeoutTimer.Dispose();
					this.timeoutTimer = null;
				}
				this.disposed = true;
			}
		}

		protected virtual IAsyncResult BeginGetResponse(object state, AsyncCallback callback, params object[] args)
		{
			return this.httpWebRequest.BeginGetResponse(callback, state);
		}

		protected virtual IAsyncResult BeginGetRequestStream(object state, AsyncCallback callback, params object[] args)
		{
			return this.httpWebRequest.BeginGetRequestStream(callback, state);
		}

		protected void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private static void SetEndCalled(CancelableHttpAsyncResult asyncResult)
		{
			if (asyncResult.EndCalled)
			{
				throw new InvalidOperationException("The End function can only be called once for each asynchronous operation.");
			}
			asyncResult.EndCalled = true;
		}

		private static byte[] GetHeadersByteArray(WebHeaderCollection headerCollection)
		{
			if (headerCollection != null)
			{
				string s = headerCollection.ToString().Replace(Environment.NewLine, " ");
				return Encoding.ASCII.GetBytes(s);
			}
			return null;
		}

		private static bool IsRedirect(HttpStatusCode statusCode)
		{
			return statusCode == HttpStatusCode.Found || statusCode == HttpStatusCode.MovedPermanently || statusCode == HttpStatusCode.MultipleChoices || statusCode == HttpStatusCode.TemporaryRedirect || statusCode == HttpStatusCode.SeeOther;
		}

		private void InitializeState(HttpSessionConfig sessionConfig)
		{
			this.urlRedirections = 0;
			this.bytesReceived = 0L;
			this.bytesUploaded = 0L;
			this.completedSynchronously = true;
			this.eTag = string.Empty;
			this.httpResponseStream = null;
			this.httpRequestStream = null;
			this.httpWebRequest = null;
			this.lastModified = null;
			this.responseStream = null;
			this.requestStream = null;
			this.sessionClosing = false;
			this.httpWebResponseContentLength = -1L;
			this.sessionConfig = sessionConfig;
			this.doesRequestContainsBody = (string.Equals(this.sessionConfig.Method, "POST", StringComparison.OrdinalIgnoreCase) || this.IsaMethodWithBody());
		}

		private bool IsaMethodWithBody()
		{
			foreach (string strA in HttpClient.verbsWithBody)
			{
				if (string.Compare(strA, this.sessionConfig.Method, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}

		private Exception BeginDownloadAction(Uri url, CancelableHttpAsyncResult cancelableAsyncResult)
		{
			Exception result = null;
			lock (this.syncRoot)
			{
				if (cancelableAsyncResult.IsCompleted)
				{
					return null;
				}
				if (this.sessionClosing)
				{
					return null;
				}
				if (!url.IsAbsoluteUri || (url.Scheme != Uri.UriSchemeHttp && url.Scheme != Uri.UriSchemeHttps))
				{
					result = new UnsupportedUriFormatException(url.ToString());
				}
				else
				{
					this.LogDebug("Download Start for: {0}", new object[]
					{
						url.AbsoluteUri
					});
					this.httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
					this.InitializeHttpSessionProperties();
					this.LogConnect(url.AbsoluteUri);
					this.LogSend(HttpClient.GetHeadersByteArray(this.httpWebRequest.Headers));
					if (this.doesRequestContainsBody && this.sessionConfig.RequestStream != null)
					{
						this.httpWebRequest.ContentType = this.sessionConfig.ContentType;
						this.httpWebRequest.ContentLength = this.sessionConfig.RequestStream.Length;
						this.requestStream = this.sessionConfig.RequestStream;
						this.BeginAsyncRequestWithTimeout(new HttpClient.AsyncRequest(this.BeginGetRequestStreamAction), cancelableAsyncResult, this.requestCallBack, new object[0]);
					}
					else
					{
						this.BeginAsyncRequestWithTimeout(new HttpClient.AsyncRequest(this.BeginGetResponseAction), cancelableAsyncResult, this.responseCallBack, new object[0]);
					}
				}
			}
			return result;
		}

		private void LogConnect(string context)
		{
			if (this.protocolLog != null)
			{
				this.protocolLog.LogOneEntry("HttpClient:Connect", null, ExecutionLog.EventType.Verbose, context, null);
			}
		}

		private void LogSend(byte[] data)
		{
			if (this.protocolLog != null)
			{
				this.protocolLog.LogOneEntry("HttpClient:Send", null, ExecutionLog.EventType.Information, Encoding.ASCII.GetString(data), null);
			}
		}

		private void LogReceive(byte[] data)
		{
			if (this.protocolLog != null)
			{
				this.protocolLog.LogOneEntry("HttpClient:Receive", null, ExecutionLog.EventType.Information, Encoding.ASCII.GetString(data), null);
			}
		}

		private void LogInformation(string informationString, bool isError = false)
		{
			if (this.protocolLog != null)
			{
				this.protocolLog.LogOneEntry("HttpClient", null, isError ? ExecutionLog.EventType.Error : ExecutionLog.EventType.Information, informationString, null);
			}
		}

		private void LogDebug(string formatString, params object[] args)
		{
			string informationString = string.Format(CultureInfo.InvariantCulture, formatString, args);
			this.LogDebug(informationString);
		}

		private void LogDebug(string informationString)
		{
			this.LogInformation(informationString, false);
		}

		private void LogError(string formatString, params object[] args)
		{
			string errorString = string.Format(CultureInfo.InvariantCulture, formatString, args);
			this.LogError(errorString);
		}

		private void LogError(string errorString)
		{
			this.LogInformation(errorString, true);
		}

		private void LogDisconnect(string reason)
		{
			if (this.protocolLog != null)
			{
				this.LogInformation("Bytes Downloaded: " + this.bytesReceived, false);
				this.protocolLog.LogOneEntry("HttpClient:Disconnect", null, ExecutionLog.EventType.Verbose, "Disconnect reason:" + reason, null);
			}
		}

		private void InitializeResponseStream()
		{
			this.responseStream = new MemoryStream();
		}

		private void InitializeHttpSessionProperties()
		{
			this.httpWebRequest.AllowAutoRedirect = (!this.doesRequestContainsBody && this.sessionConfig.AllowAutoRedirect);
			this.httpWebRequest.AuthenticationLevel = this.sessionConfig.AuthenticationLevel;
			this.httpWebRequest.CachePolicy = this.sessionConfig.CachePolicy;
			this.httpWebRequest.Credentials = this.sessionConfig.Credentials;
			HttpWebRequest.DefaultMaximumErrorResponseLength = this.sessionConfig.DefaultMaximumErrorResponseLength;
			this.httpWebRequest.ImpersonationLevel = this.sessionConfig.ImpersonationLevel;
			this.httpWebRequest.KeepAlive = this.sessionConfig.KeepAlive;
			this.httpWebRequest.MaximumAutomaticRedirections = this.sessionConfig.MaximumAutomaticRedirections;
			this.httpWebRequest.MaximumResponseHeadersLength = this.sessionConfig.MaximumResponseHeadersLength;
			this.httpWebRequest.Method = this.sessionConfig.Method;
			this.httpWebRequest.Pipelined = this.sessionConfig.Pipelined;
			this.httpWebRequest.PreAuthenticate = this.sessionConfig.PreAuthenticate;
			this.httpWebRequest.ProtocolVersion = this.sessionConfig.ProtocolVersion;
			this.httpWebRequest.Proxy = this.sessionConfig.Proxy;
			this.httpWebRequest.UnsafeAuthenticatedConnectionSharing = this.sessionConfig.UnsafeAuthenticatedConnectionSharing;
			this.httpWebRequest.UserAgent = this.sessionConfig.UserAgent;
			if (!string.IsNullOrEmpty(this.sessionConfig.IfNoneMatch))
			{
				this.httpWebRequest.Headers.Add(HttpRequestHeader.IfNoneMatch, this.sessionConfig.IfNoneMatch);
			}
			if (this.sessionConfig.IfModifiedSince != null)
			{
				this.httpWebRequest.IfModifiedSince = this.sessionConfig.IfModifiedSince.Value;
			}
			if (!string.IsNullOrEmpty(this.sessionConfig.IfHeader))
			{
				this.httpWebRequest.Headers.Add("If", this.sessionConfig.IfHeader);
			}
			if (this.sessionConfig.Headers != null)
			{
				this.httpWebRequest.Headers.Add(this.sessionConfig.Headers);
			}
			if (this.sessionConfig.ClientCertificates != null)
			{
				this.httpWebRequest.ClientCertificates = this.sessionConfig.ClientCertificates;
			}
			if (this.sessionConfig.CookieContainer != null)
			{
				this.httpWebRequest.CookieContainer = this.sessionConfig.CookieContainer;
			}
			if (this.sessionConfig.Rows != null)
			{
				this.httpWebRequest.AddRange("rows", 0, this.sessionConfig.Rows.Value);
			}
			if (!string.IsNullOrEmpty(this.sessionConfig.Accept))
			{
				this.httpWebRequest.Accept = this.sessionConfig.Accept;
			}
			if (this.sessionConfig.Expect100Continue != null)
			{
				this.httpWebRequest.ServicePoint.Expect100Continue = this.sessionConfig.Expect100Continue.Value;
			}
		}

		private void ResponseCallBack(object asyncResult)
		{
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.EnterResponseCallback);
			this.AsyncCallBack(new HttpClient.AsyncCallbackAction(this.ResponseCallBackAction), (IAsyncResult)asyncResult);
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.LeaveResponseCallback);
		}

		private Exception ResponseCallBackAction(IAsyncResult asyncResult)
		{
			Exception result = null;
			bool flag = false;
			bool flag2 = false;
			Uri url = null;
			CancelableHttpAsyncResult cancelableHttpAsyncResult = (CancelableHttpAsyncResult)asyncResult.AsyncState;
			lock (this.syncRoot)
			{
				if (cancelableHttpAsyncResult.IsCompleted)
				{
					return null;
				}
				if (this.sessionClosing)
				{
					return null;
				}
				this.UnRegisterTimeout();
				HttpWebResponse httpWebResponse = null;
				try
				{
					httpWebResponse = (HttpWebResponse)this.httpWebRequest.EndGetResponse(asyncResult);
				}
				catch (WebException ex)
				{
					if (!this.sessionConfig.ReadWebExceptionResponseStream || ex.Response == null)
					{
						throw;
					}
					httpWebResponse = (HttpWebResponse)ex.Response;
					cancelableHttpAsyncResult.Exception = ex;
				}
				this.LogDebug("Response Uri: {0}", new object[]
				{
					httpWebResponse.ResponseUri
				});
				this.LogReceive(HttpClient.GetHeadersByteArray(httpWebResponse.Headers));
				EventHandler<HttpWebResponseEventArgs> responseReceived = this.ResponseReceived;
				if (responseReceived != null)
				{
					responseReceived(this, new HttpWebResponseEventArgs(this.httpWebRequest, httpWebResponse));
				}
				if (this.sessionConfig.AllowAutoRedirect && this.doesRequestContainsBody && HttpClient.IsRedirect(httpWebResponse.StatusCode))
				{
					this.LogDebug("Redirected to: {0}", new object[]
					{
						httpWebResponse.Headers[HttpResponseHeader.Location]
					});
					if (this.urlRedirections < this.sessionConfig.MaximumAutomaticRedirections)
					{
						this.sessionConfig.RequestStream.Position = 0L;
						string text = httpWebResponse.Headers[HttpResponseHeader.Location] + httpWebResponse.ResponseUri.Query;
						try
						{
							url = new Uri(text);
							this.urlRedirections++;
							this.lastKnownRequestedUri = url;
							flag2 = true;
							goto IL_2C2;
						}
						catch (UriFormatException innerException)
						{
							result = new BadRedirectedUriException(text, innerException);
							goto IL_2C2;
						}
					}
					result = new WebException("Too many automatic redirections were attempted.", null, WebExceptionStatus.ProtocolError, httpWebResponse);
				}
				else
				{
					this.lastKnownRequestedUri = httpWebResponse.ResponseUri;
					if (this.sessionConfig.MaximumResponseBodyLength != -1L && this.sessionConfig.MaximumResponseBodyLength < httpWebResponse.ContentLength)
					{
						result = new DownloadLimitExceededException(this.sessionConfig.MaximumResponseBodyLength);
					}
					else
					{
						this.responseUri = httpWebResponse.ResponseUri;
						this.statusCode = httpWebResponse.StatusCode;
						this.responseHeaders = httpWebResponse.Headers;
						this.cookies = httpWebResponse.Cookies;
						this.httpWebResponseContentLength = httpWebResponse.ContentLength;
						string s = httpWebResponse.Headers[HttpResponseHeader.LastModified];
						DateTime value;
						if (DateTime.TryParse(s, out value))
						{
							this.lastModified = new DateTime?(value);
						}
						string text2 = httpWebResponse.Headers[HttpResponseHeader.ETag];
						if (!string.IsNullOrEmpty(text2) && text2.Length <= this.sessionConfig.MaxETagLength)
						{
							this.eTag = text2;
						}
						this.httpResponseStream = httpWebResponse.GetResponseStream();
						flag = true;
					}
				}
				IL_2C2:;
			}
			if (flag2)
			{
				result = this.BeginDownloadAction(url, cancelableHttpAsyncResult);
			}
			else if (flag)
			{
				this.BeginAsyncRequestWithTimeout(new HttpClient.AsyncRequest(this.BeginRead), cancelableHttpAsyncResult, this.readResponseCallBack, new object[]
				{
					this.httpResponseStream
				});
			}
			return result;
		}

		private void RequestCallBack(object asyncResult)
		{
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.EnterRequestCallback);
			this.AsyncCallBack(new HttpClient.AsyncCallbackAction(this.RequestCallBackAction), (IAsyncResult)asyncResult);
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.LeaveRequestCallback);
		}

		private Exception RequestCallBackAction(IAsyncResult asyncResult)
		{
			CancelableHttpAsyncResult cancelableHttpAsyncResult = (CancelableHttpAsyncResult)asyncResult.AsyncState;
			lock (this.syncRoot)
			{
				if (cancelableHttpAsyncResult.IsCompleted)
				{
					return null;
				}
				if (this.sessionClosing)
				{
					return null;
				}
				this.UnRegisterTimeout();
				this.httpRequestStream = this.httpWebRequest.EndGetRequestStream(asyncResult);
			}
			this.BeginAsyncRequestWithTimeout(new HttpClient.AsyncRequest(this.BeginRead), cancelableHttpAsyncResult, this.readRequestCallBack, new object[]
			{
				this.requestStream
			});
			return null;
		}

		private void ReadResponseCallBack(object asyncResult)
		{
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.EnterReadCallback);
			this.AsyncCallBack(new HttpClient.AsyncCallbackAction(this.ReadResponseCallBackAction), (IAsyncResult)asyncResult);
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.LeaveReadCallback);
		}

		private Exception ReadResponseCallBackAction(IAsyncResult asyncResult)
		{
			CancelableHttpAsyncResult cancelableHttpAsyncResult = (CancelableHttpAsyncResult)asyncResult.AsyncState;
			Exception result = null;
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			lock (this.syncRoot)
			{
				if (cancelableHttpAsyncResult.IsCompleted)
				{
					return null;
				}
				if (this.sessionClosing)
				{
					return null;
				}
				this.UnRegisterTimeout();
				num = this.httpResponseStream.EndRead(asyncResult);
				if (num > 0)
				{
					this.bytesReceived += (long)num;
					if (this.sessionConfig.MaximumResponseBodyLength != -1L && this.sessionConfig.MaximumResponseBodyLength < this.bytesReceived)
					{
						result = new DownloadLimitExceededException(this.sessionConfig.MaximumResponseBodyLength);
					}
					else if (this.httpWebResponseContentLength >= 0L && this.httpWebResponseContentLength < this.bytesReceived)
					{
						result = new ServerProtocolViolationException(this.httpWebResponseContentLength);
					}
					else
					{
						if (this.responseStream == null)
						{
							this.InitializeResponseStream();
						}
						flag2 = true;
					}
				}
				else
				{
					flag = this.TryClose("Operation Completed");
				}
			}
			if (flag2)
			{
				this.BeginAsyncRequestWithTimeout(new HttpClient.AsyncRequest(this.BeginWrite), cancelableHttpAsyncResult, this.writeResponseCallBack, new object[]
				{
					this.responseStream,
					num
				});
			}
			else if (flag)
			{
				this.LogDebug("Complete contents Downloaded");
				cancelableHttpAsyncResult.InvokeCompleted();
			}
			return result;
		}

		private void ReadRequestCallBack(IAsyncResult asyncResult)
		{
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.EnterReadCallback);
			this.AsyncCallBack(new HttpClient.AsyncCallbackAction(this.ReadRequestCallBackAction), asyncResult);
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.LeaveReadCallback);
		}

		private Exception ReadRequestCallBackAction(IAsyncResult asyncResult)
		{
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			CancelableHttpAsyncResult cancelableHttpAsyncResult = (CancelableHttpAsyncResult)asyncResult.AsyncState;
			lock (this.syncRoot)
			{
				if (cancelableHttpAsyncResult.IsCompleted)
				{
					return null;
				}
				if (this.sessionClosing)
				{
					return null;
				}
				this.UnRegisterTimeout();
				this.bytesUploaded = this.requestStream.Position;
				num = this.requestStream.EndRead(asyncResult);
				if (num > 0)
				{
					flag2 = true;
				}
				else
				{
					this.httpRequestStream.Flush();
					this.httpRequestStream.Dispose();
					this.httpRequestStream = null;
					flag = true;
				}
			}
			if (flag2)
			{
				this.BeginAsyncRequestWithTimeout(new HttpClient.AsyncRequest(this.BeginWrite), cancelableHttpAsyncResult, this.writeRequestCallBack, new object[]
				{
					this.httpRequestStream,
					num
				});
			}
			else if (flag)
			{
				this.LogDebug("Complete request written");
				this.BeginAsyncRequestWithTimeout(new HttpClient.AsyncRequest(this.BeginGetResponseAction), cancelableHttpAsyncResult, this.responseCallBack, new object[0]);
			}
			return null;
		}

		private void WriteResponseCallBack(IAsyncResult asyncResult)
		{
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.EnterWriteCallback);
			this.AsyncCallBack(new HttpClient.AsyncCallbackAction(this.WriteResponseCallBackAction), asyncResult);
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.LeaveWriteCallback);
		}

		private Exception WriteResponseCallBackAction(IAsyncResult asyncResult)
		{
			return this.WriteCallBackAction(asyncResult, this.responseStream, this.httpResponseStream, this.readResponseCallBack);
		}

		private void WriteRequestCallBack(object asyncResult)
		{
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.EnterWriteCallback);
			this.AsyncCallBack(new HttpClient.AsyncCallbackAction(this.WriteRequestCallBackAction), (IAsyncResult)asyncResult);
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.LeaveWriteCallback);
		}

		private AsyncCallback WrapCallbackWithUnhandledExceptionRedirect(WaitCallback callback)
		{
			return delegate(IAsyncResult asyncResult)
			{
				try
				{
					GrayException.MapAndReportGrayExceptions(delegate()
					{
						callback(asyncResult);
					});
				}
				catch (GrayException)
				{
					this.breadcrumbs.Drop(HttpClient.Breadcrumbs.CrashOnAnotherThread);
				}
			};
		}

		private Exception WriteRequestCallBackAction(IAsyncResult asyncResult)
		{
			return this.WriteCallBackAction(asyncResult, this.httpRequestStream, this.requestStream, this.readRequestCallBack);
		}

		private Exception WriteCallBackAction(IAsyncResult asyncResult, Stream writeStream, Stream readStream, AsyncCallback readCallBack)
		{
			CancelableHttpAsyncResult cancelableHttpAsyncResult = (CancelableHttpAsyncResult)asyncResult.AsyncState;
			lock (this.syncRoot)
			{
				if (cancelableHttpAsyncResult.IsCompleted)
				{
					return null;
				}
				if (this.sessionClosing)
				{
					return null;
				}
				this.UnRegisterTimeout();
				writeStream.EndWrite(asyncResult);
			}
			this.BeginAsyncRequestWithTimeout(new HttpClient.AsyncRequest(this.BeginRead), cancelableHttpAsyncResult, readCallBack, new object[]
			{
				readStream
			});
			return null;
		}

		private void AsyncCallBack(HttpClient.AsyncCallbackAction callbackAction, IAsyncResult asyncResult)
		{
			Exception exception = null;
			try
			{
				exception = callbackAction(asyncResult);
			}
			catch (WebException ex)
			{
				exception = ex;
			}
			catch (IOException ex2)
			{
				exception = ex2;
			}
			catch (SecurityException ex3)
			{
				exception = ex3;
			}
			catch (HttpWebRequestException ex4)
			{
				exception = ex4;
			}
			this.HandleException((CancelableHttpAsyncResult)asyncResult.AsyncState, exception);
		}

		private void TimeoutCallback(object ignored)
		{
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.EnterTimeoutCallback);
			IAsyncResult asyncResult = null;
			bool flag = false;
			Exception ex = new DownloadTimeoutException();
			lock (this.syncRoot)
			{
				asyncResult = this.pendingAsyncResult;
				if (asyncResult == null || asyncResult.IsCompleted)
				{
					return;
				}
				CancelableHttpAsyncResult cancelableHttpAsyncResult = (CancelableHttpAsyncResult)asyncResult.AsyncState;
				if (cancelableHttpAsyncResult.IsCompleted)
				{
					return;
				}
				if (this.sessionClosing)
				{
					return;
				}
				DateTime utcNow = DateTime.UtcNow;
				if (utcNow.Ticks < this.requestTimeout.Ticks)
				{
					this.LogDebug("This request is not supposed to be timed-out yet. UtcNow:{0}, RequestTimeout: {1}.", new object[]
					{
						utcNow.Ticks,
						this.requestTimeout.Ticks
					});
					return;
				}
				flag = this.TryClose(HttpClient.GetDisconnectReason(ex));
			}
			if (flag)
			{
				this.LogDebug("Timeout Occured");
				CancelableHttpAsyncResult cancelableHttpAsyncResult2 = (CancelableHttpAsyncResult)asyncResult.AsyncState;
				cancelableHttpAsyncResult2.InvokeCompleted(ex);
			}
			this.breadcrumbs.Drop(HttpClient.Breadcrumbs.LeaveTimeoutCallback);
		}

		private IAsyncResult IssueHttpWebRequest(HttpClient.AsyncRequest asyncRequest, object state, AsyncCallback callback, params object[] args)
		{
			IAsyncResult result = null;
			Exception ex = null;
			CancelableHttpAsyncResult state2 = state as CancelableHttpAsyncResult;
			bool flag = true;
			int num = 0;
			while (flag && num < 2)
			{
				if (num > 0)
				{
					Thread.Sleep(100);
				}
				flag = false;
				ex = null;
				try
				{
					result = asyncRequest(state2, callback, args);
				}
				catch (InvalidOperationException ex2)
				{
					ex = ex2;
					flag = true;
				}
				num++;
			}
			if (ex != null)
			{
				throw new HttpWebRequestException(ex);
			}
			return result;
		}

		private IAsyncResult BeginGetRequestStreamAction(object state, AsyncCallback callback, params object[] args)
		{
			EventHandler<HttpWebRequestEventArgs> sendingRequest = this.SendingRequest;
			if (sendingRequest != null)
			{
				sendingRequest(this, new HttpWebRequestEventArgs(this.httpWebRequest));
			}
			return this.IssueHttpWebRequest(new HttpClient.AsyncRequest(this.BeginGetRequestStream), state, callback, args);
		}

		private IAsyncResult BeginGetResponseAction(object state, AsyncCallback callback, params object[] args)
		{
			return this.IssueHttpWebRequest(new HttpClient.AsyncRequest(this.BeginGetResponse), state, callback, args);
		}

		private IAsyncResult BeginRead(object state, AsyncCallback callback, params object[] args)
		{
			Stream stream = (Stream)args[0];
			return stream.BeginRead(this.Buffer, 0, 4096, callback, state);
		}

		private IAsyncResult BeginWrite(object state, AsyncCallback callback, params object[] args)
		{
			Stream stream = (Stream)args[0];
			int count = (int)args[1];
			return stream.BeginWrite(this.Buffer, 0, count, callback, state);
		}

		private void BeginAsyncRequestWithTimeout(HttpClient.AsyncRequest asyncRequest, CancelableHttpAsyncResult cancelableHttpAsyncResult, AsyncCallback callback, params object[] args)
		{
			lock (this.syncRoot)
			{
				if (!cancelableHttpAsyncResult.IsCompleted)
				{
					if (!this.sessionClosing)
					{
						IAsyncResult asyncResult = asyncRequest(cancelableHttpAsyncResult, callback, args);
						if (!asyncResult.CompletedSynchronously)
						{
							this.completedSynchronously = false;
						}
						this.RegisterTimeout(asyncResult);
					}
				}
			}
		}

		private void UnRegisterTimeout()
		{
			lock (this.syncRoot)
			{
				this.timeoutTimer.Change(-1, -1);
				this.pendingAsyncResult = null;
			}
		}

		private void RegisterTimeout(IAsyncResult asyncResult)
		{
			lock (this.syncRoot)
			{
				if (!this.sessionClosing)
				{
					if (!asyncResult.IsCompleted)
					{
						this.requestTimeout = DateTime.UtcNow.AddMilliseconds((double)(this.sessionConfig.Timeout - 1000));
						this.timeoutTimer.Change(this.sessionConfig.Timeout, -1);
						this.pendingAsyncResult = asyncResult;
					}
				}
			}
		}

		private void HandleException(CancelableHttpAsyncResult cancelableAsyncResult, Exception exception)
		{
			if (exception != null)
			{
				string disconnectReason = HttpClient.GetDisconnectReason(exception);
				if (this.TryClose(disconnectReason))
				{
					this.LogError("Download Failed with Exception: {0}", new object[]
					{
						exception
					});
					cancelableAsyncResult.InvokeCompleted(exception);
				}
			}
		}

		public const string BPropPatchMethod = "BPROPPATCH";

		public const string DeleteMethod = "DELETE";

		public const string LockMethod = "LOCK";

		public const string MoveMethod = "MOVE";

		public const string PropFindMethod = "PROPFIND";

		public const string PropPatchMethod = "PROPPATCH";

		public const string SearchMethod = "SEARCH";

		public const string UnLockMethod = "UNLOCK";

		private const string Seperator = " ";

		private const string OperationCompleted = "Operation Completed";

		private const string ClientDisposed = "Client Disposed";

		private const string IfHeader = "If";

		private const int BufferSize = 4096;

		private const int NumberOfBreadcrumbs = 64;

		private static List<string> verbsWithBody = new List<string>
		{
			"BPROPPATCH",
			"LOCK",
			"MKCOL",
			"POST",
			"PROPFIND",
			"PROPPATCH",
			"PUT",
			"SEARCH",
			"UNLOCK"
		};

		private readonly ExecutionLog protocolLog;

		private readonly AsyncCallback responseCallBack;

		private readonly AsyncCallback requestCallBack;

		private readonly AsyncCallback readResponseCallBack;

		private readonly AsyncCallback writeResponseCallBack;

		private readonly AsyncCallback readRequestCallBack;

		private readonly AsyncCallback writeRequestCallBack;

		private object syncRoot = new object();

		private bool sessionClosing;

		private bool completedSynchronously;

		private HttpWebRequest httpWebRequest;

		private Stream httpResponseStream;

		private Stream responseStream;

		private Stream httpRequestStream;

		private Stream requestStream;

		private byte[] buffer;

		private Breadcrumbs<HttpClient.Breadcrumbs> breadcrumbs;

		private long httpWebResponseContentLength;

		private DateTime? lastModified;

		private string eTag;

		private long bytesReceived;

		private long bytesUploaded;

		private Timer timeoutTimer;

		private IAsyncResult pendingAsyncResult;

		private DateTime requestTimeout;

		private Uri responseUri;

		private Uri lastKnownRequestedUri;

		private HttpStatusCode statusCode;

		private WebHeaderCollection responseHeaders;

		private CookieCollection cookies;

		private HttpSessionConfig sessionConfig;

		private int urlRedirections;

		private bool doesRequestContainsBody;

		private volatile bool disposed;

		private delegate IAsyncResult AsyncRequest(object state, AsyncCallback callback, params object[] args);

		private delegate Exception AsyncCallbackAction(IAsyncResult asyncResult);

		private enum Breadcrumbs
		{
			EnterBeginDownload = 1,
			EnterEndDownload,
			EnterResponseCallback,
			EnterReadCallback,
			EnterWriteCallback,
			EnterTimeoutCallback,
			EnterTryClose,
			EnterRequestCallback,
			LeaveBeginDownload = 10,
			LeaveEndDownload,
			LeaveResponseCallback,
			LeaveReadCallback,
			LeaveWriteCallback,
			LeaveTimeoutCallback,
			LeaveTryClose,
			LeaveRequestCallback,
			CrashOnAnotherThread = 20
		}
	}
}
