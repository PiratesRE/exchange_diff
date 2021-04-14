using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class PooledBufferResponseItem : IResponseItem, IDisposeTrackable, IDisposable
	{
		public PooledBufferResponseItem(int bufferSize)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.pooledMemoryStream = new PooledMemoryStream(bufferSize);
		}

		public BaseSession.SendCompleteDelegate SendCompleteDelegate
		{
			get
			{
				return null;
			}
		}

		public int Size
		{
			get
			{
				return (int)this.pooledMemoryStream.Position;
			}
		}

		public virtual int GetNextChunk(BaseSession session, out byte[] buffer, out int offset)
		{
			buffer = this.pooledMemoryStream.GetBuffer();
			offset = 0;
			int result = (int)this.pooledMemoryStream.Position;
			this.pooledMemoryStream.Position = 0L;
			return result;
		}

		public void Write(byte[] buffer, int offset, int count)
		{
			this.pooledMemoryStream.Write(buffer, offset, count);
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<PooledBufferResponseItem>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
				this.disposeTracker = null;
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
			if (this.pooledMemoryStream != null)
			{
				this.pooledMemoryStream.Dispose();
				this.pooledMemoryStream = null;
			}
		}

		private PooledMemoryStream pooledMemoryStream;

		private DisposeTracker disposeTracker;
	}
}
