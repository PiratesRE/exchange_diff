using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.OCS;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class BaseUMVoipPlatform
	{
		internal abstract event VoipPlatformEventHandler<SendNotifyMessageCompletedEventArgs> OnSendNotifyMessageCompleted;

		internal abstract event VoipPlatformEventHandler<InfoMessage.PlatformMessageReceivedEventArgs> OnMessageReceived;

		internal UserToCallsMap UsersPhoneCalls { get; private set; }

		internal Hashtable CallSessionHashTable { get; private set; }

		internal CallInfoCache DisconnectedOutboundCalls { get; private set; }

		private protected bool ShutdownInProgress { protected get; private set; }

		private protected UMCallSessionHandler<EventArgs> CallHandler { protected get; private set; }

		private protected int SipPort { protected get; private set; }

		internal abstract void Start();

		internal abstract void SendNotifyMessageAsync(PlatformSipUri sipUri, UMSipPeer nextHop, System.Net.Mime.ContentType contentType, byte[] body, string eventHeader, IList<PlatformSignalingHeader> headers, object asyncState);

		internal void CreateAndMakeCallOnDependentSession(BaseUMCallSession parentCallSession, UMCallSessionHandler<OutboundCallDetailsEventArgs> onoutboundCallRequestCompleted, UMSubscriber caller, string callerIdToUse, PhoneNumber numberToCall, out BaseUMCallSession outBoundCallSession)
		{
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				outBoundCallSession = parentCallSession.CreateDependentSession(onoutboundCallRequestCompleted, caller, callerIdToUse, numberToCall);
				disposeGuard.Add<BaseUMCallSession>(outBoundCallSession);
				this.RegisterCall(outBoundCallSession);
				outBoundCallSession.MakeNewDependentSessionCall();
				disposeGuard.Success();
			}
		}

		internal virtual void Initialize(int sipPort, UMCallSessionHandler<EventArgs> callHandler)
		{
			this.isRetired = false;
			ExWatson.Init();
			this.CallSessionHashTable = new Hashtable();
			this.CallHandler = callHandler;
			this.SipPort = sipPort;
			this.activeCallsEnded = new ManualResetEvent(false);
			this.DisconnectedOutboundCalls = new CallInfoCache();
			this.UsersPhoneCalls = new UserToCallsMap();
			SipNotifyMwiTarget.Initialize();
			UserNotificationEvent.Initialize();
			SipPeerManager.Instance.SipPeerListChanged += this.SipPeerListChanged;
		}

		internal virtual void Shutdown()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoipPlatformTracer, null, "In platform shutdown", new object[0]);
			this.ShutdownInProgress = true;
			Hashtable hashtable;
			lock (this.CallSessionHashTable.SyncRoot)
			{
				hashtable = (Hashtable)this.CallSessionHashTable.Clone();
			}
			foreach (object obj in hashtable.Values)
			{
				BaseUMCallSession baseUMCallSession = (BaseUMCallSession)obj;
				baseUMCallSession.DisconnectCall();
			}
			lock (this.CallSessionHashTable.SyncRoot)
			{
				this.SignalWaitingThreadIfFinalCallEnded();
			}
			this.WaitForActiveCallsToEnd(UmServiceGlobals.ComponentStoptime);
			SipNotifyMwiTarget.Uninitialize();
		}

		internal UMCallInfoEx GetCallInfo(Guid sessionGuid)
		{
			UMCallInfoEx umcallInfoEx = null;
			try
			{
				BaseUMCallSession baseUMCallSession = this.FindSession(sessionGuid);
				if (baseUMCallSession != null)
				{
					lock (baseUMCallSession)
					{
						CallContext currentCallContext = baseUMCallSession.CurrentCallContext;
						if (currentCallContext != null)
						{
							umcallInfoEx = new UMCallInfoEx();
							umcallInfoEx.CallState = baseUMCallSession.State;
							umcallInfoEx.EndResult = currentCallContext.WebServiceRequest.EndResult;
						}
					}
				}
			}
			catch (ObjectDisposedException)
			{
				umcallInfoEx = null;
			}
			if (umcallInfoEx == null)
			{
				umcallInfoEx = this.DisconnectedOutboundCalls[sessionGuid];
			}
			return umcallInfoEx;
		}

		internal void CloseSession(Guid sessionGuid)
		{
			BaseUMCallSession session = this.FindSession(sessionGuid);
			this.CloseSession(session);
		}

		internal void Retire(BaseUMVoipPlatform.FinalCallEndedDelegate finalCallEndedDelegate)
		{
			Utils.ThreadPoolQueueUserWorkItem(new WaitCallback(this.InternalRetire), finalCallEndedDelegate);
		}

		internal BaseUMCallSession FindSession(Guid sessionGuid)
		{
			BaseUMCallSession result = null;
			lock (this.CallSessionHashTable.SyncRoot)
			{
				result = (BaseUMCallSession)this.CallSessionHashTable[sessionGuid];
			}
			return result;
		}

		internal BaseUMCallSession MakeCallForUser(UMSubscriber caller, string calledParty, UMSipPeer outboundProxy, CallContext context, UMCallSessionHandler<OutboundCallDetailsEventArgs> onoutboundCallRequestCompleted)
		{
			BaseUMCallSession result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				BaseUMCallSession baseUMCallSession = this.CreateOutboundCallSession(context, onoutboundCallRequestCompleted);
				disposeGuard.Add<BaseUMCallSession>(baseUMCallSession);
				OutCallingHandlerForUser outCallingHandlerForUser = new OutCallingHandlerForUser(caller, baseUMCallSession, outboundProxy, TypeOfOutboundCall.PlayOnPhone);
				outCallingHandlerForUser.MakeCall(caller.OutboundCallingLineId, calledParty, null);
				disposeGuard.Success();
				result = baseUMCallSession;
			}
			return result;
		}

		internal BaseUMCallSession MakeCallForAA(UMAutoAttendant caller, string calledParty, UMSipPeer outboundProxy, CallContext context, UMCallSessionHandler<OutboundCallDetailsEventArgs> onOutboundCallRequestCompleted)
		{
			BaseUMCallSession result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				BaseUMCallSession baseUMCallSession = this.CreateOutboundCallSession(context, onOutboundCallRequestCompleted);
				disposeGuard.Add<BaseUMCallSession>(baseUMCallSession);
				OutCallingHandlerForAA outCallingHandlerForAA = new OutCallingHandlerForAA(caller, baseUMCallSession, outboundProxy);
				outCallingHandlerForAA.MakeCall(calledParty, null);
				disposeGuard.Success();
				result = baseUMCallSession;
			}
			return result;
		}

		internal void UnRegisterCall(BaseUMCallSession callSession)
		{
			lock (this.CallSessionHashTable.SyncRoot)
			{
				if (this.CallSessionHashTable.Contains(callSession.SessionGuid))
				{
					this.CallSessionHashTable.Remove(callSession.SessionGuid);
					this.SignalWaitingThreadIfFinalCallEnded();
				}
			}
		}

		internal void HandleCallDisposed(BaseUMCallSession callSession)
		{
			this.UsersPhoneCalls.RemovePhoneCall(callSession.PlayOnPhoneSMTPAddress, callSession.SessionGuid);
			CallType callType = (callSession.CurrentCallContext == null) ? 0 : callSession.CurrentCallContext.CallType;
			if ((callType == 5 || callType == 10) && this.DisconnectedOutboundCalls[callSession.SessionGuid] == null)
			{
				this.AddToExpiredPOPCallCache(callSession);
			}
		}

		protected abstract BaseUMCallSession InternalCreateOutboundCallSession(CallContext context, UMCallSessionHandler<OutboundCallDetailsEventArgs> handler, UMVoIPSecurityType security);

		protected abstract void SipPeerListChanged(object sender, EventArgs args);

		protected BaseUMCallSession CreateOutboundCallSession(CallContext context, UMCallSessionHandler<OutboundCallDetailsEventArgs> handler)
		{
			BaseUMCallSession baseUMCallSession = this.InternalCreateOutboundCallSession(context, handler, context.DialPlan.VoIPSecurity);
			this.RegisterCall(baseUMCallSession);
			baseUMCallSession.OnOutboundCallRequestCompleted += handler;
			return baseUMCallSession;
		}

		protected void RegisterAndHandleCall(BaseUMCallSession callSession, IList<PlatformSignalingHeader> headers)
		{
			this.RegisterCall(callSession);
			callSession.HandleIncomingCall(headers);
		}

		protected void RegisterCall(BaseUMCallSession callSession)
		{
			callSession.OnCallConnected += this.CallHandler.Invoke;
			lock (this.CallSessionHashTable.SyncRoot)
			{
				this.CallSessionHashTable.Add(callSession.SessionGuid, callSession);
			}
		}

		protected void Fire<TArgs>(VoipPlatformEventHandler<TArgs> handler, TArgs args) where TArgs : EventArgs
		{
			if (handler != null)
			{
				handler(this, args);
			}
		}

		private void InternalRetire(object state)
		{
			BaseUMVoipPlatform.FinalCallEndedDelegate finalCallEndedDelegate = (BaseUMVoipPlatform.FinalCallEndedDelegate)state;
			lock (this.CallSessionHashTable.SyncRoot)
			{
				this.isRetired = true;
				this.SignalWaitingThreadIfFinalCallEnded();
			}
			this.WaitForActiveCallsToEnd();
			finalCallEndedDelegate();
		}

		private void CloseSession(BaseUMCallSession session)
		{
			try
			{
				if (session != null)
				{
					session.CloseSession();
				}
			}
			catch (ObjectDisposedException)
			{
			}
		}

		private void AddToExpiredPOPCallCache(BaseUMCallSession callSession)
		{
			UMCallInfoEx umcallInfoEx = new UMCallInfoEx();
			umcallInfoEx.CallState = UMCallState.Disconnected;
			if (callSession.CurrentCallContext.WebServiceRequest.EndResult != UMOperationResult.Failure)
			{
				umcallInfoEx.EndResult = UMOperationResult.Success;
			}
			else
			{
				umcallInfoEx.EndResult = callSession.CurrentCallContext.WebServiceRequest.EndResult;
			}
			this.DisconnectedOutboundCalls[callSession.SessionGuid] = umcallInfoEx;
		}

		private void WaitForActiveCallsToEnd()
		{
			this.WaitForActiveCallsToEnd(TimeSpan.FromMilliseconds(-1.0));
		}

		private void WaitForActiveCallsToEnd(TimeSpan timeout)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStopTracer, null, "Wait for all active calls to end...", new object[0]);
			if (this.activeCallsEnded.WaitOne(timeout))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStopTracer, null, "All active calls have ended", new object[0]);
				return;
			}
			CallIdTracer.TraceWarning(ExTraceGlobals.ServiceStopTracer, null, "Waiting for active calls has timed out after waiting for {0}", new object[]
			{
				timeout
			});
		}

		private void SignalWaitingThreadIfFinalCallEnded()
		{
			if (this.CallSessionHashTable.Count == 0 && (this.isRetired || this.ShutdownInProgress))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStopTracer, null, "Last call has ended when the worker process has been asked to go down", new object[0]);
				this.activeCallsEnded.Set();
			}
		}

		private bool isRetired;

		private ManualResetEvent activeCallsEnded;

		internal delegate void FinalCallEndedDelegate();
	}
}
