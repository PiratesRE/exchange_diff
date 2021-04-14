using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.Net
{
	public class RegistryReader : IRegistryReader
	{
		internal RegistryReader()
		{
		}

		public static IRegistryReader Instance
		{
			get
			{
				return RegistryReader.hookableInstance.Value;
			}
		}

		public T GetValue<T>(RegistryKey baseKey, string subkeyName, string valueName, T defaultValue)
		{
			if (baseKey == null)
			{
				throw new ArgumentNullException("baseKey");
			}
			if (string.IsNullOrEmpty(valueName))
			{
				throw new ArgumentNullException("valueName");
			}
			if (string.IsNullOrEmpty(subkeyName))
			{
				return this.GetValue<T>(baseKey, valueName, defaultValue);
			}
			using (RegistryKey registryKey = baseKey.OpenSubKey(subkeyName, false))
			{
				if (registryKey != null)
				{
					return this.GetValue<T>(registryKey, valueName, defaultValue);
				}
			}
			return defaultValue;
		}

		public string[] GetSubKeyNames(RegistryKey baseKey, string subkeyName)
		{
			if (baseKey == null)
			{
				throw new ArgumentNullException("baseKey");
			}
			using (RegistryKey registryKey = baseKey.OpenSubKey(subkeyName, false))
			{
				if (registryKey != null)
				{
					return registryKey.GetSubKeyNames();
				}
			}
			return Array<string>.Empty;
		}

		public bool DoesValueExist(RegistryKey baseKey, string subkeyName, string valueName)
		{
			if (baseKey == null)
			{
				throw new ArgumentNullException("baseKey");
			}
			bool result;
			using (RegistryKey registryKey = baseKey.OpenSubKey(subkeyName, false))
			{
				result = (registryKey != null && registryKey.GetValue(valueName) != null);
			}
			return result;
		}

		internal static IDisposable SetTestHook(IRegistryReader testHook)
		{
			return RegistryReader.hookableInstance.SetTestHook(testHook);
		}

		internal static T GetValueOrDefault<T>(object value, T defaultValue)
		{
			if (value is T)
			{
				return (T)((object)value);
			}
			if (defaultValue is IConvertible)
			{
				try
				{
					IConvertible convertible = (IConvertible)((object)defaultValue);
					switch (convertible.GetTypeCode())
					{
					case TypeCode.Boolean:
						value = Convert.ToBoolean(value);
						break;
					case TypeCode.Int16:
						value = Convert.ToInt16(value);
						break;
					case TypeCode.UInt16:
						value = Convert.ToUInt16(value);
						break;
					case TypeCode.Int32:
						value = Convert.ToInt32(value);
						break;
					case TypeCode.UInt32:
						value = Convert.ToUInt32(value);
						break;
					case TypeCode.Int64:
						value = Convert.ToInt64(value);
						break;
					case TypeCode.UInt64:
						value = Convert.ToUInt64(value);
						break;
					}
				}
				catch (FormatException)
				{
					return defaultValue;
				}
				if (value is T)
				{
					return (T)((object)value);
				}
				return defaultValue;
			}
			return defaultValue;
		}

		private T GetValue<T>(RegistryKey key, string valueName, T defaultValue)
		{
			object value = key.GetValue(valueName);
			if (value == null)
			{
				return defaultValue;
			}
			return RegistryReader.GetValueOrDefault<T>(value, defaultValue);
		}

		private static Hookable<IRegistryReader> hookableInstance = Hookable<IRegistryReader>.Create(false, new RegistryReader());
	}
}
