using System;
using System.Reflection;
using System.Security;
using Microsoft.Win32;

namespace System.Runtime.InteropServices
{
	public static class RuntimeInformation
	{
		public static string FrameworkDescription
		{
			get
			{
				if (RuntimeInformation.s_frameworkDescription == null)
				{
					AssemblyFileVersionAttribute assemblyFileVersionAttribute = (AssemblyFileVersionAttribute)typeof(object).GetTypeInfo().Assembly.GetCustomAttribute(typeof(AssemblyFileVersionAttribute));
					RuntimeInformation.s_frameworkDescription = string.Format("{0} {1}", ".NET Framework", assemblyFileVersionAttribute.Version);
				}
				return RuntimeInformation.s_frameworkDescription;
			}
		}

		public static bool IsOSPlatform(OSPlatform osPlatform)
		{
			return OSPlatform.Windows == osPlatform;
		}

		public static string OSDescription
		{
			[SecuritySafeCritical]
			get
			{
				if (RuntimeInformation.s_osDescription == null)
				{
					RuntimeInformation.s_osDescription = RuntimeInformation.RtlGetVersion();
				}
				return RuntimeInformation.s_osDescription;
			}
		}

		public static Architecture OSArchitecture
		{
			[SecuritySafeCritical]
			get
			{
				object obj = RuntimeInformation.s_osLock;
				lock (obj)
				{
					if (RuntimeInformation.s_osArch == null)
					{
						Win32Native.SYSTEM_INFO system_INFO;
						Win32Native.GetNativeSystemInfo(out system_INFO);
						RuntimeInformation.s_osArch = new Architecture?(RuntimeInformation.GetArchitecture(system_INFO.wProcessorArchitecture));
					}
				}
				return RuntimeInformation.s_osArch.Value;
			}
		}

		public static Architecture ProcessArchitecture
		{
			[SecuritySafeCritical]
			get
			{
				object obj = RuntimeInformation.s_processLock;
				lock (obj)
				{
					if (RuntimeInformation.s_processArch == null)
					{
						Win32Native.SYSTEM_INFO system_INFO = default(Win32Native.SYSTEM_INFO);
						Win32Native.GetSystemInfo(ref system_INFO);
						RuntimeInformation.s_processArch = new Architecture?(RuntimeInformation.GetArchitecture(system_INFO.wProcessorArchitecture));
					}
				}
				return RuntimeInformation.s_processArch.Value;
			}
		}

		private static Architecture GetArchitecture(ushort wProcessorArchitecture)
		{
			Architecture result = Architecture.X86;
			if (wProcessorArchitecture <= 5)
			{
				if (wProcessorArchitecture != 0)
				{
					if (wProcessorArchitecture == 5)
					{
						result = Architecture.Arm;
					}
				}
				else
				{
					result = Architecture.X86;
				}
			}
			else if (wProcessorArchitecture != 9)
			{
				if (wProcessorArchitecture == 12)
				{
					result = Architecture.Arm64;
				}
			}
			else
			{
				result = Architecture.X64;
			}
			return result;
		}

		[SecuritySafeCritical]
		private static string RtlGetVersion()
		{
			Win32Native.RTL_OSVERSIONINFOEX rtl_OSVERSIONINFOEX = default(Win32Native.RTL_OSVERSIONINFOEX);
			rtl_OSVERSIONINFOEX.dwOSVersionInfoSize = (uint)Marshal.SizeOf<Win32Native.RTL_OSVERSIONINFOEX>(rtl_OSVERSIONINFOEX);
			if (Win32Native.RtlGetVersion(out rtl_OSVERSIONINFOEX) == 0)
			{
				return string.Format("{0} {1}.{2}.{3} {4}", new object[]
				{
					"Microsoft Windows",
					rtl_OSVERSIONINFOEX.dwMajorVersion,
					rtl_OSVERSIONINFOEX.dwMinorVersion,
					rtl_OSVERSIONINFOEX.dwBuildNumber,
					rtl_OSVERSIONINFOEX.szCSDVersion
				});
			}
			return "Microsoft Windows";
		}

		private const string FrameworkName = ".NET Framework";

		private static string s_frameworkDescription;

		private static string s_osDescription = null;

		private static object s_osLock = new object();

		private static object s_processLock = new object();

		private static Architecture? s_osArch = null;

		private static Architecture? s_processArch = null;
	}
}
