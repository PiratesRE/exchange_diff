using System;
using System.Security;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	internal static class UnsafeNativeMethods
	{
		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
		[DllImport("api-ms-win-core-winrt-error-l1-1-1.dll", PreserveSig = false)]
		internal static extern IRestrictedErrorInfo GetRestrictedErrorInfo();

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
		[DllImport("api-ms-win-core-winrt-error-l1-1-1.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool RoOriginateLanguageException(int error, [MarshalAs(UnmanagedType.HString)] string message, IntPtr languageException);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
		[DllImport("api-ms-win-core-winrt-error-l1-1-1.dll", PreserveSig = false)]
		internal static extern void RoReportUnhandledError(IRestrictedErrorInfo error);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
		[DllImport("api-ms-win-core-winrt-string-l1-1-0.dll", CallingConvention = CallingConvention.StdCall)]
		internal unsafe static extern int WindowsCreateString([MarshalAs(UnmanagedType.LPWStr)] string sourceString, int length, [Out] IntPtr* hstring);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
		[DllImport("api-ms-win-core-winrt-string-l1-1-0.dll", CallingConvention = CallingConvention.StdCall)]
		internal unsafe static extern int WindowsCreateStringReference(char* sourceString, int length, [Out] HSTRING_HEADER* hstringHeader, [Out] IntPtr* hstring);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
		[DllImport("api-ms-win-core-winrt-string-l1-1-0.dll", CallingConvention = CallingConvention.StdCall)]
		internal static extern int WindowsDeleteString(IntPtr hstring);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
		[DllImport("api-ms-win-core-winrt-string-l1-1-0.dll", CallingConvention = CallingConvention.StdCall)]
		internal unsafe static extern char* WindowsGetStringRawBuffer(IntPtr hstring, [Out] uint* length);
	}
}
