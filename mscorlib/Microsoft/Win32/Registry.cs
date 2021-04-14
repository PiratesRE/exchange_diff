using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Win32
{
	[ComVisible(true)]
	public static class Registry
	{
		[SecurityCritical]
		private static RegistryKey GetBaseKeyFromKeyName(string keyName, out string subKeyName)
		{
			if (keyName == null)
			{
				throw new ArgumentNullException("keyName");
			}
			int num = keyName.IndexOf('\\');
			string text;
			if (num != -1)
			{
				text = keyName.Substring(0, num).ToUpper(CultureInfo.InvariantCulture);
			}
			else
			{
				text = keyName.ToUpper(CultureInfo.InvariantCulture);
			}
			uint num2 = <PrivateImplementationDetails>.ComputeStringHash(text);
			RegistryKey result;
			if (num2 <= 1097425318U)
			{
				if (num2 != 126972219U)
				{
					if (num2 != 457190004U)
					{
						if (num2 == 1097425318U)
						{
							if (text == "HKEY_CLASSES_ROOT")
							{
								result = Registry.ClassesRoot;
								goto IL_169;
							}
						}
					}
					else if (text == "HKEY_LOCAL_MACHINE")
					{
						result = Registry.LocalMachine;
						goto IL_169;
					}
				}
				else if (text == "HKEY_CURRENT_CONFIG")
				{
					result = Registry.CurrentConfig;
					goto IL_169;
				}
			}
			else if (num2 <= 1568329430U)
			{
				if (num2 != 1198714601U)
				{
					if (num2 == 1568329430U)
					{
						if (text == "HKEY_CURRENT_USER")
						{
							result = Registry.CurrentUser;
							goto IL_169;
						}
					}
				}
				else if (text == "HKEY_USERS")
				{
					result = Registry.Users;
					goto IL_169;
				}
			}
			else if (num2 != 2823865611U)
			{
				if (num2 == 3554990456U)
				{
					if (text == "HKEY_PERFORMANCE_DATA")
					{
						result = Registry.PerformanceData;
						goto IL_169;
					}
				}
			}
			else if (text == "HKEY_DYN_DATA")
			{
				result = RegistryKey.GetBaseKey(RegistryKey.HKEY_DYN_DATA);
				goto IL_169;
			}
			throw new ArgumentException(Environment.GetResourceString("Arg_RegInvalidKeyName", new object[]
			{
				"keyName"
			}));
			IL_169:
			if (num == -1 || num == keyName.Length)
			{
				subKeyName = string.Empty;
			}
			else
			{
				subKeyName = keyName.Substring(num + 1, keyName.Length - num - 1);
			}
			return result;
		}

		[SecuritySafeCritical]
		public static object GetValue(string keyName, string valueName, object defaultValue)
		{
			string name;
			RegistryKey baseKeyFromKeyName = Registry.GetBaseKeyFromKeyName(keyName, out name);
			RegistryKey registryKey = baseKeyFromKeyName.OpenSubKey(name);
			if (registryKey == null)
			{
				return null;
			}
			object value;
			try
			{
				value = registryKey.GetValue(valueName, defaultValue);
			}
			finally
			{
				registryKey.Close();
			}
			return value;
		}

		public static void SetValue(string keyName, string valueName, object value)
		{
			Registry.SetValue(keyName, valueName, value, RegistryValueKind.Unknown);
		}

		[SecuritySafeCritical]
		public static void SetValue(string keyName, string valueName, object value, RegistryValueKind valueKind)
		{
			string subkey;
			RegistryKey baseKeyFromKeyName = Registry.GetBaseKeyFromKeyName(keyName, out subkey);
			RegistryKey registryKey = baseKeyFromKeyName.CreateSubKey(subkey);
			try
			{
				registryKey.SetValue(valueName, value, valueKind);
			}
			finally
			{
				registryKey.Close();
			}
		}

		public static readonly RegistryKey CurrentUser = RegistryKey.GetBaseKey(RegistryKey.HKEY_CURRENT_USER);

		public static readonly RegistryKey LocalMachine = RegistryKey.GetBaseKey(RegistryKey.HKEY_LOCAL_MACHINE);

		public static readonly RegistryKey ClassesRoot = RegistryKey.GetBaseKey(RegistryKey.HKEY_CLASSES_ROOT);

		public static readonly RegistryKey Users = RegistryKey.GetBaseKey(RegistryKey.HKEY_USERS);

		public static readonly RegistryKey PerformanceData = RegistryKey.GetBaseKey(RegistryKey.HKEY_PERFORMANCE_DATA);

		public static readonly RegistryKey CurrentConfig = RegistryKey.GetBaseKey(RegistryKey.HKEY_CURRENT_CONFIG);

		[Obsolete("The DynData registry key only works on Win9x, which is no longer supported by the CLR.  On NT-based operating systems, use the PerformanceData registry key instead.")]
		public static readonly RegistryKey DynData = RegistryKey.GetBaseKey(RegistryKey.HKEY_DYN_DATA);
	}
}
