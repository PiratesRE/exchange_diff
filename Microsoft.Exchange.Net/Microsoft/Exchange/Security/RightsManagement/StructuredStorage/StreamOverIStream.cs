using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Exchange.Security.RightsManagement.StructuredStorage
{
	internal class StreamOverIStream : Stream
	{
		public StreamOverIStream(IStream stream)
		{
			this.stream = stream;
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
				return true;
			}
		}

		public override long Length
		{
			get
			{
				STATSTG statstg = default(STATSTG);
				this.stream.Stat(out statstg, STATFLAG.NoName);
				return statstg.cbSize;
			}
		}

		public override long Position
		{
			get
			{
				return this.stream.Seek(0L, 1);
			}
			set
			{
				this.stream.Seek(value, 0);
			}
		}

		public override void Flush()
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.stream.Seek(offset, (int)origin);
		}

		public override void SetLength(long value)
		{
			this.stream.SetSize(value);
		}

		public unsafe override int Read(byte[] buffer, int offset, int count)
		{
			fixed (byte* ptr = buffer)
			{
				return this.stream.Read(new IntPtr((void*)((byte*)ptr + offset)), count);
			}
		}

		public unsafe override void Write(byte[] buffer, int offset, int count)
		{
			int num;
			fixed (byte* ptr = buffer)
			{
				num = this.stream.Write(new IntPtr((void*)((byte*)ptr + offset)), count);
			}
			if (num != count)
			{
				throw new InvalidOperationException("Failed to write to IStream.");
			}
		}

		internal void ReplaceIStream(IStream newStream)
		{
			this.stream = newStream;
		}

		private IStream stream;
	}
}
