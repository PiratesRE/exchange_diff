using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Management.Tasks
{
	[Guid("6daf974e-2e37-11d2-aec9-00c04fb68820")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IMofCompiler
	{
		int CompileFile([MarshalAs(UnmanagedType.LPWStr)] string FileName, [MarshalAs(UnmanagedType.LPWStr)] string ServerAndNamespace, [MarshalAs(UnmanagedType.LPWStr)] string User, [MarshalAs(UnmanagedType.LPWStr)] string Authority, [MarshalAs(UnmanagedType.LPWStr)] string Password, int lOptionFlags, int lClassFlags, int lInstanceFlags, [In] [Out] ref WbemCompileStatusInfo pInfo);

		int CompileBuffer(int BuffSize, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [In] byte[] pBuffer, [MarshalAs(UnmanagedType.LPWStr)] string ServerAndNamespace, [MarshalAs(UnmanagedType.LPWStr)] string User, [MarshalAs(UnmanagedType.LPWStr)] string Authority, [MarshalAs(UnmanagedType.LPWStr)] string Password, int lOptionFlags, int lClassFlags, int lInstanceFlags, [In] [Out] ref WbemCompileStatusInfo pInfo);

		int CreateBMOF([MarshalAs(UnmanagedType.LPWStr)] string TextFileName, [MarshalAs(UnmanagedType.LPWStr)] string BMOFFileName, [MarshalAs(UnmanagedType.LPWStr)] string ServerAndNamespace, int lOptionFlags, int lClassFlags, int lInstanceFlags, [In] [Out] ref WbemCompileStatusInfo pInfo);
	}
}
