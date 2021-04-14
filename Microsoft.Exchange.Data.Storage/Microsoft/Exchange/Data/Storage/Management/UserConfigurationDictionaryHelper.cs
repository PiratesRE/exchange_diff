using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class UserConfigurationDictionaryHelper
	{
		public static void Fill(UserConfiguration userConfiguration, ConfigurableObject configurableObject, IEnumerable<ProviderPropertyDefinition> appliedProperties)
		{
			IDictionary dictionary = userConfiguration.GetDictionary();
			foreach (ProviderPropertyDefinition providerPropertyDefinition in appliedProperties)
			{
				if (dictionary.Contains(providerPropertyDefinition.Name))
				{
					configurableObject.propertyBag[providerPropertyDefinition] = StoreValueConverter.ConvertValueFromStore(providerPropertyDefinition, dictionary[providerPropertyDefinition.Name]);
				}
			}
		}

		internal static ConfigurableObject Fill(ConfigurableObject configObject, ProviderPropertyDefinition[] appliedProperties, UserConfigurationDictionaryHelper.GetDictionaryUserConfigurationDelegate getDictionaryUserConfigurationDelegate)
		{
			Util.ThrowOnNullArgument(configObject, "configObject");
			Util.ThrowOnNullArgument(appliedProperties, "appliedProperties");
			using (UserConfiguration userConfiguration = getDictionaryUserConfigurationDelegate(false))
			{
				if (userConfiguration == null)
				{
					return null;
				}
				UserConfigurationDictionaryHelper.Fill(userConfiguration, configObject, appliedProperties);
			}
			return configObject;
		}

		internal static void Save(ConfigurableObject configObject, ProviderPropertyDefinition[] appliedProperties, UserConfigurationDictionaryHelper.GetDictionaryUserConfigurationDelegate getDictionaryUserConfigurationDelegate)
		{
			UserConfigurationDictionaryHelper.Save(configObject, SaveMode.NoConflictResolution, appliedProperties, getDictionaryUserConfigurationDelegate);
		}

		internal static void Save(ConfigurableObject configObject, SaveMode saveMode, ProviderPropertyDefinition[] appliedProperties, UserConfigurationDictionaryHelper.GetDictionaryUserConfigurationDelegate getDictionaryUserConfigurationDelegate)
		{
			Util.ThrowOnNullArgument(configObject, "configObject");
			Util.ThrowOnNullArgument(appliedProperties, "appliedProperties");
			bool flag = false;
			do
			{
				using (UserConfiguration userConfiguration = getDictionaryUserConfigurationDelegate(!flag))
				{
					IDictionary dictionary = userConfiguration.GetDictionary();
					foreach (ProviderPropertyDefinition providerPropertyDefinition in appliedProperties)
					{
						if (configObject.IsModified(providerPropertyDefinition))
						{
							object obj = configObject[providerPropertyDefinition];
							if (obj == null)
							{
								dictionary.Remove(providerPropertyDefinition.Name);
							}
							else
							{
								dictionary[providerPropertyDefinition.Name] = StoreValueConverter.ConvertValueToStore(obj);
							}
						}
					}
					try
					{
						userConfiguration.Save(saveMode);
						break;
					}
					catch (ObjectExistedException)
					{
						if (flag)
						{
							throw;
						}
						flag = true;
					}
				}
			}
			while (flag);
		}

		internal delegate UserConfiguration GetDictionaryUserConfigurationDelegate(bool createIfNonexisting);
	}
}
