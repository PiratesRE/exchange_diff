using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal abstract class ConfigSchemaBase : ExchangeConfigurationSection, IConfigSchema, IDiagnosableObject
	{
		public ConfigSchemaBase()
		{
			this.defaultValueOverrides = new ConcurrentDictionary<string, object>();
		}

		public abstract string Name { get; }

		public virtual string SectionName
		{
			get
			{
				return "appSettings";
			}
		}

		string IDiagnosableObject.HashableIdentity
		{
			get
			{
				return this.Name;
			}
		}

		protected virtual ExchangeConfigurationSection ScopeSchema
		{
			get
			{
				return null;
			}
		}

		public static T GetDefaultConfig<T>(IConfigSchema schema, string settingName)
		{
			ConfigurationProperty configurationProperty = schema.GetConfigurationProperty(settingName, typeof(T));
			object defaultConfigValue = schema.GetDefaultConfigValue(configurationProperty);
			return ConfigSchemaBase.ConvertValue<T>(schema, settingName, defaultConfigValue);
		}

		public static T ConvertValue<T>(IConfigSchema schema, string settingName, object rawValue)
		{
			if (!(rawValue is T))
			{
				ConfigurationProperty property = schema.GetConfigurationProperty(settingName, typeof(T));
				ExchangeConfigurationSection.RunConfigOperation(delegate
				{
					TypeConverter converter = property.Converter;
					rawValue = converter.ConvertTo(rawValue, typeof(T));
				}, (Exception ex) => new ConfigurationSettingsPropertyBadTypeException(string.Format("{0}:{1}", settingName, (rawValue != null) ? rawValue.GetType().ToString() : "(null)"), typeof(T).ToString(), ex));
			}
			T result;
			try
			{
				result = (T)((object)rawValue);
			}
			catch (InvalidCastException innerException)
			{
				throw new ConfigurationSettingsPropertyBadTypeException(string.Format("{0}:{1}", settingName, (rawValue != null) ? rawValue.GetType().ToString() : "(null)"), typeof(T).ToString(), innerException);
			}
			return result;
		}

		public void ValidateConfigValue(string settingName, object value)
		{
			ConfigurationProperty configurationProperty = base.GetConfigurationProperty(settingName, null);
			this.ValidateConfigValue(configurationProperty, value);
		}

		public object ParseAndValidateConfigValue(string settingName, string serializedValue, Type settingType = null)
		{
			ConfigurationProperty property = base.GetConfigurationProperty(settingName, settingType);
			object convertedValue = null;
			ExchangeConfigurationSection.RunConfigOperation(delegate
			{
				TypeConverter converter = property.Converter;
				convertedValue = converter.ConvertFromInvariantString(serializedValue);
			}, (Exception ex) => new ConfigurationSettingsPropertyBadValueException(settingName, serializedValue, ex));
			this.ValidateConfigValue(property, convertedValue);
			return convertedValue;
		}

		public void ValidateScopeName(string scopeName)
		{
			if (string.IsNullOrEmpty(scopeName))
			{
				throw new ArgumentNullException("scopeName");
			}
			ConfigurationProperty configurationProperty;
			if (this.ScopeSchema == null || !this.ScopeSchema.TryGetConfigurationProperty(scopeName, out configurationProperty))
			{
				List<string> list = new List<string>(this.ScopeSchema.Settings);
				list.Sort();
				throw new ConfigurationSettingsScopePropertyNotFoundException(scopeName, string.Join(", ", list));
			}
		}

		public string ParseAndValidateScopeValue(string scopeName, object value)
		{
			this.ValidateScopeName(scopeName);
			ConfigurationProperty property = this.ScopeSchema.GetConfigurationProperty(scopeName, null);
			ExchangeConfigurationSection.RunConfigOperation(delegate
			{
				if (property.Type != value.GetType())
				{
					property.Converter.ConvertFrom(value);
				}
				property.Validator.Validate(value);
			}, (Exception ex) => new ConfigurationSettingsScopePropertyFailedValidationException(scopeName, (value != null) ? value.ToString() : null, ex));
			if (value == null)
			{
				return null;
			}
			if (value is string)
			{
				return (string)value;
			}
			string result;
			try
			{
				result = property.Converter.ConvertToInvariantString(value);
			}
			catch (NotSupportedException innerException)
			{
				throw new ConfigurationSettingsScopePropertyBadValueException(scopeName, (value != null) ? value.ToString() : null, innerException);
			}
			return result;
		}

		public void SetDefaultConfigValue<T>(string settingName, T value)
		{
			this.SetDefaultConfigValue<T>(base.GetConfigurationProperty(settingName, typeof(T)), value);
		}

		public void SetDefaultConfigValue<T>(ConfigurationProperty property, T value)
		{
			this.defaultValueOverrides[property.Name] = value;
		}

		public XElement GetDiagnosticInfo(string argument)
		{
			XElement xelement = new XElement(this.SectionName ?? "Application");
			foreach (KeyValuePair<string, ConfigurationProperty> keyValuePair in base.Definitions)
			{
				object defaultConfigValue = ((IConfigSchema)this).GetDefaultConfigValue(keyValuePair.Value);
				xelement.Add(new XElement(keyValuePair.Key, new object[]
				{
					new XAttribute("value", base[keyValuePair.Key] ?? string.Empty),
					new XAttribute("default", (defaultConfigValue != null) ? defaultConfigValue.ToString() : string.Empty),
					new XAttribute("type", keyValuePair.Value.Type.Name)
				}));
			}
			return xelement;
		}

		object IConfigSchema.GetDefaultConfigValue(ConfigurationProperty property)
		{
			object result;
			if (this.defaultValueOverrides.TryGetValue(property.Name, out result))
			{
				return result;
			}
			return property.DefaultValue;
		}

		ExchangeConfigurationSection IConfigSchema.ScopeSchema
		{
			get
			{
				return this.ScopeSchema;
			}
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			return true;
		}

		private void ValidateConfigValue(ConfigurationProperty property, object value)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			ExchangeConfigurationSection.RunConfigOperation(delegate
			{
				ConfigurationValidatorBase validator = property.Validator;
				if (validator != null)
				{
					validator.Validate(value);
				}
			}, (Exception ex) => new ConfigurationSettingsPropertyFailedValidationException(property.Name, (value != null) ? value.ToString() : null, ex));
		}

		private readonly IDictionary<string, object> defaultValueOverrides;
	}
}
