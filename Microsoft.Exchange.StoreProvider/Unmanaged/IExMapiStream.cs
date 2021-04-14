using System;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExMapiStream : IExInterface, IDisposeTrackable, IDisposable
	{
		int Read(byte[] pv, uint cb, out uint cbRead);

		int Write(byte[] pv, int cb, out int pcbWritten);

		int Seek(long dlibMove, int dwOrigin, out long plibNewPosition);

		int SetSize(long libNewSize);

		int CopyTo(IFastStream pstm, long cb, IntPtr pcbRead, out long pcbWritten);

		int Commit(int grfCommitFlags);

		int Revert();

		int LockRegion(long libOffset, long cb, int dwLockType);

		int UnlockRegion(long libOffset, long cb, int dwLockType);

		int Stat(out STATSTG pstatstg, int grfStatFlag);

		int Clone(out IExInterface iStreamNew);
	}
}
