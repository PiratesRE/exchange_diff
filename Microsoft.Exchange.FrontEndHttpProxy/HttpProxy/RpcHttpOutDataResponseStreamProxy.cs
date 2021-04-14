using System;
using System.IO;
using System.Net;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.HttpProxy
{
	internal class RpcHttpOutDataResponseStreamProxy : StreamProxy
	{
		internal RpcHttpOutDataResponseStreamProxy(StreamProxy.StreamProxyType streamProxyType, Stream source, Stream target, byte[] buffer, IRequestContext requestContext) : base(streamProxyType, source, target, buffer, requestContext)
		{
			this.connectTimeout = RpcHttpOutDataResponseStreamProxy.RpcHttpOutConnectingTimeoutInSeconds.Value;
			this.isConnecting = (this.connectTimeout != TimeSpan.Zero);
		}

		internal RpcHttpOutDataResponseStreamProxy(StreamProxy.StreamProxyType streamProxyType, Stream source, Stream target, BufferPoolCollection.BufferSize maxBufferSize, BufferPoolCollection.BufferSize minBufferSize, IRequestContext requestContext) : base(streamProxyType, source, target, maxBufferSize, minBufferSize, requestContext)
		{
			this.connectTimeout = RpcHttpOutDataResponseStreamProxy.RpcHttpOutConnectingTimeoutInSeconds.Value;
			this.isConnecting = (this.connectTimeout != TimeSpan.Zero);
		}

		protected override byte[] GetUpdatedBufferToSend(ArraySegment<byte> buffer)
		{
			if (!this.isConnecting)
			{
				return null;
			}
			if (RpcHttpPackets.IsConnA3PacketInBuffer(buffer))
			{
				this.endTime = new ExDateTime?(ExDateTime.Now + this.connectTimeout);
			}
			if (RpcHttpPackets.IsConnC2PacketInBuffer(buffer))
			{
				this.isConnecting = false;
				this.endTime = null;
			}
			if (RpcHttpPackets.IsPingPacket(buffer) && this.endTime != null && ExDateTime.Now >= this.endTime.Value)
			{
				Exception ex = new HttpProxyException(HttpStatusCode.InternalServerError, HttpProxySubErrorCode.RpcHttpConnectionEstablishmentTimeout, "Outbound proxy connection timed out");
				throw ex;
			}
			return null;
		}

		private static readonly TimeSpanAppSettingsEntry RpcHttpOutConnectingTimeoutInSeconds = new TimeSpanAppSettingsEntry(HttpProxySettings.Prefix("RpcHttpOutConnectingTimeoutInSeconds"), TimeSpanUnit.Seconds, TimeSpan.FromSeconds(0.0), ExTraceGlobals.VerboseTracer);

		private readonly TimeSpan connectTimeout = TimeSpan.Zero;

		private bool isConnecting;

		private ExDateTime? endTime;
	}
}
