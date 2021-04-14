using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Policy;

namespace System.Security.Util
{
	internal static class Config
	{
		[SecurityCritical]
		private static void GetFileLocales()
		{
			if (Config.m_machineConfig == null)
			{
				string machineConfig = null;
				Config.GetMachineDirectory(JitHelpers.GetStringHandleOnStack(ref machineConfig));
				Config.m_machineConfig = machineConfig;
			}
			if (Config.m_userConfig == null)
			{
				string userConfig = null;
				Config.GetUserDirectory(JitHelpers.GetStringHandleOnStack(ref userConfig));
				Config.m_userConfig = userConfig;
			}
		}

		internal static string MachineDirectory
		{
			[SecurityCritical]
			get
			{
				Config.GetFileLocales();
				return Config.m_machineConfig;
			}
		}

		internal static string UserDirectory
		{
			[SecurityCritical]
			get
			{
				Config.GetFileLocales();
				return Config.m_userConfig;
			}
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern int SaveDataByte(string path, [In] byte[] data, int length);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern bool RecoverData(ConfigId id);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern void SetQuickCache(ConfigId id, QuickCacheEntryType quickCacheFlags);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern bool GetCacheEntry(ConfigId id, int numKey, [In] byte[] key, int keyLength, ObjectHandleOnStack retData);

		[SecurityCritical]
		internal static bool GetCacheEntry(ConfigId id, int numKey, byte[] key, out byte[] data)
		{
			byte[] array = null;
			bool cacheEntry = Config.GetCacheEntry(id, numKey, key, key.Length, JitHelpers.GetObjectHandleOnStack<byte[]>(ref array));
			data = array;
			return cacheEntry;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void AddCacheEntry(ConfigId id, int numKey, [In] byte[] key, int keyLength, byte[] data, int dataLength);

		[SecurityCritical]
		internal static void AddCacheEntry(ConfigId id, int numKey, byte[] key, byte[] data)
		{
			Config.AddCacheEntry(id, numKey, key, key.Length, data, data.Length);
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern void ResetCacheData(ConfigId id);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetMachineDirectory(StringHandleOnStack retDirectory);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetUserDirectory(StringHandleOnStack retDirectory);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern bool WriteToEventLog(string message);

		private static volatile string m_machineConfig;

		private static volatile string m_userConfig;
	}
}
