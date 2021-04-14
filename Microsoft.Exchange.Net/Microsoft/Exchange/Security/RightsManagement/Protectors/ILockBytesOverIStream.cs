using System;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Security.RightsManagement.StructuredStorage;

namespace Microsoft.Exchange.Security.RightsManagement.Protectors
{
	internal sealed class ILockBytesOverIStream : ILockBytes
	{
		public ILockBytesOverIStream(Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			this.stream = stream;
		}

		public unsafe void ReadAt(ulong offset, byte[] buffer, int count, out int read)
		{
			this.stream.Seek((long)offset, 0);
			fixed (byte* ptr = buffer)
			{
				read = this.stream.Read(new IntPtr((void*)ptr), count);
			}
		}

		public unsafe void WriteAt(ulong offset, byte[] buffer, int count, out int written)
		{
			this.stream.Seek((long)offset, 0);
			fixed (byte* ptr = buffer)
			{
				written = this.stream.Write(new IntPtr((void*)ptr), count);
			}
			if (written != count)
			{
				throw new InvalidOperationException("failed to write to IStream.");
			}
		}

		public void Flush()
		{
			this.stream.Commit(STGC.STGC_DEFAULT);
		}

		public void SetSize(ulong length)
		{
			this.stream.SetSize((long)length);
		}

		public void LockRegion(ulong libOffset, ulong cb, int dwLockType)
		{
		}

		public void UnlockRegion(ulong libOffset, ulong cb, int dwLockType)
		{
		}

		public void Stat(out STATSTG pstatstg, STATFLAG grfStatFlag)
		{
			this.stream.Stat(out pstatstg, grfStatFlag);
		}

		private Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream;
	}
}
