using System;
using Microsoft.Win32;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public static class RegistryHelper
	{
		public static RegistryKey OpenKey(string rootKeyName, string subKeyName, bool isCreateKey, bool writable = false)
		{
			string text = rootKeyName;
			if (text == null)
			{
				text = string.Format("SOFTWARE\\Microsoft\\ExchangeServer\\{0}\\ActiveMonitoring\\Parameters", "v15");
			}
			if (!string.IsNullOrEmpty(subKeyName))
			{
				text = string.Format("{0}\\{1}", text, subKeyName);
			}
			RegistryKey result;
			if (isCreateKey)
			{
				result = Registry.LocalMachine.CreateSubKey(text);
			}
			else
			{
				result = Registry.LocalMachine.OpenSubKey(text, writable);
			}
			return result;
		}

		public static bool DeleteSubkeyRecursive(string rootKeyName = null, string subkeyName = null, bool isThrowOnError = false)
		{
			return RegistryHelper.HandleException<bool>(false, isThrowOnError, delegate
			{
				using (RegistryKey registryKey = RegistryHelper.OpenKey(rootKeyName, null, false, true))
				{
					if (registryKey != null)
					{
						registryKey.DeleteSubKeyTree(subkeyName, false);
						return true;
					}
				}
				return false;
			});
		}

		public static bool DeleteProperty(string propertyName, string subkeyName = null, string rootKeyName = null, bool isThrowOnError = false)
		{
			return RegistryHelper.HandleException<bool>(false, isThrowOnError, delegate
			{
				using (RegistryKey registryKey = RegistryHelper.OpenKey(rootKeyName, subkeyName, true, false))
				{
					if (registryKey != null)
					{
						registryKey.DeleteValue(propertyName);
						return true;
					}
				}
				return false;
			});
		}

		public static DateTime GetPropertyDateTime(string propertyName, DateTime defaultValue, string subkeyName = null, string rootKeyName = null, bool isThrowOnError = false)
		{
			string property = RegistryHelper.GetProperty<string>(propertyName, string.Empty, subkeyName, rootKeyName, isThrowOnError);
			DateTime result = defaultValue;
			if (!string.IsNullOrWhiteSpace(property))
			{
				result = DateTime.Parse(property);
			}
			return result;
		}

		public static void SetPropertyDateTime(string propertyName, DateTime propertyValue, string subkeyName = null, string rootKeyName = null, bool isThrowOnError = false)
		{
			string propertValue = propertyValue.ToString("o");
			RegistryHelper.SetProperty<string>(propertyName, propertValue, subkeyName, rootKeyName, isThrowOnError);
		}

		public static bool GetPropertyIntBool(string propertyName, bool defaultValue, string subkeyName = null, string rootKeyName = null, bool isThrowOnError = false)
		{
			int property = RegistryHelper.GetProperty<int>(propertyName, defaultValue ? 1 : 0, subkeyName, rootKeyName, isThrowOnError);
			return property > 0;
		}

		public static bool SetPropertyIntBool(string propertyName, bool propertyValue, string subkeyName = null, string rootKeyName = null, bool isThrowOnError = false)
		{
			return RegistryHelper.SetProperty<int>(propertyName, propertyValue ? 1 : 0, subkeyName, rootKeyName, isThrowOnError);
		}

		public static long GetPropertyAsLong(string propertyName, long defaultValue, string subkeyName = null, string rootKeyName = null, bool isThrowOnError = false)
		{
			long result = defaultValue;
			string property = RegistryHelper.GetProperty<string>(propertyName, null, subkeyName, rootKeyName, isThrowOnError);
			if (property != null && !long.TryParse(property, out result))
			{
				result = defaultValue;
			}
			return result;
		}

		public static string GetPropertyAsString(string propertyName, string subkeyName = null, string rootKeyName = null, bool isThrowOnError = false)
		{
			object property = RegistryHelper.GetProperty<object>(propertyName, null, subkeyName, rootKeyName, isThrowOnError);
			if (property != null)
			{
				return property.ToString();
			}
			return null;
		}

		public static T GetProperty<T>(string propertyName, T defaultValue, string subkeyName = null, string rootKeyName = null, bool isThrowOnError = false)
		{
			return RegistryHelper.HandleException<T>(defaultValue, isThrowOnError, delegate
			{
				T result = defaultValue;
				using (RegistryKey registryKey = RegistryHelper.OpenKey(rootKeyName, subkeyName, false, false))
				{
					if (registryKey != null)
					{
						result = (T)((object)registryKey.GetValue(propertyName, defaultValue));
					}
				}
				return result;
			});
		}

		public static bool SetProperty<T>(string propertyName, T propertValue, string subkeyName = null, string rootKeyName = null, bool isThrowOnError = false)
		{
			return RegistryHelper.HandleException<bool>(false, isThrowOnError, delegate
			{
				using (RegistryKey registryKey = RegistryHelper.OpenKey(rootKeyName, subkeyName, true, false))
				{
					if (registryKey != null)
					{
						registryKey.SetValue(propertyName, propertValue);
						return true;
					}
				}
				return false;
			});
		}

		internal static T HandleException<T>(T defaultValue, bool isThrowOnError, Func<T> action)
		{
			try
			{
				return action();
			}
			catch (Exception)
			{
				if (isThrowOnError)
				{
					throw;
				}
			}
			return defaultValue;
		}

		public static class WellKnownPropertyNames
		{
			public const string SystemStartTime = "SystemStartTime";

			public const string ActualBugCheckInitiatedTime = "ActualBugCheckInitiatedTime";
		}
	}
}
