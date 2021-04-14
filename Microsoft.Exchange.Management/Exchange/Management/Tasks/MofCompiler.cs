using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Management.Tasks
{
	[Guid("6daf9757-2e37-11d2-aec9-00c04fb68820")]
	[ComImport]
	internal class MofCompiler : IMofCompiler
	{
		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
		public extern int CompileFile([MarshalAs(UnmanagedType.LPWStr)] string FileName, [MarshalAs(UnmanagedType.LPWStr)] string ServerAndNamespace, [MarshalAs(UnmanagedType.LPWStr)] string User, [MarshalAs(UnmanagedType.LPWStr)] string Authority, [MarshalAs(UnmanagedType.LPWStr)] string Password, int lOptionFlags, int lClassFlags, int lInstanceFlags, [In] [Out] ref WbemCompileStatusInfo pInfo);

		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
		public extern int CompileBuffer(int BuffSize, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] byte[] pBuffer, [MarshalAs(UnmanagedType.LPWStr)] string ServerAndNamespace, [MarshalAs(UnmanagedType.LPWStr)] string User, [MarshalAs(UnmanagedType.LPWStr)] string Authority, [MarshalAs(UnmanagedType.LPWStr)] string Password, int lOptionFlags, int lClassFlags, int lInstanceFlags, [In] [Out] ref WbemCompileStatusInfo pInfo);

		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
		public extern int CreateBMOF([MarshalAs(UnmanagedType.LPWStr)] string TextFileName, [MarshalAs(UnmanagedType.LPWStr)] string BMOFFileName, [MarshalAs(UnmanagedType.LPWStr)] string ServerAndNamespace, int lOptionFlags, int lClassFlags, int lInstanceFlags, [In] [Out] ref WbemCompileStatusInfo pInfo);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern MofCompiler();

		internal const int WbemSNoError = 0;

		internal const int WbemSAlreadyExists = 262145;
	}
}
