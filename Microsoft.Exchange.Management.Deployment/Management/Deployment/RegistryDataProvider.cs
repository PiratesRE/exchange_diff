using System;
using Microsoft.Exchange.Management.Analysis;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class RegistryDataProvider : IRegistryDataProvider
	{
		public object GetRegistryKeyValue(RegistryKey baseRegistryKey, string subRegKeyPath, string regKeyName)
		{
			if (baseRegistryKey == null || string.IsNullOrWhiteSpace(subRegKeyPath))
			{
				throw new ArgumentNullException();
			}
			RegistryKey registryKey = baseRegistryKey.OpenSubKey(subRegKeyPath, false);
			if (registryKey == null)
			{
				throw new FailureException(Strings.RegistryKeyNotFound(baseRegistryKey.Name + "\\" + subRegKeyPath));
			}
			object result;
			if (regKeyName == null)
			{
				result = registryKey.GetSubKeyNames();
			}
			else
			{
				result = registryKey.GetValue(regKeyName);
			}
			registryKey.Dispose();
			return result;
		}
	}
}
