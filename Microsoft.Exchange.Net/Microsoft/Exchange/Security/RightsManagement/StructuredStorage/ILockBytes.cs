using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Exchange.Security.RightsManagement.StructuredStorage
{
	[Guid("0000000A-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface ILockBytes
	{
		void ReadAt(ulong offset, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] [Out] byte[] buffer, int count, out int read);

		void WriteAt(ulong offset, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] buffer, int count, out int written);

		void Flush();

		void SetSize(ulong cb);

		void LockRegion(ulong libOffset, ulong cb, int dwLockType);

		void UnlockRegion(ulong libOffset, ulong cb, int dwLockType);

		void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, [MarshalAs(UnmanagedType.I4)] [In] STATFLAG grfStatFlag);
	}
}
