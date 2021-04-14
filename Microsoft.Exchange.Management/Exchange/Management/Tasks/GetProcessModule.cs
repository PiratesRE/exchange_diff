using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "ProcessModule")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class GetProcessModule : Task
	{
		[Parameter(Mandatory = true)]
		public new int ProcessId
		{
			get
			{
				return (int)base.Fields["ProcessId"];
			}
			set
			{
				base.Fields["ProcessId"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.ReadModuleList(delegate(string path)
			{
				base.WriteObject(path);
			});
			TaskLogger.LogExit();
		}

		private void ReadModuleList(GetProcessModule.ModuleHandler moduleHandler)
		{
			Process process = this.TryGetProcess();
			if (process == null)
			{
				return;
			}
			using (process)
			{
				IntPtr value = this.TryGetHandle(process);
				if (!(value == IntPtr.Zero))
				{
					IntPtr intPtr = this.CreateProcessSnapshot();
					if (!(intPtr == IntPtr.Zero))
					{
						try
						{
							for (string path = this.GetFirstModule(intPtr); path != null; path = this.GetNextModule(intPtr))
							{
								moduleHandler(path);
							}
						}
						finally
						{
							GetProcessModule.CloseHandle(intPtr);
						}
					}
				}
			}
		}

		private Process TryGetProcess()
		{
			Process result;
			try
			{
				result = Process.GetProcessById(this.ProcessId);
			}
			catch (ArgumentException)
			{
				result = null;
			}
			return result;
		}

		private IntPtr TryGetHandle(Process process)
		{
			try
			{
				return process.Handle;
			}
			catch (InvalidOperationException)
			{
			}
			catch (Win32Exception ex)
			{
				if (ex.NativeErrorCode != 5)
				{
					throw;
				}
			}
			return IntPtr.Zero;
		}

		private IntPtr CreateProcessSnapshot()
		{
			IntPtr intPtr = GetProcessModule.CreateToolhelp32Snapshot(24, this.ProcessId);
			if (intPtr == IntPtr.Zero || intPtr == (IntPtr)(-1))
			{
				if (Marshal.GetLastWin32Error() != 299)
				{
					throw new Win32Exception();
				}
				intPtr = IntPtr.Zero;
			}
			return intPtr;
		}

		private string GetFirstModule(IntPtr snapshot)
		{
			GetProcessModule.MODULEENTRY32 entry = GetProcessModule.MODULEENTRY32.NewEntry();
			bool returnValue = GetProcessModule.Module32FirstW(snapshot, ref entry);
			return this.AnalyzeModuleResult(returnValue, entry);
		}

		private string GetNextModule(IntPtr snapshot)
		{
			GetProcessModule.MODULEENTRY32 entry = GetProcessModule.MODULEENTRY32.NewEntry();
			bool returnValue = GetProcessModule.Module32NextW(snapshot, ref entry);
			return this.AnalyzeModuleResult(returnValue, entry);
		}

		private string AnalyzeModuleResult(bool returnValue, GetProcessModule.MODULEENTRY32 entry)
		{
			if (returnValue)
			{
				return entry.Path;
			}
			if (Marshal.GetLastWin32Error() != 18)
			{
				throw new Win32Exception();
			}
			return null;
		}

		[DllImport("Kernel32.dll", SetLastError = true)]
		private static extern IntPtr CreateToolhelp32Snapshot(int flags, int processID);

		[DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern bool Module32FirstW(IntPtr snapshot, [In] [Out] ref GetProcessModule.MODULEENTRY32 entry);

		[DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern bool Module32NextW(IntPtr snapshot, [In] [Out] ref GetProcessModule.MODULEENTRY32 entry);

		[DllImport("Kernel32.dll", SetLastError = true)]
		private static extern bool CloseHandle(IntPtr handle);

		private const int TH32CS_SNAPMODULE = 8;

		private const int TH32CS_SNAPMODULE32 = 16;

		private const int MAX_MODULE_NAME32 = 255;

		private const int MAX_PATH = 260;

		private const int ERROR_NO_MORE_FILES = 18;

		private const int ERROR_INVALID_HANDLE = 6;

		private const int ERROR_ACCESS_DENIED = 5;

		private const int ERROR_PARTIAL_COPY = 299;

		private delegate void ModuleHandler(string path);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct MODULEENTRY32
		{
			public static GetProcessModule.MODULEENTRY32 NewEntry()
			{
				GetProcessModule.MODULEENTRY32 result = default(GetProcessModule.MODULEENTRY32);
				if (IntPtr.Size == 4)
				{
					result.dwSize = 1064;
				}
				else
				{
					result.dwSize = 1080;
				}
				return result;
			}

			public string Name
			{
				get
				{
					return this.szModule;
				}
			}

			public IntPtr Handle
			{
				get
				{
					return this.hModule;
				}
			}

			public string Path
			{
				get
				{
					return this.szExePath;
				}
			}

			private int dwSize;

			private int th32ModuleID;

			private int th32ProcessID;

			private int GlblcntUsage;

			private int ProccntUsage;

			private IntPtr modBaseAddr;

			private int modBaseSize;

			private IntPtr hModule;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			private string szModule;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			private string szExePath;
		}
	}
}
