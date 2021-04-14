using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	internal class MsgStorageWriteStream : Stream
	{
		internal MsgStorageWriteStream(Stream innerStream, int addStringTerminators)
		{
			Util.ThrowOnNullArgument(innerStream, "innerStream");
			if (addStringTerminators < 0 || addStringTerminators > 2)
			{
				throw new ArgumentException("addStringTerminators must be in the range 0 - 2");
			}
			this.innerStream = innerStream;
			this.offset = 0L;
			this.length = 0L;
			this.addStringTerminators = addStringTerminators;
			this.terminatorsFound = 0;
		}

		internal void AddOnCloseNotifier(MsgStorageWriteStream.OnCloseDelegate onCloseDelegate)
		{
			this.CheckDisposed("MsgStorageWriteStream::AddOnCloseNotifier");
			this.onCloseDelegate = (MsgStorageWriteStream.OnCloseDelegate)Delegate.Combine(this.onCloseDelegate, onCloseDelegate);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed("MsgStorageWriteStream::Read");
			throw new NotSupportedException("MsgStorageWriteStream::Read");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed("MsgStorageWriteStream::Write");
			this.innerStream.Write(buffer, offset, count);
			if (this.addStringTerminators != 0)
			{
				int num = (count < this.addStringTerminators) ? count : this.addStringTerminators;
				int num2 = 0;
				while (num2 != num && buffer[count - num2 - 1] == 0)
				{
					num2++;
				}
				if (num2 < count)
				{
					this.terminatorsFound = num2;
				}
				else
				{
					this.terminatorsFound += num2;
				}
			}
			this.offset += (long)count;
			if (this.offset > this.length)
			{
				this.length = this.offset;
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.CheckDisposed("MsgStorageWriteStream::Seek");
			this.offset = this.innerStream.Seek(offset, origin);
			return this.offset;
		}

		public override long Length
		{
			get
			{
				this.CheckDisposed("MsgStorageWriteStream::get_Length");
				return this.length;
			}
		}

		public override long Position
		{
			get
			{
				this.CheckDisposed("MsgStorageWriteStream::get_Position");
				return this.offset;
			}
			set
			{
				this.CheckDisposed("MsgStorageWriteStream::set_Position");
				this.innerStream.Seek(value, SeekOrigin.Begin);
			}
		}

		public override void SetLength(long value)
		{
			this.CheckDisposed("ComStream::SetLength");
			this.innerStream.SetLength(value);
			this.length = value;
			this.offset = this.innerStream.Position;
		}

		public override void Flush()
		{
			this.CheckDisposed("ComStream::Flush");
			this.innerStream.Flush();
		}

		public override bool CanRead
		{
			get
			{
				this.CheckDisposed("ComStream::get_CanRead");
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				this.CheckDisposed("ComStream::get_CanWrite");
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				this.CheckDisposed("ComStream::get_CanSeek");
				return true;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !this.isDisposed)
			{
				if (this.innerStream != null)
				{
					Exception onCloseException = null;
					try
					{
						if (this.terminatorsFound < this.addStringTerminators)
						{
							this.Write(MsgStorageWriteStream.StringTerminators, 0, this.addStringTerminators);
						}
					}
					catch (COMException ex)
					{
						onCloseException = ex;
					}
					catch (IOException ex2)
					{
						onCloseException = ex2;
					}
					if (this.onCloseDelegate != null)
					{
						this.onCloseDelegate(this, onCloseException);
						this.onCloseDelegate = null;
					}
					this.innerStream.Dispose();
					this.innerStream = null;
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

		// Note: this type is marked as 'beforefieldinit'.
		static MsgStorageWriteStream()
		{
			byte[] stringTerminators = new byte[2];
			MsgStorageWriteStream.StringTerminators = stringTerminators;
		}

		private static readonly byte[] StringTerminators;

		private Stream innerStream;

		private int addStringTerminators;

		private int terminatorsFound;

		private long offset;

		private long length;

		private MsgStorageWriteStream.OnCloseDelegate onCloseDelegate;

		private bool isDisposed;

		internal delegate void OnCloseDelegate(MsgStorageWriteStream stream, Exception onCloseException);
	}
}
