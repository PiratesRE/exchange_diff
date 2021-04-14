using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class StringResponseItemProcessor : IDisposeTrackable, IDisposable
	{
		public StringResponseItemProcessor()
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.bufferResponseItem = new BufferResponseItem();
			this.pooledMemoryStream = new PooledMemoryStream(1024);
			this.streamWriter = new StreamWriter(this.pooledMemoryStream, Encoding.ASCII);
			this.streamWriter.AutoFlush = true;
		}

		public bool DataBound
		{
			get
			{
				return this.stringResponseItem != null;
			}
		}

		public void BindData(StringResponseItem stringResponseItem)
		{
			this.stringResponseItem = stringResponseItem;
			this.bufferResponseItem.ClearData();
			this.pooledMemoryStream.Position = 0L;
			this.streamWriter.Write(this.stringResponseItem.StringResponse);
			if (!string.IsNullOrEmpty(this.stringResponseItem.StringResponse) && !this.stringResponseItem.StringResponse.EndsWith(Strings.CRLF, StringComparison.OrdinalIgnoreCase))
			{
				this.streamWriter.Write(Strings.CRLF);
			}
			this.bufferResponseItem.BindData(this.pooledMemoryStream.GetBuffer(), 0, (int)this.pooledMemoryStream.Position, this.stringResponseItem.SendCompleteDelegate);
		}

		public void UnbindData()
		{
			this.stringResponseItem = null;
		}

		public int GetNextChunk(StringResponseItem stringResponseItem, BaseSession session, out byte[] buffer, out int offset)
		{
			if (!this.DataBound)
			{
				buffer = null;
				offset = 0;
				return 0;
			}
			if (!object.ReferenceEquals(this.stringResponseItem, stringResponseItem))
			{
				throw new InvalidOperationException(string.Format("StringResponseItemProcessor won't support concurrent use for two StringResponseItem instances. \r\n                            Original: '{0}',\r\n                            Current: '{1}'", this.stringResponseItem.StringResponse, stringResponseItem.StringResponse));
			}
			return this.bufferResponseItem.GetNextChunk(session, out buffer, out offset);
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<StringResponseItemProcessor>(this);
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
			if (this.streamWriter != null)
			{
				this.streamWriter.Dispose();
				this.streamWriter = null;
				this.pooledMemoryStream = null;
			}
			if (this.stringResponseItem != null)
			{
				this.stringResponseItem.Dispose();
				this.stringResponseItem = null;
			}
		}

		private BufferResponseItem bufferResponseItem;

		private StreamWriter streamWriter;

		private PooledMemoryStream pooledMemoryStream;

		private StringResponseItem stringResponseItem;

		private DisposeTracker disposeTracker;
	}
}
