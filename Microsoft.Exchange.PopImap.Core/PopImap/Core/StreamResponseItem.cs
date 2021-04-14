using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class StreamResponseItem : IResponseItem, IDisposeTrackable, IDisposable
	{
		public StreamResponseItem(Stream s) : this(s, null)
		{
		}

		public StreamResponseItem(Stream s, BaseSession.SendCompleteDelegate sendCompleteDelegate)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.sendCompleteDelegate = sendCompleteDelegate;
			this.responseStream = s;
		}

		public BaseSession.SendCompleteDelegate SendCompleteDelegate
		{
			get
			{
				return this.sendCompleteDelegate;
			}
		}

		public int GetNextChunk(BaseSession session, out byte[] buffer, out int offset)
		{
			if (this.sendBuffer == null)
			{
				this.sendBuffer = StreamResponseItem.sendBufferPool.Acquire();
			}
			buffer = this.sendBuffer;
			offset = 0;
			int num = this.responseStream.Read(buffer, 0, buffer.Length);
			if (num == 0)
			{
				this.responseStream.Close();
				this.responseStream = null;
			}
			else
			{
				ProtocolSession protocolSession = session as ProtocolSession;
				if (protocolSession != null)
				{
					protocolSession.LogSend("{0} bytes", num);
				}
			}
			return num;
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<StreamResponseItem>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			if (this.responseStream != null)
			{
				this.responseStream.Close();
				this.responseStream = null;
			}
			if (this.sendBuffer != null)
			{
				StreamResponseItem.sendBufferPool.Release(this.sendBuffer);
				this.sendBuffer = null;
			}
		}

		private static BufferPool sendBufferPool = new BufferPool(4096);

		private Stream responseStream;

		private BaseSession.SendCompleteDelegate sendCompleteDelegate;

		private byte[] sendBuffer;

		private DisposeTracker disposeTracker;
	}
}
