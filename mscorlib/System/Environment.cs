using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public static class Environment
	{
		private static object InternalSyncObject
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			get
			{
				if (Environment.s_InternalSyncObject == null)
				{
					object value = new object();
					Interlocked.CompareExchange<object>(ref Environment.s_InternalSyncObject, value, null);
				}
				return Environment.s_InternalSyncObject;
			}
		}

		[__DynamicallyInvokable]
		public static extern int TickCount { [SecuritySafeCritical] [__DynamicallyInvokable] [MethodImpl(MethodImplOptions.InternalCall)] get; }

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern void _Exit(int exitCode);

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static void Exit(int exitCode)
		{
			Environment._Exit(exitCode);
		}

		public static extern int ExitCode { [SecuritySafeCritical] [MethodImpl(MethodImplOptions.InternalCall)] get; [SecuritySafeCritical] [MethodImpl(MethodImplOptions.InternalCall)] set; }

		[SecurityCritical]
		[__DynamicallyInvokable]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void FailFast(string message);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void FailFast(string message, uint exitCode);

		[SecurityCritical]
		[__DynamicallyInvokable]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void FailFast(string message, Exception exception);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern void TriggerCodeContractFailure(ContractFailureKind failureKind, string message, string condition, string exceptionAsString);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetIsCLRHosted();

		internal static bool IsCLRHosted
		{
			[SecuritySafeCritical]
			get
			{
				return Environment.GetIsCLRHosted();
			}
		}

		public static string CommandLine
		{
			[SecuritySafeCritical]
			get
			{
				new EnvironmentPermission(EnvironmentPermissionAccess.Read, "Path").Demand();
				string result = null;
				Environment.GetCommandLine(JitHelpers.GetStringHandleOnStack(ref result));
				return result;
			}
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetCommandLine(StringHandleOnStack retString);

		public static string CurrentDirectory
		{
			get
			{
				return Directory.GetCurrentDirectory();
			}
			set
			{
				Directory.SetCurrentDirectory(value);
			}
		}

		public static string SystemDirectory
		{
			[SecuritySafeCritical]
			get
			{
				StringBuilder stringBuilder = new StringBuilder(260);
				if (Win32Native.GetSystemDirectory(stringBuilder, 260) == 0)
				{
					__Error.WinIOError();
				}
				string text = stringBuilder.ToString();
				FileIOPermission.QuickDemand(FileIOPermissionAccess.PathDiscovery, text, false, true);
				return text;
			}
		}

		internal static string InternalWindowsDirectory
		{
			[SecurityCritical]
			get
			{
				StringBuilder stringBuilder = new StringBuilder(260);
				if (Win32Native.GetWindowsDirectory(stringBuilder, 260) == 0)
				{
					__Error.WinIOError();
				}
				return stringBuilder.ToString();
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static string ExpandEnvironmentVariables(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				return name;
			}
			if (AppDomain.IsAppXModel() && !AppDomain.IsAppXDesignMode())
			{
				return name;
			}
			int num = 100;
			StringBuilder stringBuilder = new StringBuilder(num);
			bool flag = CodeAccessSecurityEngine.QuickCheckForAllDemands();
			string[] array = name.Split(new char[]
			{
				'%'
			});
			StringBuilder stringBuilder2 = flag ? null : new StringBuilder();
			bool flag2 = false;
			int j;
			for (int i = 1; i < array.Length - 1; i++)
			{
				if (array[i].Length == 0 || flag2)
				{
					flag2 = false;
				}
				else
				{
					stringBuilder.Length = 0;
					string text = "%" + array[i] + "%";
					j = Win32Native.ExpandEnvironmentStrings(text, stringBuilder, num);
					if (j == 0)
					{
						Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
					}
					while (j > num)
					{
						num = j;
						stringBuilder.Capacity = num;
						stringBuilder.Length = 0;
						j = Win32Native.ExpandEnvironmentStrings(text, stringBuilder, num);
						if (j == 0)
						{
							Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
						}
					}
					if (!flag)
					{
						string a = stringBuilder.ToString();
						flag2 = (a != text);
						if (flag2)
						{
							stringBuilder2.Append(array[i]);
							stringBuilder2.Append(';');
						}
					}
				}
			}
			if (!flag)
			{
				new EnvironmentPermission(EnvironmentPermissionAccess.Read, stringBuilder2.ToString()).Demand();
			}
			stringBuilder.Length = 0;
			j = Win32Native.ExpandEnvironmentStrings(name, stringBuilder, num);
			if (j == 0)
			{
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
			while (j > num)
			{
				num = j;
				stringBuilder.Capacity = num;
				stringBuilder.Length = 0;
				j = Win32Native.ExpandEnvironmentStrings(name, stringBuilder, num);
				if (j == 0)
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
			}
			return stringBuilder.ToString();
		}

		public static string MachineName
		{
			[SecuritySafeCritical]
			get
			{
				new EnvironmentPermission(EnvironmentPermissionAccess.Read, "COMPUTERNAME").Demand();
				StringBuilder stringBuilder = new StringBuilder(256);
				int num = 256;
				if (Win32Native.GetComputerName(stringBuilder, ref num) == 0)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ComputerName"));
				}
				return stringBuilder.ToString();
			}
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern int GetProcessorCount();

		[__DynamicallyInvokable]
		public static int ProcessorCount
		{
			[SecuritySafeCritical]
			[__DynamicallyInvokable]
			get
			{
				return Environment.GetProcessorCount();
			}
		}

		public static int SystemPageSize
		{
			[SecuritySafeCritical]
			get
			{
				new EnvironmentPermission(PermissionState.Unrestricted).Demand();
				Win32Native.SYSTEM_INFO system_INFO = default(Win32Native.SYSTEM_INFO);
				Win32Native.GetSystemInfo(ref system_INFO);
				return system_INFO.dwPageSize;
			}
		}

		[SecuritySafeCritical]
		public static string[] GetCommandLineArgs()
		{
			new EnvironmentPermission(EnvironmentPermissionAccess.Read, "Path").Demand();
			return Environment.GetCommandLineArgsNative();
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetCommandLineArgsNative();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string nativeGetEnvironmentVariable(string variable);

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static string GetEnvironmentVariable(string variable)
		{
			if (variable == null)
			{
				throw new ArgumentNullException("variable");
			}
			if (AppDomain.IsAppXModel() && !AppDomain.IsAppXDesignMode())
			{
				return null;
			}
			new EnvironmentPermission(EnvironmentPermissionAccess.Read, variable).Demand();
			StringBuilder stringBuilder = StringBuilderCache.Acquire(128);
			int i = Win32Native.GetEnvironmentVariable(variable, stringBuilder, stringBuilder.Capacity);
			if (i == 0 && Marshal.GetLastWin32Error() == 203)
			{
				StringBuilderCache.Release(stringBuilder);
				return null;
			}
			while (i > stringBuilder.Capacity)
			{
				stringBuilder.Capacity = i;
				stringBuilder.Length = 0;
				i = Win32Native.GetEnvironmentVariable(variable, stringBuilder, stringBuilder.Capacity);
			}
			return StringBuilderCache.GetStringAndRelease(stringBuilder);
		}

		[SecuritySafeCritical]
		public static string GetEnvironmentVariable(string variable, EnvironmentVariableTarget target)
		{
			if (variable == null)
			{
				throw new ArgumentNullException("variable");
			}
			if (target == EnvironmentVariableTarget.Process)
			{
				return Environment.GetEnvironmentVariable(variable);
			}
			new EnvironmentPermission(PermissionState.Unrestricted).Demand();
			if (target == EnvironmentVariableTarget.Machine)
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Session Manager\\Environment", false))
				{
					if (registryKey == null)
					{
						return null;
					}
					return registryKey.GetValue(variable) as string;
				}
			}
			if (target == EnvironmentVariableTarget.User)
			{
				using (RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey("Environment", false))
				{
					if (registryKey2 == null)
					{
						return null;
					}
					return registryKey2.GetValue(variable) as string;
				}
			}
			throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", new object[]
			{
				(int)target
			}));
		}

		[SecurityCritical]
		private unsafe static char[] GetEnvironmentCharArray()
		{
			char[] array = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
			}
			finally
			{
				char* ptr = null;
				try
				{
					ptr = Win32Native.GetEnvironmentStrings();
					if (ptr == null)
					{
						throw new OutOfMemoryException();
					}
					char* ptr2 = ptr;
					while (*ptr2 != '\0' || ptr2[1] != '\0')
					{
						ptr2++;
					}
					int num = (int)((long)(ptr2 - ptr) + 1L);
					array = new char[num];
					try
					{
						fixed (char* ptr3 = array)
						{
							string.wstrcpy(ptr3, ptr, num);
						}
					}
					finally
					{
						char* ptr3 = null;
					}
				}
				finally
				{
					if (ptr != null)
					{
						Win32Native.FreeEnvironmentStrings(ptr);
					}
				}
			}
			return array;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static IDictionary GetEnvironmentVariables()
		{
			if (AppDomain.IsAppXModel() && !AppDomain.IsAppXDesignMode())
			{
				return new Hashtable(0);
			}
			bool flag = CodeAccessSecurityEngine.QuickCheckForAllDemands();
			StringBuilder stringBuilder = flag ? null : new StringBuilder();
			bool flag2 = true;
			char[] environmentCharArray = Environment.GetEnvironmentCharArray();
			Hashtable hashtable = new Hashtable(20);
			for (int i = 0; i < environmentCharArray.Length; i++)
			{
				int num = i;
				while (environmentCharArray[i] != '=' && environmentCharArray[i] != '\0')
				{
					i++;
				}
				if (environmentCharArray[i] != '\0')
				{
					if (i - num == 0)
					{
						while (environmentCharArray[i] != '\0')
						{
							i++;
						}
					}
					else
					{
						string text = new string(environmentCharArray, num, i - num);
						i++;
						int num2 = i;
						while (environmentCharArray[i] != '\0')
						{
							i++;
						}
						string value = new string(environmentCharArray, num2, i - num2);
						hashtable[text] = value;
						if (!flag)
						{
							if (flag2)
							{
								flag2 = false;
							}
							else
							{
								stringBuilder.Append(';');
							}
							stringBuilder.Append(text);
						}
					}
				}
			}
			if (!flag)
			{
				new EnvironmentPermission(EnvironmentPermissionAccess.Read, stringBuilder.ToString()).Demand();
			}
			return hashtable;
		}

		internal static IDictionary GetRegistryKeyNameValuePairs(RegistryKey registryKey)
		{
			Hashtable hashtable = new Hashtable(20);
			if (registryKey != null)
			{
				string[] valueNames = registryKey.GetValueNames();
				foreach (string text in valueNames)
				{
					string value = registryKey.GetValue(text, "").ToString();
					hashtable.Add(text, value);
				}
			}
			return hashtable;
		}

		[SecuritySafeCritical]
		public static IDictionary GetEnvironmentVariables(EnvironmentVariableTarget target)
		{
			if (target == EnvironmentVariableTarget.Process)
			{
				return Environment.GetEnvironmentVariables();
			}
			new EnvironmentPermission(PermissionState.Unrestricted).Demand();
			if (target == EnvironmentVariableTarget.Machine)
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Session Manager\\Environment", false))
				{
					return Environment.GetRegistryKeyNameValuePairs(registryKey);
				}
			}
			if (target == EnvironmentVariableTarget.User)
			{
				using (RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey("Environment", false))
				{
					return Environment.GetRegistryKeyNameValuePairs(registryKey2);
				}
			}
			throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", new object[]
			{
				(int)target
			}));
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static void SetEnvironmentVariable(string variable, string value)
		{
			Environment.CheckEnvironmentVariableName(variable);
			new EnvironmentPermission(PermissionState.Unrestricted).Demand();
			if (string.IsNullOrEmpty(value) || value[0] == '\0')
			{
				value = null;
			}
			else if (value.Length >= 32767)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_LongEnvVarValue"));
			}
			if (AppDomain.IsAppXModel() && !AppDomain.IsAppXDesignMode())
			{
				throw new PlatformNotSupportedException();
			}
			if (Win32Native.SetEnvironmentVariable(variable, value))
			{
				return;
			}
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error == 203)
			{
				return;
			}
			if (lastWin32Error == 206)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_LongEnvVarValue"));
			}
			throw new ArgumentException(Win32Native.GetMessage(lastWin32Error));
		}

		private static void CheckEnvironmentVariableName(string variable)
		{
			if (variable == null)
			{
				throw new ArgumentNullException("variable");
			}
			if (variable.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_StringZeroLength"), "variable");
			}
			if (variable[0] == '\0')
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_StringFirstCharIsZero"), "variable");
			}
			if (variable.Length >= 32767)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_LongEnvVarValue"));
			}
			if (variable.IndexOf('=') != -1)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_IllegalEnvVarName"));
			}
		}

		[SecuritySafeCritical]
		public static void SetEnvironmentVariable(string variable, string value, EnvironmentVariableTarget target)
		{
			if (target == EnvironmentVariableTarget.Process)
			{
				Environment.SetEnvironmentVariable(variable, value);
				return;
			}
			Environment.CheckEnvironmentVariableName(variable);
			if (variable.Length >= 1024)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_LongEnvVarName"));
			}
			new EnvironmentPermission(PermissionState.Unrestricted).Demand();
			if (string.IsNullOrEmpty(value) || value[0] == '\0')
			{
				value = null;
			}
			if (target == EnvironmentVariableTarget.Machine)
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Session Manager\\Environment", true))
				{
					if (registryKey != null)
					{
						if (value == null)
						{
							registryKey.DeleteValue(variable, false);
						}
						else
						{
							registryKey.SetValue(variable, value);
						}
					}
					goto IL_FE;
				}
			}
			if (target == EnvironmentVariableTarget.User)
			{
				if (variable.Length >= 255)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_LongEnvVarValue"));
				}
				using (RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey("Environment", true))
				{
					if (registryKey2 != null)
					{
						if (value == null)
						{
							registryKey2.DeleteValue(variable, false);
						}
						else
						{
							registryKey2.SetValue(variable, value);
						}
					}
					goto IL_FE;
				}
			}
			throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", new object[]
			{
				(int)target
			}));
			IL_FE:
			IntPtr value2 = Win32Native.SendMessageTimeout(new IntPtr(65535), 26, IntPtr.Zero, "Environment", 0U, 1000U, IntPtr.Zero);
			value2 == IntPtr.Zero;
		}

		[SecuritySafeCritical]
		public static string[] GetLogicalDrives()
		{
			new EnvironmentPermission(PermissionState.Unrestricted).Demand();
			int logicalDrives = Win32Native.GetLogicalDrives();
			if (logicalDrives == 0)
			{
				__Error.WinIOError();
			}
			uint num = (uint)logicalDrives;
			int num2 = 0;
			while (num != 0U)
			{
				if ((num & 1U) != 0U)
				{
					num2++;
				}
				num >>= 1;
			}
			string[] array = new string[num2];
			char[] array2 = new char[]
			{
				'A',
				':',
				'\\'
			};
			num = (uint)logicalDrives;
			num2 = 0;
			while (num != 0U)
			{
				if ((num & 1U) != 0U)
				{
					array[num2++] = new string(array2);
				}
				num >>= 1;
				char[] array3 = array2;
				int num3 = 0;
				array3[num3] += '\u0001';
			}
			return array;
		}

		[__DynamicallyInvokable]
		public static string NewLine
		{
			[__DynamicallyInvokable]
			get
			{
				return "\r\n";
			}
		}

		public static Version Version
		{
			get
			{
				return new Version(4, 0, 30319, 42000);
			}
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern long GetWorkingSet();

		public static long WorkingSet
		{
			[SecuritySafeCritical]
			get
			{
				new EnvironmentPermission(PermissionState.Unrestricted).Demand();
				return Environment.GetWorkingSet();
			}
		}

		public static OperatingSystem OSVersion
		{
			[SecuritySafeCritical]
			get
			{
				if (Environment.m_os == null)
				{
					Win32Native.OSVERSIONINFO osversioninfo = new Win32Native.OSVERSIONINFO();
					if (!Environment.GetVersion(osversioninfo))
					{
						throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_GetVersion"));
					}
					Win32Native.OSVERSIONINFOEX osversioninfoex = new Win32Native.OSVERSIONINFOEX();
					if (!Environment.GetVersionEx(osversioninfoex))
					{
						throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_GetVersion"));
					}
					PlatformID platform = PlatformID.Win32NT;
					Version version = new Version(osversioninfo.MajorVersion, osversioninfo.MinorVersion, osversioninfo.BuildNumber, (int)osversioninfoex.ServicePackMajor << 16 | (int)osversioninfoex.ServicePackMinor);
					Environment.m_os = new OperatingSystem(platform, version, osversioninfo.CSDVersion);
				}
				return Environment.m_os;
			}
		}

		internal static bool IsWindows8OrAbove
		{
			get
			{
				if (!Environment.s_CheckedOSWin8OrAbove)
				{
					OperatingSystem osversion = Environment.OSVersion;
					Environment.s_IsWindows8OrAbove = (osversion.Platform == PlatformID.Win32NT && ((osversion.Version.Major == 6 && osversion.Version.Minor >= 2) || osversion.Version.Major > 6));
					Environment.s_CheckedOSWin8OrAbove = true;
				}
				return Environment.s_IsWindows8OrAbove;
			}
		}

		internal static bool IsWinRTSupported
		{
			[SecuritySafeCritical]
			get
			{
				if (!Environment.s_CheckedWinRT)
				{
					Environment.s_WinRTSupported = Environment.WinRTSupported();
					Environment.s_CheckedWinRT = true;
				}
				return Environment.s_WinRTSupported;
			}
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool WinRTSupported();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetVersion(Win32Native.OSVERSIONINFO osVer);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetVersionEx(Win32Native.OSVERSIONINFOEX osVer);

		[__DynamicallyInvokable]
		public static string StackTrace
		{
			[SecuritySafeCritical]
			[__DynamicallyInvokable]
			get
			{
				new EnvironmentPermission(PermissionState.Unrestricted).Demand();
				return Environment.GetStackTrace(null, true);
			}
		}

		internal static string GetStackTrace(Exception e, bool needFileInfo)
		{
			StackTrace stackTrace;
			if (e == null)
			{
				stackTrace = new StackTrace(needFileInfo);
			}
			else
			{
				stackTrace = new StackTrace(e, needFileInfo);
			}
			return stackTrace.ToString(System.Diagnostics.StackTrace.TraceFormat.Normal);
		}

		[SecuritySafeCritical]
		private static void InitResourceHelper()
		{
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				Monitor.Enter(Environment.InternalSyncObject, ref flag);
				if (Environment.m_resHelper == null)
				{
					Environment.ResourceHelper resHelper = new Environment.ResourceHelper("mscorlib");
					Thread.MemoryBarrier();
					Environment.m_resHelper = resHelper;
				}
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(Environment.InternalSyncObject);
				}
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetResourceFromDefault(string key);

		internal static string GetResourceStringLocal(string key)
		{
			if (Environment.m_resHelper == null)
			{
				Environment.InitResourceHelper();
			}
			return Environment.m_resHelper.GetResourceString(key);
		}

		[SecuritySafeCritical]
		internal static string GetResourceString(string key)
		{
			return Environment.GetResourceFromDefault(key);
		}

		[SecuritySafeCritical]
		internal static string GetResourceString(string key, params object[] values)
		{
			string resourceString = Environment.GetResourceString(key);
			return string.Format(CultureInfo.CurrentCulture, resourceString, values);
		}

		internal static string GetRuntimeResourceString(string key)
		{
			return Environment.GetResourceString(key);
		}

		internal static string GetRuntimeResourceString(string key, params object[] values)
		{
			return Environment.GetResourceString(key, values);
		}

		public static bool Is64BitProcess
		{
			get
			{
				return true;
			}
		}

		public static bool Is64BitOperatingSystem
		{
			[SecuritySafeCritical]
			get
			{
				return true;
			}
		}

		[__DynamicallyInvokable]
		public static extern bool HasShutdownStarted { [SecuritySafeCritical] [__DynamicallyInvokable] [MethodImpl(MethodImplOptions.InternalCall)] get; }

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetCompatibilityFlag(CompatibilityFlag flag);

		public static string UserName
		{
			[SecuritySafeCritical]
			get
			{
				new EnvironmentPermission(EnvironmentPermissionAccess.Read, "UserName").Demand();
				StringBuilder stringBuilder = new StringBuilder(256);
				int capacity = stringBuilder.Capacity;
				if (Win32Native.GetUserName(stringBuilder, ref capacity))
				{
					return stringBuilder.ToString();
				}
				return string.Empty;
			}
		}

		public static bool UserInteractive
		{
			[SecuritySafeCritical]
			get
			{
				IntPtr processWindowStation = Win32Native.GetProcessWindowStation();
				if (processWindowStation != IntPtr.Zero && Environment.processWinStation != processWindowStation)
				{
					int num = 0;
					Win32Native.USEROBJECTFLAGS userobjectflags = new Win32Native.USEROBJECTFLAGS();
					if (Win32Native.GetUserObjectInformation(processWindowStation, 1, userobjectflags, Marshal.SizeOf<Win32Native.USEROBJECTFLAGS>(userobjectflags), ref num) && (userobjectflags.dwFlags & 1) == 0)
					{
						Environment.isUserNonInteractive = true;
					}
					Environment.processWinStation = processWindowStation;
				}
				return !Environment.isUserNonInteractive;
			}
		}

		[SecuritySafeCritical]
		public static string GetFolderPath(Environment.SpecialFolder folder)
		{
			if (!Enum.IsDefined(typeof(Environment.SpecialFolder), folder))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", new object[]
				{
					(int)folder
				}));
			}
			return Environment.InternalGetFolderPath(folder, Environment.SpecialFolderOption.None, false);
		}

		[SecuritySafeCritical]
		public static string GetFolderPath(Environment.SpecialFolder folder, Environment.SpecialFolderOption option)
		{
			if (!Enum.IsDefined(typeof(Environment.SpecialFolder), folder))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", new object[]
				{
					(int)folder
				}));
			}
			if (!Enum.IsDefined(typeof(Environment.SpecialFolderOption), option))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", new object[]
				{
					(int)option
				}));
			}
			return Environment.InternalGetFolderPath(folder, option, false);
		}

		[SecurityCritical]
		internal static string UnsafeGetFolderPath(Environment.SpecialFolder folder)
		{
			return Environment.InternalGetFolderPath(folder, Environment.SpecialFolderOption.None, true);
		}

		[SecurityCritical]
		private static string InternalGetFolderPath(Environment.SpecialFolder folder, Environment.SpecialFolderOption option, bool suppressSecurityChecks = false)
		{
			if (option == Environment.SpecialFolderOption.Create && !suppressSecurityChecks)
			{
				new FileIOPermission(PermissionState.None)
				{
					AllFiles = FileIOPermissionAccess.Write
				}.Demand();
			}
			StringBuilder stringBuilder = new StringBuilder(260);
			int num = Win32Native.SHGetFolderPath(IntPtr.Zero, (int)(folder | (Environment.SpecialFolder)option), IntPtr.Zero, 0, stringBuilder);
			string text;
			if (num < 0)
			{
				if (num == -2146233031)
				{
					throw new PlatformNotSupportedException();
				}
				text = string.Empty;
			}
			else
			{
				text = stringBuilder.ToString();
			}
			if (!suppressSecurityChecks)
			{
				new FileIOPermission(FileIOPermissionAccess.PathDiscovery, text).Demand();
			}
			return text;
		}

		public static string UserDomainName
		{
			[SecuritySafeCritical]
			get
			{
				new EnvironmentPermission(EnvironmentPermissionAccess.Read, "UserDomain").Demand();
				byte[] array = new byte[1024];
				int num = array.Length;
				StringBuilder stringBuilder = new StringBuilder(1024);
				uint capacity = (uint)stringBuilder.Capacity;
				byte userNameEx = Win32Native.GetUserNameEx(2, stringBuilder, ref capacity);
				if (userNameEx == 1)
				{
					string text = stringBuilder.ToString();
					int num2 = text.IndexOf('\\');
					if (num2 != -1)
					{
						return text.Substring(0, num2);
					}
				}
				capacity = (uint)stringBuilder.Capacity;
				int num3;
				if (!Win32Native.LookupAccountName(null, Environment.UserName, array, ref num, stringBuilder, ref capacity, out num3))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					throw new InvalidOperationException(Win32Native.GetMessage(lastWin32Error));
				}
				return stringBuilder.ToString();
			}
		}

		[__DynamicallyInvokable]
		public static int CurrentManagedThreadId
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			[__DynamicallyInvokable]
			get
			{
				return Thread.CurrentThread.ManagedThreadId;
			}
		}

		private const int MaxEnvVariableValueLength = 32767;

		private const int MaxSystemEnvVariableLength = 1024;

		private const int MaxUserEnvVariableLength = 255;

		private static volatile Environment.ResourceHelper m_resHelper;

		private const int MaxMachineNameLength = 256;

		private static object s_InternalSyncObject;

		private static volatile OperatingSystem m_os;

		private static volatile bool s_IsWindows8OrAbove;

		private static volatile bool s_CheckedOSWin8OrAbove;

		private static volatile bool s_WinRTSupported;

		private static volatile bool s_CheckedWinRT;

		private static volatile IntPtr processWinStation;

		private static volatile bool isUserNonInteractive;

		internal sealed class ResourceHelper
		{
			internal ResourceHelper(string name)
			{
				this.m_name = name;
			}

			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			internal string GetResourceString(string key)
			{
				if (key == null || key.Length == 0)
				{
					return "[Resource lookup failed - null or empty resource name]";
				}
				return this.GetResourceString(key, null);
			}

			[SecuritySafeCritical]
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			internal string GetResourceString(string key, CultureInfo culture)
			{
				if (key == null || key.Length == 0)
				{
					return "[Resource lookup failed - null or empty resource name]";
				}
				Environment.ResourceHelper.GetResourceStringUserData getResourceStringUserData = new Environment.ResourceHelper.GetResourceStringUserData(this, key, culture);
				RuntimeHelpers.TryCode code = new RuntimeHelpers.TryCode(this.GetResourceStringCode);
				RuntimeHelpers.CleanupCode backoutCode = new RuntimeHelpers.CleanupCode(this.GetResourceStringBackoutCode);
				RuntimeHelpers.ExecuteCodeWithGuaranteedCleanup(code, backoutCode, getResourceStringUserData);
				return getResourceStringUserData.m_retVal;
			}

			[SecuritySafeCritical]
			private void GetResourceStringCode(object userDataIn)
			{
				Environment.ResourceHelper.GetResourceStringUserData getResourceStringUserData = (Environment.ResourceHelper.GetResourceStringUserData)userDataIn;
				Environment.ResourceHelper resourceHelper = getResourceStringUserData.m_resourceHelper;
				string key = getResourceStringUserData.m_key;
				CultureInfo culture = getResourceStringUserData.m_culture;
				Monitor.Enter(resourceHelper, ref getResourceStringUserData.m_lockWasTaken);
				if (resourceHelper.currentlyLoading != null && resourceHelper.currentlyLoading.Count > 0 && resourceHelper.currentlyLoading.Contains(key))
				{
					if (resourceHelper.infinitelyRecursingCount > 0)
					{
						getResourceStringUserData.m_retVal = "[Resource lookup failed - infinite recursion or critical failure detected.]";
						return;
					}
					resourceHelper.infinitelyRecursingCount++;
					string message = "Infinite recursion during resource lookup within mscorlib.  This may be a bug in mscorlib, or potentially in certain extensibility points such as assembly resolve events or CultureInfo names.  Resource name: " + key;
					Assert.Fail("[mscorlib recursive resource lookup bug]", message, -2146232797, System.Diagnostics.StackTrace.TraceFormat.NoResourceLookup);
					Environment.FailFast(message);
				}
				if (resourceHelper.currentlyLoading == null)
				{
					resourceHelper.currentlyLoading = new Stack(4);
				}
				if (!resourceHelper.resourceManagerInited)
				{
					RuntimeHelpers.PrepareConstrainedRegions();
					try
					{
					}
					finally
					{
						RuntimeHelpers.RunClassConstructor(typeof(ResourceManager).TypeHandle);
						RuntimeHelpers.RunClassConstructor(typeof(ResourceReader).TypeHandle);
						RuntimeHelpers.RunClassConstructor(typeof(RuntimeResourceSet).TypeHandle);
						RuntimeHelpers.RunClassConstructor(typeof(BinaryReader).TypeHandle);
						resourceHelper.resourceManagerInited = true;
					}
				}
				resourceHelper.currentlyLoading.Push(key);
				if (resourceHelper.SystemResMgr == null)
				{
					resourceHelper.SystemResMgr = new ResourceManager(this.m_name, typeof(object).Assembly);
				}
				string @string = resourceHelper.SystemResMgr.GetString(key, null);
				resourceHelper.currentlyLoading.Pop();
				getResourceStringUserData.m_retVal = @string;
			}

			[PrePrepareMethod]
			private void GetResourceStringBackoutCode(object userDataIn, bool exceptionThrown)
			{
				Environment.ResourceHelper.GetResourceStringUserData getResourceStringUserData = (Environment.ResourceHelper.GetResourceStringUserData)userDataIn;
				Environment.ResourceHelper resourceHelper = getResourceStringUserData.m_resourceHelper;
				if (exceptionThrown && getResourceStringUserData.m_lockWasTaken)
				{
					resourceHelper.SystemResMgr = null;
					resourceHelper.currentlyLoading = null;
				}
				if (getResourceStringUserData.m_lockWasTaken)
				{
					Monitor.Exit(resourceHelper);
				}
			}

			private string m_name;

			private ResourceManager SystemResMgr;

			private Stack currentlyLoading;

			internal bool resourceManagerInited;

			private int infinitelyRecursingCount;

			internal class GetResourceStringUserData
			{
				public GetResourceStringUserData(Environment.ResourceHelper resourceHelper, string key, CultureInfo culture)
				{
					this.m_resourceHelper = resourceHelper;
					this.m_key = key;
					this.m_culture = culture;
				}

				public Environment.ResourceHelper m_resourceHelper;

				public string m_key;

				public CultureInfo m_culture;

				public string m_retVal;

				public bool m_lockWasTaken;
			}
		}

		public enum SpecialFolderOption
		{
			None,
			Create = 32768,
			DoNotVerify = 16384
		}

		[ComVisible(true)]
		public enum SpecialFolder
		{
			ApplicationData = 26,
			CommonApplicationData = 35,
			LocalApplicationData = 28,
			Cookies = 33,
			Desktop = 0,
			Favorites = 6,
			History = 34,
			InternetCache = 32,
			Programs = 2,
			MyComputer = 17,
			MyMusic = 13,
			MyPictures = 39,
			MyVideos = 14,
			Recent = 8,
			SendTo,
			StartMenu = 11,
			Startup = 7,
			System = 37,
			Templates = 21,
			DesktopDirectory = 16,
			Personal = 5,
			MyDocuments = 5,
			ProgramFiles = 38,
			CommonProgramFiles = 43,
			AdminTools = 48,
			CDBurning = 59,
			CommonAdminTools = 47,
			CommonDocuments = 46,
			CommonMusic = 53,
			CommonOemLinks = 58,
			CommonPictures = 54,
			CommonStartMenu = 22,
			CommonPrograms,
			CommonStartup,
			CommonDesktopDirectory,
			CommonTemplates = 45,
			CommonVideos = 55,
			Fonts = 20,
			NetworkShortcuts = 19,
			PrinterShortcuts = 27,
			UserProfile = 40,
			CommonProgramFilesX86 = 44,
			ProgramFilesX86 = 42,
			Resources = 56,
			LocalizedResources,
			SystemX86 = 41,
			Windows = 36
		}
	}
}
