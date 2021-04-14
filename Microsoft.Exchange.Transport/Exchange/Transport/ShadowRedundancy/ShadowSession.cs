using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal abstract class ShadowSession : IShadowSession
	{
		public ShadowSession(ISmtpInSession inSession, ShadowRedundancyManager shadowRedundancyManager, ShadowHubPickerBase hubPicker, ISmtpOutConnectionHandler connectionHandler)
		{
			if (inSession == null)
			{
				throw new ArgumentNullException("inSession");
			}
			if (shadowRedundancyManager == null)
			{
				throw new ArgumentNullException("shadowRedundancyManager");
			}
			if (hubPicker == null)
			{
				throw new ArgumentNullException("hubPicker");
			}
			if (connectionHandler == null)
			{
				throw new ArgumentNullException("connectionHandler");
			}
			this.inSession = inSession;
			this.shadowRedundancyManager = shadowRedundancyManager;
			this.hubPicker = hubPicker;
			this.connectionHandler = connectionHandler;
			this.writeCompleteCallback = new InboundProxyLayer.CompletionCallback(this.WriteProxyDataComplete);
			this.negotiationTimer = ShadowRedundancyManager.PerfCounters.ShadowNegotiationLatencyCounter();
			this.selectionTimer = ShadowRedundancyManager.PerfCounters.ShadowSelectionLatencyCounter();
		}

		public string ShadowServerContext
		{
			get
			{
				return this.shadowServerContext;
			}
			set
			{
				this.shadowServerContext = value;
			}
		}

		internal bool Complete
		{
			get
			{
				return this.shadowComplete;
			}
		}

		internal AckStatus CompletionStatus
		{
			get
			{
				return this.shadowCompletionStatus;
			}
		}

		internal List<ShadowServerResponseInfo> ShadowServerResponses
		{
			get
			{
				return this.shadowServerResponses;
			}
		}

		protected bool HasPendingProxyCallback
		{
			get
			{
				return this.hasPendingProxyCallback;
			}
		}

		protected bool IsClosed
		{
			get
			{
				return this.isClosed;
			}
		}

		public IAsyncResult BeginOpen(TransportMailItem transportMailItem, AsyncCallback asyncCallback, object state)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowSession.BeginOpen");
			this.DropBreadcrumb(ShadowBreadcrumbs.Open);
			if (asyncCallback == null)
			{
				throw new ArgumentNullException("asyncCallback");
			}
			this.mailItem = transportMailItem;
			this.LoadNewProxyLayer();
			ShadowPeerNextHopConnection connection = new ShadowPeerNextHopConnection(this, this.proxyLayer, Components.RoutingComponent.MailRouter, this.mailItem);
			IEnumerable<INextHopServer> shadowHubs = null;
			this.negotiationTimer.Start();
			this.selectionTimer.Start();
			if (this.hubPicker.TrySelectShadowServers(out shadowHubs))
			{
				this.connectionHandler.HandleShadowConnection(connection, shadowHubs);
			}
			else
			{
				this.AckMessage(AckStatus.Fail, SmtpResponse.ShadowRedundancyFailed);
			}
			return new AsyncResult(asyncCallback, state, true);
		}

		public bool EndOpen(IAsyncResult asyncResult)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowSession.EndOpen");
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			return true;
		}

		public IAsyncResult BeginWrite(byte[] buffer, int offset, int count, bool seenEod, AsyncCallback asyncCallback, object state)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<int, int>((long)this.GetHashCode(), "ShadowSession.BeginWrite Offset={0} Count={1}", offset, count);
			if (asyncCallback == null)
			{
				throw new ArgumentNullException("asyncCallback");
			}
			if (this.proxyLayer == null)
			{
				throw new InvalidOperationException("BeginWrite called without calling BeginOpen");
			}
			lock (this.syncObject)
			{
				if (!this.shadowComplete && !this.IsClosed)
				{
					this.WriteInternal(buffer, offset, count, seenEod);
				}
			}
			return new AsyncResult(asyncCallback, state, true);
		}

		public bool EndWrite(IAsyncResult asyncResult)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowSession.EndWrite");
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			return true;
		}

		public IAsyncResult BeginComplete(AsyncCallback asyncCallback, object state)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowSession.BeginComplete");
			this.DropBreadcrumb(ShadowBreadcrumbs.Complete);
			if (asyncCallback == null)
			{
				throw new ArgumentNullException("asyncCallback");
			}
			if (this.proxyLayer == null)
			{
				throw new InvalidOperationException("BeginComplete called without calling BeginOpen");
			}
			lock (this.syncObject)
			{
				this.completeAsyncResult = new AsyncResult(asyncCallback, state);
				if (this.shadowComplete)
				{
					this.LogMessage(this.shadowCompletionStatus);
					this.completeAsyncResult.IsCompleted = true;
				}
			}
			return this.completeAsyncResult;
		}

		public bool EndComplete(IAsyncResult asyncResult)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowSession.EndComplete");
			if (!this.shadowComplete)
			{
				throw new InvalidOperationException("EndComplete called before callback fired");
			}
			return this.shadowCompletionStatus == AckStatus.Success;
		}

		public void Close(AckStatus ackStatus, SmtpResponse smtpResponse)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<AckStatus, SmtpResponse>((long)this.GetHashCode(), "ShadowSession.Close({0},{1})", ackStatus, smtpResponse);
			this.DropBreadcrumb(ShadowBreadcrumbs.Close);
			lock (this.syncObject)
			{
				if (!this.isClosed)
				{
					this.isClosed = true;
					this.AckConnection(ackStatus, smtpResponse);
				}
			}
		}

		public void NotifyProxyFailover(string shadowServer, SmtpResponse smtpResponse)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowSession.NotifyProxyFailover");
			this.DropBreadcrumb(ShadowBreadcrumbs.ProxyFailover);
			this.NotifyShadowServerResponse(shadowServer, smtpResponse);
		}

		public bool MailItemRequiresShadowCopy(TransportMailItem mailItem)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowSession.MailItemRequiresShadowCopy");
			return this.shadowRedundancyManager.Configuration.RejectMessageOnShadowFailure;
		}

		public abstract void PrepareForNewCommand(BaseDataSmtpCommand newCommand);

		protected abstract void LoadNewProxyLayer();

		protected abstract void WriteInternal(byte[] buffer, int offset, int count, bool seenEod);

		protected virtual void WriteProxyDataComplete(SmtpResponse response)
		{
			this.DropBreadcrumb(ShadowBreadcrumbs.WriteProxyDataComplete);
			this.hasPendingProxyCallback = false;
			if (Interlocked.Increment(ref this.shadowServerSelected) == 1 && this.selectionTimer != null)
			{
				this.selectionTimer.Stop();
			}
		}

		protected void WriteToProxy(byte[] buffer, int offset, int count, bool seenEod, bool copyBuffer)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowSession writing raw data to proxy layer: length={0} offset={1} count={2} eod={3}", new object[]
			{
				buffer.Length,
				offset,
				count,
				seenEod
			});
			lock (this.syncObject)
			{
				if (this.IsClosed)
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowSession is closed, skipping write to proxy layer");
					this.DropBreadcrumb(ShadowBreadcrumbs.WriteAfterCloseSkipped);
				}
				else
				{
					this.hasPendingProxyCallback = true;
					this.DropBreadcrumb(ShadowBreadcrumbs.WriteToProxy);
					this.proxyLayer.BeginWriteData(buffer, offset, count, seenEod, this.writeCompleteCallback, copyBuffer);
				}
			}
		}

		protected void WriteToProxy(WriteRecord writeRecord)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<int, bool>((long)this.GetHashCode(), "ShadowSession writing queued record to proxy layer: count={0} eod={1}", writeRecord.WriteBuffer.Length, writeRecord.Eod);
			this.WriteToProxy(writeRecord.WriteBuffer, 0, writeRecord.WriteBuffer.Length, writeRecord.Eod, false);
		}

		public void NotifyShadowServerResponse(string shadowServer, SmtpResponse response)
		{
			if (response.SmtpResponseType != SmtpResponseType.Success)
			{
				ShadowRedundancyManager.PerfCounters.ShadowFailure(shadowServer);
			}
			lock (this.syncObject)
			{
				this.shadowServerResponses.Add(new ShadowServerResponseInfo(shadowServer, response));
			}
		}

		public void NotifyLocalMessageDiscarded(TransportMailItem mailItem)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowSession.NotifyLocalMessageDiscarded");
			this.DropBreadcrumb(ShadowBreadcrumbs.LocalMessageDiscarded);
			SystemProbeHelper.ShadowRedundancyTracer.TraceFail(mailItem, 0L, "Message failed to be stored locally on primary server");
			mailItem.DropBreadcrumb(Breadcrumb.MailItemDelivered);
			this.shadowRedundancyManager.NotifyMailItemDelivered(mailItem);
		}

		public void NotifyMessageRejected(TransportMailItem mailItem)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowSession.NotifyMessageRejected");
			this.DropBreadcrumb(ShadowBreadcrumbs.LocalMessageRejected);
			SystemProbeHelper.ShadowRedundancyTracer.TraceFail(mailItem, 0L, "Message failed to be shadowed and shadowing was required to accept the message");
			this.shadowRedundancyManager.EventLogger.LogMessageDeferredDueToShadowFailure();
		}

		public void NotifyMessageComplete(TransportMailItem mailItem)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowSession.NotifyMessageComplete updating TMI state");
			this.DropBreadcrumb(ShadowBreadcrumbs.MessageShadowingComplete);
			SystemProbeHelper.ShadowRedundancyTracer.TracePass<AckStatus>(this.mailItem, 0L, "Message accepted by the primary hub server. Shadow status = {0}", this.shadowCompletionStatus);
			this.mailItem.ShadowServerDiscardId = this.mailItem.ShadowMessageId.ToString();
			this.mailItem.ShadowServerContext = this.ShadowServerContext;
			this.mailItem.CommitLazy();
		}

		internal void AckMessage(AckStatus status, SmtpResponse smtpResponse)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<AckStatus, SmtpResponse>((long)this.GetHashCode(), "ShadowSession.AckMessage Status={0} Response={1}", status, smtpResponse);
			lock (this.syncObject)
			{
				this.shadowComplete = true;
				this.shadowCompletionStatus = status;
				this.DropBreadcrumb(ShadowBreadcrumbs.SessionAckMessage);
				if (this.completeAsyncResult != null && !this.completeAsyncResult.IsCompleted)
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowSession.AckMessage completing callback");
					this.LogMessage(status);
					if (this.negotiationTimer != null)
					{
						long num = this.negotiationTimer.Stop();
						if (status == AckStatus.Success)
						{
							ShadowRedundancyManager.PerfCounters.ShadowSuccessfulNegotiationLatencyCounter().AddSample(num);
							SystemProbeHelper.ShadowRedundancyTracer.TracePass<long>(this.mailItem, 0L, "Shadowed successfully in {0} ticks.", num);
						}
						else
						{
							SystemProbeHelper.ShadowRedundancyTracer.TraceFail<long>(this.mailItem, 0L, "Failure to shadow took {0} ticks.", num);
						}
					}
					this.completeAsyncResult.IsCompleted = true;
				}
			}
		}

		private void AckConnection(AckStatus status, SmtpResponse smtpResponse)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<AckStatus, SmtpResponse>((long)this.GetHashCode(), "ShadowSession.AckConnection Status={0} Response={1}", status, smtpResponse);
			switch (status)
			{
			case AckStatus.Pending:
				break;
			case AckStatus.Success:
				return;
			case AckStatus.Retry:
			case AckStatus.Fail:
				this.DropBreadcrumb(ShadowBreadcrumbs.SessionAckConnectionFailure);
				this.AckMessage(AckStatus.Fail, smtpResponse);
				this.proxyLayer.NotifySmtpInStopProxy();
				return;
			default:
				switch (status)
				{
				case AckStatus.Resubmit:
				case AckStatus.Skip:
					break;
				case AckStatus.Quarantine:
					return;
				default:
					return;
				}
				break;
			}
			throw new InvalidOperationException("Invalid status");
		}

		internal void DropBreadcrumb(ShadowBreadcrumbs breadcrumb)
		{
			this.breadcrumbs.Drop(breadcrumb);
		}

		private void LogMessage(AckStatus status)
		{
			if (status == AckStatus.Success)
			{
				MessageTrackingLog.TrackHighAvailabilityRedirect(MessageTrackingSource.SMTP, this.mailItem, string.Join<ShadowServerResponseInfo>(";", this.shadowServerResponses));
				return;
			}
			MessageTrackingLog.TrackHighAvailabilityRedirectFail(MessageTrackingSource.SMTP, this.mailItem, (this.shadowServerResponses.Count > 0) ? string.Join<ShadowServerResponseInfo>(";", this.shadowServerResponses) : "No suitable shadow servers");
		}

		protected InboundProxyLayer.CompletionCallback writeCompleteCallback;

		protected InboundProxyLayer proxyLayer;

		protected ISmtpInSession inSession;

		protected object syncObject = new object();

		private ISmtpOutConnectionHandler connectionHandler;

		private ShadowHubPickerBase hubPicker;

		private TransportMailItem mailItem;

		private AsyncResult completeAsyncResult;

		private bool shadowComplete;

		private AckStatus shadowCompletionStatus;

		private ITimerCounter negotiationTimer;

		private ITimerCounter selectionTimer;

		private int shadowServerSelected;

		private ShadowRedundancyManager shadowRedundancyManager;

		private string shadowServerContext;

		private bool hasPendingProxyCallback;

		private bool isClosed;

		private List<ShadowServerResponseInfo> shadowServerResponses = new List<ShadowServerResponseInfo>();

		private Breadcrumbs<ShadowBreadcrumbs> breadcrumbs = new Breadcrumbs<ShadowBreadcrumbs>(64);
	}
}
