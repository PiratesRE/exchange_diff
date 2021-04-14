using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Exchange.Data.Transport.Interop
{
	internal class InvocationContext : IProxyCallback, IStream, IDisposable
	{
		internal InvocationContext(ComProxy.AsyncCompletionCallback asyncComplete, ComArguments propertyBag, MailItem mailItem)
		{
			this.asyncComplete = asyncComplete;
			this.bag = propertyBag;
			this.mailItem = mailItem;
			if (this.mailItem != null)
			{
				this.readStream = this.mailItem.GetMimeReadStream();
				if (this.readStream == null)
				{
					throw new InvalidOperationException("Can't open read stream.");
				}
			}
		}

		void IProxyCallback.AsyncCompletion()
		{
			if (this.asyncComplete == null)
			{
				throw new InvalidOperationException();
			}
			this.asyncComplete(this.bag);
			this.asyncComplete = null;
		}

		void IProxyCallback.SetWriteStream([MarshalAs(UnmanagedType.Interface)] IStream writeStream)
		{
			throw new NotSupportedException();
		}

		void IProxyCallback.PutProperty(int id, byte[] value)
		{
			this.bag[id] = value;
		}

		void IProxyCallback.GetProperty(int id, out byte[] value)
		{
			value = this.bag[id];
		}

		void IStream.Clone(out IStream ppstm)
		{
			throw new NotSupportedException();
		}

		void IStream.Commit(int grfCommitFlags)
		{
			throw new NotSupportedException();
		}

		unsafe void IStream.CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
		{
			if (pstm == null)
			{
				throw new ArgumentException();
			}
			try
			{
				if (this.mailItem == null)
				{
					throw new InvalidOperationException();
				}
				int num = (cb > 16384L) ? 16384 : ((int)cb);
				byte[] array = new byte[num];
				int num2 = 0;
				uint num3 = 0U;
				IntPtr pcbWritten2 = new IntPtr((void*)(&num3));
				while ((long)num2 < cb)
				{
					int num4 = this.readStream.Read(array, 0, num);
					if (num4 == 0)
					{
						break;
					}
					pstm.Write(array, num4, pcbWritten2);
					if ((ulong)num3 != (ulong)((long)num4))
					{
						throw new InvalidOperationException("not all bytes were written to the stream.");
					}
					num2 += num4;
				}
				if (pcbRead != IntPtr.Zero)
				{
					Marshal.WriteInt64(pcbRead, (long)num2);
				}
				if (pcbWritten != IntPtr.Zero)
				{
					Marshal.WriteInt64(pcbWritten, (long)num2);
				}
			}
			finally
			{
				Marshal.ReleaseComObject(pstm);
			}
		}

		void IStream.LockRegion(long libOffset, long cb, int dwLockType)
		{
			throw new NotSupportedException();
		}

		void IStream.Read([Out] byte[] pv, int cb, IntPtr pcbRead)
		{
			throw new NotSupportedException();
		}

		void IStream.Revert()
		{
			throw new NotSupportedException();
		}

		void IStream.Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
		{
			throw new NotSupportedException();
		}

		void IStream.SetSize(long libNewSize)
		{
			throw new NotSupportedException();
		}

		void IStream.Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
		{
			if (this.mailItem == null)
			{
				throw new InvalidOperationException();
			}
			pstatstg = default(System.Runtime.InteropServices.ComTypes.STATSTG);
			pstatstg.type = 2;
			pstatstg.cbSize = this.readStream.Length;
			pstatstg.grfMode = 0;
		}

		void IStream.UnlockRegion(long libOffset, long cb, int dwLockType)
		{
			throw new NotSupportedException();
		}

		void IStream.Write(byte[] pv, int cb, IntPtr pcbWritten)
		{
			throw new NotSupportedException();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				if (this.readStream != null)
				{
					this.readStream.Dispose();
				}
				this.disposed = true;
			}
		}

		~InvocationContext()
		{
			this.Dispose(false);
		}

		private const long CopytoBlockSize = 16384L;

		private Stream readStream;

		private MailItem mailItem;

		private ComProxy.AsyncCompletionCallback asyncComplete;

		private ComArguments bag;

		private bool disposed;
	}
}
