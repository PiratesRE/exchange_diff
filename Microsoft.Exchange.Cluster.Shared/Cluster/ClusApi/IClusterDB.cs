using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	public interface IClusterDB : IDisposable
	{
		bool IsInstalled { get; }

		bool IsInitialized { get; }

		IEnumerable<string> GetSubKeyNames(string registryKey);

		IEnumerable<Tuple<string, RegistryValueKind>> GetValueInfos(string registryKey);

		T GetValue<T>(string keyName, string valueName, T defaultValue);

		void SetValue<T>(string keyName, string propertyName, T propetyValue);

		void DeleteValue(string keyName, string propertyName);

		IClusterDBWriteBatch CreateWriteBatch(string registryKey);
	}
}
