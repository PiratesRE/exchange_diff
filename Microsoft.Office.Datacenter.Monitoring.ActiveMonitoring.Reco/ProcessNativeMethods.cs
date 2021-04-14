using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public class ProcessNativeMethods
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern long GetTickCount64();

		[DllImport("dbghelp.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool MiniDumpWriteDump([In] IntPtr processHandle, [In] int processId, [In] IntPtr fileHandle, [In] uint dumpType, [In] IntPtr exceptionParam, [In] IntPtr userStreamParam, [In] IntPtr callbackParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetDiskFreeSpaceEx(string directoryName, out ulong freeBytesAvailable, out ulong totalNumberOfBytes, out ulong totalNumberOfFreeBytes);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern ToolhelpSnapshotSafeHandle CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool Process32FirstW(ToolhelpSnapshotSafeHandle handle, ref ProcessNativeMethods.PROCESSENTRY32W lppe);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool Process32NextW(ToolhelpSnapshotSafeHandle handle, ref ProcessNativeMethods.PROCESSENTRY32W lppe);

		[DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool QueryServiceStatusEx(SafeHandle serviceHandle, int infoLevel, IntPtr buffer, int bufferSize, out int bytesNeeded);

		[DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool ControlService(SafeHandle serviceHandle, int controlCode, ref ProcessNativeMethods.SERVICE_STATUS status);

		internal const int MaxPath = 260;

		internal const uint Th32CsSnapProcess = 2U;

		internal const int ErrorNoMoreFiles = 18;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct PROCESSENTRY32W
		{
			internal uint Size
			{
				set
				{
					this.dwSize = value;
				}
			}

			internal uint ProcessId
			{
				get
				{
					return this.th32ProcessID;
				}
			}

			internal uint ParentProcessId
			{
				get
				{
					return this.th32ParentProcessID;
				}
			}

			internal string ProcessName
			{
				get
				{
					return this.szExeFile;
				}
			}

			private uint dwSize;

			private uint cntUsage;

			private uint th32ProcessID;

			private IntPtr th32DefaultHeapID;

			private uint th32ModuleID;

			private uint cntThreads;

			private uint th32ParentProcessID;

			private uint pcPriClassBase;

			private uint dwFlags;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			private string szExeFile;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct SERVICE_STATUS
		{
			public int ServiceType
			{
				get
				{
					return this.serviceType;
				}
			}

			public int CurrentState
			{
				get
				{
					return this.currentState;
				}
			}

			public int ControlsAccepted
			{
				get
				{
					return this.controlsAccepted;
				}
			}

			public int Win32ExitCode
			{
				get
				{
					return this.win32ExitCode;
				}
			}

			public int ServiceSpecificExitCode
			{
				get
				{
					return this.serviceSpecificExitCode;
				}
			}

			public int CheckPoint
			{
				get
				{
					return this.checkPoint;
				}
			}

			public int WaitHint
			{
				get
				{
					return this.waitHint;
				}
			}

			private int serviceType;

			private int currentState;

			private int controlsAccepted;

			private int win32ExitCode;

			private int serviceSpecificExitCode;

			private int checkPoint;

			private int waitHint;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public class SERVICE_STATUS_PROCESS
		{
			internal int ServiceType
			{
				get
				{
					return this.serviceType;
				}
				set
				{
					this.serviceType = value;
				}
			}

			internal int CurrentState
			{
				get
				{
					return this.currentState;
				}
				set
				{
					this.currentState = value;
				}
			}

			internal int ControlsAccepted
			{
				get
				{
					return this.controlsAccepted;
				}
				set
				{
					this.controlsAccepted = value;
				}
			}

			internal int Win32ExitCode
			{
				get
				{
					return this.win32ExitCode;
				}
				set
				{
					this.win32ExitCode = value;
				}
			}

			internal int ServiceSpecificExitCode
			{
				get
				{
					return this.serviceSpecificExitCode;
				}
				set
				{
					this.serviceSpecificExitCode = value;
				}
			}

			internal int CheckPoint
			{
				get
				{
					return this.checkPoint;
				}
				set
				{
					this.checkPoint = value;
				}
			}

			internal int WaitHint
			{
				get
				{
					return this.waitHint;
				}
				set
				{
					this.waitHint = value;
				}
			}

			internal int ProcessID
			{
				get
				{
					return this.processID;
				}
				set
				{
					this.processID = value;
				}
			}

			internal int ServiceFlags
			{
				get
				{
					return this.serviceFlags;
				}
				set
				{
					this.serviceFlags = value;
				}
			}

			private int serviceType;

			private int currentState;

			private int controlsAccepted;

			private int win32ExitCode;

			private int serviceSpecificExitCode;

			private int checkPoint;

			private int waitHint;

			private int processID;

			private int serviceFlags;
		}
	}
}
