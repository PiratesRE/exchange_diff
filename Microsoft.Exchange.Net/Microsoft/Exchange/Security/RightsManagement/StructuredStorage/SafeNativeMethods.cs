using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security.RightsManagement.StructuredStorage
{
	internal static class SafeNativeMethods
	{
		[DllImport("RightsManagementWrapper.dll")]
		internal static extern ZLib.ErrorCode rms_deflate([In] [Out] ref ZLib.ZStream stream, [In] int flush);

		[DllImport("RightsManagementWrapper.dll")]
		internal static extern ZLib.ErrorCode rms_deflate_init([In] [Out] ref ZLib.ZStream stream, [In] int level, [MarshalAs(UnmanagedType.LPStr)] [In] string version, [In] int zStreamStructSize);

		[DllImport("RightsManagementWrapper.dll")]
		internal static extern ZLib.ErrorCode rms_inflate([In] [Out] ref ZLib.ZStream stream, [In] int flush);

		[DllImport("RightsManagementWrapper.dll")]
		internal static extern ZLib.ErrorCode rms_inflate_init([In] [Out] ref ZLib.ZStream stream, [MarshalAs(UnmanagedType.LPStr)] [In] string version, [In] int zStreamStructSize);

		[DllImport("RightsManagementWrapper.dll")]
		internal static extern ZLib.ErrorCode rms_inflateEnd([In] [Out] ref ZLib.ZStream stream);

		[DllImport("RightsManagementWrapper.dll")]
		internal static extern ZLib.ErrorCode rms_deflateEnd([In] [Out] ref ZLib.ZStream stream);

		[DllImport("user32.dll")]
		public static extern SafeIconHandle LoadIcon(IntPtr hInstance, IntPtr lpIconName);

		[DllImport("ole32.dll")]
		public static extern SafeWin32HGlobalHandle OleMetafilePictFromIconAndLabel(SafeIconHandle iconHandle, [MarshalAs(UnmanagedType.LPWStr)] string lpszLabel, [MarshalAs(UnmanagedType.LPWStr)] string lpszSourceFile, uint iIconIndex);

		[DllImport("ole32.dll")]
		public static extern SafeWin32HGlobalHandle OleGetIconOfFile([MarshalAs(UnmanagedType.LPWStr)] string lpszLabel, [MarshalAs(UnmanagedType.Bool)] bool fUseFileAsLabel);

		[DllImport("gdi32.dll")]
		public static extern uint GetMetaFileBitsEx(IntPtr hmf, uint nSize, [Out] byte[] lpvData);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GlobalLock(SafeWin32HGlobalHandle globalHandle);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GlobalUnlock(SafeWin32HGlobalHandle globalHandle);

		[DllImport("ole32.dll")]
		public static extern int StgCreateDocfileOnILockBytes(ILockBytes plkbyt, uint grfMode, uint reserved, out IStorage ppstgOpen);

		[DllImport("ole32.dll")]
		public static extern int StgIsStorageILockBytes(ILockBytes plkbyt);

		[DllImport("ole32.dll")]
		public static extern int StgOpenStorageOnILockBytes(ILockBytes plkbyt, IStorage pStgPriority, uint grfMode, IntPtr snbEnclude, uint reserved, out IStorage ppstgOpen);

		[DllImport("RightsManagementWrapper.dll")]
		public static extern int WrapEncryptedStorage([MarshalAs(UnmanagedType.Interface)] [In] IStream stream, [In] SafeRightsManagementHandle encryptorHandle, [In] SafeRightsManagementHandle decryptorHandle, [MarshalAs(UnmanagedType.Bool)] [In] bool create, [MarshalAs(UnmanagedType.Interface)] out IStorage encryptedStorage);

		[DllImport("RightsManagementWrapper.dll")]
		public static extern int WrapStreamWithEncryptingStream([MarshalAs(UnmanagedType.Interface)] [In] IStream stream, [In] SafeRightsManagementHandle encryptorHandle, [In] SafeRightsManagementHandle decryptorHandle, [MarshalAs(UnmanagedType.Interface)] out IStream encryptedStream);

		[DllImport("RightsManagementWrapper.dll")]
		internal static extern int EnsureDRMEnvironmentInitialized([MarshalAs(UnmanagedType.U4)] [In] uint eSecurityProviderType, [MarshalAs(UnmanagedType.U4)] [In] uint eSpecification, [MarshalAs(UnmanagedType.LPWStr)] [In] string securityProvider, [MarshalAs(UnmanagedType.LPWStr)] [In] string manifestCredentials, [MarshalAs(UnmanagedType.LPWStr)] [In] string machineCredentials, out SafeRightsManagementEnvironmentHandle environmentHandle, out SafeRightsManagementHandle defaultLibrary);

		[DllImport("RightsManagementWrapper.dll")]
		internal static extern int HmfpSetRenderBits([MarshalAs(UnmanagedType.U4)] [In] uint count, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [In] byte[] bytes, out SafeIconHandle iconHandle);
	}
}
