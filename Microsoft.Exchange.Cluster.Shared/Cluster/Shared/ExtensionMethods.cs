using System;
using System.Collections.Generic;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Shared
{
	public static class ExtensionMethods
	{
		public static IDistributedStoreKey OpenOrCreateKey(this IDistributedStoreKey key, string keyName, bool isIgnoreIfNotExist = false, ReadWriteConstraints constraints = null)
		{
			return key.OpenKey(keyName, DxStoreKeyAccessMode.CreateIfNotExist, isIgnoreIfNotExist, constraints);
		}

		public static bool SetValue<T>(this IDistributedStoreKey key, string propertyName, T propertyValue, bool isBestEffort = false, ReadWriteConstraints constraints = null)
		{
			RegistryValueKind valueKind = Utils.GetValueKind(propertyValue);
			return key.SetValue(propertyName, propertyValue, valueKind, isBestEffort, constraints);
		}

		public static T GetValue<T>(this IDistributedStoreKey key, string propertyName, T defaultValue = default(T), ReadWriteConstraints constraints = null)
		{
			bool flag;
			return key.GetValue(propertyName, defaultValue, out flag, constraints);
		}

		public static T GetValue<T>(this IDistributedStoreKey key, string propertyName, T defaultValue, out bool isValueExist, ReadWriteConstraints constriants = null)
		{
			RegistryValueKind registryValueKind;
			object value = key.GetValue(propertyName, out isValueExist, out registryValueKind, constriants);
			if (isValueExist)
			{
				return (T)((object)value);
			}
			return defaultValue;
		}

		public static IEnumerable<string> GetSubkeyNames(this IDistributedStoreKey parentKey, string subkeyName, ReadWriteConstraints constraints)
		{
			using (IDistributedStoreKey distributedStoreKey = parentKey.OpenKey(subkeyName, DxStoreKeyAccessMode.Read, true, constraints))
			{
				if (distributedStoreKey != null)
				{
					return distributedStoreKey.GetSubkeyNames(constraints);
				}
			}
			return null;
		}
	}
}
