using System;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class UserConfigurationXmlHelper
	{
		internal static ConfigurableObject Fill(ConfigurableObject configObject, ProviderPropertyDefinition property, UserConfigurationXmlHelper.GetXmlUserConfigurationDelegate getXmlUserConfigurationDelegate)
		{
			return UserConfigurationXmlHelper.Fill(configObject, property, (bool createIfNonexisting) => getXmlUserConfigurationDelegate(createIfNonexisting));
		}

		internal static ConfigurableObject Fill(ConfigurableObject configObject, ProviderPropertyDefinition property, UserConfigurationXmlHelper.GetReadableXmlUserConfigurationDelegate getXmlUserConfigurationDelegate)
		{
			Util.ThrowOnNullArgument(configObject, "configObject");
			Util.ThrowOnNullArgument(property, "property");
			using (IReadableUserConfiguration readableUserConfiguration = getXmlUserConfigurationDelegate(false))
			{
				if (readableUserConfiguration == null)
				{
					return null;
				}
				using (Stream xmlStream = readableUserConfiguration.GetXmlStream())
				{
					DataContractSerializer dataContractSerializer = new DataContractSerializer(property.Type);
					configObject[property] = dataContractSerializer.ReadObject(xmlStream);
				}
			}
			return configObject;
		}

		internal static void Save(ConfigurableObject configObject, ProviderPropertyDefinition property, UserConfigurationXmlHelper.GetXmlUserConfigurationDelegate getXmlUserConfigurationDelegate)
		{
			UserConfigurationXmlHelper.Save(configObject, SaveMode.NoConflictResolution, property, getXmlUserConfigurationDelegate);
		}

		internal static void Save(ConfigurableObject configObject, SaveMode saveMode, ProviderPropertyDefinition property, UserConfigurationXmlHelper.GetXmlUserConfigurationDelegate getXmlUserConfigurationDelegate)
		{
			Util.ThrowOnNullArgument(configObject, "configObject");
			Util.ThrowOnNullArgument(property, "property");
			bool flag = false;
			do
			{
				using (UserConfiguration userConfiguration = getXmlUserConfigurationDelegate(!flag))
				{
					using (Stream xmlStream = userConfiguration.GetXmlStream())
					{
						DataContractSerializer dataContractSerializer = new DataContractSerializer(property.Type);
						xmlStream.SetLength(0L);
						dataContractSerializer.WriteObject(xmlStream, configObject[property]);
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

		internal delegate UserConfiguration GetXmlUserConfigurationDelegate(bool createIfNonexisting);

		internal delegate IReadableUserConfiguration GetReadableXmlUserConfigurationDelegate(bool createIfNonexisting);
	}
}
