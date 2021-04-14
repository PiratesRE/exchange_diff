using System;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Deployment
{
	internal interface IRegistryDataProvider
	{
		object GetRegistryKeyValue(RegistryKey baseRegistryKey, string subRegKeyPath, string regKeyName);
	}
}
