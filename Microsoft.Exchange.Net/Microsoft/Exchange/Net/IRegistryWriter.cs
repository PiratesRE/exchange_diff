using System;
using Microsoft.Win32;

namespace Microsoft.Exchange.Net
{
	public interface IRegistryWriter
	{
		void SetValue(RegistryKey baseKey, string subkeyName, string valueName, object value, RegistryValueKind valueKind);

		void DeleteValue(RegistryKey baseKey, string subkeyName, string valueName);

		void CreateSubKey(RegistryKey baseKey, string subkeyName);

		void DeleteSubKeyTree(RegistryKey baseKey, string subkeyName);
	}
}
