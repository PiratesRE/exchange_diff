using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[Guid("0000000B-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComImport]
	internal interface IStorage
	{
		[return: MarshalAs(UnmanagedType.Interface)]
		IStream CreateStream([MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsName, [MarshalAs(UnmanagedType.U4)] [In] int grfMode, [MarshalAs(UnmanagedType.U4)] [In] int reserved1, [MarshalAs(UnmanagedType.U4)] [In] int reserved2);

		[return: MarshalAs(UnmanagedType.Interface)]
		IStream OpenStream([MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsName, IntPtr reserved1, [MarshalAs(UnmanagedType.U4)] [In] int grfMode, [MarshalAs(UnmanagedType.U4)] [In] int reserved2);

		[return: MarshalAs(UnmanagedType.Interface)]
		IStorage CreateStorage([MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsName, [MarshalAs(UnmanagedType.U4)] [In] int grfMode, [MarshalAs(UnmanagedType.U4)] [In] int reserved1, [MarshalAs(UnmanagedType.U4)] [In] int reserved2);

		[return: MarshalAs(UnmanagedType.Interface)]
		IStorage OpenStorage([MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsName, IntPtr pstgPriority, [MarshalAs(UnmanagedType.U4)] [In] int grfMode, IntPtr snbExclude, [MarshalAs(UnmanagedType.U4)] [In] int reserved);

		void CopyTo(int ciidExclude, [MarshalAs(UnmanagedType.LPArray)] [In] Guid[] pIIDExclude, IntPtr snbExclude, [MarshalAs(UnmanagedType.Interface)] [In] IStorage stgDest);

		void MoveElementTo([MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsName, [MarshalAs(UnmanagedType.Interface)] [In] IStorage stgDest, [MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsNewName, [MarshalAs(UnmanagedType.U4)] [In] int grfFlags);

		void Commit(int grfCommitFlags);

		void Revert();

		void EnumElements([MarshalAs(UnmanagedType.U4)] [In] int reserved1, IntPtr reserved2, [MarshalAs(UnmanagedType.U4)] [In] int reserved3, [MarshalAs(UnmanagedType.Interface)] out object ppVal);

		void DestroyElement([MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsName);

		void RenameElement([MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsOldName, [MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsNewName);

		void SetElementTimes([MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsName, [In] System.Runtime.InteropServices.ComTypes.FILETIME pctime, [In] System.Runtime.InteropServices.ComTypes.FILETIME patime, [In] System.Runtime.InteropServices.ComTypes.FILETIME pmtime);

		void SetClass([In] ref Guid clsid);

		void SetStateBits(int grfStateBits, int grfMask);

		void Stat([Out] System.Runtime.InteropServices.ComTypes.STATSTG pStatStg, int grfStatFlag);
	}
}
