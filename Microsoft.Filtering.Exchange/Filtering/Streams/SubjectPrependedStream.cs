using System;
using System.IO;
using System.Text;

namespace Microsoft.Filtering.Streams
{
	internal class SubjectPrependedStream : Stream
	{
		public SubjectPrependedStream(string subject, Stream stream)
		{
			this.stream = stream;
			this.subjectBytes = ((!string.IsNullOrEmpty(subject)) ? Encoding.Unicode.GetBytes(subject) : new byte[0]);
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
				return true;
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
				return (long)this.subjectBytes.Length + this.stream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.position;
			}
			set
			{
				this.position = value;
			}
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this.position >= this.Length)
			{
				return 0;
			}
			int num = this.subjectBytes.Length;
			int num2 = 0;
			int num3 = 0;
			if (this.position < (long)num)
			{
				num2 = Math.Min(count, num - (int)this.position);
				Buffer.BlockCopy(this.subjectBytes, (int)this.position, buffer, offset, num2);
				if (this.position + (long)count >= (long)num)
				{
					this.position = (long)num;
					num3 = num2;
				}
			}
			if (this.position >= (long)num)
			{
				this.stream.Position = this.position - (long)num;
				num2 += this.stream.Read(buffer, offset + num2, count - num2);
			}
			this.Seek((long)(num2 - num3), SeekOrigin.Current);
			return num2;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
			case SeekOrigin.Begin:
				this.Position = offset;
				break;
			case SeekOrigin.Current:
				this.Position += offset;
				break;
			case SeekOrigin.End:
				this.Position = this.Length - offset;
				break;
			default:
				throw new ArgumentException("Invalid seek origin");
			}
			return this.Position;
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		private byte[] subjectBytes;

		private Stream stream;

		private long position;
	}
}
