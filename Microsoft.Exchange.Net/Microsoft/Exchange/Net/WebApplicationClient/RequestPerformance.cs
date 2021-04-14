using System;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	public struct RequestPerformance
	{
		internal RequestPerformance(SendRequestOperation sendRequestOperation)
		{
			this.sendRequestOperation = sendRequestOperation;
			this.bytesSent = -1L;
			this.bytesReceived = -1L;
		}

		public long BytesSent
		{
			get
			{
				if (this.sendRequestOperation == null)
				{
					return 0L;
				}
				if (this.bytesSent == -1L)
				{
					this.bytesSent = this.sendRequestOperation.BytesSent;
				}
				return this.bytesSent;
			}
		}

		public long BytesReceived
		{
			get
			{
				if (this.sendRequestOperation == null)
				{
					return 0L;
				}
				if (this.bytesReceived == -1L)
				{
					this.bytesReceived = this.sendRequestOperation.BytesReceived;
				}
				return this.bytesReceived;
			}
		}

		public long ElapsedTicks
		{
			get
			{
				if (this.sendRequestOperation == null)
				{
					return 0L;
				}
				return this.sendRequestOperation.ElapsedTicks;
			}
		}

		private SendRequestOperation sendRequestOperation;

		private long bytesSent;

		private long bytesReceived;
	}
}
