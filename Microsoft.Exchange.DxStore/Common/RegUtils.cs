using System;
using Microsoft.Win32;

namespace Microsoft.Exchange.DxStore.Common
{
	public class RegUtils
	{
		internal static T GetProperty<T>(RegistryKey key, string propertyName, T defaultValue)
		{
			T result = defaultValue;
			if (key != null)
			{
				object value = key.GetValue(propertyName);
				if (value != null && value is T)
				{
					result = (T)((object)value);
				}
			}
			return result;
		}

		internal static TimeSpan GetLongPropertyAsTimeSpan(RegistryKey key, string propertyName, TimeSpan defaultValue)
		{
			TimeSpan result = defaultValue;
			if (key != null)
			{
				object value = key.GetValue(propertyName);
				if (value != null)
				{
					if (value is int)
					{
						result = TimeSpan.FromMilliseconds((double)((int)value));
					}
					else if (value is long)
					{
						result = TimeSpan.FromMilliseconds((double)((long)value));
					}
				}
			}
			return result;
		}

		internal static DateTimeOffset GetTimeProperty(RegistryKey key, string propertyName)
		{
			return RegUtils.GetTimeProperty(key, propertyName, DateTimeOffset.MinValue);
		}

		internal static DateTimeOffset GetTimeProperty(RegistryKey key, string propertyName, DateTimeOffset defaultValue)
		{
			string property = RegUtils.GetProperty<string>(key, propertyName, string.Empty);
			DateTimeOffset result;
			if (!DateTimeOffset.TryParse(property, out result))
			{
				result = defaultValue;
			}
			return result;
		}

		internal static WcfTimeout GetWcfTimeoutProperty(RegistryKey key, string propertyName, WcfTimeout defaultTimeout)
		{
			string property = RegUtils.GetProperty<string>(key, propertyName, string.Empty);
			return WcfTimeout.Parse(property, defaultTimeout);
		}

		internal static bool GetBoolProperty(RegistryKey key, string propertyName, bool defaultValue = false)
		{
			bool result = defaultValue;
			object property = RegUtils.GetProperty<object>(key, propertyName, null);
			if (property is int)
			{
				result = ((int)property > 0);
			}
			else if (property is string && !bool.TryParse(property as string, out result))
			{
				result = defaultValue;
			}
			return result;
		}

		internal static void SetProperty<T>(RegistryKey key, string propertyName, T value)
		{
			if (value is bool)
			{
				key.SetValue(propertyName, ((bool)((object)value)) ? 1 : 0);
				return;
			}
			key.SetValue(propertyName, value);
		}
	}
}
