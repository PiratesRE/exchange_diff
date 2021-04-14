using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal sealed class MapFileStream : Stream
	{
		public MapFileStream(SafeViewOfFileHandle viewHandle, int length, bool writable)
		{
			this.viewHandle = viewHandle;
			this.length = (long)length;
			this.writable = writable;
			this.position = 0L;
		}

		public override bool CanRead
		{
			get
			{
				return this.viewHandle != null && !this.viewHandle.IsInvalid;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.viewHandle != null && !this.viewHandle.IsInvalid;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.viewHandle != null && !this.viewHandle.IsInvalid && this.writable;
			}
		}

		public override long Length
		{
			get
			{
				if (this.viewHandle == null || this.viewHandle.IsInvalid)
				{
					return 0L;
				}
				return this.length;
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
				this.ThrowIfInvalidHandle();
				if (value < 0L || value > this.length)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.position = value;
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.ThrowIfInvalidHandle();
			long num;
			switch (origin)
			{
			case SeekOrigin.Begin:
				num = offset;
				break;
			case SeekOrigin.Current:
				num = this.position + offset;
				break;
			case SeekOrigin.End:
				num = this.length + offset;
				break;
			default:
				throw new ArgumentException("origin");
			}
			if (num < 0L || num > this.length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			this.position = num;
			return this.position;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.ThrowIfInvalidHandle();
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("buffer", "insufficient buffer size");
			}
			int num = (int)Math.Min(this.length - this.position, (long)count);
			Marshal.Copy((IntPtr)((long)this.viewHandle.DangerousGetHandle() + this.position), buffer, offset, num);
			this.position += (long)num;
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.ThrowIfInvalidHandle();
			if (!this.writable)
			{
				throw new InvalidOperationException("stream not writable");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("buffer", "insufficient buffer size");
			}
			int num = (int)Math.Min(this.length - this.position, (long)count);
			Marshal.Copy(buffer, offset, (IntPtr)((long)this.viewHandle.DangerousGetHandle() + this.position), num);
			this.position += (long)num;
		}

		public override void Flush()
		{
			this.ThrowIfInvalidHandle();
			bool flag = NativeMethods.FlushViewOfFile(this.viewHandle, (UIntPtr)((ulong)this.length));
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (!flag)
			{
				throw new IOException("FlushViewOfFile failed", (lastWin32Error == 0) ? null : new Win32Exception(lastWin32Error));
			}
		}

		public override void Close()
		{
			if (this.viewHandle != null)
			{
				this.viewHandle.Dispose();
				base.Close();
				this.viewHandle = null;
			}
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		private void ThrowIfInvalidHandle()
		{
			if (this.viewHandle == null)
			{
				throw new ObjectDisposedException("MapFileStream");
			}
			if (this.viewHandle.IsInvalid)
			{
				throw new InvalidOperationException("Invalid handle");
			}
		}

		private readonly long length;

		private readonly bool writable;

		private SafeViewOfFileHandle viewHandle;

		private long position;
	}
}
