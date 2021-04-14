using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MapiHttpClient;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ClientAsyncOperation : EasyCancelableAsyncResult
	{
		protected ClientAsyncOperation(ClientSessionContext context, CancelableAsyncCallback asyncCallback, object asyncState) : base(asyncCallback, asyncState)
		{
			this.context = context;
			this.perfDateTime = new PerfDateTime();
		}

		public ClientSessionContext Context
		{
			get
			{
				return this.context;
			}
		}

		public MapiHttpVersion ClientVersion { get; set; }

		public WebHeaderCollection HttpWebResponseHeaders
		{
			get
			{
				return this.context.ResponseHeaders;
			}
		}

		public WebHeaderCollection HttpWebRequestHeaders
		{
			get
			{
				if (this.httpWebRequest != null)
				{
					return ClientAsyncOperation.GetDisplayableHeaders(this.httpWebRequest.Headers);
				}
				return new WebHeaderCollection();
			}
		}

		public HttpStatusCode LastResponseStatusCode
		{
			get
			{
				return this.context.LastResponseStatusCode.GetValueOrDefault();
			}
		}

		public string LastResponseStatusDescription
		{
			get
			{
				return this.context.LastResponseStatusDescription;
			}
		}

		protected abstract string RequestType { get; }

		public bool TryGetServerVersion(out MapiHttpVersion version)
		{
			version = null;
			if (this.httpWebResponse == null)
			{
				return false;
			}
			string text = this.httpWebResponse.Headers.Get("X-ServerApplication");
			if (!string.IsNullOrWhiteSpace(text))
			{
				int num = text.IndexOf('/');
				if (num >= 0)
				{
					return MapiHttpVersion.TryParse(text.Substring(num + 1), out version);
				}
			}
			return false;
		}

		protected void InternalBegin(Action<Writer> serializer)
		{
			bool flag = false;
			try
			{
				int num = 0;
				if (serializer != null)
				{
					using (CountWriter countWriter = new CountWriter())
					{
						serializer(countWriter);
						num = (int)countWriter.Position;
					}
				}
				if (num > 268288)
				{
					throw new InvalidOperationException(string.Format("Request is larger than the maximum request size allowed; size={0}, maxSize={1}", num, 268288));
				}
				if (num > 0)
				{
					this.requestBuffer = new WorkBuffer(num);
					using (BufferWriter bufferWriter = new BufferWriter(this.requestBuffer.ArraySegment))
					{
						serializer(bufferWriter);
						this.requestData = new ArraySegment<byte>(this.requestBuffer.Array, this.requestBuffer.Offset, (int)bufferWriter.Position);
					}
				}
				this.readResponseBuffer = new WorkBuffer(32768);
				this.httpWebRequest = this.context.CreateRequest(out this.requestId);
				this.httpWebRequest.ContentLength = (long)this.requestData.Count;
				this.httpWebRequest.Headers.Add("X-RequestType", this.RequestType);
				if (this.ClientVersion != null)
				{
					this.httpWebRequest.Headers.Add("X-ClientApplication", string.Format("{0}/{1}", "MapiHttpClient", this.ClientVersion));
				}
				else
				{
					this.httpWebRequest.Headers.Add("X-ClientApplication", ClientAsyncOperation.ClientApplication);
				}
				if (this.context.DesiredExpiration != null)
				{
					int num2 = (int)this.context.DesiredExpiration.Value.TotalMilliseconds;
					this.httpWebRequest.Headers.Add("X-ExpirationInfo", num2.ToString());
				}
				this.context.UpdateElapsedTime(null);
				ExTraceGlobals.ClientAsyncOperationTracer.TraceInformation(51616, 0L, "{0}.Begin; ContextHandle={1}, RequestId={2}, URI={3}", new object[]
				{
					this.RequestType,
					this.context.ContextHandle,
					this.requestId,
					this.context.RequestPath
				});
				this.context.SetAdditionalRequestHeaders(this.httpWebRequest);
				this.Execute(delegate
				{
					this.StartTimerWrapper(delegate
					{
						this.requestStartTime = new DateTime?(this.perfDateTime.UtcNow);
						this.httpWebRequest.BeginGetRequestStream(new AsyncCallback(ClientAsyncOperation.BeginGetRequestStreamCallbackEntry), this);
					});
				});
				ExTraceGlobals.ClientAsyncOperationTracer.TraceInformation<string, IntPtr, string>(45472, 0L, "{0}.Begin; Success; ContextHandle={1}, RequestId={2}", this.RequestType, this.context.ContextHandle, this.requestId);
				flag = true;
			}
			catch (Exception ex)
			{
				ExTraceGlobals.ClientAsyncOperationTracer.TraceInformation(61856, 0L, "{0}.Begin; Failed; ContextHandle={1}, RequestId={2}, Exception={3}", new object[]
				{
					this.RequestType,
					this.context.ContextHandle,
					this.requestId,
					ex
				});
				throw;
			}
			finally
			{
				if (!flag)
				{
					this.Cleanup();
				}
			}
		}

		protected ErrorCode InternalEnd(Func<Reader, int> parser)
		{
			ErrorCode errorCode = ErrorCode.None;
			ExTraceGlobals.ClientAsyncOperationTracer.TraceInformation<string, IntPtr, string>(37280, 0L, "{0}.End; ContextHandle={1}, RequestId={2}", this.RequestType, this.context.ContextHandle, this.requestId);
			try
			{
				base.WaitForCompletion();
				if (this.exception != null)
				{
					throw new AggregateException("Operation failed", this.exception);
				}
				if (this.httpWebResponse == null)
				{
					throw new InvalidOperationException("Should have a HttpWebResponse if no exception.");
				}
				if (this.httpWebResponse.StatusCode != HttpStatusCode.OK)
				{
					this.exceptionTime = new DateTime?(this.perfDateTime.UtcNow);
					throw ProtocolException.FromHttpStatusCode((LID)47372, string.Format("Server returned HttpStatusCode.{0} failure.", this.httpWebResponse.StatusCode), this.GetFailureContext(null), this.httpWebResponse.StatusCode, this.httpWebResponse.StatusDescription, this.httpWebRequest.Headers, this.httpWebResponse.Headers, null);
				}
				if (this.responseParser == null)
				{
					throw new InvalidOperationException("Should have a ResponseParser on HttpStatusCode.OK");
				}
				if (this.responseParser.ResponseCode != ResponseCode.Success)
				{
					MapiHttpVersion mapiHttpVersion;
					if (!this.TryGetServerVersion(out mapiHttpVersion))
					{
						mapiHttpVersion = null;
					}
					this.exceptionTime = new DateTime?(this.perfDateTime.UtcNow);
					throw ProtocolException.FromResponseCode((LID)49184, string.Format("Server returned ResponseCode.{0} failure.", this.responseParser.ResponseCode), this.GetFailureContext(null), this.httpWebResponse.StatusCode, this.httpWebResponse.StatusDescription, this.responseParser.ResponseCode, null, this.httpWebRequest.Headers, this.httpWebResponse.Headers, mapiHttpVersion);
				}
				if (parser != null)
				{
					using (BufferReader bufferReader = new BufferReader(this.responseParser.ResponseData.DeepClone<byte>()))
					{
						this.CheckStatusCodeAndThrowOnFailedResponse(bufferReader);
						errorCode = (ErrorCode)parser(bufferReader);
						goto IL_206;
					}
				}
				if (this.responseParser.ResponseData.Count > 0)
				{
					throw new InvalidOperationException(string.Format("Operation {0} didn't supply a response parser and response was returned from server; size={1}", this.RequestType, this.responseParser.ResponseData.Count));
				}
				IL_206:
				if (this.context != null)
				{
					this.context.UpdateElapsedTime(this.responseParser.ElapsedTime);
					TimeSpan expiration;
					if (this.TryGetExpirationInfo(out expiration))
					{
						this.context.UpdateRefresh(expiration);
					}
				}
				ExTraceGlobals.ClientAsyncOperationTracer.TraceInformation(53664, 0L, "{0}.End; Success; ContextHandle={1}, RequestId={2}, ErrorCode={3}", new object[]
				{
					this.RequestType,
					this.context.ContextHandle,
					this.requestId,
					errorCode
				});
			}
			catch (Exception ex)
			{
				ExTraceGlobals.ClientAsyncOperationTracer.TraceInformation(41376, 0L, "{0}.End; Failed; ContextHandle={1}, RequestId={2}, Exception={3}", new object[]
				{
					this.RequestType,
					this.context.ContextHandle,
					this.requestId,
					ex
				});
				throw;
			}
			finally
			{
				this.Cleanup();
			}
			return errorCode;
		}

		protected override void InternalCancel()
		{
			try
			{
				this.StopTimeoutTimer();
				if (this.httpWebRequest != null)
				{
					this.httpWebRequest.Abort();
					ExTraceGlobals.ClientAsyncOperationTracer.TraceInformation<string, IntPtr, string>(43132, 0L, "{0}.Cancel; Success; ContextHandle={1}, RequestId={2}", this.RequestType, this.context.ContextHandle, this.requestId);
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.ClientAsyncOperationTracer.TraceInformation(59516, 0L, "{0}.Cancel; Failed; ContextHandle={1}, RequestId={2}, Exception={3}", new object[]
				{
					this.RequestType,
					this.context.ContextHandle,
					this.requestId,
					ex
				});
			}
		}

		private static void AppendHeaders(StringBuilder stringBuilder, WebHeaderCollection headers)
		{
			WebHeaderCollection displayableHeaders = ClientAsyncOperation.GetDisplayableHeaders(headers);
			if (displayableHeaders.Count > 0)
			{
				for (int i = 0; i < displayableHeaders.Count; i++)
				{
					stringBuilder.AppendFormat("{0}: {1} \r\n", displayableHeaders.Keys[i], displayableHeaders[i]);
				}
			}
		}

		private static WebHeaderCollection GetDisplayableHeaders(WebHeaderCollection headers)
		{
			WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
			if (headers != null && headers.Count > 0)
			{
				for (int i = 0; i < headers.Count; i++)
				{
					string value;
					if (ClientAsyncOperation.TryGetDisplayableHeader(headers.Keys[i], headers[i], out value))
					{
						webHeaderCollection.Add(headers.Keys[i], value);
					}
				}
			}
			return webHeaderCollection;
		}

		private static bool TryGetDisplayableHeader(string key, string header, out string displayableHeader)
		{
			displayableHeader = header;
			if (string.Compare(key, "authorization", true) == 0 || string.Compare(key, "WWW-Authenticate", true) == 0)
			{
				int num = header.IndexOf(" ");
				if (num >= 0)
				{
					displayableHeader = string.Format("{0} [truncated]", header.Substring(0, num));
				}
			}
			return true;
		}

		private static string GetMinimalElapsedTime(TimeSpan elapsedTime)
		{
			if (elapsedTime < TimeSpan.FromSeconds(1.0))
			{
				return elapsedTime.ToString("s\\.fff");
			}
			if (elapsedTime < TimeSpan.FromMinutes(1.0))
			{
				return elapsedTime.ToString("m\\:ss\\.fff");
			}
			if (elapsedTime < TimeSpan.FromHours(1.0))
			{
				return elapsedTime.ToString("h\\:mm\\:ss\\.fff");
			}
			return elapsedTime.ToString("c");
		}

		private static void StopTimerWrapper(IAsyncResult asyncResult, Action<ClientAsyncOperation> action)
		{
			ClientAsyncOperation clientAsyncOperation = (ClientAsyncOperation)asyncResult.AsyncState;
			if (clientAsyncOperation != null)
			{
				clientAsyncOperation.StopTimeoutTimer();
				action(clientAsyncOperation);
			}
		}

		private static void BeginGetRequestStreamCallbackEntry(IAsyncResult asyncResult)
		{
			ClientAsyncOperation.StopTimerWrapper(asyncResult, delegate(ClientAsyncOperation operation)
			{
				operation.BeginGetRequestStreamCallback(asyncResult);
			});
		}

		private static void BeginWriteCallbackEntry(IAsyncResult asyncResult)
		{
			ClientAsyncOperation.StopTimerWrapper(asyncResult, delegate(ClientAsyncOperation operation)
			{
				operation.BeginWriteCallback(asyncResult);
			});
		}

		private static void BeginGetResponseCallbackEntry(IAsyncResult asyncResult)
		{
			ClientAsyncOperation.StopTimerWrapper(asyncResult, delegate(ClientAsyncOperation operation)
			{
				operation.BeginGetResponseCallback(asyncResult);
			});
		}

		private static void BeginReadCallbackEntry(IAsyncResult asyncResult)
		{
			ClientAsyncOperation.StopTimerWrapper(asyncResult, delegate(ClientAsyncOperation operation)
			{
				operation.BeginReadCallback(asyncResult);
			});
		}

		private void ResponseTimeoutCallback(object stateInfo)
		{
			try
			{
				if (this.httpWebRequest != null)
				{
					this.httpWebRequest.Abort();
					ExTraceGlobals.ClientAsyncOperationTracer.TraceInformation<string, IntPtr, string>(34940, 0L, "{0}.AbortOnTimeout; Success; ContextHandle={1}, RequestId={2}", this.RequestType, this.context.ContextHandle, this.requestId);
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.ClientAsyncOperationTracer.TraceInformation(51324, 0L, "{0}.AbortOnTimeout; Failed; ContextHandle={1}, RequestId={2}, Exception={3}", new object[]
				{
					this.RequestType,
					this.context.ContextHandle,
					this.requestId,
					ex
				});
			}
		}

		private void BeginGetRequestStreamCallback(IAsyncResult asyncResult)
		{
			this.Execute(delegate
			{
				this.requestStream = this.httpWebRequest.EndGetRequestStream(asyncResult);
				if (this.requestBuffer != null && this.requestData.Count > 0)
				{
					this.StartTimerWrapper(delegate
					{
						this.requestWriteTime = new DateTime?(this.perfDateTime.UtcNow);
						this.requestStream.BeginWrite(this.requestData.Array, this.requestData.Offset, this.requestData.Count, new AsyncCallback(ClientAsyncOperation.BeginWriteCallbackEntry), this);
					});
					return;
				}
				this.requestStream.Close();
				this.requestStream = null;
				this.StartTimerWrapper(delegate
				{
					this.requestSentTime = new DateTime?(this.perfDateTime.UtcNow);
					this.httpWebRequest.BeginGetResponse(new AsyncCallback(ClientAsyncOperation.BeginGetResponseCallbackEntry), this);
				});
			});
		}

		private void BeginWriteCallback(IAsyncResult asyncResult)
		{
			this.Execute(delegate
			{
				this.requestStream.EndWrite(asyncResult);
				this.requestStream.Close();
				this.requestStream = null;
				this.StartTimerWrapper(delegate
				{
					this.requestSentTime = new DateTime?(this.perfDateTime.UtcNow);
					this.httpWebRequest.BeginGetResponse(new AsyncCallback(ClientAsyncOperation.BeginGetResponseCallbackEntry), this);
				});
			});
		}

		private void BeginGetResponseCallback(IAsyncResult asyncResult)
		{
			this.Execute(delegate
			{
				try
				{
					this.responseStartTime = new DateTime?(this.perfDateTime.UtcNow);
					this.httpWebResponse = (HttpWebResponse)this.httpWebRequest.EndGetResponse(asyncResult);
				}
				catch (WebException ex)
				{
					HttpWebResponse httpWebResponse = ex.Response as HttpWebResponse;
					if (httpWebResponse == null)
					{
						throw;
					}
					this.httpWebResponse = httpWebResponse;
				}
				this.context.Update(this.httpWebResponse);
				HttpStatusCode statusCode = this.GetStatusCode();
				ResponseCode responseCode = ResponseCode.Success;
				if (statusCode == HttpStatusCode.OK)
				{
					responseCode = this.GetResponseCode();
				}
				this.responseParser = new PendingResponseParser(statusCode, responseCode, 1054720, this.perfDateTime);
				this.responseStream = this.httpWebResponse.GetResponseStream();
				this.UpdateTimeoutTimer(this.httpWebResponse);
				this.StartTimerWrapper(delegate
				{
					this.responseStream.BeginRead(this.readResponseBuffer.Array, this.readResponseBuffer.Offset, this.readResponseBuffer.Count, new AsyncCallback(ClientAsyncOperation.BeginReadCallbackEntry), this);
				});
			});
		}

		private void BeginReadCallback(IAsyncResult asyncResult)
		{
			this.Execute(delegate
			{
				if (this.responseReadTime == null)
				{
					this.responseReadTime = new DateTime?(this.perfDateTime.UtcNow);
				}
				int num = this.responseStream.EndRead(asyncResult);
				if (num == 0)
				{
					this.responseReceivedTime = new DateTime?(this.perfDateTime.UtcNow);
					this.responseParser.Done();
					this.InvokeCallback();
					return;
				}
				this.responseParser.PutData(new ArraySegment<byte>(this.readResponseBuffer.Array, this.readResponseBuffer.Offset, num));
				this.StartTimerWrapper(delegate
				{
					this.responseStream.BeginRead(this.readResponseBuffer.Array, this.readResponseBuffer.Offset, this.readResponseBuffer.Count, new AsyncCallback(ClientAsyncOperation.BeginReadCallbackEntry), this);
				});
			});
		}

		private void UpdateTimeoutTimer(HttpWebResponse response)
		{
			string text = response.Headers.Get("X-PendingPeriod");
			int num;
			if (!string.IsNullOrEmpty(text) && int.TryParse(text, out num))
			{
				this.httpRequestTimeout = num * 2;
			}
		}

		private void StartTimeoutTimer()
		{
			if (this.httpRequestTimeoutTimer == null)
			{
				this.httpRequestTimeoutTimer = new Timer(new TimerCallback(this.ResponseTimeoutCallback), null, this.httpRequestTimeout, 0);
				return;
			}
			this.httpRequestTimeoutTimer.Change(this.httpRequestTimeout, 0);
		}

		private void StopTimeoutTimer()
		{
			if (this.httpRequestTimeoutTimer != null)
			{
				this.httpRequestTimeoutTimer.Change(-1, -1);
			}
		}

		private HttpStatusCode GetStatusCode()
		{
			if (this.httpWebResponse == null)
			{
				throw new InvalidOperationException("Attempting to retrieve response status with no valid HttpWebResponse object");
			}
			return this.httpWebResponse.StatusCode;
		}

		private ResponseCode GetResponseCode()
		{
			if (this.httpWebResponse == null)
			{
				throw new InvalidOperationException("Attempting to retrieve response status with no valid HttpWebResponse object");
			}
			string text = this.httpWebResponse.Headers.Get("X-ResponseCode");
			if (string.IsNullOrWhiteSpace(text))
			{
				throw ProtocolException.FromResponseCode((LID)56864, string.Format("Server didn't return {0} header", "X-ResponseCode"), this.GetFailureContext(null), this.httpWebResponse.StatusCode, this.httpWebResponse.StatusDescription, ResponseCode.MissingHeader, null, this.httpWebRequest.Headers, this.httpWebResponse.Headers, null);
			}
			int result;
			if (!int.TryParse(text, out result))
			{
				throw ProtocolException.FromResponseCode((LID)44576, string.Format("Unable to parse an int value from {0} header; found={1}", "X-ResponseCode", text), this.GetFailureContext(null), this.httpWebResponse.StatusCode, this.httpWebResponse.StatusDescription, ResponseCode.InvalidHeader, null, this.httpWebRequest.Headers, this.httpWebResponse.Headers, null);
			}
			return (ResponseCode)result;
		}

		private bool TryGetExpirationInfo(out TimeSpan expiration)
		{
			expiration = TimeSpan.Zero;
			if (this.httpWebResponse == null)
			{
				throw new InvalidOperationException("Attempting to retrieve response session expiration information with no valid HttpWebResponse object");
			}
			string text = this.httpWebResponse.Headers.Get("X-ExpirationInfo");
			if (string.IsNullOrWhiteSpace(text))
			{
				return false;
			}
			int num;
			if (!int.TryParse(text, out num))
			{
				return false;
			}
			expiration = TimeSpan.FromMilliseconds((double)num);
			return true;
		}

		private string GetFailureContext(string remoteExceptionTrace = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.GetHttpRequest(stringBuilder);
			this.GetHttpResponse(stringBuilder);
			if (remoteExceptionTrace != null)
			{
				stringBuilder.Append("\r\n###### REMOTE-EXCEPTION-INFO ######\r\n\r\n");
				stringBuilder.Append(remoteExceptionTrace);
			}
			if (this.exceptionTime != null)
			{
				string arg = string.Empty;
				if (this.requestStartTime != null)
				{
					TimeSpan elapsedTime = this.exceptionTime.Value - this.requestStartTime.Value;
					arg = string.Format(" [+{0}]", ClientAsyncOperation.GetMinimalElapsedTime(elapsedTime));
				}
				stringBuilder.AppendFormat("\r\n###### EXCEPTION THROWN{0} ######\r\n\r\n", arg);
			}
			return stringBuilder.ToString();
		}

		private void GetHttpRequest(StringBuilder stringBuilder)
		{
			if (this.httpWebRequest != null && this.requestStartTime != null)
			{
				DateTime value = this.requestStartTime.Value;
				stringBuilder.AppendFormat("\r\n\r\n###### REQUEST [{0}] ######\r\n\r\n", this.requestStartTime.Value.ToString("o"));
				stringBuilder.AppendFormat("{0} {1} HTTP/1.1\r\n", this.httpWebRequest.Method, this.httpWebRequest.RequestUri.PathAndQuery);
				ClientAsyncOperation.AppendHeaders(stringBuilder, this.httpWebRequest.Headers);
				if (this.requestWriteTime != null)
				{
					TimeSpan elapsedTime = this.requestWriteTime.Value - value;
					stringBuilder.AppendFormat("\r\n--- REQUEST BODY [+{0}] ---\r\n", ClientAsyncOperation.GetMinimalElapsedTime(elapsedTime));
					stringBuilder.AppendFormat("..[BODY SIZE: {0}]\r\n", this.httpWebRequest.ContentLength);
				}
				if (this.requestSentTime != null)
				{
					TimeSpan elapsedTime = this.requestSentTime.Value - value;
					stringBuilder.AppendFormat("\r\n--- REQUEST SENT [+{0}] ---\r\n", ClientAsyncOperation.GetMinimalElapsedTime(elapsedTime));
				}
			}
		}

		private void GetHttpResponse(StringBuilder stringBuilder)
		{
			if (this.httpWebResponse != null && this.requestStartTime != null && this.responseStartTime != null)
			{
				DateTime value = this.requestStartTime.Value;
				TimeSpan elapsedTime = this.responseStartTime.Value - value;
				stringBuilder.AppendFormat("\r\n###### RESPONSE [+{0}] ######\r\n\r\n", ClientAsyncOperation.GetMinimalElapsedTime(elapsedTime));
				stringBuilder.AppendFormat("HTTP/1.1 {0} {1}\r\n", (int)this.httpWebResponse.StatusCode, this.httpWebResponse.StatusDescription);
				ClientAsyncOperation.AppendHeaders(stringBuilder, this.httpWebResponse.Headers);
				if (this.responseReadTime != null)
				{
					elapsedTime = this.responseReadTime.Value - value;
					stringBuilder.AppendFormat("\r\n--- RESPONSE BODY [+{0}] ---\r\n", ClientAsyncOperation.GetMinimalElapsedTime(elapsedTime));
				}
				if (this.responseParser != null)
				{
					this.responseParser.AppendParserDiagnosticInformation(stringBuilder);
				}
				if (this.responseReceivedTime != null)
				{
					elapsedTime = this.responseReceivedTime.Value - value;
					stringBuilder.AppendFormat("\r\n--- RESPONSE DONE [+{0}] ---\r\n", ClientAsyncOperation.GetMinimalElapsedTime(elapsedTime));
				}
			}
		}

		private void CheckStatusCodeAndThrowOnFailedResponse(BufferReader reader)
		{
			ServiceCode serviceCode = (ServiceCode)reader.PeekUInt32(0L);
			if (serviceCode != ServiceCode.Success)
			{
				MapiHttpFailureResponse mapiHttpFailureResponse = new MapiHttpFailureResponse(reader);
				IEnumerable<AuxiliaryBlock> source = AuxiliaryData.ParseAuxiliaryBuffer(mapiHttpFailureResponse.AuxiliaryBuffer);
				ExceptionTraceAuxiliaryBlock exceptionTraceAuxiliaryBlock = source.OfType<ExceptionTraceAuxiliaryBlock>().FirstOrDefault<ExceptionTraceAuxiliaryBlock>();
				string remoteExceptionTrace = string.Empty;
				if (exceptionTraceAuxiliaryBlock != null)
				{
					remoteExceptionTrace = exceptionTraceAuxiliaryBlock.ExceptionTrace;
				}
				throw ProtocolException.FromServiceCode((LID)56412, string.Format("Server returned ServiceCode.{0} failure.", serviceCode), this.GetFailureContext(remoteExceptionTrace), this.httpWebResponse.StatusCode, this.httpWebResponse.StatusDescription, serviceCode, null, this.httpWebRequest.Headers, this.httpWebResponse.Headers);
			}
		}

		private void Execute(Action action)
		{
			try
			{
				action();
			}
			catch (Exception ex)
			{
				try
				{
					this.exceptionTime = new DateTime?(this.perfDateTime.UtcNow);
					WebException ex2 = ex as WebException;
					if (ex2 != null)
					{
						this.exception = ProtocolException.FromTransportException((LID)60960, "WebException thrown.", this.GetFailureContext(null), (this.httpWebResponse != null) ? this.httpWebResponse.StatusCode : HttpStatusCode.OK, (this.httpWebResponse != null) ? this.httpWebResponse.StatusDescription : string.Empty, ex, (this.httpWebRequest != null) ? this.httpWebRequest.Headers : null, (this.httpWebResponse != null) ? this.httpWebResponse.Headers : null);
					}
					else
					{
						IOException ex3 = ex as IOException;
						if (ex3 != null)
						{
							this.exception = ProtocolException.FromTransportException((LID)50444, "IOException thrown.", this.GetFailureContext(null), (this.httpWebResponse != null) ? this.httpWebResponse.StatusCode : HttpStatusCode.OK, (this.httpWebResponse != null) ? this.httpWebResponse.StatusDescription : string.Empty, ex, (this.httpWebRequest != null) ? this.httpWebRequest.Headers : null, (this.httpWebResponse != null) ? this.httpWebResponse.Headers : null);
						}
						else
						{
							this.exception = ex;
						}
					}
				}
				catch (Exception ex4)
				{
					this.exception = ex4;
				}
				base.InvokeCallback();
			}
		}

		private void StartTimerWrapper(Action action)
		{
			this.StartTimeoutTimer();
			try
			{
				action();
			}
			catch
			{
				this.StopTimeoutTimer();
				throw;
			}
		}

		private void Cleanup()
		{
			if (this.responseStream != null)
			{
				this.DisposeWithExceptionHandling(this.responseStream);
				this.responseStream = null;
			}
			if (this.httpWebResponse != null)
			{
				this.DisposeWithExceptionHandling(this.httpWebResponse);
				this.httpWebResponse = null;
			}
			if (this.requestStream != null)
			{
				this.DisposeWithExceptionHandling(this.requestStream);
				this.requestStream = null;
			}
			Util.DisposeIfPresent(this.requestBuffer);
			Util.DisposeIfPresent(this.readResponseBuffer);
			Util.DisposeIfPresent(this.responseParser);
			Util.DisposeIfPresent(this.httpRequestTimeoutTimer);
		}

		private void DisposeWithExceptionHandling(IDisposable objToDispose)
		{
			try
			{
				objToDispose.Dispose();
			}
			catch (IOException ex)
			{
				ExTraceGlobals.ClientAsyncOperationTracer.TraceInformation<string>(0, 0L, "IOException occurred: {0}", ex.Message);
			}
			catch (WebException ex2)
			{
				ExTraceGlobals.ClientAsyncOperationTracer.TraceInformation<string>(0, 0L, "WebException occurred: {0}", ex2.Message);
			}
		}

		private static readonly string ClientApplication = string.Format("{0}/{1}", "MapiHttpClient", "15.00.1497.015");

		private readonly ClientSessionContext context;

		private readonly PerfDateTime perfDateTime;

		private HttpWebRequest httpWebRequest;

		private string requestId;

		private WorkBuffer requestBuffer;

		private ArraySegment<byte> requestData;

		private Stream requestStream;

		private HttpWebResponse httpWebResponse;

		private WorkBuffer readResponseBuffer;

		private Stream responseStream;

		private ResponseParser responseParser;

		private int httpRequestTimeout = (int)Constants.HttpRequestTimeout.TotalMilliseconds;

		private Timer httpRequestTimeoutTimer;

		private Exception exception;

		private DateTime? requestStartTime;

		private DateTime? requestWriteTime;

		private DateTime? requestSentTime;

		private DateTime? responseStartTime;

		private DateTime? responseReadTime;

		private DateTime? responseReceivedTime;

		private DateTime? exceptionTime;
	}
}
