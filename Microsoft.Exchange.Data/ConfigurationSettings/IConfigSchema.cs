using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ConfigurationSettings
{
	internal interface IConfigSchema : IDiagnosableObject
	{
		string Name { get; }

		string SectionName { get; }

		IEnumerable<string> Settings { get; }

		ExchangeConfigurationSection ScopeSchema { get; }

		object GetPropertyValue(string propertyName);

		object ParseAndValidateConfigValue(string settingName, string serializedValue, Type settingType = null);

		void ValidateScopeName(string scopeName);

		string ParseAndValidateScopeValue(string scopeName, object value);

		bool TryGetConfigurationProperty(string name, out ConfigurationProperty property);

		ConfigurationProperty GetConfigurationProperty(string name, Type type = null);

		object GetDefaultConfigValue(ConfigurationProperty property);

		bool CheckSettingExists(string name);

		void ValidateConfigValue(string settingName, object value);
	}
}
