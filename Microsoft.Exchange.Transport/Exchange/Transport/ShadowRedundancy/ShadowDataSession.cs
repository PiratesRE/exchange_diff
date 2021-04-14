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
	internal class ShadowDataSession : ShadowSession
	{
		public ShadowDataSession(ISmtpInSession inSession, ShadowRedundancyManager shadowRedundancyManager, ShadowHubPickerBase hubPicker, ISmtpOutConnectionHandler connectionHandler) : base(inSession, shadowRedundancyManager, hubPicker, connectionHandler)
		{
			this.writeQueue = new Queue<WriteRecord>();
		}

		public override void PrepareForNewCommand(BaseDataSmtpCommand newCommand)
		{
			if (this.proxyLayer == null)
			{
				throw new InvalidOperationException("BeginWrite called without calling BeginOpen");
			}
			base.DropBreadcrumb(ShadowBreadcrumbs.PrepareForCommand);
		}

		protected override void LoadNewProxyLayer()
		{
			this.proxyLayer = new InboundDataProxyLayer(this.inSession.SessionId, this.inSession.ClientEndPoint, this.inSession.HelloDomain, this.inSession.AdvertisedEhloOptions, 0U, this.inSession.TransportMailItem, true, new INextHopServer[0], ByteQuantifiedSize.FromMB(1UL).ToBytes(), this.inSession.LogSession, this.inSession.SmtpInServer.SmtpOutConnectionHandler, false, Permission.None, AuthenticationSource.Anonymous, this.inSession.SmtpInServer.InboundProxyDestinationTracker);
		}

		protected override void WriteInternal(byte[] buffer, int offset, int count, bool seenEod)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowDataSession writing raw data to proxy layer: length={0} offset={1} count={2} eod={3}", new object[]
			{
				buffer.Length,
				offset,
				count,
				seenEod
			});
			base.DropBreadcrumb(ShadowBreadcrumbs.WriteInternal);
			lock (this.syncObject)
			{
				if (!base.HasPendingProxyCallback)
				{
					base.WriteToProxy(buffer, offset, count, seenEod, true);
				}
				else
				{
					base.DropBreadcrumb(ShadowBreadcrumbs.WriteQueuingBuffer);
					this.writeQueue.Enqueue(new WriteRecord(buffer, offset, count, seenEod));
				}
			}
		}

		protected override void WriteProxyDataComplete(SmtpResponse response)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowDataSession.WriteProxyDataComplete");
			lock (this.syncObject)
			{
				base.WriteProxyDataComplete(response);
				if (this.writeQueue.Count > 0)
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "WriteProxyDataComplete dequeuing write and sending to shadow");
					base.DropBreadcrumb(ShadowBreadcrumbs.WriteDequeuingBuffer);
					base.WriteToProxy(this.writeQueue.Dequeue());
				}
			}
		}

		protected Queue<WriteRecord> writeQueue;
	}
}
