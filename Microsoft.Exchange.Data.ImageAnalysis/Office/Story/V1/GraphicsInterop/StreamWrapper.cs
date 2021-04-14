using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Office.Story.V1.GraphicsInterop
{
	internal class StreamWrapper : IStream
	{
		public StreamWrapper(Stream stream)
		{
			this._stream = stream;
		}

		public Stream Stream
		{
			get
			{
				return this._stream;
			}
		}

		public void Clone(out IStream ppstm)
		{
			throw new NotSupportedException();
		}

		public void Commit(int grfCommitFlags)
		{
			this._stream.Flush();
		}

		public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
		{
			if (pstm == null)
			{
				throw new ArgumentNullException("pstm");
			}
			byte[] array = new byte[cb];
			int num = this._stream.Read(array, 0, array.Length);
			if (IntPtr.Zero != pcbRead)
			{
				Marshal.WriteInt32(pcbRead, 0, num);
			}
			pstm.Write(array, num, pcbWritten);
		}

		public void LockRegion(long libOffset, long cb, int dwLockType)
		{
			throw new NotSupportedException();
		}

		public void Read(byte[] pv, int cb, IntPtr pcbRead)
		{
			int val = this._stream.Read(pv, 0, cb);
			if (IntPtr.Zero != pcbRead)
			{
				Marshal.WriteInt32(pcbRead, 0, val);
			}
		}

		public void Revert()
		{
			throw new NotSupportedException();
		}

		public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
		{
			long val = this._stream.Seek(dlibMove, (SeekOrigin)dwOrigin);
			if (IntPtr.Zero != plibNewPosition)
			{
				Marshal.WriteInt64(plibNewPosition, 0, val);
			}
		}

		public void SetSize(long libNewSize)
		{
			this._stream.SetLength(libNewSize);
		}

		public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
		{
			pstatstg = new System.Runtime.InteropServices.ComTypes.STATSTG
			{
				cbSize = this._stream.Length,
				grfLocksSupported = 0
			};
		}

		public void UnlockRegion(long libOffset, long cb, int dwLockType)
		{
			throw new NotSupportedException();
		}

		public void Write(byte[] pv, int cb, IntPtr pcbWritten)
		{
			this._stream.Write(pv, 0, cb);
			if (IntPtr.Zero != pcbWritten)
			{
				Marshal.WriteInt32(pcbWritten, 0, cb);
			}
		}

		private readonly Stream _stream;
	}
}
