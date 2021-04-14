using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	internal class ComStream : Stream
	{
		internal ComStream(Interop.IStream iStream)
		{
			this.iStream = iStream;
			this.isDisposed = false;
		}

		public Interop.IStream IStream
		{
			get
			{
				return this.iStream;
			}
		}

		public unsafe override int Read(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed("ComStream::Read");
			int result;
			fixed (byte* ptr = &buffer[offset])
			{
				result = this.Read(ptr, count);
			}
			return result;
		}

		public unsafe int Read(byte* pBuffer, int count)
		{
			this.CheckDisposed("ComStream::Read");
			int bytesRead = 0;
			Util.InvokeComCall(MsgStorageErrorCode.FailedRead, delegate
			{
				this.iStream.Read(pBuffer, count, out bytesRead);
			});
			return bytesRead;
		}

		public unsafe override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed("ComStream::Write");
			fixed (byte* ptr = &buffer[offset])
			{
				this.Write(ptr, count);
			}
		}

		public unsafe void Write(byte* pBuffer, int count)
		{
			this.CheckDisposed("ComStream::Write");
			Util.InvokeComCall(MsgStorageErrorCode.FailedWrite, delegate
			{
				int num = 0;
				this.iStream.Write(pBuffer, count, out num);
				if (num != count)
				{
					throw new MsgStorageException(MsgStorageErrorCode.FailedWrite, MsgStorageStrings.FailedWrite(string.Empty));
				}
			});
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.CheckDisposed("ComStream::Seek");
			long position = 0L;
			Util.InvokeComCall(MsgStorageErrorCode.FailedSeek, delegate
			{
				this.iStream.Seek(offset, (int)origin, out position);
			});
			return position;
		}

		public override long Length
		{
			get
			{
				this.CheckDisposed("ComStream::get_Length");
				return this.GetStat().cbSize;
			}
		}

		public override long Position
		{
			get
			{
				this.CheckDisposed("ComStream::get_Position");
				return this.Seek(0L, SeekOrigin.Current);
			}
			set
			{
				this.CheckDisposed("ComStream::set_Position");
				this.Seek(value, SeekOrigin.Begin);
			}
		}

		public override void SetLength(long value)
		{
			this.CheckDisposed("ComStream::SetLength");
			Util.InvokeComCall(MsgStorageErrorCode.FailedWrite, delegate
			{
				this.iStream.SetSize(value);
			});
		}

		public override void Flush()
		{
			this.CheckDisposed("ComStream.Flush()");
			Util.InvokeComCall(MsgStorageErrorCode.FailedWrite, delegate
			{
				this.iStream.Commit(0U);
			});
		}

		public override bool CanRead
		{
			get
			{
				this.CheckDisposed("ComStream.get_CanRead");
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				this.CheckDisposed("ComStream.get_CanWrite");
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				this.CheckDisposed("ComStream.get_CanWrite");
				return true;
			}
		}

		private System.Runtime.InteropServices.ComTypes.STATSTG GetStat()
		{
			System.Runtime.InteropServices.ComTypes.STATSTG result = default(System.Runtime.InteropServices.ComTypes.STATSTG);
			Util.InvokeComCall(MsgStorageErrorCode.FailedRead, delegate
			{
				System.Runtime.InteropServices.ComTypes.STATSTG result;
				this.iStream.Stat(out result, 1U);
				result = result;
			});
			return result;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.iStream != null)
				{
					Marshal.ReleaseComObject(this.iStream);
					this.iStream = null;
				}
				GC.SuppressFinalize(this);
			}
			this.isDisposed = true;
		}

		protected void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException(methodName);
			}
		}

		private Interop.IStream iStream;

		private bool isDisposed;
	}
}
