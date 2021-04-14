using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal abstract class InboundProxyLayer : IInboundProxyLayer
	{
		public static SmtpResponse GetEncodedProxyFailureResponse(SessionSetupFailureReason failureReason)
		{
			switch (failureReason)
			{
			case SessionSetupFailureReason.None:
				return SmtpResponse.GenericProxyFailure;
			case SessionSetupFailureReason.UserLookupFailure:
				return SmtpResponse.EncodedProxyFailureResponseUserLookupFailure;
			case SessionSetupFailureReason.DnsLookupFailure:
				return SmtpResponse.EncodedProxyFailureResponseDnsError;
			case SessionSetupFailureReason.ConnectionFailure:
				return SmtpResponse.EncodedProxyFailureResponseConnectionFailure;
			case SessionSetupFailureReason.ProtocolError:
				return SmtpResponse.EncodedProxyFailureResponseProtocolError;
			case SessionSetupFailureReason.SocketError:
				return SmtpResponse.EncodedProxyFailureResponseSocketError;
			case SessionSetupFailureReason.Shutdown:
				return SmtpResponse.EncodedProxyFailureResponseShutdown;
			case SessionSetupFailureReason.BackEndLocatorFailure:
				return SmtpResponse.EncodedProxyFailureResponseBackEndLocatorFailure;
			default:
				throw new InvalidOperationException("Invalid session failure reason");
			}
		}

		protected InboundProxyLayer(ulong sessionId, IPEndPoint clientEndPoint, string clientHelloDomain, IEhloOptions ehloOptions, uint xProxyFromSeqNum, TransportMailItem mailItem, bool internalDestination, IEnumerable<INextHopServer> destinations, ulong maxUnconsumedBytes, IProtocolLogSession logSession, SmtpOutConnectionHandler smtpOutConnectionHandler, bool preserveTargetResponse, Permission permissions, AuthenticationSource authenticationSource, bool hasActiveCommand, IInboundProxyDestinationTracker inboundProxyDestinationTracker)
		{
			ArgumentValidator.ThrowIfNull("clientEndPoint", clientEndPoint);
			ArgumentValidator.ThrowIfNull("ehloOptions", ehloOptions);
			ArgumentValidator.ThrowIfNull("mailItem", mailItem);
			ArgumentValidator.ThrowIfNull("destinations", destinations);
			ArgumentValidator.ThrowIfNull("logSession", logSession);
			ArgumentValidator.ThrowIfNull("inboundProxyDestinationTracker", inboundProxyDestinationTracker);
			this.sessionId = sessionId;
			this.clientEndPoint = clientEndPoint;
			this.clientHelloDomain = clientHelloDomain;
			this.smtpInEhloOptions = ehloOptions;
			this.xProxyFromSeqNum = xProxyFromSeqNum;
			this.mailItem = new InboundProxyRoutedMailItem(mailItem);
			this.proxyDestinations = destinations;
			this.internalDestination = internalDestination;
			this.proxyNextHopSolutionKey = new NextHopSolutionKey(NextHopType.Empty, internalDestination ? "InternalProxy" : "ExternalProxy", Guid.Empty);
			this.unconsumedBytes = 0UL;
			this.maxUnconsumedBytes = maxUnconsumedBytes;
			this.logSession = logSession;
			this.SmtpOutConnectionHandler = smtpOutConnectionHandler;
			this.preserveTargetResponse = preserveTargetResponse;
			this.permissions = permissions;
			this.authenticationSource = authenticationSource;
			this.hasActiveCommand = hasActiveCommand;
			this.inboundProxyDestinationTracker = inboundProxyDestinationTracker;
			if (Util.TryGetNextHopFqdnProperty(mailItem.ExtendedPropertyDictionary, out this.nextHopFqdn))
			{
				this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "NextHopFqdn: {0}", new object[]
				{
					this.nextHopFqdn
				});
			}
		}

		public IInboundProxyDestinationTracker InboundProxyDestinationTracker
		{
			get
			{
				return this.inboundProxyDestinationTracker;
			}
		}

		public IPEndPoint ClientEndPoint
		{
			get
			{
				return this.clientEndPoint;
			}
		}

		public string ClientHelloDomain
		{
			get
			{
				return this.clientHelloDomain;
			}
		}

		public ulong SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		public string NextHopFqdn
		{
			get
			{
				return this.nextHopFqdn;
			}
		}

		public IEhloOptions SmtpInEhloOptions
		{
			get
			{
				return this.smtpInEhloOptions;
			}
		}

		public long BytesRead
		{
			get
			{
				return this.bytesRead;
			}
		}

		public long BytesWritten
		{
			get
			{
				return this.bytesWritten;
			}
		}

		public uint XProxyFromSeqNum
		{
			get
			{
				return this.xProxyFromSeqNum;
			}
		}

		public Permission Permissions
		{
			get
			{
				return this.permissions;
			}
		}

		public AuthenticationSource AuthenticationSource
		{
			get
			{
				return this.authenticationSource;
			}
		}

		public abstract bool IsBdat { get; }

		public abstract long OutboundChunkSize { get; }

		public abstract bool IsLastChunk { get; }

		public static void CallOutstandingReadCallback(object state)
		{
			InboundProxyLayer.OutstandingReadCallbackAsyncState outstandingReadCallbackAsyncState = (InboundProxyLayer.OutstandingReadCallbackAsyncState)state;
			outstandingReadCallbackAsyncState.Callback(outstandingReadCallbackAsyncState.Buffer, outstandingReadCallbackAsyncState.LastBuffer);
		}

		public static void CallOutstandingWriteCallback(object state)
		{
			InboundProxyLayer.OutstandingWriteCallbackAsyncState outstandingWriteCallbackAsyncState = (InboundProxyLayer.OutstandingWriteCallbackAsyncState)state;
			outstandingWriteCallbackAsyncState.Callback(outstandingWriteCallbackAsyncState.Response);
		}

		public void NotifySmtpInStopProxy()
		{
			if (!this.discardingInboundData)
			{
				lock (this.StateLock)
				{
					this.discardingInboundData = true;
				}
			}
			this.ShutdownProxySession();
		}

		public void DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs breadcrumb)
		{
			this.breadcrumbs.Drop(breadcrumb);
		}

		public void SetupProxySession()
		{
			this.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.SetupProxySession);
			WaitCallback callBack = new WaitCallback(this.StartConnection);
			ThreadPool.QueueUserWorkItem(callBack);
		}

		public void BeginWriteData(byte[] buffer, int offset, int count, bool endOfData, InboundProxyLayer.CompletionCallback writeCompleteCallback)
		{
			this.BeginWriteData(buffer, offset, count, endOfData, writeCompleteCallback, true);
		}

		public void BeginWriteData(byte[] buffer, int offset, int count, bool endOfData, InboundProxyLayer.CompletionCallback writeCompleteCallback, bool copyBuffer)
		{
			this.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BeginWriteDataEnter);
			ExTraceGlobals.SmtpSendTracer.TraceDebug<int, int, bool>((long)this.GetHashCode(), "BeginWriteData offset = {0}, count = {1}, endOfData = {2}", offset, count, endOfData);
			if (count < 0)
			{
				throw new ArgumentException("Count cannot be negative");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (writeCompleteCallback == null)
			{
				throw new ArgumentNullException("writeCompleteCallback");
			}
			if (this.discardingInboundData)
			{
				throw new InvalidOperationException("Should not write to the proxy layer when discarding");
			}
			if (this.eodSeen)
			{
				throw new InvalidOperationException("Should not write to the proxy layer when EOD has already been seen");
			}
			BufferCacheEntry bufferCacheEntry;
			if (copyBuffer)
			{
				bufferCacheEntry = this.SmtpOutConnectionHandler.BufferCache.GetBuffer(count);
				Buffer.BlockCopy(buffer, offset, bufferCacheEntry.Buffer, 0, count);
			}
			else
			{
				bufferCacheEntry = new BufferCacheEntry(buffer, false);
				count = buffer.Length;
			}
			bool flag = true;
			SmtpResponse response;
			lock (this.StateLock)
			{
				this.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BeginWriteDataEnterLock);
				if (this.outstandingWriteCompleteCallback != null)
				{
					throw new InvalidOperationException("There is already a pending write");
				}
				response = this.targetResponse;
				if (this.targetResponse.Equals(SmtpResponse.Empty))
				{
					this.eodSeen = endOfData;
					if (count != 0)
					{
						this.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BeginWriteDataEnqueueBuffer);
						this.bufferQueue.Enqueue(bufferCacheEntry);
						this.unconsumedBytes += (ulong)((long)count);
						this.bytesWritten += (long)count;
						if (this.outstandingReadCompleteCallback != null)
						{
							this.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BeginWriteDataCallOutstandingReadCallback);
							BufferCacheEntry bufferCacheEntry2 = this.bufferQueue.Dequeue();
							this.unconsumedBytes -= (ulong)((long)bufferCacheEntry2.Buffer.Length);
							this.bytesRead += (long)bufferCacheEntry2.Buffer.Length;
							bool lastBuffer = this.eodSeen && this.bufferQueue.Count == 0;
							InboundProxyLayer.OutstandingReadCallbackAsyncState state = new InboundProxyLayer.OutstandingReadCallbackAsyncState(bufferCacheEntry2, lastBuffer, this.outstandingReadCompleteCallback);
							this.outstandingReadCompleteCallback = null;
							ThreadPool.QueueUserWorkItem(InboundProxyLayer.CallOutstandingReadCallBackDelegate, state);
						}
					}
					if (this.unconsumedBytes >= this.maxUnconsumedBytes || endOfData)
					{
						this.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BeginWriteDataEnqueueWriteCallback);
						this.outstandingWriteCompleteCallback = writeCompleteCallback;
						flag = false;
					}
				}
				else
				{
					this.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BeginWriteDataResponseAlreadySet);
				}
			}
			if (flag)
			{
				writeCompleteCallback(response);
			}
		}

		public void BeginReadData(InboundProxyLayer.ReadCompletionCallback readCompleteCallback)
		{
			this.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BeginReadDataEnter);
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "InboundProxyLayer.BeginReadData");
			if (!this.targetResponse.Equals(SmtpResponse.Empty))
			{
				throw new InvalidOperationException("Cannot read from proxy layer after acking message or connection");
			}
			BufferCacheEntry bufferCacheEntry = null;
			if (this.shutdownSmtpOutSession)
			{
				this.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BeginReadDataShutdownSmtpOutSession);
				readCompleteCallback(null, false);
				return;
			}
			bool lastBuffer;
			lock (this.StateLock)
			{
				this.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BeginReadDataEnterLock);
				if (!this.hasActiveCommand)
				{
					throw new InvalidOperationException("Cannot read from proxy layer when there is no active command");
				}
				if (this.outstandingReadCompleteCallback != null)
				{
					throw new InvalidOperationException("There is already a pending read from the proxy layer");
				}
				if (this.bufferQueue.Count != 0)
				{
					this.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BeginReadDataDequeueBuffer);
					bufferCacheEntry = this.bufferQueue.Dequeue();
					this.unconsumedBytes -= (ulong)((long)bufferCacheEntry.Buffer.Length);
					this.bytesRead += (long)bufferCacheEntry.Buffer.Length;
					if (this.outstandingWriteCompleteCallback != null && !this.eodSeen && this.unconsumedBytes < this.maxUnconsumedBytes)
					{
						this.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BeginReadDataCallOutstandingWriteCallback);
						InboundProxyLayer.OutstandingWriteCallbackAsyncState state = new InboundProxyLayer.OutstandingWriteCallbackAsyncState(this.targetResponse, this.outstandingWriteCompleteCallback);
						this.outstandingWriteCompleteCallback = null;
						ThreadPool.QueueUserWorkItem(InboundProxyLayer.CallOutstandingWriteCallBackDelegate, state);
					}
				}
				else
				{
					this.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.BeginReadDataEnqueueReadCallback);
					this.outstandingReadCompleteCallback = readCompleteCallback;
				}
				lastBuffer = (this.eodSeen && this.bufferQueue.Count == 0);
			}
			if (bufferCacheEntry != null)
			{
				readCompleteCallback(bufferCacheEntry, lastBuffer);
			}
		}

		public void AckMessage(AckStatus status, SmtpResponse response, string source, SessionSetupFailureReason failureReason)
		{
			this.AckMessage(status, response, true, source, failureReason);
		}

		public void AckMessage(AckStatus status, SmtpResponse response, bool replaceFailureResponse, string source, SessionSetupFailureReason failureReason)
		{
			this.AckMessage(status, response, replaceFailureResponse, source, failureReason, false);
		}

		public void AckConnection(AckStatus status, SmtpResponse response, SessionSetupFailureReason failureReason)
		{
			this.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.AckConnection);
			ExTraceGlobals.SmtpSendTracer.TraceDebug<AckStatus, SmtpResponse>((long)this.GetHashCode(), "InboundProxyLayer.AckConnection. Ackstatus  = {0}. SmtpResponse = {1}.", status, response);
			this.AckMessage(status, response, true, "InboundProxyLayer.AckConnection", failureReason, true);
		}

		public void ReleaseMailItem()
		{
			this.mailItem.ReleaseFromActive();
		}

		public void Shutdown()
		{
			this.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.Shutdown);
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "InboundProxyLayer.Shutdown");
			this.AckMessage(AckStatus.Fail, SmtpResponse.ServiceUnavailable, "InboundProxyLayer.Shutdown", SessionSetupFailureReason.Shutdown);
			this.ShutdownProxySession();
		}

		public abstract void WaitForNewCommand(InboundBdatProxyLayer.CommandReceivedCallback commandReceivedCallback);

		public abstract void AckCommandSuccessful();

		protected abstract void ShutdownProxySession();

		private void StartConnection(object state)
		{
			string text = string.Format("Starting outbound connection for inbound session {0}", this.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo));
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), text);
			InboundProxyNextHopConnection connection = new InboundProxyNextHopConnection(this, this.proxyNextHopSolutionKey, this.mailItem);
			this.SmtpOutConnectionHandler.HandleProxyConnection(connection, this.proxyDestinations, this.internalDestination, text);
		}

		private void AckMessage(AckStatus status, SmtpResponse response, bool replaceFailureResponse, string source, SessionSetupFailureReason failureReason, bool connectionAcked)
		{
			this.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.AckMessageEnter);
			ExTraceGlobals.SmtpSendTracer.TraceDebug<AckStatus, SmtpResponse, bool>((long)this.GetHashCode(), "InboundProxyLayer.AckMessage. Ackstatus  = {0}. SmtpResponse = {1}. ReplaceFailureResponse = {2}", status, response, replaceFailureResponse);
			lock (this.StateLock)
			{
				this.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.AckMessageEnterLock);
				if (this.targetResponse.Equals(SmtpResponse.Empty))
				{
					if (replaceFailureResponse && status != AckStatus.Success)
					{
						this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Message or connection acked with status {0} and response {1}", new object[]
						{
							status,
							response
						});
					}
					if (status != AckStatus.Pending)
					{
						if (replaceFailureResponse && !this.preserveTargetResponse && status != AckStatus.Success)
						{
							response = InboundProxyLayer.GetEncodedProxyFailureResponse(failureReason);
						}
						if (connectionAcked && status == AckStatus.Success)
						{
							response = SmtpResponse.MessageNotProxiedResponse;
						}
						this.targetResponse = response;
						this.hasActiveCommand = false;
						if (this.outstandingWriteCompleteCallback != null)
						{
							this.DropBreadcrumb(InboundProxyLayer.InboundProxyLayerBreadcrumbs.AckMessageCallOutstandingWriteCallback);
							InboundProxyLayer.OutstandingWriteCallbackAsyncState state = new InboundProxyLayer.OutstandingWriteCallbackAsyncState(this.targetResponse, this.outstandingWriteCompleteCallback);
							this.outstandingWriteCompleteCallback = null;
							ThreadPool.QueueUserWorkItem(InboundProxyLayer.CallOutstandingWriteCallBackDelegate, state);
						}
					}
				}
				this.ackSource = this.ackSource + ":" + source;
			}
		}

		public void ReturnBuffer(BufferCacheEntry bufferCacheEntry)
		{
			this.SmtpOutConnectionHandler.BufferCache.ReturnBuffer(bufferCacheEntry);
		}

		private const string InternalProxyNextHopDomain = "InternalProxy";

		private const string ExternalProxyNextHopDomain = "ExternalProxy";

		private const int NumberOfBreadcrumbs = 64;

		public static readonly WaitCallback CallOutstandingReadCallBackDelegate = new WaitCallback(InboundProxyLayer.CallOutstandingReadCallback);

		public static readonly WaitCallback CallOutstandingWriteCallBackDelegate = new WaitCallback(InboundProxyLayer.CallOutstandingWriteCallback);

		protected readonly object StateLock = new object();

		protected readonly SmtpOutConnectionHandler SmtpOutConnectionHandler;

		protected InboundProxyLayer.CompletionCallback outstandingWriteCompleteCallback;

		protected InboundProxyLayer.ReadCompletionCallback outstandingReadCompleteCallback;

		protected bool hasActiveCommand;

		protected SmtpResponse targetResponse = SmtpResponse.Empty;

		protected bool discardingInboundData;

		protected bool shutdownSmtpOutSession;

		protected bool eodSeen;

		private readonly ulong maxUnconsumedBytes;

		private readonly IProtocolLogSession logSession;

		private readonly ulong sessionId;

		private readonly string nextHopFqdn;

		private readonly uint xProxyFromSeqNum;

		private readonly bool preserveTargetResponse;

		private readonly IEnumerable<INextHopServer> proxyDestinations;

		private readonly bool internalDestination;

		private readonly InboundProxyRoutedMailItem mailItem;

		private readonly NextHopSolutionKey proxyNextHopSolutionKey;

		private readonly string clientHelloDomain;

		private readonly Permission permissions;

		private readonly AuthenticationSource authenticationSource;

		private readonly Queue<BufferCacheEntry> bufferQueue = new Queue<BufferCacheEntry>();

		private ulong unconsumedBytes;

		private readonly IPEndPoint clientEndPoint;

		private readonly IEhloOptions smtpInEhloOptions;

		private long bytesRead;

		private long bytesWritten;

		private string ackSource = string.Empty;

		private readonly IInboundProxyDestinationTracker inboundProxyDestinationTracker;

		private readonly Breadcrumbs<InboundProxyLayer.InboundProxyLayerBreadcrumbs> breadcrumbs = new Breadcrumbs<InboundProxyLayer.InboundProxyLayerBreadcrumbs>(64);

		public delegate void CompletionCallback(SmtpResponse response);

		public delegate void ReadCompletionCallback(BufferCacheEntry bytes, bool lastBuffer);

		public enum InboundProxyLayerBreadcrumbs
		{
			EMPTY,
			SetupProxySession,
			BeginWriteDataEnter,
			BeginWriteDataEnterLock,
			BeginWriteDataResponseAlreadySet,
			BeginWriteDataEnqueueBuffer,
			BeginWriteDataCallOutstandingReadCallback,
			BeginWriteDataEnqueueWriteCallback,
			BeginReadDataEnter,
			BeginReadDataShutdownSmtpOutSession,
			BeginReadDataEnterLock,
			BeginReadDataDequeueBuffer,
			BeginReadDataCallOutstandingWriteCallback,
			BeginReadDataEnqueueReadCallback,
			AckMessageEnter,
			AckMessageEnterLock,
			AckMessageCallOutstandingWriteCallback,
			AckConnection,
			BdatShutdownProxySessionEnter,
			BdatShutdownProxySessionEnterLock,
			BdatShutdownProxySessionCallOutstandingReadCallback,
			BdatShutdownProxySessionCallOutstandingWaitForCommandCallback,
			BdatCreateNewCommandEnter,
			BdatCreateNewCommandEnterLock,
			BdatCreateNewCommandCallOutstandingWaitForCommandCallback,
			BdatWaitForNewCommand,
			BdatWaitForNewCommandEnterLock,
			BdatAckCommandSuccessful,
			BdatAckCommandSuccessfulEnterLock,
			BdatAckCommandSuccessfulCallOutstandingWriteCallback,
			DataShutdownProxySessionEnter,
			DataShutdownProxySessionEnterLock,
			DataShutdownProxySessionCallOutstandingReadCallback,
			Shutdown
		}

		public class OutstandingReadCallbackAsyncState
		{
			public OutstandingReadCallbackAsyncState(BufferCacheEntry buffer, bool lastBuffer, InboundProxyLayer.ReadCompletionCallback callback)
			{
				if (callback == null)
				{
					throw new ArgumentNullException("callback");
				}
				this.Buffer = buffer;
				this.LastBuffer = lastBuffer;
				this.Callback = callback;
			}

			public readonly BufferCacheEntry Buffer;

			public readonly InboundProxyLayer.ReadCompletionCallback Callback;

			public readonly bool LastBuffer;
		}

		public class OutstandingWriteCallbackAsyncState
		{
			public OutstandingWriteCallbackAsyncState(SmtpResponse response, InboundProxyLayer.CompletionCallback callback)
			{
				if (callback == null)
				{
					throw new ArgumentNullException("callback");
				}
				this.Response = response;
				this.Callback = callback;
			}

			public readonly InboundProxyLayer.CompletionCallback Callback;

			public readonly SmtpResponse Response;
		}
	}
}
