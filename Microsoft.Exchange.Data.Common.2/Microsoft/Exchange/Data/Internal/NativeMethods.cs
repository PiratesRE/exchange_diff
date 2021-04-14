using System;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Data.Internal
{
	[ComVisible(false)]
	[SuppressUnmanagedCodeSecurity]
	internal class NativeMethods
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "CreateFileW", SetLastError = true)]
		internal static extern SafeFileHandle CreateFile([In] string filename, [In] uint accessMode, [In] uint shareMode, ref NativeMethods.SecurityAttributes securityAttributes, [In] uint creationDisposition, [In] uint flags, [In] IntPtr templateFileHandle);

		internal const uint DELETE = 65536U;

		internal const uint READ_CONTROL = 131072U;

		internal const uint WRITE_DAC = 262144U;

		internal const uint WRITE_OWNER = 524288U;

		internal const uint SYNCHRONIZE = 1048576U;

		internal const uint STANDARD_RIGHTS_REQUIRED = 983040U;

		internal const uint STANDARD_RIGHTS_READ = 131072U;

		internal const uint STANDARD_RIGHTS_WRITE = 131072U;

		internal const uint STANDARD_RIGHTS_EXECUTE = 131072U;

		internal const uint STANDARD_RIGHTS_ALL = 2031616U;

		internal const uint SPECIFIC_RIGHTS_ALL = 65535U;

		internal const uint FILE_SHARE_READ = 1U;

		internal const uint FILE_SHARE_WRITE = 2U;

		internal const uint FILE_SHARE_DELETE = 4U;

		internal const uint FILE_READ_DATA = 1U;

		internal const uint FILE_LIST_DIRECTORY = 1U;

		internal const uint FILE_WRITE_DATA = 2U;

		internal const uint FILE_ADD_FILE = 2U;

		internal const uint FILE_APPEND_DATA = 4U;

		internal const uint FILE_ADD_SUBDIRECTORY = 4U;

		internal const uint FILE_CREATE_PIPE_INSTANCE = 4U;

		internal const uint FILE_READ_EA = 8U;

		internal const uint FILE_WRITE_EA = 16U;

		internal const uint FILE_EXECUTE = 32U;

		internal const uint FILE_TRAVERSE = 32U;

		internal const uint FILE_DELETE_CHILD = 64U;

		internal const uint FILE_READ_ATTRIBUTES = 128U;

		internal const uint FILE_WRITE_ATTRIBUTES = 256U;

		internal const uint FILE_ALL_ACCESS = 2032127U;

		internal const uint FILE_GENERIC_READ = 1179785U;

		internal const uint FILE_GENERIC_WRITE = 1179926U;

		internal const uint FILE_GENERIC_EXECUTE = 1179808U;

		internal const uint CREATE_NEW = 1U;

		internal const uint CREATE_ALWAYS = 2U;

		internal const uint OPEN_EXISTING = 3U;

		internal const uint OPEN_ALWAYS = 4U;

		internal const uint TRUNCATE_EXISTING = 5U;

		internal const uint FILE_ATTRIBUTE_READONLY = 1U;

		internal const uint FILE_ATTRIBUTE_HIDDEN = 2U;

		internal const uint FILE_ATTRIBUTE_SYSTEM = 4U;

		internal const uint FILE_ATTRIBUTE_DIRECTORY = 16U;

		internal const uint FILE_ATTRIBUTE_ARCHIVE = 32U;

		internal const uint FILE_ATTRIBUTE_DEVICE = 64U;

		internal const uint FILE_ATTRIBUTE_NORMAL = 128U;

		internal const uint FILE_ATTRIBUTE_TEMPORARY = 256U;

		internal const uint FILE_ATTRIBUTE_SPARSE_FILE = 512U;

		internal const uint FILE_ATTRIBUTE_REPARSE_POuint = 1024U;

		internal const uint FILE_ATTRIBUTE_COMPRESSED = 2048U;

		internal const uint FILE_ATTRIBUTE_OFFLINE = 4096U;

		internal const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 8192U;

		internal const uint FILE_ATTRIBUTE_ENCRYPTED = 16384U;

		internal const uint FILE_FLAG_WRITE_THROUGH = 2147483648U;

		internal const uint FILE_FLAG_OVERLAPPED = 1073741824U;

		internal const uint FILE_FLAG_NO_BUFFERING = 536870912U;

		internal const uint FILE_FLAG_RANDOM_ACCESS = 268435456U;

		internal const uint FILE_FLAG_SEQUENTIAL_SCAN = 134217728U;

		internal const uint FILE_FLAG_DELETE_ON_CLOSE = 67108864U;

		internal const uint FILE_FLAG_BACKUP_SEMANTICS = 33554432U;

		internal const uint FILE_FLAG_POSIX_SEMANTICS = 16777216U;

		internal const uint FILE_FLAG_OPEN_REPARSE_POuint = 2097152U;

		internal const uint FILE_FLAG_OPEN_NO_RECALL = 1048576U;

		internal const uint FILE_FLAG_FIRST_PIPE_INSTANCE = 524288U;

		internal const int ERROR_ACCESS_DENIED = 5;

		internal const int ERROR_ALREADY_EXISTS = 183;

		internal const int ERROR_FILE_EXISTS = 80;

		internal const int MAX_PATH = 260;

		internal struct SecurityAttributes
		{
			internal SecurityAttributes(bool inheritHandle)
			{
				this.length = Marshal.SizeOf(typeof(NativeMethods.SecurityAttributes));
				this.securityDescriptor = IntPtr.Zero;
				this.inheritHandle = inheritHandle;
			}

			internal int length;

			internal IntPtr securityDescriptor;

			[MarshalAs(UnmanagedType.Bool)]
			internal bool inheritHandle;
		}
	}
}
