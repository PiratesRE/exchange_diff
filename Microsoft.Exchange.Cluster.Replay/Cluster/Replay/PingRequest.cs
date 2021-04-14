using System;
using System.Net;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class PingRequest
	{
		public IPAddress IPAddress { get; set; }

		public long StartTimeStamp { get; set; }

		public long StopTimeStamp { get; set; }

		public long LatencyInUSec { get; set; }

		public bool TimedOut { get; set; }

		public bool Success { get; set; }

		public object UserContext { get; set; }

		public byte[] ReplyBuffer
		{
			get
			{
				return this.m_receiveBuf;
			}
		}

		private const int MaxResponseSize = 256;

		private byte[] m_receiveBuf = new byte[256];
	}
}
