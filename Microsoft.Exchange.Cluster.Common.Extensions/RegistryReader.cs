using System;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Common.Extensions
{
	internal class RegistryReader : IRegistryReader
	{
		private RegistryReader()
		{
		}

		public static RegistryReader Instance
		{
			get
			{
				return RegistryReader.instance;
			}
		}

		public T GetValue<T>(RegistryKey baseKey, string subkeyName, string valueName, T defaultValue)
		{
			return this.registryReader.GetValue<T>(baseKey, subkeyName, valueName, defaultValue);
		}

		public string[] GetSubKeyNames(RegistryKey baseKey, string subkeyName)
		{
			return this.registryReader.GetSubKeyNames(baseKey, subkeyName);
		}

		private static RegistryReader instance = new RegistryReader();

		private IRegistryReader registryReader = RegistryReader.Instance;
	}
}
