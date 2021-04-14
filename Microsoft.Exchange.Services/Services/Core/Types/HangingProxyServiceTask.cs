using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class HangingProxyServiceTask<T> : ProxyServiceTask<T>
	{
		public HangingProxyServiceTask(BaseRequest request, CallContext callContext, ServiceAsyncResult<T> serviceAsyncResult, WebServicesInfo[] services, int readTimeout, Func<BaseSoapResponse> getCloseConnectionResponse) : base(request, callContext, serviceAsyncResult, services)
		{
			this.readTimeout = readTimeout;
			this.getCloseConnectionResponse = getCloseConnectionResponse;
		}

		protected override void ProcessResponseMessageAndCompleteIfNecessary(TimeSpan elapsed, HttpWebResponse response, ProxyResult result)
		{
			this.proxyResponseStream = response.GetResponseStream();
			this.elapsedTime = elapsed;
			this.proxyResponseStream.ReadTimeout = this.readTimeout;
			this.proxyDoneEvent.Set();
			base.SetProxyHopHeaders(this.proxiedToService);
			this.WriteProxyHopHeadersToResponse();
			this.wireWriter = EwsResponseWireWriter.Create(CallContext.Current, true);
			this.BeginProxyResponseStreamRead();
		}

		private void BeginProxyResponseStreamRead()
		{
			try
			{
				this.proxyResponseStream.BeginRead(this.responseBuffer, 0, this.responseBuffer.Length, new AsyncCallback(this.EndProxyResponseStreamRead), this.responseBuffer);
			}
			catch (IOException exception)
			{
				this.HandleProxyStreamReadException(exception);
			}
			catch (WebException exception2)
			{
				this.HandleProxyStreamReadException(exception2);
			}
		}

		private void EndProxyResponseStreamRead(IAsyncResult asyncResult)
		{
			try
			{
				byte[] responseBytes = asyncResult.AsyncState as byte[];
				int num = this.proxyResponseStream.EndRead(asyncResult);
				if (num != 0)
				{
					this.wireWriter.WriteResponseToWire(responseBytes, 0, num);
					this.BeginProxyResponseStreamRead();
				}
				else
				{
					this.CompleteRequest(null);
				}
			}
			catch (IOException exception)
			{
				this.HandleProxyStreamReadException(exception);
			}
			catch (WebException exception2)
			{
				this.HandleProxyStreamReadException(exception2);
			}
			catch (HttpException exception3)
			{
				this.HandleClientSendException(exception3);
			}
		}

		private void HandleProxyStreamReadException(Exception exception)
		{
			ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<Exception>((long)this.GetHashCode(), "[HangingProxyServiceTask::HandleProxyStreamReadException] Encountered exception reading from the proxied response stream: {0}", exception);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(base.CallContext.ProtocolLog, exception, "HangingProxy_ProxyReadError");
			try
			{
				BaseSoapResponse response = this.getCloseConnectionResponse();
				this.wireWriter.WriteResponseToWire(response, false);
				this.CompleteRequest(null);
			}
			catch (HttpException exception2)
			{
				this.HandleClientSendException(exception2);
			}
		}

		private void HandleClientSendException(Exception exception)
		{
			ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<Exception>((long)this.GetHashCode(), "[HangingProxyServiceTask::HandleClientSentException] Encountered exception sending response to client: {0}", exception);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(base.CallContext.ProtocolLog, exception, "HangingProxy_ClientSendError");
			this.CompleteRequest(null);
		}

		private void CompleteRequest(Exception exception)
		{
			this.wireWriter.WaitForSendCompletion();
			this.FinishRequest(string.Format("[C,P({0})]", this.proxiedToService.Url), this.queueAndDelayTime, this.elapsedTime, exception);
		}

		private void WriteProxyHopHeadersToResponse()
		{
			Dictionary<string, string> proxyHopHeaders = EWSSettings.ProxyHopHeaders;
			if (Global.WriteProxyHopHeaders && proxyHopHeaders != null && HttpContext.Current != null && HttpContext.Current.Response != null)
			{
				HttpResponse response = HttpContext.Current.Response;
				foreach (KeyValuePair<string, string> keyValuePair in proxyHopHeaders)
				{
					response.AppendHeader(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		protected override void InternalDispose()
		{
			base.InternalDispose();
			if (this.wireWriter != null)
			{
				this.wireWriter.Dispose();
				this.wireWriter = null;
			}
		}

		private const int BufferSize = 4096;

		private EwsResponseWireWriter wireWriter;

		private Stream proxyResponseStream;

		private TimeSpan elapsedTime;

		private int readTimeout;

		private byte[] responseBuffer = new byte[4096];

		private Func<BaseSoapResponse> getCloseConnectionResponse;
	}
}
