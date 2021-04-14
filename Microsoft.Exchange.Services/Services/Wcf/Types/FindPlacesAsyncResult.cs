using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	internal class FindPlacesAsyncResult : ServiceAsyncResult<Persona[]>
	{
		public BingRequestAsyncState LocationAsyncState { get; private set; }

		public BingRequestAsyncState PhonebookAsyncState { get; private set; }

		public FaultException Fault { get; set; }

		public FindPlacesAsyncResult(AsyncCallback asyncCallback, object asyncState, int maxResults, CallContext callContext)
		{
			base.AsyncCallback = asyncCallback;
			base.AsyncState = asyncState;
			this.maxResults = maxResults;
			this.callContext = callContext;
		}

		public void InitializeLocationRequest()
		{
			ExTraceGlobals.FindPlacesCallTracer.TraceDebug<FindPlacesAsyncResult>((long)this.GetHashCode(), "{0}: Initializing a Location request.", this);
			this.locationRequestCompleted = false;
			this.LocationAsyncState = new BingRequestAsyncState(FindPlacesMetadata.LocationStatusCode, FindPlacesMetadata.LocationResultsCount, FindPlacesMetadata.LocationLatency, FindPlacesMetadata.LocationErrorCode, FindPlacesMetadata.LocationErrorMessage, FindPlacesMetadata.LocationFailed, new Action(this.OnLocationCompleted));
		}

		public void InitializePhonebookRequest()
		{
			ExTraceGlobals.FindPlacesCallTracer.TraceDebug<FindPlacesAsyncResult>((long)this.GetHashCode(), "{0}: Initializing a Phonebook request.", this);
			this.phonebookRequestCompleted = false;
			this.PhonebookAsyncState = new BingRequestAsyncState(FindPlacesMetadata.PhonebookStatusCode, FindPlacesMetadata.PhonebookResultsCount, FindPlacesMetadata.PhonebookLatency, FindPlacesMetadata.PhonebookErrorCode, FindPlacesMetadata.PhonebookErrorMessage, FindPlacesMetadata.PhonebookFailed, new Action(this.OnPhonebookCompleted));
		}

		public void StartTimeoutDetection()
		{
			ExTraceGlobals.FindPlacesCallTracer.TraceDebug<FindPlacesAsyncResult>((long)this.GetHashCode(), "{0}: Starting timeout detection.", this);
			this.timeoutDetector = new GuardedTimer(new TimerCallback(this.TimeoutTriggered), null, FindPlacesAsyncResult.RequestTimeout, TimeSpan.Zero);
		}

		public void EndTimeoutDetection()
		{
			ExTraceGlobals.FindPlacesCallTracer.TraceDebug<FindPlacesAsyncResult>((long)this.GetHashCode(), "{0}: Ending timeout detection.", this);
			if (this.timeoutDetector != null)
			{
				this.timeoutDetector.Dispose(true);
				this.timeoutDetector = null;
			}
		}

		public void CompleteSuccessfully(Persona[] results)
		{
			base.Data = results;
			base.Complete(null);
		}

		public void CompleteWithFault(FaultException fault)
		{
			this.Fault = fault;
			base.Complete(fault);
		}

		private void OnPhonebookCompleted()
		{
			ExTraceGlobals.FindPlacesCallTracer.TraceDebug<FindPlacesAsyncResult>((long)this.GetHashCode(), "{0}: Phonebook request completed.", this);
			lock (this.locker)
			{
				this.phonebookRequestCompleted = true;
				this.VerifyIfRequestIsComplete();
			}
		}

		private void OnLocationCompleted()
		{
			ExTraceGlobals.FindPlacesCallTracer.TraceDebug<FindPlacesAsyncResult>((long)this.GetHashCode(), "{0}: Location request completed.", this);
			lock (this.locker)
			{
				this.locationRequestCompleted = true;
				this.VerifyIfRequestIsComplete();
			}
		}

		private void TimeoutTriggered(object stateNotUsed)
		{
			ExTraceGlobals.FindPlacesCallTracer.TraceDebug<FindPlacesAsyncResult>((long)this.GetHashCode(), "{0}: Timeout reached.", this);
			lock (this.locker)
			{
				this.requestAborted = true;
				if (this.locationRequestCompleted && this.phonebookRequestCompleted)
				{
					ExTraceGlobals.FindPlacesCallTracer.TraceDebug<FindPlacesAsyncResult>((long)this.GetHashCode(), "{0}: Timeout reached after requests have already completed.", this);
				}
				else
				{
					LocationServicesRequestTimedOutException ex = new LocationServicesRequestTimedOutException();
					if (!this.locationRequestCompleted)
					{
						this.callContext.ProtocolLog.Set(FindPlacesMetadata.LocationFailed, "True");
						this.callContext.ProtocolLog.Set(FindPlacesMetadata.LocationErrorCode, "601");
						this.callContext.ProtocolLog.Set(FindPlacesMetadata.LocationErrorMessage, ex.Message);
					}
					if (!this.phonebookRequestCompleted)
					{
						this.callContext.ProtocolLog.Set(FindPlacesMetadata.PhonebookFailed, "True");
						this.callContext.ProtocolLog.Set(FindPlacesMetadata.PhonebookErrorCode, "601");
						this.callContext.ProtocolLog.Set(FindPlacesMetadata.PhonebookErrorMessage, ex.Message);
					}
					ExTraceGlobals.FindPlacesCallTracer.TraceDebug<FindPlacesAsyncResult>((long)this.GetHashCode(), "{0}: Aborting http requests.", this);
					if (this.LocationAsyncState != null)
					{
						this.LocationAsyncState.Abort();
					}
					if (this.PhonebookAsyncState != null)
					{
						this.PhonebookAsyncState.Abort();
					}
					this.CompleteWithFault(FaultExceptionUtilities.CreateFault(ex, FaultParty.Receiver, ExchangeVersion.Current));
				}
			}
		}

		private void VerifyIfRequestIsComplete()
		{
			ExTraceGlobals.FindPlacesCallTracer.TraceDebug((long)this.GetHashCode(), "{0}: Verifying if the request has completed. Aborted: {1}, Location: {2}, Phonebook: {3}", new object[]
			{
				this,
				this.requestAborted,
				this.locationRequestCompleted,
				this.phonebookRequestCompleted
			});
			if (!this.requestAborted && this.locationRequestCompleted && this.phonebookRequestCompleted)
			{
				this.timeoutDetector.Pause();
				FaultException requestFault = this.GetRequestFault();
				if (requestFault != null)
				{
					ExTraceGlobals.FindPlacesCallTracer.TraceError<FindPlacesAsyncResult, FaultException>((long)this.GetHashCode(), "{0}: Request failed with {1}", this, requestFault);
					this.CompleteWithFault(requestFault);
					return;
				}
				List<Persona> list = new List<Persona>();
				if (this.PhonebookAsyncState != null)
				{
					ExTraceGlobals.FindPlacesCallTracer.TraceDebug<FindPlacesAsyncResult, int>((long)this.GetHashCode(), "{0}: Total of {1} results returned by the Phonebook services.", this, this.PhonebookAsyncState.ResultsList.Count);
					list.AddRange(this.PhonebookAsyncState.ResultsList);
				}
				if (this.LocationAsyncState != null)
				{
					ExTraceGlobals.FindPlacesCallTracer.TraceDebug<FindPlacesAsyncResult, int>((long)this.GetHashCode(), "{0}: Total of {1} results returned by the Location services.", this, this.LocationAsyncState.ResultsList.Count);
					list.AddRange(this.LocationAsyncState.ResultsList);
				}
				this.CompleteSuccessfully(list.GetRange(0, Math.Min(this.maxResults, list.Count)).ToArray());
			}
		}

		private FaultException GetRequestFault()
		{
			int num = 0;
			List<Exception> list = new List<Exception>();
			if (this.PhonebookAsyncState != null)
			{
				num++;
				if (this.PhonebookAsyncState.Exception != null)
				{
					list.Add(this.PhonebookAsyncState.Exception);
				}
			}
			if (this.LocationAsyncState != null)
			{
				num++;
				if (this.LocationAsyncState.Exception != null)
				{
					list.Add(this.LocationAsyncState.Exception);
				}
			}
			if (list.Count == num)
			{
				Exception innerException = new Exception(string.Join("; ", from v in list
				select v.ToString()));
				return FaultExceptionUtilities.CreateFault(new LocationServicesRequestFailedException(innerException), FaultParty.Receiver, ExchangeVersion.Current);
			}
			return null;
		}

		private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(2.0);

		private readonly int maxResults;

		private GuardedTimer timeoutDetector;

		private object locker = new object();

		private bool locationRequestCompleted = true;

		private bool phonebookRequestCompleted = true;

		private bool requestAborted;

		private CallContext callContext;
	}
}
