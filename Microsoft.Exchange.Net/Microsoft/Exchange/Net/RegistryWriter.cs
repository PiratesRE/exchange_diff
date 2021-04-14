using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.Net
{
	public class RegistryWriter : IRegistryWriter
	{
		internal RegistryWriter()
		{
		}

		public static IRegistryWriter Instance
		{
			get
			{
				return RegistryWriter.hookableInstance.Value;
			}
		}

		public void SetValue(RegistryKey baseKey, string subkeyName, string valueName, object value, RegistryValueKind valueKind)
		{
			if (baseKey == null)
			{
				throw new ArgumentNullException("baseKey");
			}
			using (RegistryKey registryKey = baseKey.OpenSubKey(subkeyName, true))
			{
				if (registryKey != null)
				{
					registryKey.SetValue(valueName, value, valueKind);
				}
			}
		}

		public void DeleteValue(RegistryKey baseKey, string subkeyName, string valueName)
		{
			if (baseKey == null)
			{
				throw new ArgumentNullException("baseKey");
			}
			using (RegistryKey registryKey = baseKey.OpenSubKey(subkeyName, true))
			{
				if (registryKey != null)
				{
					registryKey.DeleteValue(valueName, false);
				}
			}
		}

		public void CreateSubKey(RegistryKey baseKey, string subkeyName)
		{
			if (baseKey == null)
			{
				throw new ArgumentNullException("baseKey");
			}
			if (string.IsNullOrEmpty(subkeyName))
			{
				throw new ArgumentNullException("subKeyName");
			}
			using (RegistryKey registryKey = baseKey.CreateSubKey(subkeyName))
			{
				if (registryKey == null)
				{
					throw new ArgumentException(string.Empty, "subkeyName");
				}
			}
		}

		public void DeleteSubKeyTree(RegistryKey baseKey, string subkeyName)
		{
			if (baseKey == null)
			{
				throw new ArgumentNullException("baseKey");
			}
			if (string.IsNullOrEmpty(subkeyName))
			{
				throw new ArgumentNullException("subKeyName");
			}
			baseKey.DeleteSubKeyTree(subkeyName);
		}

		internal static IDisposable SetTestHook(IRegistryWriter testHook)
		{
			return RegistryWriter.hookableInstance.SetTestHook(testHook);
		}

		private static Hookable<IRegistryWriter> hookableInstance = Hookable<IRegistryWriter>.Create(false, new RegistryWriter());
	}
}
