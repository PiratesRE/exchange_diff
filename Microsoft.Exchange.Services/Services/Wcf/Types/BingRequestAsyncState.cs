using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	internal class BingRequestAsyncState
	{
		public HttpWebRequest Request { get; set; }

		public Exception Exception { get; private set; }

		public FindPlacesMetadata StatusCodeTag { get; private set; }

		public FindPlacesMetadata ResultsCountTag { get; private set; }

		public FindPlacesMetadata ErrorCodeTag { get; private set; }

		public FindPlacesMetadata ErrorMessageTag { get; private set; }

		public FindPlacesMetadata RequestFailedTag { get; private set; }

		public List<Persona> ResultsList { get; private set; }

		public BingRequestAsyncState(FindPlacesMetadata statusCodeTag, FindPlacesMetadata resultsCountTag, FindPlacesMetadata latencyTag, FindPlacesMetadata errorCodeTag, FindPlacesMetadata errorMessageTag, FindPlacesMetadata requestFailedTag, Action onRequestCompletedCallback)
		{
			this.StatusCodeTag = statusCodeTag;
			this.ResultsCountTag = resultsCountTag;
			this.latencyTag = latencyTag;
			this.ErrorCodeTag = errorCodeTag;
			this.RequestFailedTag = requestFailedTag;
			this.ErrorMessageTag = errorMessageTag;
			this.onRequestCompletedCallback = onRequestCompletedCallback;
			this.ResultsList = new List<Persona>();
		}

		public void Abort()
		{
			ExTraceGlobals.FindPlacesCallTracer.TraceDebug<BingRequestAsyncState>((long)this.GetHashCode(), "{0}: BingRequestAsyncState::Abort() called.", this);
			this.aborted = true;
			HttpWebRequest request = this.Request;
			if (request != null)
			{
				request.Abort();
			}
		}

		public void Begin()
		{
			ExTraceGlobals.FindPlacesCallTracer.TraceDebug<BingRequestAsyncState>((long)this.GetHashCode(), "{0}: BingRequestAsyncState::Begin() called.", this);
			this.stopWatch = Stopwatch.StartNew();
		}

		public void InProgress(CallContext callContext, Exception exception)
		{
			ExTraceGlobals.FindPlacesCallTracer.TraceDebug<BingRequestAsyncState>((long)this.GetHashCode(), "{0}: BingRequestAsyncState::InProgress called.", this);
			if (exception != null)
			{
				ExTraceGlobals.FindPlacesCallTracer.TraceDebug<BingRequestAsyncState, Exception>((long)this.GetHashCode(), "{0}: BingRequestAsyncState::InProgress() already failed {1}", this, exception);
				this.End(callContext, exception);
			}
		}

		public void End(CallContext callContext, Exception exception)
		{
			if (this.aborted)
			{
				ExTraceGlobals.FindPlacesCallTracer.TraceDebug<BingRequestAsyncState>((long)this.GetHashCode(), "{0}: BingRequestAsyncState::End() called after request was aborted.", this);
				return;
			}
			ExTraceGlobals.FindPlacesCallTracer.TraceDebug<BingRequestAsyncState>((long)this.GetHashCode(), "{0}: BingRequestAsyncState::End() called.", this);
			this.Request = null;
			this.stopWatch.Stop();
			callContext.ProtocolLog.Set(this.latencyTag, this.stopWatch.ElapsedMilliseconds);
			this.Exception = exception;
			if (exception != null)
			{
				callContext.ProtocolLog.Set(this.RequestFailedTag, "True");
				callContext.ProtocolLog.Set(this.ErrorMessageTag, exception.Message);
				WebException ex = exception as WebException;
				if (ex != null)
				{
					if (ex.Status == WebExceptionStatus.ProtocolError)
					{
						HttpStatusCode statusCode = ((HttpWebResponse)ex.Response).StatusCode;
						callContext.ProtocolLog.Set(this.StatusCodeTag, (int)statusCode);
						callContext.ProtocolLog.Set(this.ErrorCodeTag, (int)statusCode);
					}
				}
				else
				{
					SerializationException ex2 = exception as SerializationException;
					if (ex2 != null)
					{
						callContext.ProtocolLog.Set(this.ErrorCodeTag, "600");
					}
				}
			}
			this.onRequestCompletedCallback();
		}

		private readonly FindPlacesMetadata latencyTag;

		private readonly Action onRequestCompletedCallback;

		private bool aborted;

		private Stopwatch stopWatch;
	}
}
