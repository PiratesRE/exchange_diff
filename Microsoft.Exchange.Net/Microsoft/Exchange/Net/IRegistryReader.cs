using System;
using Microsoft.Win32;

namespace Microsoft.Exchange.Net
{
	public interface IRegistryReader
	{
		T GetValue<T>(RegistryKey baseKey, string subkeyName, string valueName, T defaultValue);

		string[] GetSubKeyNames(RegistryKey baseKey, string subkeyName);

		bool DoesValueExist(RegistryKey baseKey, string subkeyName, string valueName);
	}
}
