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
	internal class InboundBdatProxyLayer : InboundProxyLayer
	{
		public InboundBdatProxyLayer(ulong sessionId, IPEndPoint clientEndPoint, string clientHelloDomain, IEhloOptions ehloOptions, uint xProxyFromSeqNum, TransportMailItem mailItem, bool internalDestination, IEnumerable<INextHopServer> destinations, ulong maxUnconsumedBytes, IProtocolLogSession logSession, SmtpOutConnectionHandler smtpOutConnectionHandler, bool preserveTargetResponse, Permission permissions, AuthenticationSource authenticationSource, IInboundProxyDestinationTracker inboundProxyDestinationTracker) : base(sessionId, clientEndPoint, clientHelloDomain, ehloOptions, xProxyFromSeqNum, mailItem, internalDestination, destinations, maxUnconsumedBytes, logSession, smtpOutConnectionHandler, preserveTargetResponse, permissions, authenticationSource, false, inboundProxyDestinationTracker)
		{
		}

		public override bool IsBdat
		{
			get
			{
				return true;
			}
		}

		public override long OutboundChunkSize
		{
			get
			{
				return this.outboundChunkSize;
			}
		}

		public override bool IsLastChunk
		{
			get
			{
				return this.isLastChunk;
			}
		}

		public static void CallOutstandingWaitForCommandCallback(object state)
		{
			InboundBdatProxyLayer.OutstandingWaitForCommandCallbackAsyncState outstandingWaitForCommandCallbackAsyncState = (InboundBdatProxyLayer.OutstandingWaitForCommandCallbackAsyncState)state;
			outstandingWaitForCommandCallbackAsyncState.Callback(outstandingWaitForCommandCallbackAsyncState.CommandReceived);
		}

		protected override void ShutdownProxySession()
		{
			base.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BdatShutdownProxySessionEnter);
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Shutting down proxy session");
			if (!this.shutdownSmtpOutSession)
			{
				lock (this.StateLock)
				{
					base.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BdatShutdownProxySessionEnterLock);
					this.shutdownSmtpOutSession = true;
					if (this.outstandingReadCompleteCallback != null)
					{
						base.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BdatShutdownProxySessionCallOutstandingReadCallback);
						InboundProxyLayer.OutstandingReadCallbackAsyncState state = new InboundProxyLayer.OutstandingReadCallbackAsyncState(null, true, this.outstandingReadCompleteCallback);
						this.outstandingReadCompleteCallback = null;
						ThreadPool.QueueUserWorkItem(InboundProxyLayer.CallOutstandingReadCallBackDelegate, state);
					}
					if (this.outstandingWaitForCommandCallback != null)
					{
						base.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BdatShutdownProxySessionCallOutstandingWaitForCommandCallback);
						InboundBdatProxyLayer.OutstandingWaitForCommandCallbackAsyncState state2 = new InboundBdatProxyLayer.OutstandingWaitForCommandCallbackAsyncState(false, this.outstandingWaitForCommandCallback);
						this.outstandingWaitForCommandCallback = null;
						ThreadPool.QueueUserWorkItem(InboundBdatProxyLayer.CallOutstandingWaitForCommandCallBackDelegate, state2);
					}
				}
			}
		}

		public void CreateNewCommand(long inboundChunkSize, long outboundChunkSize, bool isLast)
		{
			base.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BdatCreateNewCommandEnter);
			lock (this.StateLock)
			{
				base.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BdatCreateNewCommandEnterLock);
				if (this.hasActiveCommand)
				{
					throw new InvalidOperationException("Active command already present");
				}
				if (this.isLastChunk && isLast)
				{
					throw new InvalidOperationException("Last command has already been sent");
				}
				if (this.discardingInboundData)
				{
					throw new InvalidOperationException("New command should not be created when already discarding data");
				}
				if (inboundChunkSize == 0L && !isLast)
				{
					throw new InvalidOperationException("Chunk size should not be zero unless last chunk");
				}
				this.outboundChunkSize = outboundChunkSize;
				this.inboundChunkSize = inboundChunkSize;
				this.isLastChunk = isLast;
				this.targetResponse = SmtpResponse.Empty;
				this.eodSeen = false;
				this.hasActiveCommand = true;
				if (this.outstandingWaitForCommandCallback != null)
				{
					base.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BdatCreateNewCommandCallOutstandingWaitForCommandCallback);
					InboundBdatProxyLayer.OutstandingWaitForCommandCallbackAsyncState state = new InboundBdatProxyLayer.OutstandingWaitForCommandCallbackAsyncState(true, this.outstandingWaitForCommandCallback);
					this.outstandingWaitForCommandCallback = null;
					ThreadPool.QueueUserWorkItem(InboundBdatProxyLayer.CallOutstandingWaitForCommandCallBackDelegate, state);
				}
			}
		}

		public override void WaitForNewCommand(InboundBdatProxyLayer.CommandReceivedCallback commandReceivedCallback)
		{
			base.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BdatWaitForNewCommand);
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "InboundBdatProxyLayer.WaitForNewCommand");
			if (commandReceivedCallback == null)
			{
				throw new ArgumentNullException("commandReceivedCallback");
			}
			bool flag = false;
			lock (this.StateLock)
			{
				base.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BdatWaitForNewCommandEnterLock);
				if (this.outstandingWaitForCommandCallback != null)
				{
					throw new InvalidOperationException("A pending WaitForCommand is already present");
				}
				if (this.hasActiveCommand)
				{
					flag = true;
				}
				else
				{
					this.outstandingWaitForCommandCallback = commandReceivedCallback;
				}
			}
			if (flag)
			{
				commandReceivedCallback(true);
			}
		}

		public override void AckCommandSuccessful()
		{
			base.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BdatAckCommandSuccessful);
			if (this.IsLastChunk)
			{
				throw new InvalidOperationException("AckCommandSuccessful should not be called for the last chunk");
			}
			if (!this.targetResponse.Equals(SmtpResponse.Empty))
			{
				throw new InvalidOperationException("Ack has already been called");
			}
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "InboundBdatProxyLayer.AckCommandSuccessful");
			lock (this.StateLock)
			{
				base.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BdatAckCommandSuccessfulEnterLock);
				this.targetResponse = SmtpResponse.OctetsReceived(this.inboundChunkSize);
				if (this.outstandingWriteCompleteCallback != null)
				{
					base.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BdatAckCommandSuccessfulCallOutstandingWriteCallback);
					InboundProxyLayer.OutstandingWriteCallbackAsyncState state = new InboundProxyLayer.OutstandingWriteCallbackAsyncState(this.targetResponse, this.outstandingWriteCompleteCallback);
					this.outstandingWriteCompleteCallback = null;
					ThreadPool.QueueUserWorkItem(InboundProxyLayer.CallOutstandingWriteCallBackDelegate, state);
				}
				this.hasActiveCommand = false;
			}
		}

		private static readonly WaitCallback CallOutstandingWaitForCommandCallBackDelegate = new WaitCallback(InboundBdatProxyLayer.CallOutstandingWaitForCommandCallback);

		private long inboundChunkSize;

		private long outboundChunkSize;

		private bool isLastChunk;

		private InboundBdatProxyLayer.CommandReceivedCallback outstandingWaitForCommandCallback;

		public delegate void CommandReceivedCallback(bool commandReceived);

		public class OutstandingWaitForCommandCallbackAsyncState
		{
			public OutstandingWaitForCommandCallbackAsyncState(bool commandReceived, InboundBdatProxyLayer.CommandReceivedCallback callback)
			{
				if (callback == null)
				{
					throw new ArgumentNullException("callback");
				}
				this.Callback = callback;
				this.CommandReceived = commandReceived;
			}

			public readonly bool CommandReceived;

			public readonly InboundBdatProxyLayer.CommandReceivedCallback Callback;
		}
	}
}
