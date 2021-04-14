using System;
using Microsoft.Win32;

namespace Microsoft.Exchange.Servicelets.RPCHTTP
{
	internal class RegistryUtilities
	{
		internal static RegistryUtilities.RegistryValueAction UpdateRegValue(string keyName, string valueName, object newValue, out object oldValue)
		{
			RegistryUtilities.RegistryValueAction registryValueAction = RegistryUtilities.RegistryValueAction.None;
			oldValue = null;
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(keyName))
			{
				oldValue = registryKey.GetValue(valueName, null);
				if ((oldValue == null && newValue != null) || (object.Equals(oldValue, string.Empty) && !object.Equals(newValue, string.Empty)))
				{
					registryValueAction = RegistryUtilities.RegistryValueAction.Enabled;
				}
				else if ((oldValue != null && newValue == null) || (!object.Equals(oldValue, string.Empty) && object.Equals(newValue, string.Empty)))
				{
					registryValueAction = RegistryUtilities.RegistryValueAction.Disabled;
				}
				else if (!object.Equals(oldValue, newValue))
				{
					registryValueAction = RegistryUtilities.RegistryValueAction.Updated;
				}
				if (registryValueAction != RegistryUtilities.RegistryValueAction.None)
				{
					if (newValue != null)
					{
						registryKey.SetValue(valueName, newValue);
					}
					else
					{
						registryKey.DeleteValue(valueName, false);
					}
				}
			}
			return registryValueAction;
		}

		internal static void SetRegkeyDWord(string keyName, string valueName, int newValue)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(keyName))
			{
				object value = registryKey.GetValue(valueName, null);
				if (value is int)
				{
					int num = (int)value;
					if (num != newValue)
					{
						registryKey.SetValue(valueName, newValue, RegistryValueKind.DWord);
					}
				}
				else
				{
					registryKey.SetValue(valueName, newValue, RegistryValueKind.DWord);
				}
			}
		}

		internal static void SetRegkeyString(string keyName, string valueName, string newValue)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(keyName))
			{
				object value = registryKey.GetValue(valueName, string.Empty);
				if (value is string)
				{
					string a = (string)value;
					if (!string.Equals(a, newValue))
					{
						registryKey.SetValue(valueName, newValue, RegistryValueKind.String);
					}
				}
				else
				{
					registryKey.SetValue(valueName, newValue, RegistryValueKind.String);
				}
			}
		}

		internal static void RemoveRegkey(string keyName)
		{
			RegistryKey registryKey = null;
			try
			{
				registryKey = Registry.LocalMachine.OpenSubKey(keyName, false);
				if (registryKey != null)
				{
					try
					{
						Registry.LocalMachine.DeleteSubKeyTree(keyName);
					}
					catch (ArgumentException)
					{
					}
				}
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
		}

		internal static void RemoveRegValue(string keyName, string valueName)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(keyName, true))
			{
				if (registryKey != null)
				{
					registryKey.DeleteValue(valueName, false);
				}
			}
		}

		internal static T GetRegistryValueOrUseDefault<T>(string registryKeyName, string registryValueName, RegistryValueKind expectedValueKind, T defaultValue)
		{
			T registryValueOrUseDefault;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(registryKeyName, false))
			{
				registryValueOrUseDefault = RegistryUtilities.GetRegistryValueOrUseDefault<T>(registryKey, registryValueName, expectedValueKind, defaultValue);
			}
			return registryValueOrUseDefault;
		}

		internal static T GetRegistryValueOrUseDefault<T>(RegistryKey registryKey, string registryValueName, RegistryValueKind expectedValueKind, T defaultValue)
		{
			T result;
			if (registryKey != null && RegistryUtilities.TryGetRegistryValue<T>(registryKey, registryValueName, expectedValueKind, out result))
			{
				return result;
			}
			return defaultValue;
		}

		internal static bool TryGetRegistryValue<T>(RegistryKey registryKey, string registryValueName, RegistryValueKind expectedValueKind, out T result)
		{
			result = default(T);
			object value = registryKey.GetValue(registryValueName);
			if (value != null)
			{
				RegistryValueKind valueKind = registryKey.GetValueKind(registryValueName);
				if (valueKind == expectedValueKind)
				{
					result = (T)((object)value);
					return true;
				}
			}
			return false;
		}

		internal enum RegistryValueAction
		{
			None,
			Enabled,
			Disabled,
			Updated
		}
	}
}
