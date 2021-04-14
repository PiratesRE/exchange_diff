using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class NativeMethods
	{
		[DllImport("exrpc32.dll", ExactSpelling = true)]
		internal static extern int EcInitProvider(out IntPtr pPerfData);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		internal static extern int EcGetExRpcManage(out SafeExRpcManageHandle iExRpcManage);

		[DllImport("exrpc32.dll", ExactSpelling = true, PreserveSig = false)]
		internal static extern void DiagnosticCtxGetContext(out THREAD_DIAG_CONTEXT ctx);

		[DllImport("exrpc32.dll", ExactSpelling = true, PreserveSig = false)]
		internal static extern void DiagnosticCtxReleaseContext();

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		internal static extern void DiagnosticCtxLogLocation(int lid);

		[DllImport("exrpc32.dll", CharSet = CharSet.Ansi, EntryPoint = "DiagnosticCtxLogInfoEx1", ExactSpelling = true)]
		internal static extern void DiagnosticCtxLogInfo(int lid, uint value, string message);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetTLSPerformanceContext(out PerformanceContext ctx);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		internal static extern void SetForceMapiRpc([MarshalAs(UnmanagedType.Bool)] bool forceMapiRpc);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		internal static extern void AbandonNotificationsDuringShutdown([MarshalAs(UnmanagedType.Bool)] bool abandon);

		[DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern int StgOpenStorage(string pwcsName, IntPtr pstgPriority, NativeMethods.StorageMode mode, IntPtr snbExclude, int reserved, out IStorage ppstgOpen);

		[DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern int StgCreateStorageEx(string pwcsName, NativeMethods.StorageMode mode, NativeMethods.StorageFormat format, int grfAttrs, IntPtr pStgOptions, int reserved, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppstgOpen);

		internal const string EXRPC32 = "exrpc32.dll";

		internal const uint DIAG_CTX_RECORD_LEN_MASK = 4026531840U;

		internal const uint DIAG_CTX_RECORD_LEN_SHIFT = 28U;

		internal const uint DIAG_CTX_RECORD_LAYOUT_MASK = 267386880U;

		internal const uint DIAG_CTX_RECORD_LAYOUT_SHIFT = 20U;

		internal const uint DIAG_CTX_RECORD_LID_MASK = 1048575U;

		internal const uint THREAD_DIAG_CTX_BUFF_SIZE = 512U;

		[Flags]
		internal enum StorageMode : uint
		{
			Direct = 0U,
			Transacted = 65536U,
			Simple = 134217728U,
			Read = 0U,
			Write = 1U,
			ReadWrite = 2U,
			ShareDenyNone = 64U,
			ShareDenyRead = 48U,
			ShareDenyWrite = 32U,
			ShareExclusive = 16U,
			Priority = 262144U,
			DeleteOnRelease = 67108864U,
			NoScratch = 1048576U,
			Create = 4096U,
			Convert = 131072U,
			FailIfThere = 0U,
			NoSnapshot = 2097152U,
			DirectSWMR = 4194304U
		}

		internal enum StorageFormat
		{
			Storage,
			File = 3,
			Any,
			DocumentFile
		}
	}
}
