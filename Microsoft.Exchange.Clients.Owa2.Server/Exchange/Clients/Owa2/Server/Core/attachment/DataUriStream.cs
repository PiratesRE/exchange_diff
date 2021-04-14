using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.attachment
{
	public class DataUriStream : Stream
	{
		public DataUriStream(Stream dataStream, string contentType)
		{
			if (dataStream == null)
			{
				throw new ArgumentNullException("dataStream");
			}
			if (string.IsNullOrEmpty(contentType))
			{
				throw new ArgumentNullException("contentType");
			}
			this.templateStream = this.GetTemplateStream(contentType);
			this.dataStream = dataStream;
			this.streams = new Queue<Stream>();
			this.streams.Enqueue(this.templateStream);
			this.streams.Enqueue(this.dataStream);
			this.SetCurrentStream();
			this.templateStream.Seek(0L, SeekOrigin.Begin);
			this.dataStream.Seek(0L, SeekOrigin.Begin);
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				this.ThrowIfDisposed();
				return this.templateStream.Length + this.dataStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.ThrowIfDisposed();
			if (this.currentStream == null)
			{
				return 0;
			}
			int num = this.currentStream.Read(buffer, offset, count);
			if (num <= 0)
			{
				this.SetCurrentStream();
				return this.Read(buffer, offset, count);
			}
			return num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.dataStream != null)
				{
					this.dataStream.Dispose();
					this.dataStream = null;
				}
				if (this.templateStream != null)
				{
					this.templateStream.Dispose();
					this.templateStream = null;
				}
			}
			base.Dispose(disposing);
		}

		private Stream GetTemplateStream(string contentType)
		{
			return new MemoryStream(new UTF8Encoding().GetBytes(string.Format("data:{0};base64,", contentType)));
		}

		private void SetCurrentStream()
		{
			if (this.streams.Count > 0)
			{
				this.currentStream = this.streams.Dequeue();
				return;
			}
			this.currentStream = null;
		}

		private void ThrowIfDisposed()
		{
			if (this.dataStream == null || this.templateStream == null)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
		}

		public const string DataUriTemplate = "data:{0};base64,";

		private Stream dataStream;

		private Stream templateStream;

		private Queue<Stream> streams;

		private Stream currentStream;
	}
}
