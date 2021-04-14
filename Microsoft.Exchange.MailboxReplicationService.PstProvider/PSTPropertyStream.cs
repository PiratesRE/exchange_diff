using System;
using System.IO;
using Microsoft.Exchange.PST;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PSTPropertyStream : Stream
	{
		public PSTPropertyStream(bool isRead, IProperty pstProperty)
		{
			this.position = 0;
			this.overflowBuffer = new byte[0];
			this.overflowPosition = 0;
			this.propTag = new PropertyTag(pstProperty.PropTag);
			if (isRead)
			{
				this.pstStreamReader = pstProperty.OpenStreamReader();
				this.length = (long)this.pstStreamReader.Length;
				return;
			}
			this.pstStreamWriter = pstProperty.OpenStreamWriter();
		}

		public PropertyTag PropTag
		{
			get
			{
				return this.propTag;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.pstStreamReader != null;
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
				return this.pstStreamWriter != null;
			}
		}

		public override long Length
		{
			get
			{
				return this.length;
			}
		}

		public override long Position
		{
			get
			{
				return (long)this.position;
			}
			set
			{
				if (value != 0L)
				{
					throw new NotImplementedException("PSTPropertyStream does not implement 'public override long Position'");
				}
			}
		}

		public override void Close()
		{
			if (this.pstStreamReader != null)
			{
				this.pstStreamReader.Close();
				this.pstStreamReader = null;
				return;
			}
			if (this.pstStreamWriter != null)
			{
				this.pstStreamWriter.Close();
				this.pstStreamWriter = null;
			}
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (count < 0 || offset < 0)
			{
				throw new UnableToStreamPSTPropPermanentException(this.propTag, offset, count, this.Length);
			}
			int i = count;
			while (i > 0)
			{
				if (this.overflowBuffer.Length - this.overflowPosition <= i)
				{
					if (this.overflowBuffer.Length - this.overflowPosition > 0)
					{
						Array.Copy(this.overflowBuffer, this.overflowPosition, buffer, offset + count - i, this.overflowBuffer.Length - this.overflowPosition);
						this.position += this.overflowBuffer.Length - this.overflowPosition;
						i -= this.overflowBuffer.Length - this.overflowPosition;
					}
					if (this.pstStreamReader.IsEnd)
					{
						if (i != 0)
						{
							throw new UnableToStreamPSTPropPermanentException(this.propTag, offset, i, this.Length);
						}
						break;
					}
					else
					{
						this.overflowBuffer = this.pstStreamReader.Read();
						this.overflowPosition = 0;
					}
				}
				else
				{
					Array.Copy(this.overflowBuffer, this.overflowPosition, buffer, offset + count - i, i);
					this.overflowPosition += i;
					this.position += i;
					i = 0;
				}
			}
			return count - i;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException("PSTPropertyStream does not implement 'public override long Seek'");
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException("PSTPropertyStream does not implement 'public override void SetLength'");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			byte[] array = new byte[count];
			Array.Copy(buffer, offset, array, 0, count);
			this.pstStreamWriter.Write(array);
			this.position += count;
			this.length += (long)count;
		}

		protected override void Dispose(bool disposing)
		{
		}

		private readonly PropertyTag propTag;

		private IPropertyReader pstStreamReader;

		private IPropertyWriter pstStreamWriter;

		private long length;

		private byte[] overflowBuffer;

		private int overflowPosition;

		private int position;
	}
}
