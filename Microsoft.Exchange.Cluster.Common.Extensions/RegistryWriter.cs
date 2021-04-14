using System;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Common.Extensions
{
	internal class RegistryWriter : IRegistryWriter
	{
		private RegistryWriter()
		{
		}

		public static RegistryWriter Instance
		{
			get
			{
				return RegistryWriter.instance;
			}
		}

		public void SetValue(RegistryKey baseKey, string subkeyName, string valueName, object value, RegistryValueKind valueKind)
		{
			this.registryWriter.SetValue(baseKey, subkeyName, valueName, value, valueKind);
		}

		public void SetValue(RegistryKey baseKey, string subkeyName, string valueName, object value)
		{
			this.SetValue(baseKey, subkeyName, valueName, value, RegistryValueKind.String);
		}

		public void DeleteValue(RegistryKey baseKey, string subkeyName, string valueName)
		{
			this.registryWriter.DeleteValue(baseKey, subkeyName, valueName);
		}

		public void CreateSubKey(RegistryKey baseKey, string subkeyName)
		{
			this.registryWriter.CreateSubKey(baseKey, subkeyName);
		}

		public void DeleteSubKeyTree(RegistryKey baseKey, string subkeyName)
		{
			this.registryWriter.DeleteSubKeyTree(baseKey, subkeyName);
		}

		private static RegistryWriter instance = new RegistryWriter();

		private IRegistryWriter registryWriter = RegistryWriter.Instance;
	}
}
