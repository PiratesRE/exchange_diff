using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement.StructuredStorage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class IStreamOverStream : IStream
	{
		public IStreamOverStream(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			this.stream = stream;
		}

		public int Read(IntPtr buf, int len)
		{
			throw new NotImplementedException();
		}

		public int Write(IntPtr buf, int len)
		{
			if (buf == IntPtr.Zero)
			{
				throw new ArgumentNullException("buf");
			}
			if (len < 0)
			{
				throw new ArgumentOutOfRangeException("len", len, "len cannot be less than zero.");
			}
			byte[] array = new byte[len];
			Marshal.Copy(buf, array, 0, array.Length);
			this.stream.Write(array, 0, len);
			return len;
		}

		public long Seek(long dlibMove, int dwOrigin)
		{
			return this.stream.Seek(dlibMove, (SeekOrigin)dwOrigin);
		}

		public void SetSize(long libNewSize)
		{
			throw new NotImplementedException();
		}

		public long CopyTo(IStream pstm, long cb, long[] pcbRead)
		{
			throw new NotImplementedException();
		}

		public void Commit(STGC grfCommitFlags)
		{
			this.stream.Flush();
		}

		public void Revert()
		{
			throw new NotSupportedException();
		}

		public void LockRegion(long libOffset, long cb, int dwLockType)
		{
			throw new NotImplementedException();
		}

		public void UnlockRegion(long libOffset, long cb, int dwLockType)
		{
			throw new NotImplementedException();
		}

		public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pStatstg, STATFLAG grfStatFlag)
		{
			pStatstg = default(System.Runtime.InteropServices.ComTypes.STATSTG);
			if (grfStatFlag != STATFLAG.NoName)
			{
				throw new NotImplementedException();
			}
			pStatstg.pwcsName = null;
			pStatstg.type = 2;
			pStatstg.cbSize = this.stream.Length;
			pStatstg.atime = default(System.Runtime.InteropServices.ComTypes.FILETIME);
			pStatstg.mtime = default(System.Runtime.InteropServices.ComTypes.FILETIME);
			pStatstg.ctime = default(System.Runtime.InteropServices.ComTypes.FILETIME);
			pStatstg.clsid = Guid.Empty;
			pStatstg.grfMode = (this.stream.CanWrite ? 2 : 0);
			pStatstg.grfLocksSupported = 2;
			pStatstg.grfStateBits = 0;
			pStatstg.reserved = 0;
		}

		public IStream Clone()
		{
			throw new NotSupportedException();
		}

		private Stream stream;
	}
}
