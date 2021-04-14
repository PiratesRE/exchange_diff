using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class InboundDataProxyLayer : InboundProxyLayer
	{
		public InboundDataProxyLayer(ulong sessionId, IPEndPoint clientEndPoint, string clientHelloDomain, IEhloOptions ehloOptions, uint xProxyFromSeqNum, TransportMailItem mailItem, bool internalDestination, IEnumerable<INextHopServer> destinations, ulong maxUnconsumedBytes, IProtocolLogSession logSession, SmtpOutConnectionHandler smtpOutConnectionHandler, bool preserveTargetResponse, Permission permissions, AuthenticationSource authenticationSource, IInboundProxyDestinationTracker inboundProxyDestinationTracker) : base(sessionId, clientEndPoint, clientHelloDomain, ehloOptions, xProxyFromSeqNum, mailItem, internalDestination, destinations, maxUnconsumedBytes, logSession, smtpOutConnectionHandler, preserveTargetResponse, permissions, authenticationSource, true, inboundProxyDestinationTracker)
		{
		}

		public override bool IsBdat
		{
			get
			{
				return false;
			}
		}

		public override long OutboundChunkSize
		{
			get
			{
				return 0L;
			}
		}

		public override bool IsLastChunk
		{
			get
			{
				return true;
			}
		}

		protected override void ShutdownProxySession()
		{
			base.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.DataShutdownProxySessionEnter);
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Shutting down proxy session");
			if (!this.shutdownSmtpOutSession)
			{
				lock (this.StateLock)
				{
					base.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.DataShutdownProxySessionEnter);
					this.shutdownSmtpOutSession = true;
					if (this.outstandingReadCompleteCallback != null)
					{
						base.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.DataShutdownProxySessionCallOutstandingReadCallback);
						InboundProxyLayer.OutstandingReadCallbackAsyncState state = new InboundProxyLayer.OutstandingReadCallbackAsyncState(null, true, this.outstandingReadCompleteCallback);
						this.outstandingReadCompleteCallback = null;
						ThreadPool.QueueUserWorkItem(InboundProxyLayer.CallOutstandingReadCallBackDelegate, state);
					}
				}
			}
		}

		public override void WaitForNewCommand(InboundBdatProxyLayer.CommandReceivedCallback commandReceivedCallback)
		{
			throw new InvalidOperationException("WaitForNewCommand should not be called for DATA since there should be only one");
		}

		public override void AckCommandSuccessful()
		{
			throw new InvalidOperationException("AckCommandSuccessful should not be called for DATA");
		}
	}
}
