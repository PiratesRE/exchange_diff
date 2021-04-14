using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Cluster.Shared.MountPoint;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal class NativeMethods
	{
		[DllImport("shlwapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "PathMatchSpecW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool PathMatchSpec([In] string fileName, [In] string pattern);

		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern uint FormatMessage(uint dwFlags, ModuleHandle lpSource, uint dwMessageId, uint dwLanguageId, ref IntPtr bufferPointer, uint bufferSize, IntPtr arguments);

		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern ModuleHandle LoadLibraryEx(string libFileName, IntPtr hFile, uint dwFlags);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("kernel32", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool FreeLibrary(IntPtr hmodule);

		[DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern SafeFileHandle CreateFile(string fileName, FileAccess fileAccess, FileShare fileShare, IntPtr securityAttributes, FileMode creationDisposition, FileFlags flags, IntPtr template);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetFilePointerEx(SafeFileHandle handle, long distanceToMove, out long newFilePointer, uint moveMethod);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetEndOfFile(SafeFileHandle handle);

		[DllImport("kernel32", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, out int lpBytesReturned, IntPtr lpOverlapped);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetFileAttributesExW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetFileAttributesEx([In] string fileName, [In] NativeMethods.GET_FILEEX_INFO_LEVELS fInfoLevelId, out NativeMethods.WIN32_FILE_ATTRIBUTE_DATA fileInformation);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DeviceIoControl([In] SafeFileHandle hDevice, [In] int dwIoControlCode, [In] IntPtr lpInBuffer, [In] int nInBufferSize, [Out] IntPtr lpOutBuffer, [In] int nOutBufferSize, out int lpBytesReturned, [In] IntPtr lpOverlapped);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern void SetLastError([In] int errorCode);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseHandle([In] IntPtr handle);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool FindClose([In] SafeFileHandle handle);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool QueryPerformanceFrequency(out long freq);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool QueryPerformanceCounter(out long count);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern long GetTickCount64();

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetVolumePathNameW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetVolumePathName([In] string fileName, [Out] StringBuilder volumePathName, [In] uint bufferLength);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetVolumeNameForVolumeMountPointW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetVolumeNameForVolumeMountPoint([In] string volumeMountPoint, [Out] StringBuilder volumeName, [In] uint bufferLength);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetVolumePathNamesForVolumeNameW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetVolumePathNamesForVolumeName([MarshalAs(UnmanagedType.LPWStr)] [In] string volumeName, [MarshalAs(UnmanagedType.LPWStr)] [Out] string volumePathNames, [In] uint bufferLen, [In] [Out] ref uint requiredBufferLen);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "FindFirstVolumeW", SetLastError = true)]
		internal static extern SafeVolumeFindHandle FindFirstVolume([Out] StringBuilder volumeName, [In] uint bufferLength);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "FindNextVolumeW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool FindNextVolume([In] SafeVolumeFindHandle hFindVolume, [Out] StringBuilder volumeName, [In] uint bufferLength);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "FindFirstVolumeMountPointW", SetLastError = true)]
		internal static extern SafeVolumeMountPointFindHandle FindFirstVolumeMountPoint([In] string volumeName, [Out] StringBuilder volumeMountPoint, [In] uint bufferLength);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "FindNextVolumeMountPointW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool FindNextVolumeMountPoint([In] SafeVolumeMountPointFindHandle hFindVolumeMountPoint, [Out] StringBuilder volumeMountPoint, [In] uint bufferLength);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "DeleteVolumeMountPointW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DeleteVolumeMountPoint([In] string volumeMountPoint);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "SetVolumeMountPointW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetVolumeMountPoint([In] string volumeMountPoint, [In] string volumeName);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "SetVolumeLabelW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetVolumeLabel([In] string volumeMountPoint, [In] string volumeLabel);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetDriveTypeW", SetLastError = true)]
		internal static extern DriveType GetDriveType([In] string rootPathName);

		[DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool ControlService(SafeHandle serviceHandle, int controlCode, ref NativeMethods.SERVICE_STATUS status);

		[DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr OpenService(IntPtr databaseHandle, string serviceName, int access);

		[DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr RegisterServiceCtrlHandler(string serviceName, Delegate callback);

		[DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr RegisterServiceCtrlHandlerEx(string serviceName, Delegate callback, IntPtr userData);

		[DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public unsafe static extern bool SetServiceStatus(IntPtr serviceStatusHandle, NativeMethods.SERVICE_STATUS* status);

		[DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool StartServiceCtrlDispatcher(IntPtr entry);

		[DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr CreateService(IntPtr databaseHandle, string serviceName, string displayName, int access, int serviceType, int startType, int errorControl, string binaryPath, string loadOrderGroup, IntPtr pTagId, string dependencies, string servicesStartName, string password);

		[DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool DeleteService(IntPtr serviceHandle);

		[DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool QueryServiceStatusEx(SafeHandle serviceHandle, int infoLevel, IntPtr buffer, int bufferSize, out int bytesNeeded);

		internal const uint FILE_BEGIN = 0U;

		internal const uint FILE_CURRENT = 1U;

		internal const uint FILE_END = 2U;

		internal const uint LOAD_LIBRARY_AS_DATAFILE = 2U;

		internal const uint FORMAT_MESSAGE_ALLOCATE_BUFFER = 256U;

		internal const uint FORMAT_MESSAGE_IGNORE_INSERTS = 512U;

		internal const uint FORMAT_MESSAGE_FROM_HMODULE = 2048U;

		internal const uint FORMAT_MESSAGE_ARGUMENT_ARRAY = 8192U;

		internal const string KERNEL32 = "kernel32.dll";

		internal const string SHLWAPI = "shlwapi.dll";

		internal const int ERROR_SUCCESS = 0;

		internal const int ERROR_FILE_NOT_FOUND = 2;

		internal const int ERROR_PATH_NOT_FOUND = 3;

		internal const int ERROR_ACCESS_DENIED = 5;

		internal const int ERROR_NO_MORE_FILES = 18;

		internal const int ERROR_SHARING_VIOLATION = 32;

		internal const int ERROR_HANDLE_EOF = 38;

		internal const int ERROR_MORE_DATA = 234;

		internal const int ERROR_SHUTDOWN_IN_PROGRESS = 1115;

		internal const int ERROR_RECOVERY_NOT_NEEDED = 6821;

		internal const int ERROR_SERVICE_DOES_NOT_EXIST = 1060;

		internal const int ERROR_NOT_A_REPARSE_POINT = 4390;

		internal const int FILE_FLAG_BACKUP_SEMANTICS = 33554432;

		private const int METHOD_BUFFERED = 0;

		private const int FILE_WRITE_DATA = 2;

		private const int FILE_DEVICE_FILE_SYSTEM = 9;

		private const int ROLLFORWARD_REDO_FUNCTION = 84;

		private const int ROLLFORWARD_UNDO_FUNCTION = 85;

		private const int START_RM_FUNCTION = 86;

		private const int SHUTDOWN_RM_FUNCTION = 87;

		private const int CREATE_SECONDARY_RM_FUNCTION = 90;

		internal const int MAX_VOLUME_GUID_LENGTH = 50;

		internal const int MAX_PATH = 260;

		public const int START_TYPE_AUTO = 2;

		public const int START_TYPE_BOOT = 0;

		public const int START_TYPE_DEMAND = 3;

		public const int START_TYPE_DISABLED = 4;

		public const int START_TYPE_SYSTEM = 1;

		public const int SERVICE_ACTIVE = 1;

		public const int SERVICE_INACTIVE = 2;

		public const int SERVICE_STATE_ALL = 3;

		public const int STATE_CONTINUE_PENDING = 5;

		public const int STATE_PAUSED = 7;

		public const int STATE_PAUSE_PENDING = 6;

		public const int STATE_RUNNING = 4;

		public const int STATE_START_PENDING = 2;

		public const int STATE_STOPPED = 1;

		public const int STATE_STOP_PENDING = 3;

		public const int STATUS_ACTIVE = 1;

		public const int STATUS_INACTIVE = 2;

		public const int STATUS_ALL = 3;

		public const int STATUS_OBJECT_NAME_NOT_FOUND = -1073741772;

		public const int NO_ERROR = 0;

		public const int ACCEPT_NETBINDCHANGE = 16;

		public const int ACCEPT_PAUSE_CONTINUE = 2;

		public const int ACCEPT_PARAMCHANGE = 8;

		public const int ACCEPT_POWEREVENT = 64;

		public const int ACCEPT_SHUTDOWN = 4;

		public const int ACCEPT_STOP = 1;

		public const int ACCEPT_SESSIONCHANGE = 128;

		public const int ACCEPT_PRESHUTDOWN = 256;

		public const int CONTROL_CONTINUE = 3;

		public const int CONTROL_INTERROGATE = 4;

		public const int CONTROL_NETBINDADD = 7;

		public const int CONTROL_NETBINDDISABLE = 10;

		public const int CONTROL_NETBINDENABLE = 9;

		public const int CONTROL_NETBINDREMOVE = 8;

		public const int CONTROL_PARAMCHANGE = 6;

		public const int CONTROL_PAUSE = 2;

		public const int CONTROL_POWEREVENT = 13;

		public const int CONTROL_SHUTDOWN = 5;

		public const int CONTROL_STOP = 1;

		public const int CONTROL_DEVICEEVENT = 11;

		public const int CONTROL_SESSIONCHANGE = 14;

		public const int CONTROL_PRESHUTDOWN = 15;

		public const int SERVICE_CONFIG_DESCRIPTION = 1;

		public const int SERVICE_CONFIG_FAILURE_ACTIONS = 2;

		public const int SERVICE_TYPE_WIN32_OWN_PROCESS = 16;

		public const int SERVICE_TYPE_WIN32_SHARE_PROCESS = 32;

		public const int SC_STATUS_PROCESS_INFO = 0;

		[Flags]
		internal enum CopyFileFlags : uint
		{
			COPY_FILE_FAIL_IF_EXISTS = 1U,
			COPY_FILE_RESTARTABLE = 2U,
			COPY_FILE_OPEN_SOURCE_FOR_WRITE = 4U,
			COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 8U,
			COPY_FILE_COPY_SYMLINK = 2048U
		}

		[Flags]
		internal enum MoveFileFlags : uint
		{
			MOVEFILE_REPLACE_EXISTING = 1U,
			MOVEFILE_COPY_ALLOWED = 2U,
			MOVEFILE_DELAY_UNTIL_REBOOT = 4U,
			MOVEFILE_WRITE_THROUGH = 8U,
			MOVEFILE_CREATE_HARDLINK = 16U,
			MOVEFILE_FAIL_IF_NOT_TRACKABLE = 32U
		}

		internal enum FINDEX_INFO_LEVELS
		{
			FindExInfoStandard,
			FindExInfoMaxInfoLevel
		}

		internal enum FINDEX_SEARCH_OPS
		{
			FindExSearchNameMatch,
			FindExSearchLimitToDirectories,
			FindExSearchLimitToDevices,
			FindExSearchMaxSearchOp
		}

		internal struct FILETIME
		{
			public uint DateTimeLow;

			public uint DateTimeHigh;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		internal struct WIN32_FIND_DATA
		{
			public uint dwFileAttributes;

			public NativeMethods.FILETIME ftCreationTime;

			public NativeMethods.FILETIME ftLastAccessTime;

			public NativeMethods.FILETIME ftLastWriteTime;

			public uint nFileSizeHigh;

			public uint nFileSizeLow;

			public uint dwReserved0;

			public uint dwReserved1;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string cFileName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
			public string cAlternateFileName;
		}

		[BestFitMapping(false)]
		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct WIN32_FILE_ATTRIBUTE_DATA
		{
			internal FileAttributes FileAttributes;

			internal NativeMethods.FILETIME CreationTime;

			internal NativeMethods.FILETIME LastAccessTime;

			internal NativeMethods.FILETIME LastWriteTime;

			public uint FileSizeHigh;

			public uint FileSizeLow;
		}

		public enum GET_FILEEX_INFO_LEVELS
		{
			GetFileExInfoStandard,
			GetFileExMaxInfoLevel
		}

		internal enum ReparseTags : uint
		{
			MountPoint = 2684354563U,
			SymbolicLink = 2684354572U
		}

		public struct SERVICE_STATUS
		{
			public int serviceType;

			public int currentState;

			public int controlsAccepted;

			public int win32ExitCode;

			public int serviceSpecificExitCode;

			public int checkPoint;

			public int waitHint;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public class SERVICE_STATUS_PROCESS
		{
			public int serviceType;

			public int currentState;

			public int controlsAccepted;

			public int win32ExitCode;

			public int serviceSpecificExitCode;

			public int checkPoint;

			public int waitHint;

			public int processID;

			public int serviceFlags;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public class ENUM_SERVICE_STATUS
		{
			public string serviceName;

			public string displayName;

			public int serviceType;

			public int currentState;

			public int controlsAccepted;

			public int win32ExitCode;

			public int serviceSpecificExitCode;

			public int checkPoint;

			public int waitHint;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class SERVICE_TABLE_ENTRY
		{
			public IntPtr name;

			public Delegate callback;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public class WTSSESSION_NOTIFICATION
		{
			public int size;

			public int sessionId;
		}

		public delegate void ServiceMainCallback(int argCount, IntPtr argPointer);

		public delegate void ServiceControlCallback(int control);

		public delegate int ServiceControlCallbackEx(int control, int eventType, IntPtr eventData, IntPtr eventContext);
	}
}
