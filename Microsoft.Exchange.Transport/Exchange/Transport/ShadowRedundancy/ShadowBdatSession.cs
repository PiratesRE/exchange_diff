using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal class ShadowBdatSession : ShadowSession
	{
		public ShadowBdatSession(ISmtpInSession inSession, ShadowRedundancyManager shadowRedundancyManager, ShadowHubPickerBase hubPicker, ISmtpOutConnectionHandler connectionHandler) : base(inSession, shadowRedundancyManager, hubPicker, connectionHandler)
		{
			this.commandQueue = new Queue<ShadowBdatSession.BdatCommandRecord>();
			this.writeQueue = new Queue<WriteRecord>();
		}

		public override void PrepareForNewCommand(BaseDataSmtpCommand newCommand)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowBdatSession.PrepareForNewCommand");
			base.DropBreadcrumb(ShadowBreadcrumbs.PrepareForCommand);
			BdatSmtpCommand bdatSmtpCommand = newCommand as BdatSmtpCommand;
			if (bdatSmtpCommand == null)
			{
				throw new ArgumentException("newCommand must be of type BdatSmtpCommand");
			}
			if (this.proxyLayer == null)
			{
				throw new InvalidOperationException("BeginWrite called without calling BeginOpen");
			}
			if (bdatSmtpCommand.IsBlob)
			{
				throw new InvalidOperationException("Shadow session does not support transferring of Messagecontext information");
			}
			lock (this.syncObject)
			{
				ShadowBdatSession.BdatCommandRecord bdatCommandRecord = new ShadowBdatSession.BdatCommandRecord(bdatSmtpCommand.ChunkSize, bdatSmtpCommand.IsLastChunk);
				if (this.currentCommand == null)
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowBdatSession directly passing command to proxy layer");
					this.RegisterCommand(bdatCommandRecord);
				}
				else
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<long, bool>((long)this.GetHashCode(), "ShadowBdatSession enqueuing command: {0} isLast:{1}", bdatCommandRecord.ChunkSize, bdatCommandRecord.IsLastChunk);
					base.DropBreadcrumb(ShadowBreadcrumbs.WriteQueuingCommand);
					this.commandQueue.Enqueue(bdatCommandRecord);
				}
			}
		}

		protected override void LoadNewProxyLayer()
		{
			this.proxyLayer = new InboundBdatProxyLayer(this.inSession.SessionId, this.inSession.ClientEndPoint, this.inSession.HelloDomain, this.inSession.AdvertisedEhloOptions, 0U, this.inSession.TransportMailItem, true, new INextHopServer[0], ByteQuantifiedSize.FromMB(1UL).ToBytes(), this.inSession.LogSession, this.inSession.SmtpInServer.SmtpOutConnectionHandler, false, Permission.None, AuthenticationSource.Anonymous, this.inSession.SmtpInServer.InboundProxyDestinationTracker);
		}

		protected override void WriteInternal(byte[] buffer, int offset, int count, bool seenEod)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowBdatSession.WriteInternal");
			base.DropBreadcrumb(ShadowBreadcrumbs.WriteInternal);
			if (this.currentCommand == null)
			{
				throw new InvalidOperationException("WriteInternal called before PrepareForNewCommand");
			}
			lock (this.syncObject)
			{
				if (this.commandQueue.Count == 0 && !base.HasPendingProxyCallback)
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowBdatSession directly writing to shadow");
					base.WriteToProxy(buffer, offset, count, seenEod, true);
				}
				else
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowBdatSession enqueuing write");
					base.DropBreadcrumb(ShadowBreadcrumbs.WriteQueuingBuffer);
					WriteRecord item = new WriteRecord(buffer, offset, count, seenEod);
					this.writeQueue.Enqueue(item);
				}
			}
		}

		protected override void WriteProxyDataComplete(SmtpResponse response)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowBdatSession.WriteProxyDataComplete");
			lock (this.syncObject)
			{
				base.WriteProxyDataComplete(response);
				if (response.Equals(SmtpResponse.Empty))
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "WriteProxyDataComplete handling acked write");
					if (this.writeQueue.Count > 0)
					{
						ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "WriteProxyDataComplete dequeuing write and sending to shadow");
						base.DropBreadcrumb(ShadowBreadcrumbs.WriteDequeuingBuffer);
						base.WriteToProxy(this.writeQueue.Dequeue());
					}
				}
				else if (response.SmtpResponseType == SmtpResponseType.Success)
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "WriteProxyDataComplete handling acked command");
					if (this.commandQueue.Count > 0)
					{
						ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "WriteProxyDataComplete dequeuing and registering command");
						base.DropBreadcrumb(ShadowBreadcrumbs.WriteDequeuingCommand);
						this.RegisterCommand(this.commandQueue.Dequeue());
					}
					else
					{
						ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "WriteProxyDataComplete has no pending commands");
						this.currentCommand = null;
					}
					if (this.writeQueue.Count > 0)
					{
						ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "WriteProxyDataComplete dequeuing write and sending to shadow");
						base.DropBreadcrumb(ShadowBreadcrumbs.WriteDequeuingBuffer);
						base.WriteToProxy(this.writeQueue.Dequeue());
					}
				}
			}
		}

		private void RegisterCommand(ShadowBdatSession.BdatCommandRecord nextBdatCommand)
		{
			if (base.IsClosed)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowSession is aborted, skipping RegisterCommand");
				return;
			}
			this.currentCommand = nextBdatCommand;
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<long, bool>((long)this.GetHashCode(), "Registering new BDAT command: {0} isLast:{1}", nextBdatCommand.ChunkSize, nextBdatCommand.IsLastChunk);
			base.DropBreadcrumb(ShadowBreadcrumbs.WriteNewBdatCommand);
			(this.proxyLayer as InboundBdatProxyLayer).CreateNewCommand(nextBdatCommand.ChunkSize, nextBdatCommand.ChunkSize, nextBdatCommand.IsLastChunk);
		}

		protected Queue<WriteRecord> writeQueue;

		protected Queue<ShadowBdatSession.BdatCommandRecord> commandQueue;

		private ShadowBdatSession.BdatCommandRecord currentCommand;

		internal class BdatCommandRecord
		{
			public bool IsLastChunk { get; private set; }

			public long ChunkSize { get; private set; }

			public BdatCommandRecord(long chunkSize, bool isLastChunk)
			{
				this.ChunkSize = chunkSize;
				this.IsLastChunk = isLastChunk;
			}
		}
	}
}
