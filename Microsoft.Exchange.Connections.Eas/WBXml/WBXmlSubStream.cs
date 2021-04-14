using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.WBXml
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class WBXmlSubStream : StreamAccessor
	{
		internal WBXmlSubStream(Stream stream, long startPosition, long length) : base(stream)
		{
			this.startPosition = startPosition;
			this.length = length;
		}

		internal override long Length
		{
			get
			{
				return this.length;
			}
		}

		internal override long Position
		{
			get
			{
				return base.InternalStream.Position - this.startPosition;
			}
			set
			{
				if (value > this.length)
				{
					base.InternalStream.Position = this.startPosition + this.length;
					return;
				}
				base.InternalStream.Position = this.startPosition + value;
			}
		}

		internal override long Seek(long offset, SeekOrigin origin)
		{
			long num;
			if (origin == SeekOrigin.Begin)
			{
				num = this.startPosition + offset;
			}
			else if (origin == SeekOrigin.Current)
			{
				num = base.InternalStream.Position + offset;
			}
			else
			{
				if (origin != SeekOrigin.End)
				{
					throw new ArgumentException();
				}
				num = this.length + offset;
			}
			if (num < this.startPosition)
			{
				num = this.startPosition;
			}
			if (num > this.startPosition + this.length)
			{
				num = this.startPosition + this.length;
			}
			return base.InternalStream.Seek(num, SeekOrigin.Begin);
		}

		internal override int Read(byte[] buffer, int offset, int count)
		{
			if (base.InternalStream.Position + (long)count > this.startPosition + this.length)
			{
				count = (int)(this.startPosition + this.length - base.InternalStream.Position);
			}
			if (count > 0)
			{
				return base.InternalStream.Read(buffer, offset, count);
			}
			return 0;
		}

		private readonly long startPosition;

		private readonly long length;
	}
}
