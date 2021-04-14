using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Management.TenantMonitoring
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class NativeMethods
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern SafeLibraryHandle LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern int FormatMessage(uint dwFlags, SafeLibraryHandle lpSource, uint dwMessageId, uint dwLanguageId, out StringBuilder lpBuffer, uint nSize, string[] arguments);

		private const string Kernel32 = "kernel32.dll";

		internal const uint LOAD_LIBRARY_AS_DATAFILE = 2U;

		internal const uint LOAD_LIBRARY_AS_IMAGE_RESOURCE = 32U;

		internal const uint FORMAT_MESSAGE_ALLOCATE_BUFFER = 256U;

		internal const uint FORMAT_MESSAGE_IGNORE_INSERTS = 512U;

		internal const uint FORMAT_MESSAGE_FROM_HMODULE = 2048U;

		internal const uint FORMAT_MESSAGE_ARGUMENT_ARRAY = 8192U;

		internal const uint ERROR_RESOURCE_LANG_NOT_FOUND = 1815U;
	}
}
