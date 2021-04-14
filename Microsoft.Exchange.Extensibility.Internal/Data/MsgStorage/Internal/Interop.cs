using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	internal static class Interop
	{
		[DllImport("ole32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int StgOpenStorageEx([MarshalAs(UnmanagedType.LPWStr)] [In] string fileName, [In] uint grfMode, [In] int stgfmt, [In] uint grfAttrs, [In] IntPtr stgOptions, [In] IntPtr reserved2, [In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object result);

		[DllImport("ole32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int StgCreateStorageEx([MarshalAs(UnmanagedType.LPWStr)] [In] string fileName, [In] uint grfMode, [In] int stgfmt, [In] uint grfAttrs, [In] IntPtr stgOptions, [In] IntPtr reserved2, [In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object result);

		[DllImport("ole32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int StgOpenStorageOnILockBytes([MarshalAs(UnmanagedType.Interface)] [In] Interop.ILockBytes lockBytes, [In] IntPtr pStgPriority, [In] uint grfMode, [In] IntPtr snbExclude, [In] uint reserved, [MarshalAs(UnmanagedType.Interface)] out Interop.IStorage newStorage);

		[DllImport("ole32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int StgCreateDocfileOnILockBytes([MarshalAs(UnmanagedType.Interface)] [In] Interop.ILockBytes lockBytes, [In] uint grfMode, [In] int reserved, [MarshalAs(UnmanagedType.Interface)] out Interop.IStorage newStorage);

		public static Guid IIDIStorage = new Guid("0000000B-0000-0000-C000-000000000046");

		[Guid("0000000C-0000-0000-C000-000000000046")]
		[SuppressUnmanagedCodeSecurity]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[ComImport]
		public interface IStream
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			unsafe void Read([In] byte* buffer, [In] int bufferSize, out int bytesRead);

			[MethodImpl(MethodImplOptions.InternalCall)]
			unsafe void Write([In] byte* buffer, [In] int bufferSize, out int bytesWritten);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void Seek([In] long offset, [In] int origin, out long newPosition);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void SetSize([In] long newSize);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void CopyTo([MarshalAs(UnmanagedType.Interface)] [In] Interop.IStream pstm, [In] long bytesToCopy, out long bytesRead, out long pcbWritten);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void Commit([In] uint commitFlags);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void Revert();

			[MethodImpl(MethodImplOptions.InternalCall)]
			void LockRegion([In] long offset, [In] long size, [In] int lockType);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void UnlockRegion([In] ulong offset, [In] ulong size, [In] uint lockType);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG statStg, [In] uint statFlag);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void Clone([MarshalAs(UnmanagedType.Interface)] out Interop.IStream newCopy);
		}

		[Guid("0000000D-0000-0000-C000-000000000046")]
		[SuppressUnmanagedCodeSecurity]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[ComImport]
		public interface IEnumStatStg
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			void Next([In] int celt, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [Out] System.Runtime.InteropServices.ComTypes.STATSTG[] rgelt, out int pceltFetched);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void Skip([In] int celt);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void Reset();

			[MethodImpl(MethodImplOptions.InternalCall)]
			void Clone([MarshalAs(UnmanagedType.Interface)] out Interop.IEnumStatStg ppenum);
		}

		[SuppressUnmanagedCodeSecurity]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[Guid("0000000B-0000-0000-C000-000000000046")]
		[ComImport]
		public interface IStorage
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			void CreateStream([MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsName, [In] uint grfMode, [In] uint reserved1, [In] uint reserved2, [MarshalAs(UnmanagedType.Interface)] out Interop.IStream ppstm);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void OpenStream([MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsName, [In] IntPtr reserved1, [In] uint grfMode, [In] uint reserved2, [MarshalAs(UnmanagedType.Interface)] out Interop.IStream ppstm);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void CreateStorage([MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsName, [In] uint grfMode, [In] uint reserved1, [In] uint reserved2, [MarshalAs(UnmanagedType.Interface)] out Interop.IStorage ppstg);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void OpenStorage([MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsName, [MarshalAs(UnmanagedType.Interface)] [In] Interop.IStorage pstgPriority, [In] uint grfMode, [In] IntPtr snbExclude, [In] uint reserved, [MarshalAs(UnmanagedType.Interface)] out Interop.IStorage ppstg);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void CopyTo([In] uint ciidExclude, [MarshalAs(UnmanagedType.LPArray)] [In] Guid[] rgiidExclude, [In] IntPtr snbExclude, [MarshalAs(UnmanagedType.Interface)] [In] Interop.IStorage pstgDest);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void MoveElementTo([MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsName, [MarshalAs(UnmanagedType.Interface)] [In] Interop.IStorage pstgDest, [MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsNewName, [In] uint grfFlags);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void Commit([In] uint grfCommitFlags);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void Revert();

			[MethodImpl(MethodImplOptions.InternalCall)]
			void EnumElements([In] uint reserved1, [In] IntPtr reserved2, [In] uint reserved3, [MarshalAs(UnmanagedType.Interface)] out Interop.IEnumStatStg ppenum);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void DestroyElement([MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsName);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void RenameElement([MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsOldName, [MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsNewName);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void SetElementTimes([MarshalAs(UnmanagedType.LPWStr)] [In] string pwcsName, [In] System.Runtime.InteropServices.ComTypes.FILETIME pctime, [In] System.Runtime.InteropServices.ComTypes.FILETIME patime, [In] System.Runtime.InteropServices.ComTypes.FILETIME pmtime);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void SetClass(ref Guid clsid);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void SetStateBits([In] uint grfStateBits, [In] uint grfMask);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, [In] uint grfStatFlag);
		}

		[Guid("0000000A-0000-0000-C000-000000000046")]
		[SuppressUnmanagedCodeSecurity]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[ComImport]
		public interface ILockBytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			void ReadAt([In] long offset, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] [Out] byte[] buffer, [In] int bufferSize, out int bytesRead);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void WriteAt([In] long offset, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] [In] byte[] buffer, [In] int bufferSize, out int pcbWritten);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void Flush();

			[MethodImpl(MethodImplOptions.InternalCall)]
			void SetSize([In] long newSize);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void LockRegion([In] long offset, [In] long length, [In] uint lockType);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void UnlockRegion([In] long offset, [In] long length, [In] int lockType);

			[MethodImpl(MethodImplOptions.InternalCall)]
			void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, [In] uint statFlag);
		}

		[Flags]
		internal enum StorageOpenMode : uint
		{
			Read = 0U,
			Write = 1U,
			ReadWrite = 2U,
			ShareExclusive = 16U,
			ShareDenyWrite = 32U,
			ShareDenyRead = 48U,
			ShareDenyNone = 64U,
			Priority = 262144U,
			Create = 4096U,
			Convert = 131072U,
			FailIfThere = 0U,
			Direct = 0U,
			Transacted = 65536U,
			NoScratch = 1048576U,
			NoSnapshot = 2097152U,
			Simple = 134217728U,
			DirectSWMR = 4194304U,
			DeleteOnRelease = 67108864U
		}

		internal enum STGTY
		{
			Storage = 1,
			Stream,
			LockBytes,
			Property
		}

		internal enum MoveCopyMode
		{
			Move,
			Copy
		}

		internal enum StatFlags
		{
			Default,
			NoName
		}

		internal enum StorageFormat
		{
			Storage,
			File = 3,
			Any,
			DocFile
		}
	}
}
