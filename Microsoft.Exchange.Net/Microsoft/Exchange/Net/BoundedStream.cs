using System;
using System.IO;

namespace Microsoft.Exchange.Net
{
	internal class BoundedStream : StreamWrapper
	{
		public BoundedStream(Stream wrappedStream, bool canDispose, long indexOfFirstByte, long indexOfLastByte) : base(wrappedStream, canDispose)
		{
			this.indexOfFirstByte = indexOfFirstByte;
			this.indexOfLastByte = indexOfLastByte;
			this.boundedLength = this.indexOfLastByte - this.indexOfFirstByte + 1L;
		}

		public override long Length
		{
			get
			{
				return Math.Min(base.Length, this.boundedLength);
			}
		}

		public override long Position
		{
			get
			{
				return base.Position - this.indexOfFirstByte;
			}
			set
			{
				if (value > this.boundedLength)
				{
					throw this.BoundsViolationException();
				}
				base.Position = value + this.indexOfFirstByte;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return base.Read(buffer, offset, Math.Min(checked((int)(this.boundedLength - this.Position)), count));
		}

		public override int ReadByte()
		{
			if (this.Position < this.boundedLength)
			{
				return base.ReadByte();
			}
			return -1;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			long num;
			switch (origin)
			{
			case SeekOrigin.Begin:
				num = offset;
				break;
			case SeekOrigin.Current:
				num = this.Position + offset;
				break;
			case SeekOrigin.End:
				num = this.Length - offset;
				break;
			default:
				throw new ArgumentException(string.Empty, "origin");
			}
			if (num > this.boundedLength)
			{
				throw this.BoundsViolationException();
			}
			return base.Seek(this.indexOfFirstByte + num, SeekOrigin.Begin);
		}

		public override void SetLength(long value)
		{
			if (value > this.boundedLength)
			{
				throw this.BoundsViolationException();
			}
			base.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.Position + (long)count > this.boundedLength)
			{
				throw this.BoundsViolationException();
			}
			base.Write(buffer, offset, count);
		}

		public override void WriteByte(byte value)
		{
			if (this.Position >= this.boundedLength)
			{
				throw this.BoundsViolationException();
			}
			base.WriteByte(value);
		}

		private Exception BoundsViolationException()
		{
			return new IOException(string.Format("The stream is opened for positions ({0}, {1}). Can't operate outside of this range.", this.indexOfFirstByte, this.indexOfLastByte));
		}

		private readonly long indexOfFirstByte;

		private readonly long indexOfLastByte;

		private readonly long boundedLength;
	}
}
